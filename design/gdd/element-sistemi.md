# Element Sistemi (Elemental System)

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Topla Hepsini, Güç Hisset

## Overview

**Element Sistemi**, oyundaki tüm hasar etkileşimlerinin, takım kurma stratejisinin ve sinerji mekaniklerinin temelini oluşturan döngüsel avantaj/dezavantaj çerçevesidir. Dört element — Ateş, Su, Toprak, Hava — arasında döngüsel bir güç dengesi tanımlar (Ateş → Toprak → Hava → Su → Ateş): avantajlı element 1.5x hasar verirken, dezavantajlı element 0.75x hasarda kalır. Bu matris salt bir çarpan tablosu olmanın ötesinde, oyuncunun her savaş öncesinde "hangi canavarları seçmeliyim?" sorusunu sormasını sağlayan stratejik karar katmanıdır.

Veri katmanı olarak Element Sistemi, Canavar Veritabanı'ndan her canavarın element aidiyetini alır ve Hasar Hesaplama, Takım Kurma, Düşman AI gibi downstream sistemlere element çarpanlarını sağlar. Oyuncu perspektifinden ise element avantajını doğru kullanmak "akıllıca savaştım" tatminini verir — yanlış element seçimi cezalandırıcı değil ama doğru seçim belirgin şekilde ödüllendiricidir. Takımını düşman elementlerine göre kuran oyuncu, %50'ye varan hasar artışıyla "strateji işe yaradı" hissini yaşar.

MVP kapsamında 4 element ve basit döngüsel matris yeterlidir. İleride element sinerjileri (aynı elementten 2+ canavar bonusu) ve çift-element canavarlar gibi derinlik katmanları eklenebilir.

## Player Fantasy

Oyuncu element sisteminde **taktik üstünlük** fantezisi yaşar. Zindan girişinden önce düşman dalgalarının element kompozisyonunu gördüğünde, takımını buna göre ayarlamak bir "satranç hamlesi" hissi verir. Ateş canavarlarla dolu bir kata Su ağırlıklı takım kurmak, ardından hasarların 1.5x çarpanla uçuşmasını izlemek — "ben bunu bildim, hazırlıklıydım" tatminidir.

**Çekirdek duygu**: Stratejik hazırlığın somut ödülü. Oyuncu rastgele savaşmıyor, bilinçli seçimler yapıyor ve bu seçimlerin karşılığını ekranda sayılar olarak görüyor. "Super Effective!" anı — Pokémon'un 30 yıldır kanıtladığı o kısa ama tatmin edici "doğru hamle" geri bildirimi.

**Negatif fantazi (kaçınılacak)**: Element dezavantajı asla "cezalandırılma" hissi vermemeli. 0.75x çarpan yeterince hafiftir ki oyuncu "yanlış seçim yaptım" yerine "doğru seçimle daha da iyi olabilirdim" hisseder. Bu, "Cömert Zindan" ve "Güç Hisset" sütunlarıyla doğrudan uyumludur — oyuncu her zaman güçlü hisseder, ama akıllıca oynayınca *daha da* güçlü hisseder.

**Keşif katmanı**: İlk başta oyuncu sadece "kırmızı yeşili döver" düzeyinde farkındalık kazanır. Zamanla element sinerjilerini (aynı elementten 2+ canavar bonusu) keşfeder ve takım kurma derinliği açılır. Bu kademeli keşif, "Topla Hepsini" sütununu besler — her yeni element kombinasyonu denemeye değer bir deneyimdir.

**Pillar bağlantısı**: "Güç Hisset" — element avantajı güçlü olma hissini %50 artırır. "Topla Hepsini" — her element, toplanmaya değer farklı canavarlar anlamına gelir. "Senin Tempon" — otofarm'da element sistemi basitleştirilmiş çalışır, aktif oyunda tam derinlik sunar.

## Detailed Design

### Core Rules

**Kural 1 — Element Tanımları**

Oyunda 4 element bulunur. Her element bir doğa gücünü temsil eder ve kendine özgü renk, ikon ve VFX paleti taşır:

| Element | Enum | Renk Paleti | Döngüde Avantajlı Olduğu |
|---------|------|-------------|--------------------------|
| **Ateş** | `fire` | Turuncu-kırmızı | Toprak |
| **Su** | `water` | Mavi-turkuaz | Ateş |
| **Toprak** | `earth` | Yeşil-kahve | Hava |
| **Hava** | `air` | Beyaz-mor | Su |

**Kural 2 — Döngüsel Avantaj Matrisi**

Ateş → Toprak → Hava → Su → Ateş (saat yönünde döngü).

| Saldıran \ Savunan | Ateş | Su | Toprak | Hava |
|---------------------|------|-----|--------|------|
| **Ateş** | 1.0x | 0.75x | 1.5x | 1.0x |
| **Su** | 1.5x | 1.0x | 1.0x | 0.75x |
| **Toprak** | 0.75x | 1.0x | 1.0x | 1.5x |
| **Hava** | 1.0x | 1.5x | 0.75x | 1.0x |

Her elementin tam olarak 1 avantajlı, 1 dezavantajlı ve 2 nötr ilişkisi vardır. Matris simetriktir: A→B avantajlıysa, B→A dezavantajlıdır.

**Kural 3 — Element Çarpanı Uygulama Noktası**

Element çarpanı, Hasar Hesaplama pipeline'ında temel hasar hesaplandıktan **sonra** çarpan olarak uygulanır:

```
final_damage = base_damage × element_multiplier × [diğer çarpanlar]
```

Element çarpanı diğer çarpanlarla (kritik, buff/debuff) çarpımsal ilişkidedir, toplamsal değil.

**Kural 4 — Element Sinerjisi (Takım Bonusu)**

Aynı elementten birden fazla canavar takımda bulunduğunda sinerji bonusu aktif olur:

| Aynı Element Sayısı | Sinerji Bonusu | Bonus Türü |
|----------------------|----------------|------------|
| 2 canavar | +10% ATK | Saldırı artışı |
| 3 canavar | +15% ATK, +10% DEF | Saldırı + savunma artışı |
| 4 canavar (tam takım) | +20% ATK, +15% DEF, +10% SPD | Tam stat artışı |

Sinerji kuralları:
- Sinerji bonusu sadece aynı elementteki canavarlara uygulanır (farklı element takım arkadaşlarına değil).
- Takımda birden fazla element grubu varsa, her grup kendi sinerji eşiğine göre değerlendirilir (ör: 2 Ateş + 2 Su → her ikisi de +10% ATK alır).
- Sinerji bonusu base stat'lara yüzdesel olarak uygulanır, element çarpanından bağımsızdır.
- Otofarm modunda sinerji bonusu aynı şekilde hesaplanır — basitleştirme yok.

**Kural 5 — Element Bilgi Sistemi**

Oyuncu savaş öncesinde ve sırasında element bilgisine erişebilir:

| Bilgi | Ne Zaman Görünür | Nerede Görünür |
|-------|-------------------|----------------|
| Düşman element ikonu | Savaş öncesi (kat önizlemesi) + savaş sırası | Düşman HP barının yanında |
| Avantaj/dezavantaj okları | Savaş öncesi takım seçimi | Takım kurma ekranında her canavar kartının köşesinde (↑ yeşil = avantaj, ↓ kırmızı = dezavantaj) |
| "Etkili!" / "Etkisiz..." yazısı | Hasar anında | Hasar sayısının üstünde |
| Sinerji göstergesi | Takım kurma | Takım panelinde aktif sinerji ikonu + bonus yüzdesi |

**Kural 6 — Tek Element Kısıtı (MVP)**

MVP'de her canavar tam olarak 1 elemente sahiptir. Çift-element canavarlar Tier 3+ kapsamında değerlendirilecektir. Element, canavarın doğuştan özelliğidir ve asla değişmez (evrim dahil).

### States and Transitions

Element Sistemi durum makinesi değildir — statik bir veri katmanıdır. Ancak sinerji bonusu takım değişikliklerinde aktifleşir/deaktifleşir:

| Durum | Tetikleyici | Sonuç |
|-------|-------------|-------|
| **Sinerji Pasif** | Takımda aynı elementten <2 canavar | Bonus yok |
| **Sinerji Aktif (Lv1)** | Takımda aynı elementten 2 canavar | +10% ATK |
| **Sinerji Aktif (Lv2)** | Takımda aynı elementten 3 canavar | +15% ATK, +10% DEF |
| **Sinerji Aktif (Lv3)** | Takımda aynı elementten 4 canavar | +20% ATK, +15% DEF, +10% SPD |

Geçişler anlık ve deterministiktir — takım değişikliği anında sinerji yeniden hesaplanır.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | Canavarın element aidiyeti | `GetMonsterElement(monsterId)` → enum |
| **Hasar Hesaplama** | → sağlar | Element çarpanı (0.75–1.50) | `GetElementMultiplier(attackerElement, defenderElement)` → float |
| **Takım Kurma** | → sağlar | Sinerji bonusu hesabı | `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus, spd_bonus} |
| **Düşman AI** | → sağlar | Düşman element bilgisi (AI element-bilinçli kararlar için) | `GetElementMultiplier()` (aynı arayüz) |
| **Savaş UI** | → sağlar | Element ikon/renk, avantaj/dezavantaj göstergesi, sinerji ikonu | UI widget'larına enum → ikon/renk mapping |
| **Zindan Keşif** | → sağlar | Kattaki düşmanların element kompozisyonu | `GetElementComposition(floorEnemies[])` → element dağılımı |
| **Loot / Ödül** | dolaylı | Element-bazlı loot filtresi (ileride) | MVP'de yok — Tier 2+ |

Veri akışı Canavar Veritabanı'ndan tek yönlü gelir. Element Sistemi runtime'da immutable matris verir.

## Formulas

### Formül 1: Element Hasar Çarpanı (Referans)

> Bu formül Canavar Veritabanı GDD'sinde tanımlanmıştır — burada referans verilir, yeniden tanımlanmaz.

`element_multiplier = ELEMENT_MATRIX[attacker_element][defender_element]`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Saldıran elementi | attacker_element | enum | {fire, water, earth, air} | Saldıranın elementi |
| Savunan elementi | defender_element | enum | {fire, water, earth, air} | Savunanın elementi |
| Çarpan | element_multiplier | float | 0.75–1.50 | Hasar çarpanı |

**Çıktı Aralığı**: 0.75 (dezavantaj), 1.0 (nötr/aynı), 1.5 (avantaj)

**Kaynak**: `design/gdd/canavar-veritabani.md` — Formül 3

### Formül 2: Sinerji Bonusu Hesaplama

`synergy_bonus = SYNERGY_TABLE[same_element_count]`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Aynı element sayısı | same_element_count | int | 0–4 | Takımdaki aynı elementten canavar sayısı |
| ATK bonusu | atk_bonus | float | 0.00–0.20 | Saldırı yüzde artışı |
| DEF bonusu | def_bonus | float | 0.00–0.15 | Savunma yüzde artışı |
| SPD bonusu | spd_bonus | float | 0.00–0.10 | Hız yüzde artışı |

Lookup tablosu:

| same_element_count | atk_bonus | def_bonus | spd_bonus |
|--------------------|-----------|-----------|-----------|
| 0–1 | 0.00 | 0.00 | 0.00 |
| 2 | 0.10 | 0.00 | 0.00 |
| 3 | 0.15 | 0.10 | 0.00 |
| 4 | 0.20 | 0.15 | 0.10 |

**Uygulama**:
`effective_stat = floor(base_stat × (1 + synergy_bonus_for_stat))`

**Çıktı Aralığı**: base_stat (bonus yok) – floor(base_stat × 1.20) (maksimum ATK bonusu)

**Örnek**: Nadir Saldırgan (ATK=52), takımda 3 Ateş canavar → effective_ATK = floor(52 × 1.15) = 59

### Formül 3: Sinerji + Element Çarpanı Birlikte

Sinerji ve element çarpanı birbirinden bağımsız, çarpımsal olarak uygulanır:

```
final_damage = base_damage × element_multiplier × (1 + synergy_atk_bonus) × [diğer çarpanlar]
```

**Worst case**: dezavantajlı + sinerji yok → base_damage × 0.75
**Best case**: avantajlı + 4'lü sinerji → base_damage × 1.5 × 1.20 = base_damage × 1.80

**Örnek**: ATK=52, avantajlı element, 3'lü sinerji → 52 × 1.5 × 1.15 = 89.7 → floor(89) efektif hasar çarpanı girişi

## Edge Cases

- **If canavarın element değeri geçersizse (4 enum dışında)**: Tüm element çarpanları 1.0x olarak uygulanır (nötr). Hata loglanır. Sinerji grubuna dahil edilmez. *(Canavar Veritabanı GDD'sindeki kural ile tutarlı.)*

- **If takımda 0 canavar varsa (boş takım)**: Sinerji hesaplanmaz, bonus = 0. Savaş başlatılamaz (Takım Kurma'nın sorumluluğu).

- **If saldıran ve savunan aynı elemente sahipse**: Çarpan = 1.0x. Ne avantaj ne dezavantaj. UI'da nötr gösterge (gri ikon).

- **If takımda 2 Ateş + 2 Su canavar varsa**: Her grup bağımsız hesaplanır. 2 Ateş canavar → Ateş grubu +10% ATK (sadece Ateş canavarlara). 2 Su canavar → Su grubu +10% ATK (sadece Su canavarlara). Çapraz bonus yok.

- **If savaş sırasında bir canavar savaş dışı kalırsa (HP=0)**: Sinerji bonusu **yeniden hesaplanmaz** — savaş başındaki takım kompozisyonu savaş sonuna kadar geçerlidir. Aksi halde oyuncu savaş ortasında beklenmedik stat düşüşü yaşar (Güç Hisset sütununa aykırı).

- **If düşman takımında element sinerjisi varsa**: Düşmanlar da aynı sinerji kurallarından yararlanır. AI takım kompozisyonu Zindan Keşif/Düşman AI tarafından belirlenir — oyuncu adaletsizlik hissetmemeli, düşman sinerjileri oyuncu sinerjilerine kıyasla daha seyrek olmalıdır (Tuning Knobs'ta ayarlanır).

- **If ileride çift-element canavar eklenirse (Tier 3+)**: Çift elementli canavar her iki element grubunda da sayılır (ör: Ateş/Su canavar → Ateş sinerji grubuna +1, Su sinerji grubuna +1). Element matrisi sorgusu: saldırıda birincil element kullanılır, savunmada savunanın zayıf elementine göre daha yüksek çarpan uygulanır. *(MVP'de geçerli değil — ilerideki GDD revizyonuyla detaylanır.)*

- **If takım büyüklüğü 4'ten farklıysa (ileride değişirse)**: Sinerji tablosu takım büyüklüğüne göre ölçeklenir. MVP'de sabit 4 kişilik takım varsayılır.

- **If element çarpanı diğer çarpanlarla birlikte 0 veya negatif final hasar üretirse**: Final hasar minimum 1 olarak clamp edilir. Hasar Hesaplama GDD'sinin sorumluluğu — Element Sistemi sadece çarpan sağlar.

## Dependencies

### Upstream (Bu sistem neye bağlı)

Yok — Foundation katmanı, sıfır bağımlılık. Element matrisi ve sinerji tablosu kendi içinde tanımlı statik veri yapılarıdır.

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `GetElementMultiplier(attackerElement, defenderElement)` → float | Olmadan element farkı yok — tüm hasarlar nötr |
| **Takım Kurma** | Sert | `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus, spd_bonus} | Olmadan sinerji bonusu hesaplanamaz |
| **Düşman AI** | Yumuşak | `GetElementMultiplier()` (aynı arayüz) | AI element-bilinçli karar verebilir; olmadan rastgele seçer |
| **Savaş UI** | Yumuşak | Element enum → ikon/renk mapping, avantaj/dezavantaj göstergesi | Olmadan element bilgisi gösterilemez |
| **Zindan Keşif** | Yumuşak | `GetElementComposition(floorEnemies[])` → element dağılımı | Olmadan kat önizlemesinde element bilgisi yok |
| **Canavar Veritabanı** | Referans | Element enum tanımı paylaşılır | Enum uyumluluğu gerekli |

**Bağımlılık doğası**: Element Sistemi aşağıya veri **sağlar**, yukarıdan veri **almaz**. Canavar Veritabanı ile ilişkisi referanstır — her ikisi de aynı element enum'unu kullanır, ancak biri diğerine bağımlı değildir. Element matrisi bu GDD'de tanımlanır; canavarların element aidiyeti Canavar Veritabanı GDD'sinde tanımlanır.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `element_advantage_multiplier` | 1.50 | 1.25–2.00 | Element seçimi çok kritik → yanlış takımla savaş imkansız (Güç Hisset ihlali) | Element farkı hissedilmez → sistem anlamsız |
| `element_disadvantage_multiplier` | 0.75 | 0.50–0.90 | Dezavantaj çok ağır → cezalandırıcı hisseder (Cömert Zindan ihlali) | Dezavantaj hissedilmez → element stratejisi önemsiz |
| `synergy_2_atk_bonus` | 0.10 | 0.05–0.20 | Sinerji çok güçlü → çeşitli takım kurmak cezalı | Sinerji fark edilmez |
| `synergy_3_atk_bonus` | 0.15 | 0.10–0.25 | — | — |
| `synergy_3_def_bonus` | 0.10 | 0.05–0.15 | — | — |
| `synergy_4_atk_bonus` | 0.20 | 0.15–0.35 | 4'lü mono-element takım dominant meta → çeşitlilik ölür | Tam sinerji ödülsüz |
| `synergy_4_def_bonus` | 0.15 | 0.10–0.25 | — | — |
| `synergy_4_spd_bonus` | 0.10 | 0.05–0.20 | — | — |
| `enemy_synergy_frequency` | 0.15 | 0.05–0.30 | Düşmanlar çok sık sinerji kullanır → adaletsiz hisseder | Düşman sinerjisi hiç yok → oyuncu sinerjisi aşırı güçlü |

**Etkileşim Uyarıları**:
- `element_advantage_multiplier` × `synergy_4_atk_bonus` birlikte best-case çarpanı belirler (1.50 × 1.20 = 1.80). İkisini aynı anda artırmak hasar eğrisini kırabilir.
- `element_disadvantage_multiplier` × düşman sinerji sıklığı birlikte "adaletsiz savaş" hissini kontrol eder. Dezavantajı ağırlaştırırken düşman sinerjisini azalt.
- Canavar Veritabanı'ndaki `element_advantage_multiplier` ve `element_disadvantage_multiplier` tuning knob'ları ile **aynı değerlerdir** — tek kaynak bu GDD'dir, Canavar Veritabanı referans alır.

## Visual/Audio Requirements

### Element Renk Paleti (Art Bible Section 1 — kilitli)

| Element | Birincil Renk | İkincil Renk | VFX Tonu |
|---------|---------------|--------------|----------|
| Ateş | Turuncu-kırmızı (#FF6B35) | Sarı (#FFD700) | Alev parçacıkları, sıcak parıltı |
| Su | Mavi-turkuaz (#00B4D8) | Koyu mavi (#0077B6) | Damla/dalga parçacıkları, soğuk parıltı |
| Toprak | Yeşil-kahve (#6B8E23) | Koyu kahve (#8B4513) | Toz/yaprak parçacıkları, ağır titreşim |
| Hava | Beyaz-mor (#E0BBE4) | Açık mor (#9B59B6) | Rüzgar çizgileri, hafif parıltı |

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Avantajlı hasar (1.5x) | "Etkili!" yazısı (büyük, parlak, element renginde) + ekstra parçacık patlaması | 0.8s | MVP |
| Dezavantajlı hasar (0.75x) | "Etkisiz..." yazısı (küçük, soluk, gri) | 0.5s | MVP |
| Nötr hasar (1.0x) | Standart hasar sayısı (ek VFX yok) | — | MVP |
| Sinerji aktifleşme | Takım panelinde element renginde hafif parlama + sinerji ikonu belirme | 0.5s | MVP |
| Sinerji seviye artışı (2→3→4) | Parlama yoğunluğu artar, ikon büyür | 0.3s | Nice-to-have |

### İkon Gereksinimleri

| İkon | Boyut | Kullanım Yeri |
|------|-------|---------------|
| Element ikonu (×4) | 32×32 px (UI), 64×64 px (detay) | Canavar kartı, HP barı yanı, takım paneli |
| Avantaj oku (↑ yeşil) | 24×24 px | Takım kurma ekranı — canavar kartı köşesi |
| Dezavantaj oku (↓ kırmızı) | 24×24 px | Takım kurma ekranı — canavar kartı köşesi |
| Nötr gösterge (— gri) | 24×24 px | Takım kurma ekranı (opsiyonel — gizlenebilir) |
| Sinerji ikonu | 40×40 px | Takım panelinde — aktif sinerji göstergesi |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Avantajlı hasar | Yükselen, parlak "ding" + element efekti (ateş çatırtısı, su sıçraması vb.) | Pozitif, tatmin edici | MVP |
| Dezavantajlı hasar | Kısa, boğuk "thud" | Nötr — cezalandırıcı değil | MVP |
| Sinerji aktifleşme | Kısa melodic chime | Ödüllendirici | Nice-to-have |

## UI Requirements

### Takım Kurma Ekranı
- Her canavar kartının sol üst köşesinde 32×32 element ikonu (renk kodlu)
- Düşman bilgisi görünürken, oyuncunun her canavarının sağ alt köşesinde avantaj/dezavantaj oku (↑↓ veya —)
- Takım panelinin altında aktif sinerji göstergesi: element ikonu + "2/3/4 [Element] Sinerjisi: +X% ATK, +Y% DEF" yazısı
- Sinerji bonusu stat değişikliklerini yeşil renkte göster (base stat → buffed stat)

### Savaş Ekranı
- Düşman HP barının yanında 24×24 element ikonu
- Hasar sayısı üstünde element etkileşim yazısı ("Etkili!" / "Etkisiz...")
- Avantajlı hasar sayıları element renginde ve %30 daha büyük font
- Dezavantajlı hasar sayıları gri ve %20 daha küçük font

### Zindan Kat Önizlemesi
- Kattaki düşmanların element dağılımı özeti: "🔥×3 💧×1 🌍×2" formatında
- Oyuncunun mevcut takımına göre genel avantaj/dezavantaj özeti göstergesi (opsiyonel)

### Canavar Detay Ekranı
- Element alanında 64×64 element ikonu + element adı
- Element avantaj/dezavantaj referans tablosu (ilk gösterimlerde tooltip ile)

**Minimum Dokunma Hedefi**: Tüm element ikonları ve avantaj okları minimum 44×44 dp dokunma alanına sahip olmalı (mobil erişilebilirlik).

> 📌 **UX Flag — Element Sistemi**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te (Pre-Production) `/ux-design` çalıştırarak takım kurma ve savaş ekranları için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Ateş saldırgan ve Toprak savunan, **WHEN** element çarpanı sorgulanırsa, **THEN** 1.5x döner.

2. **GIVEN** Su saldırgan ve Hava savunan, **WHEN** element çarpanı sorgulanırsa, **THEN** 0.75x döner.

3. **GIVEN** Ateş saldırgan ve Ateş savunan, **WHEN** element çarpanı sorgulanırsa, **THEN** 1.0x döner.

4. **GIVEN** Ateş saldırgan ve Su savunan, **WHEN** element çarpanı sorgulanırsa, **THEN** 0.75x döner (Ateş, Su'ya dezavantajlı).

5. **GIVEN** takımda 2 Ateş canavar (ATK=52 ve ATK=35), **WHEN** sinerji hesaplanırsa, **THEN** her ikisinin ATK'sı %10 artar → 57 ve 38.

6. **GIVEN** takımda 3 Su canavar, **WHEN** sinerji hesaplanırsa, **THEN** Su canavarlar +15% ATK ve +10% DEF alır.

7. **GIVEN** takımda 4 Toprak canavar, **WHEN** sinerji hesaplanırsa, **THEN** tüm Toprak canavarlar +20% ATK, +15% DEF, +10% SPD alır.

8. **GIVEN** takımda 2 Ateş + 2 Su canavar, **WHEN** sinerji hesaplanırsa, **THEN** Ateş canavarlar +10% ATK alır, Su canavarlar +10% ATK alır, çapraz bonus yok.

9. **GIVEN** takımda 1 Ateş + 1 Su + 1 Toprak + 1 Hava canavar, **WHEN** sinerji hesaplanırsa, **THEN** hiçbir sinerji bonusu aktif olmaz (her elementten <2).

10. **GIVEN** savaş başlangıcında 3 Ateş canavar sinerji aktif, **WHEN** bir Ateş canavar HP=0'a düşerse, **THEN** sinerji bonusu savaş sonuna kadar 3'lü seviyede kalır (yeniden hesaplanmaz).

11. **GIVEN** avantajlı element (1.5x) + 4'lü sinerji (+20% ATK) + base_damage=52, **WHEN** final hasar hesaplanırsa, **THEN** element katkısı = 52 × 1.5 × 1.20 = 93.6 → floor(93).

12. **GIVEN** geçersiz element enum değeri, **WHEN** element çarpanı sorgulanırsa, **THEN** 1.0x döner ve hata loglanır.

13. **GIVEN** düşman Ateş elementli, **WHEN** kat önizlemesi gösterilirse, **THEN** düşmanın yanında Ateş ikonu (turuncu-kırmızı) görünür.

14. **GIVEN** oyuncu Su canavarı Ateş düşmana karşı seçtiğinde, **WHEN** takım kurma ekranında, **THEN** Su canavarın kartında yeşil ↑ avantaj oku görünür.

15. **GIVEN** avantajlı element ile hasar verildiğinde, **WHEN** hasar animasyonu oynarsa, **THEN** hasar sayısının üstünde "Etkili!" yazısı görünür.

## Open Questions

1. **Sinerji görsel geri bildirimi ne kadar abartılı olmalı?** 4'lü tam sinerji'de özel bir aura veya takım efekti olmalı mı? → Art direction kararı, art bible ile uyumlu olmalı.

2. **Element döngüsü öğretimi**: Yeni oyuncuya döngüyü nasıl öğreteceğiz? Tutorial sırasında mı, yoksa keşfe mi bırakacağız? → Tutorial/Onboarding GDD'sinde (Tier 2) tanımlanacak.

3. **Tier 3+ çift-element mekanik detayları**: Çift elementli canavarın hasar matrisinde nasıl davranacağı (birincil/ikincil element, veya her ikisi birden sorgulama) → Tier 3 tasarım aşamasında detaylanacak.

4. **Element-bazlı zindan bölgeleri**: İleride zindan bölgeleri element temalı mı olacak (Ateş zindanı, Su zindanı)? Bu, element sisteminin takım kurma stratejisini daha da kritik yapar → Zindan Keşif GDD'sinde tanımlanacak.
