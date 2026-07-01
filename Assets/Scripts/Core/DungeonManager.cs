using System;
using System.Collections;
using UnityEngine;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;
using CanavarZindanlari.Equipment;
using CanavarZindanlari.UI;

namespace CanavarZindanlari.Core
{
    public enum DungeonState
    {
        WaitingForClass,  // Sınıf seçim ekranı açık
        MapView,          // Zindan haritası
        InWaveCombat,     // Savaş aktif
        WaveTransition,   // Dalga arası bekleme
        FloorCleared,     // Kat temizlendi
        FloorFailed,      // Kat başarısız
    }

    public enum FloorState { Locked, Unlocked, Cleared }

    public enum FloorType { Normal, Champion, Boss, MainBoss }

    [Serializable]
    public struct FloorStatus
    {
        public int        Floor;
        public FloorState State;
        public bool       FirstCleared;
    }

    /// <summary>
    /// Zindan Keşif Sistemi orkestratörü.
    /// GDD: design/gdd/zindan-kesif.md
    /// </summary>
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private CombatManager _combat;

        // ── Sabitler ─────────────────────────────────────────────────────────

        public const int TotalFloors    = 20;
        public const int EnergyMax      = 100;
        public const int EnergyPerFloor = 2;

        // ── Public state ──────────────────────────────────────────────────────

        public DungeonState  State        { get; private set; } = DungeonState.WaitingForClass;
        public int           CurrentFloor { get; private set; }
        public int           CurrentWave  { get; private set; }
        public int           TotalWaves   { get; private set; }
        public int           Energy       { get; private set; } = EnergyMax;
        public ClassData     PlayerClass  { get; private set; }
        public FloorStatus[] Floors       { get; private set; } = new FloorStatus[TotalFloors + 1];

        // Son temizlenen katın first-clear bilgisi (HUD için)
        public bool LastClearWasFirstTime { get; private set; }

        public event Action OnStateChanged;

        // ── Internal ──────────────────────────────────────────────────────────

        private CombatUnit        _persistedPlayer;
        private BattleHUD         _hud;
        private MonsterCollection _collection;
        private bool              _keepAutoBattle; // kat/dalga geçişlerinde oto durumu korunur

        // Son kat sonuçları (HUD için)
        public CapturedMonster LastCaptured        { get; private set; }
        public OwnedEquipment  LastEquipmentDropped { get; private set; }
        public int             LastGoldEarned       { get; private set; }
        public int             LastGemsEarned       { get; private set; }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        private const string KeyClass = "player_class_name";

        private void Awake()
        {
            for (int i = 1; i <= TotalFloors; i++)
                Floors[i] = new FloorStatus
                {
                    Floor        = i,
                    State        = i == 1 ? FloorState.Unlocked : FloorState.Locked,
                    FirstCleared = false,
                };

            // Kayıtlı sınıfı yükle
            string saved = PlayerPrefs.GetString(KeyClass, "");
            if (!string.IsNullOrEmpty(saved))
            {
                var data = Resources.Load<ClassData>("Classes/" + saved);
                if (data != null) { PlayerClass = data; State = DungeonState.MapView; }
            }

            LoadProgress();
            _hud        = UnityEngine.Object.FindFirstObjectByType<BattleHUD>();
            _collection = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();
        }

        /// <summary>Sınıf seçim ekranından çağrılır; sınıfı kaydeder ve haritayı açar.</summary>
        public void SetClass(ClassData data)
        {
            PlayerClass = data;
            PlayerPrefs.SetString(KeyClass, data.name);
            PlayerPrefs.Save();
            State = DungeonState.MapView;
            OnStateChanged?.Invoke();
        }

        private void OnEnable()
        {
            _combat.OnBattleEnded += HandleBattleEnded;

            var selector = UnityEngine.Object.FindFirstObjectByType<ClassSelectionScreen>();
            if (selector != null)
                selector.OnClassSelected += HandleClassSelected;
        }

        private void OnDisable()
        {
            _combat.OnBattleEnded -= HandleBattleEnded;
        }

        // ── Sınıf seçimi ──────────────────────────────────────────────────────

        private void HandleClassSelected(ClassData classData)
        {
            PlayerClass = classData;
            State       = DungeonState.MapView;
            OnStateChanged?.Invoke();
        }

        // ── Kat girişi ────────────────────────────────────────────────────────

        public bool CanEnterFloor(int floor)
        {
            if (floor < 1 || floor > TotalFloors) return false;
            return Floors[floor].State != FloorState.Locked
                && Energy >= EnergyPerFloor
                && State  == DungeonState.MapView
                && PlayerClass != null;
        }

        public void EnterFloor(int floor)
        {
            if (!CanEnterFloor(floor)) return;

            CurrentFloor = floor;
            CurrentWave  = 1;
            TotalWaves   = IsBossFloor(floor) ? 3 : 2;

            _persistedPlayer = BuildPlayer();
            StartWave();
        }

        private void StartWave()
        {
            _hud?.ResetForNewFight();
            var enemy = BuildEnemy(CurrentFloor, CurrentWave);
            State = DungeonState.InWaveCombat;
            OnStateChanged?.Invoke();
            _combat.StartCombat(_persistedPlayer, enemy, PlayerClass);
            if (_keepAutoBattle) _combat.SetAutoBattle(true);
        }

        // ── Savaş sonucu ──────────────────────────────────────────────────────

        private void HandleBattleEnded(BattleReward reward)
        {
            if (State != DungeonState.InWaveCombat) return;

            _keepAutoBattle = _combat.AutoBattle; // oto durumunu bir sonraki dalga/kata taşı

            if (reward.IsVictory)
            {
                if (CurrentWave < TotalWaves)
                {
                    CurrentWave++;
                    State = DungeonState.WaveTransition;
                    OnStateChanged?.Invoke();
                    StartCoroutine(WaveTransitionDelay());
                }
                else
                {
                    HandleFloorCleared();
                }
            }
            else
            {
                HandleFloorFailed();
            }
        }

        private IEnumerator WaveTransitionDelay()
        {
            yield return new WaitForSeconds(1.5f);
            StartWave();
        }

        private void HandleFloorCleared()
        {
            Energy = Mathf.Max(0, Energy - EnergyPerFloor);

            LastClearWasFirstTime = !Floors[CurrentFloor].FirstCleared;
            var status = Floors[CurrentFloor];
            status.State        = FloorState.Cleared;
            status.FirstCleared = true;
            Floors[CurrentFloor] = status;

            // Sonraki katı aç
            if (CurrentFloor < TotalFloors)
            {
                var next = Floors[CurrentFloor + 1];
                if (next.State == FloorState.Locked)
                {
                    next.State = FloorState.Unlocked;
                    Floors[CurrentFloor + 1] = next;
                }
            }

            // GDD Kural 10 adım 7: kat sonrası tam iyileşme
            if (_persistedPlayer != null)
                _persistedPlayer.CurrentHP = _persistedPlayer.MaxHP;

            SaveProgress();

            _combat.ResetToIdle();

            // Altın ve elmas ödülü
            LastGoldEarned = EconomyManager.FloorGoldReward(CurrentFloor);
            LastGemsEarned = LastClearWasFirstTime ? GetFirstClearGems(CurrentFloor) : 0;
            EconomyManager.Instance?.AddGold(LastGoldEarned);
            if (LastGemsEarned > 0) EconomyManager.Instance?.AddDiamonds(LastGemsEarned);

            // Canavar düşme — GDD canavar-toplama-evrim.md Kural 1
            LastCaptured = _collection != null ? _collection.TryCapture(CurrentFloor) : null;

            // Ekipman düşme — pet oranının 1/3'ü
            LastEquipmentDropped = TryDropEquipment(CurrentFloor);
            if (LastEquipmentDropped != null)
                EquipmentManager.Instance.AddEquipment(LastEquipmentDropped);

            State = DungeonState.FloorCleared;
            OnStateChanged?.Invoke();
        }

        private void HandleFloorFailed()
        {
            _combat.ResetToIdle();
            State = DungeonState.FloorFailed;
            OnStateChanged?.Invoke();
        }

        public void ReturnToMap()
        {
            _combat.ResetToIdle();
            State = DungeonState.MapView;
            OnStateChanged?.Invoke();
        }

        // ── Ekipman düşme ─────────────────────────────────────────────────────

        private static OwnedEquipment TryDropEquipment(int floor)
        {
            float chance = MonsterCollection.CaptureChance(floor) * 3f;
            if (UnityEngine.Random.value > chance) return null;

            Rarity tier = MonsterCollection.TierForFloor(floor);

            var slot = UnityEngine.Random.value < 0.5f
                ? EquipmentManager.RandomArmorSlot()
                : EquipmentManager.RandomAccessorySlot();

            return EquipmentManager.CreateEquipment(slot, tier);
        }

        // ── Yardımcılar ───────────────────────────────────────────────────────

        // Dalga sayısı için: her 5. kat 3 dalga (şampiyon + boss + ana boss)
        public static bool IsBossFloor(int floor) => floor % 5 == 0;

        // Yakalama tier ve görsel etiket için kat türü
        public static FloorType GetFloorType(int floor)
        {
            if (floor == TotalFloors)    return FloorType.MainBoss;
            if (floor % 10 == 0)         return FloorType.Boss;
            if (floor % 5  == 0)         return FloorType.Champion;
            return FloorType.Normal;
        }

        public int GetFirstClearGems(int floor) => GetFloorType(floor) switch
        {
            FloorType.MainBoss  => 200,
            FloorType.Boss      => 100,
            FloorType.Champion  => 50,
            _                   => 5,
        };

        // ── Oyuncu oluşturma ──────────────────────────────────────────────────

        private CombatUnit BuildPlayer()
        {
            if (PlayerClass != null)
            {
                int baseHP  = PlayerClass.StatAtLevel(PlayerClass.BaseHP,  1);
                int baseATK = PlayerClass.StatAtLevel(PlayerClass.BaseATK, 1);

                // Seçili pet stat bonusu
                var pet = _collection?.SelectedPet;
                if (pet != null)
                {
                    var (hpMult, atkMult) = MonsterCollection.BonusForTier(pet.Tier);
                    baseHP  = Mathf.RoundToInt(baseHP  * hpMult);
                    baseATK = Mathf.RoundToInt(baseATK * atkMult);
                }

                int baseDEF = PlayerClass.StatAtLevel(PlayerClass.BaseDEF, 1);
                int baseSPD = PlayerClass.StatAtLevel(PlayerClass.BaseSPD, 1);

                // Ekipman bonusu
                var (eqATK, eqHP, eqDEF, eqSPD) = EquipmentManager.Instance.GetTotalBonuses();
                baseHP  += eqHP;
                baseATK += eqATK;
                baseDEF += eqDEF;
                baseSPD += eqSPD;

                return new CombatUnit
                {
                    DisplayName = PlayerClass.ClassName,
                    Archetype   = Archetype.Saldirgan,
                    MaxHP       = baseHP,
                    CurrentHP   = baseHP,
                    BaseATK     = baseATK,
                    BaseDEF     = baseDEF,
                    SPD         = baseSPD,
                };
            }

            return new CombatUnit
            {
                DisplayName = "Oyuncu", Archetype = Archetype.Saldirgan,
                MaxHP = 55, CurrentHP = 55, BaseATK = 18, BaseDEF = 40, SPD = 20,
            };
        }

        // ── Düşman oluşturma (GDD Formül 1 — formula bazlı) ──────────────────

        private CombatUnit BuildEnemy(int floor, int wave)
        {
            bool isBossWave = IsBossFloor(floor) && wave == TotalWaves;
            int  level      = floor;

            // Önce Resources/Monsters/ havuzundan yüklemeyi dene
            var pool = Resources.LoadAll<MonsterData>("Monsters");
            if (pool.Length > 0)
                return BuildFromPool(pool, floor, level, isBossWave);

            return BuildFormulaEnemy(floor, isBossWave);
        }

        private static CombatUnit BuildFromPool(MonsterData[] pool, int floor, int level, bool isBoss)
        {
            int idx = isBoss
                ? pool.Length - 1
                : Mathf.Min(Mathf.FloorToInt((float)(floor - 1) / TotalFloors * pool.Length), pool.Length - 1);

            var m    = pool[idx];
            float bm = isBoss ? 2.5f : 1f;
            int   bl = isBoss ? Mathf.FloorToInt(level * 0.5f) : 0;
            int   hp = Mathf.FloorToInt(m.StatAtLevel(m.BaseHP, level) * bm);

            return new CombatUnit
            {
                DisplayName = isBoss ? $"[BOSS] {m.DisplayName}" : m.DisplayName,
                Archetype   = m.Archetype,
                MaxHP       = hp, CurrentHP = hp,
                BaseATK     = m.StatAtLevel(m.BaseATK, level + bl),
                BaseDEF     = m.StatAtLevel(m.BaseDEF, level + bl),
                SPD         = m.StatAtLevel(m.BaseSPD, level),
            };
        }

        // GDD Formül 1 bazlı — bölge havuzu yokken kullanılır
        private static CombatUnit BuildFormulaEnemy(int floor, bool isBoss)
        {
            float mult = isBoss ? 2.5f : 1f;
            int   hp   = Mathf.FloorToInt((20 + floor *  8) * mult);
            int   atk  = Mathf.FloorToInt((10 + floor *  3) * mult);
            int   def  = Mathf.FloorToInt(( 8 + floor *  2) * mult);

            return new CombatUnit
            {
                DisplayName = isBoss ? $"Kat {floor} Boss" : $"Kat {floor} Düşman",
                Archetype   = Archetype.Saldirgan,
                MaxHP       = hp, CurrentHP = hp,
                BaseATK     = atk, BaseDEF = def, SPD = 20,
            };
        }

        // ── Kaydet / Yükle ────────────────────────────────────────────────────

        private void SaveProgress()
        {
            PlayerPrefs.SetInt("dungeon_energy", Energy);
            for (int i = 1; i <= TotalFloors; i++)
            {
                PlayerPrefs.SetInt($"dungeon_floor_{i}",       (int)Floors[i].State);
                PlayerPrefs.SetInt($"dungeon_first_{i}",       Floors[i].FirstCleared ? 1 : 0);
            }
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            Energy = PlayerPrefs.GetInt("dungeon_energy", EnergyMax);
            for (int i = 1; i <= TotalFloors; i++)
            {
                int defaultState = i == 1 ? (int)FloorState.Unlocked : (int)FloorState.Locked;
                Floors[i] = new FloorStatus
                {
                    Floor        = i,
                    State        = (FloorState)PlayerPrefs.GetInt($"dungeon_floor_{i}", defaultState),
                    FirstCleared = PlayerPrefs.GetInt($"dungeon_first_{i}", 0) == 1,
                };
            }
        }

        /// <summary>Test için ilerlemeyi sıfırla.</summary>
        public void ResetProgress()
        {
            for (int i = 1; i <= TotalFloors; i++)
            {
                PlayerPrefs.DeleteKey($"dungeon_floor_{i}");
                PlayerPrefs.DeleteKey($"dungeon_first_{i}");
            }
            PlayerPrefs.DeleteKey("dungeon_energy");
            PlayerPrefs.Save();

            Energy = EnergyMax;
            for (int i = 1; i <= TotalFloors; i++)
                Floors[i] = new FloorStatus
                {
                    Floor        = i,
                    State        = i == 1 ? FloorState.Unlocked : FloorState.Locked,
                    FirstCleared = false,
                };

            State = DungeonState.MapView;
            OnStateChanged?.Invoke();
        }
    }
}
