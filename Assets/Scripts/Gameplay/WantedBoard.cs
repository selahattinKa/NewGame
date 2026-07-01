using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Gameplay
{
    /// <summary>
    /// Aranıyor Tahtası — her gün 3 hedef, gece yarısı sıfırlanır.
    /// Hedefi yenince bonus altın kazanılır.
    /// </summary>
    public class WantedBoard : MonoBehaviour
    {
        [Serializable]
        public struct WantedEntry
        {
            public string  MonsterId;
            public string  DisplayName;
            public Element Element;
            public float   BonusMultiplier;  // altın çarpanı: 1.5x – 3.0x
            public bool    IsCompleted;
        }

        [SerializeField] private CombatManager _combat;

        public IReadOnlyList<WantedEntry> Entries => _entries;

        public event Action                    OnBoardRefreshed;
        public event Action<WantedEntry, int>  OnWantedCompleted; // (entry, bonusGold)

        private List<WantedEntry> _entries = new();
        private string _lastGeneratedDate;

        private const int EntryCount     = 3;
        private const int BaseWantedGold = 300;
        private const string PrefDate    = "wanted_date";
        private const string PrefDone    = "wanted_done_";

        // ── Prototype canavar havuzu ───────────────────────────────────────────

        private static readonly (string Id, string Name, Element El)[] MonsterPool =
        {
            ("ates_ejderi",   "Ateş Ejderi",     Element.Ates),
            ("golge_kurdu",   "Gölge Kurdu",      Element.Hava),
            ("demir_golem",   "Demir Golem",      Element.Toprak),
            ("buz_buyucusu",  "Buz Büyücüsü",     Element.Su),
            ("kan_vampiri",   "Kan Vampiri",       Element.Hava),
            ("zehir_yilani",  "Zehir Yılanı",     Element.Toprak),
            ("firtina_kartali","Fırtına Kartalı",  Element.Hava),
            ("lav_devi",      "Lav Devi",          Element.Ates),
            ("buz_kaplani",   "Buz Kaplanı",       Element.Su),
            ("toprak_tanrici","Toprak Tanrıçası",  Element.Toprak),
        };

        private static readonly float[] Multipliers = { 1.5f, 2.0f, 2.5f, 3.0f };

        // ── Lifecycle ──────────────────────────────────────────────────────────

        private void Awake()
        {
            RefreshIfNewDay();
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

        // ── Günlük yenileme ────────────────────────────────────────────────────

        public void RefreshIfNewDay()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            if (PlayerPrefs.GetString(PrefDate, "") == today)
            {
                LoadCompletions(today);
                return;
            }

            GenerateEntries(today);
            PlayerPrefs.SetString(PrefDate, today);
            PlayerPrefs.Save();
            OnBoardRefreshed?.Invoke();
            Debug.Log($"[WantedBoard] Yeni gün — {EntryCount} hedef oluşturuldu.");
        }

        private void GenerateEntries(string dateKey)
        {
            _entries.Clear();
            int seed = int.Parse(dateKey);  // yyyyMMdd → deterministik
            var rng  = new System.Random(seed);

            var pool    = new List<int>(Enumerable.Range(0, MonsterPool.Length));
            var mulPool = new List<float>(Multipliers);

            for (int i = 0; i < EntryCount && pool.Count > 0; i++)
            {
                int poolIdx    = rng.Next(pool.Count);
                int monsterIdx = pool[poolIdx];
                pool.RemoveAt(poolIdx);

                int mulIdx = rng.Next(mulPool.Count);
                float mul  = mulPool[mulIdx];

                var (id, name, el) = MonsterPool[monsterIdx];
                _entries.Add(new WantedEntry
                {
                    MonsterId       = id,
                    DisplayName     = name,
                    Element         = el,
                    BonusMultiplier = mul,
                    IsCompleted     = false,
                });
            }

            LoadCompletions(dateKey);
        }

        private void LoadCompletions(string dateKey)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                e.IsCompleted = PlayerPrefs.GetInt($"{PrefDone}{dateKey}_{i}", 0) == 1;
                _entries[i]   = e;
            }
        }

        // ── Savaş sonu ────────────────────────────────────────────────────────

        private void HandleBattleEnded(BattleReward reward)
        {
            if (!reward.IsVictory || _combat.Enemy == null) return;

            string defeatedName = _combat.Enemy.DisplayName;
            string today        = DateTime.Now.ToString("yyyyMMdd");

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (entry.IsCompleted) continue;
                if (!string.Equals(entry.DisplayName, defeatedName,
                                   StringComparison.OrdinalIgnoreCase)) continue;

                entry.IsCompleted = true;
                _entries[i]       = entry;

                PlayerPrefs.SetInt($"{PrefDone}{today}_{i}", 1);
                PlayerPrefs.Save();

                int bonus = Mathf.RoundToInt(BaseWantedGold * entry.BonusMultiplier);
                Debug.Log($"[WantedBoard] Hedef tamamlandı: {entry.DisplayName} " +
                          $"(x{entry.BonusMultiplier}) → +{bonus} altın");
                OnWantedCompleted?.Invoke(entry, bonus);
                break;
            }
        }

        /// <summary>Debug — editörde bugünün tahtasını sıfırla.</summary>
        [ContextMenu("Bugünün Tahtasını Sıfırla")]
        private void ResetToday()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            PlayerPrefs.DeleteKey(PrefDate);
            for (int i = 0; i < EntryCount; i++)
                PlayerPrefs.DeleteKey($"{PrefDone}{today}_{i}");
            RefreshIfNewDay();
            Debug.Log("[WantedBoard] Tahta sıfırlandı.");
        }
    }
}
