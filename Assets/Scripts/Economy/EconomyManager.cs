using System;
using UnityEngine;

namespace CanavarZindanlari.Economy
{
    /// <summary>
    /// Manages Gold, Energy, and Diamonds.
    /// Caps and formulas from design/gdd/ekonomi.md.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        private const int GoldCap    = 999_999;
        private const int EnergyCap  = 100;
        private const int DiamondCap = 99_999;

        private const float EnergyRegenMinutes = 5f;   // 1 energy per 5 min
        private const int   EnergyPerFloor     = 2;

        public int Gold    { get; private set; }
        public int Energy  { get; private set; }
        public int Diamond { get; private set; }

        public event Action OnResourceChanged;

        private DateTime _lastEnergyRegen;

        // ── Bağlantı ───────────────────────────────────────────────────────────

        private void Start()
        {
            if (IAPManager.Instance != null)
                IAPManager.Instance.GemsGranted += OnGemsGranted;

            if (AdManager.Instance != null)
                AdManager.Instance.RewardGranted += OnAdReward;
        }

        private void OnDestroy()
        {
            if (IAPManager.Instance != null)
                IAPManager.Instance.GemsGranted -= OnGemsGranted;

            if (AdManager.Instance != null)
                AdManager.Instance.RewardGranted -= OnAdReward;
        }

        private void OnGemsGranted(string productId, int gems)
        {
            Debug.Log($"[EconomyManager] IAP ödülü: {gems} elmas ({productId})");
            AddDiamonds(gems);
        }

        private void OnAdReward(int gems)
        {
            Debug.Log($"[EconomyManager] Reklam ödülü: {gems} elmas");
            AddDiamonds(gems);
        }

        // ── Gold ───────────────────────────────────────────────────────────────

        public bool SpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold = Mathf.Max(0, Gold - amount);
            OnResourceChanged?.Invoke();
            return true;
        }

        public void AddGold(int amount)
        {
            Gold = Mathf.Min(GoldCap, Gold + amount);
            OnResourceChanged?.Invoke();
        }

        // ── Energy ─────────────────────────────────────────────────────────────

        public bool SpendEnergy(int amount)
        {
            if (Energy < amount) return false;
            Energy -= amount;
            OnResourceChanged?.Invoke();
            return true;
        }

        public bool CanEnterDungeon() => Energy >= EnergyPerFloor;

        public void TickEnergyRegen()
        {
            if (Energy >= EnergyCap) return;
            var elapsed = (float)(DateTime.UtcNow - _lastEnergyRegen).TotalMinutes;
            int gained = Mathf.FloorToInt(elapsed / EnergyRegenMinutes);
            if (gained <= 0) return;
            Energy = Mathf.Min(EnergyCap, Energy + gained);
            _lastEnergyRegen = DateTime.UtcNow;
            OnResourceChanged?.Invoke();
        }

        // ── Diamonds ───────────────────────────────────────────────────────────

        public bool SpendDiamonds(int amount)
        {
            if (Diamond < amount) return false;
            Diamond -= amount;
            OnResourceChanged?.Invoke();
            return true;
        }

        public void AddDiamonds(int amount)
        {
            Diamond = Mathf.Min(DiamondCap, Diamond + amount);
            OnResourceChanged?.Invoke();
        }

        // ── Floor gold reward ──────────────────────────────────────────────────

        /// <summary>
        /// Formula from ekonomi.md:
        /// floor(base_gold * floor_number * difficulty_multiplier)
        /// </summary>
        public static int FloorGoldReward(int floorNumber, float difficultyMultiplier = 1f)
        {
            const int BaseGold = 100;
            return Mathf.FloorToInt(BaseGold * floorNumber * difficultyMultiplier);
        }
    }
}
