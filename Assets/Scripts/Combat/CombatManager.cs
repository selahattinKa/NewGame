using System;
using System.Collections;
using UnityEngine;
using CanavarZindanlari.Data;
using System.Collections.Generic;

namespace CanavarZindanlari.Combat
{
    public enum CombatState { PlayerTurn, EnemyTurn, Victory, Defeat }

    public struct CombatActionResult
    {
        public int Damage;
        public int HealAmount;
        public bool IsCrit;
        public bool IsHeal;
        public CombatState NextState;
    }

    /// <summary>
    /// Turn-based combat state machine.
    /// Maps to design/gdd/hibrit-savas.md.
    /// Raises events; UI layer subscribes — no direct UI calls here.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Skills (0=Normal, 1=Heavy, 2=Heal)")]
        [SerializeField] private SkillData[] _playerSkills;

        public CombatUnit Player { get; private set; }
        public CombatUnit Enemy  { get; private set; }
        public CombatState State { get; private set; }
        public bool AutoBattle   { get; private set; }

        // Events for UI
        public event Action<CombatActionResult, bool> OnPlayerAction;  // result, isPlayerActor
        public event Action<CombatActionResult>        OnEnemyAction;
        public event Action<CombatState>               OnStateChanged;
        public event Action<BattleReward>              OnBattleEnded;

        private bool _waitingForInput;

        // ── Setup ──────────────────────────────────────────────────────────────

        public void StartCombat(CombatUnit player, CombatUnit enemy)
        {
            Player = player;
            Enemy  = enemy;
            State  = CombatState.PlayerTurn;
            AutoBattle = false;
            _waitingForInput = true;
            OnStateChanged?.Invoke(State);
        }

        public void SetAutoBattle(bool enabled)
        {
            AutoBattle = enabled;
            if (enabled && State == CombatState.PlayerTurn && _waitingForInput)
                StartCoroutine(AutoPlayerTurn());
        }

        // ── Player actions ─────────────────────────────────────────────────────

        /// <summary>Called by UI when player taps a skill button.</summary>
        public void PlayerUseSkill(int skillSlot)
        {
            if (State != CombatState.PlayerTurn || !_waitingForInput) return;
            if (!Player.SkillReady(skillSlot)) return;

            _waitingForInput = false;
            ExecutePlayerSkill(skillSlot);
        }

        private void ExecutePlayerSkill(int slot)
        {
            var skill = _playerSkills[slot];
            var result = new CombatActionResult();

            if (skill.Type == SkillType.Heal)
            {
                result.IsHeal = true;
                result.HealAmount = DamageCalculator.CalculateHeal(Player.MaxHP, skill);
                Player.Heal(result.HealAmount);
            }
            else
            {
                result.Damage = DamageCalculator.Calculate(
                    Player.ATK, Enemy.DEF,
                    Player.Element, Enemy.Element,
                    skill, out result.IsCrit);
                Enemy.TakeDamage(result.Damage);
            }

            Player.SetCooldown(slot, skill.CooldownTurns);
            OnPlayerAction?.Invoke(result, true);

            if (!Enemy.IsAlive)
            {
                StartCoroutine(EndCombat(CombatState.Victory));
                return;
            }

            StartCoroutine(EnemyTurnDelay());
        }

        // ── Enemy AI turn ──────────────────────────────────────────────────────

        private IEnumerator EnemyTurnDelay()
        {
            State = CombatState.EnemyTurn;
            OnStateChanged?.Invoke(State);
            yield return new WaitForSeconds(1.0f);
            ExecuteEnemyTurn();
        }

        private void ExecuteEnemyTurn()
        {
            // Simple AI: weighted random from dusuman-ai.md
            // Enemies always use Form A, no skills — basic attack only at prototype stage
            var attackSkill = _playerSkills[0]; // reuse normal attack data for enemy
            int enemyDamage = DamageCalculator.Calculate(
                Enemy.ATK, Player.DEF,
                Enemy.Element, Player.Element,
                attackSkill, out bool isCrit);
            var result = new CombatActionResult { Damage = enemyDamage, IsCrit = isCrit };
            Player.TakeDamage(result.Damage);
            Player.TickCooldowns();
            Enemy.TickCooldowns();

            OnEnemyAction?.Invoke(result);

            if (!Player.IsAlive)
            {
                StartCoroutine(EndCombat(CombatState.Defeat));
                return;
            }

            State = CombatState.PlayerTurn;
            _waitingForInput = true;
            OnStateChanged?.Invoke(State);

            if (AutoBattle)
                StartCoroutine(AutoPlayerTurn());
        }

        // ── Auto-battle AI ─────────────────────────────────────────────────────

        private IEnumerator AutoPlayerTurn()
        {
            yield return new WaitForSeconds(0.85f);
            if (State != CombatState.PlayerTurn || !_waitingForInput) yield break;

            // Smart auto: heal if HP < 40%, heavy if ready, else normal
            if (Player.HPPercent < 0.4f && Player.SkillReady(2))
                PlayerUseSkill(2);
            else if (Player.SkillReady(1))
                PlayerUseSkill(1);
            else
                PlayerUseSkill(0);
        }

        // ── End state ──────────────────────────────────────────────────────────

        private IEnumerator EndCombat(CombatState outcome)
        {
            yield return new WaitForSeconds(0.6f);
            State = outcome;
            OnStateChanged?.Invoke(State);

            var reward = GenerateReward(outcome == CombatState.Victory);
            OnBattleEnded?.Invoke(reward);
        }

        // ── Ödül üretimi ───────────────────────────────────────────────────────

        // Prototype loot havuzu — gerçek item sistemi gelince genişletilir
        private static readonly (string Name, Rarity Rarity)[] PrototypeLootPool =
        {
            ("Ejderha Pulu",    Rarity.B),
            ("Eski Kemik",      Rarity.F),
            ("Kristal Tozu",    Rarity.C),
            ("Gölge Özü",       Rarity.C),
            ("Demir Parçası",   Rarity.F),
            ("Ruh Taşı",        Rarity.B),
            ("Zehir Bezi",      Rarity.D),
        };

        private BattleReward GenerateReward(bool isVictory)
        {
            int exp = isVictory ? UnityEngine.Random.Range(200, 281) : 0;

            LootDrop[] items = Array.Empty<LootDrop>();
            if (isVictory)
            {
                int count = UnityEngine.Random.Range(1, 4); // 1-3 item
                var picked = new List<LootDrop>(count);
                var usedIndices = new HashSet<int>();

                while (picked.Count < count)
                {
                    int idx = UnityEngine.Random.Range(0, PrototypeLootPool.Length);
                    if (usedIndices.Add(idx))
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

            // First-clear: PlayerPrefs ile tek seferlik
            string clearKey = $"cleared_{Enemy?.DisplayName ?? "unknown"}";
            bool isFirstClear = isVictory && !PlayerPrefs.HasKey(clearKey);
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
    }
}
