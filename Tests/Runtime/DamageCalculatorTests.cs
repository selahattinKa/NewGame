using NUnit.Framework;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;
using UnityEngine;

namespace CanavarZindanlari.Tests.Runtime
{
    /// <summary>
    /// Unit tests for damage formula — must pass before any combat implementation ships.
    /// Covers: element matrix, crit, minimum damage floor.
    /// </summary>
    public class DamageCalculatorTests
    {
        private SkillData MakeSkill(float multiplier, float crit = 0f)
        {
            var skill = ScriptableObject.CreateInstance<SkillData>();
            skill.Type = SkillType.Attack;
            skill.DamageMultiplier = multiplier;
            skill.CritChance = crit;
            skill.CritMultiplier = 1.5f;
            return skill;
        }

        [Test]
        public void Damage_NeutralElement_NoMitigation_EqualsATKTimesMultiplier()
        {
            var skill = MakeSkill(1f);
            int dmg = DamageCalculator.Calculate(100, 0, Element.Ates, Element.Ates, skill, out _);
            Assert.AreEqual(100, dmg);
        }

        [Test]
        public void Damage_FireVsEarth_IsAdvantage_1_5x()
        {
            var skill = MakeSkill(1f);
            int dmg = DamageCalculator.Calculate(100, 0, Element.Ates, Element.Toprak, skill, out _);
            Assert.AreEqual(150, dmg); // 100 * 1.5 = 150
        }

        [Test]
        public void Damage_FireVsWater_IsDisadvantage_0_75x()
        {
            var skill = MakeSkill(1f);
            int dmg = DamageCalculator.Calculate(100, 0, Element.Ates, Element.Su, skill, out _);
            Assert.AreEqual(75, dmg); // 100 * 0.75 = 75
        }

        [Test]
        public void Damage_WithDefense_ReducedByHalfDEF()
        {
            var skill = MakeSkill(1f);
            int dmg = DamageCalculator.Calculate(100, 40, Element.Ates, Element.Ates, skill, out _);
            Assert.AreEqual(80, dmg); // 100 - floor(40 * 0.5) = 100 - 20 = 80
        }

        [Test]
        public void Damage_MinimumIsOne_EvenWithHighDefense()
        {
            var skill = MakeSkill(1f);
            int dmg = DamageCalculator.Calculate(10, 999, Element.Ates, Element.Ates, skill, out _);
            Assert.GreaterOrEqual(dmg, 1);
        }

        [Test]
        public void Heal_ReturnsPercentOfMaxHP()
        {
            var skill = ScriptableObject.CreateInstance<SkillData>();
            skill.Type = SkillType.Heal;
            skill.HealPercent = 0.3f;
            int heal = DamageCalculator.CalculateHeal(100, skill);
            Assert.AreEqual(30, heal);
        }
    }
}
