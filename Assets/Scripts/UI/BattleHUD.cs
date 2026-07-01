using UnityEngine;
using CanavarZindanlari.Combat;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Prototype savaş HUD'u — IMGUI. Üretimde UI Toolkit ile değiştirilecek.
    /// Portrait 1080×1920 baz alınır; boyutlar ekrana oranla ölçeklenir.
    /// </summary>
    public class BattleHUD : MonoBehaviour
    {
        private CombatManager _combat;
        private string _lastLog = "";
        private string _overlay = "";
        private bool   _gameOver;

        private void Start()
        {
            _combat = FindObjectOfType<CombatManager>();
            if (_combat == null) return;

            _combat.OnPlayerAction += (r, _) => LogAction("Sen", r);
            _combat.OnEnemyAction  += r       => LogAction(_combat.Enemy?.DisplayName ?? "Düşman", r);
            _combat.OnStateChanged += s =>
            {
                if (s == CombatState.Victory) { _overlay = "ZAFER!";    _gameOver = true; }
                if (s == CombatState.Defeat)  { _overlay = "YENİLDİN!"; _gameOver = true; }
            };
        }

        private void LogAction(string actor, CombatActionResult r)
        {
            _lastLog = r.IsHeal
                ? $"{actor} +{r.HealAmount} HP iyileşti"
                : $"{actor} {r.Damage} hasar verdi{(r.IsCrit ? " (KRİT!)" : "")}";
        }

        private void OnGUI()
        {
            if (_combat == null) return;

            float sw = Screen.width;
            float sh = Screen.height;

            // Tüm font boyutları ekrana oranla
            int labelFont  = Mathf.Max(14, Mathf.RoundToInt(sh * 0.022f));
            int btnFont    = Mathf.Max(14, Mathf.RoundToInt(sh * 0.025f));
            GUI.skin.label.fontSize  = labelFont;
            GUI.skin.button.fontSize = btnFont;
            GUI.skin.box.fontSize    = labelFont;

            float pad = sw * 0.04f;   // kenar boşluğu
            float barH = sh * 0.035f; // HP çubuğu yüksekliği

            DrawEnemyPanel(sw, sh, pad, barH);
            DrawLog(sw, sh);
            DrawPlayerPanel(sw, sh, pad, barH);
            DrawSkillButtons(sw, sh, pad);

            if (_gameOver) DrawOverlay(sw, sh);
        }

        // ── Düşman paneli — ekranın üst %22'si ──────────────────────────────

        private void DrawEnemyPanel(float sw, float sh, float pad, float barH)
        {
            var e = _combat.Enemy;
            if (e == null) return;

            float panelW = sw - pad * 2f;
            float y = sh * 0.05f;

            GUI.Label(new Rect(pad, y, panelW, barH * 1.4f),
                $"{e.DisplayName}  [{e.Archetype}]");

            DrawHPBar(pad, y + barH * 1.5f, panelW, barH,
                e.CurrentHP, e.MaxHP, new Color(0.85f, 0.2f, 0.2f));
        }

        // ── Son eylem logu — orta ────────────────────────────────────────────

        private void DrawLog(float sw, float sh)
        {
            if (string.IsNullOrEmpty(_lastLog)) return;
            float lw = sw * 0.80f;
            float lh = sh * 0.055f;
            GUI.Box(new Rect((sw - lw) * 0.5f, sh * 0.44f, lw, lh), _lastLog);
        }

        // ── Oyuncu paneli — %68'den %79'a ────────────────────────────────────

        private void DrawPlayerPanel(float sw, float sh, float pad, float barH)
        {
            var p = _combat.Player;
            if (p == null) return;

            float panelW = sw * 0.55f; // sol yarıda kalır, buton alanıyla çakışmaz
            float y = sh * 0.68f;

            GUI.Label(new Rect(pad, y, panelW, barH * 1.4f), p.DisplayName);
            DrawHPBar(pad, y + barH * 1.5f, panelW, barH,
                p.CurrentHP, p.MaxHP, new Color(0.2f, 0.75f, 0.3f));

            // Oto-savaş toggle — HP çubuğunun hemen altında
            float toggleW = panelW;
            float toggleH = barH * 1.6f;
            float toggleY = y + barH * 3.2f;
            string autoLabel = _combat.AutoBattle ? "Oto: AÇIK" : "Oto: KAPALI";
            if (GUI.Button(new Rect(pad, toggleY, toggleW, toggleH), autoLabel))
                _combat.SetAutoBattle(!_combat.AutoBattle);
        }

        // ── Beceri butonları — %84'ten %94'e, tam genişlik ───────────────────

        private static readonly string[] SkillLabels = { "Normal\nSaldırı", "Ağır\nSaldırı", "İyileşme" };

        private void DrawSkillButtons(float sw, float sh, float pad)
        {
            if (_gameOver) return;
            if (_combat.State != CombatState.PlayerTurn) return;
            if (_combat.AutoBattle) return;

            float totalW = sw - pad * 2f;
            float gap    = sw * 0.015f;
            float btnW   = (totalW - gap * 2f) / 3f; // 3 eşit buton, aralarında boşluk
            float btnH   = sh * 0.10f;
            float by     = sh * 0.84f;

            for (int i = 0; i < 3; i++)
            {
                bool ready = _combat.Player != null && _combat.Player.SkillReady(i);
                GUI.enabled = ready;
                float bx = pad + i * (btnW + gap);
                string label = SkillLabels[i] + (ready ? "" : "\n(bekle)");
                if (GUI.Button(new Rect(bx, by, btnW, btnH), label))
                    _combat.PlayerUseSkill(i);
            }
            GUI.enabled = true;
        }

        // ── Zafer / Yenildi overlay ───────────────────────────────────────────

        private void DrawOverlay(float sw, float sh)
        {
            // Yarı şeffaf arka plan
            var darkTex = Texture2D.blackTexture;
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(0f, 0f, sw, sh), darkTex);
            GUI.color = Color.white;

            int bigFont = Mathf.RoundToInt(sh * 0.075f);
            GUI.skin.label.fontSize = bigFont;

            float ow = sw * 0.8f;
            float oh = sh * 0.14f;
            GUI.Label(new Rect((sw - ow) * 0.5f, sh * 0.36f, ow, oh), _overlay);
            GUI.skin.label.fontSize = Mathf.RoundToInt(sh * 0.022f);

            float rw = sw * 0.50f;
            float rh = sh * 0.08f;
            if (GUI.Button(new Rect((sw - rw) * 0.5f, sh * 0.54f, rw, rh), "Yeniden Oyna"))
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        // ── Yardımcı: HP çubuğu ───────────────────────────────────────────────

        private static void DrawHPBar(float x, float y, float w, float h, int cur, int max, Color fill)
        {
            GUI.color = new Color(0.15f, 0.15f, 0.15f);
            GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

            if (max > 0)
            {
                float pct = Mathf.Clamp01((float)cur / max);
                GUI.color = fill;
                GUI.DrawTexture(new Rect(x + 2f, y + 2f, (w - 4f) * pct, h - 4f), Texture2D.whiteTexture);
            }

            GUI.color = Color.white;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(x, y, w, h), $"{cur} / {max}");
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
        }
    }
}
