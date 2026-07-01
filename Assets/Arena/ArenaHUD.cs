using System.Collections.Generic;
using UnityEngine;
using CanavarZindanlari.Arena;
using CanavarZindanlari.Backend;

/// <summary>
/// Arena IMGUI ekranı.
/// ArenaManager ve FirebaseAuthManager ile aynı sahnede bulunur.
/// </summary>
[RequireComponent(typeof(ArenaManager))]
public class ArenaHUD : MonoBehaviour
{
    // ── Renkler ──────────────────────────────────────────────────────────────

    private static readonly Color ColBronze  = new Color(0.80f, 0.50f, 0.20f);
    private static readonly Color ColSilver  = new Color(0.75f, 0.75f, 0.80f);
    private static readonly Color ColGold    = new Color(1.00f, 0.84f, 0.10f);
    private static readonly Color ColPlatin  = new Color(0.40f, 0.90f, 1.00f);
    private static readonly Color ColWin     = new Color(0.20f, 0.85f, 0.30f);
    private static readonly Color ColLose    = new Color(0.90f, 0.25f, 0.25f);
    private static readonly Color ColPanel   = new Color(0.10f, 0.10f, 0.18f, 0.95f);

    // ── Stiller ──────────────────────────────────────────────────────────────

    private GUIStyle _styleTitle;
    private GUIStyle _styleLabel;
    private GUIStyle _styleBtn;
    private GUIStyle _styleBtnSmall;
    private GUIStyle _stylePanel;
    private GUIStyle _styleCard;
    private bool     _stylesBuilt;

    // ── Durum ────────────────────────────────────────────────────────────────

    private ArenaManager        _arena;
    private bool                _isOpen;
    private bool                _showLeaderboard;
    private Vector2             _lbScroll;
    private List<ArenaProfile>  _lbCache;
    private bool                _lbLoading;
    private string              _lbError;

    public bool IsOpen => _isOpen;

    public void Open()  => _isOpen = true;
    public void Close() => _isOpen = false;

    private void Awake()  => _arena = GetComponent<ArenaManager>();

    private void OnGUI()
    {
        if (!_isOpen) return;

        BuildStyles();

        float w = Screen.width;
        float h = Screen.height;

        // Arka plan paneli
        GUI.color = ColPanel;
        GUI.DrawTexture(new Rect(0, 0, w, h), Texture2D.whiteTexture);
        GUI.color = Color.white;

        switch (_arena.State)
        {
            case ArenaManager.ArenaState.Idle:          DrawIdle(w, h);         break;
            case ArenaManager.ArenaState.SigningIn:     DrawWaiting(w, h, "Giriş yapılıyor..."); break;
            case ArenaManager.ArenaState.LoadingProfile:DrawWaiting(w, h, "Profil yükleniyor..."); break;
            case ArenaManager.ArenaState.Matchmaking:   DrawWaiting(w, h, "Rakip aranıyor..."); break;
            case ArenaManager.ArenaState.BattleResult:  DrawResult(w, h);        break;
        }
    }

    // ── Ekranlar ─────────────────────────────────────────────────────────────

    private void DrawIdle(float w, float h)
    {
        float pad    = w * 0.05f;
        float btnW   = w - pad * 2;
        float btnH   = h * 0.07f;
        float startY = h * 0.08f;

        // Başlık
        _styleTitle.normal.textColor = ColGold;
        GUI.Label(new Rect(pad, startY, btnW, btnH * 1.4f), "⚔  ARENA", _styleTitle);
        startY += btnH * 1.6f;

        var auth = FirebaseAuthManager.Instance;

        if (auth == null || !auth.IsSignedIn)
        {
            // Giriş butonu
            _styleLabel.normal.textColor = Color.gray;
            GUI.Label(new Rect(pad, startY, btnW, btnH), "Arenaya katılmak için giriş yap.", _styleLabel);
            startY += btnH * 1.2f;

            GUI.color = ColGold;
            if (GUI.Button(new Rect(pad, startY, btnW, btnH), "Google ile Giriş Yap", _styleBtn))
                _arena.StartSignIn();
            GUI.color = Color.white;
            return;
        }

        // Profil kartı
        DrawProfileCard(pad, startY, btnW, _arena.MyProfile);
        startY += btnH * 3.2f;

        // Maç bul
        GUI.color = ColWin;
        if (GUI.Button(new Rect(pad, startY, btnW, btnH), "⚔  Maç Bul", _styleBtn))
            _arena.FindMatch();
        GUI.color = Color.white;
        startY += btnH * 1.3f;

        // Liderlik tablosu toggle
        string lbLabel = _showLeaderboard ? "▲ Sıralamayı Gizle" : "▼ Sıralamayı Göster";
        if (GUI.Button(new Rect(pad, startY, btnW, btnH * 0.8f), lbLabel, _styleBtnSmall))
        {
            _showLeaderboard = !_showLeaderboard;
            if (_showLeaderboard && _lbCache == null && !_lbLoading)
                LoadLeaderboard();
        }
        startY += btnH * 1.0f;

        if (_showLeaderboard)
            DrawLeaderboardSection(pad, startY, btnW, h - startY - pad);

        // Geri + Çıkış
        float bottomY = h - pad - btnH * 0.7f;
        GUI.color = new Color(0.5f, 0.5f, 0.5f);
        if (GUI.Button(new Rect(pad, bottomY, btnW * 0.45f, btnH * 0.7f), "← Geri", _styleBtnSmall))
            Close();
        if (auth.IsSignedIn)
        {
            if (GUI.Button(new Rect(pad + btnW * 0.55f, bottomY, btnW * 0.45f, btnH * 0.7f), "Çıkış Yap", _styleBtnSmall))
                auth.SignOut();
        }
        GUI.color = Color.white;

        // Durum mesajı
        if (!string.IsNullOrEmpty(_arena.StatusMessage))
        {
            _styleLabel.normal.textColor = Color.gray;
            GUI.Label(new Rect(pad, h - pad - btnH * 1.6f, btnW, btnH * 0.8f), _arena.StatusMessage, _styleLabel);
        }
    }

    private void DrawWaiting(float w, float h, string msg)
    {
        float pad  = w * 0.05f;
        float btnW = w - pad * 2;
        _styleTitle.normal.textColor = Color.white;
        GUI.Label(new Rect(pad, h * 0.4f, btnW, h * 0.1f), msg, _styleTitle);
        _styleLabel.normal.textColor = Color.gray;
        GUI.Label(new Rect(pad, h * 0.52f, btnW, h * 0.06f), _arena.StatusMessage, _styleLabel);
    }

    private void DrawResult(float w, float h)
    {
        float pad  = w * 0.05f;
        float btnW = w - pad * 2;
        float btnH = h * 0.07f;
        float cy   = h * 0.12f;

        bool won = _arena.LastMatchWon;

        // Sonuç başlığı
        _styleTitle.normal.textColor = won ? ColWin : ColLose;
        GUI.Label(new Rect(pad, cy, btnW, btnH * 1.4f), won ? "ZAFER!" : "YENİLDİ!", _styleTitle);
        cy += btnH * 1.8f;

        // Ben
        _styleLabel.normal.textColor = Color.white;
        GUI.Label(new Rect(pad, cy, btnW, btnH), "Sen", _styleLabel);
        cy += btnH * 0.7f;
        DrawProfileCard(pad, cy, btnW, _arena.MyProfile);
        cy += btnH * 3.0f;

        // Rakip
        _styleLabel.normal.textColor = Color.white;
        GUI.Label(new Rect(pad, cy, btnW, btnH), "Rakip", _styleLabel);
        cy += btnH * 0.7f;
        DrawProfileCard(pad, cy, btnW, _arena.LastOpponent);
        cy += btnH * 3.2f;

        // Puan değişimi
        _styleLabel.normal.textColor = won ? ColWin : ColLose;
        string delta = won ? "+25 puan" : "-15 puan";
        GUI.Label(new Rect(pad, cy, btnW, btnH), $"{delta}  →  {_arena.MyProfile.ArenaPoints} puan  ({_arena.MyProfile.LeagueTier})", _styleLabel);
        cy += btnH * 1.4f;

        // Devam
        GUI.color = ColGold;
        if (GUI.Button(new Rect(pad, cy, btnW, btnH), "Devam", _styleBtn))
            _arena.BackToIdle();
        GUI.color = Color.white;
    }

    // ── Profil kartı ─────────────────────────────────────────────────────────

    private void DrawProfileCard(float x, float y, float w, ArenaProfile p)
    {
        if (p == null) return;

        float cardH = Screen.height * 0.07f * 2.8f;
        GUI.color = new Color(0.15f, 0.15f, 0.25f, 1f);
        GUI.DrawTexture(new Rect(x, y, w, cardH), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float pad = w * 0.04f;
        float lh  = cardH * 0.28f;

        Color tierColor = p.LeagueTier switch
        {
            "Platin" => ColPlatin,
            "Altın"  => ColGold,
            "Gümüş"  => ColSilver,
            _        => ColBronze,
        };

        _styleLabel.normal.textColor = tierColor;
        GUI.Label(new Rect(x + pad, y + pad, w - pad * 2, lh),
            $"{p.LeagueIcon} {p.DisplayName}  [{p.LeagueTier}]", _styleLabel);

        _styleLabel.normal.textColor = Color.white;
        GUI.Label(new Rect(x + pad, y + pad + lh, w - pad * 2, lh),
            $"Savaş Gücü: {p.CombatPower}", _styleLabel);

        _styleLabel.normal.textColor = Color.gray;
        GUI.Label(new Rect(x + pad, y + pad + lh * 2, w - pad * 2, lh),
            $"G: {p.Wins}  M: {p.Losses}  Oran: %{p.WinRate}  |  {p.ArenaPoints} puan", _styleLabel);
    }

    // ── Liderlik tablosu ─────────────────────────────────────────────────────

    private async void LoadLeaderboard()
    {
        _lbLoading = true;
        _lbError   = null;
        try
        {
            _lbCache = await PlayerProfileService.GetLeaderboard();
        }
        catch (System.Exception e)
        {
            _lbError = $"Yüklenemedi: {e.Message}";
        }
        _lbLoading = false;
    }

    private void DrawLeaderboardSection(float x, float y, float w, float maxH)
    {
        float rowH   = Screen.height * 0.058f;
        float headerH = rowH * 0.9f;

        // Başlık + yenile butonu
        _styleLabel.normal.textColor = ColGold;
        GUI.Label(new Rect(x, y, w * 0.65f, headerH), "🏆 Top 20", _styleLabel);

        GUI.color = new Color(0.6f, 0.6f, 1f);
        if (GUI.Button(new Rect(x + w * 0.68f, y, w * 0.32f, headerH * 0.85f), "↻ Yenile", _styleBtnSmall))
        {
            _lbCache   = null;
            LoadLeaderboard();
        }
        GUI.color = Color.white;
        y += headerH + 4f;

        float viewH = maxH - headerH - 8f;
        var   viewR = new Rect(x, y, w, viewH);

        if (_lbLoading)
        {
            _styleLabel.normal.textColor = Color.gray;
            GUI.Label(viewR, "Yükleniyor...", _styleLabel);
            return;
        }

        if (_lbError != null)
        {
            _styleLabel.normal.textColor = new Color(1f, 0.4f, 0.4f);
            GUI.Label(viewR, _lbError, _styleLabel);
            return;
        }

        if (_lbCache == null || _lbCache.Count == 0)
        {
            _styleLabel.normal.textColor = Color.gray;
            GUI.Label(viewR, "Henüz sıralama yok.", _styleLabel);
            return;
        }

        // Scroll view
        float contH = _lbCache.Count * (rowH + 3f);
        var   contR = new Rect(0, 0, w - 16f, contH);
        _lbScroll = GUI.BeginScrollView(viewR, _lbScroll, contR, false, false);

        for (int i = 0; i < _lbCache.Count; i++)
        {
            var    p       = _lbCache[i];
            float  ry      = i * (rowH + 3f);
            bool   isMe    = _arena.MyProfile != null && p.Uid == _arena.MyProfile.Uid;

            // Satır arka planı
            GUI.color = isMe
                ? new Color(0.25f, 0.35f, 0.55f, 1f)
                : new Color(0.13f, 0.13f, 0.22f, 1f);
            GUI.DrawTexture(new Rect(0, ry, w - 16f, rowH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float pad = w * 0.025f;

            // Sıra numarası
            Color rankColor = i == 0 ? ColGold : i == 1 ? ColSilver : i == 2 ? ColBronze : Color.white;
            _styleLabel.normal.textColor = rankColor;
            GUI.Label(new Rect(pad, ry + 2f, rowH, rowH), $"{i + 1}.", _styleLabel);

            // Amblem + isim
            Color tierColor = p.LeagueTier switch
            {
                "Platin" => ColPlatin,
                "Altın"  => ColGold,
                "Gümüş"  => ColSilver,
                _        => ColBronze,
            };
            _styleLabel.normal.textColor = tierColor;
            string name = isMe ? $"{p.DisplayName} (Sen)" : p.DisplayName;
            GUI.Label(new Rect(pad + rowH, ry + 2f, w * 0.45f, rowH), $"{p.LeagueIcon} {name}", _styleLabel);

            // Puan + G/M
            _styleLabel.normal.textColor = Color.white;
            GUI.Label(new Rect(w * 0.62f, ry + 2f, w * 0.36f, rowH),
                $"{p.ArenaPoints}p  {p.Wins}G/{p.Losses}M", _styleLabel);
        }

        GUI.EndScrollView();
    }

    // ── Stil oluşturma ────────────────────────────────────────────────────────

    private void BuildStyles()
    {
        if (_stylesBuilt) return;
        _stylesBuilt = true;

        int fs = Mathf.Max(14, Screen.width / 22);

        _styleTitle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = fs + 10,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
        };

        _styleLabel = new GUIStyle(GUI.skin.label)
        {
            fontSize  = fs,
            wordWrap  = true,
            alignment = TextAnchor.MiddleLeft,
        };

        _styleBtn = new GUIStyle(GUI.skin.button)
        {
            fontSize  = fs,
            fontStyle = FontStyle.Bold,
        };

        _styleBtnSmall = new GUIStyle(GUI.skin.button)
        {
            fontSize = fs - 2,
        };

        _stylePanel = new GUIStyle(GUI.skin.box);
        _styleCard  = new GUIStyle(GUI.skin.box);
    }
}
