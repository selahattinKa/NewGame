using System.Text;
using UnityEngine;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Prototype savaş HUD'u — IMGUI. Üretimde UI Toolkit ile değiştirilecek.
    /// Sınıf yetenekleri, aktif durum efektleri ve kalkan gösterir.
    /// </summary>
    public class BattleHUD : MonoBehaviour
    {
        private CombatManager _combat;
        private string _lastLog = "";
        private bool   _gameOver;

        private void Start()
        {
            _combat = Object.FindFirstObjectByType<CombatManager>();
            if (_combat == null) return;

            _combat.OnPlayerAction += (r, _) => LogAction("Sen", r);
            _combat.OnEnemyAction  += r       => LogAction(_combat.Enemy?.DisplayName ?? "Düşman", r);
            _combat.OnStateChanged += s =>
            {
                if (s == CombatState.Victory || s == CombatState.Defeat)
                    _gameOver = true;
            };
        }

        private void LogAction(string actor, CombatActionResult r)
        {
            if (r.IsStunSkipped) { _lastLog = $"{actor} sersemletildi — tur atlandı"; return; }
            if (r.DoTDamage > 0 && r.Damage == 0)
                { _lastLog = $"{actor} DoT: {r.DoTDamage} hasar"; return; }

            var sb = new StringBuilder();
            sb.Append(actor).Append(' ');
            if (r.IsHeal)
                sb.Append('+').Append(r.HealAmount).Append(" HP iyileşti");
            else
                sb.Append(r.Damage).Append(" hasar").Append(r.IsCrit ? " (KRİT!)" : "");

            if (r.HealAmount > 0 && !r.IsHeal)
                sb.Append(" +").Append(r.HealAmount).Append(" HP");
            if (!string.IsNullOrEmpty(r.EffectApplied))
                sb.Append(" [").Append(r.EffectApplied).Append(']');
            if (r.DoTDamage > 0)
                sb.Append(" (DoT:").Append(r.DoTDamage).Append(')');

            _lastLog = sb.ToString();
        }

        private void OnGUI()
        {
            if (_combat == null) return;

            float sw = Screen.width;
            float sh = Screen.height;

            GUI.skin.label.fontSize  = Mathf.Max(14, Mathf.RoundToInt(sh * 0.022f));
            GUI.skin.button.fontSize = Mathf.Max(14, Mathf.RoundToInt(sh * 0.025f));
            GUI.skin.box.fontSize    = Mathf.Max(14, Mathf.RoundToInt(sh * 0.020f));

            float pad  = sw * 0.04f;
            float barH = sh * 0.035f;

            DrawEnemyPanel(sw, sh, pad, barH);
            DrawLog(sw, sh);
            if (!_gameOver) DrawPlayerPanel(sw, sh, pad, barH);
            DrawSkillButtons(sw, sh, pad);
        }

        // ── Düşman paneli ─────────────────────────────────────────────────────

        private void DrawEnemyPanel(float sw, float sh, float pad, float barH)
        {
            var e = _combat.Enemy;
            if (e == null) return;

            float w = sw - pad * 2f;
            float y = sh * 0.05f;

            GUI.Label(new Rect(pad, y, w, barH * 1.4f),
                $"{e.DisplayName}  [{e.Archetype}]");

            DrawHPBar(pad, y + barH * 1.5f, w, barH,
                e.CurrentHP, e.MaxHP, e.ShieldHP, new Color(0.85f, 0.2f, 0.2f));

            DrawEffects(pad, y + barH * 3.2f, w, e);
        }

        // ── Oyuncu paneli ─────────────────────────────────────────────────────

        private void DrawPlayerPanel(float sw, float sh, float pad, float barH)
        {
            var p = _combat.Player;
            if (p == null) return;

            float w = sw * 0.55f;
            float y = sh * 0.68f;

            GUI.Label(new Rect(pad, y, w, barH * 1.4f), p.DisplayName);
            DrawHPBar(pad, y + barH * 1.5f, w, barH,
                p.CurrentHP, p.MaxHP, p.ShieldHP, new Color(0.2f, 0.75f, 0.3f));

            DrawEffects(pad, y + barH * 3.2f, w, p);

            float toggleH = barH * 1.6f;
            float toggleY = y + barH * 5f;
            string autoLabel = _combat.AutoBattle ? "Oto: AÇIK" : "Oto: KAPALI";
            if (GUI.Button(new Rect(pad, toggleY, w, toggleH), autoLabel))
                _combat.SetAutoBattle(!_combat.AutoBattle);
        }

        // ── Beceri butonları ──────────────────────────────────────────────────

        private void DrawSkillButtons(float sw, float sh, float pad)
        {
            if (_gameOver) return;
            if (_combat.State != CombatState.PlayerTurn) return;

            int count = _combat.SkillCount;
            if (count == 0) return;

            float totalW = sw - pad * 2f;
            float gap    = sw * 0.012f;
            float btnW   = (totalW - gap * (count - 1)) / count;
            float btnH   = sh * 0.10f;
            float by     = sh * 0.85f;

            bool auto = _combat.AutoBattle;

            for (int i = 0; i < count; i++)
            {
                var skill = _combat.GetPlayerSkill(i);
                if (skill == null) continue;

                bool ready = _combat.Player != null && _combat.Player.SkillReady(i);
                // Oto-savaştayken buton görünür ama soluk; basınca oto kapanır ve yetenek kullanılır
                GUI.enabled = ready;
                float bx = pad + i * (btnW + gap);

                string cdText = ready ? "" : $"\n(CD:{_combat.Player.SkillCooldowns[i]})";
                string label  = (skill.SkillName ?? $"Slot {i}") + cdText;
                if (GUI.Button(new Rect(bx, by, btnW, btnH), label))
                {
                    if (auto) _combat.SetAutoBattle(false);
                    _combat.PlayerUseSkill(i);
                }
            }
            GUI.enabled = true;
        }

        // ── Yeni savaş başlarken çağrılır (DungeonManager dalga geçişi) ─────────

        public void ResetForNewFight()
        {
            _gameOver = false;
            _lastLog  = "";
        }

        // ── Log ───────────────────────────────────────────────────────────────

        private void DrawLog(float sw, float sh)
        {
            if (string.IsNullOrEmpty(_lastLog)) return;
            float lw = sw * 0.80f;
            float lh = sh * 0.055f;
            GUI.Box(new Rect((sw - lw) * 0.5f, sh * 0.44f, lw, lh), _lastLog);
        }

        // ── Aktif efektler ────────────────────────────────────────────────────

        private static readonly StringBuilder _sb = new StringBuilder();

        private void DrawEffects(float x, float y, float w, CombatUnit unit)
        {
            if (unit.ActiveEffects.Count == 0 && unit.ShieldHP <= 0) return;

            _sb.Clear();
            if (unit.ShieldHP > 0) _sb.Append($"[Kalkan:{unit.ShieldHP}] ");
            foreach (var e in unit.ActiveEffects)
                if (!e.IsExpired)
                    _sb.Append($"[{EffectShortName(e.Type)}:{e.RemainingTurns}t] ");

            GUI.skin.label.fontSize = Mathf.Max(12, Mathf.RoundToInt(Screen.height * 0.018f));
            GUI.Label(new Rect(x, y, w, 28f), _sb.ToString());
            GUI.skin.label.fontSize = Mathf.Max(14, Mathf.RoundToInt(Screen.height * 0.022f));
        }

        private static string EffectShortName(StatusEffectType t) => t switch
        {
            StatusEffectType.BurnDoT         => "Yanma",
            StatusEffectType.PoisonDoT       => "Zehir",
            StatusEffectType.Stun            => "Sersem",
            StatusEffectType.DefMod          => "DEF×",
            StatusEffectType.AtkMod          => "ATK×",
            StatusEffectType.Shield          => "Kalkan",
            StatusEffectType.GuaranteedCrit  => "KesKrit",
            StatusEffectType.DamageReduction => "HsrAzlt",
            _                                => t.ToString(),
        };

        // ── HP çubuğu ─────────────────────────────────────────────────────────

        private static void DrawHPBar(float x, float y, float w, float h,
            int cur, int max, int shield, Color fill)
        {
            GUI.color = new Color(0.15f, 0.15f, 0.15f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            if (max > 0)
            {
                float pct = Mathf.Clamp01((float)cur / max);
                GUI.color = fill;
                GUI.DrawTexture(new Rect(x + 2f, y + 2f, (w - 4f) * pct, h - 4f), Texture2D.whiteTexture);
            }

            if (shield > 0)
            {
                GUI.color = new Color(0.4f, 0.7f, 1f, 0.6f);
                float sp = Mathf.Clamp01((float)shield / max);
                GUI.DrawTexture(new Rect(x + 2f, y + 2f, (w - 4f) * sp, (h - 4f) * 0.4f), Texture2D.whiteTexture);
            }

            GUI.color = Color.white;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(x, y, w, h), $"{cur} / {max}");
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
    }
}
