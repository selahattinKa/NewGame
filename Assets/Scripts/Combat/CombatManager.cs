using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;

namespace CanavarZindanlari.Combat
{
    public enum CombatState { PlayerTurn, EnemyTurn, Victory, Defeat }

    public struct CombatActionResult
    {
        public int    Damage;
        public int    HealAmount;
        public int    DoTDamage;
        public bool   IsCrit;
        public bool   IsHeal;
        public bool   IsStunSkipped;
        public string EffectApplied;  // durum etkisi adı, varsa
    }

    /// <summary>
    /// Tur bazlı savaş durum makinesi.
    /// Maps to design/gdd/hibrit-savas.md ve oyuncu-sinif-sistemi.md.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        // ── Referanslar ───────────────────────────────────────────────────────

        private ClassData _playerClass;

        // Düşman için basit saldırı — prototype'ta düşman sınıfsız
        [Header("Düşman Becerisi (prototype — basic attack)")]
        [SerializeField] private SkillData _enemyAttackSkill;

        // ── Public state ──────────────────────────────────────────────────────

        public CombatUnit  Player     { get; private set; }
        public CombatUnit  Enemy      { get; private set; }
        public CombatState State      { get; private set; }
        public bool        AutoBattle { get; private set; }

        // ── Olaylar ───────────────────────────────────────────────────────────

        public event Action<CombatActionResult, bool> OnPlayerAction;
        public event Action<CombatActionResult>        OnEnemyAction;
        public event Action<CombatState>               OnStateChanged;
        public event Action<BattleReward>              OnBattleEnded;
        public event Action<int>                       OnPotUsed;    // healAmount

        private bool _waitingForInput;
        private int  _defReductionFactor;
        private int  _potCooldownTurns;

        public int PotCooldown => _potCooldownTurns; // sınıfın hasar türüne göre: 2=Fiziksel, 4=Büyü

        // ── Kurulum ───────────────────────────────────────────────────────────

        public void StartCombat(CombatUnit player, CombatUnit enemy, ClassData classData = null)
        {
            Player       = player;
            Enemy        = enemy;
            _playerClass = classData;
            _defReductionFactor = (classData?.DamageType == DamageType.Magic) ? 4 : 2;

            State              = CombatState.PlayerTurn;
            AutoBattle         = false;
            _waitingForInput   = true;
            _potCooldownTurns  = 0;
            OnStateChanged?.Invoke(State);
        }

        public SkillData GetPlayerSkill(int slot)
        {
            if (_playerClass != null && slot < _playerClass.Skills.Length)
                return _playerClass.Skills[slot];
            return null;
        }

        public int SkillCount => _playerClass?.Skills.Length ?? 0;

        public void SetAutoBattle(bool enabled)
        {
            AutoBattle = enabled;
            if (enabled && State == CombatState.PlayerTurn && _waitingForInput)
                StartCoroutine(AutoPlayerTurn());
        }

        // ── Oyuncu aksiyonu ───────────────────────────────────────────────────

        public void PlayerUseSkill(int slot)
        {
            if (State != CombatState.PlayerTurn || !_waitingForInput) return;
            if (!Player.SkillReady(slot)) return;

            var skill = GetPlayerSkill(slot);
            if (skill == null) return;

            _waitingForInput = false;
            StartCoroutine(ExecutePlayerTurn(slot, skill));
        }

        private IEnumerator ExecutePlayerTurn(int slot, SkillData skill)
        {
            var result = new CombatActionResult();

            // DoT ticking — oyuncunun kendi turunda tetiklenir
            int dotDmg = Player.TickDoT();
            result.DoTDamage = dotDmg;

            if (!Player.IsAlive)
            {
                yield return StartCoroutine(EndCombat(CombatState.Defeat));
                yield break;
            }

            // Stun kontrolü
            if (Player.IsStunned)
            {
                result.IsStunSkipped = true;
                Player.TickEffectDurations();
                OnPlayerAction?.Invoke(result, true);
                yield return StartCoroutine(EnemyTurnDelay());
                yield break;
            }

            // Beceri çözümleme
            switch (skill.TargetType)
            {
                case TargetType.Self:
                    ExecuteSelfSkill(skill, ref result);
                    break;

                case TargetType.AllEnemies:
                case TargetType.SingleEnemy:
                    ExecuteAttackSkill(skill, ref result);
                    break;
            }

            Player.SetCooldown(slot, skill.CooldownTurns);
            Player.TickEffectDurations();
            OnPlayerAction?.Invoke(result, true);

            if (!Enemy.IsAlive)
            {
                yield return StartCoroutine(EndCombat(CombatState.Victory));
                yield break;
            }

            yield return StartCoroutine(EnemyTurnDelay());
        }

        private void ExecuteAttackSkill(SkillData skill, ref CombatActionResult result)
        {
            int hitCount = Mathf.Max(1, skill.MultiHitCount);
            bool anyHit  = false;
            int totalDmg = 0;

            for (int i = 0; i < hitCount; i++)
            {
                int dmg;
                bool crit;

                if (hitCount > 1)
                {
                    dmg = DamageCalculator.CalculateOneHit(
                        Player.ATK, Enemy.DEF, _defReductionFactor, skill, out crit);
                }
                else
                {
                    bool gc = Player.HasGuaranteedCrit;
                    dmg = DamageCalculator.Calculate(
                        Player.ATK, Enemy.DEF, _defReductionFactor, skill, out crit, gc);
                    if (gc) Player.ConsumeGuaranteedCrit();
                }

                Enemy.TakeDamage(dmg);
                totalDmg += dmg;
                if (crit) result.IsCrit = true;
                if (!Enemy.IsAlive) break;
                anyHit = true;
            }

            result.Damage = totalDmg;

            // OnHitEffects — hedefe uygula
            if (anyHit || Enemy.IsAlive)
            {
                foreach (var fx in skill.OnHitEffects)
                {
                    Enemy.ApplyEffect(fx, Enemy.MaxHP);
                    result.EffectApplied = fx.Type.ToString();
                }
            }

            // HealSelfPercent — saldırı+iyileştirme kombinasyonu (Şifacı Slot 1)
            if (skill.HealSelfPercent > 0f)
            {
                int healAmt = DamageCalculator.CalculateHeal(Player.MaxHP, skill.HealSelfPercent);
                Player.Heal(healAmt);
                result.HealAmount = healAmt;
            }

            // OnSelfEffects
            foreach (var fx in skill.OnSelfEffects)
                Player.ApplyEffect(fx, Player.MaxHP);
        }

        private void ExecuteSelfSkill(SkillData skill, ref CombatActionResult result)
        {
            // Saf iyileştirme
            if (skill.HealPercent > 0f)
            {
                int healAmt = DamageCalculator.CalculateHeal(Player.MaxHP, skill.HealPercent);
                Player.Heal(healAmt);
                result.IsHeal     = true;
                result.HealAmount = healAmt;
            }

            // Kendine efekt uygula
            foreach (var fx in skill.OnSelfEffects)
            {
                Player.ApplyEffect(fx, Player.MaxHP);
                result.EffectApplied = fx.Type.ToString();
            }
        }

        // ── Düşman turu ───────────────────────────────────────────────────────

        private IEnumerator EnemyTurnDelay()
        {
            State = CombatState.EnemyTurn;
            OnStateChanged?.Invoke(State);
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(ExecuteEnemyTurn());
        }

        private IEnumerator ExecuteEnemyTurn()
        {
            var result = new CombatActionResult();

            // DoT ticking
            int dotDmg = Enemy.TickDoT();
            result.DoTDamage = dotDmg;

            if (!Enemy.IsAlive)
            {
                yield return StartCoroutine(EndCombat(CombatState.Victory));
                yield break;
            }

            // Stun kontrolü
            if (Enemy.IsStunned)
            {
                result.IsStunSkipped = true;
                Enemy.TickEffectDurations();
                OnEnemyAction?.Invoke(result);
                BeginPlayerTurn();
                yield break;
            }

            // Düşman saldırısı — prototype: basit basic attack, physical
            var attackSkill = _enemyAttackSkill ?? CreateFallbackAttack();
            int dmg = DamageCalculator.Calculate(
                Enemy.ATK, Player.DEF, 2, attackSkill, out bool isCrit);

            Player.TakeDamage(dmg);
            result.Damage = dmg;
            result.IsCrit = isCrit;

            Enemy.TickEffectDurations();
            Player.TickCooldowns();

            OnEnemyAction?.Invoke(result);

            if (!Player.IsAlive)
            {
                yield return StartCoroutine(EndCombat(CombatState.Defeat));
                yield break;
            }

            BeginPlayerTurn();
        }

        private void BeginPlayerTurn()
        {
            if (_potCooldownTurns > 0) _potCooldownTurns--;
            TryAutoPot();

            State            = CombatState.PlayerTurn;
            _waitingForInput = true;
            OnStateChanged?.Invoke(State);
            if (AutoBattle) StartCoroutine(AutoPlayerTurn());
        }

        private void TryAutoPot()
        {
            if (Player == null || !Player.IsAlive) return;
            if (Player.HPPercent >= 0.30f) return;
            if (_potCooldownTurns > 0) return;
            UsePot();
        }

        public bool UsePot()
        {
            if (_potCooldownTurns > 0) return false;
            if (!(EconomyManager.Instance?.SpendPot() ?? false)) return false;
            int healAmt = Mathf.FloorToInt(Player.MaxHP * 0.40f);
            Player.Heal(healAmt);
            _potCooldownTurns = 3;
            OnPotUsed?.Invoke(healAmt);
            return true;
        }

        // ── Oto-savaş ─────────────────────────────────────────────────────────

        private IEnumerator AutoPlayerTurn()
        {
            yield return new WaitForSeconds(0.85f);
            if (State != CombatState.PlayerTurn || !_waitingForInput) yield break;

            // İyileştirme — sadece HP %30 altındaysa
            bool lowHp = Player.HPPercent < 0.30f;
            if (lowHp)
            {
                for (int i = 0; i < SkillCount; i++)
                {
                    var s = GetPlayerSkill(i);
                    if (s != null && s.TargetType == TargetType.Self
                        && s.HealPercent > 0f && Player.SkillReady(i))
                    { PlayerUseSkill(i); yield break; }
                }
            }

            // En yüksek cooldown'lu hazır saldırı becerisini kullan
            // HP yeterliyken iyileştirme becerilerini atla
            int best = -1;
            for (int i = SkillCount - 1; i >= 0; i--)
            {
                if (!Player.SkillReady(i)) continue;
                var s = GetPlayerSkill(i);
                if (s == null) continue;
                bool isHeal = s.TargetType == TargetType.Self && s.HealPercent > 0f;
                if (isHeal && !lowHp) continue;
                best = i;
                break;
            }
            // Sadece iyileştirme kaldıysa yine de kullan
            if (best < 0)
                for (int i = 0; i < SkillCount; i++)
                    if (Player.SkillReady(i)) { best = i; break; }
            if (best < 0) best = 0;
            PlayerUseSkill(best);
        }

        // ── Savaş sonu ────────────────────────────────────────────────────────

        private IEnumerator EndCombat(CombatState outcome)
        {
            yield return new WaitForSeconds(0.6f);
            State = outcome;
            OnStateChanged?.Invoke(State);
            OnBattleEnded?.Invoke(GenerateReward(outcome == CombatState.Victory));
        }

        private BattleReward GenerateReward(bool isVictory)
        {
            int exp = isVictory ? UnityEngine.Random.Range(200, 281) : 0;

            var items = System.Array.Empty<LootDrop>();
            if (isVictory)
            {
                int count  = UnityEngine.Random.Range(1, 4);
                var picked = new List<LootDrop>(count);
                var used   = new System.Collections.Generic.HashSet<int>();
                while (picked.Count < count)
                {
                    int idx = UnityEngine.Random.Range(0, PrototypeLootPool.Length);
                    if (used.Add(idx))
                    {
                        var (name, rarity) = PrototypeLootPool[idx];
                        picked.Add(new LootDrop
                        {
                            ItemName = name,
                            Quantity = UnityEngine.Random.Range(1, 4),
                            Rarity   = rarity,
                            Icon     = null,
                        });
                    }
                }
                items = picked.ToArray();
            }

            string clearKey    = $"cleared_{Enemy?.DisplayName ?? "unknown"}";
            bool   isFirstClear = isVictory && !PlayerPrefs.HasKey(clearKey);
            if (isFirstClear) PlayerPrefs.SetInt(clearKey, 1);

            return new BattleReward
            {
                IsVictory    = isVictory,
                ExpGained    = exp,
                Items        = items,
                IsFirstClear = isFirstClear,
                BonusGems    = isFirstClear ? 50 : 0,
            };
        }

        private static readonly (string Name, Rarity Rarity)[] PrototypeLootPool =
        {
            ("Ejderha Pulu",  Rarity.B), ("Eski Kemik",    Rarity.F),
            ("Kristal Tozu",  Rarity.C), ("Gölge Özü",     Rarity.C),
            ("Demir Parçası", Rarity.F), ("Ruh Taşı",      Rarity.B),
            ("Zehir Bezi",    Rarity.D),
        };

        private static SkillData CreateFallbackAttack()
        {
            var s = ScriptableObject.CreateInstance<SkillData>();
            s.SkillName        = "Saldırı";
            s.DamageMultiplier = 1f;
            s.CritChance       = 0.1f;
            s.CritMultiplier   = 2f;
            s.MultiHitCount    = 1;
            return s;
        }
    }
}
