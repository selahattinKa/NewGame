# Keşif Alanı Harita UI

> **Status**: Designed
> **Author**: user + ux-designer, ui-programmer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon, Hep Bir Şey Var

## Overview

**Keşif Alanı Harita UI**, oyunun ana içerik ekranıdır. Oyuncu bu ekranda aktif Keşif Alanı'nın 20 aşamasını kıvrımlı bir yol üzerinde görür, aşamaları SG renk koduna göre değerlendirir ve bir aşamaya dokunarak savaşa girer. Ekran iki katmandan oluşur: **Yol Haritası** (20 aşama düğümü, kayan/scroll edilebilir) ve **Aşama Detay Paneli** (bir aşamaya dokunulunca alttan açılan bottom sheet). HUD görünür kalır (altın/elmas/enerji). Tab Bar'dan "Keşif" sekmesi üzerinden erişilir; savaş başlayınca Tab Bar kilitlenir (UI Framework Kural 9).

MVP kapsamında 1 alan, 20 aşama düğümü, 4 aşama tipi ikonu, SG renk kodu gösterimi, 3 kilit durumu, aşama detay paneli ve savaş başlatma akışı yer alır.

## Player Fantasy

Oyuncu harita ekranını açtığında **macera haritasına bakan kaşif** hissini yaşar. Karanlık Orman'ın sisli bir perspektifinden geçen kıvrımlı bir yol, ilerledikçe aydınlanan aşamalar. Tamamlanmış aşamalarda altın yıldız parlar; önündeki aşama sarı veya yeşil ışıl ışıl bekler — "bir sonraki aşama beni çağırıyor."

**İlerleme tatmini**: Aşama 20 hâlâ gri ve karanlık, ama aşama 8'i temizlediğinde 9 aniden canlanır. Bu küçük aydınlanma anı "büyüdüm" hissini somutlaştırır.

**SG renk psikolojisi**: Yeşil aşama rahatlık ve özgüven verir; sarı aşama dikkat ve heyecan; kırmızı aşama "acaba yapabilir miyim?" gerilimi. Bu renk kodu oyuncuya hazırlık durumunu sormadan söyler.

**Detay paneli merakı**: Bir aşamaya dokunmak önce detay panelini açar — savaşa girmek için bir adım daha gerekir. Bu bekleme anı "bu savaşı kazanabilir miyim?" sorusunun cevabını aramayı sağlar: SG karşılaştırması, enerji maliyeti. Bilgili karar.

**Negatif fantezi (kaçınılacak)**: Haritanın tamamı kilit ikonuyla dolup gitmemeli — bu "duvar" hissi yaratır. Oyuncunun her zaman en az 1-2 sarı/yeşil erişilebilir aşama görmesi gerekir. Kilit ikonları mümkün olduğunca sessiz/sade olmalı — büyük kırmızı çarpı veya uyarı mesajı değil.

## Detailed Rules

### Core Rules

**Kural 1 — Ekran Genel Yapısı**

```
┌────────────────────────────────┐
│  HUD (altın / elmas / enerji)  │  ← UIDocument_HUD (sort order 10)
├────────────────────────────────┤
│  Üst Bar                       │  ← Alan adı + Aktif Pet SG
│  ┌──────────────────────────┐  │
│  │  Yol Haritası (Scroll)   │  │  ← Tüm 20 aşama düğümü
│  │       Aşama 20           │  │
│  │          |               │  │
│  │       Aşama 19           │  │
│  │          ⋮               │  │
│  │       Aşama 1            │  │
│  └──────────────────────────┘  │
│  Tab Bar                       │  ← Keşif sekmesi aktif
└────────────────────────────────┘
```

- **Üst Bar**: Sabit (scroll ile kaybolmaz). Alan adı (ortada) + Aktif Pet mini portresi + SG değeri (sağda).
- **Yol Haritası**: Dikey scroll, alt → üst (Aşama 1 altta, Aşama 20 üstte). Ekrana sığmayan kısımlar kaydırılarak görülür.
- **HUD**: UI Framework standart HUD katmanı. Bu ekranda görünür.
- **Tab Bar**: Savaş dışında serbest. Savaş başlayınca `UIManager.LockTabs()`.

**Kural 2 — Yol Haritası Görseli**

Aşamalar kıvrımlı bir patika üzerinde düğüm (node) olarak sıralanır:

- Patika sol-sağ zigzag çizer (tek sütun değil, hafif kıvrımlı — Candy Crush / Clash Royale tarzı).
- Her aşama bir **Düğüm**'dür: daire şeklinde, ortada aşama numarası.
- Düğümler arası bağlantı çizgisi: tamamlanmış bölümler tam renkli, kilitli bölümler soluk/gri.
- Alan teması arka plan: Karanlık Orman için koyu yeşil/gri sisli orman illüstrasyonu (looping parallax değil, statik).

**Kural 3 — Aşama Düğümü Tipleri ve Görsel Farkı**

| Tip | Düğüm Boyutu | Şekil | Özel İkon |
|-----|-------------|-------|-----------|
| Normal | 56×56 dp | Daire | Kılıç ikonu |
| Şampiyon (5, 15) | 72×72 dp | Daire + dış halka | Taç ikonu |
| Mini Boss (10) | 80×80 dp | Sekizgen | Kafatası ikonu |
| Alan Patronu (20) | 96×96 dp | Yıldız/kalkan şekli | Ejderha/patron ikonu |

**Kural 4 — Düğüm Durumları ve Görsel Kodlama**

Her düğüm 6 olası görsel durumdan birindedir:

| Durum | Tetikleyen Koşul | Kenarlık Rengi | Arka Plan | İkon |
|-------|-----------------|----------------|-----------|------|
| **Sıralı Kilitli** | Önceki aşama temizlenmemiş | Gri | Karanlık gri | 🔒 kilit |
| **SG Kilitli** | Sıralı açık, ama player_SG < stage_SG×0.70 | Kırmızı-gri | Karanlık | 🔒 kilit (kırmızı) |
| **Kırmızı** | 0.70 ≤ player_SG/stage_SG < 0.90 | Kırmızı | Koyu kırmızı iç | Aşama numarası |
| **Sarı** | 0.90 ≤ player_SG/stage_SG < 1.10 | Sarı | Koyu sarı iç | Aşama numarası |
| **Yeşil** | player_SG/stage_SG ≥ 1.10 | Yeşil | Koyu yeşil iç | Aşama numarası |
| **Temizlenmiş** | first_clear = true | Altın | Altın iç | ★ yıldız (altın) |

- **Temizlenmiş** durumu renk kodunun üstüne eklenir: altın yıldız + SG rengi kenarlığı aynı anda görünebilir (oyuncu yeniden oynayabilir, dolayısıyla hangi SG bölgesinde olduğu hâlâ bilgidir).
- Tamamlanmış aşamalar için: altın yıldız iç kısımda, kenarlık SG rengini gösterir.
- Mevcut ilerleme göstergesi: en son tamamlanan aşamanın üstündeki açık aşama hafif pulse animasyonu (1.5s loop, scale 1.0→1.06→1.0).

**Kural 5 — Otomatik Scroll Konumu**

Harita ekranı her açılışında şu kurala göre scroll pozisyonunu ayarlar:

1. En yüksek sıralı açık ama **temizlenmemiş** aşamayı (ilk incomplete) bulur.
2. O aşama düğümü ekranın ortasına gelecek şekilde scroll pozisyonu set edilir.
3. Animasyonlu: 400ms ease-out smooth scroll (ReducedMotion aktifse anında jump).
4. Tüm aşamalar tamamlanmışsa (Alan Patronu dahil): Aşama 20 düğümü ekran ortasında.

**Kural 6 — Düğüme Dokunma Davranışı**

| Durum | Dokunma Tepkisi |
|-------|-----------------|
| Sıralı Kilitli | Toast: "Aşama [N-1]'i temizlemeden bu aşamaya geçemezsin." |
| SG Kilitli | Detay paneli açılır — ama "Savaş" butonu disabled, mesaj: "Savaş Gücün çok düşük." |
| Kırmızı / Sarı / Yeşil | Detay paneli açılır. |
| Temizlenmiş | Detay paneli açılır (tekrar oynama seçeneğiyle). |

- Dokunma geri bildirimi (Haptic): tüm erişilebilir düğümler dokunulduğunda hafif titreşim (`HapticFeedback.LightImpact`). Kilitli düğümler titreşim vermez.

**Kural 7 — Aşama Detay Paneli (Bottom Sheet)**

Erişilebilir bir aşamaya dokunulduğunda alt kısımdan süzülerek açılan panel. Modal sistemi KULLANMAZ — bağımsız bir VisualElement olarak Game katmanında açılır. Panel açıkken arka plan (harita) dokunuşlara yanıt vermez; panel dışına dokunulunca panel kapanır.

**Panel yapısı (üstten alta):**

```
┌──────────────────────────────────────┐
│  ── drag handle ──                   │
│  [Aşama 10] [MİNİ BOSS rozeti]       │
│  Buz Büyücüsü        [💧 Su]         │
│  ─────────────────────────────────── │
│  Savaş Gücü:                         │
│  Düşman SG: 300   Seninki: 255  🟡   │
│  ─────────────────────────────────── │
│  Enerji Maliyeti: ⚡2                 │
│  ─────────────────────────────────── │
│  İlk Temizleme Bonusu:               │
│  🪙×900  💎×10  + Garantili Canavar  │
│  ─────────────────────────────────── │
│  [       SAVAŞ       ]  (64dp yüksek)│
└──────────────────────────────────────┘
```

Panel içeriği:
- **Aşama Başlığı**: "Aşama [N]" + Tip rozeti (Normal / Şampiyon / Mini Boss / Alan Patronu)
- **Düşman Kartı**: Monster adı + tier ikonu (F/D/C/B). İlk karşılaşmada monster siluet (adı bilinmiyor); tamamlanmışsa tam isim + ikon.
- **SG Karşılaştırması**: Düşman SG + oyuncu SG + renk ikonu (🟢🟡🔴🔒)
- **Enerji Maliyeti**: ⚡ ikonu + maliyet (1/2/3)
- **İlk Temizleme Bonusu**: Aşama ilk kez temizlenmemişse gösterilir. Bonuslar ikon + miktar olarak listelenir. Zaten temizlenmişse bu bölüm gösterilmez — yerine "✓ Tamamlandı" etiketi.
- **Savaş Butonu**: Erişilebilirse yeşil "SAVAŞ". SG Kilitli ise gri "KİLİTLİ". Yeterli enerji yoksa turuncu "ENERJİ YETERSİZ".
- **Kapama**: Drag handle'a veya panel dışına dokunulunca 200ms aşağı kayarak kapanır.

**Kural 8 — Savaş Başlatma Akışı**

1. Oyuncu "SAVAŞ" butonuna basar.
2. Detay paneli kapanır (200ms).
3. Savaş ekranı stack'e push edilir (250ms soldan giriş animasyonu).
4. `UIManager.LockTabs()` çağrılır.
5. Savaş biter → `UIManager.UnlockTabs()`.
6. Savaş ekranı pop edilir → Harita ekranına dönülür.
7. Savaş sonucu: Galibiyet ise harita düğümü anında güncellenir (SG renk kodu + yıldız).
8. Galibiyette toast: "Aşama [N] temizlendi! ⭐" (success rengi).
9. Yeni aşama açıldıysa: yeni düğüm unlock animasyonu — gri'den renkli'ye fade (600ms), kısa parlama efekti.

**Kural 9 — Üst Bar İçeriği**

- **Alan Adı**: Ortada, büyük font. Örn. "🌲 Karanlık Orman"
- **Aktif Pet Bilgisi** (sağda): Pet mini portresi (32×32 dp daire) + SG sayısı
- SG sayısı pet güçlenince dinamik güncellenir — bu harita açıkken de gerçek zamanlı yansır (düğüm renkleri de güncellenir).

**Kural 10 — Alan Değişimi (Tier 2)**

MVP'de yalnızca 1 alan var. Alan Patronu ilk kez yenilince sistem ikinci alanın kapısını açar. Kapı açıldığında haritanın en üstünde "YENİ ALAN: [İsim] →" kartı belirir; dokunulunca bir sonraki alanın haritasına geçilir. Bu geçiş ayrı bir screen push'tur — geri dönünce Karanlık Orman haritasına döner.

---

### States and Transitions

```
[Harita Ekranı Açılır]
    ↓
[Otomatik Scroll] → [İlk Tamamlanmamış Aşama Ortalanır]
    ↓
[Bekleme / Harita İnceleme]
    ↓ dokunma
[Aşama Detay Paneli Açılır]
    ├─ [Savaş] → [Savaş Ekranı] → [Sonuç] → [Haritaya Dön]
    └─ [Kapat / Dışarı Dokun] → [Bekleme]
```

| Durum | Görsel | Etkileşim |
|-------|--------|-----------|
| **Harita Aktif** | Tüm düğümler görünür, scroll serbest | Düğümlere dokunulabilir |
| **Detay Paneli Açık** | Panel ön planda, harita karartılmaz ama input bloklanır | Yalnızca panel içi |
| **Savaş Devam Ediyor** | Tab Bar kilitli | Yok (savaş UI aktif) |
| **Unlock Animasyonu** | Yeni düğüm parlıyor | Animasyon sırasında düğüme dokunulamaz (500ms) |

## Formulas

### Formül 1: Düğüm Renk Durumu Hesabı

```
ratio = player_SG / stage_SG

if   ratio ≥ 1.10  → Yeşil
elif ratio ≥ 0.90  → Sarı
elif ratio ≥ 0.70  → Kırmızı
else               → SG Kilitli  (ayrıca sıralı kilit kontrolü önce gelir)
```

**Uygulama notu**: `ratio` float; eşik karşılaştırmaları inclusive (≥). Sıralı kilit kontrolü SG kontrolünden önce yapılır — sıralı kilitli aşama için SG hesabı atlanır.

### Formül 2: Scroll Hedef Pozisyonu

```
target_stage = min(i) where i ∈ [1..20] AND unlocked[i] = true AND first_clear[i] = false
// Yoksa target_stage = 20

scroll_y = (20 - target_stage) × node_spacing_dp - (screen_height / 2)
scroll_y = max(0, min(scroll_y, max_scroll_y))
```

### Formül 3: Pulse Animasyon Zamanlaması

Aktif (açık ama temizlenmemiş) en güncel aşama düğümüne pulse uygulanır:

```
scale(t) = 1.0 + 0.06 × sin(2π × t / 1.5)   // t: saniye, 1.5s period
```

ReducedMotion aktifse pulse devre dışı.

### Formül 4: SG Karşılaştırma Göstergesi (Detay Paneli)

```
diff_pct = ((player_SG - stage_SG) / stage_SG) × 100

if diff_pct ≥ 10  → "🟢 Güçlüsün (+{diff_pct}%)"
elif diff_pct ≥ -10 → "🟡 Dengeli"
elif diff_pct ≥ -30 → "🔴 Zayıfsın ({diff_pct}%)"
else              → "🔒 Çok Zayıf"
```

Metin kullanıcıya görünür; gizli Kauçuk Bant değeri gösterilmez.

## Edge Cases

- **If aktif pet yoksa**: Harita açılınca tüm düğümler SG Kilitli gibi görünür. Üst bar'da "Pet Seç" uyarısı. Düğümlere dokunulunca "Önce aktif bir pet seç" mesajı.

- **If alan tamamlanmışsa (Aşama 20 temizlendi) ama Tier 2 alanı yoksa**: Harita açık kalır, tüm düğümler yıldızlı. Aşama 20 düğümünün üstünde "Alan Tamamlandı ✓" banner'ı. Savaş hâlâ oynanabilir (tekrar loot için).

- **If oyuncu savaş sırasında uygulamayı arka plana atarsa**: Savaş sistemi kaydetme kurallarına göre davranır. Harita ekranına dönünce scroll pozisyonu ve panel durumu korunur (harita ekranı yığından çıkarılmamış).

- **If detay paneli açıkken enerji biter (başka yerden harcandıysa)**: Panel kapanıp tekrar açılınca buton durumu güncellenir. Panel açıkken enerji düşerse real-time güncellenmez — kullanıcı "SAVAŞ"a basınca kontrol yapılır, enerji yetersizse buton "ENERJİ YETERSİZ" durumuna geçer, savaş başlamaz.

- **If oyuncu çok hızlı birden fazla düğüme arka arkaya dokunursa**: İkinci dokunma, birincinin paneli açma animasyonu tamamlanmadan gelirse reddedilir (debounce 300ms).

- **If player_SG değişimi (pet güçlendirme) harita açıkken gerçekleşirse**: Tüm düğüm renkleri anlık güncellenir. Panel açıksa SG karşılaştırma satırı da güncellenir. Yeşile dönen düğüm için kısa renk geçişi animasyonu (400ms fade).

- **If scroll harita en üstünde veya en altındaysa**: Over-scroll bounce efekti uygulanmaz (mobil standart bounce devre dışı). Sert dur.

- **If unlock animasyonu oynanırken oyuncu o düğüme dokunursa**: Animasyon süresince (500ms) dokunma engellenir. Sonrasında normal.

- **If aşama SG verisi yüklenemezse (veri hatası)**: Düğüm "?" ikonu + gri kenarlık gösterir. Dokunulunca toast: "Veri yüklenemedi, tekrar dene."

## Dependencies

### Upstream

| Sistem | Veri | Arayüz |
|--------|------|--------|
| **Keşif Alanı Sistemi** | Aşama durumu (kilitli/açık/temizlenmiş), stage_SG, stage tipi, loot önizleme | `GetStageStatus(areaId, stageNum)`, `GetStageConfig(areaId, stageNum)` |
| **Pet Sistemi** | Aktif pet SG, pet mini portresi | `GetActivePet()` → {SG, portrait} |
| **Ekonomi** | Güncel enerji miktarı | `GetCurrentEnergy()` |
| **Canavar Veritabanı** | Düşman adı, tier'ı, first-encounter durumu | `GetMonsterIdentity(monsterId)` |
| **UI Framework** | Ekran stack, tab bar, toast, HUD | `UIManager.*` |
| **Kaydetme / Yükleme** | İlk temizleme durumu, açık aşamalar | `IsFirstClear(stageId)` |

### Downstream

| Sistem | Tetikleme |
|--------|-----------|
| **Savaş Sistemi** | "SAVAŞ" butonuna basılınca `StartBattle(petId, enemyId)` |
| **Savaş UI** | Savaş başlayınca ekran stack'e push edilir |
| **Keşif Alanı Sistemi** | Savaş sonrası `OnBattleComplete(result)` alınır, harita güncellenir |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `node_spacing_dp` | 88 | 72–110 | Harita çok uzun, scroll yorucu | Düğümler üst üste binir |
| `champion_node_size_dp` | 72 | 64–88 | Aşırı gösterişli | Normal'dan ayırt edilemez |
| `boss_node_size_dp` | 96 | 80–112 | Dev, orantısız | Şampiyondan fark edilemez |
| `patron_node_size_dp` | 96 | 88–120 | Alan patronu beklenenden büyük | Mini boss ile aynı görünür |
| `auto_scroll_duration_ms` | 400 | 200–600 | Yavaş, ekran geçiş gibi hisseder | Çok hızlı, nereye gittiği anlaşılmaz |
| `pulse_period_s` | 1.5 | 1.0–2.5 | Göze batıcı flaş hissi | Fark edilmez |
| `pulse_scale_max` | 1.06 | 1.03–1.10 | Rahatsız edici titreme | Hissedilmez |
| `unlock_anim_duration_ms` | 600 | 400–900 | Çok uzun, ritim bozuluyor | Fark edilmez |
| `detail_panel_open_ms` | 250 | 200–350 | Ağır his | Aniden fırlar |
| `detail_panel_close_ms` | 200 | 150–280 | Yavaş kapanma | Kesik his |
| `touch_debounce_ms` | 300 | 200–400 | Kasıtlı hızlı dokunmalar engellenir | Çift açılma bug'ı |
| `min_touch_target_dp` | 56 | 44–64 | Gereksiz büyük alanlar | Dokunma hatası artar |

## Acceptance Criteria

1. **GIVEN** harita ekranı açıldığında, **WHEN** Aşama 7 tamamlanmış Aşama 8 açık ama temizlenmemişse, **THEN** scroll otomatik olarak Aşama 8'i ekran ortasında konumlar (400ms animasyonu).

2. **GIVEN** player_SG=110, stage_SG=100, **WHEN** harita render edilirse, **THEN** o düğüm Yeşil kenarlık gösterir.

3. **GIVEN** player_SG=85, stage_SG=100, **WHEN** harita render edilirse, **THEN** o düğüm Kırmızı kenarlık gösterir.

4. **GIVEN** player_SG=60, stage_SG=100, **WHEN** harita render edilirse, **THEN** o düğüm Kırmızı-gri kilit (SG Kilitli) gösterir.

5. **GIVEN** Aşama 5 (Şampiyon) henüz sıralı kilitliyse, **WHEN** düğüme dokunulursa, **THEN** toast: "Aşama 4'ü temizlemeden bu aşamaya geçemezsin."

6. **GIVEN** Aşama 10 (Mini Boss) SG Kilitli, **WHEN** düğüme dokunulursa, **THEN** detay paneli açılır ama Savaş butonu gri "KİLİTLİ" ve "Savaş Gücün çok düşük." mesajı gösterilir.

7. **GIVEN** Aşama 10 ilk kez temizlenmemiş, **WHEN** detay paneli açılırsa, **THEN** "İlk Temizleme Bonusu: 🪙×[Nx3] 💎×10 + Garantili Canavar" gösterilir.

8. **GIVEN** Aşama 10 daha önce temizlenmiş, **WHEN** detay paneli açılırsa, **THEN** İlk Temizleme Bonusu bölümü yok; "✓ Tamamlandı" etiketi gösterilir.

9. **GIVEN** enerji < 2 (Mini Boss maliyeti), **WHEN** detay panelinde Savaş butonuna basılırsa, **THEN** savaş başlamaz; buton "ENERJİ YETERSİZ" olarak güncellenir.

10. **GIVEN** Alan Patronu (Aşama 20) düğümü, **WHEN** render edilirse, **THEN** 96×96 dp yıldız/kalkan şekli, patron ikonu gösterilir.

11. **GIVEN** Aşama 8 tamamlandıktan sonra Aşama 9 açılınca, **WHEN** harita güncellenir, **THEN** Aşama 9 düğümü gri'den renkli'ye 600ms fade + parlama animasyonu oynar.

12. **GIVEN** harita açıkken pet güçlendirilip player_SG yükselirse (Kırmızı → Sarı eşiğini geçerse), **WHEN** değişim kaydedilirse, **THEN** haritadaki o aşama düğümü 400ms fade ile Kırmızı'dan Sarı'ya geçer.

13. **GIVEN** savaş başlatıldığında, **WHEN** "SAVAŞ" butonuna basılırsa, **THEN** detay paneli 200ms kapanır, ardından savaş ekranı 250ms soldan girer, Tab Bar kilitlenir.

14. **GIVEN** savaş bitip Victory ile haritaya dönüldüğünde, **WHEN** harita ekranı tekrar aktif olursa, **THEN** tamamlanan düğümde altın yıldız belirir; toast "Aşama [N] temizlendi! ⭐" gösterilir.

15. **GIVEN** aktif pet yokken harita açılırsa, **WHEN** herhangi bir düğüme dokunulursa, **THEN** "Önce aktif bir pet seç" mesajı gösterilir. Savaş başlatılamaz.

16. **GIVEN** ReducedMotion açıkken harita açılırsa, **WHEN** ekran render edilirse, **THEN** pulse animasyonu çalışmaz, auto-scroll animasyonsuz (anında) gerçekleşir, unlock animasyonu anında renk değişimdir.

17. **GIVEN** tüm 20 aşama temizlenmiş ve Tier 2 alanı yokken harita açılırsa, **WHEN** render edilirse, **THEN** harita en üstünde "Alan Tamamlandı ✓" banner'ı gösterilir. Tüm düğümler altın yıldırlı ve hâlâ dokunulabilir (tekrar loot için).

18. **GIVEN** detay paneli açıkken panel dışına dokunulursa, **WHEN** dokunma alınırsa, **THEN** panel 200ms aşağı kayarak kapanır. Harita etkileşimleri yeniden aktif olur.

19. **GIVEN** arka arkaya 2 düğüme 150ms içinde dokunulursa, **WHEN** ikinci dokunma gelirse, **THEN** ikinci dokunma reddedilir (debounce 300ms). Yalnızca ilk düğümün paneli açılır.

20. **GIVEN** Şampiyon düğümü (Aşama 5) temizlenmiş ve player_SG yüksek (Yeşil), **WHEN** render edilirse, **THEN** 72×72 dp taç ikonu + altın yıldız + yeşil kenarlık aynı anda gösterilir.
