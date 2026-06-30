# Canavar Güçlendirme (Monster Enhancement)

> **Status**: Designed
> **Author**: user + systems-designer, economy-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Güç Hisset, Topla Hepsini

## Overview

**Canavar Güçlendirme**, oyuncunun sahip olduğu canavar instance'larının güç seviyesini artırmasını yöneten üç katmanlı progression sistemidir. Canavar Veritabanı'ndan tür şablonlarını (base stat havuzu, arketip dağılımı, evrim yolu) ve Ekonomi'den maliyet tablolarını tüketerek, üç bağımsız güçlendirme ekseni tanımlar: **Seviye Atlama** (altın harcayarak stat büyütme, Lv1–50), **Evrim** (form değişimi ile stat havuzunu %40 artırma, görsel ve yetenek yükseltme) ve **Yıldız Sistemi** (aynı canavarın kopyalarını birleştirerek stat tavanını kalıcı olarak yükseltme). Her eksen bağımsız ilerler — seviye atlamak için evrim gerekmez, yıldız yükseltmek için seviye gerekmez — ancak üçü birlikte canavarın nihai güç tavanını belirler.

Sistem, Canavar Toplama ve Evrim'e güçlendirilmiş stat değerlerini, Koleksiyon UI'a güncel güç bilgilerini sağlar. Downstream sistemler canavar gücünü değiştirmez, bu sisteme sorarak okur. MVP kapsamında 3 güçlendirme ekseni, seviye başına stat büyüme formülleri, evrim malzeme gereksinimleri ve yıldız birleştirme kuralları tanımlanır.

## Player Fantasy

Güçlendirme sistemi oyuncunun **"benim canavarım gelişiyor"** fantezisinin doğrudan yaşandığı yerdir. Üç güçlendirme ekseni üç farklı duyguyu tetikler:

**Seviye atlama** güç büyümesinin tatminini verir — her seviyede stat sayıları gözle görülür şekilde artar, ATK 52→55, HP 31→33. "Seviye atla" butonuna her dokunuş anında sonuç verir; canavar daha güçlü, bu savaşta fark hissedilir. Sayıların büyümesi oyuncunun ilerleme ritmini belirler.

**Evrim** dönüşüm heyecanını yaratır — kopya canavar biriktirip gerekli seviyeye ulaştığında "Evrimleştir" butonuna basma anı oyunun doruk noktalarından biridir. Canavarın görünümü değişir, yeni yetenek açılır, stat havuzu %40 sıçrar. Bu an nadir ve değerlidir — her gün yaşanmaz, ama yaşandığında hatırlanır.

**Yıldız sistemi** yatırım karşılığının somut hissini verir — kopya canavarlar çöp değil, güç kaynağıdır. Her birleştirme stat tavanını kalıcı olarak yükseltir. "Bu kopya boşa gitmedi, canavarımı daha güçlü yaptı" düşüncesi Topla Hepsini sütununu pekiştirir: koleksiyonda her canavar değerli, fazlası bile.

**Negatif fantezi (kaçınılacak)**: "Güçlendirme duvarı" — oyuncunun kaynak yetersizliğinden güçlendirme yapamaması. Ekonomi GDD'nin "bir oturumluk zindan en az 1-2 güçlendirme karşılasın" prensibi bu fantezinin güvencesidir.

**Pillar bağlantısı**: "Güç Hisset" — her güçlendirme eylemi oyuncuyu somut olarak daha güçlü yapar. "Topla Hepsini" — kopya canavarlar bile değerli, koleksiyondaki her parça güce dönüşür.

## Detailed Rules

### Core Rules

**Kural 1 — Canavar Instance Yapısı**

Her oyuncu canavar instance'ı şu güçlendirme verilerini taşır:

| Alan | Tip | Açıklama | Varsayılan |
|------|-----|----------|-----------|
| `level` | int | Mevcut seviye | 1 |
| `current_xp` | int | Mevcut XP birikimi | 0 |
| `evolution_stage` | int | Evrim aşaması (1=A, 2=B, 3=C) | 1 |
| `star_rank` | int | Yıldız seviyesi | 0 |
| `pity_counter` | int | Evrim başarısızlık sayacı | 0 |

Bu alanlar Canavar Veritabanı'ndaki tür şablonunun üzerine biner — şablon immutable kalır, instance verileri bu sistemde yönetilir.

**Kural 2 — Seviye Atlama (XP Hibrit Sistemi)**

Her canavar bir XP barına sahiptir. XP iki yoldan kazanılır:

- **Savaş XP (pasif)**: Canavar savaşa katıldığında XP kazanır. Takımdaki canavarlar arası eşit bölünür.
- **Altın Enjeksiyon (aktif)**: Oyuncu altın harcayarak XP barını doldurabilir. Kısmi doldurma mümkün.

Seviye atlama kuralları:
1. XP barı dolduğunda canavar otomatik seviye atlar (XP barı sıfırlanır, fazla XP sonraki seviyeye taşınır)
2. Maksimum seviye: **50**
3. Altın ile tam bir seviyenin maliyeti = Ekonomi GDD `level_up_cost` formülü. Kısmi XP birikmiş ise maliyet orantılı azalır: `remaining_gold_cost = level_up_cost × (1 - current_xp / xp_threshold)`
4. "Hızlı Seviye" modu: Buton basılı tutulduğunda mevcut altınla mümkün olan kadar ardışık seviye atlar. **Güvenlik**: Toplam maliyet oyuncunun altınının %50'sini aşarsa veya 10+ seviye atlanacaksa onay dialogu gösterilir
5. Seviye arttıkça stat büyümesi uygulanır (Formüller bölümünde tanımlı)

**Kural 3 — Stat Büyüme (Seviye Başına)**

Her seviyede canavar statları büyür. Büyüme, Canavar Veritabanı'ndaki Lv1 base stat'tan hesaplanır:

`current_stat = floor(base_stat × (1 + growth_rate × (level - 1)))`

Büyüme oranı nadirlik kademesine göre değişir — nadir canavarlar seviye başına daha fazla büyür:

| Nadirlik | growth_rate | Lv50 toplam büyüme |
|----------|-------------|-------------------|
| Common | 0.02 | +98% (×1.98) |
| Uncommon | 0.022 | +108% (×2.08) |
| Rare | 0.025 | +122% (×2.22) |
| Epic | 0.028 | +137% (×2.37) |
| Legendary | 0.03 | +147% (×2.47) |

**Kural 4 — Evrim Sistemi**

Evrim, canavarın formunu değiştirir (A→B, B→C). Evrim koşulları:

| Gereksinim | A→B | B→C |
|-----------|-----|-----|
| Minimum seviye | 15 | 30 |
| Altın maliyeti | Ekonomi GDD `evolution_cost` formülü (stage=1) | Aynı formül (stage=2) |
| Evrim Malzemesi | Element Taşı ×3 | Element Taşı ×5 |
| Başarı oranı | %80 (base) | %60 (base) |

Evrim kuralları:
1. Evrim element ve arketipi **değiştirmez** (Canavar Veritabanı kararı)
2. Evrim stat havuzunu **%40 artırır** (evolution_bonus=0.40, registry'de kilitli)
3. Form B'de Slot 2 (Evrim Yeteneği) açılır
4. Evrim sonrası **seviye resetlenmez** — Lv15'te evrimleşen canavar Lv15 kalır
5. Evrim sonrası mevcut statlar yeni havuza göre yeniden hesaplanır

**Kural 5 — Evrim Pity Sistemi**

Evrim başarısız olabilir. Her denemede:

| Sonuç | Olan | Pity etkisi |
|-------|------|-------------|
| **Başarılı** | Canavar evrimleşir. Malzeme + altın harcanır. Pity sıfırlanır. | — |
| **Başarısız** | Canavar aynı kalır. Malzeme harcanır. Altının %50'si anında cüzdana iade edilir (ayrı bildirimle gösterilir). | Pity counter +1, başarı oranı +%15 |

Pity garantisi:
- A→B: Base %80 → %95 → %100 (maks 2 başarısızlık, 3. denemede garanti)
- B→C: Base %60 → %75 → %90 → %100 (maks 3 başarısızlık, 4. denemede garanti)

**Kural 6 — Evrim Malzemesi**

Evrim malzemeleri element bazlıdır:

| Malzeme | Element | Kullanım |
|---------|---------|----------|
| Ateş Taşı | Ateş | Ateş canavarı evrimi |
| Su Taşı | Su | Su canavarı evrimi |
| Toprak Taşı | Toprak | Toprak canavarı evrimi |
| Hava Taşı | Hava | Hava canavarı evrimi |

Kazanım kaynakları:
- **Normal zindan**: Nadir düşme (%5-10 per kat, elementle eşleşen bölgede)
- **Evrim zindanı**: Bol düşme (%80-100 per kat), element bazlı ayrı zindan, günde 3 giriş hakkı

Evrim zindanları Zindan Keşif Sistemi'nin bir alt türü olarak tasarlanacak.

**Kural 7 — Yıldız Sistemi (Ascension)**

Aynı canavarın kopyalarını birleştirerek stat tavanını kalıcı olarak yükseltir:

| Yıldız | Gereken Kopya | Kümülatif Kopya | Stat Bonusu | Etki |
|--------|---------------|-----------------|-------------|------|
| ★☆☆☆☆ | 1 | 1 | +5% | Tüm statlar %5 artar |
| ★★☆☆☆ | 2 | 3 | +10% | Tüm statlar %10 artar |
| ★★★☆☆ | 3 | 6 | +15% | Tüm statlar %15 artar |
| ★★★★☆ | 4 | 10 | +22% | Tüm statlar %22 artar |
| ★★★★★ | 5 | 15 | +30% | Tüm statlar %30 artar |

Yıldız kuralları:
1. Kopya canavar **aynı tür** olmalı (aynı `id`), evrim aşaması ve seviyesi farketmez
2. Birleştirilen kopya **tüketilir** (koleksiyondan kalkar)
3. Yıldız bonusu tüm statlara çarpımsal olarak uygulanır: `final_stat = calculated_stat × (1 + star_bonus)`
4. Yıldız, evrimden bağımsızdır — evrim yıldızı resetlemez, yıldız evrimi gerektirmez
5. Altın maliyeti: `star_gold_cost = base_star_cost × star_rank × rarity_cost_multiplier`

**Kural 8 — Güçlendirme Bağımsızlığı**

Üç eksen bağımsız ilerler:

| Eksen | Ön koşul | Diğer eksenlerle ilişki |
|-------|----------|------------------------|
| Seviye | Yok | Evrim min seviye gerektirir |
| Evrim | Min seviye + malzeme + altın | Seviye resetlemez, yıldız korunur |
| Yıldız | Kopya canavar | Seviye/evrim gerektirmez, korunur |

### States and Transitions

Canavar instance güçlendirme durumları:

| Durum | Tetikleyici | Sonuç | Sahip |
|-------|-------------|-------|-------|
| **Lv1 ★0 Form A** | Canavar kazanıldığında | Başlangıç durumu | Canavar Toplama |
| **XP Birikimi** | Savaş veya altın enjeksiyonu | XP barı dolar | Bu sistem |
| **Seviye Atlama** | XP barı threshold'u aşar | Statlar yeniden hesaplanır | Bu sistem |
| **Evrime Hazır** | Lv ≥ evrim eşiği + malzeme + altın yeterli | "Evrimleştir" butonu aktif | Bu sistem |
| **Evrim Denemesi** | Oyuncu "Evrimleştir" basar | Başarılı: form değişir, stat havuzu %40↑. Başarısız: pity counter↑ | Bu sistem |
| **Yıldız Yükseltme** | Oyuncu kopya birleştirir | Stat bonus uygulanır, kopya tüketilir | Bu sistem |
| **Max Güç** | Lv50 + Max Evrim + ★5 | Tüm güçlendirme eksenleri tamamlanmış | Bu sistem |

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | Base stat, arketip %, nadirlik, evrim yolu | `GetBaseStats(monsterId)`, `GetGrowthRates(monsterId)`, `GetEvolutionTarget(monsterId)` |
| **Ekonomi** | ← okur | Seviye maliyet, evrim maliyet, altın→XP oranı | `GetLevelUpCost(level, rarity)`, `GetEvolutionCost(rarity, stage)` |
| **Canavar Toplama ve Evrim** | → sağlar | Güçlendirilmiş stat değerleri, mevcut güç seviyesi | `GetEnhancedStats(instanceId)` → {hp, atk, def, spd} |
| **Koleksiyon UI** | → sağlar | Seviye, yıldız, evrim durumu, XP barı | `GetEnhancementInfo(instanceId)` |
| **Hasar Hesaplama** | → sağlar | Effective ATK, DEF (güçlendirilmiş) | `GetEnhancedStats()` üzerinden |
| **Sağlık/Can** | → sağlar | Effective Max HP (güçlendirilmiş) | `GetEnhancedStats()` üzerinden |
| **Zindan Keşif** | ← / → | Savaş XP kazanımı, evrim malzemesi düşmesi | `GrantBattleXP(instanceId, xpAmount)` |
| **Loot / Ödül** | ← okur | Evrim malzemesi loot tanımı | Loot tablosunda evrim taşı tanımlı |

**Önemli not**: Ekonomi GDD `level_up_cost` formülü şimdi "altın ile tam bir seviyenin maliyeti" olarak yorumlanır. Kısmi XP birikmiş ise maliyet orantılı azalır — formülün kendisi değişmez, uygulanma şekli genişler.

## Formulas

### Formül 1: Stat Büyüme (Seviye Başına)

`current_stat = floor(base_stat × (1 + growth_rate × (level - 1)))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel stat | base_stat | int | 15–79 | Canavar Veritabanı Lv1 stat |
| Büyüme oranı | growth_rate | float | 0.02–0.03 | Nadirlik kademesine göre |
| Seviye | level | int | 1–50 | Mevcut seviye |
| Güncel stat | current_stat | int | 15–195 | Sonuç |

**Çıktı Aralığı**: 15 (Common düşük stat Lv1) – 195 (Legendary yüksek stat Lv50)

**Örnek**: Nadir Saldırgan ATK=52, Lv50 → floor(52 × (1 + 0.025 × 49)) = floor(52 × 2.225) = **115**

### Formül 2: XP Eşiği (Seviye Başına)

`xp_threshold = floor(base_xp × level ^ xp_exponent)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel XP | base_xp | int | 50 | Lv1→2 XP gereksinimi |
| Seviye | level | int | 1–49 | Geçilen seviye |
| XP üssü | xp_exponent | float | 1.8 | Ölçekleme eğrisi |
| XP eşiği | xp_threshold | int | 50–56.703 | Gerekli XP |

**Çıktı Aralığı**: 50 (Lv1→2) – ~56.700 (Lv49→50). Kümülatif ~750.000 XP.

**Kilometre taşları**:

| Seviye | XP Eşiği | Kümülatif XP |
|--------|----------|-------------|
| 1→2 | 50 | 50 |
| 10→11 | 3.154 | ~12.500 |
| 25→26 | 15.547 | ~150.000 |
| 49→50 | 56.703 | ~750.000 |

### Formül 3: Savaş XP Kazanımı

`battle_xp = floor(floor(base_battle_xp × floor_number ^ 0.5) / team_size)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel savaş XP | base_battle_xp | int | 15 | Kat 1 toplam XP |
| Kat numarası | floor_number | int | 1–100+ | Zindan katı |
| Takım boyutu | team_size | int | 1–4 | XP bölünen canavar sayısı |
| Savaş XP | battle_xp | int | 4–37 | Canavar başına XP |

**Çıktı Aralığı**: 4 (Kat 1, 4 kişi) – 37 (Kat 100, 1 kişi)

**Örnek**: Kat 25, 4 kişi takım → floor(15 × 5.0) / 4 = floor(75/4) = **18 XP/canavar**

**Denge notu**: Kat 25'te 18 XP/canavar. Lv10→11 eşiği 3.155 XP → ~175 kat temizleme. Altın ile 2 kat yeter. Oran ~87:1 — pasif XP katkısı ~%10-15 seviyesinde, ana yol altın enjeksiyonu.

### Formül 4: Nihai Stat Pipeline

`final_stat = floor(base_stat × (1 + growth_rate × (level - 1)) × (1 + evolution_bonus) ^ (evo_stage - 1) × (1 + star_bonus))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel stat | base_stat | int | 15–79 | Lv1 stat |
| Büyüme oranı | growth_rate | float | 0.02–0.03 | Nadirlik |
| Seviye | level | int | 1–50 | Mevcut seviye |
| Evrim bonusu | evolution_bonus | float | 0.40 | Registry: kilitli |
| Evrim aşaması | evo_stage | int | 1–3 | A=1, B=2, C=3 |
| Yıldız bonusu | star_bonus | float | 0.00–0.30 | Yıldız sisteminden |
| Nihai stat | final_stat | int | 15–499 | Tam güçlendirilmiş |

**Çıktı Aralığı**: 15 (Common düşük stat, başlangıç) – 499 (Legendary yüksek stat, max her şey)

**Kilometre taşları (Nadir Saldırgan ATK=52)**:

| Aşama | Lv | Evrim | Yıldız | Hesaplama | Final ATK |
|-------|-----|-------|--------|-----------|-----------|
| Yeni kazanılmış | 1 | A | ★0 | 52 × 1.0 × 1.0 × 1.0 | **52** |
| Erken oyun | 15 | B | ★1 | 52 × 1.35 × 1.4 × 1.05 | **103** |
| Orta oyun | 25 | B | ★★ | 52 × 1.6 × 1.4 × 1.10 | **128** |
| Geç oyun | 50 | C | ★★★★★ | 52 × 2.225 × 1.96 × 1.30 | **294** |

### Formül 5: Yıldız Altın Maliyeti

`star_gold_cost = base_star_cost × star_rank × rarity_cost_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel yıldız maliyeti | base_star_cost | int | 500 | ★1 Common maliyeti |
| Yıldız sırası | star_rank | int | 1–5 | Yükseltilen yıldız |
| Nadirlik çarpanı | rarity_cost_multiplier | float | 1.0–3.0 | Ekonomi GDD tablosu |
| Yıldız maliyeti | star_gold_cost | int | 500–7.500 | Altın maliyeti |

**Nadirlik çarpanları**: Common=1.0, Uncommon=1.3, Rare=1.8, Epic=2.4, Legendary=3.0

**Maliyet tablosu**:

| Nadirlik | ★1 | ★2 | ★3 | ★4 | ★5 | Toplam |
|----------|-----|-----|-----|-----|-----|--------|
| Common | 500 | 1.000 | 1.500 | 2.000 | 2.500 | 7.500 |
| Uncommon | 650 | 1.300 | 1.950 | 2.600 | 3.250 | 9.750 |
| Rare | 900 | 1.800 | 2.700 | 3.600 | 4.500 | 13.500 |
| Epic | 1.200 | 2.400 | 3.600 | 4.800 | 6.000 | 18.000 |
| Legendary | 1.500 | 3.000 | 4.500 | 6.000 | 7.500 | 22.500 |

### Formül 6: Evrim Başarı Oranı

`success_rate = min(1.0, base_rate + pity_counter × pity_increment)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel oran | base_rate | float | 0.60–0.80 | A→B=0.80, B→C=0.60 |
| Pity sayacı | pity_counter | int | 0–3 | Başarısız deneme sayısı |
| Pity artışı | pity_increment | float | 0.15 | Deneme başına artış |
| Başarı oranı | success_rate | float | 0.60–1.00 | Min(1.0) ile sınırlı |

**Pity tablosu**:

| Deneme | A→B | B→C |
|--------|-----|-----|
| 1. | %80 | %60 |
| 2. (1 fail) | %95 | %75 |
| 3. (2 fail) | %100 ✓ | %90 |
| 4. (3 fail) | — | %100 ✓ |

**Ortalama deneme sayısı**: A→B: ~1.21, B→C: ~1.51

**Ortalama malzeme tüketimi**: A→B: ~3.6 taş (base 3), B→C: ~7.6 taş (base 5)

**Registry notu**: `damage_formula` output_range [1, 1800]'e güncellendi (2026-06-25) — güçlendirilmiş statlarla uyumlu.

## Edge Cases

- **If oyuncu Lv50 canavarı seviye atlamaya çalışırsa**: "Seviye Atla" butonu devre dışı. "Maksimum seviye!" gösterilir. XP kazanımı durur. Altın enjeksiyonu engellenir.

- **If XP fazlası bir sonraki seviyeyi de aşarsa (çok XP)**: Zincirleme seviye atlama uygulanır. Her seviye sırayla hesaplanır, fazla XP taşınır. Lv50'de taşma durur, fazla XP kaybolur.

- **If savaşta tüm takım Lv50 ise**: Savaş XP kazanılmaz (boşa gitmez, verilmez). Oyuncu bilgilendirilmez — sessiz optimizasyon.

- **If evrim denemesi başarısız ve oyuncunun malzemesi bitmişse**: Pity counter korunur. Sonraki deneme artırılmış oranla başlar. Oyuncu malzeme topladığında devam eder.

- **If evrim denemesi sırasında bağlantı kesilirse**: İşlem sunucu tarafında atomik. Ya tam başarılı ya tam başarısız. Yarım kalan durum yok. Kaydetme/Yükleme ile kurtarılır.

- **If oyuncu ★5 canavarın kopyasını daha kazanırsa**: Kopya normal canavar olarak koleksiyona eklenir. Birleştirme mümkün değil (maks yıldız). Fazla kopya satılabilir.

- **If aynı anda seviye atlama + evrim koşulları karşılanırsa**: İki bağımsız eylem. Oyuncu ikisini de yapabilir, sıra önemsiz. Evrim sonrası statlar yeniden hesaplandığından, evrimden önce veya sonra seviye atlama aynı sonucu verir.

- **If oyuncu kopya canavarı yıldız birleştirmeye koyarken o canavar takımda ise**: Engellenir — takımdaki canavar birleştirme malzemesi olamaz. Önce takımdan çıkarılmalı.

- **If star_bonus + evolution_bonus birlikte Lv1 canavar güçlendirilirse**: Formül doğru çalışır. Lv1 ★5 Form C canavar: base_stat × 1.0 × 1.96 × 1.30. Yıldız ve evrim seviye gerektirmez.

- **If evrim success_rate hesaplaması 1.0'ı aşarsa**: `min(1.0, ...)` ile clamp edilir. %100 üstü mümkün değil.

- **If oyuncunun altını seviye atlama maliyetinin sadece %10'unu karşılıyorsa**: Kısmi XP enjeksiyonu mümkün. Mevcut altınla karşılanabilecek kadar XP eklenir.

- **If evrim malzemesi yanlış element ise**: Evrim butonu aktif olmaz. Sadece doğru element taşı kabul edilir. UI'da gereken malzeme gösterilir.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetBaseStats(monsterId)`, `GetGrowthRates(monsterId)`, `GetEvolutionTarget(monsterId)` | Olmadan stat büyüme hesaplanamaz |
| **Ekonomi** | Sert | `GetLevelUpCost(level, rarity)`, `GetEvolutionCost(rarity, stage)` | Olmadan maliyet tanımsız |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Toplama ve Evrim** | Sert | `GetEnhancedStats(instanceId)`, `GetEvolutionRequirements(monsterId)` | Evrim akışını bu sistem yönetir |
| **Hasar Hesaplama** | Sert | `GetEnhancedStats()` → effective ATK, DEF | Güçlendirilmiş statlar hasar pipeline'ına girer |
| **Sağlık/Can** | Sert | `GetEnhancedStats()` → effective Max HP | Güçlendirilmiş HP |
| **Koleksiyon / Envanter UI** | Yumuşak | `GetEnhancementInfo(instanceId)` | Seviye, yıldız, evrim durumu görüntüleme |
| **Zindan Keşif** | Yumuşak | Evrim zindanı alt türü, savaş XP kaynağı | Evrim malzemesi düşme + XP kazanım |
| **Loot / Ödül** | Yumuşak | Evrim taşı loot tanımı | Loot tablosunda malzeme eklenmeli |

### Çapraz bağımlılık notları

- Canavar Veritabanı bu sistemi downstream olarak listeliyor ✅
- Ekonomi bu sistemi downstream olarak listeliyor ✅
- Hasar Hesaplama GDD: `effective_ATK`/`effective_DEF` güçlendirilmiş statlardan gelir → registry'de `final_stat_pipeline` referansı eklendi ✅
- Sağlık/Can GDD: `max_hp` güçlendirilmiş → registry'de `final_stat_pipeline` referansı eklendi ✅
- Zindan Keşif GDD: ✅ Yazıldı — evrim zindanı Tier 2+ olarak ertelenmiş (Zindan Keşif Open Q #1)
- Loot / Ödül GDD: ✅ Yazıldı — evrim taşı loot tanımı mevcut (Loot GDD Kural 5)

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `growth_rate_common` | 0.02 | 0.01–0.04 | Common çok güçlü → nadirlik farkı kapanır | Seviye atlama anlamsız |
| `growth_rate_uncommon` | 0.022 | 0.01–0.04 | — | — |
| `growth_rate_rare` | 0.025 | 0.015–0.045 | — | — |
| `growth_rate_epic` | 0.028 | 0.015–0.045 | — | — |
| `growth_rate_legendary` | 0.03 | 0.02–0.05 | Legendary aşırı güçlü → PvP meta kırılır | Legendary'nin "özel" hissi kaybolur |
| `max_level` | 50 | 30–100 | Çok uzun grind → endgame ulaşılamaz | Çok kısa → progression biter |
| `base_xp` | 50 | 25–100 | İlk seviyeler yavaş → onboarding bozulur | İlk seviyeler anlık → değersiz hisseder |
| `xp_exponent` | 1.8 | 1.5–2.2 | Üst seviyeler ulaşılamaz | Maliyet düz artar → endgame sink yok |
| `base_battle_xp` | 15 | 5–30 | Pasif XP çok güçlü → altın gereksiz | Pasif XP anlamsız → "Senin Tempon" ihlali |
| `evolution_level_req_ab` | 15 | 10–20 | Evrim çok geç → frustrasyon | Evrim çok erken → değersiz |
| `evolution_level_req_bc` | 30 | 20–40 | Evrim çok geç → endgame ulaşılamaz | Evrim çok erken → Form C ucuz |
| `evolution_material_ab` | 3 | 1–5 | Çok malzeme → grind duvarı | Çok kolay → evrim zindanı anlamsız |
| `evolution_material_bc` | 5 | 3–8 | — | — |
| `base_rate_ab` | 0.80 | 0.60–1.00 | 1.00 = garanti, heyecan yok | Çok düşük → frustrasyon |
| `base_rate_bc` | 0.60 | 0.40–0.80 | — | — |
| `pity_increment` | 0.15 | 0.10–0.25 | Garanti çok hızlı → risk anlamsız | Garanti çok geç → frustrasyon |
| `base_star_cost` | 500 | 200–1.000 | Yıldız çok pahalı → kopya sink zaten yeterli | Yıldız bedava → altın sink yok |
| `star_bonus_1` | 0.05 | 0.03–0.10 | Erken yıldızlar çok güçlü | İlk yıldız hissedilmez |
| `star_bonus_5` | 0.30 | 0.20–0.50 | ★5 çok baskın → ★5'siz yarışılamaz | ★5 hedeflemeye değmez |

**Etkileşim Uyarıları**:
- `growth_rate` × `evolution_bonus` × `star_bonus` birlikte nihai güç tavanını belirler. Üçünü aynı anda artırmak güç eğrisini patlatır.
- `base_battle_xp` × `base_xp` × `xp_exponent` birlikte pasif seviye atlama hızını belirler. `base_battle_xp` artırılırsa `base_xp` de artırılmalı.
- `evolution_material_ab` × normal zindan düşme oranı (%5-10) × evrim zindanı düşme oranı (%80-100) birlikte evrim malzemesi erişilebilirliğini belirler.
- `base_rate_ab`/`base_rate_bc` × `pity_increment` birlikte ortalama deneme sayısını belirler. Ekonomi GDD `evolution_cost` formülü bu ortalamayla çarpılmalıdır.
- Ekonomi GDD `level_up_cost` formülündeki `base_level_cost` (50) ve `cost_exponent` (1.5) bu sistemin altın enjeksiyon maliyetini doğrudan etkiler.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Seviye atlama | Stat sayıları yukarı uçuş animasyonu (yeşil) + parlama efekti | MVP |
| Hızlı seviye (basılı tut) | Ardışık parlama + sayı cascading efekti | MVP |
| Evrim başarılı | Tam ekran dönüşüm animasyonu — ışık patlaması + yeni form reveal | MVP |
| Evrim başarısız | Kısa titreme + kırılma efekti (kırmızı) + "%50 altın iade" ikonu | MVP |
| Yıldız yükseltme | Yıldız dolma animasyonu + kopya canavar absorbe efekti | MVP |
| ★5 tamamlama | Özel altın aura efekti — kalıcı parıltı | MVP |
| Evrim malzemesi düşmesi | Element renginde taş ikonu + parlama | MVP |

### Audio Gereksinimleri

| Olay | Ses | Öncelik |
|------|-----|---------|
| Seviye atlama | Kısa "level up" fanfarı + stat artış "ding" | MVP |
| Hızlı seviye | Hızlanan "ding-ding-ding" seri ses | MVP |
| Evrim başarılı | Epik dönüşüm müziği (2-3 sn) | MVP |
| Evrim başarısız | Kısa "crack" sesi + yumuşak "fail" tonu (cezalandırıcı değil) | MVP |
| Evrim denemesi (zar atma anı) | Gerilim tırmanması sesi (1-2 sn) | MVP |
| Yıldız yükseltme | Kristal birleşme sesi + yıldız "shine" | MVP |

> 📌 **Asset Spec** — Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:canavar-guclendirme` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- **Güçlendirme ana ekranı**: Canavar portresi (büyük) + mevcut statlar (HP/ATK/DEF/SPD) + 3 tab: Seviye / Evrim / Yıldız
- **Seviye tab'ı**: XP barı (mevcut/eşik) + "Seviye Atla" butonu (altın maliyeti gösterilir, kısmi XP varsa indirimli) + "Basılı tut = hızlı seviye" ipucu. Stat değişimi preview'ı (mevcut → yeni değerler yeşil okla)
- **Evrim tab'ı**: Mevcut form → hedef form görseli (yan yana) + gereksinim listesi (✅ seviye / ✅ altın / ❌ malzeme) + başarı oranı göstergesi (%80 gibi) + pity sayacı (varsa). "Evrimleştir" butonu tüm gereksinimler karşılanınca aktif
- **Yıldız tab'ı**: 5 yıldız göstergesi (dolu/boş) + "Birleştir" butonu + kopya canavar seçim listesi (takımdakiler gri, seçilemez) + altın maliyeti + stat bonus preview
- **Evrim animasyon ekranı**: Tam ekran overlay — gerilim buildup → başarı/başarısızlık sonucu → "Devam" butonu
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar

> 📌 **UX Flag — Canavar Güçlendirme**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak güçlendirme ekranı, evrim animasyonu ve yıldız birleştirme UI'ı için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Rare Striker (base ATK=52, growth=0.025) Lv1, **WHEN** Lv50'ye ulaşırsa, **THEN** current_ATK = floor(52 × 2.225) = **115**.

2. **GIVEN** Common canavar (growth=0.02) base HP=30 Lv1, **WHEN** Lv25'e ulaşırsa, **THEN** current_HP = floor(30 × 1.48) = **44**.

3. **GIVEN** herhangi bir canavar Lv1, **WHEN** XP eşiği sorgulanırsa, **THEN** xp_threshold = **50**.

4. **GIVEN** herhangi bir canavar Lv10, **WHEN** Lv10→11 XP eşiği sorgulanırsa, **THEN** xp_threshold = floor(50 × 10^1.8) = **3.154**.

5. **GIVEN** 4 kişilik takım, zindan kat 25, **WHEN** kat temizlenirse, **THEN** canavar başı XP = floor(15 × 5.0) / 4 = **18**.

6. **GIVEN** Rare Striker ATK=52, Lv15, Form B, ★1, **WHEN** final stat hesaplanırsa, **THEN** final_ATK = floor(52 × 1.35 × 1.4 × 1.05) = **103**.

7. **GIVEN** Rare Striker ATK=52, Lv50, Form C, ★5, **WHEN** final stat hesaplanırsa, **THEN** final_ATK = floor(52 × 2.225 × 1.96 × 1.30) = **294**.

8. **GIVEN** Rare canavar (rarity_cost_multiplier=1.8), **WHEN** ★3'e yükseltme talep edilirse, **THEN** star_gold_cost = 500 × 3 × 1.8 = **2.700** altın.

9. **GIVEN** B→C evrimi, pity_counter=2, **WHEN** evrim denenirse, **THEN** success_rate = min(1.0, 0.60 + 0.30) = **%90**.

10. **GIVEN** A→B evrimi, pity_counter=2, **WHEN** evrim denenirse, **THEN** success_rate = min(1.0, 1.10) = **%100** (garanti).

11. **GIVEN** Lv50 canavar, **WHEN** oyuncu seviye atlama dener, **THEN** işlem engellenir, buton devre dışı, XP kazanımı durur.

12. **GIVEN** Lv8 canavar, XP eşiğe 10 kala, **WHEN** 2 seviye dolduracak kadar XP kazanırsa, **THEN** Lv10'a ulaşır, fazla XP Lv10 barına taşınır.

13. **GIVEN** A→B evrimi başarısız (pity_counter=1), **WHEN** oturum kapatılıp yeniden açılırsa, **THEN** pity_counter=1 korunur, sonraki deneme %95 ile başlar.

14. **GIVEN** kopya canavar aktif takımda, **WHEN** yıldız birleştirme malzemesi olarak seçilmeye çalışılırsa, **THEN** işlem engellenir, "Önce takımdan çıkar" uyarısı.

15. **GIVEN** seviye atlama maliyeti 2.846 altın, oyuncunun 500 altını var, **WHEN** altın enjeksiyonu yaparsa, **THEN** 500 altın harcanır, XP barı orantılı dolar, seviye atlanmaz.

16. **GIVEN** Ateş canavarı, sadece Su Taşı var, **WHEN** evrim ekranı açılırsa, **THEN** buton devre dışı, "Ateş Taşı ×3" gösterilir.

17. **GIVEN** Rare Striker Lv15 Form B ★1 (final ATK=103), 3'lü Ateş sinerjisi (+15% ATK), vs DEF=17, avantajlı element, **WHEN** hasar hesaplanırsa, **THEN** effective_ATK=118, base_damage=110, element_damage=floor(110 × 1.5) = **165**.

18. **GIVEN** canavar base HP=31, Lv25, Form B, ★2, **WHEN** max_hp sorgulanırsa, **THEN** final_HP = floor(31 × 1.60 × 1.4 × 1.10) = **76**.

## Open Questions

1. **Otofarm sırasında XP kazanımı**: Otofarm modunda canavarlar XP kazanır mı? Kazanırsa ne oranda? (Aktif savaşın %50'si?) → Otofarm/Idle GDD'sinde netleşecek.

2. **Evrim zindanı detayları**: Günde 3 giriş hakkı, element bazlı — ama enerji maliyeti, zorluk seviyeleri, ödül tablosu? → Zindan Keşif GDD'sinde tanımlanacak.

3. **Yıldız sistemi ve kopya nadirliği**: Legendary canavar 15 kopya gerektiriyor (★5 için) — düşme oranı %1 ile bu gerçekçi mi? → Balance testing ile doğrulanacak.

4. **Evrim başarısızlık VFX tonu**: Başarısızlık efekti ne kadar "ağır" olmalı? Cezalandırıcı hissettirmemeli ama kayıp hissi vermeli. → Art director + UX designer ile netleşecek.

5. ~~**damage_formula registry güncellemesi**~~: **RESOLVED** — Registry'de `damage_formula` output_range [1, 1800]'e güncellendi (2026-06-25).
