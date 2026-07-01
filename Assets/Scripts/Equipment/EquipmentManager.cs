using System;
using System.Collections.Generic;
using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Equipment
{
    public enum EquipmentSlotType
    {
        Weapon,
        Head, Chest, Gloves, Legs, Boots,
        Necklace, Ring1, Ring2, Earring1, Earring2
    }

    [Serializable]
    public class OwnedEquipment
    {
        public string            Id;
        public EquipmentSlotType Slot;
        public Rarity            Tier;
        public int               BonusATK;
        public int               BonusHP;
        public int               BonusDEF;
        public int               BonusSPD;
        public string            DisplayName;
        public string            ClassRestriction;
    }

    /// <summary>
    /// Ekipman envanterini ve kuşanılan slotları yönetir.
    /// Singleton — sahnede yoksa otomatik oluşturulur.
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        private static EquipmentManager _instance;
        public static EquipmentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("EquipmentManager");
                    _instance = go.AddComponent<EquipmentManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private List<OwnedEquipment>                  _inventory = new();
        private Dictionary<EquipmentSlotType, string> _equipped  = new();

        private const string KeyInv = "eq_inventory";
        private const string KeyEq  = "eq_equipped";

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        // ── Public API ────────────────────────────────────────────────────────

        public IReadOnlyList<OwnedEquipment> Inventory => _inventory;

        public void AddEquipment(OwnedEquipment eq)
        {
            _inventory.Add(eq);
            Save();
        }

        public void Equip(string id)
        {
            var eq = _inventory.Find(e => e.Id == id);
            if (eq == null) return;
            _equipped[eq.Slot] = id;
            Save();
        }

        public void Unequip(EquipmentSlotType slot)
        {
            _equipped.Remove(slot);
            Save();
        }

        public OwnedEquipment GetEquipped(EquipmentSlotType slot)
        {
            if (!_equipped.TryGetValue(slot, out string id)) return null;
            return _inventory.Find(e => e.Id == id);
        }

        public bool IsEquipped(string id)
        {
            foreach (var kv in _equipped)
                if (kv.Value == id) return true;
            return false;
        }

        public (int atk, int hp, int def, int spd) GetTotalBonuses()
        {
            int atk = 0, hp = 0, def = 0, spd = 0;
            foreach (EquipmentSlotType slot in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                var eq = GetEquipped(slot);
                if (eq == null) continue;
                atk += eq.BonusATK;
                hp  += eq.BonusHP;
                def += eq.BonusDEF;
                spd += eq.BonusSPD;
            }
            return (atk, hp, def, spd);
        }

        // ── Fabrika ───────────────────────────────────────────────────────────

        public static OwnedEquipment CreateEquipment(EquipmentSlotType slot, Rarity tier,
            string classRestriction = "")
        {
            var (atk, hp, def, spd) = StatsFor(slot, tier);
            return new OwnedEquipment
            {
                Id               = Guid.NewGuid().ToString(),
                Slot             = slot,
                Tier             = tier,
                BonusATK         = atk,
                BonusHP          = hp,
                BonusDEF         = def,
                BonusSPD         = spd,
                DisplayName      = BuildName(slot, tier, classRestriction),
                ClassRestriction = classRestriction,
            };
        }

        public static (int atk, int hp, int def, int spd) StatsFor(EquipmentSlotType slot, Rarity tier)
        {
            bool isWeapon = slot == EquipmentSlotType.Weapon;
            bool isArmor  = slot >= EquipmentSlotType.Head && slot <= EquipmentSlotType.Boots;

            if (isWeapon) return tier switch
            {
                Rarity.B  => (80,   0,   0,  0),
                Rarity.A  => (200,  0,   0,  0),
                Rarity.S  => (450,  0,   0,  0),
                Rarity.SS => (1000, 0,   0,  0),
                _         => (20,   0,   0,  0),
            };

            if (isArmor) return tier switch
            {
                Rarity.B  => (0, 200,  20,  0),
                Rarity.A  => (0, 450,  45,  0),
                Rarity.S  => (0, 900,  90,  0),
                Rarity.SS => (0, 1800, 180, 0),
                _         => (0, 60,   6,   0),
            };

            // takı
            return tier switch
            {
                Rarity.B  => (60,  0, 0, 6),
                Rarity.A  => (140, 0, 0, 14),
                Rarity.S  => (300, 0, 0, 30),
                Rarity.SS => (620, 0, 0, 62),
                _         => (20,  0, 0, 2),
            };
        }

        private static string BuildName(EquipmentSlotType slot, Rarity tier, string cls)
        {
            string prefix = tier switch
            {
                Rarity.SS => "Efsanevi",
                Rarity.S  => "Destansı",
                Rarity.A  => "Nadir",
                Rarity.B  => "Sağlam",
                _         => "Eski",
            };
            string item = slot switch
            {
                EquipmentSlotType.Weapon   => cls switch
                {
                    "Savasco" => "Kılıç",
                    "Buyucu"  => "Asa",
                    "Hirsiz"  => "Hançer",
                    "Sifaci"  => "Kutsal Asa",
                    _         => "Silah",
                },
                EquipmentSlotType.Head     => "Kask",
                EquipmentSlotType.Chest    => "Göğüslük",
                EquipmentSlotType.Gloves   => "Eldiven",
                EquipmentSlotType.Legs     => "Pantolon",
                EquipmentSlotType.Boots    => "Çizme",
                EquipmentSlotType.Necklace => "Kolye",
                EquipmentSlotType.Ring1    => "Yüzük",
                EquipmentSlotType.Ring2    => "Yüzük",
                EquipmentSlotType.Earring1 => "Küpe",
                EquipmentSlotType.Earring2 => "Küpe",
                _                          => "Eşya",
            };
            return $"{prefix} {item} [{tier}]";
        }

        // ── Gacha yardımcıları ────────────────────────────────────────────────

        public static EquipmentSlotType RandomArmorSlot()
        {
            var slots = new[]
            {
                EquipmentSlotType.Head, EquipmentSlotType.Chest,
                EquipmentSlotType.Gloves, EquipmentSlotType.Legs, EquipmentSlotType.Boots
            };
            return slots[UnityEngine.Random.Range(0, slots.Length)];
        }

        public static EquipmentSlotType RandomAccessorySlot()
        {
            var slots = new[]
            {
                EquipmentSlotType.Necklace,
                EquipmentSlotType.Ring1, EquipmentSlotType.Ring2,
                EquipmentSlotType.Earring1, EquipmentSlotType.Earring2
            };
            return slots[UnityEngine.Random.Range(0, slots.Length)];
        }

        // ── Kaydet / Yükle ────────────────────────────────────────────────────

        private void Save()
        {
            PlayerPrefs.SetString(KeyInv, JsonUtility.ToJson(new InvWrapper { items = _inventory }));
            var entries = new List<SlotEntry>();
            foreach (var kv in _equipped)
                entries.Add(new SlotEntry { slot = (int)kv.Key, id = kv.Value });
            PlayerPrefs.SetString(KeyEq, JsonUtility.ToJson(new EqWrapper { entries = entries }));
            PlayerPrefs.Save();
        }

        private void Load()
        {
            string invJson = PlayerPrefs.GetString(KeyInv, "");
            if (!string.IsNullOrEmpty(invJson))
            {
                var w = JsonUtility.FromJson<InvWrapper>(invJson);
                _inventory = w?.items ?? new List<OwnedEquipment>();
            }
            string eqJson = PlayerPrefs.GetString(KeyEq, "");
            if (!string.IsNullOrEmpty(eqJson))
            {
                var w = JsonUtility.FromJson<EqWrapper>(eqJson);
                _equipped.Clear();
                if (w?.entries != null)
                    foreach (var e in w.entries)
                        _equipped[(EquipmentSlotType)e.slot] = e.id;
            }
        }

        [Serializable] private class InvWrapper { public List<OwnedEquipment> items;   }
        [Serializable] private class EqWrapper  { public List<SlotEntry>      entries; }
        [Serializable] private class SlotEntry  { public int slot; public string id;   }
    }
}
