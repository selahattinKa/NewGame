# Savaş UI

> **Status**: Designed
> **Author**: user + ux-designer, ui-programmer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon

## Overview

**Savaş UI**, Savaş Sistemi'nin tüm görsel katmanıdır. Oyuncu + Pet vs Düşman savaşını dört dikey bölgede (Düşman Zonu, Savaş Alanı, Oyuncu Zonu, Pet Zonu) sunar; alt kenar boyunca uzanan Aksiyon Çubuğu'nda oyuncu sınıf yeteneklerini (4 slot), pet yeteneği göstergesini ve iksir butonunu barındırır. HUD (altın/elmas/enerji) savaş süresince gizlenir; ekranın sol üstünde Mod Toggle (Komutan/Otofarm), sağ üstünde Hız (1x/2x/3x) ve Çekil butonu yer alır. Tab Bar görünür ama kilitli kalır (UI Framework Kural 9).

**Oyuncu ana savaşçıdır.** Düşman oyuncuyu hedef alır; oyuncu HP barı ekranın odağındadır. Pet ikincil saldırgan olarak daha küçük bir alanda gösterilir. Defeat = Oyuncu HP = 0.

Komutan modunda Aksiyon Çubuğu etkileşimli; oyuncu doğru anda doğru butona basar. Otofarm modunda butonlar gri/pasif; her şey otomatik ilerler. Her tur aktif birimin zonunda renkli çerçeve belirir; hasar sayıları yüzen metin olarak hedefe yakın çıkar. Savaş sonu Victory veya Defeat overlay'i sahnede kaybolmadan açılır.

MVP kapsamında tüm savaş layout'u, skill butonları, hasar sayıları, status efekt ikonları, tur göstergesi, enerji barı, iksir butonu, Komutan/Otofarm mod farkı, Victory/Defeat overlay ve savaş ödül ekranı yer alır.

## Player Fantasy

Savaş UI'da oyuncu kendini **savaşın içinde** hisseder. Büyük sprite'lar, hasar sayılarının patlaması ve ekran sarsıntısı onu seyirci değil, aktif katılımcı yapar.

**Zamanlamanın hazzı**: Komutan modunda Slot 3 butonu parladığında, düşmanın bir sonraki turda büyük saldırı yapacağını görüyorsun — tam o anda patin yeteneğini basan "iyi oynadım" hissi. UI bu anı net sunmalı: her şey okunaklı, her buton durumu açık.

**Büyüme görünür**: Yeni pette hasar sayıları "59" iken haftalarca sonra "217" yazıyor. Ekranda beliren sayı güçlenmenin kanıtıdır.

**Otofarm sessizliği**: Auto modunda savaş kendi kendine ilerler. Oyuncu telefonu indirip geri gelir — loot hazır, VFX'ler oynanmış, düşman yenilmiş. Mod ikonu "AUTO" yazar, başka bir şey yoktur; altta bir "ceza göstergesi" yoktur.

**Negatif fantezi (kaçınılacak)**: Kalabalık UI, konsantrasyonu kırar. Özellikle Komutan modunda gereksiz bilgi oyuncuyu bunaltır. Aksiyon çubuğu temiz: dokunulacak buton parlak, bekleyecek buton soluk. Hasar sayıları 1-2 saniye sonra kaybolur. DoT ve status ikonları küçük tutulur — sahneyi ele geçirmez.

## Detailed Rules

### Core Rules

**Kural 1 — Genel Ekran Layout'u**

Tam ekran dikey (portrait). HUD katmanı gizli. Tab Bar görünür ama `LockTabs()` ile disabled.

```
┌────────────────────────────────┐  ← 390 dp genişlik (referans)
│ [◀ MOD]  [SPD]        [✕ KAÇ] │  Üst Çubuk — 48 dp
├────────────────────────────────┤
│                                │
│   DÜŞMAN ZONU                  │  ~160 dp
│   [El] Monster Adı   SG: 300   │   ↑ 28 dp başlık
│        [ SPRITE ]              │   ↑ ~90 dp sprite
│   HP ████████████░░░ 240/300   │   ↑ 28 dp HP barı
│   [status ikon satırı]         │   ↑ 14 dp
│                                │
├────────────────────────────────┤
│                                │
│   SAVAŞ ALANI                  │  ~140 dp
│   (animasyonlar, hasar sayılar)│
│                                │
├────────────────────────────────┤
│                                │
│   OYUNCU ZONU          ◀ ANA   │  ~130 dp
│   [Avatar/Sınıf İkonu]         │
│   HP ████████████░░  200/300 [🧪] │  ← Büyük HP barı + İksir
│   [status ikon satırı]         │
│                                │
├────────────────────────────────┤
│   PET ZONU (ikincil)           │  ~80 dp
│   [Pet İkonu] Pet Adı          │
│   HP ██████░░ 150/200          │  ← Küçük HP barı
│   EN ████░░░░  50/100          │  ← Enerji barı
├────────────────────────────────┤
│ [SK0] [SK1] [SK2] [SK3] [PET▣]│  Aksiyon Çubuğu — 88 dp
├────────────────────────────────┤
│ [Ana] [Keşif] [Koleksiyon]..  │  Tab Bar — 56 dp (kilitli, gri)
└────────────────────────────────┘
```

- Toplam içerik alanı: 48 + 160 + 140 + 130 + 80 + 88 = 646 dp + safe area + Tab Bar.
- Oyuncu HP barı ekranın en büyük ve görünür HP barıdır — oyuncu ana savaşçıdır.
- Tüm dokunma hedefleri minimum 64×64 dp (Aksiyon Çubuğu butonları).

---

**Kural 2 — Üst Çubuk**

| Eleman | Pozisyon | Boyut | Davranış |
|--------|----------|-------|----------|
| **Mod Toggle** | Sol | 100×40 dp | Komutan modu: altın "⚔ KOMUTAN". Otofarm: gri "🔄 AUTO". Dokunulunca mod değişir (Savaş Sistemi Kural 10). |
| **Hız Butonu** | Orta | 64×40 dp | "1×" → "2×" → "3×" → "1×" döngüsü. Her dokunmada değişir. |
| **Çekil Butonu** | Sağ | 80×40 dp | "✕ KAÇIN" kırmızı. Dokunulunca onay diyaloğu (Kural 11). |

---

**Kural 3 — Düşman Zonu**

Ekranın üst bölümü. Düşman oyuncuyu hedef alır; hasar sayıları Oyuncu Zonu'nda belirir. Bu zonun arka planı düz koyu — sprite odak noktasıdır.

| Eleman | Detay |
|--------|-------|
| **Başlık Satırı** | Element ikonu (küçük, renkli) + Monster Adı (font orta) + SG değeri (sağ köşe, küçük, gri) |
| **Sprite** | Merkezi hizalı. Boyut: normal monster ~110×110 dp, boss ~130×130 dp. |
| **HP Barı** | Kırmızı/turuncu dolgu, soldan sağa. Anlık değer metni: "240/300" sağda. Hasar gelince bar hızlı azalır (200ms tween). |
| **Status İkon Satırı** | HP barının altında. Küçük 20×20 dp ikonlar, soldan sağa aktif efektler. Max 6 ikon; 7+: son ikona "..." |
| **Aktif Tur Çerçevesi** | Düşman turu aktifken zone'un etrafında kırmızı glow border (4 dp, pulsing). |

**Düşman HP barı rengi**:
- HP > %50: Turuncu
- HP %25–50: Kırmızı
- HP < %25: Kıpkırmızı + hafif pulse

---

**Kural 4 — Savaş Alanı**

Ortadaki boş alan; statik arka plan yok, sadece animasyonlar ve yüzen elementler bu katta render edilir.

| Eleman | Detay |
|--------|-------|
| **Hasar Sayıları** | Hedefe yakın belirip yukarı süzülerek kaybolur. 0.8s görünür, 0.3s fade. |
| **Tur Sırası Göstergesi** | Sağ alt köşede 3 küçük ikon (Pet / Oyuncu / Düşman), aktif olan büyük ve parlak. Soldan sağa sıralı, her tur güncellenir. |
| **Animasyon Canvas** | Slash efektleri, element dalgaları, iyileştirme ışınları bu katmanda oynar. |

**Hasar sayısı tipleri**:

| Tip | Renk | Boyut | Ek |
|-----|------|-------|-----|
| Fiziksel normal | Beyaz | 28 sp | — |
| Fiziksel kritik | Altın | 40 sp | "KRIT!" ek etiketi |
| Büyü | Mor/mavi | 28 sp | — |
| Büyü kritik | Açık mor | 40 sp | "KRIT!" |
| İyileştirme | Yeşil | 28 sp | "+" öneki |
| DoT Yanma | Turuncu | 22 sp | Küçük 🔥 ikonu |
| DoT Zehir | Sarı-yeşil | 22 sp | Küçük ☠ ikonu |
| DEF aşımı (min 1) | Gri | 18 sp | — |

---

**Kural 5 — Oyuncu Zonu**

Düşman ve Savaş Alanı'nın hemen altı. Oyuncu ana savaşçı olduğundan bu zon ekranın en önemli bilgi alanıdır.

| Eleman | Detay |
|--------|-------|
| **Sınıf Avatarı** | Sol köşede küçük (48×48 dp) sınıf ikonu/avatarı. Sınıf renginde çerçeve. |
| **Sınıf Adı** | Avatar yanında, küçük metin. |
| **HP Barı** | Geniş (ekran genişliğinin ~80%'i), 18 dp yüksek. Kırmızı dolgu. Sağda "200/300" metni. Hasar: hızlı azalır (200ms tween). İyileşme: yavaş dolar (400ms). |
| **İksir Butonu [🧪]** | HP barının sağında, 44×44 dp. Envanterdeki toplam iksir adetini küçük rozet olarak gösterir. Basılınca Pot Panel açılır (Kural 6b). |
| **Status İkon Satırı** | HP barının altında. |
| **Aktif Tur Çerçevesi** | Oyuncu turu aktifken zone etrafında altın glow border. |

**Oyuncu HP barı rengi**:
- HP > %50: Kırmızı (canlı)
- HP %25–50: Turuncu
- HP < %25: Sarı + hızlı pulse ("tehlike" uyarısı)

---

**Kural 6a — Pet Zonu (İkincil)**

Oyuncu Zonu'nun hemen altı, Aksiyon Çubuğu'nun üstünde. Pet ikincil saldırgan — daha kompakt görünüm.

| Eleman | Detay |
|--------|-------|
| **Pet İkonu** | Sol köşede 40×40 dp küçük sprite/ikon. Element rengi kenarlık. |
| **Pet Adı** | İkon yanında, küçük metin. |
| **HP Barı** | Daha ince (12 dp yüksek), yeşil dolgu. Sağda "150/200". |
| **Enerji Barı** | HP barının altında, 10 dp yüksek, mavi. Sağda kalan enerji. |
| **Status İkon Satırı** | Enerji barının altında, küçük ikonlar. |
| **Aktif Tur Çerçevesi** | Pet turu aktifken zone etrafında mavi glow border. |

**Pet HP barı rengi**:
- HP > %50: Yeşil
- HP %25–50: Sarı
- HP < %25: Kırmızı (devre dışı riski — defeat değil ama saldırı kesilir)

**Enerji barı**: Mavi. 100'de dolar, %100'de altın parlama + PET▣ buton pulse başlar.

---

**Kural 6b — İksir (Pot) Paneli**

[🧪] butonuna basılınca Oyuncu Zonu üzerine küçük panel (bottom sheet değil, overlay panel):

```
┌──────────────────────┐
│ 🧪 İksir Kullan      │
│                      │
│ [Küçük İksir]  x 3   │  +30% HP — "KULLAN"
│ [Büyük İksir]  x 1   │  +70% HP — "KULLAN"
│                      │
│        [İPTAL]       │
└──────────────────────┘
```

- Paneli açmak turları durdurmaz (serbest aksiyon).
- "KULLAN" → oyuncu HP anında iyileşir, adet 1 azalır, panel kapanır.
- Stok 0/0 ise: satır gri + "Stok Yok".
- Panel açıkken savaş devam eder (oyuncu iksir kullanmayı geciktirebilir).

---

**Kural 7 — Aksiyon Çubuğu (Commander vs Auto)**

Ekran en altında, Tab Bar'ın hemen üstünde. Yatay dizi: **[SK0] [SK1] [SK2] [SK3] [PET▣]**.

Her buton: 64×64 dp. Aralarında 8 dp boşluk.

**Pet Yetenek Butonu [PET]**:

| Durum | Görsel |
|-------|--------|
| Energy 0–99 | Radial fill daire (mavi dolgu, saat yönünde ilerler). Ortada "X/100" metin. Tıklanamaz. |
| Energy = 100 (Commander) | Tam dolmuş + altın/mor pulse animasyonu. Tıklanabilir. |
| Energy = 100 (Auto) | Tam dolmuş + gri overlay + "AUTO" etiketi. Tıklanamaz. |

**Oyuncu Sınıf Butonları [SK0]–[SK3]**:

| Durum | Görsel |
|-------|--------|
| Hazır (CD=0, Commander, oyuncu turu) | Parlak, tam renkli, sınıf rengiyle. Tıklanabilir. |
| Hazır (CD=0, Commander, oyuncu turu DEĞİL) | Yarı saydam (%70 opacity). Tıklanabilir (önceden basılabilir — tur gelince uygulanır). |
| Bekleme (CD>0) | Soluk. Ortada büyük sayı "3" (kalan tur). Tıklanamaz. |
| Auto modu | Tüm butonlar gri overlay + "AUTO". Tıklanamaz. |

**Sınıf renk kodları** (buton çerçevesi ve ikon tonu):
- Savaşçı: Kırmızı-kahverengi
- Büyücü: Mor
- Hırsız: Koyu yeşil
- Şifacı: Açık mavi

**Buton içeriği**: İkonun altında yetenek adının kısaltması (2-3 harf) ve varsa CD sayısı.

---

**Kural 7 — Tur Sırası Göstergesi**

Savaş Alanı'nın sağ alt köşesinde 3 ikon (28×28 dp her biri) yatay sırada:

```
[Pet İkonu] [Oyuncu İkonu] [Düşman İkonu]
```

- Aktif tur: İkon büyük (36×36 dp) + altın çerçeve.
- Sıradaki tur: Normal boyut + gri çerçeve.
- Üçüncü: Normal boyut + soluk.
- Her tur başı tur sırası güncellenir (SPD hesabına göre).

---

**Kural 8 — Status Efekt İkonları**

Her birim için HP/enerji barlarının altında ikon satırı:

| Efekt | İkon | Renk | Süre Göstergesi |
|-------|------|------|-----------------|
| Sersemletme | ⚡ | Sarı | Kalan tur sayısı küçük köşe etiketi |
| DEF Kırma | 🛡↓ | Turuncu | Kalan tur |
| ATK Zayıflatma | ⚔↓ | Sarı-gri | Kalan tur |
| Kalkan | 🔵 | Mavi | Kalan kalkan HP / kalan tur |
| Kesin Kritik | ⭐ | Altın | "1" (tek kullanım) |
| Yanma DoT | 🔥 | Turuncu | Kalan tur |
| Zehir DoT | ☠ | Yeşil-sarı | Kalan tur |
| DEF Buff (kendi) | 🛡↑ | Yeşil | Kalan tur |

İkon boyutu: 20×20 dp. Kalan tur etiketi: 10 sp küçük sayı, ikonun sağ alt köşesinde.

---

**Kural 9 — Komutan Modu Görsel Farkları**

Komutan modu aktifken ek görsel göstergeler:

- Üst Çubuk: "⚔ KOMUTAN" altın renkli.
- Pet Zonu: Sprite etrafında ince altın glow (4 dp, sürekli, soluk — gösterişli değil, nüanslı).
- Aksiyon Çubuğu: Butonlar parlak ve etkileşimli.
- Oyuncu turu gelince: Aksiyon Çubuğu hafif yukarı 4 dp kayar (spring animasyon, 150ms) — "sıra sende" hissi.

Otofarm modunda:
- "🔄 AUTO" gri.
- Pet aura yok.
- Aksiyon Çubuğu tüm butonlar üzerinde hafif gri overlay + "AUTO" etiketi.

---

**Kural 10 — Savaş Başlangıç Animasyonu**

1. Ekran siyah açılır (fade-in 300ms).
2. Pet soldan giriş — 300ms ease-out, hedef pozisyona yerleşir.
3. Düşman sağdan giriş — 300ms ease-out, eş zamanlı.
4. 200ms bekleme.
5. Kısa "VS" flash (200ms, sonra kaybolur).
6. İlk tur başlar: aktif birimin zone border'ı parlama animasyonu.

ReducedMotion: Giriş animasyonları atlanır, birimler anında pozisyonlarında.

---

**Kural 11 — Çekilme Onay Diyaloğu**

"✕ KAÇIN" butonuna basılınca oyun duraklar (tur dondurulur), modal açılır:

```
┌──────────────────────────────┐
│     Savaştan Çekilmek        │
│       İstiyor Musun?         │
│                              │
│  Enerji harcanmaz.           │
│  Loot düşmez.                │
│                              │
│  [EVET, ÇEKİL]  [DEVAM ET]  │
└──────────────────────────────┘
```

- "EVET, ÇEKİL": Savaş biter → PostCombat → Harita ekranına dön.
- "DEVAM ET": Modal kapanır, savaş kaldığı yerden devam.
- ReducedMotion dahil tüm durumlarda tur gerçekten duraklatılır.

---

**Kural 12 — Victory Overlay**

Düşman HP=0 olunca savaş durur, Victory overlay sahneye eklenir (yeni ekrana geçilmez — overlay):

```
┌──────────────────────────────────┐
│  ✦ Z A F E R ! ✦                │  ← Altın büyük metin, parçacık efekti
│                                  │
│  [EXP barı dolma animasyonu]     │
│  +420 EXP  Lv.8 → Lv.9 (varsa) │
│                                  │
│  LOOT:                           │
│  🪙 350 Altın                    │
│  [Canavar kartı varsa]           │
│  [Evrim taşı varsa]              │
│                                  │
│  [      D E V A M      ]         │  ← Büyük yeşil buton
└──────────────────────────────────┘
```

- Loot öğeleri sırayla belirir (her biri 200ms arayla) — nadirlik sırasına göre (düşüktan yükseğe, en nadir en son, beklentiyi artırır).
- "Devam" butonu loot animasyonu bitmeden de basılabilir (animasyonu skip eder).
- Ekran dokunulunca loot animasyonu skip edilir, doğrudan son duruma atlanır.
- Overlay kapanırken 300ms fade — harita ekranına dönülür.

---

**Kural 13 — Defeat Overlay**

Oyuncu HP = 0 olunca:

```
┌──────────────────────────────────┐
│                                  │
│       Y E N İ L D İ N            │  ← Kırmızı metin
│                                  │
│  Enerji harcanmadı.              │
│                                  │
│  [YENİDEN DENE]  [ÇEKİL]        │
└──────────────────────────────────┘
```

- Arka plan: ekran %70 karartması (dim overlay).
- "Yeniden Dene": Savaşı sıfırdan başlatır. Aynı düşman, yeni savaş.
- "Çekil": Savaş biter, harita ekranına dönülür.
- Kaybetmede loot ekranı gösterilmez (Savaş Sistemi ile uyumlu).

---

**Kural 14 — EXP Bar Animasyonu (Victory)**

Victory overlay'de oyuncu EXP barı gösterilir:

- Mevcut EXP değerinden hedef EXP değerine 600ms ease-out dolar.
- Level-up gerçekleşirse: bar sona ulaşınca kısa gold flash → bar sıfırlanır → yeni seviyede kalan EXP'ye kadar tekrar dolar.
- Level-up metni: "+1 SEVİYE!" overlay'de kaybolmadan soluk görünür, 1s sonra kaybolur.

---

### States and Transitions

```
[PreCombat]
    ↓ start anim (600ms)
[Combat — Active]
    ├─ [Komutan: Oyuncu Turu] ← skill butonları aktif, aksiyon çubuğu yükselir
    ├─ [Komutan: Pet Turu]    ← pet zone border mavi, pet energy pulse
    ├─ [Komutan: Düşman Turu] ← enemy zone border kırmızı
    ├─ [Auto: Tüm Turlar]     ← butonlar pasif, animasyonlar devam
    ├─ → Victory Overlay
    ├─ → Defeat Overlay
    └─ → Retreat Dialog
[Victory Overlay]
    ↓ Devam
[Harita Ekranı]

[Defeat Overlay]
    ├─ Yeniden Dene → [PreCombat]
    └─ Çekil → [Harita Ekranı]

[Retreat Dialog]
    ├─ Evet → [Harita Ekranı]
    └─ Devam Et → [Combat — Active] (kaldığı yerden)
```

## Formulas

### Formül 1: HP Bar Doluluk Oranı

```
fill_ratio = current_hp / max_hp          // 0.0–1.0
bar_width_dp = floor(fill_ratio × max_bar_width_dp)
```

HP barı genişliği: ~280 dp (güvenli alan sonrası).
Tween: `current_display_hp` hedef HP'ye doğru 200ms ease-out.

### Formül 2: Enerji Barı Doluluk Oranı

```
fill_ratio = current_energy / 100
bar_width_dp = floor(fill_ratio × max_bar_width_dp)
```

Enerji barı yüksekliği 14 dp, HP barının altında. Tween: energy += 25/tur → anlık artış (200ms).

### Formül 3: Radial Fill (Pet Yetenek Butonu)

```
fill_angle = (current_energy / 100) × 360°
```

Saat yönünde 0°'den başlar, 360° = tam dolu.

### Formül 4: Hasar Sayısı Boyutu (Kritik)

```
display_size = is_crit ? 40 sp : 28 sp
```

DoT sayıları 22 sp — ikincil hasar, odak kırmamalı.

### Formül 5: EXP Bar Animasyon Süresi

```
fill_duration_ms = max(400, min(1200, delta_exp / max_exp_per_level × 1200))
```

Küçük EXP artışı 400ms, tam bar dolumu 1200ms — monoton değil.

### Formül 6: CD Counter Güncelleme

```
displayed_cd = current_cd   // her tur başı güncellenir
// current_cd = 0 ise buton parlak, 0'dan büyükse soluk + sayı gösterir
```

## Edge Cases

- **If komutan modunda oyuncunun turu sırasında 5+ saniye buton basılmazsa**: Slot 0 (CD0) otomatik kullanılır (Savaş Sistemi Kural 4). UI: "AUTO-SKIP" küçük etiketi 0.5s belirir, yetenek uygulanır.

- **If enerji 100'de ve oyuncu pet butonuna defalarca basarsa**: İlk dokunma yeteneği tetikler, sonrakiler debounce (300ms) ile reddedilir. Double-tap bug'ı oluşmaz.

- **If Victory overlay açıkken oyuncu hızlıca "Devam"a basarsa**: Loot animasyonu skip edilir, doğrudan son duruma geçilir. Hasar metrikleri veya loot kaybolmaz.

- **If Defeat overlay açıkken "Yeniden Dene"ye basılır ama enerji yoksa**: Enerji maliyeti kaybetmede alınmaz — yeniden deneme ücretsiz. Enerji kontrolü yapılmaz.

- **If çekilme diyaloğu açıkken uygulama arka plana geçerse**: Diyalog açık kalır. Geri dönünce aynı durum — savaş dondurulmuş.

- **If Pet HP animasyonu tween sırasında ek hasar gelirse**: Tween hedef değeri güncellenir; mevcut display position'dan yeni hedefe doğru 200ms tween yeniden başlar. Bar geriye gitmez (iyileşme yoksa).

- **If aynı anda hem DoT hem fiziksel hasar aynı turda gelirse**: İki ayrı hasar sayısı aynı bölgede belirir; sağa/sola hafif offset uygulanır. Üst üste basan sayılar okunmaz olmamalı.

- **If status ikon satırında 7+ efekt aktifse**: İlk 5 ikon görünür, 6. ikonda "...+N" etiketi (N = geri kalan efekt sayısı). Dokunulunca tüm efektleri gösteren küçük tooltip.

- **If Hırsız Suikast Fırtınası 5 vuruş yaparken her biri krit olursa**: 5 ayrı "KRIT!" sayısı yüzüp kaybolur. Performans: sayı animasyonları pooling ile yönetilir (max 10 eş zamanlı sayı objesi).

- **If savaş 3x hızda çalışırken oyuncu 1x'e geçerse**: Mevcut tur animasyonu kalan süresini 1x tempoyla tamamlar. Ani atlama olmaz.

- **If ReducedMotion aktifse**: Giriş animasyonları yok (birimler anında), hasar sayıları yüzmez (anlık belirir/kaybolur), EXP barı anında dolar, overlay'ler fade yoktur. Savaş okunabilirliği korunur.

## Dependencies

### Upstream

| Sistem | Veri | Arayüz |
|--------|------|--------|
| **Savaş Sistemi** | Her tur başı/sonu event'leri, aksiyon sonuçları | `OnTurnStart(unitId)`, `OnActionExecuted(result)`, `OnBattleEnd(outcome)`, `OnModeChanged(mode)` |
| **Sağlık / Can Sistemi** | HP değişimleri (real-time) | `OnHPChanged(unitId, newHP, maxHP)` |
| **Oyuncu Sınıf Sistemi** | Sınıf adı, slot yetenekleri, CD durumları, SPD | `GetClassInfo()` → {name, slots[], color} |
| **Pet Sistemi** | Aktif pet adı, element, tier, sprite referansı, SG | `GetActivePet()` → {name, element, tier, sprite, SG, energy} |
| **Canavar Veritabanı** | Düşman adı, element, tier, sprite referansı, SG | `GetMonsterIdentity(enemyId)` |
| **Loot / Ödül Sistemi** | Savaş sonu loot listesi, EXP miktarı | `GetBattleReward(battleResult)` → {lootItems[], exp} |
| **UI Framework** | Stack navigasyon, modal, toast | `UIManager.*` |

### Downstream

| Sistem | Tetikleme |
|--------|-----------|
| **Savaş Sistemi** | Buton dokunmaları → `ExecutePlayerAction(slotId)`, `ExecutePetAbility()`, `SetMode()`, `SetSpeed()`, `Retreat()` |
| **Keşif Alanı** | "Devam" → `OnBattleComplete(result)` üzerinden harita güncellemesi |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `hp_tween_ms` | 200 | 100–400 | Yavaş, bar "akıyor" hissi | Anlık atlama, hasar hissiz |
| `heal_tween_ms` | 400 | 200–600 | Yavaş "iyileşiyor" | Anlık, sihir hissi kaybolur |
| `damage_num_duration_ms` | 800 | 500–1200 | Ekran dolup taşar | Okumadan önce kayboluyor |
| `damage_num_float_dp` | 32 | 20–50 | Çok uzak uçuyor | Hedefe çok yakın kalıyor |
| `crit_font_sp` | 40 | 32–52 | Aşırı gösterişli | Normal'dan ayırt edilemez |
| `normal_font_sp` | 28 | 22–34 | Okunabilir ama büyük | Küçük ekranda okunmaz |
| `dot_font_sp` | 22 | 16–26 | Normal hasar kadar büyük, odak kırar | Çok küçük, görünmez |
| `energy_pulse_period_ms` | 800 | 600–1200 | Çok hızlı, rahatsız edici | Fark edilmez |
| `unit_entry_anim_ms` | 300 | 200–500 | Savaş geç başlar | Birimler aniden beliriyor |
| `victory_loot_item_delay_ms` | 200 | 100–400 | Çok yavaş açılıyor | Tüm loot aynı anda beliriyor |
| `exp_bar_max_anim_ms` | 1200 | 800–2000 | Kazanım hissi uzuyor | EXP anında doluyor, tatminsiz |
| `aksiyon_raise_dp` | 4 | 2–8 | Buton çubuğu zıplıyor | Fark edilmez |
| `auto_skip_wait_s` | 5 | 3–10 | Uzun bekleme, tempo bozulur | Çok hızlı, kasıtsız skip |

## Acceptance Criteria

1. **GIVEN** savaş başlarken, **WHEN** PreCombat tamamlanırsa, **THEN** pet soldan, düşman sağdan 300ms ease-out giriş animasyonu oynar. HP barları full, enerji barı boş gösterilir.

2. **GIVEN** Pet turu aktif, **WHEN** tur başlarsa, **THEN** Pet Zonu'nda mavi glow border pulse başlar, tur sırası göstergesinde pet ikonu büyür.

3. **GIVEN** Düşman turu aktif, **WHEN** tur başlarsa, **THEN** Düşman Zonu'nda kırmızı glow border pulse başlar.

4. **GIVEN** Oyuncu turu aktif (Komutan), **WHEN** tur başlarsa, **THEN** Aksiyon Çubuğu 4 dp yukarı kayar (spring 150ms). CD=0 slotlar parlak, CD>0 slotlar soluk + CD sayısı gösterir.

5. **GIVEN** Slot 2 (CD5) kullanıldı, **WHEN** tur başlarsa, **THEN** SK2 butonu soluk + ortada "5" gösterir. Her tur "5"→"4"→"3"→"2"→"1"→"0" (parlak) iner.

6. **GIVEN** pet energy 75'ten 100'e çıktı (Commander), **WHEN** EnergyPhase çalışırsa, **THEN** enerji barı tam dolar, PET butonu altın pulse başlar ve tıklanabilir olur.

7. **GIVEN** PET butonu full + Commander, **WHEN** oyuncu butona basarsa, **THEN** yetenek uygulanır, PET butonu enerji sıfırlanır (0/100 radial fill).

8. **GIVEN** Auto modu, **WHEN** tüm butonlar render edilirse, **THEN** SK0–SK3 ve PET butonu üzerinde gri overlay + "AUTO" etiketi. Dokunuşlara yanıt yok.

9. **GIVEN** Komutan → Auto geçişi yapıldı, **WHEN** mod toggle basıldıktan sonraki turda, **THEN** butonlar pasife geçer, üst çubuk "🔄 AUTO" gri gösterir, pet sprite aura kaybolur.

10. **GIVEN** fiziksel kritik hasar, **WHEN** hasar uygulanırsa, **THEN** altın renk 40 sp "217" + "KRIT!" etiketi belirir, hedefe yakın 32 dp yukarı süzülür, 800ms sonra kaybolur.

11. **GIVEN** Yanma DoT tiki, **WHEN** DoTPhase çalışırsa, **THEN** turuncu 22 sp "🔥 15" belirir ve kaybolur. HP barı 200ms tween ile azalır.

12. **GIVEN** Savaşçı Slot 1 Sersemletme uygulandı, **WHEN** düşman status göstergesi güncellenir, **THEN** ⚡ ikonu + "1" köşe etiketi belirir. Bir sonraki turda güncellenir veya kalkar.

13. **GIVEN** düşman HP %20'ye düştü, **WHEN** HP bar güncellenir, **THEN** bar kıpkırmızı + hafif pulse animasyonu başlar.

14. **GIVEN** Victory tetiklendi, **WHEN** son hasar uygulandı, **THEN** 300ms: altın parçacık patlaması + "✦ ZAFER! ✦" metin. EXP barı 600ms–1200ms ease-out dolar. Loot öğeleri 200ms arayla belirir (düşüktan yükseğe nadirlik).

15. **GIVEN** Victory overlay açık, **WHEN** "Devam"a basılırsa, **THEN** overlay 300ms fade ile kapanır, harita ekranına dönülür.

16. **GIVEN** Defeat tetiklendi, **WHEN** oyuncu HP=0 olursa, **THEN** ekran %70 dim overlay, "YENİLDİN" kırmızı metin, "Enerji harcanmadı." satırı, iki buton: "YENİDEN DENE" + "ÇEKİL".

17. **GIVEN** Defeat overlay'de "Yeniden Dene"ye basıldı, **WHEN** basılırsa, **THEN** overlay kapanır, savaş PreCombat'tan başlar (HP full, energy=0, CD=0).

18. **GIVEN** "✕ KAÇIN" butonuna basıldı, **WHEN** basılırsa, **THEN** oyun duraklar + "Savaştan Çekilmek İstiyor Musun?" diyaloğu açılır. "DEVAM ET": diyalog kapanır, kaldığı yerden devam. "EVET, ÇEKİL": harita ekranına dön.

19. **GIVEN** ReducedMotion aktif, **WHEN** savaş başlarsa, **THEN** giriş animasyonu yok (birimler anında pozisyonunda). Hasar sayıları yüzmez (anında beli̇rir 800ms sonra kaybolur). EXP barı anında dolar.

20. **GIVEN** 7 aktif status efekti aynı birimde, **WHEN** status satırı render edilirse, **THEN** ilk 5 ikon gösterilir, 6. ikonun yerinde "...+2" etiketi. Etikete dokunulunca tüm 7 efekt tooltip'te listelenir.
