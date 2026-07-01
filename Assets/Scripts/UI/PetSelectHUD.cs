using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// Pet seçim + evrim ekranı — Hub'dan açılır, X ile Hub'a döner.
    /// </summary>
    public class PetSelectHUD : MonoBehaviour
    {
        private static readonly Color ColBg      = new Color(0.05f, 0.04f, 0.10f, 1.00f);
        private static readonly Color ColGold     = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColCard     = new Color(0.12f, 0.11f, 0.18f, 1.00f);
        private static readonly Color ColSel      = new Color(0.20f, 0.35f, 0.55f, 1.00f);
        private static readonly Color ColEvo      = new Color(0.55f, 0.25f, 0.75f, 1.00f);
        private static readonly Color ColEvoOff   = new Color(0.30f, 0.20f, 0.35f, 1.00f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleLabel;
        private GUIStyle _styleSmall;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnX;
        private bool     _stylesReady;

        private MonsterCollection _collection;
        private Vector2           _scroll;

        private string _evoMsg;
        private float  _evoMsgTime;

        private void Awake() =>
            _collection = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();

        private void OnGUI()
        {
            if (ScreenNavigator.Current != GameScreen.PetSelect) return;
            BuildStyles();

            float sw  = Screen.width;
            float sh  = Screen.height;
            float pad = sw * 0.05f;
            float w   = sw - pad * 2f;

            GUI.color = ColBg;
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            DrawCloseBtn(sw, sh);

            float y = sh * 0.06f;

            _styleTitle.normal.textColor = ColGold;
            GUI.Label(new Rect(pad, y, w * 0.80f, sh * 0.07f), "🐾 Pet Seç & Evrim", _styleTitle);
            y += sh * 0.09f;

            if (_collection == null || _collection.Monsters.Count == 0)
            {
                _styleLabel.normal.textColor = Color.gray;
                GUI.Label(new Rect(pad, y, w, sh * 0.07f),
                    "Henüz petın yok — zindanda canavar yakala!", _styleLabel);
                return;
            }

            // Aktif pet özeti
            var selPet = _collection.SelectedPet;
            if (selPet != null)
            {
                _styleSmall.normal.textColor = TierColor(selPet.Tier);
                GUI.Label(new Rect(pad, y, w, sh * 0.04f),
                    $"Aktif Pet: {selPet.DisplayName}  [{selPet.Tier}]", _styleSmall);
                y += sh * 0.045f;
            }

            // Evrim mesajı
            if (!string.IsNullOrEmpty(_evoMsg) && Time.realtimeSinceStartup < _evoMsgTime)
            {
                bool ok = _evoMsg.StartsWith("✓");
                _styleSmall.normal.textColor = ok ? new Color(0.3f, 0.9f, 0.3f) : new Color(1f, 0.4f, 0.3f);
                GUI.Label(new Rect(pad, y, w, sh * 0.04f), _evoMsg, _styleSmall);
                y += sh * 0.045f;
            }

            var   monsters = _collection.Monsters;
            string selId   = _collection.SelectedPetId;

            float rowH  = sh * 0.165f;
            float viewH = sh - y - sh * 0.03f;
            var   viewR = new Rect(pad, y, w, viewH);
            var   contR = new Rect(0, 0, w - 16f, monsters.Count * rowH);
            _scroll = GUI.BeginScrollView(viewR, _scroll, contR, false, false);

            for (int i = 0; i < monsters.Count; i++)
            {
                var  m   = monsters[i];
                float ry = i * rowH;
                bool isSel = m.InstanceId == selId;

                // Kart arka planı
                GUI.color = isSel ? ColSel : ColCard;
                GUI.DrawTexture(new Rect(0, ry, w - 16f, rowH - 5f), Texture2D.whiteTexture);
                GUI.color = Color.white;

                float ip  = w * 0.03f;
                float lh  = rowH * 0.22f;

                // Satır 1: Tier + isim
                _styleLabel.normal.textColor = TierColor(m.Tier);
                GUI.Label(new Rect(ip, ry + 4f, w * 0.70f, lh),
                    $"[{m.Tier}]  {m.DisplayName}", _styleLabel);

                // Satır 2: Bonus
                var (hpM, _) = MonsterCollection.BonusForTier(m.Tier);
                int bonusPct  = Mathf.RoundToInt((hpM - 1f) * 100);
                _styleSmall.normal.textColor = Color.gray;
                GUI.Label(new Rect(ip, ry + 4f + lh, w * 0.70f, lh),
                    $"Kat {m.FloorCaptured}  •  +{bonusPct}% HP & ATK", _styleSmall);

                // Satır 3: Max tier + fodder
                bool maxReached = m.Tier >= m.MaxEvolutionTier || m.Tier == Rarity.SS;
                if (maxReached)
                {
                    _styleSmall.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                    GUI.Label(new Rect(ip, ry + 4f + lh * 2, w * 0.70f, lh),
                        $"Max Tier: {m.MaxEvolutionTier}  ✓ Ulaşıldı", _styleSmall);
                }
                else
                {
                    var cost   = MonsterCollection.GetEvoCost(m.Tier);
                    int fodder = _collection.FodderAvailable(m);
                    int gold   = EconomyManager.Instance?.Gold ?? 0;
                    bool hasFodder = fodder >= cost.FodderCount;
                    bool hasGold   = gold   >= cost.Gold;

                    _styleSmall.normal.textColor = hasFodder && hasGold
                        ? new Color(0.75f, 0.55f, 1.0f)
                        : new Color(0.5f, 0.4f, 0.6f);
                    GUI.Label(new Rect(ip, ry + 4f + lh * 2, w * 0.70f, lh),
                        $"Evrim → {MonsterCollection.NextTier(m.Tier)}  " +
                        $"[{fodder}/{cost.FodderCount} pet  •  {cost.Gold:N0}🪙]  Max:{m.MaxEvolutionTier}",
                        _styleSmall);
                }

                GUI.color = Color.white;

                // Butonlar (sağ taraf)
                float bw = w * 0.28f;
                float bx = w - 16f - bw - ip;

                // Seç / Aktif butonu
                float by1 = ry + rowH * 0.08f;
                float bh1 = rowH * 0.36f;
                if (isSel)
                {
                    GUI.color = new Color(0.3f, 0.8f, 0.3f);
                    _styleBtn.normal.textColor = new Color(0.3f, 0.8f, 0.3f);
                    GUI.Label(new Rect(bx, by1, bw, bh1), "✓ Aktif", _styleBtn);
                    GUI.color = Color.white;
                }
                else
                {
                    if (GUI.Button(new Rect(bx, by1, bw, bh1), "Seç", _styleBtn))
                        _collection.SelectPet(m.InstanceId);
                }

                // Evrim butonu
                float by2 = ry + rowH * 0.55f;
                float bh2 = rowH * 0.36f;
                if (!maxReached)
                {
                    bool canEvo = _collection.CanEvolve(m);
                    GUI.color = canEvo ? ColEvo : ColEvoOff;
                    if (GUI.Button(new Rect(bx, by2, bw, bh2),
                        canEvo ? "⬆ Evrim" : "⬆ Evrim", _styleBtn) && canEvo)
                    {
                        var (ok, err) = _collection.TryEvolve(m);
                        _evoMsg     = ok ? $"✓ {m.DisplayName} [{m.Tier}] evrimlendi!" : $"✗ {err}";
                        _evoMsgTime = Time.realtimeSinceStartup + 3f;
                    }
                    GUI.color = Color.white;
                }
            }

            GUI.EndScrollView();
            GUI.color = Color.white;
        }

        private void DrawCloseBtn(float sw, float sh)
        {
            float bw = sw * 0.12f;
            float bh = sh * 0.055f;
            GUI.color = new Color(0.70f, 0.20f, 0.20f);
            if (GUI.Button(new Rect(sw - bw - 8f, 8f, bw, bh), "✕", _styleBtnX))
                ScreenNavigator.GoToHub();
            GUI.color = Color.white;
        }

        private static Color TierColor(Rarity tier) => tier switch
        {
            Rarity.SS => new Color(1.00f, 0.60f, 0.10f),
            Rarity.S  => new Color(0.90f, 0.30f, 0.90f),
            Rarity.A  => new Color(0.40f, 0.75f, 1.00f),
            Rarity.B  => new Color(0.30f, 0.90f, 0.50f),
            Rarity.C  => new Color(0.55f, 0.90f, 0.40f),
            Rarity.D  => new Color(0.95f, 0.80f, 0.30f),
            _         => new Color(0.80f, 0.78f, 0.80f),
        };

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            int fs = Mathf.Max(14, Screen.width / 22);

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs + 6,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleSmall = new GUIStyle(GUI.skin.label)
            {
                fontSize  = fs - 2,
                alignment = TextAnchor.MiddleLeft,
            };

            _styleBtn = new GUIStyle(GUI.skin.button)
            {
                fontSize  = fs - 1,
                fontStyle = FontStyle.Bold,
            };

            _styleBtnX = new GUIStyle(GUI.skin.button)
            {
                fontSize  = fs + 2,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
            };
        }
    }
}
