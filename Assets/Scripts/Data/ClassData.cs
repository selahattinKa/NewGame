using UnityEngine;

namespace CanavarZindanlari.Data
{
    public enum DamageType { Physical, Magic }

    /// <summary>
    /// Oyuncu sınıfı şablonu. Stat profili + 4 yetenek slotu.
    /// Maps to design/gdd/oyuncu-sinif-sistemi.md Kural 1-3.
    /// </summary>
    [CreateAssetMenu(fileName = "ClassData", menuName = "CanavarZindanlari/Class Data")]
    public class ClassData : ScriptableObject
    {
        [Header("Identity")]
        public string ClassName;
        public DamageType DamageType;

        [Header("Base Stats (Level 1) — oyuncu-sinif-sistemi.md Kural 1")]
        public int BaseHP;
        public int BaseATK;
        public int BaseDEF;
        public int BaseSPD;

        [Header("Skills (Slot 0=CD0, 1=CD3, 2=CD5, 3=CD8)")]
        public SkillData[] Skills = new SkillData[4];

        // stat_growth = 0.08 (Kural 1 Formül 3)
        private const float StatGrowth = 0.08f;

        public int StatAtLevel(int baseStat, int level)
            => Mathf.FloorToInt(baseStat * (1f + StatGrowth * (level - 1)));
    }
}
