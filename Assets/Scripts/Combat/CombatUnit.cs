using System;
using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Runtime combat state for one combatant (player monster or enemy).
    /// Pure data — no Unity lifecycle. Owned by CombatManager.
    /// </summary>
    [Serializable]
    public class CombatUnit
    {
        public string DisplayName;
        public Element Element;
        public Archetype Archetype;

        public int MaxHP;
        public int CurrentHP;
        public int ATK;
        public int DEF;
        public int SPD;

        public bool IsAlive => CurrentHP > 0;
        public float HPPercent => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f;

        // Skill cooldown trackers (indexed by skill slot)
        public int[] SkillCooldowns = new int[3];

        public void TakeDamage(int amount)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - amount);
        }

        public void Heal(int amount)
        {
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        }

        public void TickCooldowns()
        {
            for (int i = 0; i < SkillCooldowns.Length; i++)
            {
                if (SkillCooldowns[i] > 0) SkillCooldowns[i]--;
            }
        }

        public bool SkillReady(int slot) =>
            slot < SkillCooldowns.Length && SkillCooldowns[slot] == 0;

        public void SetCooldown(int slot, int turns)
        {
            if (slot < SkillCooldowns.Length)
                SkillCooldowns[slot] = turns;
        }
    }
}
