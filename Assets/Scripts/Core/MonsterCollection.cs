using System;
using System.Collections.Generic;
using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Core
{
    [Serializable]
    public class CapturedMonster
    {
        public string InstanceId;
        public string DisplayName;
        public Rarity Tier;
        public int    FloorCaptured;
        public string CaptureDate;
    }

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
            Rarity.B => (1.35f, 1.35f),
            Rarity.C => (1.20f, 1.20f),
            Rarity.D => (1.10f, 1.10f),
            _        => (1.05f, 1.05f),  // F
        };

        // ── Koleksiyona ekleme ────────────────────────────────────────────────

        public CapturedMonster TryCapture(int floor)
        {
            if (UnityEngine.Random.value > CaptureChance(floor)) return null;

            var tier    = TierForFloor(floor);
            var monster = new CapturedMonster
            {
                InstanceId    = Guid.NewGuid().ToString("N").Substring(0, 8),
                DisplayName   = GenerateName(tier, floor),
                Tier          = tier,
                FloorCaptured = floor,
                CaptureDate   = DateTime.Now.ToString("yyyy-MM-dd"),
            };

            _monsters.Add(monster);
            Save();
            OnMonsterAdded?.Invoke(monster);
            return monster;
        }

        // ── İsim üretici ─────────────────────────────────────────────────────

        private static readonly string[][] NamesPerTier =
        {
            new[] { "Ateş Goblin", "Küçük Yarasa", "Çalı Ruhu",    "Kaya Kertenkele", "Bataklık Sıçanı"  }, // F
            new[] { "Hava Perisi", "Orman Canavarı","Gölge Tilkisi","Demir Böceği",    "Buz Parçası"      }, // D
            new[] { "Buz Ejderi",  "Alev Gözlü",   "Taş Troll",    "Fırtına Kuşu",    "Zehirli Ejder"    }, // C
            new[] { "Taş Golem",   "Ejder Lordu",   "Gece Avcısı",  "Karanlık Şövalye","Rüzgar Ruhu"      }, // B
        };

        private static string GenerateName(Rarity tier, int floor)
        {
            int   idx    = tier == Rarity.F ? 0 : tier == Rarity.D ? 1 : tier == Rarity.C ? 2 : 3;
            var   names  = NamesPerTier[idx];
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
                if (data?.Monsters != null) _monsters = data.Monsters;
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
