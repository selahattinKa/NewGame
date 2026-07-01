using UnityEngine;
using CanavarZindanlari.Combat;

namespace CanavarZindanlari.Data
{
    public enum SkillType { Attack, HeavyAttack, Heal } // geriye dönük uyumluluk için tutuldu

    public enum TargetType
    {
        SingleEnemy, // normal saldırı
        Self,        // kendine buff/iyileştirme
        AllEnemies,  // tüm düşmanlara (prototipte tek düşman var — aynı davranır)
    }

    /// <summary>
    /// Bir savaş becerisinin tüm parametrelerini tanımlar.
    /// Maps to design/gdd/oyuncu-sinif-sistemi.md Kural 3.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "CanavarZindanlari/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        public string SkillName;
        public SkillType Type;         // geriye dönük uyumluluk; CombatManager TargetType'ı kullanır
        public TargetType TargetType = TargetType.SingleEnemy;
        public int CooldownTurns;
        [TextArea] public string Description;

        [Header("Hasar")]
        [Range(0f, 5f)] public float DamageMultiplier = 1f;
        [Range(0f, 1f)] public float CritChance       = 0f;
        public float CritMultiplier = 2f;

        [Header("Çoklu Vuruş (Suikast Fırtınası vb.)")]
        public int   MultiHitCount      = 1;    // 1 = normal; 5 = Suikast Fırtınası
        [Range(0f, 1f)]
        public float MultiHitCritChance = 0f;   // MultiHitCount > 1 iken CritChance'ı geçersiz kılar

        [Header("İyileştirme")]
        [Range(0f, 1f)] public float HealPercent     = 0f; // saf iyileştirme (max_hp oranı)
        [Range(0f, 1f)] public float HealSelfPercent = 0f; // saldırı+iyileştirme kombosunda ek iyileşme

        [Header("Durum Etkileri — Hedefe Uygulanır")]
        public SkillEffect[] OnHitEffects  = System.Array.Empty<SkillEffect>();

        [Header("Durum Etkileri — Kendine Uygulanır")]
        public SkillEffect[] OnSelfEffects = System.Array.Empty<SkillEffect>();
    }
}
