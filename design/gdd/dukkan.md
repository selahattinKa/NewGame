# Dükkan Sistemi

> **Status**: Designed
> **Author**: user + ux-designer, economy-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Fantezisi, Cömert Zindan

## Overview

**Dükkan**, oyuncunun biriktirdiği altını harcayarak ekipman ve iksir satın aldığı NPC bazlı mağaza ekranıdır. Mağaza UI'ından (ad/elmas bölümü) bağımsız, ayrı bir ekrandır. Tab Bar'dan veya Keşif Alanı harita ekranındaki kısayol butonu aracılığıyla erişilir.

Stok iki bölümden oluşur: günlük dönen **Ekipman Rafı** (4 rastgele ekipman item, her gece yarısı yenilenir) ve her zaman sabit olan **İksir Tezgâhı** (Küçük / Büyük İksir sınırsız stok). Tüm işlemler altın (Altın) karşılığıdır — elmas veya reklam gerektirmez.

## Player Fantasy

Savaştan dönen oyuncu dükkanı açıp rafta ne olduğunu görmek ister. "Bugün iyi bir şey var mı?" döngüsü. Bazen rafta mükemmel bir item yoktur ama iksiri doldurmak her zaman anlamlıdır.

Altını biriktirip tam ihtiyaç duyduğu anda C tier Zırh görmek ve satın almak — kısa dönem tatmin. Uzun dönem: dükkan sayesinde altının harcama yeri var, kazanmak anlam taşıyor.

**Negatif fantezi (kaçınılacak)**: Dükkanın çok pahalı olması ve kimsenin alamaması. Ekipman fiyatları, oyuncunun birkaç Keşif Alanı aşaması temizleyerek ulaşabileceği aralıkta olmalı.

## Detailed Rules

### Core Rules

**Kural 1 — Ekran Layout'u**

```
┌─────────────────────────────────┐
│  HUD (altın / elmas / enerji)   │
├─────────────────────────────────┤
│  Üst Bar: "Dükkan"  [🔄 XX:XX] │  ← Kalan yenileme süresi
├─────────────────────────────────┤
│  ── EKİPMAN RAFI ──────────     │
│  Stok: 4 kart (2×2 grid)        │
│  [Kart 1] [Kart 2]              │
│  [Kart 3] [Kart 4]              │
│  "Yenileniyor: 07:22:15"        │
├─────────────────────────────────┤
│  ── İKSİR TEZGÂHI ─────────     │
│  [Küçük İksir] [Büyük İksir]   │
│   50 Altın/ad    150 Altın/ad   │
└─────────────────────────────────┘
  Tab Bar
```

**Kural 2 — Ekipman Rafı (Günlük Dönen Stok)**

- Her gece yarısı (yerel saat) 4 yeni rastgele ekipman item üretilir.
- Her item şu özellikleri gösterir: Slot tipi (ikonu), Tier (çerçeve rengi), İsim, Stat önizlemesi, Altın fiyatı.
- Satın alınan item raftan kalkar — kalan günde o yer boş kalır (yeniden dolmaz).
- Yenileme oyuncu tarafından tetiklenemez (manüel refresh yok).

**Raf Üretim Tablosu (4 item için tier dağılımı):**

| Slot Sayısı | Tier Havuzu | Ağırlık |
|-------------|------------|---------|
| 2 item | F veya D | F:%60, D:%40 |
| 1 item | D veya C | D:%65, C:%35 |
| 1 item | C veya B | C:%80, B:%20 |

Her item için slot (Silah/Zırh/Aksesuar — Pet veya Oyuncu) rasgele seçilir. Aynı slot 2 kez üst üste gelebilir.

**Kural 3 — Ekipman Fiyat Tablosu**

| Tier | Fiyat (Altın) |
|------|--------------|
| F | 200 |
| D | 600 |
| C | 1.800 |
| B | 5.000 |

- Fiyat sabittir — pazarlık, indirim veya zaman bazlı değişim yoktur (prototip).
- Altın yetersizse buton gri: "Yetersiz Altın (X/Y)".

**Kural 4 — İksir Tezgâhı (Sabit Stok)**

| İksir | Fiyat | Stok |
|-------|-------|------|
| Küçük İksir | 50 Altın / adet | Sınırsız |
| Büyük İksir | 150 Altın / adet | Sınırsız |

- Günlük limit yok — altın oldukça istediği kadar alınabilir.
- İksir taşıma limiti Ekipman Sistemi Kural 4'te tanımlıdır (Küçük: 10, Büyük: 5).
- Taşıma limiti dolduğunda "İksir Envanteri Dolu" uyarısı.
- Adet seçimi: "–" ve "+" butonları veya alınabilecek max adet butonu. Toplam maliyet ve adet anlık gösterilir.

**Kural 5 — Satın Alma Akışı**

1. Karta dokunulur → kartın detayı (büyütülmüş stat preview) gösterilir.
2. "SATIN AL (X Altın)" butonuna basılır.
3. Onayla → Altın düşülür, item envantere eklenir, toast: "[İsim] satın alındı!"
4. Ekipman ise: Raf'ta kart "Satıldı ✓" soluk görünümüne döner.
5. İksir ise: tezgah sayacı güncellenmez (sınırsız stok), iksir adedi envanterde artar.

**Kural 6 — Ekipman Satışı (Envantere Geri Satış)**

Dükkan'da oyuncu mevcut envanterindeki ekipmanı da satabilir:

- Ekran altında "Envantere Git" linki veya tab — oyuncu kendi envanterini listeler.
- Satış fiyatı: satın alma fiyatının %25'i (altın geri kazanımı düşük ama sıfır değil).

| Tier | Satış Fiyatı |
|------|-------------|
| F | 50 Altın |
| D | 150 Altın |
| C | 450 Altın |
| B | 1.250 Altın |

- Satış onay dialog: "F tier [İsim] satıyorsun — 50 Altın kazanacaksın. Onaylıyor musun?"
- Takılı ekipman satışta gösterilmez (çıkarılmadan satılamaz).

**Kural 7 — Stok Yenileme Göstergesi**

- Ekipman Rafı üstünde geri sayım: "Yenileniyor: SS:DD:SS".
- Gece yarısı → tüm raf anında yenilenir, sayaç 24:00:00'a sıfırlanır.
- Oyuncu Dükkan ekranındayken yenileme olursa: raf anlık güncellenir, "Stok yenilendi!" toast.

**Kural 8 — Ekrana Erişim Yolları**

- Tab Bar'da "Dükkan" ikonu (5. sekme veya mevcut Tab Bar'a eklenir).
- Keşif Alanı harita ekranındaki "⚔ Dükkan" kısayol butonu (sol üst köşe).
- HUD'dan kısayol: altın ikonuna uzun basma → "Dükkan'a Git" seçeneği.

### States and Transitions

```
[Dükkan Ekranı]
    ├─ Ekipman kartına dokun → [Detay Preview Overlay]
    │   ├─ "SATIN AL" → yeterli altın → item envantere eklenir, raf güncellenir
    │   ├─ "SATIN AL" → yetersiz altın → "Yetersiz Altın" uyarısı
    │   └─ "İptal" / dışarı dokun → overlay kapanır
    ├─ İksir adet seç → "SATIN AL" → envanter güncellenir
    ├─ "Envanterden Sat" → [Satış Listesi] → onay dialog → altın eklenir
    └─ Gece yarısı → raf yenilenir
```

| Durum | Görsel |
|-------|--------|
| Ekipman rafta mevcut | Parlak kart, fiyat görünür |
| Ekipman satın alındı | Kart soluk, "Satıldı ✓" etiketi |
| Altın yetersiz | "SATIN AL" butonu gri, "(X/Y)" fiyat kırmızı |
| İksir envanter dolu | "İksir Envanteri Dolu" uyarısı |
| Stok yenilenme anı | "Stok yenilendi!" toast + raf canlanır |

## Formulas

### Formül 1: Raf Üretimi

```
// Her gece yarısı:
raf = []
raf += generateItem(pool=["F","D"], weights=[0.60,0.40])
raf += generateItem(pool=["F","D"], weights=[0.60,0.40])
raf += generateItem(pool=["D","C"], weights=[0.65,0.35])
raf += generateItem(pool=["C","B"], weights=[0.80,0.20])

// Her item için slot seçimi:
slot = random.choice(["PET_SILAH","PET_ZIRH","PET_AKSESUAR","OYUNCU_SILAH","OYUNCU_AKSESUAR"])
item = GetEquipmentByTierAndSlot(tier, slot)
```

### Formül 2: Satış Geri Kazanımı

```
sell_price = floor(buy_price × 0.25)
```

**Örnek**: C tier ekipman → `floor(1800 × 0.25) = 450 Altın`

### Formül 3: İksir Çoklu Alım Maliyet Hesabı

```
quantity = oyuncu seçimi (1..max_alınabilir)
max_alınabilir = stack_limit - current_stack
total_cost = unit_price × quantity
```

**Örnek**: Küçük İksir 50 Altın, mevcut 3/10, max alınabilir 7:
Oyuncu 5 adet seçti → `total_cost = 50 × 5 = 250 Altın`

### Formül 4: Stok Yenileme Geri Sayımı

```
reset_time = midnight_local_time
seconds_remaining = reset_time - now()
display = format_HH:MM:SS(seconds_remaining)
```

## Edge Cases

- **If raf yenilenirken oyuncu satın alma işlemi başlattıysa**: Satın alma onaylandığında raf stok kontrolü yapılır. Item artık mevcut değilse "Bu ürün stokta kalmadı" toast, işlem iptal. (Prototipte bu durum gerçekleşmez çünkü raf yalnızca gece yarısı yenilenir — aynı anda başka oyuncu yoktur. Güvenli not olarak bırakıldı.)

- **If B tier ekipman rafta çıkar ve oyuncunun altını yetersizse**: Kart görünür, fiyat kırmızı, buton gri. Oyuncu geri gelip altın biriktirince satabileceğini umabilir — ama raf gece yarısı yenilenir. Ekranda "Yenileniyor: XX:XX:XX" sayacı bunu gösterir.

- **If tüm 4 raf item'i satın alınırsa**: Raf tamamen boş ("Bugünkü stok tükendi") görünümü. Geri sayım devam eder.

- **If iksir alımı sırasında altın tam olarak sıfıra iner**: İşlem tamamlanır. HUD altın = 0 gösterir. Bir sonraki alımda "Yetersiz Altın" engel.

- **If iksir alımı sırasında taşıma limiti aşılmak üzere**: Kalan alınabilecek adet hesaplanır (Formül 3). Maksimum adet otomatik olarak limit ile kısıtlanır — limit ötesi alım mümkün değildir.

- **If raf yenilenme anı Dükkan ekranı açıkken gelirse**: Raf anlık yenilenir (API call), "Stok yenilendi!" toast gösterilir. Yeni 4 item beliriyor.

- **If envanter dolu iken Dükkan'dan ekipman satın alınmak istenirse**: Satın alma butonu disabled: "Ekipman Envanteri Dolu (50/50)". Ekipman alınamaz. Envanterden önce satış yapılmalı.

- **If takılı ekipman satılmak istenirse**: Satış listesinde takılı ekipmanlar gösterilmez. Önce Pet Detay ekranında çıkarılması gerekir.

- **If oyuncu dükkanı açar ve hiç altını yoksa (0)**: Tüm satın alma butonları gri. İksir tezgâhı da erişilemez. "Altın kazanmak için Keşif Alanı'nı keşfet!" ipucu banner gösterilir.

## Dependencies

### Upstream

| Sistem | Veri / Arayüz |
|--------|--------------|
| **Ekonomi** | Altın bakiyesi okuma, harcama ve kazanım | `GetGold()`, `SpendGold(amount)`, `GrantGold(amount)` |
| **Ekipman Sistemi** | Ekipman tanımları, envantere ekleme, satış işlemi | `GetEquipmentByTierAndSlot(tier, slot)`, `AddEquipmentToInventory(item)`, `RemoveEquipment(itemId)` |
| **Kaydetme / Yükleme** | Günlük raf içeriği persist edilir (gece yarısı yenilenmesi için) |
| **UI Framework** | Ekran stack, toast, modal, HUD | `UIManager.*` |

### Downstream

| Sistem | Etki |
|--------|------|
| **Ekonomi** | Satın alma → altın düşer; satış → altın eklenir |
| **Ekipman Sistemi** | Alınan item envantere eklenir; satılan item envanterden çıkar |
| **Koleksiyon / Envanter UI** | Yeni ekipman eklendikten sonra liste güncellenir |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `equip_price_F` | 200 | 100–400 | F tier değersiz hisseder | Herkes F taşır |
| `equip_price_D` | 600 | 400–1.000 | D tier geç erişilir | D tier herkesin elinde |
| `equip_price_C` | 1.800 | 1.200–3.000 | C tier hedefe ulaşılmaz | C tier çok erken |
| `equip_price_B` | 5.000 | 3.500–8.000 | B tier teoride var ama alınamaz | B tier değersizleşir |
| `potion_small_price` | 50 | 30–100 | İksir stoklamak yetersiz | İksir çok ucuz, strateji azalır |
| `potion_large_price` | 150 | 100–300 | Büyük iksir tercih edilmez | Her savaşa büyük iksir spam |
| `sell_ratio` | 0.25 | 0.15–0.40 | Satış çok cazip, alsat döngüsü | Satış teşviki yok |
| `raf_B_tier_rate` | 0.20 | 0.10–0.30 | B tier çok sık görünür | B tier mağazada yok |
| `raf_C_tier_rate` | 0.35 | 0.25–0.50 | Raf zengin, altın biriktirme anlamsız | Raf çoğunlukla F/D |
| `raf_reset_hour` | 00:00 yerel | değiştirme önerilmez | — | — |

## Acceptance Criteria

1. **GIVEN** Dükkan açıldı, **WHEN** ekipman rafı render edilirse, **THEN** 4 item kart görünür; her kart tier çerçeve rengi (F gri, D yeşil, C mavi, B altın) ile gösterilir.

2. **GIVEN** raf üretimi, **WHEN** çalıştırılırsa, **THEN** istatistiksel olarak ~%30 F, ~%48 D, ~%17 C, ~%5 B tier item beklenir (uzun vadeli ortalama).

3. **GIVEN** oyuncunun 1.800 Altın'ı var ve rafta C tier ekipman var, **WHEN** "SATIN AL" basılırsa, **THEN** 1.800 Altın düşer, item envantere eklenir, raf'ta kart "Satıldı ✓" olur, toast gösterilir.

4. **GIVEN** oyuncunun 500 Altın'ı var ve C tier ekipman fiyatı 1.800, **WHEN** karta dokunulursa, **THEN** "SATIN AL" butonu gri, "(500/1800)" kırmızı fiyat gösterilir.

5. **GIVEN** 4 item'in tamamı satın alındı, **WHEN** raf render edilirse, **THEN** "Bugünkü stok tükendi" mesajı ve geri sayım görünür.

6. **GIVEN** İksir Tezgâhı'nda Küçük İksir 50 Altın, oyuncu 3 adet seçti (150 Altın), **WHEN** "SATIN AL" basılırsa, **THEN** 150 Altın düşer, iksir adedi 3 artar.

7. **GIVEN** Küçük İksir stack 8/10 iken 5 adet alınmak istenirse, **WHEN** adet seçicisi açılırsa, **THEN** maksimum 2 adet seçilebilir (8+2=10 limit).

8. **GIVEN** Küçük İksir stack 10/10 tam doluyken, **WHEN** satın alma butonuna basılırsa, **THEN** "İksir Envanteri Dolu (10/10)" uyarısı, işlem iptal.

9. **GIVEN** gece yarısı geldi, **WHEN** raf yenilendi, **THEN** 4 yeni item oluştu, geri sayım 24:00:00'a sıfırlandı. Oyuncu Dükkan ekranındaysa "Stok yenilendi!" toast görünür.

10. **GIVEN** C tier ekipman envanterde var ve satılmak isteniyor, **WHEN** satış onaylanırsa, **THEN** ekipman envanterden çıkar, `floor(1800 × 0.25) = 450 Altın` eklenir.

11. **GIVEN** takılı ekipman satılmak istenirse, **WHEN** "Envanterden Sat" listesi açılırsa, **THEN** takılı ekipmanlar listede görünmez.

12. **GIVEN** ekipman envanteri 50/50 doluyken Dükkan'da ekipman satın alınmak istenirse, **WHEN** "SATIN AL" butonuna basılırsa, **THEN** "Ekipman Envanteri Dolu (50/50)" mesajı, işlem iptal.

13. **GIVEN** oyuncunun 0 Altın'ı var, **WHEN** Dükkan açılırsa, **THEN** tüm butonlar gri + "Altın kazanmak için Keşif Alanı'nı keşfet!" ipucu banner görünür.

14. **GIVEN** Raf Üretim Formülü çalışırken, **WHEN** 1. ve 2. slot üretilirse, **THEN** tier pool [F:%60, D:%40] kullanılır; 3. slot [D:%65, C:%35]; 4. slot [C:%80, B:%20].

15. **GIVEN** Dükkan Tab Bar'dan erişilir, **WHEN** Tab'e basılırsa, **THEN** HUD görünür (altın/elmas/enerji) ve Dükkan ekranı tam ekran açılır.

16. **GIVEN** raf yenilenme süresi gösterilir, **WHEN** ekran açıkken 1 dakika geçerse, **THEN** geri sayım 1 dakika azalır (anlık güncellenir).

17. **GIVEN** Büyük İksir 150 Altın, oyuncu 1 adet alır, altın tam olarak sıfıra iner, **WHEN** işlem tamamlanırsa, **THEN** HUD altın = 0 gösterir. Bir sonraki alımda "Yetersiz Altın" engel.

18. **GIVEN** Keşif Alanı harita ekranındaki "Dükkan" kısayol butonu, **WHEN** basılırsa, **THEN** Dükkan ekranı overlay olarak veya yeni stack push olarak açılır.
