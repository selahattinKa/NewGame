using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Pure static damage math.
    /// Maps to design/gdd/hasar-hesaplama.md pipeline (element çarpanı kaldırıldı).
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Fiziksel veya büyü hasarı hesaplar.
        /// defenseReductionFactor: Fiziksel=2, Büyü=4 (oyuncu-sinif-sistemi.md Kural 2)
        /// </summary>
        public static int Calculate(
            int attackerATK,
            int defenderDEF,
            int defenseReductionFactor,
            SkillData skill,
            out bool isCrit,
            bool guaranteedCrit = false)
        {
            isCrit = false;

            float critMod = 1f;
            bool rollCrit = guaranteedCrit || (skill.CritChance > 0f && Random.value < skill.CritChance);
            if (rollCrit)
            {
                critMod = skill.CritMultiplier;
                isCrit  = true;
            }

            float variance  = Random.Range(0.85f, 1.15f);
            int rawDamage   = Mathf.FloorToInt(attackerATK * skill.DamageMultiplier * critMod * variance);
            int mitigated   = Mathf.FloorToInt(defenderDEF / (float)defenseReductionFactor);
            return Mathf.Max(1, rawDamage - mitigated);
        }

        /// <summary>
        /// Çoklu vuruş (Suikast Fırtınası) — her vuruş bağımsız crit, ayrı MultiHitCritChance.
        /// </summary>
        public static int CalculateOneHit(
            int attackerATK,
            int defenderDEF,
            int defenseReductionFactor,
            SkillData skill,
            out bool isCrit)
        {
            isCrit = false;
            float critMod = 1f;
            if (skill.MultiHitCritChance > 0f && Random.value < skill.MultiHitCritChance)
            {
                critMod = skill.CritMultiplier;
                isCrit  = true;
            }

            float variance = Random.Range(0.85f, 1.15f);
            int raw        = Mathf.FloorToInt(attackerATK * skill.DamageMultiplier * critMod * variance);
            int mit        = Mathf.FloorToInt(defenderDEF / (float)defenseReductionFactor);
            return Mathf.Max(1, raw - mit);
        }

        public static int CalculateHeal(int maxHP, float healPercent)
            => Mathf.Max(1, Mathf.FloorToInt(maxHP * healPercent));
    }
}
