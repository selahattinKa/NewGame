using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.UI
{
    /// <summary>
    /// IMGUI tabanlı zindan haritası ve kat geçiş ekranları.
    /// GDD: design/gdd/zindan-kesif.md — UI Requirements
    /// </summary>
    [RequireComponent(typeof(DungeonManager))]
    public class DungeonMapHUD : MonoBehaviour
    {
        private DungeonManager    _dungeon;
        private MonsterCollection _collection;
        private bool              _showCollection;
        private Vector2           _mapScroll;

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

        private void Awake()
        {
            _dungeon    = GetComponent<DungeonManager>();
            _collection = UnityEngine.Object.FindFirstObjectByType<MonsterCollection>();
        }

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

            if (_showCollection && _dungeon.State == DungeonState.MapView)
                DrawCollectionPanel();
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

            // Kat ızgarası — 2 sütun × 10 satır, scroll edilebilir
            float cellW    = (w - 8) * 0.5f;
            float cellH    = 56f;
            float cellGap  = 5f;
            int   rows     = DungeonManager.TotalFloors / 2;
            float gridH    = rows * (cellH + cellGap);
            float viewH    = h - contentY - 80f;

            var viewRect = new Rect(pad, contentY, w, viewH);
            var contRect = new Rect(0, 0, w - 16, gridH);
            _mapScroll = GUI.BeginScrollView(viewRect, _mapScroll, contRect, false, false);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    int floor = row * 2 + col + 1;
                    if (floor > DungeonManager.TotalFloors) break;

                    float x = col * (cellW + 8);
                    float y = row * (cellH + cellGap);
                    DrawFloorButton(new Rect(x, y, cellW, cellH), floor);
                }
            }

            GUI.EndScrollView();
            contentY += viewH + 8;

            // Koleksiyon özeti
            int count = _collection != null ? _collection.Monsters.Count : 0;
            GUI.Label(new Rect(pad, contentY, w, 26),
                $"Koleksiyon: {count} canavar",
                new GUIStyle(_styleLabel) { fontSize = 13, normal = { textColor = ColWave } });
            contentY += 28;

            // İlerleme sıfırla butonu (test kolaylığı)
            if (GUI.Button(new Rect(pad, contentY, w * 0.48f, 36), "İlerlemeyi Sıfırla (Test)", _styleBtn))
                _dungeon.ResetProgress();

            if (GUI.Button(new Rect(pad + w * 0.52f, contentY, w * 0.48f, 36), "Koleksiyonu Göster (Test)", _styleBtn))
                _showCollection = !_showCollection;
        }

        private void DrawFloorButton(Rect r, int floor)
        {
            var      status   = _dungeon.Floors[floor];
            FloorType ft      = DungeonManager.GetFloorType(floor);
            bool     special  = ft != FloorType.Normal;

            Color btnColor = status.State switch
            {
                FloorState.Cleared  => special ? ColBossClr : ColCleared,
                FloorState.Unlocked => special ? ColBoss    : ColUnlocked,
                _                   => ColLocked,
            };

            bool locked   = status.State == FloorState.Locked;
            bool noEnergy = _dungeon.Energy < DungeonManager.EnergyPerFloor;

            GUI.color = btnColor;
            string label = BuildFloorLabel(floor, status, ft, locked, noEnergy);

            GUIStyle style = (locked || noEnergy || _dungeon.PlayerClass == null)
                ? _styleBtnDisabled
                : _styleBtn;

            if (GUI.Button(r, label, style) && !locked && _dungeon.State == DungeonState.MapView)
                _dungeon.EnterFloor(floor);

            GUI.color = Color.white;
        }

        private static string FloorTypeLabel(FloorType ft) => ft switch
        {
            FloorType.MainBoss  => "👑 ANA BOSS",
            FloorType.Boss      => "👑 BOSS",
            FloorType.Champion  => "⚔ Şampiyon",
            _                   => "",
        };

        private static string FloorTypeIcon(FloorType ft) => ft switch
        {
            FloorType.MainBoss  => "👑",
            FloorType.Boss      => "👑",
            FloorType.Champion  => "⚔",
            _                   => "▶",
        };

        private string BuildFloorLabel(int floor, FloorStatus status, FloorType ft, bool locked, bool noEnergy)
        {
            string icon  = locked ? "🔒" : status.State == FloorState.Cleared ? "✓" : FloorTypeIcon(ft);
            string badge = (!status.FirstCleared && status.State != FloorState.Locked) ? " ★" : "";
            string type  = ft != FloorType.Normal ? $"  {FloorTypeLabel(ft)}" : "";

            if (locked)   return $"{icon} Kat {floor}{type}";
            if (noEnergy) return $"{icon} Kat {floor}{badge}{type}\n(Enerji yetersiz)";

            string action = status.State == FloorState.Cleared ? " Tekrar" : " Gir";
            return $"{icon} Kat {floor}{badge}{type}{action}";
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

            string floorTag = DungeonManager.GetFloorType(_dungeon.CurrentFloor) switch
            {
                FloorType.MainBoss => "[ANA BOSS] ",
                FloorType.Boss     => "[BOSS] ",
                FloorType.Champion => "[ŞAMPİYON] ",
                _                  => "",
            };
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

            bool      firstClear = _dungeon.LastClearWasFirstTime;
            int       floor      = _dungeon.CurrentFloor;
            FloorType ft         = DungeonManager.GetFloorType(floor);
            bool      isSpecial  = ft != FloorType.Normal;

            // Başlık
            _styleResult.normal.textColor = isSpecial ? ColBoss : ColCleared;
            string header = ft switch
            {
                FloorType.MainBoss => "ANA BOSS YENİLDİ!",
                FloorType.Boss     => "BOSS YENİLDİ!",
                FloorType.Champion => "ŞAMPİYON YENİLDİ!",
                _                  => "KAT TEMİZLENDİ!",
            };
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
                "HP tam yenilendi.", _styleLabel);
            py += 32;

            // Yakalanan canavar
            var captured = _dungeon.LastCaptured;
            if (captured != null)
            {
                Color tierColor = TierColor(captured.Tier);
                GUI.color = tierColor;
                GUI.Label(new Rect(px, py, pw, 36),
                    $"🎉 Canavar Yakalandı!  [{captured.Tier}]  {captured.DisplayName}",
                    new GUIStyle(_styleLabel) { fontSize = 15, fontStyle = FontStyle.Bold });
                GUI.color = Color.white;
            }
            else
            {
                _styleLabel.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                GUI.Label(new Rect(px, py, pw, 28), "Canavar kaçtı...", _styleLabel);
                _styleLabel.normal.textColor = Color.white;
            }
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

        // ── Koleksiyon paneli ─────────────────────────────────────────────────

        private Vector2 _collScroll;

        private void DrawCollectionPanel()
        {
            if (_collection == null) return;

            float pw = Screen.width  * 0.88f;
            float ph = Screen.height * 0.72f;
            float px = (Screen.width  - pw) * 0.5f;
            float py = (Screen.height - ph) * 0.5f;

            DrawBg(new Rect(px - 6, py - 6, pw + 12, ph + 12));

            GUI.Label(new Rect(px, py, pw, 36), "Canavar Koleksiyonu", _styleTitle);
            py += 40;

            var monsters = _collection.Monsters;
            if (monsters.Count == 0)
            {
                GUI.Label(new Rect(px, py, pw, 40), "Henüz canavar yok — zindanda savaş!", _styleLabel);
            }
            else
            {
                float rowH   = 30f;
                float viewH  = ph - 80f;
                var   viewRect  = new Rect(px, py, pw, viewH);
                var   contRect  = new Rect(0, 0, pw - 20, monsters.Count * rowH);

                _collScroll = GUI.BeginScrollView(viewRect, _collScroll, contRect);
                for (int i = 0; i < monsters.Count; i++)
                {
                    var m = monsters[i];
                    GUI.color = TierColor(m.Tier);
                    GUI.Label(new Rect(4, i * rowH, pw - 24, rowH),
                        $"[{m.Tier}]  {m.DisplayName}   Kat {m.FloorCaptured}   {m.CaptureDate}",
                        _styleLabel);
                }
                GUI.color = Color.white;
                GUI.EndScrollView();
                py += viewH + 4;
            }

            if (GUI.Button(new Rect(px + pw * 0.25f, py + (monsters.Count == 0 ? 48 : 4), pw * 0.50f, 36),
                "Kapat", _styleBtn))
                _showCollection = false;
        }

        // ── Yardımcı ──────────────────────────────────────────────────────────

        private static Color TierColor(Rarity tier) => tier switch
        {
            Rarity.B => new Color(0.40f, 0.75f, 1.00f),
            Rarity.C => new Color(0.55f, 0.90f, 0.40f),
            Rarity.D => new Color(0.95f, 0.80f, 0.30f),
            _        => new Color(0.80f, 0.78f, 0.80f),
        };

        private static void DrawBg(Rect r)
        {
            GUI.color = ColBg;
            GUI.Box(r, "");
            GUI.color = Color.white;
        }
    }
}
