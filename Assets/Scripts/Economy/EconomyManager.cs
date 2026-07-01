using System;
using UnityEngine;
using CanavarZindanlari.Core;

namespace CanavarZindanlari.Economy
{
    /// <summary>
    /// Altın ve elmas yönetimi. PlayerPrefs ile kalıcı.
    /// Formüller: design/gdd/ekonomi.md
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        // ── Limitler ─────────────────────────────────────────────────────────

        private const int GoldCap    = 999_999;
        private const int DiamondCap =  99_999;

        private const string KeyGold    = "eco_gold";
        private const string KeyDiamond = "eco_diamond";

        // ── Kaynaklar ────────────────────────────────────────────────────────

        public int Gold    { get; private set; }
        public int Diamond { get; private set; }

        public event Action OnResourceChanged;

        // ── Lifecycle ────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            Load();
        }

        // ── Altın ────────────────────────────────────────────────────────────

        public void AddGold(int amount)
        {
            Gold = Mathf.Min(GoldCap, Gold + amount);
            Save();
            OnResourceChanged?.Invoke();
        }

        public bool SpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold = Mathf.Max(0, Gold - amount);
            Save();
            OnResourceChanged?.Invoke();
            return true;
        }

        // ── Elmas ────────────────────────────────────────────────────────────

        public void AddDiamonds(int amount)
        {
            Diamond = Mathf.Min(DiamondCap, Diamond + amount);
            Save();
            OnResourceChanged?.Invoke();
        }

        public bool SpendDiamonds(int amount)
        {
            if (Diamond < amount) return false;
            Diamond -= amount;
            Save();
            OnResourceChanged?.Invoke();
            return true;
        }

        // ── Ödül formülleri ──────────────────────────────────────────────────

        /// <summary>Kat temizleme altın ödülü.</summary>
        public static int FloorGoldReward(int floor)
        {
            float mult = DungeonManager.GetFloorType(floor) switch
            {
                FloorType.MainBoss => 5.0f,
                FloorType.Boss     => 2.5f,
                FloorType.Champion => 1.5f,
                _                  => 1.0f,
            };
            return Mathf.FloorToInt(100 * floor * mult);
        }

        /// <summary>Arena maç ödülü.</summary>
        public static int ArenaGoldReward(bool won) => won ? 50 : 10;

        // ── Kaydet / Yükle ───────────────────────────────────────────────────

        private void Save()
        {
            PlayerPrefs.SetInt(KeyGold,    Gold);
            PlayerPrefs.SetInt(KeyDiamond, Diamond);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            Gold    = PlayerPrefs.GetInt(KeyGold,    0);
            Diamond = PlayerPrefs.GetInt(KeyDiamond, 0);
        }

        public void ResetForTest()
        {
            Gold = Diamond = 0;
            Save();
            OnResourceChanged?.Invoke();
        }
    }
}

