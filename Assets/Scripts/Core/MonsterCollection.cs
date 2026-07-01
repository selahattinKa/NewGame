using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;

namespace CanavarZindanlari.Core
{
    [Serializable]
    public class CapturedMonster
    {
        public string InstanceId;
        public string DisplayName;
        public Rarity Tier;
        public Rarity MaxEvolutionTier;
        public int    FloorCaptured;
        public string CaptureDate;
    }

    public struct EvoCost { public int FodderCount; public int Gold; }

    /// <summary>
    /// Oyuncunun yakalanan canavar koleksiyonunu yönetir.
    /// GDD: design/gdd/canavar-toplama-evrim.md
    ///
    /// Tier sistemi — kat tipine göre sabit (rastgele değil):
    ///   Normal     → F
    ///   Şampiyon   → D  (kat 5, 15)
    ///   Boss       → C  (kat 10)
    ///   Ana Boss   → B  (kat 20)
    ///
    /// Yakalama şansı — yüzdeler placeholder, ileride tartışılacak:
    ///   Normal: %15 | Şampiyon: %8 | Boss: %4 | Ana Boss: %2
    /// </summary>
    public class MonsterCollection : MonoBehaviour
    {
        [Serializable]
        private class SaveData { public List<CapturedMonster> Monsters = new List<CapturedMonster>(); }

        private const string PrefKey    = "monster_collection";
        private const string PrefPetKey = "selected_pet";

        public IReadOnlyList<CapturedMonster> Monsters => _monsters;
        private List<CapturedMonster> _monsters = new List<CapturedMonster>();

        public string          SelectedPetId { get; private set; }
        public CapturedMonster SelectedPet   => _monsters.Find(m => m.InstanceId == SelectedPetId);

        public event Action<CapturedMonster> OnMonsterAdded;
        public event Action<CapturedMonster> OnPetSelected;

        private void Awake() => Load();

        // ── Yakalama şansı (placeholder — ileride tuning) ─────────────────────

        public static float CaptureChance(int floor)
        {
            return DungeonManager.GetFloorType(floor) switch
            {
                FloorType.MainBoss => 0.02f,
                FloorType.Boss     => 0.04f,
                FloorType.Champion => 0.08f,
                _                  => 0.15f,
            };
        }

        // ── Tier: kat tipine göre sabit ───────────────────────────────────────

        public static Rarity TierForFloor(int floor)
        {
            return DungeonManager.GetFloorType(floor) switch
            {
                FloorType.MainBoss => Rarity.B,
                FloorType.Boss     => Rarity.C,
                FloorType.Champion => Rarity.D,
                _                  => Rarity.F,
            };
        }

        // ── Pet seçimi ───────────────────────────────────────────────────────

        public void SelectPet(string instanceId)
        {
            SelectedPetId = instanceId;
            PlayerPrefs.SetString(PrefPetKey, instanceId);
            PlayerPrefs.Save();
            OnPetSelected?.Invoke(SelectedPet);
        }

        public void DeselectPet()
        {
            SelectedPetId = "";
            PlayerPrefs.DeleteKey(PrefPetKey);
            PlayerPrefs.Save();
            OnPetSelected?.Invoke(null);
        }

        // ── Pet tier stat bonusu ─────────────────────────────────────────────

        /// <summary>HP ve ATK için çarpan (1.0 = bonus yok).</summary>
        public static (float hp, float atk) BonusForTier(Rarity tier) => tier switch
        {
            Rarity.SS => (2.20f, 2.20f),
            Rarity.S  => (1.80f, 1.80f),
            Rarity.A  => (1.55f, 1.55f),
            Rarity.B  => (1.35f, 1.35f),
            Rarity.C  => (1.20f, 1.20f),
            Rarity.D  => (1.10f, 1.10f),
            _         => (1.05f, 1.05f),
        };

        // ── Evrim yardımcıları ────────────────────────────────────────────────

        public static Rarity NextTier(Rarity tier) => tier switch
        {
            Rarity.F => Rarity.D,
            Rarity.D => Rarity.C,
            Rarity.C => Rarity.B,
            Rarity.B => Rarity.A,
            Rarity.A => Rarity.S,
            Rarity.S => Rarity.SS,
            _        => Rarity.SS,
        };

        public static EvoCost GetEvoCost(Rarity tier) => tier switch
        {
            Rarity.F => new EvoCost { FodderCount = 5, Gold =     500 },
            Rarity.D => new EvoCost { FodderCount = 5, Gold =   1_500 },
            Rarity.C => new EvoCost { FodderCount = 5, Gold =   4_000 },
            Rarity.B => new EvoCost { FodderCount = 4, Gold =  12_000 },
            Rarity.A => new EvoCost { FodderCount = 3, Gold =  35_000 },
            Rarity.S => new EvoCost { FodderCount = 3, Gold =  80_000 },
            _        => new EvoCost { FodderCount = 99, Gold = int.MaxValue },
        };

        public int FodderAvailable(CapturedMonster target) =>
            _monsters.Count(m => m.InstanceId != target.InstanceId && m.Tier == target.Tier);

        public bool CanEvolve(CapturedMonster m)
        {
            if (m == null || m.Tier == Rarity.SS || m.Tier >= m.MaxEvolutionTier) return false;
            var cost = GetEvoCost(m.Tier);
            return FodderAvailable(m) >= cost.FodderCount &&
                   (EconomyManager.Instance?.Gold ?? 0) >= cost.Gold;
        }

        public (bool success, string error) TryEvolve(CapturedMonster m)
        {
            if (m.Tier == Rarity.SS)          return (false, "En yüksek tier!");
            if (m.Tier >= m.MaxEvolutionTier)  return (false, "Maksimum tier'a ulaştı!");

            var cost   = GetEvoCost(m.Tier);
            var fodder = _monsters
                .Where(x => x.InstanceId != m.InstanceId && x.Tier == m.Tier)
                .Take(cost.FodderCount).ToList();

            if (fodder.Count < cost.FodderCount)
                return (false, $"Yeterli pet yok! ({fodder.Count}/{cost.FodderCount} adet {m.Tier})");

            if (!(EconomyManager.Instance?.SpendGold(cost.Gold) ?? false))
                return (false, $"Yeterli altın yok! ({cost.Gold:N0} gerekli)");

            foreach (var f in fodder)
            {
                if (f.InstanceId == SelectedPetId) DeselectPet();
                _monsters.Remove(f);
            }

            m.Tier        = NextTier(m.Tier);
            m.DisplayName = ApplyEvoPrefix(m.DisplayName, m.Tier);
            Save();
            return (true, null);
        }

        private static string ApplyEvoPrefix(string name, Rarity tier)
        {
            string stripped = name.TrimStart('★').TrimStart(' ');
            string prefix = tier switch
            {
                Rarity.A  => "★ ",
                Rarity.S  => "★★ ",
                Rarity.SS => "★★★ ",
                _         => "",
            };
            return prefix + stripped;
        }

        private static Rarity RollMaxTier(Rarity captured)
        {
            float r = UnityEngine.Random.value;
            return captured switch
            {
                Rarity.F => r < 0.40f ? Rarity.D : r < 0.75f ? Rarity.C : r < 0.95f ? Rarity.B : r < 0.99f ? Rarity.A : Rarity.S,
                Rarity.D => r < 0.35f ? Rarity.C : r < 0.75f ? Rarity.B : r < 0.95f ? Rarity.A : r < 0.99f ? Rarity.S : Rarity.SS,
                Rarity.C => r < 0.35f ? Rarity.B : r < 0.70f ? Rarity.A : r < 0.95f ? Rarity.S : Rarity.SS,
                Rarity.B => r < 0.30f ? Rarity.A : r < 0.75f ? Rarity.S : Rarity.SS,
                _        => Rarity.SS,
            };
        }

        // ── Gacha çekimi ──────────────────────────────────────────────────────

        public CapturedMonster PullFromGacha(Rarity tier)
        {
            var monster = new CapturedMonster
            {
                InstanceId       = Guid.NewGuid().ToString("N").Substring(0, 8),
                DisplayName      = GenerateGachaName(tier),
                Tier             = tier,
                MaxEvolutionTier = RollMaxTier(tier),
                FloorCaptured    = 0,
                CaptureDate      = DateTime.Now.ToString("yyyy-MM-dd"),
            };
            _monsters.Add(monster);
            Save();
            OnMonsterAdded?.Invoke(monster);
            return monster;
        }

        private static string GenerateGachaName(Rarity tier)
        {
            int idx = tier switch { Rarity.D => 1, Rarity.C => 2, Rarity.B => 3, Rarity.A => 4, Rarity.S => 5, Rarity.SS => 6, _ => 0 };
            var names = NamesPerTier[idx];
            return names[UnityEngine.Random.Range(0, names.Length)];
        }

        // ── Koleksiyona ekleme ────────────────────────────────────────────────

        public CapturedMonster TryCapture(int floor)
        {
            if (UnityEngine.Random.value > CaptureChance(floor)) return null;

            var tier    = TierForFloor(floor);
            var maxTier = RollMaxTier(tier);
            var monster = new CapturedMonster
            {
                InstanceId       = Guid.NewGuid().ToString("N").Substring(0, 8),
                DisplayName      = GenerateName(tier, floor),
                Tier             = tier,
                MaxEvolutionTier = maxTier,
                FloorCaptured    = floor,
                CaptureDate      = DateTime.Now.ToString("yyyy-MM-dd"),
            };

            _monsters.Add(monster);
            Save();
            OnMonsterAdded?.Invoke(monster);
            return monster;
        }

        // ── İsim üretici ─────────────────────────────────────────────────────

        private static readonly string[][] NamesPerTier =
        {
            new[] { "Ateş Goblin",   "Küçük Yarasa",  "Çalı Ruhu",     "Kaya Kertenkele", "Bataklık Sıçanı"  }, // F
            new[] { "Hava Perisi",   "Orman Canavarı","Gölge Tilkisi",  "Demir Böceği",    "Buz Parçası"      }, // D
            new[] { "Buz Ejderi",    "Alev Gözlü",    "Taş Troll",      "Fırtına Kuşu",    "Zehirli Ejder"    }, // C
            new[] { "Taş Golem",     "Ejder Lordu",   "Gece Avcısı",    "Karanlık Şövalye","Rüzgar Ruhu"      }, // B
            new[] { "Demir Ejder",   "Alev Savaşçısı","Fırtına Ruhu",   "Gölge Lordu",     "Buz Kraliçesi"    }, // A
            new[] { "Göksel Ejder",  "Rüzgar Tanrısı","Kor Tiranı",     "Gece Efendisi",   "Kristal Dev"      }, // S
            new[] { "Kaos Ejderi",   "Ebedi Alev",    "Efsane Gölge",   "Tanrı Savaşçısı", "Ölümsüz Kral"    }, // SS
        };

        private static string GenerateName(Rarity tier, int floor)
        {
            int idx = tier switch { Rarity.D => 1, Rarity.C => 2, Rarity.B => 3, Rarity.A => 4, Rarity.S => 5, Rarity.SS => 6, _ => 0 };
            var names = NamesPerTier[idx];
            string name  = names[UnityEngine.Random.Range(0, names.Length)];

            string prefix = DungeonManager.GetFloorType(floor) switch
            {
                FloorType.MainBoss => "Efsanevi ",
                FloorType.Boss     => "Güçlü ",
                FloorType.Champion => "Şampiyon ",
                _                  => "",
            };
            return prefix + name;
        }

        // ── Kaydet / Yükle ────────────────────────────────────────────────────

        private void Save()
        {
            PlayerPrefs.SetString(PrefKey, JsonUtility.ToJson(new SaveData { Monsters = _monsters }));
            PlayerPrefs.Save();
        }

        private void Load()
        {
            string json = PlayerPrefs.GetString(PrefKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                var data = JsonUtility.FromJson<SaveData>(json);
                if (data?.Monsters != null)
                {
                    _monsters = data.Monsters;
                    bool dirty = false;
                    foreach (var m in _monsters)
                    {
                        // MaxEvolutionTier == F (0) → eski kayıt, atama yapılmamış
                        if (m.MaxEvolutionTier == Rarity.F)
                        {
                            m.MaxEvolutionTier = RollMaxTier(m.Tier);
                            dirty = true;
                        }
                    }
                    if (dirty) Save();
                }
            }
            SelectedPetId = PlayerPrefs.GetString(PrefPetKey, "");
        }

        public void ResetCollection()
        {
            _monsters.Clear();
            PlayerPrefs.DeleteKey(PrefKey);
            PlayerPrefs.Save();
        }
    }
}
