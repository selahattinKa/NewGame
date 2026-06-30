# Keşif Alanı Sistemi

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Cömert Zindan, Güç Hisset, Senin Tempon, Hep Bir Şey Var

## Overview

**Keşif Alanı Sistemi**, oyunun ana içerik döngüsüdür. Her Keşif Alanı, belirli bir temaya ve monster kadrosuna sahip 20 aşamalı bir bölgedir. Oyuncu sırayla ilerler: bir aşamayı temizlemeden bir sonrakine geçemez. Her aşamada tam olarak **1 düşman monster** vardır — savaş sistemi çıktısı budur. Aşamaların dördü beş gruba bölünür: Normal (1-4, 6-9, 11-14, 16-19), Şampiyon (5 ve 15, tekrar oynanabilir), Mini Boss (10, tek seferlik ilk temizleme bonusu), Alan Patronu (20, tek seferlik ilk temizleme bonusu).

Her aşamanın bir **Savaş Gücü (SG)** değeri vardır. Oyuncunun aktif petinin SG'si aşama SG'siyle kıyaslanır; renk kodu (Yeşil/Sarı/Kırmızı/Kilitli) oyuncuya hazırlık durumunu gösterir. Kilitli aşamalar SG farkı %30'u aşarsa erişilemez. Gizli bir **Kauçuk Bant** mekanizması, SG farkına göre düşman statlarını ±%30 oranında ayarlar — oyuncu hiçbir zaman bunu görmez, ama savaş adil hissettirmeye devam eder.

MVP kapsamında 1 Keşif Alanı, 20 aşama, 4 aşama tipi, SG renk kodu, Kauçuk Bant ve aşamaya özgü loot tabloları yer alır. Birden fazla alan Tier 2'de eklenir.

## Player Fantasy

Oyuncu Keşif Alanı'nda **kaşif ve fatih** fantezisi yaşar. Orman, mağara, harabe — her alanda tema değişir, monster kadrosu değişir, görsel stil değişir. Yeni bir alana girmek "bu dünyada ne var?" merakını besler.

**İlerleme tatmini**: Aşama haritası görsel olarak ilerler — kilitli kapılar açılır, yollar aydınlanır. Şampiyon ve Mini Boss aşamalarında beklenmedik güçlü düşmanlar vardır; bunları yenmek bir dönüm noktasıdır. Alan Patronu (Aşama 20) bir bölümün finalidir — onu yenmek hem hikaye hem de mekanik bir zaferdir.

**Renk kodu psikolojisi**: Sarı aşama (dengeli savaş) oyuncuyu heyecanlandırır; Kırmızı aşama (alt sınırda) endişe yaratır. Bir aşama Kırmızı'dan Sarı'ya döndüğünde — peti güçlendirdi çünkü — oyuncu ilerlemesini somut görür.

**"Bir aşama daha"**: Her aşama 30 saniye (5-8 tur @ 2x). Otofarm ile oyuncu telefonu bırakır; geri döndüğünde loot birikmiş olur. Manuel Komutan moduyla bizzat savaşır, zamanlama bonusu alır. Her iki ritim de desteklenir — Senin Tempon.

**Negatif fantezi (kaçınılacak)**: Duvar hissi — oyuncu Kırmızı bir aşamada defalarca kayeder, çıkış göremez. Kauçuk Bant bu hissi hafifletir (düşman statları nerf'lenir). Monotonluk — her aşama aynı türde monster. Şampiyon ve Boss aşamaları ritmi kırar.

## Detailed Rules

### Core Rules

**Kural 1 — Alan Yapısı**

Her Keşif Alanı 20 aşamadan oluşur:

| Aşama | Tip | Tekrar Oynanabilir | İlk Temizleme Bonusu |
|-------|-----|-------------------|----------------------|
| 1-4 | Normal | Evet | Evet (yalnızca ilk kez) |
| 5 | Şampiyon | Evet | Evet (yalnızca ilk kez) |
| 6-9 | Normal | Evet | Evet (yalnızca ilk kez) |
| 10 | Mini Boss | Evet | Evet (yalnızca ilk kez) |
| 11-14 | Normal | Evet | Evet (yalnızca ilk kez) |
| 15 | Şampiyon | Evet | Evet (yalnızca ilk kez) |
| 16-19 | Normal | Evet | Evet (yalnızca ilk kez) |
| 20 | Alan Patronu | Evet | Evet (yalnızca ilk kez) |

- Her aşamada **tam olarak 1 düşman monster** bulunur.
- Her aşama tekrar oynanabilir. İlk temizleme bonusu (ekstra EXP + elmas) yalnızca bir kez verilir.
- Normal savaş loot'u (altın + olası monster/malzeme) her galibiyet tekrarlanır.

**Kural 2 — Sıralı Kilit (Aşama Açılımı)**

- Aşama N+1 yalnızca Aşama N temizlendikten (galip gelinip ödül alındıktan) sonra açılır.
- Kilitleme SG kodu (Kural 4) ile bağımsızdır — aşama sırasal açık olsa bile SG Kilitli ise girilemez.
- Aşama 1 başlangıçta açık gelir (alan kilidi yoktur).

**Kural 3 — Aşama Tipleri ve Özellikleri**

**Normal Aşama (1-4, 6-9, 11-14, 16-19)**:
- Standart bir monster, standart loot tablosu.
- Oyuncu otofarm veya komutan moduyla girebilir.
- Loot: Altın (garanti) + olası canavar / evrim malzemesi (Loot Sistemi Kural'larına göre).
- Enerji maliyeti (galibiyet): 1 enerji.

**Şampiyon Aşama (5 ve 15)**:
- Normal aşamadan daha yüksek SG'ye sahip güçlü bir monster.
- Loot tablosu: Altın ×1.5 + canavar düşme şansı ×1.3 + evrim malzemesi şansı ×1.3.
- Enerji maliyeti (galibiyet): 1 enerji.
- Özel özellik: Şampiyon aşama tekrar oynandığında her seferinde ilk temizleme dışı ödül verir (başka bir deyişle "farming hotspot" olabilir).

**Mini Boss Aşama (10)**:
- Alan temasına uygun orta güçlü boss monster.
- Düşmana Sersemletme bağışıklığı eklenir (Savaş Sistemi Kural 9 ile uyumlu).
- İlk temizleme bonusu: Altın ×3 + elmas 10 adet + garantili canavar (Mini Boss'a özgü, o alana özel — C/B tier).
- Tekrar loot tablosu: Altın ×2 + canavar düşme şansı ×1.5 + evrim malzemesi şansı ×1.5.
- Enerji maliyeti (galibiyet): 2 enerji.

**Alan Patronu (20)**:
- Alanın en güçlü monster'ı, alan SG'sinin 1.5×'i kadar SG.
- Düşmana Sersemletme bağışıklığı eklenir.
- DoT hasarı ×0.5 alır (patron direnci — DoT sistemi ile uyumlu).
- İlk temizleme bonusu: Altın ×5 + elmas 25 adet + garantili canavar (Patron'a özgü, o alana özel — B tier) + bir sonraki alanın kapısı açılır (varsa).
- Tekrar loot tablosu: Altın ×3 + canavar düşme şansı ×2 + evrim malzemesi şansı ×2.
- Enerji maliyeti (galibiyet): 3 enerji.

**Kural 4 — SG Renk Kodu Sistemi**

Her aşamanın bir `stage_SG` değeri vardır. Oyuncunun aktif petinin `player_SG` değeri ile kıyaslanır:

| Renk | Koşul | Anlam | Giriş |
|------|-------|-------|-------|
| 🟢 **Yeşil** | `player_SG ≥ stage_SG × 1.10` | Oyuncu aşamadan %10+ güçlü | Serbest |
| 🟡 **Sarı** | `stage_SG × 0.90 ≤ player_SG < stage_SG × 1.10` | ±%10 dengeli | Serbest |
| 🔴 **Kırmızı** | `stage_SG × 0.70 ≤ player_SG < stage_SG × 0.90` | Oyuncu %10-30 zayıf | Serbest (riskli) |
| 🔒 **Kilitli** | `player_SG < stage_SG × 0.70` | Oyuncu %30+ zayıf | Girilemez |

- Renk kodu yalnızca **aktif pet**'in SG'sine göre hesaplanır.
- Renk kodu dinamik: pet güçlenince otomatik güncellenir.
- Kilitli aşamaya dokunulunca: "Savaş Gücün çok düşük. Petini güçlendirmeden devam edemezsin." mesajı.
- Kilitli aşama, sırasal açılmış olsa bile girilemez (iki bağımsız kural birlikte çalışır).

**Kural 5 — Kauçuk Bant Mekanizması (Gizli)**

Oyuncu aşamaya girdiğinde, SG farkına göre düşman statları sessizce ayarlanır. Oyuncu bu mekanizmayı görmez — stat değerleri ekranda gerçek (ham) değerler değil, ayarlanmış statlar temel alınarak hasar hesaplandıktan sonra sayılar olarak görünür.

```
cp_ratio = player_SG / stage_SG
rubber_band_factor = clamp(cp_ratio, 0.70, 1.30)
adjusted_stat = floor(base_stat × rubber_band_factor)
```

`rubber_band_factor` tüm düşman statlarına uygulanır: HP, ATK, DEF, SPD.

Örnek davranış:

| Senaryo | player_SG / stage_SG | Kauçuk Bant | Sonuç |
|---------|----------------------|-------------|-------|
| Çok güçlü oyuncu | 2.00 → ratio=2.0 → clamp=1.30 | Düşman +%30 güçlü | Savaş hâlâ anlamlı |
| Dengeli | 1.00 | 1.00 | Ham stat |
| %15 zayıf (Kırmızı) | 0.85 | 0.85 | Düşman -%15 zayıf |
| %30 zayıf (Kırmızı alt sınır) | 0.70 | 0.70 | Düşman -%30 zayıf |
| %31 zayıf | 0.69 → Kilitli | — | Girilemez |

**Kural 6 — Enerji ve Yeniden Deneme**

- **Enerji yalnızca galibiyette harcanır.** Kaybetme ve çekilme enerji tüketmez (Savaş Sistemi ile uyumlu).
- Oyuncu kilitli olmayan bir aşamayı enerji bitene kadar deneyebilir.
- Enerji maliyeti: Normal/Şampiyon = 1 enerji, Mini Boss = 2 enerji, Alan Patronu = 3 enerji.

**Kural 7 — Alan Açılımı**

- MVP başlangıcında **1 alan** (Karanlık Orman) açık.
- Alan 20 (Patron) ilk kez temizlenince bir sonraki alan kapısı açılır.
- Tier 2'de 2. ve 3. alanlar eklenir. Her alanın kendi monster kadrosu ve tema rengi vardır.

---

### Alan Örneği (MVP): Karanlık Orman

| Aşama | Tip | Monster (Taslak) | Tahmini SG |
|-------|-----|-----------------|-----------|
| 1-4 | Normal | F tier Hava/Toprak | ~100-160 |
| 5 | Şampiyon | D tier Hava | ~200 |
| 6-9 | Normal | D tier Su/Hava | ~220-280 |
| 10 | Mini Boss | D tier Su | ~300 |
| 11-14 | Normal | C tier Toprak/Hava | ~320-400 |
| 15 | Şampiyon | C tier Toprak | ~420 |
| 16-19 | Normal | C/B tier Hava/Ateş | ~450-550 |
| 20 | Alan Patronu | B tier Ateş | ~700 |

> **Not**: SG değerleri taslak — Canavar Veritabanı ve Pet Evrim Sistemi formülleri kesinleşince güncellenecek.

## Formulas

### Formül 1: SG Renk Kodu Sınırları

```
player_SG / stage_SG → renk:
  ≥ 1.10  → Yeşil
  0.90 – 1.09  → Sarı
  0.70 – 0.89  → Kırmızı
  < 0.70  → Kilitli (girilemez)
```

### Formül 2: Kauçuk Bant Düşman Stat Ayarı

```
cp_ratio = player_SG / stage_SG
rubber_band_factor = clamp(cp_ratio, 0.70, 1.30)
adjusted_stat = floor(base_stat × rubber_band_factor)
```

Tüm statlar etkilenir: HP, ATK, DEF, SPD.

**Örnek**: Aşama monster'ı (ATK=50, DEF=35, HP=200, SPD=25), player_SG/stage_SG=0.80

→ rubber_band_factor = 0.80
→ adjusted_ATK=40, adjusted_DEF=28, adjusted_HP=160, adjusted_SPD=20

### Formül 3: Şampiyon Loot Çarpanı

```
gold_reward = floor(base_gold × 1.5)
monster_drop_rate = base_rate × 1.3
material_drop_rate = base_rate × 1.3
```

### Formül 4: Mini Boss İlk Temizleme Bonusu

```
first_clear_gold = floor(base_gold × 3)
first_clear_gems = 10
first_clear_monster = guaranteed (mini_boss_monster_pool)
```

### Formül 5: Alan Patronu İlk Temizleme Bonusu

```
first_clear_gold = floor(base_gold × 5)
first_clear_gems = 25
first_clear_monster = guaranteed (patron_monster_pool, B tier)
```

### Formül 6: Alan Patronu DoT Direnci

```
actual_dot_damage = floor(calculated_dot_damage × 0.5)
```

### Formül 7: Stage SG Tanımı

`stage_SG` değeri, o aşamadaki monster'ın Canavar Veritabanı'nda tanımlı CP değeridir. Formula:

```
stage_SG = monster.base_CP × level_factor × star_factor
```

Detaylar Canavar Veritabanı ve Pet Evrim Sistemi GDD'lerinde tanımlanır. Keşif Alanı sistemi bu değeri okur, hesaplamaz.

## Edge Cases

- **If oyuncu SG eşik sınırında (tam %30 zayıf)**: `player_SG = stage_SG × 0.70` → Kırmızı (girilebilir). `player_SG = stage_SG × 0.699` → Kilitli.

- **If oyuncu bir aşamayı Kırmızı'da temizleyip sonraki aşama Kilitli'ye düşerse**: Temizlenen aşama açık kalır, sonraki aşama SG gereksinimi karşılanmadan girilemez. Oyuncu peti güçlendirip geri dönmelidir.

- **If Mini Boss aşamasında enerji 2'nin altındaysa**: Giriş engellenir. "Yeterli enerjin yok" mesajı.

- **If Alan Patronu'nu tekrar oynarken enerji 3'ün altındaysa**: Giriş engellenir.

- **If ilk temizleme bonuslu canavar envanteri doluysa**: Canavar yine de garantilidir — envanter dolsa bile `inbox` sistemine düşer (Koleksiyon GDD'sinde tanımlanır). İlk temizleme bonusu kaybolmaz.

- **If oyuncu alan haritasındayken peti değiştirirse**: Renk kodu anında yeni peti yansıtacak şekilde güncellenir.

- **If Kauçuk Bant adjusted_SPD'yi 0'ın altına düşürürse**: `adjusted_SPD = max(1, floor(base_SPD × rubber_band_factor))` — SPD asla 0 olamaz.

- **If rubber_band_factor ile adjusted_HP = 0 olursa (çok düşük base HP ve düşük factor)**: `adjusted_HP = max(1, floor(base_HP × rubber_band_factor))` — HP asla 0 olamaz.

- **If oyuncu aşama haritasını açarken aktif pet yoksa**: Tüm aşamalar Kilitli görünür, "Aktif pet seç" yönlendirmesi.

- **If oyuncu sırasal kilitli bir aşamaya erişmeye çalışırsa (N+1 açılmamış)**: Kilit ikonu + "Aşama [N]'i temizle" mesajı, SG renk kodu gösterilmez.

- **If aynı aşama aynı seansta arka arkaya farklı pet'lerle girilirse**: Her girişte Kauçuk Bant o anki aktif pet'in SG'sine göre hesaplanır.

## Dependencies

### Upstream

| Sistem | Tip | Veri | Arayüz |
|--------|-----|------|--------|
| **Pet Sistemi** | Sert | Aktif pet + SG değeri | `GetActivePet()` → {petId, SG} |
| **Canavar Veritabanı** | Sert | Aşama monster tanımı (SG, element, arketip) | `GetMonsterByStage(areaId, stageNum)` → MonsterDef |
| **Savaş Sistemi** | Sert | Savaş yürütme, sonuç | `StartBattle(petId, enemyId)` → BattleResult |
| **Loot / Ödül Sistemi** | Sert | Loot dağıtımı | `DistributeLoot(stageId, stageType, isFirstClear)` |
| **Ekonomi** | Sert | Enerji okuma/harcama | `GetCurrentEnergy()`, `SpendEnergy(amount)` |
| **Kaydetme / Yükleme** | Sert | İlk temizleme durumu, açık aşamalar | `IsFirstClear(stageId)`, `MarkFirstClear(stageId)`, `GetUnlockedStages(areaId)` |

### Downstream

| Sistem | Tip | Veri | Arayüz |
|--------|-----|------|--------|
| **Savaş Sistemi** | Sert | Düşman tanımı (Kauçuk Bant uygulanmış statlar) | `GetStageEnemy(stageId)` → {enemyId, adjusted_stats} |
| **Loot / Ödül Sistemi** | Sert | Savaş sonucu → loot | `OnBattleComplete(result)` → loot trigger |
| **Zindan Harita UI** | Sert | Aşama durumu, renk kodu, ilerleme | `GetStageStatus(areaId, stageNum)` → {locked, colorCode, isFirstClear} |
| **Zindan Keşif** | Dolaylı | Özel etkinlik olarak bağımsız çalışır | Keşif Alanı'ndan bağımsız — farklı içerik kuyruğu |
| **Otofarm / Idle** | Sert | Otofarm hangi aşamayı tekrar oynadığını bildirir | `GetFarmableStages(areaId)` → [stageId] |
| **Aranıyor Tahtası** | Dolaylı | Hangi monster'lar hangi aşamada görünür | `GetStageMonsterPool(areaId)` |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `green_threshold` | 1.10 | 1.05–1.25 | Oyuncu çok erken yeşile geçer | Hiç yeşil göremez |
| `yellow_lower_bound` | 0.90 | 0.80–0.95 | Sarı bölge dar → Yellow anında kırmızıya | Sarı bölge çok geniş |
| `red_lower_bound` | 0.70 | 0.60–0.80 | Zor içerik çok erken erişilir | Oyuncu sürekli duvarla karşılaşır |
| `rubber_band_max` | 1.30 | 1.10–1.50 | Overpowered oyuncu çok zorluk çeker | Farklılık hissedilmez |
| `rubber_band_min` | 0.70 | 0.50–0.85 | Kırmızı'da savaş çok kolay | Kırmızı'da kaybetmek kaçınılmaz |
| `champion_gold_mult` | 1.5 | 1.2–2.0 | Şampiyon farming ezici | Şampiyon vs Normal farkı yok |
| `champion_drop_mult` | 1.3 | 1.1–1.8 | Şampiyon çok cazip → Normal anlamsız | Fark edilmez |
| `miniboss_energy_cost` | 2 | 1–3 | Farming frenlenir | Mini Boss farming çok kolay |
| `patron_energy_cost` | 3 | 2–5 | Patron farming nadirleşir | Patron çok hızlı farming |
| `patron_dot_resist` | 0.5 | 0.3–0.7 | DoT DoT'u öldürür | Patron çok kolay (Hırsız tek başına yeter) |
| `miniboss_first_gems` | 10 | 5–20 | Elmas enflasyonu | Anlamsız |
| `patron_first_gems` | 25 | 15–50 | Elmas enflasyonu | Patron galibiyeti tatminsiz |

## Acceptance Criteria

1. **GIVEN** Aşama 1 açık, oyuncu Aşama 2'ye girmek isterse, **WHEN** Aşama 1 temizlenmemişse, **THEN** Aşama 2 kilitli görünür, girilemez.

2. **GIVEN** Aşama 1 temizlendi, **WHEN** savaş sonucu `Victory` olursa, **THEN** Aşama 2 açılır, kayıt kalıcılaştırılır.

3. **GIVEN** `player_SG = 110`, `stage_SG = 100`, **WHEN** renk kodu hesaplanırsa, **THEN** Yeşil (110/100 = 1.10 ≥ 1.10).

4. **GIVEN** `player_SG = 95`, `stage_SG = 100`, **WHEN** renk kodu hesaplanırsa, **THEN** Sarı (0.95 → 0.90–1.10 aralığı).

5. **GIVEN** `player_SG = 75`, `stage_SG = 100`, **WHEN** renk kodu hesaplanırsa, **THEN** Kırmızı (0.75 → 0.70–0.89).

6. **GIVEN** `player_SG = 65`, `stage_SG = 100`, **WHEN** renk kodu hesaplanırsa, **THEN** Kilitli (0.65 < 0.70), giriş engellenir.

7. **GIVEN** `player_SG = 80`, `stage_SG = 100` (Kırmızı), **WHEN** savaş başlarsa, **THEN** düşman statları `base × 0.80` olarak ayarlanır (Kauçuk Bant).

8. **GIVEN** `player_SG = 200`, `stage_SG = 100` (Yeşil, ratio=2.0), **WHEN** savaş başlarsa, **THEN** düşman statları `base × 1.30` olarak ayarlanır (cap'e takılır).

9. **GIVEN** `player_SG = 100`, `stage_SG = 100` (Sarı, ratio=1.0), **WHEN** savaş başlarsa, **THEN** düşman statları değişmez (factor=1.00).

10. **GIVEN** oyuncu Aşama 5 (Şampiyon) kazanır, **WHEN** loot hesaplanırsa, **THEN** altın `base_gold × 1.5`, canavar drop şansı `base_rate × 1.3`.

11. **GIVEN** Aşama 10 (Mini Boss) ilk kez temizlendi, **WHEN** ödül verilirse, **THEN** `first_clear_gold × 3` + 10 elmas + garantili canavar. `IsFirstClear(stage10)` → `true`.

12. **GIVEN** Aşama 10 ikinci kez temizlendi, **WHEN** ödül verilirse, **THEN** ilk temizleme bonusu yok. Tekrar loot: `base_gold × 2`, canavar şansı ×1.5.

13. **GIVEN** Aşama 20 (Alan Patronu) ilk kez temizlendi, **WHEN** ödül verilirse, **THEN** `base_gold × 5` + 25 elmas + B tier garantili canavar + Tier 2 alan kapısı açılır.

14. **GIVEN** Mini Boss aşamasına giriş, enerji < 2, **WHEN** oyuncu girmeye çalışırsa, **THEN** giriş engellenir, "Yeterli enerjin yok" mesajı.

15. **GIVEN** oyuncu Aşama 10'a girer (Mini Boss), **WHEN** Sersemletme yeteneği kullanılırsa, **THEN** Sersemletme uygulanmaz (boss bağışıklığı, Savaş Sistemi Kural 9).

16. **GIVEN** Alan Patronu'na Yanma DoT uygulandı (base 5 hasar/tur), **WHEN** DoTPhase çalışırsa, **THEN** gerçek hasar = `floor(5 × 0.5) = 2` hasar/tur.

17. **GIVEN** oyuncu Aşama N'de kayeder, **WHEN** savaş biter, **THEN** enerji harcanmaz, loot yok, aşama kilidi değişmez (kazanılmamış).

18. **GIVEN** savaş sırasında aktif pet değiştirilirse (eğer sistem izin veriyorsa), **WHEN** savaş başladıktan sonra pet değişimi denemesi, **THEN** savaş sırasında pet değişimi engellenir — Kauçuk Bant savaş başında kilitlenir.

19. **GIVEN** oyuncu aşama haritasındayken peti değiştirirse, **WHEN** harita yeniden render'lanırsa, **THEN** tüm renk kodları yeni pet'in SG'sine göre güncellenir.

20. **GIVEN** Kauçuk Bant `rubber_band_factor = 0.70` ve düşman `base_SPD = 1`, **WHEN** adjusted_SPD hesaplanırsa, **THEN** adjusted_SPD = max(1, floor(1 × 0.70)) = max(1, 0) = **1** (minimum koruma).
