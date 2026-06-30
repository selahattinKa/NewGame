using UnityEngine;
using UnityEngine.Video;

namespace CanavarZindanlari.Data
{
    public enum Element { Ates, Su, Toprak, Hava }
    public enum Archetype { Saldirgan, Tank, Destekci, Buyucu }
    public enum Rarity { F, D, C, B, A, S, SS }

    /// <summary>
    /// Immutable monster template — instances managed by MonsterInstance.
    /// Maps to design/gdd/canavar-veritabani.md
    /// </summary>
    [CreateAssetMenu(fileName = "MonsterData", menuName = "CanavarZindanlari/Monster Data")]
    public class MonsterData : ScriptableObject
    {
        [Header("Identity")]
        public string MonsterId;
        public string DisplayName;
        public Element Element;
        public Archetype Archetype;
        public Rarity BaseRarity;
        public int EvolutionStage;        // 1 = A form, 2 = B form, 3 = C form (SS)
        public MonsterData EvolvesTo;
        public MonsterData EvolvesFrom;

        [Header("Base Stats (Level 1, Stage A, Star 0)")]
        public int BaseHP;
        public int BaseATK;
        public int BaseDEF;
        public int BaseSPD;

        [Header("Visual")]
        public Sprite CardSprite;
        public VideoClip AttackVideo;     // Kling AI generated attack animation

        [Header("Description")]
        [TextArea] public string Description;

        // ── Stat formulas from canavar-guclendirme.md ──────────────────────────

        private static readonly float[] GrowthRates =
            { 0.02f, 0.022f, 0.025f, 0.028f, 0.030f }; // F, D, C, B, A (S/SS use A rate)

        public float GetGrowthRate()
        {
            int index = Mathf.Clamp((int)BaseRarity, 0, GrowthRates.Length - 1);
            return GrowthRates[index];
        }

        /// <summary>
        /// Stat at given level, before evolution/star multipliers.
        /// Formula: floor(base_stat * (1 + growth_rate * (level - 1)))
        /// </summary>
        public int StatAtLevel(int baseStat, int level)
        {
            float factor = 1f + GetGrowthRate() * (level - 1);
            return Mathf.FloorToInt(baseStat * factor);
        }

        /// <summary>
        /// XP required to reach next level.
        /// Formula: floor(50 * level^1.8)
        /// </summary>
        public static int XpThreshold(int level)
        {
            return Mathf.FloorToInt(50f * Mathf.Pow(level, 1.8f));
        }
    }
}
