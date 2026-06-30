# Loot / Ödül Sistemi (Loot / Reward System)

> **Status**: Designed
> **Author**: user + economy-designer, systems-designer
> **Last Updated**: 2026-06-25
> **Implements Pillar**: Cömert Zindan, Topla Hepsini, Güç Hisset

## Overview

**Loot / Ödül Sistemi**, zindan katlarından, boss savaşlarından ve özel etkinliklerden düşen tüm ödüllerin — altın, canavar, evrim malzemesi, elmas — ne olduğunu, ne sıklıkta düştüğünü ve nasıl dağıtıldığını yöneten merkezi ödül sistemidir. Her zindan katının bir loot tablosu vardır; bu tablo kat numarası, zorluk ve bölge tipine göre hangi ödüllerin hangi olasılıkla düşeceğini tanımlar. Sistem, Canavar Veritabanı'ndan nadirlik düşme çarpanlarını ve Ekonomi'den altın/elmas ödül formüllerini tüketerek, downstream sistemlere (Zindan Keşif, Otofarm, Canavar Toplama) hazır ödül paketleri sunar.

Oyuncu loot'u doğrudan toplamaz — düşen ödüller savaş sırasında görsel olarak ekranda belirir ve sistem tarafından otomatik toplanır. Asıl ödül deneyimi zindan sonu raporunda yaşanır: toplanan altın, kazanılan canavarlar, düşen malzemeler tek bir özet ekranında sunulur. "Cömert Zindan" sütununu mekanik olarak korumak için her zindan girişi, harcanan enerjinin altın karşılığının en az 1.5x'ini garanti eden bir minimum ödül tabanı uygular — oyuncu asla eli boş dönmez.

MVP kapsamında zindan kat loot tabloları, boss ödülleri, canavar düşme oranları (nadirlik çarpanlarıyla), evrim malzemesi düşme kuralları ve zindan sonu rapor yapısı tanımlanır.

## Player Fantasy

Loot / Ödül Sistemi'nde oyuncu **"her zindan cömert, her sandık heyecanlı"** fantezisini yaşar. İki katmanda hissedilir:

**Doğrudan deneyim**: Savaş sırasında düşman grupları yenildikçe ekranda altın sikkeleri, malzeme ikonları ve nadir canavar siluetleri belirir — "kazanıyorum" hissi her katta pekişir. Asıl doruk an zindan sonu raporudur: toplanan ödüller sırayla açılır, nadirlik sırasına göre gerilim artar. "Bu sefer ne düştü?" sorusu her zindan girişinin motivasyon çekirdeğidir. Nadir veya Epik canavar düştüğünde ekran altın parlamayla kaplanır — bu an oyuncunun arkadaşına "bak ne düştü!" diyeceği andır.

**Dolaylı etki**: Düşen her ödül bir güçlenme adımıdır. Altın → seviye atlama, evrim taşı → form değişimi, kopya canavar → yıldız yükseltme. Oyuncu zindan sonu raporuna baktığında sadece "ne kazandım" değil, "bununla ne yapabilirim" düşünür. Loot, güçlendirme döngüsünün yakıtıdır.

**Çekirdek duygu**: Bolluk ve keşif heyecanı. Her kat ödüllendirici, ama "bir kat daha" dürtüsü hiç bitmeyen bir merak yaratır — belki bir sonraki katta o Nadir canavar düşer.

**Negatif fantezi (kaçınılacak)**: "Boş kat" — oyuncunun bir katı temizleyip hiçbir şey kazanmaması. Cömert Zindan sütunu bunu yasaklar: her kat en azından altın ve bir miktar malzeme düşürür. Ayrıca "hayal kırıklığı loot'u" — oyuncunun 50 kat boyunca aynı Common canavarları görmesi. Çeşitlilik ve nadir sürprizler sisteme serpiştirilmeli.

**Pillar bağlantısı**: "Cömert Zindan" — minimum ödül tabanı her girişte tatmin sağlar. "Topla Hepsini" — canavar düşmeleri koleksiyon genişletme dürtüsünü besler. "Güç Hisset" — her loot bir güçlenme fırsatı.

## Detailed Design

### Core Rules

**Kural 1 — Loot Tipleri**

| Tip | Enum | Garanti/Random | Açıklama |
|-----|------|----------------|----------|
| **Altın** | `gold` | Garanti | Her katta düşer. Miktarı Ekonomi formülünden. |
| **Canavar** | `monster` | Random | Nadirlik çarpanlarıyla ağırlıklı. Koleksiyona eklenir. |
| **Evrim Malzemesi** | `evolution_material` | Random | Element taşları (ateş/su/toprak/hava). |
| **XP İksiri** | `xp_potion` | Random | Sabit XP miktarlı, 5 boyut. |
| **Elmas** | `gems` | Random (nadir) | Çoğunlukla tek seferlik ödüller + düşük tekrar oranı. |

**Kural 2 — Loot Tablosu Yapısı**

Her zindan katı iki katmanlı loot tablosuna sahiptir:

| Katman | İçerik | Hesaplama |
|--------|--------|-----------|
| **Garanti Katmanı** | Her zaman verilen ödüller | Sabit formüller (Ekonomi GDD) |
| **Random Katmanı** | Olasılık rulosuyla belirlenen ödüller | Her loot tipi için bağımsız olasılık kontrolü |

Loot tablosu seçimi üç parametreye bağlıdır:
- **Kat tipi**: Normal / Boss (her 5. kat) / Evrim Zindanı
- **Kat numarası**: İleri katlar daha cömert
- **Bölge**: Hangi canavarlar ve element taşları düşebileceğini belirler

**Kural 3 — Normal Kat Loot Tablosu**

| Loot Tipi | Garanti? | Base Oran | Miktar | Not |
|-----------|----------|-----------|--------|-----|
| Altın | ✅ Garanti | %100 | Ekonomi `floor_gold_formula` | Her katta |
| Canavar | ❌ Random | %15 | 1 adet | Nadirlik ağırlıklı (Kural 5). **Kat 1-3: %100 garanti** (Kural 3a) |
| Evrim Malzemesi | ❌ Random | %8 | 1 adet | Bölge elementine ağırlıklı |
| XP İksiri | ❌ Random | %20 | 1 adet | Boyut ağırlıklı (Kural 7) |
| Elmas | ❌ Random | %2 | 1-3 adet | Çok nadir |

**Kural 3a — Onboarding Garanti Canavarı (İlk 3 Kat)**

Zindan katları 1, 2 ve 3'te (normal katlar) canavar düşmesi %100 garantidir. Nadirlik normal weighted random ile belirlenir (Kural 5) — genellikle Common ama nadir sürpriz mümkündür. Bu kural "her sandık heyecanlı" fantezisini onboarding'den itibaren somutlaştırır.

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| `first_floors_guaranteed_monster` | 3 | İlk N normal katta canavar %100 garanti |

- Garanti canavar düştüğünde pity_counter artmaz (canavar düştü → sıfırda kalır)
- Bu kural tekrar oturumlarında da geçerlidir — Kat 1-3 her zaman en az 1 canavar verir
- Boss katları (Kat 5, 10) bu kuraldan etkilenmez — kendi tabloları geçerlidir

**Kural 4 — Boss Kat Loot Tablosu (Her 5. Kat)**

| Loot Tipi | Garanti? | Base Oran | Miktar | Not |
|-----------|----------|-----------|--------|-----|
| Altın | ✅ Garanti | %100 | `boss_gold = floor_gold × 3` | Boss bonusu |
| Normal Canavar | ❌ Random | %35 | 1 adet | Nadirlik ağırlıklı |
| Boss Canavar | ❌ Random | Boss nadirliğine göre (Kural 6) | 1 adet | Çok düşük |
| Evrim Malzemesi | ✅ Garanti | %100 | 1 adet + %30 ek 1 adet | Garanti 1, şansla 2 |
| XP İksiri | ❌ Random | %40 | 1 adet | Orta+ boyut garantili |
| Elmas | ❌ Random | %15 | 5-15 adet | Boss cömertliği |

**Kural 5 — Canavar Nadirlik Seçimi (Weighted Random)**

Bir canavar düştüğünde, nadirliği ağırlıklı rastgele seçimle belirlenir:

| Nadirlik | Düşme Ağırlığı | Normalize Olasılık |
|----------|----------------|-------------------|
| Common | 1.00 | %59.0 |
| Uncommon | 0.50 | %29.4 |
| Rare | 0.15 | %8.8 |
| Epic | 0.04 | %2.4 |
| Legendary | 0.01 | %0.6 |
| **Toplam** | **1.70** | **%100** |

Düşme ağırlıkları Canavar Veritabanı'ndaki `düşme çarpanı` ile aynıdır. Normalize olasılık: `ağırlık / toplam_ağırlık`.

Bölge canavar havuzundan seçim: Nadirlik belirlenince, o nadirlikte ve o bölgede mevcut canavarlar arasından eşit olasılıkla seçilir.

**Kural 6 — Boss Canavar Düşme Oranları**

Boss'un kendisinin düşmesi çok daha zordur:

| Boss Nadirliği | Düşme Oranı | Not |
|----------------|-------------|-----|
| Rare Boss | %5 | Her yenilgide %5 şans |
| Epic Boss | %3 | Çok nadir |
| Legendary Boss | %1 | Neredeyse efsanevi |

Boss canavarlar normal canavar havuzundan ayrı değerlendirilir — boss loot rulosu ve normal canavar rulosu birbirinden bağımsız çalışır. Bir boss katında hem normal canavar hem boss canavar düşebilir.

**Kural 7 — XP İksiri Sistemi**

5 boyut, sabit XP miktarlı:

| Boyut | Enum | XP Miktarı | Düşme Ağırlığı | Referans |
|-------|------|------------|----------------|----------|
| Mini | `xp_potion_mini` | 25 | 1.00 | Lv1→2'nin yarısı |
| Küçük | `xp_potion_small` | 100 | 0.60 | ~Lv1→3 |
| Orta | `xp_potion_medium` | 500 | 0.25 | ~Lv6→7 |
| Büyük | `xp_potion_large` | 2.000 | 0.08 | ~Lv9→10 |
| Dev | `xp_potion_giant` | 10.000 | 0.02 | ~Lv22→23 |

XP İksiri düştüğünde boyut ağırlıklı rastgele seçilir. İksir envanterde depolanır, oyuncu istediği canavara uygular.

**Kural 8 — Evrim Malzemesi Düşme**

| Zindan Tipi | Düşme Oranı | Element Eşleşmesi |
|-------------|-------------|-------------------|
| Normal Zindan | %8 per kat | Bölge elementine %70, diğer elementlere %10'ar |
| Evrim Zindanı | %90 per kat | Seçilen element %100 |
| Boss Katı | %100 garanti 1 + %30 ek | Bölge elementine ağırlıklı |

Evrim zindanları hakkında: Zindan Keşif GDD'sinde detaylandırılacak. Bu GDD sadece düşme oranlarını tanımlar.

**Kural 9 — Elmas Düşme**

| Kaynak | Miktar | Tip |
|--------|--------|-----|
| Normal kat (tekrar) | 1-3 | Random, %2 şans |
| Boss katı (tekrar) | 5-15 | Random, %15 şans |
| İlk kez kat temizleme | 5 (MVP'de sabit) | Tek seferlik |
| Boss ilk yenilgi | 50 | Tek seferlik |

Tek seferlik elmas ödülleri `first_clear` flag'i ile kontrol edilir — Kaydetme/Yükleme'de persist.

**Kural 10 — Progressive Pity (Artan Şans)**

Canavar düşme şansı her boş katta progresif olarak artar. İki katmanlı pity sistemi:

**10a — Monster Pity (Soft + Hard)**

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| `monster_pity_increment` | +%3 / kat | Her canavar düşmeyen normal katta base oran artar |
| `monster_pity_cap` | %45 | Soft cap — maksimum birikmiş oran |
| `monster_hard_pity` | 10 kat | Hard cap — 10 ardışık canavarssız normal kattan sonra garanti Common canavar |
| `monster_pity_reset` | Canavar düştüğünde | Herhangi bir canavar düşünce hem soft hem hard counter sıfırlanır |

Soft pity: Normal katta canavar base şansı %15. 5 kat boyunca canavar düşmezse: %15 + (5 × %3) = %30. Canavar düştüğünde sıfırlanır. `pity_counter` sınırlanmaz ama `effective_rate` min() ile %45'te cap'lenir.

Hard pity: 10 ardışık normal katta canavar düşmediyse, 11. normal katta Common canavar garanti düşer. Hard pity sadece normal katları sayar — boss katları (base_rate %35) ayrı değerlendirilir. Hard pity tetiklendiğinde canavar, bölge havuzundan Common nadirlikte rastgele seçilir.

**10b — Rare Pity (Nadirlik Koruma)**

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| `rare_pity_increment` | +%2 / düşen canavar | Rare+ düşmeyen her canavar düşmesinde Rare ağırlığı artar |
| `rare_pity_cap` | 20 | Maksimum rare_pity_counter değeri — counter 20'de clamp'lenir |
| `rare_pity_reset` | Rare+ düştüğünde | Rare veya üstü düşünce sıfırlanır |

Çalışma şekli: 10 canavar düştü hepsi Common/Uncommon → rare_pity_counter=10, Rare ağırlığı 0.15 + 10×0.02 = 0.35. Counter 20'de clamp'lenir: max adjusted_rare = 0.15 + 20×0.02 = 0.55, total_weight = 2.10, P(Rare) = %26.2, P(Epic) = %1.9, P(Legendary) = %0.48. Epic/Legendary dilüsyonu sınırlı tutulur.

Rare pity sadece **aktif** zindanda geçerlidir. İdle'da düşen Common/Uncommon canavarlar rare_pity_counter'ı artırmaz — idle nadirlik tablosu bağımsız çalışır.

**Kural 11 — Cömert Zindan Garantisi**

Her katta verilen altın, o katın beklenen altınının en az %80'i kadardır. Bu garanti kat numarasıyla ölçeklenir — tüm katlarda aktif koruma sağlar.

`min_gold_per_floor = floor_gold × comert_floor_ratio`

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| `comert_floor_ratio` | 0.80 | Her katta floor_gold'un %80'i garanti |

Eğer garanti katman + random katmandaki altın bu eşiğin altındaysa, fark bonus altın olarak eklenir. Normal koşullarda (difficulty_multiplier ≥ 1.0) garanti katman zaten floor_gold'un %100'ünü verdiğinden bu mekanizma tetiklenmez. Garanti, gelecekte difficulty_multiplier < 1.0 olan özel zindan modları veya zorluk ayarlamaları için aktif koruma sağlar.

Ek olarak, Ekonomi GDD Kural 7'deki oturum bazlı garanti de geçerlidir: her zindan girişi, harcanan toplam enerjinin altın karşılığının en az 1.5x'ini döndürmelidir.

**Kural 12 — Zindan Sonu Raporu**

Tüm katlardan toplanan loot tek bir rapor ekranında sunulur:

1. **Toplam altın** — animasyonlu sayaç (garanti, beklenen ödül → düşük gerilim)
2. **Evrim malzemeleri** — element ikonlarıyla (düşük-orta heyecan)
3. **XP İksirleri** — boyut ve adet (orta heyecan)
4. **Elmas** — varsa vurgulanır (yüksek heyecan)
5. **Kazanılan canavarlar** — nadirlik sırasına göre artan (Common önce, en nadir son), her biri kartla gösterilir. Rare+ canavar doruk anı. (en yüksek heyecan)
6. **İlk kez temizleme bonusları** — ayrı "YENİ!" işaretiyle (sürpriz finali)

Sıralama prensibi: Artan heyecan deseni (ascending excitement). Garanti ödüllerle başla, nadir sürprizlerle bitir. Son kart açılışı doruk anıdır — oyuncuyu raporun sonuna kadar ekrana bağlar.

### States and Transitions

| Durum | Tetikleyici | Sonuç |
|-------|-------------|-------|
| **Loot tablosu seçimi** | Kat temizlenir | Kat tipi + numara + bölge → loot tablosu belirlenir |
| **Garanti katman hesaplama** | Tablo belirlenir | Altın formülü hesaplanır, boss ise ek garanti eklenir |
| **Random katman ruloları** | Garanti sonrası | Her loot tipi için bağımsız olasılık kontrolü yapılır |
| **Boss canavar rulosu** | Boss katı + random katman sonrası | Normal canavar rulosundan bağımsız: boss'un kendisinin düşme kontrolü (Kural 6 oranlarıyla). İki rulo aynı katta birlikte çalışır. |
| **Hard pity kontrolü** | Random rulo sonrası canavar düşmedi | `hard_pity_counter` 10'a ulaştıysa → garanti Common canavar. Sadece normal katları sayar. |
| **Pity güncelleme** | Tüm rulo sonuçları belirlenir | Canavar düşmediyse pity counter artar; düştüyse (normal, boss veya hard pity) sıfırlanır |
| **Cömert Zindan kontrolü** | Tüm loot hesaplandı | Minimum altın eşiği kontrol edilir, gerekirse bonus eklenir |
| **Loot paketi oluşturma** | Tüm kontroller tamamlandı | Tek kat için loot paketi hazır |
| **Birikmiş loot** | Zindan devam ediyor | Her kat loot'u oturum boyunca birikir |
| **Zindan sonu raporu** | Zindan tamamlanır veya oyuncu çıkar | Birikmiş tüm loot gösterilir |
| **Envantere ekleme** | Rapor kapatılır | Altın → ekonomi, canavar → koleksiyon, malzeme → envanter |

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | Nadirlik düşme çarpanları, bölge canavar havuzu | `GetRarityWeight(rarity)`, `GetRegionMonsterPool(regionId)` |
| **Ekonomi** | ← okur + → çağırır | Altın formülleri; ödül verme | `GetFloorReward(floorNumber, difficulty)` → {gold}, `GrantReward(rewards)` |
| **Canavar Güçlendirme** | → sağlar | Evrim malzemesi loot tanımı | Loot tablosunda element taşı kayıtlı |
| **Canavar Toplama** | → çağırır | Yeni canavar instance oluşturma | `OnMonsterDropped(monsterId)` |
| **Zindan Keşif** | ← okur | Kat numarası, kat tipi, bölge, boss bilgisi | `GetCurrentFloorInfo()` → {floor, type, region, boss_id} |
| **Otofarm / Idle** | → sağlar | Idle loot hesaplama oranları | `GetIdleLootRate(teamPower, region)` → loot/minute |
| **Kaydetme/Yükleme** | ↔ | Pity counter'lar, first_clear flagları persist | `SaveLootState()` / `LoadLootState()` |
| **Zindan Sonu UI** | → sağlar | Rapor verisi | `GetSessionLootReport()` → {gold, monsters[], materials[], potions[], gems} |

## Formulas

### Formül 1: Loot Roll (Kat Başı Olasılık)

Her random loot tipi için bağımsız olasılık kontrolü:

`loot_dropped = rand() < effective_rate`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel oran | base_rate | float | 0.02–0.90 | Loot tipine özel (Kural 3-4) |
| Pity bonusu | pity_bonus | float | 0.00–0.30 | Sadece canavar için |
| Efektif oran | effective_rate | float | 0.02–0.45 | `min(pity_cap, base_rate + pity_bonus)` |

Kat numarasına bağlı oran ölçeklemesi yoktur — katlar arası fark altın formülüyle (Ekonomi GDD) ve boss kat tablolarıyla sağlanır.

### Formül 2: Canavar Nadirlik Seçimi (Weighted Random)

`P(rarity) = weight(rarity) / total_weight`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Nadirlik ağırlığı | weight(r) | float | 0.01–1.00 | Canavar Veritabanı düşme çarpanı |
| Toplam ağırlık | total_weight | float | 1.70–2.10 | Tüm ağırlıkların toplamı. Base 1.70; rare pity aktifken dinamik olarak yeniden hesaplanır (bkz. Formül 4) |
| Olasılık | P(r) | float | 0.006–0.588 | Normalize olasılık |

**Çıktı**: Common=%59.0, Uncommon=%29.4, Rare=%8.8, Epic=%2.4, Legendary=%0.6

**Örnek**: `rand()=0.72` → Kümülatif: Common [0, 0.588), Uncommon [0.588, 0.882) → 0.72 Uncommon aralığında → Uncommon seçilir.

### Formül 3: Progressive Pity — Canavar (Soft + Hard)

**Soft pity**: `effective_monster_rate = min(pity_cap, base_monster_rate + pity_counter × pity_increment)`

**Hard pity**: `if hard_pity_counter >= monster_hard_pity → garanti Common canavar`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel canavar oranı | base_monster_rate | float | 0.15 | Normal kat |
| Pity artışı | pity_increment | float | 0.03 | Kat başı |
| Pity sayacı | pity_counter | int | 0–∞ | Canavarssız kat sayısı (clamp yok, cap formülle sağlanır) |
| Pity tavanı | pity_cap | float | 0.45 | Soft cap — maks birikmiş oran |
| Hard pity sayacı | hard_pity_counter | int | 0–10 | Ardışık canavarssız normal kat (boss katları hariç) |
| Hard pity eşiği | monster_hard_pity | int | 10 | Bu değere ulaşınca garanti Common |

**Çıktı Aralığı**: %15 (sıfır pity) – %45 (10+ kat sonra soft cap). 10 ardışık normal katta düşmezse → garanti.

**Örnek**: 7 kat canavarssız → min(0.45, 0.15 + 7×0.03) = min(0.45, 0.36) = **%36**

Pity'siz 10 normal katta canavar düşmeme: 0.85^10 = %19.7. Soft pity ile: (1-0.15)×(1-0.18)×(1-0.21)×(1-0.24)×(1-0.27)×(1-0.30)×(1-0.33)×(1-0.36)×(1-0.39)×(1-0.42) = **~%3.2** — 6x iyileşme. Hard pity ile: 10 normal kattan sonra **%0** (garanti).

Gerçek 10 katlık oturumda (8 normal + 2 boss katı) 0 canavar olasılığı çok daha düşüktür — boss katlarında base_rate %35.

### Formül 3b: İdle Pity (Otofarm İçin)

İdle-pity aktif pity'den tamamen bağımsız çalışır. İdle'da `idle_loot_efficiency = 0.25` uygulandığı için base oran düşüktür (%3.75/kat); pity bu düşük oranı telafi eder.

```
idle_monster_chance = min(idle_pity_cap, base_monster_rate × idle_loot_efficiency + idle_pity_bonus)
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| İdle pity bonusu | idle_pity_bonus | float | 0.0–0.30 | Oturumlar arası kalıcı |
| Pity artışı | idle_pity_increment | float | 0.06 | Her idle simüle edilen kat başına (canavarssız). İdle-spesifik (aktif: 0.03, 2x hızlı ramp) |
| İdle pity tavanı | idle_pity_cap | float | 0.30 | Efektif idle canavar oranının max değeri. Aktiften (%45) düşük — "Senin Tempon" pillar'ı: idle daha az verimli. |
| Min offline süresi | min_offline_for_pity | int | 30 dk | Pity birikimi için minimum çevrimdışı süre |

**Efektif oran cap'i**: `idle_monster_chance` asla %30'u aşamaz. Teorik max (0.0375 + 0.30 = 0.3375) cap ile %30'da kesilir. Aktif pity cap'inden (%45) düşük — idle modun aktif oynamadan belirgin şekilde az verimli olmasını sağlar.

**Aktif pity ile farklar:**
- Aktif pity `pity_counter` (int, sınırsız ama cap formülle uygulanır) kullanır; idle pity `idle_pity_bonus` (float 0.0-0.30) kullanır — float accumulator, 5 canavarssız idle kat'ta tavana ulaşır.
- İdle pity increment 0.06 (aktif: 0.03) — idle'ın düşük base rate'ini (%3.75 vs %15) telafi eder.
- İdle pity yalnızca `offline_duration >= 30 dk` olan oturumlarda birikir (mikro-oturum koruması). Bu eşik **prospektiftir**: 30 dk'dan kısa çevrimdışı kalınırsa o oturumda idle pity hiç birikmez. 30 dk+ kalınırsa tüm idle katlar için pity birikir.
- Aktif pity her zindan oturumu sonunda ve `OnApplicationPause`'da persist edilir; idle pity `idle_state.idle_pity_bonus` olarak save'de persist edilir.
- İdle pity oturum değişikliğinde **sıfırlanmaz** — yalnızca canavar düştüğünde sıfırlanır.
- **Tam izolasyon**: İdle pity, aktif pity counter'ını etkilemez ve aktif pity idle pity'yi etkilemez. Aktif zindanda canavar düşmesi idle_pity_bonus'u **sıfırlamaz**. İdle pity yalnızca idle modda canavar düştüğünde sıfırlanır. Oyuncu idle'dan %30 pity ile çıkıp aktif oynayıp geri dönerse idle pity korunur.
- İdle'da düşen canavarlar **rare_pity_counter'ı artırmaz** — idle nadirlik tablosu bağımsız çalışır, rare pity sadece aktif zindanda geçerlidir.

**İdle nadirlik tablosu** (canavar düştüğünde — ağırlık formatında):

| Nadirlik | Ağırlık | Normalize Olasılık |
|----------|---------|-------------------|
| Common | 0.70 | %70.0 |
| Uncommon | 0.24 | %24.0 |
| Rare | 0.05 | %5.0 |
| Epic | 0.008 | %0.8 |
| Legendary | 0.002 | %0.2 |
| **Toplam** | **1.000** | **%100** |

### Formül 4: Rare Pity (Sadece Rare Ağırlığına, Cap'li)

`adjusted_rare_weight = base_rare_weight + min(rare_pity_cap, rare_pity_counter) × rare_pity_increment`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Rare temel ağırlığı | base_rare_weight | float | 0.15 | Canavar Veritabanı |
| Rare pity artışı | rare_pity_increment | float | 0.02 | Common/Uncommon düştükçe |
| Rare pity sayacı | rare_pity_counter | int | 0–20 | Rare+ düşmeyen canavar sayısı, 20'de clamp'lenir |
| Rare pity cap | rare_pity_cap | int | 20 | Counter bu değerde durur |
| Düzeltilmiş ağırlık | adjusted_rare_weight | float | 0.15–0.55 | Rare ağırlığı artar, max 0.55 |

Epic (0.04) ve Legendary (0.01) ağırlıkları sabit kalır. Sadece Rare ağırlığı artar → toplam ağırlık büyür → Rare oranı yükselir, diğerleri orantılı düşer. Cap sayesinde Epic/Legendary dilüsyonu sınırlı tutulur.

**Cap'teki değerler** (rare_pity_counter=20): adjusted_rare = 0.15 + 20×0.02 = 0.55. Total = 1.00+0.50+0.55+0.04+0.01 = 2.10. P(Rare) = %26.2, P(Epic) = %1.9 (base %2.4'ten), P(Legendary) = %0.48 (base %0.6'dan). Dilüsyon kabul edilebilir seviyede.

**Örnek**: 10 Common/Uncommon canavar düştü → adjusted_rare = 0.15 + 10×0.02 = 0.35. Yeni total = 1.00+0.50+0.35+0.04+0.01 = 1.90. P(Rare) = **%18.4** (base %8.8'den).

Rare pity sadece **aktif** zindanda geçerlidir (bkz. Formül 3b).

### Formül 5: XP İksiri Boyut Seçimi

`P(size) = weight(size) / total_potion_weight`

| Boyut | XP | Ağırlık | P(size) | Beklenen XP katkısı |
|-------|-----|---------|---------|---------------------|
| Mini | 25 | 1.00 | %51.3 | 12.8 |
| Küçük | 100 | 0.60 | %30.8 | 30.8 |
| Orta | 500 | 0.25 | %12.8 | 64.1 |
| Büyük | 2.000 | 0.08 | %4.1 | 82.1 |
| Dev | 10.000 | 0.02 | %1.0 | 102.6 |
| **Toplam** | — | **1.95** | **%100** | **292.4 XP/iksir** |

### Formül 6: Boss Altın Bonusu

`boss_gold = floor_gold × boss_gold_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Kat altını | floor_gold | int | 100–20.000 | Ekonomi `floor_gold_formula` |
| Boss çarpanı | boss_gold_multiplier | float | 3.0 | Sabit 3x |
| Boss altını | boss_gold | int | 300–60.000 | Sonuç |

**Örnek**: Kat 5 boss → 100×5×1.0×3 = **1.500 altın**. Kat 10 boss → 100×10×1.0×3 = **3.000 altın**.

### Formül 7: Cömert Zindan Garantisi (Kat-Ölçekli)

`min_gold_per_floor = floor_gold × comert_floor_ratio`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Kat altını | floor_gold | int | 100–20.000 | Ekonomi `floor_gold_formula` |
| Cömert kat oranı | comert_floor_ratio | float | 0.80 | Her katta floor_gold'un %80'i garanti |
| Minimum altın | min_gold_per_floor | int | 80–16.000 | Kat numarasıyla ölçeklenir |

**Uygulama**: Normal koşullarda (difficulty_multiplier ≥ 1.0) garanti katman zaten floor_gold'un %100'ünü verdiğinden tetiklenmez. Garanti, `difficulty_multiplier < 1.0` olan özel zindan modları veya gelecek zorluk ayarlamaları için aktif koruma sağlar. Örnek: difficulty=0.7, Kat 10 → floor_gold = 100×10×0.7 = 700. min_gold = 700×0.8 = 560. Garanti katman 700 verdiğinden tetiklenmez. difficulty=0.5 → floor_gold = 500, garanti katman 500 → yine tetiklenmez. Garanti yalnızca garanti katman çıktısının floor_gold×0.8'in altına düştüğü edge case'lerde devreye girer.

Ek olarak, Ekonomi GDD Kural 7'deki oturum bazlı garanti de geçerlidir: her zindan girişi toplam enerjinin altın karşılığının en az 1.5x'ini döndürmelidir.

### Formül 8: Idle Loot Oranı

İdle'da iki ayrı verimlilik parametresi uygulanır:

| Parametre | Değer | Uygulandığı Alan | Kaynak |
|-----------|-------|------------------|--------|
| `idle_efficiency` | 0.50 (%50) | Altın kazanımı | Ekonomi GDD Formül 4 |
| `idle_loot_efficiency` | 0.25 (%25) | Canavar ve malzeme düşme oranları | Bu GDD |

**Altın** — Azalan getirili kademeli formül (Ekonomi GDD Formül 4 / Otofarm GDD ile tutarlı):

`idle_gold_per_minute = active_gold_per_minute × idle_efficiency` (idle_efficiency = 0.50)

`idle_gold = Tier1 + Tier2 + Tier3` — Tier1: `idle_gpm × min(D,480)`, Tier2: `idle_gpm × 0.75 × min(max(0,D-480),480)`, Tier3: `idle_gpm × 0.50 × min(max(0,D-960),480)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Aktif altın/dk | active_gold_per_minute | float | 10–100 | Takım gücüne göre (Ekonomi GDD) |
| İdle altın verimi | idle_efficiency | float | 0.50 | Aktif altının %50'si |
| Idle altın/dk | idle_gold_per_minute | float | 5–50 | active_gpm × 0.50 |
| Çevrimdışı süre | D_off | int | 0–1440 | Maks 24 saat |
| Tier 2 çarpanı | — | float | 0.75 | 8-16 saat verimlilik |
| Tier 3 çarpanı | — | float | 0.50 | 16-24 saat verimlilik |

**Loot** (aktifin %25'i — kat-tabanlı stokastik, Otofarm GDD detayları):

`idle_monster_chance_per_floor = min(idle_pity_cap, base_monster_rate × idle_loot_efficiency + idle_pity_bonus)` (base: %3.75/kat + idle pity, cap %30)
`idle_material_chance_per_floor = base_material_rate × idle_loot_efficiency = %2/kat`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Idle loot verimi | idle_loot_efficiency | float | 0.25 | Aktif loot oranlarının %25'i |
| Kat başı canavar | idle_monster/floor | float | 0.0375–0.30 | %15 × 0.25 + idle pity, cap %30 |
| Kat başı malzeme | idle_material/floor | float | 0.02 | %8 × 0.25 |

**Idle canavar nadirliği**: İdle nadirlik tablosu kullanılır (bkz. Formül 3b). XP İksiri ve Elmas idle'da düşmez. İdle'da düşen canavarlar rare_pity_counter'ı artırmaz.

### Formül 9: Beklenen Loot / 10 Kat Oturumu

Senaryo: Kat 1-10, difficulty_multiplier=1.0, sıfır pity, **tekrar oturumu** (first_clear yok).

| Loot Tipi | Hesaplama | Beklenen Miktar |
|-----------|-----------|-----------------|
| **Altın** | Normal: 100×(1+2+3+4+6+7+8+9)=4.000 + Boss: 1.500+3.000 | **~8.500 altın** |
| **Canavar** | Kat 1-3 garanti: 3 + Kat 4,6,7,8,9: 5×0.15 + Boss: 2×0.35 | **~4.45 canavar** |
| **Evrim Malzemesi** | 8×0.08 + 2×1.0 + 2×0.30 | **~3.2 malzeme** |
| **XP İksiri** | 8×0.20 + 2×0.40 | **~2.4 iksir (~702 XP)** |
| **Elmas (tekrar)** | 8×0.02×avg(2) + 2×0.15×avg(10) | **~3.3 elmas** |

**İlk temizleme ek ödülleri** (sadece ilk seferde, tekrarda verilmez):

| Kaynak | Hesaplama | Miktar |
|--------|-----------|--------|
| Normal kat ilk temizleme | 8 kat × 5 elmas | 40 elmas |
| Boss ilk yenilgi (Kat 5) | 1 boss × 50 elmas | 50 elmas |
| Son Boss ilk yenilgi (Kat 10) | 1 boss × 100 elmas | 100 elmas |
| **Toplam first_clear** | — | **~190 elmas** |

İlk temizleme elmasları tekrar eden gelirin ~58x'i — bu beklenen davranıştır (keşif ödülü). Tekrar oturumlarda elmas geliri çok düşüktür (~3.3/oturum).

**Ekonomi kontrolü**: ~8.500 altın → Common canavar Lv1→11 (10 seviye atlama, maliyet ~7.130 altın) veya Rare canavar Lv1→9 (8 seviye, maliyet ~7.250×1.5 rarity_cost = ~5.880 altın). "Bir oturumda 1-2 güçlendirme" garantisi **sağlanıyor**. ~4.45 canavar/oturum ile koleksiyon ilerlemesi hızlı ve tatmin edici.

## Edge Cases

- **If bir katta hem normal canavar hem boss canavar düşerse**: Her ikisi de kabul edilir — iki bağımsız rulo, iki canavar koleksiyona eklenir. Zindan sonu raporunda ayrı gösterilir.

- **If pity_counter soft cap'e ulaşır (%45) ve hâlâ canavar düşmezse**: Oran %45'te kalır. Hard pity devreye girer: 10 ardışık canavarssız normal kattan sonra Common canavar garanti düşer. Boss katları hard pity sayacına dahil değildir. Hard pity tetiklendiğinde bölge havuzundan rastgele Common canavar seçilir ve tüm pity counter'lar sıfırlanır.

- **If rare_pity_counter cap'e ulaşırsa (20 Common/Uncommon ardışık)**: Counter 20'de clamp'lenir. adjusted_rare = 0.15 + 20×0.02 = 0.55. Total = 2.10. P(Rare) = %26.2, P(Epic) = %1.9, P(Legendary) = %0.48. Dilüsyon sınırlı, sistem stable.

- **If oyuncu zindan ortasında çıkarsa (çekilme/kaybetme)**: Loot verilmez — loot yalnızca kat tamamen temizlendiğinde dağıtılır (Zindan Keşif GDD Kural 10). Enerji harcanmaz. Pity counter'lar korunur. *(Cömert Zindan: kayıp cezasızdır — tekrar deneme ücretsiz.)*

- **If oyuncunun canavar envanteri doluysa ve canavar düşerse**: Canavar beklemeye alınır. Geri dönüşte "Envanter dolu — canavar beklemede! Yer aç veya satarak boşalt." uyarısı. Canavar 7 gün beklemeye alınır, süre sonunda en düşük nadirlikte canavar otomatik satılır ve altına çevrilir.

- **If aynı katta birden fazla XP İksiri düşerse**: Mümkün değil — her katta tek rulo. Boss katında XP İksiri rulo oranı %40 olduğundan ve normal katla ayrı tablodan çalıştığından, çakışma oluşmaz.

- **If evrim malzemesi düşer ve oyuncu o elementte canavar sahip değilse**: Malzeme envantere eklenir, kullanım ileride mümkün. Malzeme asla kaybolmaz.

- **If first_clear flag'ı kayıp/bozulursa**: İlk temizleme elmas ödülü tekrar verilir — oyuncu lehine hata. Exploit önleme: sunucu tarafında doğrulama (ileride).

- **If idle mod 24 saat aşarsa**: Altın birikimi 1440 dk'da durur (Ekonomi GDD). İdle loot (canavar/malzeme) da aynı cap'e tabidir. 24 saatten fazla birikim yok.

- **If idle'da Common canavar düşerse ve envanter doluysa**: Canavar beklemeye alınır, aynı aktif kural.

- **If boss katında Cömert Zindan garantisi tetiklenirse**: Boss katında floor_gold zaten 3x olduğundan (Kat 5: 1.500 >> 150), garanti hiçbir zaman boss katında tetiklenmez.

- **If oyuncu 0 canavar sahipken zindan sonu raporunda canavar satmaya çalışırsa**: Engellenir — son canavar satılamaz (Ekonomi GDD edge case ile tutarlı).

- **If difficulty_multiplier ile floor_gold çok yüksekse ve idle tabanı aynıysa**: Idle oranı takım gücüne bağlıdır, floor_gold'dan türetilmez. Tutarsızlık oluşmaz.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetRarityWeight(rarity)` — nadirlik düşme çarpanları. `GetRegionMonsterPool(regionId)` — bölge canavar havuzu. | Olmadan canavar loot'u hesaplanamaz |
| **Ekonomi** | Sert | `GetFloorReward(floorNumber, difficulty)` — altın formülü. `GrantReward(rewards)` — kaynak ekleme. | Olmadan altın ödül miktarı tanımsız |
| **Savaş Sistemi** | Sert | `DistributeLoot(battleResult, floorNumber)` — savaş sonucu tetikler, loot dağıtımı başlatır | Olmadan savaş kazanma loot üretmez |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Toplama ve Evrim** | Sert | `OnMonsterDropped(monsterId)` — loot'tan canavar instance oluşturma | Loot, canavar kazanımının ana kaynağı |
| **Zindan Keşif** | Sert | `GetFloorLootTable(floorNumber, floorType, regionId)` — kat loot tablosu sorgulama | Zindan katlara loot atar bu sistem üzerinden |
| **Otofarm / Idle** | Sert | `GetIdleLootRate(teamPower, region)` — idle loot hesaplama | Idle birikim bu sistemin oranlarını kullanır |
| **Canavar Güçlendirme** | Yumuşak | Evrim taşı loot tanımı | Loot tablosunda element taşı kayıtlı |
| **Kaydetme/Yükleme** | Sert | `SaveLootState()` / `LoadLootState()` — pity counter + first_clear persist | Olmadan pity ve ilk temizleme kaybolur |
| **Zindan Sonu UI** | Yumuşak | `GetSessionLootReport()` — rapor verisi | Olmadan rapor gösterilemez |

### Çapraz bağımlılık notları

> **Bu GDD, tüm ödül oranları ve düşme tablolarının tek kaynağıdır (single source of truth).** Diğer GDD'lerdeki referans değerler bu dokümandaki değerlerle çelişirse, bu GDD geçerlidir.

- Canavar Veritabanı bu sistemi downstream olarak listeliyor ✅
- Ekonomi bu sistemi downstream olarak listeliyor ✅
- Savaş Sistemi bu sistemi downstream olarak listeliyor ✅
- Canavar Güçlendirme: "Loot / Ödül — Yumuşak — Evrim taşı loot tanımı" ✅. **Güncelleme gerekli**: Güçlendirme GDD'de evrim malzemesi oranı "%5-10" yazıyor → bu GDD'deki kesin değer **%8** ile güncellenmeli.
- Zindan Keşif GDD: ✅ Yazıldı — bu sistem loot tablosunu sağlar, Zindan Keşif kat bilgisini sağlar. İlk temizleme elması tutarlılığı doğrulandı: normal kat=5, boss=50, son boss=100 (toplam 190 elmas/bölge).
- Otofarm GDD: ✅ Yazıldı (Approved) — idle loot oranları bu sistemde tanımlı. **Güncelleme gerekli**: Otofarm GDD'deki aktif nadirlik referans tablosu hâlâ tutarsız (Legendary %0.3 → doğru: %0.6, Common %60.0 → doğru: %59.0). Otofarm GDD'nin bir sonraki revizyonunda bu GDD'deki Kural 5 değerleriyle güncellenmeli.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `first_floors_guaranteed_monster` | 3 | 1–5 | Çok yüksek → onboarding çok uzun garanti | Çok düşük → ilk kat "boş sandık" hissi |
| `base_monster_rate` | 0.15 | 0.08–0.30 | Her katta canavar → nadir canavar heyecanı kaybolur | Çok az canavar → koleksiyon ilerlemesi durur |
| `boss_monster_rate` | 0.35 | 0.20–0.60 | Boss katı = garanti canavar → boss farkı azalır | Boss katı yetersiz → boss ödülü hayal kırıklığı |
| `boss_self_drop_rare` | 0.05 | 0.02–0.15 | Boss kolay elde → değeri düşer | Neredeyse imkansız → frustrasyon |
| `boss_self_drop_epic` | 0.03 | 0.01–0.10 | — | — |
| `boss_self_drop_legendary` | 0.01 | 0.005–0.05 | — | — |
| `boss_gold_multiplier` | 3.0 | 2.0–5.0 | Boss çok cömert → normal katlar anlamsız | Boss farkı hissedilmez |
| `monster_pity_increment` | 0.03 | 0.01–0.05 | Şans çok hızlı artar → pity çok yumuşak | Artış yavaş → şanssız oyuncu cezalanır |
| `monster_pity_cap` | 0.45 | 0.30–0.60 | Tavan çok yüksek → neredeyse garanti | Tavan düşük → pity yetersiz |
| `monster_hard_pity` | 10 | 8–15 | Çok düşük → canavar çok kolay, soft pity anlamsız | Çok yüksek → şanssız oyuncu frustrasyon |
| `rare_pity_increment` | 0.02 | 0.01–0.04 | Rare çok sık düşer → nadir hissetmez | Rare ulaşılamaz → koleksiyon stagnates |
| `rare_pity_cap` | 20 | 10–30 | Çok düşük → Rare pity etkisiz | Çok yüksek → Epic/Legendary aşırı dilüe olur |
| `evolution_material_rate` | 0.08 | 0.04–0.15 | Malzeme çok kolay → evrim zindanı gereksiz | Malzeme çok zor → evrim duvarı |
| `evolution_dungeon_rate` | 0.90 | 0.70–1.00 | 1.00 = garanti, zindanın amacı azalır | Evrim zindanına rağmen malzeme düşmez |
| `xp_potion_rate` | 0.20 | 0.10–0.35 | İksir çok bol → altın enjeksiyonu anlamsız | İksir çok nadir → XP katkısı sıfır |
| `gem_rate_normal` | 0.02 | 0.01–0.05 | Elmas enflasyonu → premium değeri düşer | Elmas çok nadir → ilerlemede duvar |
| `gem_rate_boss` | 0.15 | 0.05–0.30 | Boss cömert → elmas değersiz | Boss elması düşmez → hayal kırıklığı |
| `comert_floor_ratio` | 0.80 | 0.60–0.95 | Çok yüksek → garanti neredeyse tam floor_gold | Çok düşük → Cömert Zindan koruması zayıf |
| `idle_loot_efficiency` | 0.25 | 0.10–0.40 | Idle çok güçlü → aktif oynama gereksiz | Idle anlamsız → "Senin Tempon" ihlali |
| `idle_pity_cap` | 0.30 | 0.15–0.40 | Çok yüksek → idle aktife yaklaşır | Çok düşük → idle pity etkisiz |
| `xp_potion_mini` | 25 XP | 10–50 | Mini çok değerli → boyut farkı anlamsız | Mini çöp → düşünce hayal kırıklığı |
| `xp_potion_giant` | 10.000 XP | 5.000–25.000 | Dev çok güçlü → bir iksirle 10 seviye | Dev yetersiz → heyecan yok |

**Etkileşim Uyarıları**:
- `base_monster_rate` × `monster_pity_increment` × `monster_pity_cap` birlikte ortalama canavar düşme sıklığını belirler. Üçünü aynı anda artırmak canavar enflasyonu yaratır.
- `xp_potion_rate` × `xp_potion_giant` değerleri birlikte XP iksiri kanalının güçlendirmeye etkisini belirler. İkisini artırmak altın enjeksiyon yolunu devalüe eder.
- `evolution_material_rate` (normal) × evrim zindanı giriş hakkı (günde 3) × `evolution_dungeon_rate` birlikte evrim malzemesi erişilebilirliğini belirler. Canavar Güçlendirme GDD'deki evrim gereksinimlerine (3 taş A→B, 5 taş B→C) göre dengelenmeli.
- `idle_loot_efficiency` × `base_monster_rate` birlikte idle canavar kazanım hızını belirler. Aktif 10 kat seansı ~1.9 canavar veriyorsa, idle 10 saatte ~4.5 canavar verir — aktif oyunun ~2.4x süresi için ~2.4x canavar, makul denge.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Canavar düşmesi (savaşta) | Canavar silueti + nadirlik renginde aura (Common=beyaz, Rare=mavi, Epic=mor, Legendary=altın) | MVP |
| Altın düşmesi (savaşta) | Altın sikke parçacıkları uçuşur, sayı popup'ı | MVP |
| Evrim malzemesi düşmesi | Element renginde taş ikonu + parlama | MVP |
| XP İksiri düşmesi | Yeşil iksir ikonu + boyuta göre parıltı (Dev = tam parlama) | MVP |
| Elmas düşmesi | Mavi kristal parçacıkları + flash | MVP |
| Zindan sonu rapor — altın sayacı | Animasyonlu sayılar yukarı kayar, "ka-ching" efekti | MVP |
| Zindan sonu rapor — canavar kartı açılışı | Kart döner, nadirlik arka plan rengi, Rare+ parlama | MVP |
| Zindan sonu rapor — ilk temizleme bonusu | "YENİ!" animasyonlu damgası + ekstra parlama | MVP |
| Boss canavar düşmesi | Ekran titremesi + altın patlaması + özel parlama animasyonu | MVP |

### Audio Gereksinimleri

| Olay | Ses | Öncelik |
|------|-----|---------|
| Altın düşmesi | Metalik "clink" (miktar arttıkça yoğunlaşır) | MVP |
| Canavar düşmesi (Common) | Kısa "pop" ses | MVP |
| Canavar düşmesi (Rare+) | Artan gerilim tonu + "shimmer" ses (nadirlik arttıkça daha epik) | MVP |
| Zindan sonu rapor açılışı | Tatmin edici "summary fanfare" (1-2 sn) | MVP |
| Rapor — canavar kart açılışı | Kart çevirme sesi + nadirlik bazlı jingle | MVP |
| XP İksiri düşmesi | Yumuşak "glug" ses | MVP |
| Elmas düşmesi | Kristal "ting" ses | MVP |
| Boss canavar düşmesi | Epik düşme fanfarı (3-4 sn) | MVP |

> **Asset Spec** — Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:loot-odul-sistemi` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- **Savaş içi loot göstergesi**: Düşen lootun ikonu savaş alanında belirir ve ekranın üstüne doğru kayarak birikir. Nadirlik renkli çerçeve. Metin yok — sadece ikon.
- **Zindan sonu rapor ekranı**: Tam ekran overlay. Artan heyecan sıralamasıyla gösterim:
  1. Toplam altın (animasyonlu sayaç, büyük font) — garanti, düşük gerilim
  2. Evrim malzemeleri (element ikonu + adet) — düşük-orta heyecan
  3. XP İksirleri (boyut ikonu + adet) — orta heyecan
  4. Elmas (elmas ikonu + adet, varsa vurgulanır) — yüksek heyecan
  5. Canavar kartları (Common önce → en nadir son, kart döndürme animasyonu) — doruk an
  6. İlk temizleme bonusları ("YENİ!" damgası) — sürpriz finali
  Düşmeyen kategoriler atlanır — boş kademe gösterilmez.
- **Rapor hızlandırma**: Dokunarak her adımı hızlandırma + "Hepsini Göster" butonu (tüm loot tek ekranda). İlk temizleme zindanlarında skip devre dışı.
- **Rapor alt bar**: "Devam" butonu (ikincil) + "Tekrar Oyna" butonu (birincil, enerji yeterliyse)
- **Pity göstergesi**: Oyuncuya gösterilmez — iç mekanik.
- **Nadirlik gösterimi**: Renk + sembol/desen diferansiyasyonu (renk körü erişilebilirlik). Her nadirlik seviyesinde farklı çerçeve deseni ve yıldız sayısı (1-5). Kart açılışında nadirlik ismi metin olarak gösterilir.
- **Envanter dolu bildirimi**: Zindan sonu raporunda "Envanter dolu!" uyarısı + "Yer Aç" butonu
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar

> **UX Flag — Loot / Ödül**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak zindan sonu rapor ekranı ve savaş içi loot göstergesi için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** oyuncu Kat 1'i tamamlıyor (difficulty_multiplier=1.0), **WHEN** Cömert Zindan garantisi kontrol edilirse, **THEN** min_gold = 100 × 0.80 = 80 altın. floor_gold=100 ≥ 80 → bonus eklenmez, 100 altın verilir.

2. **GIVEN** oyuncu Kat 5'i (boss katı) tamamlıyor (difficulty_multiplier=1.0), **WHEN** boss altını hesaplanırsa, **THEN** boss_gold = (100 × 5 × 1.0) × 3.0 = 1.500 altın.

3. **GIVEN** normal kat loot tablosu yükleniyor, **WHEN** canavar düşme temel oranı sorgulanırsa, **THEN** base_monster_rate = 0.15.

4. **GIVEN** boss katı loot tablosu yükleniyor, **WHEN** canavar düşme temel oranı sorgulanırsa, **THEN** base_monster_rate = 0.35.

5. **GIVEN** nadirlik ağırlıkları {Common:1.00, Uncommon:0.50, Rare:0.15, Epic:0.04, Legendary:0.01} ve total_weight=1.70 (base, rare pity=0), **WHEN** P(rarity) hesaplanırsa, **THEN** P(Common)=0.588±0.001, P(Uncommon)=0.294±0.001, P(Rare)=0.088±0.001, P(Epic)=0.024±0.001, P(Legendary)=0.006±0.001. total_weight rare pity aktifken dinamik olarak yeniden hesaplanır.

6. **GIVEN** oyuncu art arda 5 katta canavar almamış (pity_counter=5), **WHEN** 6. katta loot roll yapılırsa, **THEN** effective_rate = min(0.45, 0.15 + 5×0.03) = **%30**.

7. **GIVEN** pity_counter=10, **WHEN** effective_rate hesaplanırsa, **THEN** effective_rate = min(0.45, 0.15 + 10×0.03) = min(0.45, 0.45) = **%45**. VE pity_counter=15 → effective_rate = min(0.45, 0.60) = **%45** (cap uygulanır).

8. **GIVEN** oyuncu art arda 4 kez Common/Uncommon canavar almış (rare_pity_counter=4), **WHEN** nadirlik seçimi yapılırsa, **THEN** Rare ağırlığı = 0.15 + 4×0.02 = **0.23**; Epic (0.04) ve Legendary (0.01) değişmez.

9. **GIVEN** oyuncu normal kattayken, **WHEN** loot roll yapılırsa, **THEN** evrim malzemesi düşme base oranı = 0.08.

10. **GIVEN** oyuncu boss katını tamamlıyor, **WHEN** loot dağıtılırsa, **THEN** en az 1 evrim malzemesi garantili olarak verilir.

11. **GIVEN** XP iksiri düşer, **WHEN** boyut ağırlıkları sorgulanırsa, **THEN** ağırlıklar: Mini=1.00, Küçük=0.60, Orta=0.25, Büyük=0.08, Dev=0.02 (total=1.95).

12a. **GIVEN** normal kat loot tablosu, **WHEN** gem base oranı sorgulanırsa, **THEN** gem_rate = 0.02.

12b. **GIVEN** boss katı loot tablosu, **WHEN** gem base oranı sorgulanırsa, **THEN** gem_rate = 0.15.

13. **GIVEN** boss katı tamamlanıyor, **WHEN** boss canavar drop kontrolü yapılırsa, **THEN** oranlar: Rare boss=%5, Epic=%3, Legendary=%1. Bu rulo normal canavar rulosundan bağımsız çalışır.

14a. **GIVEN** idle mod aktif, **WHEN** idle altın hesaplanırsa, **THEN** idle_gold_per_minute = active_gold_per_minute × 0.50 (idle_efficiency).

14b. **GIVEN** idle katta canavar rulosu yapılır, **WHEN** base oran hesaplanırsa, **THEN** idle_monster_chance = min(0.30, 0.15 × 0.25 + idle_pity_bonus) = min(0.30, 0.0375 + pity).

14c. **GIVEN** idle modda XP iksiri veya gem rulosu, **WHEN** kontrol edilirse, **THEN** bu loot tipleri idle'da düşmez.

14d. **GIVEN** idle'da canavar düşer, **WHEN** nadirlik seçilirse, **THEN** idle nadirlik tablosu uygulanır: Common=0.70, Uncommon=0.24, Rare=0.05, Epic=0.008, Legendary=0.002 (toplam=1.000).

15. **GIVEN** oyuncu 7. katta zindandan çıkar (10 katlık zindan), **WHEN** çıkış onaylanırsa, **THEN** sadece 1-7. katların loot'u verilir VE pity counter'lar mevcut değerinde korunur.

16. **GIVEN** oyuncunun canavar envanteri dolu, **WHEN** yeni canavar düşerse, **THEN** canavar bekleme alanına eklenir ve bildirim gösterilir. Bekleme süresi nadirliğe göre değişir: Common/Uncommon=7 gün, Rare+=14 gün (bkz. Canavar Toplama GDD Kural 4). Süre dolduğunda bekleme alanındaki ilgili canavar otomatik satılır. Satış fiyatı Canavar Toplama GDD Formül 2 ile belirlenir.

17. **GIVEN** oyuncu bir katı tamamlıyor, **WHEN** random katman rolleri yapılırsa, **THEN** canavar, malzeme, XP iksiri, gem birbirinden bağımsız çalışır — aynı kattan birden fazla tür düşebilir.

18. **GIVEN** oyuncu 10 katlık zindanı tamamlıyor, **WHEN** zindan sonu raporu açılırsa, **THEN** loot artan heyecan sıralamasıyla gösterilir: altın → malzeme → iksir → elmas → canavarlar (Common→Legendary) → ilk temizleme bonusu. Düşmeyen kategoriler atlanır.

19. **GIVEN** LootSimulator.RunBatch(iterations=10000, floors=10, difficulty=1.0, pity=0, first_clear=false, seed=sabit), **WHEN** ortalama sonuçlar hesaplanırsa, **THEN** beklenen değerler ±%10 toleransta: ~8.500 altın, ~4.45 canavar, ~3.2 malzeme, ~2.4 iksir, ~3.3 gem.

20. **GIVEN** 10 ardışık normal katta canavar düşmemiş (hard_pity_counter=10), **WHEN** 11. normal katta loot roll yapılırsa, **THEN** Common canavar garanti düşer (bölge havuzundan rastgele).

21. **GIVEN** rare_pity_counter=20, **WHEN** Common canavar düşer, **THEN** rare_pity_counter 20'de kalır (clamp), artmaz. adjusted_rare_weight = 0.15 + 20×0.02 = 0.55.

22. **GIVEN** idle oturumda 5 ardışık kat canavar düşmemiş ve offline_duration ≥ 30 dk, **WHEN** idle pity hesaplanırsa, **THEN** idle_pity_bonus = min(0.30, 0 + 5×0.06) = 0.30 (tavana ulaşmış).

23. **GIVEN** offline_duration < 30 dk, **WHEN** idle pity birikimi kontrol edilirse, **THEN** idle_pity_bonus artmaz (min_offline_for_pity koruması).

24. **GIVEN** normal katta evrim malzemesi düşer ve bölge elementi "Ateş", **WHEN** element seçimi yapılırsa, **THEN** Ateş ağırlığı=%70, Su=%10, Toprak=%10, Hava=%10.

25. **GIVEN** normal katta elmas düşer, **WHEN** miktar belirlenir, **THEN** 1 ≤ gem_amount ≤ 3. Boss katında: 5 ≤ gem_amount ≤ 15.

26. **GIVEN** oyuncu bir katı ilk kez temizler, **WHEN** ilk temizleme elmas ödülü verilir, **THEN** gem_amount = 5 (MVP sabit). VE aynı katı ikinci kez temizlerse first_clear ödülü verilmez.

27. **GIVEN** oyuncu boss'u ilk kez yener, **WHEN** ilk yenilgi ödülü verilir, **THEN** gem_amount = boss_first_clear_gems (normal boss: 50, son boss: 100 — Zindan Keşif GDD Kural 8). VE aynı boss'u tekrar yenerse first_clear ödülü verilmez.

28. **GIVEN** pity_counter=7, rare_pity_counter=4, idle_pity_bonus=0.18, first_clear[5]=true, **WHEN** oyun kaydedilir ve yüklenir, **THEN** tüm değerler korunur: pity_counter=7, rare_pity_counter=4, idle_pity_bonus=0.18, first_clear[5]=true.

29. **GIVEN** oyuncu Kat 1'i tamamlıyor (ilk normal kat), **WHEN** canavar loot roll yapılırsa, **THEN** canavar %100 garanti düşer (Kural 3a). VE Kat 2 ve 3 için de aynı garanti geçerlidir. VE Kat 4'te canavar base oranı normal %15'e döner.

30. **GIVEN** pity_counter=7, hard_pity_counter=7, **WHEN** herhangi bir canavar düşerse (normal roll, hard pity veya garanti), **THEN** pity_counter=0 VE hard_pity_counter=0. Her iki soft ve hard counter sıfırlanır.

31. **GIVEN** rare_pity_counter=12, **WHEN** Rare veya üstü nadirlikte canavar düşerse, **THEN** rare_pity_counter=0. VE Common/Uncommon düşerse rare_pity_counter=13 olur.

32. **GIVEN** idle modda Common canavar düşer, **WHEN** rare_pity_counter kontrol edilirse, **THEN** rare_pity_counter artmaz (idle'da düşen canavarlar rare_pity'yi etkilemez).

33. **GIVEN** evrim zindanı katı tamamlanır, **WHEN** evrim malzemesi rulosu yapılırsa, **THEN** düşme oranı %90 VE element eşleşmesi seçilen element %100.

34. **GIVEN** 8 normal kat + 2 boss katı tamamlanmış, hiçbirinde canavar düşmemiş, **WHEN** hard_pity_counter kontrol edilirse, **THEN** hard_pity_counter = 8 (boss katları sayılmaz, sadece normal katlar).

35. **GIVEN** oyuncu idle modda idle_pity_bonus=0.24, **WHEN** aktif zindana girip canavar düşürürse, **THEN** aktif pity_counter sıfırlanır AMA idle_pity_bonus=0.24 olarak korunur (tam izolasyon).

## Open Questions

1. **Canavar envanter limiti**: MVP'de kaç canavar taşınabilir? Bu limit Loot düşme deneyimini doğrudan etkiler. → Canavar Toplama GDD'sinde tanımlanacak.

2. **Bölge bazlı canavar havuzu**: MVP 1 bölge, 15-20 canavar. Tüm canavarlar her katta mı düşebilir, yoksa kat aralıklarına göre filtre mi uygulanır? → Zindan Keşif GDD'sinde netleşecek.

3. **Evrim zindanı loot tablosu**: Evrim zindanında sadece element taşı mı düşer, yoksa normal loot da var mı? → Zindan Keşif GDD'sinde tanımlanacak.

4. **XP İksiri envanter limiti**: İksirler sınırsız mı depolanır yoksa envanter slotu mu gerektirir? → Canavar Toplama / Envanter GDD'sinde netleşecek.

5. **Idle canavar bildirimi**: Idle'da canavar düştüğünde push notification gönderilmeli mi? → Bildirim Sistemi GDD'sinde (Tier 2) tanımlanacak.

6. **İlk temizleme elmas miktarı ölçeklemesi**: MVP'de sabit 5 elmas/kat. Post-MVP'de kata göre ölçekleme (5-20 arası) eklenebilir — artış tipi (lineer/kademeli) balance testing ile belirlenecek.

7. **Late-game altın ölçekleme**: floor_gold lineer artarken level_up_cost üstel (^1.5) artıyor — Kat 40+ sonrası grind duvarı riski. difficulty_multiplier ölçekleme tablosu veya floor_gold üstel bileşeni gerekebilir. → Ekonomi GDD revizyonunda ele alınacak.

8. **Tekrar eden elmas kaynakları**: First_clear sonrası elmas geliri çok düşük (~3.3/10 kat). Günlük giriş ödülü, haftalık hedefler gibi ek kaynaklar tanımlanmalı. → Ekonomi GDD ve İlerleme Döngüleri GDD'sinde (Tier 2) tanımlanacak.

9. **Lv50 sonrası altın sink**: Canavar max seviyeye ulaştığında altın birikimi sink'siz kalır. Yıldız birleşme altın maliyeti, kozmetik satın alma gibi late-game sink'ler tanımlanmalı. → Canavar Güçlendirme GDD revizyonunda ele alınacak.

10. **XP İksiri envanter limiti ve progression bypass**: Dev iksirler (10.000 XP) altın sink'ini bypass edebilir. Envanter limiti ve/veya seviye-bazlı XP cap'i tanımlanmalı. → Canavar Toplama GDD'sinde netleşecek.

11. **İdle dönüş ekranı UX**: Oyuncunun oyuna döndüğünde idle birikimini nasıl göreceği (ekran tasarımı, bilgi hiyerarşisi) tanımlanmamış. → `/ux-design idle-return` ile tasarlanacak veya Otofarm GDD'ye delegasyon.
