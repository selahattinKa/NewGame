# Keşif Alanı Sistemi

> **Status**: Revised (2026-07-02 — gerçek implementasyona göre yeniden yazıldı)
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-07-02
> **Implements Pillar**: Cömert Zindan, Güç Hisset, Senin Tempon, Hep Bir Şey Var

> **Revizyon notu (2026-07-02)**: Bu dosya, `Assets/Scripts/Core/DungeonManager.cs`'in gerçek, çalışan implementasyonuna göre baştan yazıldı. Önceki sürümdeki **SG renk kodu (🟢🟡🔴🔒), Kauçuk Bant mekanizması ve Sweep (süpürme)** kodda hiç karşılığı olmadığı için kaldırıldı — bunlar hiç implemente edilmemiş tasarım fikirleriydi. Ayrıca önceki sürüm "her aşamada tam 1 canavar" diyordu; gerçekte her aşama 2-3 **dalga** halinde düşman grubu içeriyor (Zindan Keşif'in eski dalga sistemi burada birleşti — bkz. `zindan-saldirisi.md`'nin ayrılma notu). Eskiden ayrı iki sistem (Keşif Alanı + Zindan Keşif) olarak tasarlanan şey, kodda tek bir sistemde birleşmiş halde çalışıyor; bu dosya artık o tek gerçek sistemi belgeliyor.

## Overview

**Keşif Alanı Sistemi**, oyunun ana içerik döngüsüdür ve `DungeonManager` tarafından yönetilir. Bir Keşif Alanı **20 aşamadan** oluşur; oyuncu lineer ilerler (Aşama N'i temizlemeden N+1'e giremez). Her aşama **2 veya 3 dalga** halinde düşman grubu içerir — dalgalar arası oyuncunun HP'si ve enerjisi korunur, aşama tamamlanınca tam iyileşme uygulanır. Her 5. aşama (5, 10, 15, 20) 3 dalgalıdır ve son dalgası güçlendirilmiş bir düşman (Şampiyon/Boss/Ana Boss) içerir.

Aşama girişi sabit **2 enerji** maliyetlidir (aşama tipinden bağımsız — tüm aşamalar aynı maliyet). Düşman gücü aşama numarasına göre lineer formülle ölçeklenir; oyuncunun kendi gücüyle karşılaştıran bir "renk kodu" veya erişim kilidi yoktur — tek kilit mekanizması sıralı ilerlemedir (önceki aşama temizlenmeli).

MVP kapsamında 1 Keşif Alanı, 20 aşama, 4 aşama tipi (Normal/Şampiyon/Mini Boss/Alan Patronu), dalga sistemi ve aşama tipine göre ölçeklenen ödül tabloları (altın, ilk temizleme elması, canavar yakalama şansı, ekipman düşme şansı) yer alır.

## Player Fantasy

Oyuncu Keşif Alanı'nda **kaşif ve fatih** fantezisi yaşar. Her aşama bir meydan okumadır: düşman dalgalarını yen, ödülü topla, bir sonraki aşamaya ilerle. Şampiyon (5, 15) ve Boss (10, 20) aşamalarında beklenmedik güçlü düşmanlar vardır; bunları yenmek bir dönüm noktasıdır. Aşama 20'yi (Alan Patronu) yenmek bir bölümün finalidir.

**"Bir aşama daha"**: Her aşama birkaç dakika sürer (2-3 dalga @ otofarm veya komutan modu). Otofarm ile oyuncu telefonu bırakır; geri döndüğünde loot birikmiş olur. Manuel Komutan moduyla bizzat savaşır. Her iki ritim de desteklenir — Senin Tempon.

**İlerleme tatmini**: Daha önce zor geçilen bir aşama, pet/ekipman güçlendikçe kolaylaşır. Oyuncu bu farkı somut hisseder — otomatik saldırının hızı, alınan hasarın azlığı.

**Negatif fantezi (kaçınılacak)**: Duvar hissi — oyuncu bir aşamada defalarca kaybeder, çıkış göremez. Düşman gücü lineer arttığından ve oyuncu gücü (pet + ekipman) buna paralel büyüdüğünden bu risk düşük tutulmalı — dengeleme oturumlarında düşman formülü ile oyuncu güç eğrisi karşılaştırılmalı (bkz. Open Questions).

**Pillar bağlantısı**: "Cömert Zindan" — kaybetmek enerji harcamaz, cezasızdır. "Güç Hisset" — güçlenince aşamalar hızla düşer. "Senin Tempon" — otofarm/komutan ikisi de desteklenir. "Hep Bir Şey Var" — 20 aşama boyunca sürekli yeni bir aşama tipi/ödül var.

## Detailed Rules

### Core Rules

**Kural 1 — Aşama Yapısı**

Bir Keşif Alanı 20 aşamadan oluşur (`DungeonManager.TotalFloors = 20`):

| Aşama | Tip (`GetFloorType`) | Dalga Sayısı |
|-------|----------------------|---------------|
| 1-4, 6-9, 11-14, 16-19 | Normal | 2 |
| 5, 15 | Şampiyon | 3 |
| 10 | Mini Boss | 3 |
| 20 | Alan Patronu | 3 |

- Her 5. aşama (`floor % 5 == 0`) 3 dalgalıdır; diğerleri 2 dalgalıdır.
- Aşama tipi belirleme sırası: `floor == 20` → Alan Patronu; `floor % 10 == 0` → Mini Boss; `floor % 5 == 0` → Şampiyon; aksi halde Normal.
- Her aşama tekrar oynanabilir (yeniden giriş 2 enerji harcar). İlk temizleme ödülü yalnızca bir kez verilir.

**Kural 2 — Sıralı Kilit (Aşama Açılımı)**

- Aşama 1 başlangıçta **Unlocked** (açık) gelir, Aşama 2-20 **Locked** (kilitli) başlar.
- Aşama N temizlenince (`FloorState.Cleared`) Aşama N+1 otomatik **Unlocked** olur.
- Oyuncunun gücüyle karşılaştıran ayrı bir kilit mekanizması **yoktur** — tek kilit koşulu önceki aşamanın temizlenmiş olmasıdır.

**Kural 3 — Dalga Sistemi**

Bir aşamaya girildiğinde (`EnterFloor`):
1. `CurrentWave = 1`, `TotalWaves = 2` (Normal) veya `3` (Şampiyon/Boss/Alan Patronu)
2. Oyuncu birimi bir kez inşa edilir (`BuildPlayer`) ve **tüm dalgalar boyunca aynı instance** kullanılır — HP ve durum etkileri dalgalar arası **korunur**, sıfırlanmaz.
3. Her dalga kazanılınca (`HandleBattleEnded`, `IsVictory`): `CurrentWave < TotalWaves` ise 1.5 saniyelik geçiş beklemesinden (`WaveTransitionDelay`) sonra bir sonraki dalga başlar; son dalgaysa aşama temizlenir.
4. Herhangi bir dalgada kaybedilirse (`IsVictory == false`) aşama **başarısız** sayılır — kalan dalgalar oynanmaz, enerji harcanmaz, ödül verilmez.
5. Son dalga (`wave == TotalWaves`), aşama bir 5'in katıysa **güçlendirilmiş düşman** içerir (bkz. Formül 2 — HP ×2.5, seviye +%50).

**Kural 4 — Enerji**

| Sabit | Değer |
|-------|-------|
| `EnergyMax` | 100 |
| `EnergyPerFloor` | 2 (tüm aşama tiplerinde sabit — Normal/Şampiyon/Boss/Ana Boss farksız) |

- Enerji yalnızca aşama **temizlendiğinde** harcanır (`HandleFloorCleared`). Kaybetme veya çekilme enerji harcamaz.
- 100 enerji = 50 aşama giriş kapasitesi.
- Enerji < 2 ise `CanEnterFloor` false döner, giriş engellenir.

**Kural 5 — Aşama Ödülleri**

Aşama temizlendiğinde (`HandleFloorCleared`) verilenler:

| Ödül | Koşul | Kaynak |
|------|-------|--------|
| Altın | Her temizlemede | `EconomyManager.FloorGoldReward(floor)` — Formül 1 |
| Elmas | Yalnızca ilk temizleme | `GetFirstClearGems(floor)` — Formül 2 |
| Canavar yakalama | Şansa bağlı, her temizlemede | `MonsterCollection.TryCapture(floor)` — Formül 3 |
| Ekipman düşme | Şansa bağlı, her temizlemede | `TryDropEquipment(floor)` — Formül 4 |
| Tam iyileşme | Her temizlemede | Oyuncu `CurrentHP = MaxHP` olur |

Not: Bu implementasyonda ayrı bir "evrim malzemesi" düşme mekaniği **yoktur** — `loot-odul-sistemi.md`'nin Evrim Taşı/malzeme sistemi henüz bu akışa entegre edilmedi (bkz. Open Questions).

**Kural 6 — Oyuncu Gücü İnşası**

Aşamaya girerken oyuncu birimi şu şekilde kurulur (`BuildPlayer`):

1. Taban stat: `PlayerClass.StatAtLevel(base, level=1)` — **oyuncunun kendi seviyesi henüz bu formüle dahil değil**, her zaman Lv1 taban kullanılıyor (bkz. Open Questions — `level-deneyim-sistemi.md` entegrasyonu bekliyor).
2. Seçili pet varsa: `MonsterCollection.BonusForTier(pet.Tier)` ile HP ve ATK'ya flat çarpan uygulanır (bkz. `oyuncu-sinif-sistemi.md` Kural 6 — GDD-kod uyumsuzluğu notu, [[project_gdd_code_drift]]).
3. Ekipman bonusu: `EquipmentManager.GetTotalBonuses()` ile ATK/HP/DEF/SPD'ye flat toplama uygulanır.

**Kural 7 — Düşman Gücü İnşası**

Düşman birimi (`BuildEnemy`) iki kaynaktan biriyle kurulur:

- **Canavar havuzu varsa** (`Resources/Monsters` doluysa): Aşama numarasına orantılı bir canavar seçilir (havuzdaki indeks = `floor(( floor-1) / TotalFloors × pool.Length)`), boss dalgasında havuzun son (en güçlü) canavarı zorlanır. Stat: `StatAtLevel(level=floor)`, boss dalgasında HP ×2.5 ve level +`floor(floor×0.5)`.
- **Havuz boşsa** (formül fallback): Formül 5'teki lineer formül kullanılır.

## Formulas

### Formül 1: Aşama Altın Ödülü

`gold_reward = floor(100 × floor × mult)`

| Aşama Tipi | mult |
|------------|------|
| Alan Patronu | 5.0 |
| Mini Boss | 2.5 |
| Şampiyon | 1.5 |
| Normal | 1.0 |

**Örnek**: Aşama 10 (Mini Boss) → `floor(100 × 10 × 2.5) = 2.500` altın. Aşama 4 (Normal) → `floor(100 × 4 × 1.0) = 400` altın.

### Formül 2: İlk Temizleme Elması

Sabit tablo (`GetFirstClearGems`), aşama tipine göre:

| Aşama Tipi | Elmas |
|------------|-------|
| Alan Patronu | 200 |
| Mini Boss | 100 |
| Şampiyon | 50 |
| Normal | 5 |

### Formül 3: Canavar Yakalama Şansı ve Tier'i

`capture_roll < CaptureChance(floor)` ise yakalama gerçekleşir:

| Aşama Tipi | Yakalama Şansı | Yakalanan Tier (`TierForFloor`) |
|------------|-----------------|-----------------------------------|
| Alan Patronu | %2 | B |
| Mini Boss | %4 | C |
| Şampiyon | %8 | D |
| Normal | %15 | F |

Yakalanan canavarın `MaxEvolutionTier`'i ayrıca olasılıksal olarak belirlenir (`RollMaxTier` — `canavar-veritabani.md`/`canavar-toplama-evrim.md` kapsamında, bkz. [[project_gdd_code_drift]] madde 5).

### Formül 4: Ekipman Düşme Şansı

`equipment_chance = CaptureChance(floor) × 2`

Tier, Formül 3'teki `TierForFloor` ile aynıdır. Düşen slot (zırh/aksesuar) %50/%50 rastgele seçilir.

**Örnek**: Aşama 20 (Alan Patronu) → yakalama %2, ekipman düşme %4, tier B.

### Formül 5: Düşman Stat Formülü (Canavar Havuzu Boşken)

```
mult = isBossWave ? 2.5 : 1.0
HP   = floor((20 + floor × 8)  × mult)
ATK  = floor((10 + floor × 3)  × mult)
DEF  = floor((8  + floor × 2)  × mult)
SPD  = 20 (sabit)
```

**Örnek**: Aşama 10, normal dalga (mult=1.0) → HP=100, ATK=40, DEF=28, SPD=20. Aşama 10, boss dalgası (mult=2.5) → HP=250, ATK=100, DEF=70, SPD=20.

## Edge Cases

- **If oyuncu bir dalgada tüm canavarlarını/kendini kaybederse**: Aşama başarısız sayılır (`FloorFailed`). Enerji harcanmaz, ödül verilmez, önceki dalgaların sonucu geçersizdir. Oyuncu "Tekrar Dene" ile Dalga 1'den başlar.

- **If oyuncu aşamayı temizledikten sonra tekrar oynarsa**: `FirstCleared` zaten `true` olduğundan yalnızca normal ödül (altın + şans bazlı yakalama/ekipman) verilir, elmas verilmez.

- **If oyuncunun enerjisi 2'nin altındaysa**: `CanEnterFloor` false döner, giriş engellenir.

- **If oyuncu Aşama 20'yi (son aşama) temizlerse**: Bir sonraki aşama yoktur (`CurrentFloor < TotalFloors` koşulu false) — aşama açma adımı atlanır. Oyuncu mevcut 20 aşamayı tekrar oynayabilir.

- **If dalga geçişi sırasında (1.5s bekleme) uygulama arka plana atılırsa**: State `WaveTransition`'da kalır; uygulama öne dönünce coroutine kaldığı yerden devam eder (Unity coroutine davranışı — mobil arka plan kısıtlamaları test edilmeli, bkz. Open Questions).

- **If canavar/ekipman düşer ve envanter/ekipman deposu doluysa**: `canavar-toplama-evrim.md` Kural 4 (bekleme alanı) devreye girer.

- **If aktif pet yoksa**: `BuildPlayer`'da pet bonusu adımı atlanır (`if (pet != null)`), oyuncu yalnızca sınıf + ekipman statlarıyla savaşır.

## Dependencies

### Upstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Oyuncu Sınıf Sistemi** | Sert | `PlayerClass.StatAtLevel(base, 1)` — taban stat | Olmadan oyuncu birimi kurulamaz |
| **Pet Sistemi** | Sert | `MonsterCollection.SelectedPet`, `BonusForTier(tier)` | Olmadan pet bonusu uygulanmaz (pet yoksa atlanır, hata değil) |
| **Ekipman Sistemi** | Sert | `EquipmentManager.GetTotalBonuses()` | Olmadan ekipman bonusu uygulanmaz |
| **Canavar Veritabanı / Havuzu** | Yumuşak | `Resources.LoadAll<MonsterData>("Monsters")` | Havuz boşsa Formül 5 fallback'i devreye girer |
| **Ekonomi** | Sert | `EconomyManager.FloorGoldReward()`, `AddGold()`, `AddDiamonds()` | Olmadan ödül verilemez |
| **Canavar Toplama ve Evrim** | Sert | `MonsterCollection.TryCapture(floor)`, `CaptureChance()`, `TierForFloor()` | Olmadan canavar yakalama çalışmaz |
| **Ekipman Sistemi** | Sert | `EquipmentManager.CreateEquipment()`, `RandomArmorSlot()/RandomAccessorySlot()` | Olmadan ekipman düşmesi çalışmaz |
| **Savaş Sistemi** | Sert | `CombatManager.StartCombat()`, `OnBattleEnded` | Olmadan dalga savaşı yürütülemez |
| **Kaydetme/Yükleme** | Sert | `PlayerPrefs` tabanlı `SaveProgress()`/`LoadProgress()` | Olmadan ilerleme/enerji kaybolur |

### Downstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Keşif Alanı Harita UI** | Sert | `Floors[]`, `State`, `CurrentFloor/Wave/TotalWaves` | UI verileri bu sistemden gelir |
| **Otofarm / Idle** | Yumuşak | En yüksek temizlenmiş aşama (idle hesaplama referansı) | Henüz otofarm entegrasyonu kod tarafında doğrulanmadı — bkz. Open Questions |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|-----------------|---------------|--------------|
| `TotalFloors` | 20 | 10–40 | İçerik yetiştirmek zorlaşır | Bölüm çok kısa biter |
| `EnergyPerFloor` | 2 | 1–5 | Oturum kısalır | Enerji anlamsızlaşır |
| `EnergyMax` | 100 | 50–200 | Enerji hiç bitmez | Sık sık enerji bekleme |
| Aşama gold `mult` (Normal/Şampiyon/Boss/AnaBoss) | 1.0/1.5/2.5/5.0 | — | Boss farming'i ezici kılar | Boss ödülü sıradan hissettirir |
| İlk temizleme elması (Normal/Şampiyon/Boss/AnaBoss) | 5/50/100/200 | — | Elmas enflasyonu | İlk temizleme heyecansız |
| Yakalama şansı (Normal/Şampiyon/Boss/AnaBoss) | %15/%8/%4/%2 | — | Yüksek tier çok kolay yakalanır | Koleksiyon ilerlemesi çok yavaş |
| Düşman stat formülü çarpanları (8/3/2 katsayıları) | — | — | Düşmanlar çok hızlı güçlenir → duvar | Düşmanlar hep kolay kalır |
| `isBossWave` HP çarpanı | 2.5 | 1.5–4.0 | Boss dalgası aşırı zor | Boss farksız hissettirir |

## Acceptance Criteria

1. **GIVEN** Aşama 1 temizlenmemiş, **WHEN** oyuncu Aşama 2'ye girmeye çalışırsa, **THEN** `CanEnterFloor(2)` false döner.

2. **GIVEN** Aşama 1 temizlendi, **WHEN** `HandleFloorCleared` çalışırsa, **THEN** Aşama 2 `Unlocked` olur ve kalıcılaştırılır (`SaveProgress`).

3. **GIVEN** Aşama 4 (Normal, mult=1.0), **WHEN** temizlenirse, **THEN** `gold_reward = floor(100×4×1.0) = 400`.

4. **GIVEN** Aşama 10 (Mini Boss, mult=2.5), **WHEN** temizlenirse, **THEN** `gold_reward = floor(100×10×2.5) = 2.500`.

5. **GIVEN** Aşama 5 (Şampiyon) ilk kez temizlendi, **WHEN** ödül verilirse, **THEN** 50 elmas verilir; ikinci temizlemede elmas verilmez.

6. **GIVEN** Aşama 20 (Alan Patronu) ilk kez temizlendi, **WHEN** ödül verilirse, **THEN** 200 elmas verilir.

7. **GIVEN** oyuncu normal aşamada (2 dalgalı), **WHEN** aşamaya girerse, **THEN** `TotalWaves = 2`.

8. **GIVEN** oyuncu Aşama 10'a (5'in katı) girerse, **WHEN** aşama başlarsa, **THEN** `TotalWaves = 3` ve son dalga `isBossWave = true` olarak işaretlenir.

9. **GIVEN** oyuncu Dalga 1'i kazanmış, **WHEN** Dalga 2 başlarsa, **THEN** oyuncunun HP'si Dalga 1 sonundaki değerle aynıdır (sıfırlanmaz).

10. **GIVEN** oyuncu herhangi bir dalgada kaybederse, **WHEN** sonuç işlenirse, **THEN** enerji harcanmaz, `State = FloorFailed`.

11. **GIVEN** oyuncunun enerjisi 1, **WHEN** herhangi bir aşamaya girmeye çalışırsa, **THEN** `CanEnterFloor` false döner.

12. **GIVEN** aktif pet Tier=C, **WHEN** `BuildPlayer` çalışırsa, **THEN** HP ve ATK `BonusForTier(C) = ×1.20` ile çarpılır.

13. **GIVEN** aktif pet yok, **WHEN** `BuildPlayer` çalışırsa, **THEN** pet bonusu adımı atlanır, yalnızca sınıf + ekipman statları kullanılır.

14. **GIVEN** Aşama 20 temizlendi, **WHEN** oyuncu tekrar "Devam" seçerse, **THEN** yeni aşama açılmaz (`CurrentFloor < TotalFloors` false), oyuncu haritaya döner.

15. **GIVEN** canavar havuzu (`Resources/Monsters`) boş, **WHEN** düşman inşa edilirse, **THEN** Formül 5 fallback formülü kullanılır.

## Open Questions

1. **Oyuncu seviyesi entegrasyonu**: `BuildPlayer` hâlâ sabit Lv1 taban stat kullanıyor — `level-deneyim-sistemi.md`'deki oyuncu seviyeleme sistemi bu formüle henüz bağlanmadı. → Kod tarafında bir sonraki entegrasyon adımı.

2. **Evrim malzemesi / Evrim Taşı düşmesi**: `loot-odul-sistemi.md`'nin tanımladığı malzeme düşme sistemi bu aşama-temizleme akışına henüz eklenmedi (yalnızca altın/elmas/canavar/ekipman düşüyor). → Kod tarafında eklenmesi gereken bir adım, ayrı bir görev.

3. **Otofarm/Idle entegrasyonu**: `otofarm-idle.md`'nin tanımladığı idle hesaplama akışının bu sistemle (en yüksek temizlenmiş aşama okuma) gerçekten bağlı olup olmadığı kod tarafında ayrıca doğrulanmalı.

4. **Sersemletme bağışıklığı / DoT direnci (boss'lara özgü)**: Önceki tasarımda Mini Boss/Alan Patronu'na özel durum-etkisi direnci vardı — kodda (`CombatUnit`/`CombatManager`) böyle bir ayrım yok, tüm düşmanlar aynı kurala tabi. Bu bilinçli bir sadeleştirme mi yoksa eksik bir özellik mi, kullanıcıyla netleştirilmeli.

5. **Birden fazla Keşif Alanı (Tier 2+)**: Kod şu an tek bir sabit `TotalFloors=20` alanı yönetiyor, bölge/alan çoğullaması henüz yok. Tier 2 genişlemesinde nasıl ölçekleneceği ayrı bir tasarım gerektirir.
