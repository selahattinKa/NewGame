# Ekonomi / Kaynak Yönetimi (Economy / Resource Management)

> **Status**: Designed
> **Author**: user + economy-designer, systems-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Cömert Zindan, Güç Hisset, Senin Tempon

## Overview

**Ekonomi / Kaynak Yönetimi**, oyundaki tüm kazanım ve harcama akışlarının merkezi denge noktasıdır. Üç temel kaynak — **Altın** (genel para birimi), **Enerji** (zindan girişi yakıtı) ve **Elmas** (nadir/premium kaynak) — etrafında dönen bir faucet-sink (musluk-lavabo) ekonomisi tanımlar. Her kaynak için nereden kazanılır, nereye harcanır ve ne hızda akar soruları bu sistemde cevaplanır.

Altyapı olarak Ekonomi sistemi, Canavar Güçlendirme'ye maliyet tabloları, Loot/Ödül'e ödül miktarları, Otofarm'a idle kazanım oranları sağlar — downstream sistemler kaynak üretmez veya harcamaz, bu sisteme sorarak yapar. Oyuncu perspektifinden ise her zindan çıkışında cüzdanın dolması, güçlendirme harcamasından sonra "bu yatırıma değdi" tatmini ve geri döndüğünde otofarm birikimlerinin onu beklemesi — bunların tamamı ekonomi sisteminin cömert ama sürdürülebilir dengesine bağlıdır.

MVP kapsamında 3 kaynak tipi (Altın, Enerji, Elmas), temel faucet/sink tanımları ve güçlendirme maliyet tablosu tasarlanır. Monetizasyon henüz belirlenmedi — ekonomi P2W olmadan tamamen oynanabilir olmalıdır.

## Player Fantasy

Oyuncu ekonomi sisteminde **cömert zenginleşme** fantezisi yaşar. Her zindan çıkışında altın yağmuru ekranı kaplar — "eli boş dönmedim, hatta beklenenden fazla kazandım" hissi. Güçlendirme masasına oturduğunda elinde yeterli kaynak olduğunu görür ve "bir tane daha güçlendirebilirim" dürtüsüyle oturumu uzatır. Geri döndüğünde otofarm birikimi onu bekler — "oynamasam bile kazandım" rahatlığı.

**Çekirdek duygu**: Bolluk ve yatırım tatmini. Oyuncu asla "param yetmiyor" frustrasyonu yaşamamalı — aksine, "neye harcasam?" seçimi yapmalı. Bu, kaynak kıtlığı yerine **kaynak seçimi** yaratan bir ekonomidir. Altın her zaman yeterli, ama Elmas ile daha hızlı ilerleyebilirsin — hiçbir zaman mecbur değilsin.

**Negatif fantazi (kaçınılacak)**: "Grind duvarı" — oyuncunun ilerlemek için aynı zindanı onlarca kez dönmesi. Ekonomi her oturumda anlamlı ilerleme sağlamalı. Bir oturumda en az 1 güçlendirme yapabilecek kaynak kazanılmalı.

**Pillar bağlantısı**: "Cömert Zindan" — her girişte cömert ödül. "Güç Hisset" — harcama = güçlenme, sayılar büyümeli. "Senin Tempon" — aktif oynayan daha hızlı kazanır ama pasif de ilerler.

## Detailed Rules

### Core Rules

**Kural 1 — Kaynak Tipleri**

| Kaynak | Enum | Rol | Kazanım Hızı | Oyuncu Hissi |
|--------|------|-----|-------------|--------------|
| **Altın** | `gold` | Genel para birimi — güçlendirme, satın alma, günlük işlemler | Yüksek (her savaşta) | "Her zaman kazanıyorum" |
| **Enerji** | `energy` | Zindan girişi yakıtı — oturum uzunluğunu doğal olarak kısıtlar | Zamanla dolar | "Yetecek kadar var, ama sınırsız değil" |
| **Elmas** | `gems` | Nadir/premium kaynak — hızlandırıcılar, özel satın almalar | Düşük (özel ödüller) | "Değerli, dikkatli harcamalıyım" |

**Kural 2 — Altın Faucet'ları (Kazanım Kaynakları)**

| Faucet | Kaynak | Miktar | Sıklık |
|--------|--------|--------|--------|
| Zindan kat temizleme | Her kat sonu | `base_gold_per_floor × floor_number × difficulty_multiplier` | Her kat |
| Zindan boss ödülü | Boss yenilgisi | `floor_gold × boss_gold_multiplier` (3.0x, Loot GDD) | Her 5. kat |
| Otofarm birikimi | Geri dönüşte | `idle_gold_per_minute × offline_minutes` (capped) | Pasif |
| Canavar satışı | Fazla canavar satma | `rarity_sell_value[rarity]` | İsteğe bağlı |

**Kural 3 — Altın Sink'leri (Harcama Noktaları)**

| Sink | Hedef | Maliyet | Sıklık |
|------|-------|---------|--------|
| Canavar seviye atlama | Canavar Güçlendirme | `level_up_cost(level, rarity)` | Sık |
| Canavar evrimi | Canavar Güçlendirme | `evolution_cost(rarity, stage)` | Nadir |
| Takım slot genişletme | Takım Kurma (ileride) | Sabit yüksek maliyet | Çok nadir |

**Denge prensibi**: Bir oturumluk zindan keşfi (5-10 kat), en az 1-2 canavar seviye atlama maliyetini karşılamalı. Oyuncu asla "güçlendirmek için 5 oturum grind yapmalıyım" hissetmemeli.

**Kural 4 — Enerji Sistemi**

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| Maksimum enerji | 100 | Enerji tavanı |
| Yenilenme hızı | 1 enerji / 5 dakika | Zamanla otomatik dolar |
| Tam dolum süresi | ~8.3 saat | 0'dan 100'e |
| Zindan kat maliyeti | 2 enerji / kat | Her kata giriş için |
| Oturum kapasitesi | 50 kat (100 enerji ile) | Tam enerjide ~50 kat keşfedilebilir |
| Enerji taşması | Yok | Maks üzerine çıkamaz (hediye hariç) |
| Reklam ile enerji | +20 enerji / reklam | Günde maks 5 kez (günlük reset) |

**Kural 5 — Elmas Kaynakları ve Kullanımları**

| Faucet | Miktar | Sıklık | Açıklama |
|--------|--------|--------|----------|
| İlk kez zindan kat temizleme (yıldız) | 5-20 elmas (kata göre) | Tek seferlik | İlk başarı ödülü |
| Boss ilk yenilgi | 50 elmas | Tek seferlik | Boss katı ilk başarısı |
| **Günlük giriş ödülü** | 7 elmas | Günlük | Her gün ilk giriş (9 gün = 63 elmas/ay) |
| **Haftalık görev ödülü** | 50 elmas | Haftalık | Haftada 5 görev tamamlama (200 elmas/ay) |
| **Arena ödülü** (Tier 2+) | 20-50 elmas | Haftalık sıralama | Arena sezon sonu ödülü |
| Başarım ödülleri | 10-100 elmas | Tek seferlik | Çeşitli başarımlar |

| Sink | Maliyet | Etki |
|------|---------|------|
| Enerji yenileme | 50 elmas → tam enerji | Anlık |
| Nadir canavar sandığı | 100 elmas → garantili C tier+ canavar | Tek seferlik çekim |
| Altın paketi | 20 elmas → altın×2 saat (mevcut kat kazanımı) | Hızlandırıcı |

**Kural 6 — Kaynak Tavanları**

| Kaynak | Tavan | Taşma Kuralı |
|--------|-------|--------------|
| Altın | 999.999 | Tavanda kazanım durur, uyarı gösterilir |
| Enerji | 100 | Zamanla dolan enerji tavanda durur. Elmas ile satın alınan enerji tavanı aşabilir (maks 200). |
| Elmas | 99.999 | Tavanda kazanım durur |

**Kural 7 — Cömert Zindan Garantisi**

"Cömert Zindan" sütununu iki katmanlı garanti ile korur:

**Katman 1 — Kat bazlı** (Loot GDD sahip): Her katta floor_gold'un en az %80'i garanti edilir (`comert_floor_ratio = 0.80`). Detaylar Loot/Ödül GDD'sinde.

**Katman 2 — Oturum bazlı** (bu GDD sahip): Her zindan oturumunun toplam altın getirisi, harcanan toplam enerjinin altın karşılığının en az 1.5x'ini döndürmelidir.

`minimum_session_gold = total_energy_spent × gold_per_energy_base × 1.5`

- `gold_per_energy_base` = 50 (kaynak: Loot GDD `gold_per_energy_base` registry)
- `total_energy_spent` = kat_sayısı × energy_per_floor (2)

**Örnek**: 5 kat oturumu → 10 enerji → minimum = 10 × 50 × 1.5 = 750 altın. Formül 2'ye göre 5 kattan kazanılan: 100 × (1+2+3+4+5) = 1.500 altın >> 750 ✓

**Not**: 1 katlık minimum oturumda (Floor 1: 100 altın, garanti: 150 altın) bu katman tutmaz. Katman 1 (kat bazlı %80 garanti) tek katlık senaryoları kapsar. Oturum garantisi 2+ kat için geçerlidir.

### States and Transitions

| Durum | Tetikleyici | Sonuç |
|-------|-------------|-------|
| **Enerji Dolu** (100) | Zaman geçişi veya satın alma | Yenilenme durur, zindan girişi mümkün |
| **Enerji Kısmi** (1-99) | Enerji harcanır veya yenilenir | Yenilenme aktif, zindan girişi mümkün |
| **Enerji Boş** (0) | Tüm enerji harcanır | Yenilenme aktif, zindan girişi engelli, elmas ile yenileme teklifi |
| **Altın Yeterli** | Altın ≥ maliyet | Güçlendirme/satın alma mümkün |
| **Altın Yetersiz** | Altın < maliyet | İşlem engelli, "Zindan'a git" yönlendirmesi |
| **Elmas Yeterli** | Elmas ≥ maliyet | Premium işlem mümkün |
| **Elmas Yetersiz** | Elmas < maliyet | İşlem engelli, kazanım yolları gösterilir |

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Güçlendirme** | → sağlar | Güçlendirme maliyet tablosu (altın+elmas) | `GetLevelUpCost(level, rarity)` → {gold, gems} |
| **Loot / Ödül** | ← ve → | Ödül miktarlarını tanımlar, ödülleri işler | `GetFloorReward(floorNumber, difficulty)` → {gold, gems, items}, `GrantReward(rewards)` |
| **Otofarm / Idle** | → sağlar | Idle kazanım oranı | `GetIdleGoldRate(teamPower)` → gold/minute |
| **Zindan Keşif** | → sağlar | Enerji maliyeti | `GetFloorEnergyCost(floorNumber)` → int |
| **Canavar Toplama** | → sağlar | Canavar satış değeri | `GetMonsterSellValue(rarity)` → gold |
| **Kaydetme/Yükleme** | ↔ | Kaynak durumu persist | `SaveResources()` / `LoadResources()` |
| **Savaş UI** | → sağlar | Anlık kaynak gösterimi | Altın/enerji/elmas miktarları |
| **Canavar Veritabanı** | ← okur | Nadirlik kademeleri (maliyet ölçekleme) | `rarity` enum |

## Formulas

### Formül 1: Seviye Atlama Maliyeti

`level_up_cost = floor(base_level_cost × rarity_cost_multiplier × level ^ cost_exponent)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel maliyet | base_level_cost | int | 50 | Lv1→Lv2 F tier maliyet |
| Nadirlik çarpanı | rarity_cost_multiplier | float | 1.0–3.0 | Nadir canavarlar daha pahalı |
| Seviye | level | int | 1–50 | Mevcut seviye |
| Maliyet üssü | cost_exponent | float | 1.5 | Kademeli artış eğrisi |

Nadirlik çarpan tablosu:

| Kademe | rarity_cost_multiplier |
|--------|----------------------|
| F | 1.0 |
| D | 1.3 |
| C | 1.8 |
| B | 2.4 |
| A | 3.0 |
| S | 3.5 |
| SS | 4.5 |

**Çıktı Aralığı**: 50 (F tier Lv1→2) – ~77.175 (SS tier Lv49→50: floor(50 × 4.5 × 49^1.5) = floor(50 × 4.5 × 343) = 77.175)

**Örnek**: C tier canavar Lv10→11: floor(50 × 1.8 × 10^1.5) = floor(50 × 1.8 × 31.623) = floor(2.846) = 2.846 altın

### Formül 2: Zindan Kat Altın Ödülü

`floor_gold = base_gold_per_floor × floor_number × difficulty_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Kat başı temel altın | base_gold_per_floor | int | 100 | Kat 1 altın ödülü |
| Kat numarası | floor_number | int | 1–100+ | Mevcut kat |
| Zorluk çarpanı | difficulty_multiplier | float | 1.0–2.0 | İleri bölgeler daha cömert |

**Çıktı Aralığı**: 100 (Kat 1) – 20.000+ (Kat 100)

**Örnek**: Kat 5: 100 × 5 × 1.0 = 500 altın. Kat 10 (boss): 100 × 10 × 1.5 = 1.500 altın.

### Formül 3: teamPower → active_gold_per_minute

`active_gold_per_minute = 10 + 90 × (clamp(teamPower, 60, 6864) - 60) / 6804`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Takım güç skoru | teamPower | int | 60–6864 | Takım Kurma GDD'den |
| Dakika başı aktif altın | active_gold_per_minute | float | 10–100 | Lineer interpolasyon |

**Çıktı Örnekleri:** TP=60 → 10/dk, TP=3462 → 55/dk, TP=6864 → 100/dk.

### Formül 4: Idle Altın Oranı (Azalan Getirili)

```
idle_gold = Tier1_gold + Tier2_gold + Tier3_gold

Tier1_gold = idle_gold_per_minute × min(D_off, 480)
Tier2_gold = idle_gold_per_minute × tier_2_multiplier × min(max(0, D_off - 480), 480)
Tier3_gold = idle_gold_per_minute × tier_3_multiplier × min(max(0, D_off - 960), 480)

idle_gold_per_minute = active_gold_per_minute × idle_efficiency
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Dakika başı idle altın | idle_gold_per_minute | float | 5–50 | active × idle_efficiency |
| Çevrimdışı süre | D_off | int | 0–1440 | Maks 24 saat birikimi |
| Idle verimlilik | idle_efficiency | float | 0.50 | Aktif oynamanın %50'si |
| Tier 2 çarpanı | tier_2_multiplier | float | 0.75 | 8-16 saat verimlilik |
| Tier 3 çarpanı | tier_3_multiplier | float | 0.50 | 16-24 saat verimlilik |

**Çıktı Örnekleri (idle_gold_per_minute = 25, orta oyun):**
- 8 saat (480 dk): 25 × 480 = **12.000 altın** (tamamı Tier 1)
- 16 saat (960 dk): 12.000 + 25 × 0.75 × 480 = **21.000 altın**
- 24 saat (1440 dk): 21.000 + 25 × 0.50 × 480 = **27.000 altın**

**Çıktı Aralığı**: 0 (0 dk) – 54.000 (24 saat, ileri oyun, idle_gold_per_minute=50)

**Cap**: Maksimum 24 saat birikimi (1440 dk). Bunun üzerinde birikim duraklar — geri dönüş motivasyonu. Azalan getiri kademeleri günde 2-3 giriş teşvik eder.

### Formül 5: Canavar Satış Değeri

`sell_value = base_sell_value × rarity_sell_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel satış değeri | base_sell_value | int | 100 | F tier canavar satışı |
| Nadirlik çarpanı | rarity_sell_multiplier | float | 1.0–10.0 | Nadir = daha değerli |

| Kademe | rarity_sell_multiplier | Satış Değeri |
|--------|----------------------|-------------|
| F | 1.0 | 100 |
| D | 2.0 | 200 |
| C | 4.0 | 400 |
| B | 7.0 | 700 |
| A | 12.0 | 1.200 |
| S | 20.0 | 2.000 |
| SS | 35.0 | 3.500 |

### Formül 6: Evrim Maliyeti

`evolution_cost = base_evolution_cost × rarity_evolution_multiplier × stage`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel evrim maliyeti | base_evolution_cost | int | 1.000 | F tier 1. evrim maliyeti |
| Kademe çarpanı | rarity_evolution_multiplier | float | 1.0–6.0 | Üst kademe evrim daha pahalı |
| Aşama | stage | int | 1–2 | Evrim aşama numarası (1. evrim=1, 2. evrim=2) |

| Kademe | Çarpan | Evrim 1 Maliyet | Evrim 2 Maliyet |
|--------|--------|----------------|----------------|
| F | 1.0 | 1.000 | 2.000 |
| D | 1.5 | 1.500 | 3.000 |
| C | 2.5 | 2.500 | 5.000 |
| B | 4.0 | 4.000 | 8.000 |
| A | 5.0 | 5.000 | 10.000 |
| S | 6.0 | 6.000 | — |
| SS | — | — | — |

### Formül 7: Enerji Yenilenme

`current_energy = min(max_energy, stored_energy + floor(elapsed_minutes / energy_regen_interval))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Maks enerji | max_energy | int | 100 | Tavan |
| Yenilenme aralığı | energy_regen_interval | int | 5 | Dakika başına |
| Geçen süre | elapsed_minutes | int | 0–∞ | Son kontrolden bu yana |

**Çıktı Aralığı**: 0–100 (normal), 0–200 (elmas ile satın alındığında)

## Edge Cases

- **If altın tavana ulaşırsa (999.999)**: Yeni altın kazanımı durur. UI'da "Cüzdan dolu — harca!" uyarısı gösterilir. Zindan ödülleri kaybolmaz, beklemeye alınır (tavan düşünce eklenir). *(Cömert Zindan: ödül asla kaybedilmemeli.)*

- **If enerji 0 iken oyuncu zindana girmeye çalışırsa**: Giriş engellenir. "Enerji yenileniyor — [kalan süre]" + iki seçenek sunulur: "Elmas ile hemen doldur? (50 elmas → tam enerji)" veya "Reklam izle → +20 enerji". Reklam seçeneği günde maksimum 5 kez kullanılabilir.

- **If oyuncu 24+ saat çevrimdışı kalırsa**: Idle birikimi 1440 dk (24 saat) ile sınırlı. Fazla süre hesaplanmaz. Geri dönüş ekranında "24 saatlik birikim!" gösterilir.

- **If elmas ile enerji satın alınır ve mevcut enerji >0 ise**: Satın alınan enerji mevcut enerjiye eklenir, tavanı aşabilir (maks 200). Zamanlı yenilenme tavan aşımı süresince duraklar.

- **If reklam ile enerji kazanılır ve günlük limit dolmuşsa**: "Reklam izle" butonu devre dışı kalır, "Yarın tekrar kullanılabilir" yazısı gösterilir. Elmas seçeneği her zaman aktif kalır.

- **If oyuncu son canavarını satmaya çalışırsa**: Engellenir — takımda en az 1 canavar olmalı. "Son canavarını satamazsın!" uyarısı.

- **If zindan ortasında oyuncu çıkarsa (çekilme/kaybetme)**: Enerji harcanmaz, loot verilmez. Oyuncu cezasız tekrar deneyebilir. *(Zindan Keşif GDD Kural 4 + Kural 10 — "Cömert Zindan": kayıp cezasızdır, enerji yalnızca başarılı temizlemede harcanır.)*

- **If negatif kaynak durumu oluşursa (bug/exploit)**: Kaynak minimum 0'a clamp edilir. Hata loglanır. İşlem geri alınmaz — oyuncu cezalandırılmaz.

- **If eş zamanlı iki harcama işlemi tetiklenirse**: İlk gelen işlenir, ikinci yetersiz bakiye ile reddedilir. Atomik işlem garantisi.

- **If güçlendirme maliyeti oyuncunun toplam altınını aşarsa**: İşlem engellenir. En yakın karşılanabilir güçlendirme önerilir. "X altın daha lazım — 1 kat daha temizle!" yönlendirmesi.

## Dependencies

### Upstream (Bu sistem neye bağlı)

Yok — Foundation katmanı, sıfır bağımlılık. Kaynak tipleri, maliyet tabloları ve ödül formülleri kendi içinde tanımlıdır.

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Güçlendirme** | Sert | `GetLevelUpCost(level, rarity)`, `GetEvolutionCost(rarity, stage)` | Olmadan güçlendirme maliyetsiz |
| **Loot / Ödül** | Sert | `GetFloorReward(floorNumber, difficulty)`, `GrantReward(rewards)` | Olmadan loot miktarı tanımsız |
| **Otofarm / Idle** | Sert | `GetIdleGoldRate(teamPower)` | Olmadan idle kazanım oranı yok |
| **Zindan Keşif** | Sert | `GetFloorEnergyCost(floorNumber)` | Olmadan enerji maliyeti yok |
| **Canavar Toplama** | Yumuşak | `GetMonsterSellValue(rarity)` | Olmadan satış özelliği çalışmaz |
| **Kaydetme/Yükleme** | Sert | `SaveResources()` / `LoadResources()` | Olmadan ilerleme kaybolur |
| **Savaş UI** | Yumuşak | Kaynak miktarlarını gösterir | Olmadan oyuncu kaynağını göremez |
| **Canavar Veritabanı** | Referans | `rarity` enum paylaşılır | Maliyet ölçekleme nadirliğe bağlı |

**Bağımlılık doğası**: Ekonomi sistemi aşağıya maliyet/ödül **tanımları** sağlar, yukarıdan veri almaz. Nadirlik enum'u Canavar Veritabanı ile paylaşılır (referans ilişkisi).

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `base_level_cost` | 50 | 30–100 | Güçlendirme pahalı → grind hissi | Çok ucuz → güçlendirme anlamsız |
| `cost_exponent` | 1.5 | 1.2–2.0 | Üst seviyeler ulaşılamaz | Maliyet düz artar → endgame sink yok |
| `base_gold_per_floor` | 100 | 50–200 | Altın çok hızlı birikir → enflasyon | Zindan ödülleri yetersiz → grind |
| `difficulty_multiplier` | 1.0–2.0 | 0.8–3.0 | İleri katlar çok cömert | İlerleme ödüllendirmez |
| `idle_efficiency` | 0.50 | 0.30–0.70 | Idle çok güçlü → aktif oynama gereksiz | Idle anlamsız → Senin Tempon ihlali |
| `max_offline_minutes` | 1440 | 720–1440 | Çok uzun birikim → altın enflasyonu (2880'de azalan getiriyle bile fazla) | Kısa cap → sık giriş baskısı |
| `energy_regen_interval` | 5 dk | 3–10 dk | Enerji çok hızlı dolar → sınırsız oynama | Enerji çok yavaş → frustrasyon |
| `max_energy` | 100 | 60–150 | Çok uzun oturumlar → pil/attention sorunu | Çok kısa oturumlar → tatminsizlik |
| `energy_per_floor` | 2 | 1–5 | Çok az kat gezilebilir → session kısa | Çok çok kat → enerji anlamsız |
| `ad_energy_reward` | 20 | 10–30 | Reklam çok değerli → elmas satışı düşer | Reklam değmez → kullanılmaz |
| `daily_ad_limit` | 5 | 3–10 | Çok fazla reklam → oyuncu yorulur | Az → seçenek kısıtlı |
| `gem_energy_refill_cost` | 50 | 30–100 | Elmas çok pahalı → kimse kullanmaz | Çok ucuz → enerji sistemi bypass |

**Etkileşim Uyarıları**:
- `base_gold_per_floor` × `idle_efficiency` birlikte idle kazanım gücünü belirler. İkisini artırmak aktif oynamayı devalüe eder.
- `energy_regen_interval` × `max_energy` birlikte tam dolum süresini belirler (şu an ~8.3 saat). Bu süre oyuncunun günde kaç oturum yapacağını şekillenir.
- `cost_exponent` × `rarity_cost_multiplier` birlikte endgame Legendary güçlendirme maliyetini belirler. Aşırı yüksek → P2W hissi.
- `ad_energy_reward` × `daily_ad_limit` birlikte günlük ücretsiz enerji miktarını belirler (şu an 100 = tam dolum). `gem_energy_refill_cost` ile denge kurulmalı.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Altın kazanımı | Altın sikke parçacıkları + sayı uçuşu (altın sarısı) | MVP |
| Enerji yenilenmesi | Yeşil enerji barı dolma animasyonu | MVP |
| Elmas kazanımı | Parlak mavi kristal parçacıkları | MVP |
| Idle birikimi toplama | Sandık açılışı efekti + altın yağmuru | MVP |
| Kaynak yetersiz | Kırmızı titreme + "yetersiz" ikonu | MVP |

### Audio Gereksinimleri

| Olay | Ses | Öncelik |
|------|-----|---------|
| Altın kazanımı | Metalik "clink-clink" (miktar arttıkça daha yoğun) | MVP |
| Güçlendirme harcaması | Tatmin edici "whoosh + sparkle" | MVP |
| Enerji bitti | Yumuşak "boş" ses, cezalandırıcı değil | MVP |
| Idle toplama | Uzun, tatmin edici sikke akışı sesi | MVP |

## UI Requirements

- **Kaynak barı (HUD)**: Ekranın üst kısmında her zaman görünür — Altın (sikke ikonu + sayı), Enerji (bar + sayı/maks + yenilenme sayacı), Elmas (elmas ikonu + sayı)
- **Enerji yenilenme sayacı**: Enerji dolu değilken, sonraki enerji noktasına kalan süre (mm:ss)
- **Güçlendirme maliyet göstergesi**: Güçlendirme ekranında mevcut altın vs maliyet karşılaştırması. Yeterliyse yeşil, yetersizse kırmızı.
- **Idle birikim ekranı**: Geri dönüşte tam ekran overlay — birikmiş altın + süre + "Topla!" butonu
- **Enerji boş popup**: Elmas ile dolum + reklam izle seçenekleri yan yana
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar

> 📌 **UX Flag — Ekonomi**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak HUD kaynak barı ve idle birikim ekranı için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** F tier canavar Lv1, **WHEN** seviye atlama maliyeti sorgulanırsa, **THEN** 50 altın döner.

2. **GIVEN** C tier canavar Lv10, **WHEN** seviye atlama maliyeti sorgulanırsa, **THEN** floor(50 × 1.8 × 10^1.5) = floor(2.846,07) = 2.846 altın döner.

3. **GIVEN** oyuncu Kat 5'i temizler, **WHEN** ödül hesaplanırsa, **THEN** en az 500 altın (100 × 5 × 1.0) verilir.

4. **GIVEN** oyuncu 8 saat çevrimdışı kalır (480 dk), **WHEN** geri döner, **THEN** idle birikimi = idle_gold_per_minute × 480 gösterilir.

5. **GIVEN** oyuncu 30 saat çevrimdışı kalır, **WHEN** geri döner, **THEN** idle birikimi 1440 dk ile sınırlıdır (24 saat cap).

6. **GIVEN** enerji 0, **WHEN** oyuncu zindana girmeye çalışırsa, **THEN** giriş engellenir, elmas yenileme + reklam seçenekleri sunulur.

7. **GIVEN** enerji 0, **WHEN** oyuncu reklam izlerse, **THEN** +20 enerji eklenir.

8. **GIVEN** günlük reklam limiti 5'e ulaştı, **WHEN** oyuncu reklam izlemek isterse, **THEN** buton devre dışı, "Yarın tekrar" mesajı gösterilir.

9. **GIVEN** 100 enerji, **WHEN** oyuncu 50 kat gezer (2 enerji/kat), **THEN** enerji 0'a düşer.

10. **GIVEN** F tier canavar, **WHEN** satış fiyatı sorgulanırsa, **THEN** 100 altın döner. SS tier → 3.500 altın.

11. **GIVEN** C tier canavar 1. evrimi, **WHEN** evrim maliyeti sorgulanırsa, **THEN** 2.500 altın döner.

12. **GIVEN** altın = 999.999, **WHEN** kat ödülü kazanılırsa, **THEN** ödül beklemeye alınır, kaybolmaz.

13. **GIVEN** bir oturumluk zindan (10 kat), **WHEN** toplam altın ödülü hesaplanırsa, **THEN** en az 1-2 güçlendirme maliyetini karşılar (Cömert Zindan garantisi).

14. **GIVEN** enerji 80, **WHEN** elmas ile tam dolum satın alınırsa, **THEN** enerji 180'e çıkar (tavanı aşar, maks 200).

## Open Questions

1. **Monetizasyon entegrasyonu**: Gerçek para ile elmas satın alma mekanizması MVP'de olacak mı? → Monetizasyon kararı ile beraber netleşecek.

2. **Reklam SDK entegrasyonu**: Hangi reklam ağı kullanılacak (AdMob, Unity Ads, IronSource)? → Teknik karar, ADR ile belirlenecek.

3. **Enflasyon testi**: Simülasyon ile 30 günlük oyuncu altın birikimi test edilmeli — sink'ler yeterli mi? → Prototype/balance testing aşamasında.

4. **Elmas-altın dönüşüm oranı**: Elmas ile doğrudan altın satın alma olmalı mı, yoksa sadece hızlandırıcılar mı? → Monetizasyon kararıyla beraber.
