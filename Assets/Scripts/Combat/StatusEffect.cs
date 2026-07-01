using System;
using UnityEngine;

namespace CanavarZindanlari.Combat
{
    public enum StatusEffectType
    {
        BurnDoT,         // Yanma — her tur floor(max_hp × value) hasar
        PoisonDoT,       // Zehir — her tur floor(max_hp × value) hasar
        Stun,            // Sersemletme — 1 tur aksiyon atlanır
        DefMod,          // DEF çarpanı: >1 = güçlendir, <1 = kır
        AtkMod,          // ATK çarpanı: >1 = güçlendir, <1 = zayıflat
        Shield,          // Kalkan — value = max_hp oranı; runtime'da HP'ye çevrilir
        GuaranteedCrit,  // Kesin Kritik — ilk saldırıda tükenir (RemainingTurns=-1)
        DamageReduction, // Hasar Azaltma — gelen hasar × value
    }

    [Serializable]
    public class ActiveEffect
    {
        public StatusEffectType Type;
        public int RemainingTurns; // -1 = tüketilene kadar aktif (GuaranteedCrit)
        public float Value;

        public bool IsExpired => RemainingTurns == 0;

        public void Tick()
        {
            if (RemainingTurns > 0) RemainingTurns--;
        }
    }

    /// <summary>SkillData Inspector'ında bir becerinin uyguladığı etkiyi tanımlar.</summary>
    [Serializable]
    public class SkillEffect
    {
        public StatusEffectType Type;
        /// <summary>DoT için oran (0.05); DefMod/AtkMod için çarpan (0.70 / 1.50); Shield için max_hp oranı (0.25).</summary>
        public float Value;
        /// <summary>Kaç tur sürer. -1 = tüketilene kadar (GuaranteedCrit).</summary>
        public int Duration;
    }
}
