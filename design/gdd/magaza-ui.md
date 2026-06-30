# Mağaza UI

> **Status**: Designed
> **Author**: user + ux-designer, economy-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Cömert Zindan, Hep Bir Şey Var, Güç Hisset

## Overview

**Mağaza UI**, oyuncunun reklam izleyerek ücretsiz kaynak kazandığı ve birikmiş elmasını harcadığı alışveriş ekranıdır. Tab Bar'ın "Mağaza" sekmesinden erişilir. HUD görünürdür.

Oyun tamamen ücretsiz ve reklam tabanlıdır — gerçek para satın alımı (IAP) prototipte yer almaz, Tier 2'de eklenebilir. Elmas yalnızca oyun içi kaynaklardan kazanılır (ilk temizleme bonusları, günlük giriş, başarımlar). Mağaza iki sekmeye ayrılır: **Ücretsiz** (günlük reklam ödülleri) ve **Sandıklar** (elmas ile pet sandığı açma).

MVP kapsamında günlük reklam ödül kartları, elma bazlı sandıklar, sandık açma animasyonu, enerji yenileme ve IAP sistemine bağlantı stub'u yer alır.

## Player Fantasy

Oyuncu mağazaya her gün bir kez uğrar — "bugünkü ücretsiz ödüllerimi aldım mı?" ritüeli. Reklam izleme biraz zaman alıyor ama karşılığı cömert hissettiriyor: birkaç reklamın ardından anlamlı bir kaynak birikiyor.

**Sandık açma heyecanı**: 300 elmas biriktirip Nadide Sandık'a basmak ve B tier bir petin kartının yavaş yavaş açılmasını izlemek — "bu sandıktan ne çıkacak?" gerilimi küçük bir piyango andır.

**"Kazanmak için oynadım" tatmini**: Oyuncu elmasını reklam değil, savaşarak ve keşfederek kazandı. Mağazada harcadığında "hak ettim" hissi vardır. Hiçbir şey zorla satılmıyor.

**Negatif fantezi (kaçınılacak)**: Agresif satış baskısı — büyük "SATIN AL!" butonu veya "son 2 saat!" sayaçları. Mağaza rahat bir alışveriş yeri olmalı, baskı hissettirmemeli. Elmas yetersizse "Nasıl kazanırım?" ipucu verilir, utandırıcı satın alma teklifi değil.

## Detailed Rules

### Core Rules

**Kural 1 — Genel Ekran Layout'u**

```
┌────────────────────────────────┐
│  HUD (altın / elmas / enerji)  │
├────────────────────────────────┤
│  Üst Bar                       │
│  "Mağaza"                      │
├────────────────────────────────┤
│  [Ücretsiz ★] [Sandıklar]      │  ← Sekme çubuğu (2 sekme)
├────────────────────────────────┤
│                                │
│  Aktif sekme içeriği (scroll)  │
│                                │
├────────────────────────────────┤
│  Tab Bar                       │
└────────────────────────────────┘
```

- **Sekme çubuğu**: Ücretsiz (varsayılan) + Sandıklar. Şu anki elmas miktarı üst bar sağında.
- **İçerik alanı**: Dikey scroll. Kartlar arası 12 dp boşluk.
- **HUD**: Görünür — altın/elmas/enerji anlık izlenebilir.

---

**Kural 2 — Ücretsiz Sekmesi**

Günlük sıfırlanan (gece yarısı yerel saat) reklam ödül kartlarını içerir. Tüm kartlar scroll edilebilir tek sütunda listelenir.

**Ödül kartı anatomisi:**

```
┌────────────────────────────────────┐
│ [Büyük ikon]  Ödül Adı             │
│               Kazanacağın: [miktar]│
│               ─────────────────    │
│  [██░░░] 3/5 kalan                 │
│               [ REKLAM İZLE ]      │
└────────────────────────────────────┘
```

**MVP Ücretsiz Ödül Listesi:**

| Kart | Ödül | Günlük Limit | Buton Durumu |
|------|------|-------------|-------------|
| ⚡ Enerji Yüklemesi | +20 Enerji | 5 / gün | Reklam İzle |
| 🪙 Altın Yağmuru | +1.000 Altın | 3 / gün | Reklam İzle |
| 💎 Elmas Ödülü | +5 Elmas | 2 / gün | Reklam İzle |

**Limit dolduğunda**: Kart soluk görünür, buton "Yarın Yenilenir ⏰" olarak değişir. Yenileme zamanı (saat:dakika) butonda gösterilir.

**Tüm limitler dolduysa** (3/3 kart tüketildi): Sekme başlığında "★ → ✓" işareti belirir. Sayfada teşvik mesajı: "Bugünkü ücretsiz ödüllerin tamamı alındı! Yarın tekrar gel."

---

**Kural 3 — Reklam İzleme Akışı**

1. Oyuncu "REKLAM İZLE" butonuna basar.
2. IAP + Reklam Sistemi'ne reklam talebi iletilir (`RequestRewardedAd(rewardType)`).
3. Reklam yükleniyorsa: "Yükleniyor..." spinner (maks 5 sn).
4. Reklam oynar (IAP sistemi kontrolünde — UI dokunuşlara yanıt vermez).
5. Reklam tamamlandıysa:
   - Ödül anında verilir (`GrantReward(rewardType, amount)`).
   - Toast: "⚡ +20 Enerji kazandın!" (success rengi).
   - Kart sayacı güncellenir: "3/5 → 2/5".
6. Reklam atlandıysa veya hata oluştuysa:
   - Ödül verilmez.
   - Toast: "Reklam tamamlanmadı — ödül kazanılamadı." (warning rengi).
   - Buton aktif kalır (tekrar denenebilir).
7. Reklam yüklenemezse (internet yok / reklam stoku boş):
   - Toast: "Şu an reklam yok — daha sonra tekrar dene." (info rengi).
   - Buton aktif kalır.

---

**Kural 4 — Sandıklar Sekmesi**

Elmas harcayarak pet sandığı açma. Üç sandık tipi, ayrı kartlarda gösterilir.

**Sandık kart anatomisi:**

```
┌────────────────────────────────────┐
│  [Sandık görseli — 120×120 dp]     │
│                                    │
│  Seçkin Sandık                     │
│  D/C tier pet garantili            │
│                                    │
│  Olasılıklar:                      │
│  D tier: %70  ▓▓▓▓▓▓▓░░░           │
│  C tier: %30  ▓▓▓░░░░░░░           │
│                                    │
│         [ 💎 150 — AÇ ]            │
└────────────────────────────────────┘
```

**MVP Sandık Tablosu:**

| Sandık | Maliyet | İçerik | Olasılıklar |
|--------|---------|--------|-------------|
| **Temel Sandık** | 💎 50 | F veya D tier pet | F: %80 / D: %20 |
| **Seçkin Sandık** | 💎 150 | D veya C tier pet | D: %70 / C: %30 |
| **Nadide Sandık** | 💎 400 | C veya B tier pet | C: %80 / B: %20 |

- Olasılıklar kart üzerinde açıkça gösterilir (gizli değil).
- Elmas yetersizse buton gri, "Yeterli Elmas Yok" etiketi + küçük "Nasıl kazanırım?" linki.
- "Nasıl kazanırım?" tooltip: "Keşif Alanı ilk temizlemelerinden, günlük girişlerden ve başarımlardan elmas kazanabilirsin."

---

**Kural 5 — Sandık Açma Animasyonu**

Sandık "AÇ" butonuna basılınca tam ekran overlay açılır:

```
Aşama 1 (0.5s):   Sandık ekranda titreşir / ışık saçar
Aşama 2 (0.8s):   Sandık kapağı açılır, ışık patlaması
Aşama 3 (0.5s):   Pet kartı kapaktan yükselir (flip animasyon)
Aşama 4 (süresiz): Kart ekranda durur — tier rozeti + isim + element görünür
```

- Pet kartı tier'a göre çerçeve rengi (F gri / D yeşil / C mavi / B altın).
- B tier çıkışında ekrana altın parçacık efekti eklenir.
- Ekrana herhangi bir yere dokunmak kartı kapatır, Sandıklar sekmesine dönülür.
- Kazanılan pet koleksiyona anında eklenir (arka planda, animasyon süresinde de).
- Toast (overlay kapandıktan sonra): "[Pet Adı] koleksiyona eklendi!"
- ReducedMotion: animasyon atlanır, kart doğrudan son durumda görünür.

---

**Kural 6 — Elmas Yetersizlik Akışı**

Sandık butonuna basılınca elmas yetersizse açılır panel:

```
┌────────────────────────────────────┐
│  Yeterli Elmas Yok                 │
│                                    │
│  Seçkin Sandık: 💎 150             │
│  Mevcut: 💎 87  (63 elmas eksik)   │
│                                    │
│  Elmas kazanmak için:              │
│  • Keşif Alanı ilk temizleme       │
│  • Günlük giriş ödülü (💎 7/gün)  │
│  • Ücretsiz sekmesi (💎 5 × 2/gün)│
│                                    │
│  [TAMAM]                           │
└────────────────────────────────────┘
```

Satın alma baskısı yok — yalnızca kazanım yolları gösterilir.

---

**Kural 7 — Enerji Yenileme (Ücretsiz Sekme Alt Kısmı)**

Ücretsiz sekmenin en altına sabit bir "Acil Enerji" kartı eklenir (günlük limite dahil değil):

```
┌──────────────────────────────────────┐
│ ⚡ Acil Enerji Yenilemesi             │
│                                      │
│ Enerjini anında tam doldur.          │
│ Mevcut: 45/100                       │
│                                      │
│ [ 💎 50  — TAM DOLDUR ]              │
└──────────────────────────────────────┘
```

- Elmas ile tam doldurma — limit yok (her zaman kullanılabilir).
- Enerji zaten 100'se buton disabled: "Enerji Zaten Dolu ✓".
- Elmas yetersizse Kural 6 akışı.

---

**Kural 8 — Günlük Sıfırlama Göstergesi**

Ücretsiz sekmesinin en üstünde küçük sayaç:

```
Günlük ödüller yenileniyor: 07:23:45
```

Geri sayım. Gece yarısı local time'da tüm günlük limitler sıfırlanır, sayaç 24:00:00'a sıfırlanır.

---

### States and Transitions

```
[Mağaza Ekranı — Ücretsiz Sekmesi]
    ├─ "Reklam İzle" → [Reklam Oynatma] → Tamamlandı: ödül + toast + sayaç güncelleme
    │                                   → Atlandı/Hata: toast, buton aktif kalır
    ├─ [Sandıklar] sekmesine geç
    └─ "Tam Doldur" (💎50) → Elmas yeterliyse: anlık enerji dolumu + toast
                           → Yetersizse: Elmas Yetersizlik Paneli

[Mağaza Ekranı — Sandıklar Sekmesi]
    ├─ "AÇ" (elmas yeterliyse) → [Sandık Açma Overlay]
    │   └─ Overlay dokunma → Kapanır → Sandıklar sekmesine dön + toast
    └─ "AÇ" (elmas yetersizse) → [Elmas Yetersizlik Paneli]
```

| Durum | Görsel |
|-------|--------|
| Reklam kartı kullanılabilir | Parlak buton, sayaç aktif |
| Reklam kartı tükendi | Soluk kart, "Yarın Yenilenir ⏰" butonu |
| Sandık açılabilir | Parlak "AÇ" butonu |
| Sandık açılamaz (elmas) | Gri buton, "Yeterli Elmas Yok" |
| Sandık açılıyor | Tam ekran overlay |
| Tüm günlük ödüller alındı | Sekme başlığında ✓, teşvik mesajı |

## Formulas

### Formül 1: Günlük Kalan Kullanım Göstergesi

```
remaining = daily_limit - used_today
// Bar fill:
fill_ratio = remaining / daily_limit
```

Kart üzerinde dolu bar = kalan kullanım.

**Örnek**: Enerji kartı, 2/5 kullanılmış → remaining=3, fill_ratio=3/5=0.60 → bar %60 dolu.

### Formül 2: Yenileme Geri Sayımı

```
reset_time = midnight_local_time
seconds_remaining = reset_time - now()
display = format_as_HH:MM:SS(seconds_remaining)
```

### Formül 3: Sandık Ödül Seçimi

```
roll = random(0.0, 1.0)

// Temel Sandık:
if roll < 0.80 → F tier
else           → D tier

// Seçkin Sandık:
if roll < 0.70 → D tier
else           → C tier

// Nadide Sandık:
if roll < 0.80 → C tier
else           → B tier
```

Tier belirlendikten sonra o tier'dan rastgele bir monster seçilir (`GetRandomMonsterByTier(tier)`).

### Formül 4: Elmas Açığı Gösterimi

```
deficit = chest_cost - current_gems
display_text = f"{current_gems} elmas var ({deficit} elmas eksik)"
```

### Formül 5: Enerji Yenileme Maliyeti

```
// Sabit: 50 elmas → tam doldurma
// Kısmi doldurma yok — her zaman tam (ekonomi GDD Kural 5 ile uyumlu)
energy_refill_cost = 50
```

## Edge Cases

- **If reklam izlenirken uygulama arka plana geçerse**: Reklam kesilir, ödül verilmez. Ön plana dönünce "Reklam tamamlanmadı" toast + buton aktif kalır.

- **If aynı ödül butonuna hızlı iki kez basılırsa**: Debounce 500ms — ikinci basma reddedilir. Reklam yalnızca bir kez başlar.

- **If sandık açma animasyonu sırasında geri tuşuna basılırsa**: Animasyon durdurulur, overlay kapanır. Pet zaten kazanıldıysa koleksiyona eklenmiş olur (overlay arka planda işlendi). Toast: "[Pet Adı] koleksiyona eklendi!"

- **If kazanılan pet ile koleksiyon doluysa (max kapasite)**: Pet yine de eklenmeye çalışılır. Kapasite aşımı durumu Pet Sistemi'nin sorumluluğundadır — overflow inbox kullanılır (Koleksiyon GDD ile uyumlu).

- **If internet bağlantısı yoksa "Reklam İzle" butonuna basılırsa**: IAP sistemi "reklam yok" hatası döner → toast: "Şu an reklam yok — internet bağlantını kontrol et."

- **If günlük limit sıfırlanma anında oyuncu Ücretsiz sekmesindeyse**: Kartlar anlık güncellenir (sayaçlar sıfırlanır, butonlar aktifleşir) — yeniden giriş gerekmez.

- **If elmas Sandık açma işlemi sırasında eş zamanlı başka yerden harcanırsa**: Sandık açma onaylandığında anlık elmas kontrolü yapılır. Yeterli değilse işlem iptal edilir, Elmas Yetersizlik Paneli açılır.

- **If tüm Sandık sekmesi kartları elmas yetersizliği yüzünden disabled olursa**: Sekme içeriğinde "Elmas Kazanmak İçin" ipucu kartı en altta görünür.

- **If ReducedMotion aktifse**: Sandık açma animasyonu atlanır, pet kartı anında beliriyor. Overlay tasarımı aynı, sadece geçiş efektleri yok.

- **If Ücretsiz sekmesindeki tüm reklam kartlarının limiti doluysa**: Sayfada "Bugün harikasın! Tüm ödülleri topladın. Yarın geri gel." mesajı + Sandıklar sekmesine yönlendiren küçük link.

## Dependencies

### Upstream

| Sistem | Veri | Arayüz |
|--------|------|--------|
| **IAP + Reklam Sistemi** | Reklam yükleme, oynatma, tamamlanma callback | `RequestRewardedAd(rewardType)`, `OnAdComplete(success)`, `OnAdFailed(reason)` |
| **Ekonomi** | Güncel altın/elmas/enerji; kaynak verme | `GetCurrentResources()`, `GrantReward(type, amount)`, `SpendGems(amount)` |
| **Canavar Veritabanı** | Tier'a göre rastgele monster seçimi | `GetRandomMonsterByTier(tier)` → monsterId |
| **Pet Sistemi** | Kazanılan peti koleksiyona ekle | `AddPetToCollection(monsterId)` |
| **Kaydetme / Yükleme** | Günlük kullanım sayaçları | `GetDailyUsage(rewardType)`, `IncrementDailyUsage(rewardType)`, `GetDailyResetTime()` |
| **UI Framework** | Stack navigasyon, modal, toast, HUD | `UIManager.*` |

### Downstream

| Sistem | Tetikleme |
|--------|-----------|
| **Ekonomi** | Reklam/sandık ödülü → `GrantReward()` |
| **Pet Sistemi** | Sandık açma → `AddPetToCollection()` |
| **Koleksiyon UI** | Yeni pet eklendikten sonra açılırsa güncel veriyle gösterilir |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `daily_energy_ad_limit` | 5 | 3–8 | Oyuncu enerjisi bitmez, enerji sistemi anlamsız | Reklam ile yeterli katkı yok |
| `daily_gold_ad_limit` | 3 | 2–5 | Altın çok kolay | Reklam teşviki zayıf |
| `daily_gems_ad_limit` | 2 | 1–3 | Sandık çok sık açılır | Elmas teşviki zayıf |
| `energy_ad_reward` | 20 | 10–30 | Tek reklamla %20+ enerji → banyo molası sistemi bozulur | Reklamın değeri yok |
| `gold_ad_reward` | 1.000 | 500–2.000 | Mağaza altını zindanı geride bırakır | Kimse izlemez |
| `gems_ad_reward` | 5 | 3–10 | Sandık çok ucuzlaşır | Teşvik yok |
| `basic_chest_cost` | 50 | 30–80 | Ulaşılmaz | Elmas sinks boşalır |
| `premium_chest_cost` | 150 | 100–250 | Nadiren açılır | Çok ucuz, B tier değersizleşir |
| `rare_chest_cost` | 400 | 250–600 | Hiç açılmaz | B tier çok kolay |
| `basic_chest_D_rate` | 0.20 | 0.10–0.35 | D tier değersizleşir | Temel sandık çekici değil |
| `premium_chest_C_rate` | 0.30 | 0.20–0.45 | Seçkin sandık çok güçlü | Seçkin ile temel arasında fark yok |
| `rare_chest_B_rate` | 0.20 | 0.10–0.30 | B tier enflasyonu | Nadide sandık değeri yok |
| `energy_refill_cost` | 50 | 30–80 | Çok pahalı, kullanılmaz | Enerji sistemi trivial |
| `ad_debounce_ms` | 500 | 300–1000 | Kasıtlı çift tıklama engellenir | Çift reklam başlayabilir |

## Acceptance Criteria

1. **GIVEN** Ücretsiz sekmesi açıldığında, **WHEN** günlük limitlerde kullanım varsa, **THEN** kalan kullanım sayısı doluluk barı ile gösterilir.

2. **GIVEN** "REKLAM İZLE" basıldı ve reklam tamamlandı, **WHEN** IAP sistemi `OnAdComplete(true)` çağırırsa, **THEN** ödül verilir, kart sayacı 1 azalır, toast gösterilir.

3. **GIVEN** reklam atlandı, **WHEN** IAP sistemi `OnAdComplete(false)` çağırırsa, **THEN** ödül verilmez, toast: "Reklam tamamlanmadı", buton aktif kalır.

4. **GIVEN** Enerji kartı 5/5 kullanıldı, **WHEN** kart render edilirse, **THEN** kart soluk, buton "Yarın Yenilenir ⏰" + geri sayım saati gösterilir.

5. **GIVEN** 3 reklam kartı da tüketildi, **WHEN** Ücretsiz sekmesi görüntülenir, **THEN** sekme başlığında "Ücretsiz ✓" ve teşvik mesajı gösterilir.

6. **GIVEN** Seçkin Sandık'a basıldı, 150 💎 mevcutsa, **WHEN** "AÇ" basılırsa, **THEN** 150 elmas düşer, sandık açma animasyonu oynar (titreşme 0.5s → kapak 0.8s → kart flip 0.5s → kart durur).

7. **GIVEN** sandık açma animasyonu sırasında, **WHEN** ekrana dokunulursa, **THEN** overlay kapanır, pet zaten koleksiyona eklenmiş, toast "[Pet Adı] koleksiyona eklendi!" gösterilir.

8. **GIVEN** Nadide Sandık açıldı ve B tier çıktı, **WHEN** kart belirir, **THEN** kart altın çerçeve + ekranda altın parçacık efekti oynar.

9. **GIVEN** Nadide Sandık açılmak isteniyor, 250 💎 var (400 gerekli), **WHEN** "AÇ" basılırsa, **THEN** Elmas Yetersizlik Paneli açılır: "Mevcut: 250, Eksik: 150" + kazanım yolları listesi.

10. **GIVEN** Elmas Yetersizlik Paneli, **WHEN** "TAMAM" basılırsa, **THEN** panel kapanır. Sandık sayfası açık kalır. Satın alma baskısı yapılmaz.

11. **GIVEN** Enerji 45/100, 50 💎 mevcutsa, **WHEN** "TAM DOLDUR" basılırsa, **THEN** 50 elmas düşer, enerji 100 olur, toast "⚡ Enerji tam!" HUD anında güncellenir.

12. **GIVEN** Enerji 100/100 iken, **WHEN** Acil Enerji kartı render edilirse, **THEN** buton disabled, "Enerji Zaten Dolu ✓" yazıyor.

13. **GIVEN** Sandık açma animasyonu oynarken uygulama arka plana geçerse, **WHEN** geri dönülürse, **THEN** overlay kapalı, pet koleksiyonda, toast gösterilir (eğer henüz gösterilmediyse).

14. **GIVEN** "Reklam İzle" butonuna 300ms içinde iki kez basılırsa, **WHEN** ikinci basma gelirse, **THEN** debounce ile reddedilir, yalnızca 1 reklam başlar.

15. **GIVEN** ReducedMotion aktif, **WHEN** sandık açma tetiklense, **THEN** animasyon atlanır, pet kartı anında son durumda görünür. Ödül normal verilir.

16. **GIVEN** Sandıklar sekmesi açıldı, **WHEN** sandık kartları render edilirse, **THEN** her sandık için olasılık barları (örn. C: %30 / D: %70) açıkça gösterilir.

17. **GIVEN** gece yarısı geçince günlük sıfırlama gerçekleşti, **WHEN** oyuncu Ücretsiz sekmesindeyse, **THEN** kartlar anlık güncellenir (sayaçlar sıfırlanır, butonlar aktifleşir). Yeniden giriş gerekmez.

18. **GIVEN** reklam yüklenemedi (internet yok), **WHEN** "REKLAM İZLE" basılırsa, **THEN** 5s bekleme sonrası toast: "Şu an reklam yok — internet bağlantını kontrol et." Buton aktif kalır.

19. **GIVEN** tüm sandık kartları elmas yetersizliği nedeniyle disabled, **WHEN** Sandıklar sekmesi görüntülenirse, **THEN** sayfanın altında "Elmas Kazanmak İçin" ipucu kartı görünür.

20. **GIVEN** Temel Sandık açıldı, **WHEN** `GetRandomMonsterByTier(tier)` çağrılırsa, **THEN** olasılık dağılımı: random roll < 0.80 → F tier, ≥ 0.80 → D tier. Seçilen tier'dan rastgele monster belirlenir.
