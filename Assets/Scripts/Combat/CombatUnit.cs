using System;
using System.Collections.Generic;
using UnityEngine;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Combat
{
    /// <summary>
    /// Bir savaşçının runtime durumu. Durum etkilerini ve kalkanı yönetir.
    /// </summary>
    [Serializable]
    public class CombatUnit
    {
        public string    DisplayName;
        public Element   Element;   // görsel/narrative — savaşta mekanik etkisi yok
        public Archetype Archetype;

        public int MaxHP;
        public int CurrentHP;
        public int BaseATK;
        public int BaseDEF;
        public int SPD;

        public bool  IsAlive    => CurrentHP > 0;
        public float HPPercent  => MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f;

        public int[] SkillCooldowns = new int[4];

        // ── Durum etkileri ────────────────────────────────────────────────────

        public List<ActiveEffect> ActiveEffects = new List<ActiveEffect>();
        public int ShieldHP;

        // ── Efektlere göre hesaplanan statlar ────────────────────────────────

        public int ATK
        {
            get
            {
                float mod = GetModifier(StatusEffectType.AtkMod);
                return Mathf.FloorToInt(BaseATK * mod);
            }
        }

        public int DEF
        {
            get
            {
                float mod = GetModifier(StatusEffectType.DefMod);
                return Mathf.FloorToInt(BaseDEF * mod);
            }
        }

        private float GetModifier(StatusEffectType type)
        {
            float mod = 1f;
            foreach (var e in ActiveEffects)
                if (!e.IsExpired && e.Type == type) mod *= e.Value;
            return mod;
        }

        // ── Hasar / iyileştirme ───────────────────────────────────────────────

        /// <summary>
        /// Kalkan önce hasar emer; kalan HP'den düşer.
        /// </summary>
        public void TakeDamage(int amount)
        {
            // Hasar Azaltma
            float reduction = GetModifier(StatusEffectType.DamageReduction);
            if (reduction < 1f)
                amount = Mathf.Max(1, Mathf.FloorToInt(amount * reduction));

            // Kalkan absorpsiyonu
            if (ShieldHP > 0)
            {
                int absorbed = Mathf.Min(ShieldHP, amount);
                ShieldHP -= absorbed;
                amount   -= absorbed;
                if (ShieldHP <= 0) RemoveEffect(StatusEffectType.Shield);
            }

            CurrentHP = Mathf.Max(0, CurrentHP - amount);
        }

        public void Heal(int amount)
        {
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        }

        // ── Durum etkisi uygulama ─────────────────────────────────────────────

        /// <summary>
        /// Becerinin tanımladığı efekti bu birime uygular.
        /// sourceMaxHP: DoT/Shield hesabı için kaynak ya da hedef max HP.
        /// </summary>
        public void ApplyEffect(SkillEffect def, int sourceMaxHP)
        {
            // Stack olmaz — aynı tür varsa yeniler
            RemoveEffect(def.Type);

            if (def.Type == StatusEffectType.Shield)
            {
                ShieldHP = Mathf.Max(1, Mathf.FloorToInt(sourceMaxHP * def.Value));
                // Kalkan süre takibi için de ekle
                ActiveEffects.Add(new ActiveEffect
                {
                    Type           = StatusEffectType.Shield,
                    RemainingTurns = def.Duration,
                    Value          = def.Value,
                });
                return;
            }

            ActiveEffects.Add(new ActiveEffect
            {
                Type           = def.Type,
                RemainingTurns = def.Duration == 0 ? -1 : def.Duration,
                Value          = def.Value,
            });
        }

        // ── Tur mantığı ───────────────────────────────────────────────────────

        /// <summary>Bu birimin kendi turunda DoT hasarını uygular. Hasar toplamını döner.</summary>
        public int TickDoT()
        {
            int total = 0;
            foreach (var e in ActiveEffects)
            {
                if (e.IsExpired) continue;
                if (e.Type == StatusEffectType.BurnDoT || e.Type == StatusEffectType.PoisonDoT)
                {
                    int dmg = Mathf.Max(1, Mathf.FloorToInt(MaxHP * e.Value));
                    CurrentHP = Mathf.Max(0, CurrentHP - dmg);
                    total += dmg;
                }
            }
            return total;
        }

        /// <summary>Tur sonunda tüm aktif efektlerin sayacını azaltır; dolananları kaldırır.</summary>
        public void TickEffectDurations()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                ActiveEffects[i].Tick();
                if (ActiveEffects[i].IsExpired)
                {
                    if (ActiveEffects[i].Type == StatusEffectType.Shield) ShieldHP = 0;
                    ActiveEffects.RemoveAt(i);
                }
            }
        }

        public bool IsStunned         => HasActive(StatusEffectType.Stun);
        public bool HasGuaranteedCrit => HasActive(StatusEffectType.GuaranteedCrit);

        public void ConsumeGuaranteedCrit() => RemoveEffect(StatusEffectType.GuaranteedCrit);

        // ── Skill cooldown'ları ───────────────────────────────────────────────

        public void TickCooldowns()
        {
            for (int i = 0; i < SkillCooldowns.Length; i++)
                if (SkillCooldowns[i] > 0) SkillCooldowns[i]--;
        }

        public bool SkillReady(int slot) =>
            slot < SkillCooldowns.Length && SkillCooldowns[slot] == 0;

        public void SetCooldown(int slot, int turns)
        {
            if (slot < SkillCooldowns.Length)
                SkillCooldowns[slot] = turns;
        }

        // ── Yardımcılar ───────────────────────────────────────────────────────

        private bool HasActive(StatusEffectType type)
        {
            foreach (var e in ActiveEffects)
                if (!e.IsExpired && e.Type == type) return true;
            return false;
        }

        private void RemoveEffect(StatusEffectType type)
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
                if (ActiveEffects[i].Type == type) ActiveEffects.RemoveAt(i);
        }
    }
}
