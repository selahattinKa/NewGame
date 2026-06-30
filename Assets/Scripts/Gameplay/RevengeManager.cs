using System;
using UnityEngine;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Gameplay
{
    /// <summary>
    /// İntikam Sistemi — yenilince düşman kaydedilir, rematçta bonus verilir.
    /// CombatManager.OnBattleEnded'e abone olur.
    /// </summary>
    public class RevengeManager : MonoBehaviour
    {
        [Serializable]
        public struct RevengeTarget
        {
            public string EnemyName;
            public Element Element;
            public int ATK;
            public int DEF;
            public int MaxHP;
            public string DefeatedAt;  // ISO timestamp
        }

        [SerializeField] private CombatManager _combat;

        public const int RevengeBonusGems = 20;

        public bool         HasActiveRevenge { get; private set; }
        public RevengeTarget ActiveRevenge   { get; private set; }

        /// <summary>Yenilince tetiklenir — UI intikam butonu göstermek için dinler.</summary>
        public event Action<RevengeTarget> OnRevengeAvailable;
        /// <summary>İntikam alınınca tetiklenir — EconomyManager dinler.</summary>
        public event Action<int>           OnRevengeCompleted;  // (bonusGems)

        private const string PrefKey = "revenge_target";

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            LoadFromPrefs();
        }

        private void OnEnable()
        {
            if (_combat != null)
                _combat.OnBattleEnded += HandleBattleEnded;
        }

        private void OnDisable()
        {
            if (_combat != null)
                _combat.OnBattleEnded -= HandleBattleEnded;
        }

        // ── Savaş sonu ────────────────────────────────────────────────────────

        private void HandleBattleEnded(BattleReward reward)
        {
            if (!reward.IsVictory)
            {
                RecordDefeat();
            }
            else if (HasActiveRevenge && IsRevengeMatch())
            {
                CompleteRevenge();
            }
        }

        private void RecordDefeat()
        {
            if (_combat.Enemy == null) return;

            ActiveRevenge = new RevengeTarget
            {
                EnemyName  = _combat.Enemy.DisplayName,
                Element    = _combat.Enemy.Element,
                ATK        = _combat.Enemy.ATK,
                DEF        = _combat.Enemy.DEF,
                MaxHP      = _combat.Enemy.MaxHP,
                DefeatedAt = DateTime.UtcNow.ToString("o"),
            };

            HasActiveRevenge = true;
            SaveToPrefs();

            Debug.Log($"[RevengeManager] İntikam hedefi kaydedildi: {ActiveRevenge.EnemyName}");
            OnRevengeAvailable?.Invoke(ActiveRevenge);
        }

        private bool IsRevengeMatch()
        {
            // Aynı isimde düşmana karşı kazanıldı mı?
            return _combat.Enemy != null &&
                   string.Equals(_combat.Enemy.DisplayName, ActiveRevenge.EnemyName,
                                 StringComparison.OrdinalIgnoreCase);
        }

        private void CompleteRevenge()
        {
            Debug.Log($"[RevengeManager] İntikam alındı! +{RevengeBonusGems} elmas.");
            HasActiveRevenge = false;
            PlayerPrefs.DeleteKey(PrefKey);
            OnRevengeCompleted?.Invoke(RevengeBonusGems);
        }

        // ── Kalıcılık ──────────────────────────────────────────────────────────

        private void SaveToPrefs()
        {
            PlayerPrefs.SetString(PrefKey, JsonUtility.ToJson(ActiveRevenge));
            PlayerPrefs.SetInt(PrefKey + "_active", 1);
        }

        private void LoadFromPrefs()
        {
            if (PlayerPrefs.GetInt(PrefKey + "_active", 0) == 0) return;
            string json = PlayerPrefs.GetString(PrefKey, "");
            if (string.IsNullOrEmpty(json)) return;
            ActiveRevenge    = JsonUtility.FromJson<RevengeTarget>(json);
            HasActiveRevenge = true;
        }

        /// <summary>Debug — editörde intikam kaydını temizle.</summary>
        [ContextMenu("İntikam Kaydını Temizle")]
        private void ClearRevenge()
        {
            HasActiveRevenge = false;
            PlayerPrefs.DeleteKey(PrefKey);
            PlayerPrefs.DeleteKey(PrefKey + "_active");
            Debug.Log("[RevengeManager] İntikam kaydı temizlendi.");
        }
    }
}
