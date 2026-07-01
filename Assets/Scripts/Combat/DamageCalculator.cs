using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Pure static damage math — no MonoBehaviour state.
    /// All formulas from hasar-hesaplama.md.
    /// Element advantages removed — no element multiplier.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Calculate final physical damage.
        /// Formula: max(1, floor(ATK * skill_multiplier * crit_mod * variance) - floor(DEF * 0.5))
        /// </summary>
        public static int Calculate(
            int attackerATK,
            int defenderDEF,
            SkillData skill,
            out bool isCrit)
        {
            isCrit = false;

            float critMod = 1f;
            if (skill.CritChance > 0f && Random.value < skill.CritChance)
            {
                critMod = skill.CritMultiplier;
                isCrit = true;
            }

            float variance  = Random.Range(0.85f, 1.15f);
            int rawDamage   = Mathf.FloorToInt(attackerATK * skill.DamageMultiplier * critMod * variance);
            int mitigated   = Mathf.FloorToInt(defenderDEF * 0.5f);
            return Mathf.Max(1, rawDamage - mitigated);
        }

        /// <summary>Returns heal amount from heal skill.</summary>
        public static int CalculateHeal(int maxHP, SkillData skill)
        {
            return Mathf.FloorToInt(maxHP * skill.HealPercent);
        }
    }
}
