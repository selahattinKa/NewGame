using UnityEngine;

namespace CanavarZindanlari.Data
{
    public enum SkillType { Attack, HeavyAttack, Heal }

    /// <summary>
    /// ScriptableObject definition for a combat skill.
    /// Maps to hibrit-savas.md skill system.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "CanavarZindanlari/Skill Data")]
    public class SkillData : ScriptableObject
    {
        public string SkillName;
        public SkillType Type;
        public int CooldownTurns;       // 0 = always available
        public int ManaCost;            // reserved for future MMORPG mode
        [TextArea] public string Description;

        [Header("Damage (% of ATK stat)")]
        [Range(0f, 3f)] public float DamageMultiplier = 1f;
        [Range(0f, 1f)] public float CritChance = 0f;
        public float CritMultiplier = 1.5f;

        [Header("Heal (% of max HP)")]
        [Range(0f, 1f)] public float HealPercent = 0f;
    }
}
