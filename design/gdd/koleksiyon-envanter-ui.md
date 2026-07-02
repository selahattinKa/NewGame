# Koleksiyon / Envanter UI

> **Status**: Designed
> **Author**: user + ux-designer, ui-programmer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Topla Hepsini, Güç Hisset, Hep Bir Şey Var

## Overview

**Koleksiyon / Envanter UI**, oyuncunun sahip olduğu tüm petleri görüntülediği, aktif petini seçtiği, peti detaylarıyla incelediği ve pet yönetim işlemlerini (satma, evrim, yıldız yükseltme) başlattığı ekrandır. Tab Bar'ın "Koleksiyon" sekmesinden erişilir. HUD görünürdür (altın/elmas/enerji).

Ekran iki ana görünümden oluşur: **Izgara Görünümü** (tüm petler kart biçiminde, filtrelenebilir/sıralanabilir) ve **Pet Detay Paneli** (bir pete dokunulunca açılan tam bilgi sayfası — stat'lar, yetenek, aktif seçme, sat/evrimleştir). Pet Detay Paneli ayrı bir screen push'tur.

MVP kapsamında tüm petlerin listesi, filtre (tier / aktif), sıralama (SG / tier / yeni eklenen), pet detay sayfası, aktif pet seçme, satma onayı ve otomatik satış filtresi ayarı yer alır. Evrim ve yıldız yükseltme işlemleri Detay Paneli'nden Pet Evrim Sistemi UI'ına bağlanır (ayrı GDD).

## Player Fantasy

Oyuncu koleksiyon ekranında **müze direktörü** hissini yaşar. Topladığı yaratıklar sıralanmış, hepsi bir arada — "bak neler biriktirdim." Tier arttıkça kart çerçeveleri renkleniyor, yıldızlar parlıyor; B tier bir pet ızgarada diğerlerinin arasında altın çerçevesiyle öne çıkıyor.

**Seçim tatmini**: Aktif peti değiştirmek bir karar anıdır. Oyuncu farklı arketipler arasında gezinir, "bu sefer Büyücü denerim" der, peti seçer — harita ekranına döndüğünde SG renk kodu anında güncellenir.

**Büyüme görünür**: İlk günkü F tier Gölge Sıçanı hâlâ koleksiyonda, yanında şimdi C tier Demir Golem var. Izgara görsel büyümenin kaydıdır.

**Cömertliğin somut kanıtı**: Savaşlardan düşen her pet koleksiyona eklendi — ızgara dolup taşmadan önce oyuncunun satma kararı vermesi gerekiyor. Satma = küçük altın ödülü; koleksiyonu boş tutmak da bir strateji.

**Negatif fantezi (kaçınılacak)**: Uzun liste, sonsuz scroll, hiçbir şey öne çıkmaz. Filtre ve sıralama bu sorunu çözer. Ayrıca "sattım, geri alamam" pişmanlığı — güvenlik kilidi C ve B tier petleri kazara satmayı engeller.

## Detailed Rules

### Core Rules

**Kural 1 — Genel Ekran Layout'u**

```
┌────────────────────────────────┐
│  HUD (altın / elmas / enerji)  │
├────────────────────────────────┤
│  Üst Bar                       │
│  "Koleksiyon"  [Filtre▼][Sırala▼] │
├────────────────────────────────┤
│  Aktif Pet Satırı              │  ← Mevcut aktif pet özeti — 72 dp
├────────────────────────────────┤
│                                │
│  Izgara (2 sütun, scroll)      │  ← Kalan ekran
│  [Pet Kartı][Pet Kartı]        │
│  [Pet Kartı][Pet Kartı]        │
│       ⋮                        │
├────────────────────────────────┤
│  Tab Bar                       │
└────────────────────────────────┘
```

- **Üst Bar**: Başlık + sağda Filtre açılır menüsü + Sıralama açılır menüsü.
- **Aktif Pet Satırı**: Şu an aktif seçili petin mini özeti. Dokunulunca Detay Paneli açılır.
- **Izgara**: 2 sütun kart dizisi. Dikey scroll. Her kart 156×180 dp.

---

**Kural 2 — Pet Kartı**

```
┌───────────────────────┐
│ [Tier rozeti] [El ikon]│  ← 28 dp üst bar
│                        │
│     [ PORTRAIT ]       │  ← 96 dp kare (AI görseli)
│                        │
│  Pet Adı               │  ← 18 sp
│  SG: 255    ★★★☆☆☆     │  ← SG + yıldız sayısı
│  [AKTİF]  (varsa)      │  ← Aktif ise altın "AKTİF" etiketi
└───────────────────────┘
```

**Tier çerçeve rengi**:
| Tier | Çerçeve Rengi |
|------|--------------|
| F | Gri |
| D | Yeşil |
| C | Mavi |
| B | Altın |

**Aktif pet kartı**: Tüm kartların üstünde hafif altın glow arka plan. "AKTİF" etiketi sol alt köşede.

**Kart dokunma**: Tek dokunma → Pet Detay sayfasına push. Uzun basma (500ms) → Hızlı işlem menüsü (Kural 7).

---

**Kural 3 — Filtre Seçenekleri**

Filtre açılır menüsü çoklu seçime izin verir (multiselect):

| Filtre Grubu | Seçenekler |
|-------------|-----------|
| **Tier** | F / D / C / B |
| **Arketip** | Saldırgan / Tank / Destekçi / Büyücü |
| **Durum** | Aktif / Kilitsiz / Kilitli |

- Birden fazla filtre birlikte uygulandığında AND mantığı: tüm koşulları sağlayan petler gösterilir.
- Aktif filtre sayısı üst bardaki "Filtre" butonuna badge olarak eklenir: "Filtre (2)".
- "Filtreyi Temizle" butonu menü altında.

---

**Kural 4 — Sıralama Seçenekleri**

Tek seçim (radio):

| Seçenek | Açıklama |
|---------|----------|
| **SG (Yüksek→Düşük)** | Varsayılan |
| **SG (Düşük→Yüksek)** | |
| **Tier (Yüksek→Düşük)** | B → F |
| **Tier (Düşük→Yüksek)** | F → B |
| **Yeni Eklenen** | En son kazanılan önce |
| **İsim (A–Z)** | |

Filtre + sıralama birlikte çalışır. Seçim oturumlar arası kaydedilmez (her oturumda varsayılan: SG yüksekten düşüğe).

---

**Kural 5 — Aktif Pet Satırı**

Üst ızgaranın hemen altında, sabit bir satır. Şu an savaşa çıkacak peti gösterir.

```
┌───────────────────────────────────────────┐
│ [Mini Portrait 48dp] Pet Adı      SG: 255  │
│ [El ikon] [Tier] [★★★☆☆☆]  [DEĞİŞTİR →]  │
└───────────────────────────────────────────┘
```

- "DEĞİŞTİR →" butonu: Izgara'ya scroll edip bir pet seçmeyi hatırlatır (toast: "Değiştirmek için bir pet kartına uzan bas").
- Aktif pet yoksa: "Aktif pet seçilmedi — bir karta uzun bas veya Detay'dan seç."

---

**Kural 6 — Pet Detay Sayfası**

Bir karta dokunulduğunda yeni bir screen push'u ile açılır (soldan giriş, 250ms).

**Layout:**

```
┌────────────────────────────────┐
│ [← Geri]   Pet Adı    [Sat ⚙] │  ← Üst bar
├────────────────────────────────┤
│                                │
│     [BÜYÜK PORTRAIT — 160dp]   │
│     [Tier] [★★★☆☆☆]            │
│     SG: 255                    │
│                                │
├────────────────────────────────┤
│  STAT'LAR                      │
│  HP   ████░░ 250   ATK ████░░ 65│
│  DEF  ███░░░  35   SPD ██░░░░ 25│
├────────────────────────────────┤
│  YETENEĞİ                      │
│  [Yetenek ikonu] Güçlü Vuruş   │
│  ATK × 2.0 fiziksel hasar      │
│  Enerji: 4 turda 1 kez         │
├────────────────────────────────┤
│  SEVİYE & EVRİM                │
│  Lv. 12 / 50  [EXP barı]       │
│  ★ Evrim: 3/3 materyal ✓       │
├────────────────────────────────┤
│  [  AKTİF YAP  ]               │  ← Zaten aktifse: "AKTİF ✓" (disabled)
│  [  EVRİMLEŞTİR  ]             │  ← Malzeme yoksa disabled + "Malzeme Yok"
└────────────────────────────────┘
```

**Stat barları**: Her stat'ın o tier için maksimum değerine oranla görsel bar. Mutlak sayı yanında.

**Yetenek kartı**: Arketip ikonu + yetenek adı + tek satır açıklama + enerji bilgisi.

**Seviye & Evrim satırı**:
- Mevcut seviye / max seviye + EXP barı
- Evrim için gereken malzeme sayısı (X/Y materyal) + durum (✓ hazır / eksik)

**Aktif Yap butonu**: Bu pet zaten aktifse gri "AKTİF ✓". Aktif değilse yeşil "AKTİF YAP". Basılınca onaysız direkt seçilir, "AKTİF ✓" olur. Harita ekranındaki SG de güncellenir.

**Evrimleştir butonu**: Pet Evrim Sistemi UI'ına push. MVP'de evrim sayfası ayrı GDD kapsamı — bu buton sadece o sayfaya kapı görevi görür.

**Sat (⚙) butonu** (üst bar sağ): Kural 8.

---

**Kural 7 — Hızlı İşlem Menüsü (Uzun Basma)**

Izgara kartına 500ms uzun basıldığında küçük bir bottom sheet açılır:

```
┌──────────────────────┐
│ [Portrait+İsim]      │
│ ─────────────────    │
│ ⚔ Aktif Yap          │
│ 🔍 Detaya Git        │
│ 💰 Sat (100 🪙)      │
└──────────────────────┘
```

- C ve B tier petlerde "Sat" seçeneği kırmızı ve kilit ikonu ile gösterilir: "🔒 Sat (Kilitli)". Tıklanırsa toast: "C ve B tier petler güvenlik kilidi kapsamında. Detaydan kilidi kaldır, sonra sat."
- "Aktif Yap" kendi aktif pet olduğunda "AKTİF ✓" (disabled).

---

**Kural 8 — Satma İşlemi**

**Satma yolu 1**: Detay Sayfası → üst bar "⚙" → "Sat" seçeneği.
**Satma yolu 2**: Izgara'da uzun basma → "Sat" seçeneği.

Her iki yolda da onay diyaloğu açılır (F/D tier için):

```
┌──────────────────────────────────┐
│  Gölge Sıçanı'yı sat?            │
│                                  │
│  Kazanacağın: 🪙 100 Altın       │
│                                  │
│  [SAT]          [VAZGEÇ]         │
└──────────────────────────────────┘
```

C/B tier için onay diyaloğu açılmadan önce **güvenlik kilidi uyarısı** modali:

```
┌──────────────────────────────────┐
│  ⚠️ Nadir Pet Satışı             │
│                                  │
│  Demir Golem (C) tier bir pet.   │
│  Satmak geri alınamaz.           │
│                                  │
│  Onaylamak için "SATIYOR" yaz:   │
│  [______________]                │
│                                  │
│  [İPTAL]                         │
└──────────────────────────────────┘
```

"SATIYOR" (büyük/küçük harf duyarsız) yazılırsa "SAT" butonu aktifleşir.

**Aktif pet satışı engeli**: Şu an aktif olan pet satılamaz. Onay diyaloğunda "SAT" disabled + "Aktif pet olduğu için satılamaz. Önce başka bir pet aktif yap."

---

**Kural 9 — Otomatik Satış Filtresi Ayarı**

Koleksiyon ekranının üst bar'ında ⚙ (ayarlar) ikonu ile erişilir. Bottom sheet açılır:

```
┌────────────────────────────────────┐
│  Otomatik Satış Filtresi           │
│                                    │
│  Yeni kazanılan petleri otomatik   │
│  sat:                              │
│                                    │
│  [●] F Tier                        │
│  [●] D Tier                        │
│  [ ] C Tier  (korumalı)            │
│  [ ] B Tier  (korumalı)            │
│                                    │
│  C ve B tier otomatik satışa       │
│  açılamaz (güvenlik kilidi).       │
│                                    │
│  [KAYDET]                          │
└────────────────────────────────────┘
```

- C ve B tier toggle'ları disabled (gri) — açılamaz.
- Ayar `PlayerPrefs` ile kaydedilir.
- Varsayılan: F ve D Tier açık, C ve B kapalı (Pet Evrim Sistemi GDD ile uyumlu).

---

**Kural 10 — Boş Durum**

Koleksiyon hiç pet içermiyorsa (yeni oyuncu):

```
┌────────────────────────────────┐
│  Henüz pet yok.                │
│                                │
│  Keşif Alanı'nda savaşarak     │
│  pet kazanabilirsin!           │
│                                │
│  [Keşif Alanı'na Git →]        │
└────────────────────────────────┘
```

Buton: Tab Bar'da "Keşif" sekmesini aktif yapar.

---

### States and Transitions

```
[Izgara Görünümü]
    ├─ Kart dokunma → [Pet Detay Sayfası] (stack push)
    ├─ Uzun basma   → [Hızlı İşlem Bottom Sheet]
    ├─ Filtre       → [Filtre Bottom Sheet]
    ├─ Sıralama     → [Sıralama Bottom Sheet]
    └─ ⚙ (ayarlar) → [Otomatik Satış Ayarı Bottom Sheet]

[Pet Detay Sayfası]
    ├─ Aktif Yap → Anlık güncelleme (sayfa kapanmaz)
    ├─ Evrimleştir → [Evrim Sayfası] (stack push, ayrı GDD)
    └─ Sat → [Onay Diyaloğu] → Başarılı: stack pop (ızgaraya dön), toast "X Altın kazandın"

[Hızlı İşlem]
    ├─ Aktif Yap → Bottom sheet kapanır, Aktif Pet Satırı güncellenir
    ├─ Detaya Git → Bottom sheet kapanır → [Pet Detay Sayfası]
    └─ Sat → Bottom sheet kapanır → [Onay Diyaloğu]
```

## Formulas

### Formül 1: Izgara Kart Sayısı ve Scroll Yüksekliği

```
card_height_dp = 180
gap_dp = 8
total_cards = pet_count
rows = ceil(total_cards / 2)
scroll_height_dp = rows × (card_height_dp + gap_dp)
```

### Formül 2: Stat Bar Doluluk Oranı (Detay Sayfası)

```
// Her stat için tier max değeri referans alınır
fill_ratio = current_stat / tier_max_stat
bar_fill_dp = floor(fill_ratio × max_bar_width_dp)
// fill_ratio > 1.0 olabilir (buff ile); bar 100%'de cap'lenir, sayı gerçek değer gösterir
```

Tier max stat değerleri Canavar Veritabanı GDD'sinden alınır.

### Formül 3: EXP Bar Doluluk (Detay Sayfası)

```
fill_ratio = current_exp / exp_to_next_level
bar_fill_dp = floor(fill_ratio × max_bar_width_dp)
```

### Formül 4: Satış Fiyatı Gösterimi

```
sell_price = base_sell_price[tier]
// F=100, D=200, C=400, B=700 (Pet Evrim GDD'den)
// Yıldız bonusu varsa: sell_price × (1 + star_count × 0.1) — Pet Evrim GDD referansı
```

## Edge Cases

- **If aktif pet silinirse (satılırsa) ama izin verilmemişse**: Aktif pet satış engeli (Kural 8) bunu zaten önler. Ama bir race condition oluşursa (aynı anda iki yerden sat tetiklenirse): ikinci sat isteği reddedilir, "Aktif pet" toast gösterilir.

- **If envanter çok doluysa (100+ pet)**: Izgara scroll performansı: item virtualization zorunlu — yalnızca ekranda görünür kartlar render edilir. Kural 1'deki 2 sütun grid virtualized list olarak implemente edilir.

- **If filtre hiç sonuç döndürmezse**: Izgara alanında "Bu filtreyle eşleşen pet yok." mesajı + "Filtreyi Temizle" butonu.

- **If Detay Sayfası'nda "Aktif Yap" basıldıktan sonra geri dönülürse**: Aktif Pet Satırı ızgarada anında güncellenir. Önceki aktif pet kartından "AKTİF" etiketi kalkar, yenisine eklenir.

- **If C/B tier pet uzun basma menüsünden "Sat"a basılırsa**: Toast gösterilmez, küçük bir "🔒" animasyonu + "Detaydan kilidi kaldır" toast.

- **If aynı anda iki karta hızlı dokunulursa**: İlk dokunma kabul edilir, stack push başlar. İkinci dokunma debounce (300ms) ile reddedilir.

- **If otomatik satış filtresi açıkken yeni bir C tier pet kazanılırsa**: Pet koleksiyona eklenir (otomatik satış filtresi C tier'ı korur). Toast: "Yeni pet kazandın: [İsim] (C) — Koleksiyona eklendi."

- **If "SATIYOR" onay metnini giren oyuncu sonra vazgeçerse**: "İPTAL" veya modal dışına dokununca modal kapanır, pet satılmaz.

- **If ızgara scroll ortasında filtre değiştirilirse**: Scroll pozisyonu sıfırlanır (listenin en üstüne gidilir).

- **If pet evrim sırasında (ayrı sayfada) Detay Sayfası arka planda açıksa**: Evrim tamamlanınca Detay Sayfası'na dönünce stat'lar ve tier güncel verisini gösterir (cache'lenmez, her açılışta `GetEffectiveStats()` çağrılır).

## Dependencies

### Upstream

| Sistem | Veri | Arayüz |
|--------|------|--------|
| **Pet Sistemi** | Tüm petler, aktif pet | `GetAllPets()` → [PetData], `GetActivePet()` |
| **Canavar Veritabanı** | Pet adı, arketip, tier, portrait referansı | `GetMonsterIdentity(petId)` |
| **Canavar Güçlendirme / Pet Evrim** | Effective stats, seviye, EXP, yıldız, evrim malzeme durumu | `GetEffectiveStats(petId)`, `GetLevelInfo(petId)`, `GetEvolutionStatus(petId)` |
| **Ekonomi** | Satış sonrası altın güncelleme | `SellPet(petId)` → gold_gained |
| **Kaydetme / Yükleme** | Otomatik satış filtresi ayarı, aktif pet | `GetAutoSellFilter()`, `SetActivePet(petId)`, `SetAutoSellFilter(config)` |
| **UI Framework** | Stack navigasyon, modal, toast, HUD | `UIManager.*` |

### Downstream

| Sistem | Tetikleme |
|--------|-----------|
| **Pet Evrim Sistemi UI** | "Evrimleştir" butonu → evrim sayfasına push |
| **Keşif Alanı Harita UI** | `SetActivePet()` çağrısı → harita SG renk kodları güncellenir |
| **Ekonomi** | Satış işlemi → `SellPet(petId)` |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `grid_columns` | 2 | 2–3 | 3 sütun: küçük ekranda sıkışık | 1 sütun: çok az bilgi, çok scroll |
| `card_height_dp` | 180 | 160–200 | Scroll çok uzar | Portrait sıkışır |
| `card_gap_dp` | 8 | 4–12 | Kartlar birbirinden uzak | Nefes almıyor |
| `long_press_ms` | 500 | 300–700 | Kasıtlı uzun basma gerekiyor | Yanlışlıkla tetikler |
| `detail_push_ms` | 250 | 200–350 | Yavaş geçiş | Birdenbire beliriyor |
| `rare_sell_confirm_text` | "SATIYOR" | — | Farklı kelime: aynı güvenlik seviyesi | Sadece onay butonu: kazara satış |
| `auto_sell_default_F` | true | — | F'ler anında gider | Envanter hızla dolar |
| `auto_sell_default_D` | true | — | D'ler anında gider | Fazla pet birikir |

## Acceptance Criteria

1. **GIVEN** koleksiyon ekranı açıldığında, **WHEN** petler varsa, **THEN** varsayılan sıralama SG yüksekten düşüğe, 2 sütun ızgara gösterilir.

2. **GIVEN** B tier pet kartı render edilirse, **THEN** altın çerçeve, B tier rozeti ve yıldız sayısı gösterilir.

3. **GIVEN** aktif pet seçiliyse, **WHEN** kartı render edilirse, **THEN** kart hafif altın glow arka plana sahip ve "AKTİF" etiketi sol alt köşede görünür.

4. **GIVEN** "Tier: C, D" filtresi uygulandı, **WHEN** ızgara güncellenir, **THEN** yalnızca C ve D tier petler gösterilir. Üst barda "Filtre (1)" badge'i belirir.

5. **GIVEN** filtre uygulanmış, hiçbir pet eşleşmiyorsa, **WHEN** ızgara güncellenir, **THEN** "Bu filtreyle eşleşen pet yok." + "Filtreyi Temizle" butonu gösterilir.

6. **GIVEN** bir karta dokunulduğunda, **WHEN** 250ms geçiş sonrası, **THEN** Pet Detay Sayfası stack'e eklenir. Stat barları, yetenek kartı, seviye/EXP barı gösterilir.

7. **GIVEN** Detay Sayfası'nda "Aktif Yap" basıldı, **WHEN** işlem tamamlanırsa, **THEN** buton "AKTİF ✓" olarak güncellenir. Önceki aktif pet kartından "AKTİF" etiketi kalkar. Keşif Harita UI'ndaki SG renk kodları güncellenir.

8. **GIVEN** aktif pet Detay Sayfası'nda "Sat" butonuna basıldı, **WHEN** işleme çalışılırsa, **THEN** "SAT" butonu disabled + "Aktif pet olduğu için satılamaz." mesajı, işlem başlamaz.

9. **GIVEN** F tier pet satışı onay diyaloğu açıldı, **WHEN** "SAT" butonuna basılırsa, **THEN** pet koleksiyondan çıkar, altın eklenir, Detay Sayfası stack'ten çıkar (ızgaraya dönülür), toast: "100 Altın kazandın."

10. **GIVEN** C tier pet satışı denenirse, **WHEN** onay diyaloğu açılırsa, **THEN** "SATIYOR" metin girişi istenir. Yanlış metin girilirse "SAT" butonu disabled kalır.

11. **GIVEN** "SATIYOR" doğru girilirse (büyük/küçük harf fark etmez), **WHEN** "SAT" basılırsa, **THEN** C tier pet satılır, 400 Altın eklenir.

12. **GIVEN** C/B tier pet uzun basma menüsünden "Sat" seçilirse, **THEN** "Detaydan kilidi kaldır" toast gösterilir, satış işlemi başlamaz.

13. **GIVEN** otomatik satış filtre ayarı açılırsa, **WHEN** C tier toggle'a dokunulursa, **THEN** toggle disabled kalır (toggle'lanamaz). "C ve B tier otomatik satışa açılamaz" mesajı gösterilir.

14. **GIVEN** koleksiyonda hiç pet yoksa, **WHEN** Koleksiyon sekmesine girilirse, **THEN** boş durum gösterilir: açıklama metni + "Keşif Alanı'na Git →" butonu.

15. **GIVEN** ızgara scroll ortasındayken filtre değiştirilirse, **WHEN** filtre uygulanırsa, **THEN** scroll pozisyonu sıfırlanır (listenin en üstüne gidilir).

16. **GIVEN** "Yeni Eklenen" sıralaması seçilirse, **WHEN** ızgara güncellenir, **THEN** en son kazanılan pet ilk sırada (sol üst köşe) görünür.

17. **GIVEN** aynı anda iki karta hızlı dokunulursa (300ms içinde), **WHEN** ikinci dokunma alınırsa, **THEN** yalnızca ilk kart için Detay Sayfası açılır, ikinci debounce ile reddedilir.

18. **GIVEN** Detay Sayfası açıkken geri tuşuna basılırsa, **WHEN** pop animasyonu tamamlanırsa, **THEN** ızgara scroll pozisyonu (Detay Sayfası'nı açmadan önceki konuma) korunur.

19. **GIVEN** 100+ pet koleksiyonunda, **WHEN** ızgara scroll edilirse, **THEN** yalnızca ekranda görünür kartlar render edilir (item virtualization). Ekran frame drop yaşamamalı.

20. **GIVEN** Detay Sayfası'nda "Evrimleştir" butonuna basıldı (malzeme tam), **WHEN** basılırsa, **THEN** Pet Evrim UI sayfasına stack push yapılır.
