using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Pure static damage math — no MonoBehaviour state.
    /// All formulas from hasar-hesaplama.md and element-sistemi.md.
    /// </summary>
    public static class DamageCalculator
    {
        // Element advantage matrix: [attacker][defender] -> multiplier
        // Fire=0, Water=1, Earth=2, Air=3
        // Cycle: Fire > Earth > Air > Water > Fire
        private static readonly float[,] ElementMatrix =
        {
            // vs Fire  Water  Earth  Air
            { 1.00f,  0.75f,  1.50f,  1.00f }, // Fire attacks
            { 1.50f,  1.00f,  1.00f,  0.75f }, // Water attacks
            { 1.00f,  1.50f,  1.00f,  0.75f }, // Earth attacks
            { 0.75f,  1.00f,  1.50f,  1.00f }, // Air attacks
        };

        /// <summary>
        /// Calculate final physical damage.
        /// Formula: floor(ATK * skill_multiplier * element_mod * crit_mod) - floor(DEF * 0.5)
        /// </summary>
        public static int Calculate(
            int attackerATK,
            int defenderDEF,
            Element attackerElement,
            Element defenderElement,
            SkillData skill,
            out bool isCrit)
        {
            isCrit = false;

            float elementMod = ElementMatrix[(int)attackerElement, (int)defenderElement];

            float critMod = 1f;
            if (skill.CritChance > 0f && Random.value < skill.CritChance)
            {
                critMod = skill.CritMultiplier;
                isCrit = true;
            }

            int rawDamage = Mathf.FloorToInt(attackerATK * skill.DamageMultiplier * elementMod * critMod);
            int mitigated = Mathf.FloorToInt(defenderDEF * 0.5f);
            return Mathf.Max(1, rawDamage - mitigated);
        }

        /// <summary>Returns heal amount (flat HP from heal skill).</summary>
        public static int CalculateHeal(int maxHP, SkillData skill)
        {
            return Mathf.FloorToInt(maxHP * skill.HealPercent);
        }

        public static float GetElementMultiplier(Element attacker, Element defender)
        {
            return ElementMatrix[(int)attacker, (int)defender];
        }
    }
}
