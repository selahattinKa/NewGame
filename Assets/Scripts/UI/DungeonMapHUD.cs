using UnityEngine;
using CanavarZindanlari.Core;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// IMGUI tabanlı zindan haritası ve kat geçiş ekranları.
    /// GDD: design/gdd/zindan-kesif.md — UI Requirements
    /// </summary>
    [RequireComponent(typeof(DungeonManager))]
    public class DungeonMapHUD : MonoBehaviour
    {
        private DungeonManager _dungeon;

        // Renkler
        private static readonly Color ColLocked    = new Color(0.35f, 0.32f, 0.38f);
        private static readonly Color ColUnlocked  = new Color(0.25f, 0.50f, 0.88f);
        private static readonly Color ColCleared   = new Color(0.25f, 0.75f, 0.35f);
        private static readonly Color ColBoss      = new Color(0.90f, 0.35f, 0.20f);
        private static readonly Color ColBossClr   = new Color(0.75f, 0.50f, 0.10f);
        private static readonly Color ColBg        = new Color(0.05f, 0.04f, 0.10f, 0.97f);
        private static readonly Color ColGold      = new Color(0.86f, 0.72f, 0.31f);
        private static readonly Color ColWave      = new Color(0.50f, 0.85f, 1.00f);

        private GUIStyle _styleTitle;
        private GUIStyle _styleBtn;
        private GUIStyle _styleBtnDisabled;
        private GUIStyle _styleLabel;
        private GUIStyle _styleResult;
        private GUIStyle _styleWave;
        private bool     _stylesReady;

        private void Awake() => _dungeon = GetComponent<DungeonManager>();

        private void BuildStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _styleTitle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = ColGold },
            };

            _styleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 14,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white },
                wordWrap  = true,
            };

            _styleWave = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = ColWave },
            };

            _styleResult = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap  = true,
            };

            _styleBtn = new GUIStyle(GUI.skin.button)
            {
                fontSize  = 14,
                fontStyle = FontStyle.Bold,
                normal    = { textColor = Color.white },
                hover     = { textColor = ColGold },
            };

            _styleBtnDisabled = new GUIStyle(_styleBtn)
            {
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
            };
        }

        private void OnGUI()
        {
            if (_dungeon == null) return;
            BuildStyles();

            switch (_dungeon.State)
            {
                case DungeonState.MapView:
                    DrawMapView();
                    break;

                case DungeonState.InWaveCombat:
                case DungeonState.WaveTransition:
                    DrawWaveIndicator();
                    break;

                case DungeonState.FloorCleared:
                    DrawFloorCleared();
                    break;

                case DungeonState.FloorFailed:
                    DrawFloorFailed();
                    break;
            }
        }

        // ── Zindan Haritası ───────────────────────────────────────────────────

        private void DrawMapView()
        {
            var screen = new Rect(0, 0, Screen.width, Screen.height);
            DrawBg(screen);

            float pad     = Screen.width * 0.04f;
            float w       = Screen.width  - pad * 2;
            float h       = Screen.height - pad * 2;
            float contentY = pad;

            // Başlık
            GUI.Label(new Rect(pad, contentY, w, 36), "Karanlık Orman Zindanı", _styleTitle);
            contentY += 42;

            // Enerji barı
            DrawEnergyBar(pad, contentY, w, 28);
            contentY += 36;

            // Kat ızgarası — 2 sütun × 5 satır
            float cellW   = (w - 8) * 0.5f;
            float cellH   = (h - contentY - 110) / 5f;
            cellH = Mathf.Clamp(cellH, 44, 72);

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    int floor = row * 2 + col + 1;
                    if (floor > DungeonManager.TotalFloors) break;

                    float x = pad + col * (cellW + 8);
                    float y = contentY + row * (cellH + 6);
                    DrawFloorButton(new Rect(x, y, cellW, cellH), floor);
                }
            }

            contentY += 5 * (cellH + 6) + 8;

            // İlerleme sıfırla butonu (test kolaylığı)
            if (GUI.Button(new Rect(pad, contentY, w, 36), "İlerlemeyi Sıfırla (Test)", _styleBtn))
                _dungeon.ResetProgress();
        }

        private void DrawFloorButton(Rect r, int floor)
        {
            var status = _dungeon.Floors[floor];
            bool isBoss = DungeonManager.IsBossFloor(floor);

            Color btnColor = status.State switch
            {
                FloorState.Cleared  => isBoss ? ColBossClr : ColCleared,
                FloorState.Unlocked => isBoss ? ColBoss    : ColUnlocked,
                _                   => ColLocked,
            };

            bool locked = status.State == FloorState.Locked;
            bool noEnergy = _dungeon.Energy < DungeonManager.EnergyPerFloor;

            GUI.color = btnColor;
            string label = BuildFloorLabel(floor, status, isBoss, locked, noEnergy);

            GUIStyle style = (locked || noEnergy || _dungeon.PlayerClass == null)
                ? _styleBtnDisabled
                : _styleBtn;

            if (GUI.Button(r, label, style) && !locked && _dungeon.State == DungeonState.MapView)
                _dungeon.EnterFloor(floor);

            GUI.color = Color.white;
        }

        private string BuildFloorLabel(int floor, FloorStatus status, bool isBoss, bool locked, bool noEnergy)
        {
            string icon = locked ? "🔒" : isBoss ? "👑" : status.State == FloorState.Cleared ? "✓" : "▶";
            string badge = status.FirstCleared ? "" : " ★";
            string suffix = isBoss ? "\n[BOSS]" : "";

            if (locked) return $"{icon} Kat {floor}{suffix}";
            if (noEnergy) return $"{icon} Kat {floor}{badge}{suffix}\n(Enerji yetersiz)";

            string stateText = status.State == FloorState.Cleared ? " Temizlendi" : " Gir";
            return $"{icon} Kat {floor}{badge}{suffix}{stateText}";
        }

        private void DrawEnergyBar(float x, float y, float w, float h)
        {
            float ratio  = (float)_dungeon.Energy / DungeonManager.EnergyMax;
            GUI.color = new Color(0.20f, 0.20f, 0.30f);
            GUI.Box(new Rect(x, y, w, h), "");
            GUI.color = new Color(0.20f, 0.65f, 0.90f);
            GUI.Box(new Rect(x, y, w * ratio, h), "");
            GUI.color = Color.white;
            GUI.Label(
                new Rect(x, y, w, h),
                $"Enerji: {_dungeon.Energy} / {DungeonManager.EnergyMax}  (Giriş: 2 enerji)",
                new GUIStyle(_styleLabel) { fontSize = 13 }
            );
        }

        // ── Dalga göstergesi (savaş sırasında) ───────────────────────────────

        private void DrawWaveIndicator()
        {
            float barH = 44;
            GUI.color = new Color(0.04f, 0.04f, 0.12f, 0.92f);
            GUI.Box(new Rect(0, 0, Screen.width, barH), "");
            GUI.color = Color.white;

            bool isBoss = DungeonManager.IsBossFloor(_dungeon.CurrentFloor);
            string floorTag = isBoss ? "[BOSS] " : "";
            string waveTxt = _dungeon.State == DungeonState.WaveTransition
                ? "Sonraki dalga yükleniyor..."
                : $"Dalga {_dungeon.CurrentWave} / {_dungeon.TotalWaves}";

            GUI.Label(
                new Rect(0, 0, Screen.width, barH),
                $"{floorTag}Kat {_dungeon.CurrentFloor} — {waveTxt}",
                _styleWave
            );
        }

        // ── Kat temizlendi ────────────────────────────────────────────────────

        private void DrawFloorCleared()
        {
            float pw = Screen.width  * 0.80f;
            float ph = Screen.height * 0.52f;
            float px = (Screen.width  - pw) * 0.5f;
            float py = (Screen.height - ph) * 0.5f;

            DrawBg(new Rect(px - 8, py - 8, pw + 16, ph + 16));

            bool  firstClear = _dungeon.LastClearWasFirstTime;
            int   floor      = _dungeon.CurrentFloor;
            bool  isBoss     = DungeonManager.IsBossFloor(floor);

            // Başlık
            _styleResult.normal.textColor = isBoss ? ColBoss : ColCleared;
            string header = isBoss ? "BOSS YENİLDİ!" : "KAT TEMİZLENDİ!";
            GUI.Label(new Rect(px, py, pw, 40), header, _styleResult);
            py += 44;

            // First-clear ödülü
            if (firstClear)
            {
                int gems = _dungeon.GetFirstClearGems(floor);
                _styleLabel.normal.textColor = ColGold;
                GUI.Label(new Rect(px, py, pw, 32),
                    $"İLK TEMİZLEME! +{gems} 💎", _styleLabel);
                _styleLabel.normal.textColor = Color.white;
                py += 36;
            }

            // Enerji bilgisi
            GUI.Label(new Rect(px, py, pw, 28),
                $"-2 Enerji (Kalan: {_dungeon.Energy})", _styleLabel);
            py += 32;

            // Full-heal bildirimi
            GUI.Label(new Rect(px, py, pw, 28),
                "Takım tam HP'ye iyileşti.", _styleLabel);
            py += 36;

            float btnW = pw * 0.44f;
            float btnH = 48;
            float btnY = py + 4;

            bool hasNext = floor < DungeonManager.TotalFloors;

            // Devam butonu için durum kontrolü (State == FloorCleared'da CanEnterFloor false döner)
            bool nextUnlocked = hasNext
                && _dungeon.Floors[floor + 1].State != FloorState.Locked
                && _dungeon.Energy >= DungeonManager.EnergyPerFloor;

            // "Devam" — sonraki kat
            if (hasNext && nextUnlocked)
            {
                if (GUI.Button(new Rect(px, btnY, btnW, btnH),
                    $"Devam (Kat {floor + 1})", _styleBtn))
                {
                    _dungeon.ReturnToMap();
                    _dungeon.EnterFloor(floor + 1);
                    return;
                }
            }
            else if (!hasNext)
            {
                _styleLabel.normal.textColor = ColGold;
                GUI.Label(new Rect(px, btnY, pw, btnH),
                    "Tüm katları temizlediniz!\nYeni bölge yakında...", _styleLabel);
                _styleLabel.normal.textColor = Color.white;
                btnY += btnH + 6;
                if (GUI.Button(new Rect(px, btnY, pw, btnH), "Haritaya Dön", _styleBtn))
                    _dungeon.ReturnToMap();
                return;
            }

            // "Haritaya Dön" butonu
            if (GUI.Button(new Rect(px + pw - btnW, btnY, btnW, btnH),
                "Haritaya Dön", _styleBtn))
                _dungeon.ReturnToMap();
        }

        // ── Kat başarısız ─────────────────────────────────────────────────────

        private void DrawFloorFailed()
        {
            float pw = Screen.width  * 0.75f;
            float ph = Screen.height * 0.42f;
            float px = (Screen.width  - pw) * 0.5f;
            float py = (Screen.height - ph) * 0.5f;

            DrawBg(new Rect(px - 8, py - 8, pw + 16, ph + 16));

            _styleResult.normal.textColor = new Color(0.90f, 0.30f, 0.30f);
            GUI.Label(new Rect(px, py, pw, 40), "KAT BAŞARISIZ", _styleResult);
            py += 46;

            _styleLabel.normal.textColor = new Color(0.80f, 0.90f, 0.80f);
            GUI.Label(new Rect(px, py, pw, 32), "Enerji harcanmadı.", _styleLabel);
            py += 36;

            _styleLabel.normal.textColor = Color.white;
            GUI.Label(new Rect(px, py, pw, 32),
                "Sınıf ve yeteneklerin hakkında iyi düşün!", _styleLabel);
            py += 40;

            float btnW = pw * 0.44f;
            float btnH = 48;

            if (GUI.Button(new Rect(px, py, btnW, btnH), "Tekrar Dene", _styleBtn))
            {
                _dungeon.ReturnToMap();
                _dungeon.EnterFloor(_dungeon.CurrentFloor);
                return;
            }

            if (GUI.Button(new Rect(px + pw - btnW, py, btnW, btnH),
                "Haritaya Dön", _styleBtn))
                _dungeon.ReturnToMap();
        }

        // ── Yardımcı ──────────────────────────────────────────────────────────

        private static void DrawBg(Rect r)
        {
            GUI.color = ColBg;
            GUI.Box(r, "");
            GUI.color = Color.white;
        }
    }
}
