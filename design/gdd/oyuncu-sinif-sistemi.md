# Oyuncu Sınıf Sistemi (Player Class System)

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-29
> **Implements Pillar**: Güç Hisset, Senin Tempon

## Overview

Oyuncu Sınıf Sistemi, oyuncunun savaştaki kimliğini ve oyun stilini belirleyen temel sistemdir. 4 ana sınıf (Savaşçı, Büyücü, Hırsız, Şifacı) arasından seçim yapılır; her sınıfın benzersiz stat profili, 4 yetenek slotu ve 2 yan sınıf dalı (TBD) vardır. Sınıf seçimi oyunun başında yapılır.

Yetenek Sistemi GDD'sindeki Slot 0–3'ün tam içeriği bu GDD'de tanımlanır. Savaşçı ve Hırsız fiziksel hasar türü kullanır; Büyücü ve Şifacı büyü hasarı türü kullanır (magic DEF penetrasyonu). Yeni durum etkileri bu GDD ile sisteme eklenir: DoT (yanma/zehir), sersemletme, DEF kırma, ATK zayıflatma, kalkan ve hasar azaltma.

Her sınıf için 2 yan sınıf dalı tanımlanmıştır (isimler sabit, içerikler TBD). Pet pasif buff sistemi (pet → oyuncu stat bonusu) de bu sistemle entegre olacak, detaylar TBD.

## Player Fantasy

- **Savaşçı**: "Ben ölmüyorum." — düşmanlar kendi kendine erirken oyuncu yerinden kımıldamaz. Yıkım Darbesi'nin 4× hasarı bir mini-boss'u devirdiğinde "bu benim zaferim" anı.
- **Büyücü**: "Bir dokunuşta çöküyorlar." — cam kanon riski. Büyü Fırtınası sahneyi kaplarken 3 düşman aynı anda yanmaya başlar.
- **Hırsız**: "Gördüklerimi seçerim." — Suikast Fırtınası'nın 5 bıçağından 3'ü kritik yaparken sayılar patlar.
- **Şifacı**: "Hiçbir şey ölmez benim yanımda." — Koruma Aura'sı aktifken düşmanların çektiği hasar erirken kontrol hissi.

**Negatif fantezi (kaçınılacak)**: Şifacı "sadece iyileştirir" olmamalı — Kutsal Işın ile gerçek hasar verir. Savaşçı "sadece tank" değil — Yıkım Darbesi en yüksek tek vuruş çarpanıdır (4×).

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Kural 1 — Sınıf Stat Profilleri (Level 1 Baz Değerler)

| Sınıf | HP | ATK | DEF | SPD | Kimlik |
|-------|----|----|-----|-----|--------|
| **Savaşçı** | 55 | 18 | 40 | 20 | Yüksek DEF + HP, düşük ATK ve SPD |
| **Büyücü** | 25 | 45 | 10 | 28 | Çok yüksek ATK, çok düşük DEF ve HP |
| **Hırsız** | 35 | 32 | 15 | 45 | En yüksek SPD, yüksek ATK, düşük DEF |
| **Şifacı** | 48 | 20 | 28 | 25 | Dengeli HP/DEF, düşük ATK |

Stat ölçekleme: `stat(level) = floor(base_stat × (1 + stat_growth × (level - 1)))`

`stat_growth = 0.08` (tüm sınıflarda eşit; yan sınıf bonusları bu değerin üstüne eklenir, TBD)

Oyuncu max seviyesi **30** (kapatıldı — bkz. `level-deneyim-sistemi.md` Kural 7).

---

### Kural 2 — Hasar Türleri

| Hasar Türü | Kullanan Sınıflar | defense_reduction_factor | Açıklama |
|------------|-------------------|--------------------------|----------|
| **Fiziksel** | Savaşçı, Hırsız | 2 | Mevcut pipeline — değişmedi |
| **Büyü** | Büyücü, Şifacı | 4 | DEF yarısı kadar etkili — magic zırha daha az takılır |

Büyü hasarı tüm saldırı slotlarına (Slot 0–3) uygulanır; "magic" olarak işaretli birim her zaman `defense_reduction_factor=4` kullanır.

⚠️ **Hasar Hesaplama GDD revizyonu gerekli**: `magic_defense_factor=4` parametresi mevcut GDD'ye eklenmeli.

---

### Kural 3 — Sınıf Yetenek Tabloları

#### Savaşçı (Warrior)

| Slot | Yetenek | CD | Etki | Hedef |
|------|---------|----|------|-------|
| 0 | **Kılıç Darbesi** | 0 | Fiziksel ATK × 1.0 | Tek düşman |
| 1 | **Kalkan Ezme** | 3 | Fiziksel ATK × 2.0 + **Sersemletme**: hedef 1 tur aksiyon yapamaz | Tek düşman |
| 2 | **Demir Zırh** | 5 | **DEF buff**: kendi DEF × 1.5, 3 tur | Kendisi |
| 3 | **Yıkım Darbesi** | 8 | Fiziksel ATK × 4.0 + **DEF Kırma**: hedef DEF ×0.70, 2 tur | Tek düşman |

#### Büyücü (Mage)

| Slot | Yetenek | CD | Etki | Hedef |
|------|---------|----|------|-------|
| 0 | **Büyü Oku** | 0 | Büyü ATK × 1.0 (defense_reduction_factor=4) | Tek düşman |
| 1 | **Büyü Patlaması** | 3 | Büyü ATK × 2.0 + **Yanma DoT**: max_hp × 0.05/tur, 3 tur | Tek düşman |
| 2 | **Büyü Zırhı** | 5 | **Kalkan**: max_hp × 0.25 hasar emer, 3 tur (dolunca kalkar) | Kendisi |
| 3 | **Büyü Fırtınası** | 8 | Tüm düşmanlara Büyü ATK × 1.5 + her hedefe **Yanma DoT**: max_hp × 0.05/tur, 3 tur | Tüm düşmanlar |

#### Hırsız (Thief)

| Slot | Yetenek | CD | Etki | Hedef |
|------|---------|----|------|-------|
| 0 | **Hızlı Bıçak** | 0 | Fiziksel ATK × 1.0 + **+%25 crit şansı** | Tek düşman |
| 1 | **Zehir Hançeri** | 3 | Fiziksel ATK × 1.5 + **Zehir DoT**: max_hp × 0.04/tur, 4 tur + **ATK Zayıflatma**: hedef ATK ×0.80, 2 tur | Tek düşman |
| 2 | **Gölge Adımı** | 5 | **DEF buff**: kendi DEF × 1.5, 2 tur + **Kesin Kritik**: sonraki 1 saldırı kesin kritik | Kendisi |
| 3 | **Suikast Fırtınası** | 8 | **5 hızlı vuruş**: her biri Fiziksel ATK × 0.8, her biri bağımsız %40 crit şansı | Tek düşman |

#### Şifacı (Healer)

| Slot | Yetenek | CD | Etki | Hedef |
|------|---------|----|------|-------|
| 0 | **Kutsal Işın** | 0 | Büyü ATK × 1.0 (kutsal tür, defense_reduction_factor=4) | Tek düşman |
| 1 | **Kutsal Darbe** | 3 | Büyü ATK × 2.0 + **İyileştirme**: kendini ve aktif peti max_hp × 0.10 | Düşman + kendisi + pet |
| 2 | **Büyük İyileştirme** | 5 | Kendini max_hp × 0.20 + aktif peti max_hp × 0.35 iyileştirir | Kendisi + pet |
| 3 | **Koruma Aura** | 8 | Kendini + peti max_hp × 0.40 iyileştirir + **Hasar Azaltma**: gelen hasar ×0.75, 2 tur | Kendisi + pet |

---

### Kural 4 — Yeni Durum Etkileri (Status Effects)

| Etki | Kaynak | Mekanik | Birikim |
|------|--------|---------|---------|
| **Yanma DoT** | Büyücü Slot 1/3 | Her tur `floor(max_hp × 0.05)` hasar, pipeline dışı (DEF bypass) | Aynı tür: süre yenilenir, hasar stack olmaz |
| **Zehir DoT** | Hırsız Slot 1 | Her tur `floor(max_hp × 0.04)` hasar, pipeline dışı | Aynı tür: süre yenilenir, hasar stack olmaz |
| **Sersemletme** | Savaşçı Slot 1 | 1 tur DecisionPhase+ActionPhase atlanır (tur gelir ama aksiyon yok) | Stack olmaz; yeni uygulama süreyi yeniler |
| **DEF Kırma** | Savaşçı Slot 3 | Hasar hesaplamada `effective_DEF × 0.70` kullanılır, 2 tur | Stack olmaz |
| **ATK Zayıflatma** | Hırsız Slot 1 | Hasar hesaplamada `effective_ATK × 0.80` kullanılır, 2 tur | Stack olmaz |
| **Kalkan** | Büyücü Slot 2 | Gelen hasar önce kalkandan düşer; kalkan sıfırlanınca HP'ye geçer | Stack olmaz; yeni kalkan eskisini sıfırlayıp değiştirir |
| **Kesin Kritik** | Hırsız Slot 2 | Sonraki 1 saldırıda crit roll atlanır, crit_multiplier kesin uygulanır | Tek kullanım — ilk saldırıda tükenir |
| **Hasar Azaltma** | Şifacı Slot 3 | Gelen hasar `floor(damage × 0.75)` olarak uygulanır, 2 tur | Stack olmaz |

**DoT uygulama zamanı**: Her birimin kendi turunda, RegenPhase'den önce tetiklenir.

**Boss/mini-boss bağışıklıkları**: Sersemletme boss ve mini-boss hedeflere uygulanmaz; hasar uygulanır. Diğer tüm etkiler (DoT, DEF kırma vb.) boss'a da uygulanır.

---

### Kural 5 — Yan Sınıf Sistemi (Placeholder — TBD)

Her ana sınıfın 2 yan sınıf dalı vardır. Yan sınıf seçimi şartı (belirli bir level veya görev) TBD. İsimler sabit; stat bonusları ve yetenek değişiklikleri sonraki tasarım oturumunda.

| Ana Sınıf | Dal 1 | Dal 2 |
|-----------|-------|-------|
| **Savaşçı** | Berserker (saldırı dal) | Koruyucu (savunma dal) |
| **Büyücü** | Elementalist (DoT odaklı — isim TBD, element sistemi kaldırıldı) | Kaoscu (saf burst) |
| **Hırsız** | Gölge (zehir + DoT) | Düellocu (kritik + hız) |
| **Şifacı** | Işık Rahibi (iyileştirme odaklı) | Savaş Rahibi (iyileştirme + büyü hasarı) |

---

### Kural 6 — Pet Pasif Buff Sistemi (Placeholder — TBD)

Aktif pet oyuncuya pasif stat bonusu verir (DEF, fiziksel ATK, büyü ATK, max HP vb.). Bonus miktarları pet tier ve arketipine göre değişir. Detaylar Pet/Canavar Veritabanı revizyonuyla tanımlanacaktır.

## Formulas

### Formül 1: Büyü Hasarı

`magic_damage = max(1, floor(effective_ATK × multiplier - floor(effective_DEF / magic_defense_factor)) × [crit])`

| Değişken | Sembol | Tip | Değer | Açıklama |
|----------|--------|-----|-------|----------|
| Büyü DEF faktörü | magic_defense_factor | int | 4 | Fiziksel (2) yerine; DEF yarısı kadar etkili |
| ATK çarpanı | multiplier | float | 1.0–4.0 | Slota göre değişir |

**Karşılaştırma örneği** — Büyücü (ATK=45) vs Savaşçı tipi (DEF=40), nötr, crit yok:
- Büyü Oku: `max(1, 45 - floor(40/4)) = max(1, 45-10) = **35**`
- Fiziksel aynı ATK: `max(1, 45 - floor(40/2)) = max(1, 45-20) = **25**`
→ Büyü, yüksek DEF hedefe karşı **%40 daha fazla** hasar.

---

### Formül 2: DoT Hasarı

`dot_damage_per_tick = max(1, floor(target_max_hp × dot_rate))`

| DoT Türü | dot_rate | Süre | Toplam (HP=30) | Toplam (HP=258) |
|----------|----------|------|----------------|-----------------|
| Yanma | 0.05 | 3 tur | 1×3 = **3** | 12×3 = **36** |
| Zehir | 0.04 | 4 tur | 1×4 = **4** | 10×4 = **40** |

`dot_total = dot_damage_per_tick × duration_turns`

---

### Formül 3: Stat Ölçekleme

`stat(level) = floor(base_stat × (1 + stat_growth × (level - 1)))`

`stat_growth = 0.08`

| Level | Büyücü ATK | Savaşçı DEF | Hırsız SPD |
|-------|------------|-------------|------------|
| 1 | 45 | 40 | 45 |
| 10 | floor(45×1.72)=**77** | floor(40×1.72)=**68** | floor(45×1.72)=**77** |
| 30 | floor(45×3.32)=**149** | floor(40×3.32)=**132** | floor(45×3.32)=**149** |

---

### Formül 4: Suikast Fırtınası Toplam Hasar

`total_ambush = Σ(i=1 to 5) max(1, floor(effective_ATK × 0.8 - floor(effective_DEF / 2)) × [crit_i])`

Her vuruş bağımsız crit roll (%40 şans, crit_multiplier=2.0):

`E[total] = 5 × base_hit × (0.60 + 0.40 × 2.0) = 5 × base_hit × 1.40`

**Örnek** — Hırsız (ATK=32) vs düşman (DEF=20), nötr:
→ base_hit = max(1, floor(32×0.8) - 10) = 15
→ E[total] = 5 × 15 × 1.40 = **105**
→ Normal saldırı: 22. Suikast: ~**4.8× daha etkili** (beklenen değer)

## Edge Cases

- **If Sersemletme uygulandığında hedef zaten savaş dışıysa**: Etki yok, sersemletme uygulanmaz.

- **If Boss/mini-boss'a Sersemletme uygulanırsa**: Hasar uygulanır, sersemletme bağışıklığı devreye girer. VFX ile "bağışık" bildirimi gösterilir.

- **If Yanma aktif hedef üzerine tekrar Büyü Patlaması uygulanırsa**: Yanma süresi 3'e yenilenir, tick hasarı değişmez (stack olmaz).

- **If Zehir ve Yanma aynı hedefte aynı anda aktifse**: Bağımsız sayaçlar — her biri kendi tur başında tetiklenir. Her tur iki ayrı DoT hasarı.

- **If Büyü Zırhı (kalkan=12) aktifken 30 hasar gelirse**: Kalkan 12 hasar emer (sıfırlanır, kalkar), kalan 18 HP'den düşer.

- **If Gölge Adımı sonrası Kesin Kritik bayrağı varken Slot 0 yerine Slot 3 kullanılırsa**: Kesin Kritik bayrağı ilk saldırı aksiyonunda tükenir — hangi slot olursa olsun.

- **If Savaşçı Yıkım Darbesi (DEF Kırma) aktifken hedef DEF buff alırsa**: Bağımsız çarpımlar: `effective_DEF × 0.70 × buff_multiplier`. Sıra önemli değil, çarpım sırası sonucu değiştirmez.

- **If Şifacı Büyük İyileştirme aktif pet yokken kullanılırsa**: Pet iyileştirmesi atlanır, sadece Şifacı iyileştirilir. CD başlar, yetenek "yarım" çalışmaz.

- **If DoT kaynağı savaş dışı kalırsa**: Uygulanmış DoT devam eder (etki zaten hedefte). DoT, kaynağa bağlı değildir.

- **If Suikast Fırtınası 5 vuruşunun ilkinde hedef savaş dışı kalırsa**: Kalan vuruşlar iptal edilir. Hasar "boşa harcanmaz" — düşman ölmüş.

- **If Level 1 hedef HP=18 için Yanma tick hesabı floor(18×0.05)=0 çıkarsa**: `max(1, ...)` garantisi — min 1 hasar/tur uygulanır.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `CalculateDamage(atk, def, multiplier, damageType)` — `damageType: "physical"/"magic"` | Olmadan saldırı yetenekleri hasar üretemez; magic tipi revizyon gerekli ⚠️ |
| **Sağlık / Can Sistemi** | Sert | `Heal(targetId, amount)`, `TakeDamage(targetId, amount)`, `GetMaxHP(targetId)` | İyileştirme ve DoT uygulaması için zorunlu |
| **Yetenek Sistemi** | Sert | Cooldown yönetimi, slot çerçevesi | Slot 0–3 CD yapısı Yetenek Sistemi'nden gelir |
| **Ekonomi / Kaydetme** | Orta | Sınıf seçimi + seviye persist | Olmadan sınıf seçimi kaybolur |
| **Level / Deneyim Sistemi** | Sert | `GetPlayerLevel()` — Kural 1'deki `stat(level)` formülüne girdi sağlar; `player_level_cap=30` bu sistemden gelir | Olmadan level sabit kalır (şu an sabit 1) |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Savaş Sistemi** | Sert | Slot 0–3 etki içerikleri, durum etkisi uygulama | Savaş sistemi yetenek içeriklerini bu GDD'den alır |
| **Yetenek Sistemi** | Sert | Slot 2/3 etki tanımları | Bu GDD olmadan Slot 2/3 devre dışı (Yetenek GDD Kural 2) |

**Bidirectional check**:
- Yetenek Sistemi GDD: Kural 2 / Slot 2/3 → "Oyuncu Sınıf GDD'sinde tanımlanır" ✓
- Hasar Hesaplama GDD: `magic_defense_factor` revizyonu gerekli ⚠️

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `warrior_base_def` | 40 | 30–50 | Savaşçı hasar almaz → savaşlar uzar | Tank hissi kaybolur |
| `mage_base_atk` | 45 | 35–55 | Büyücü diğer sınıfları gölgede bırakır | Büyü Oku zayıf; Büyücü önemsiz |
| `thief_base_spd` | 45 | 35–55 | Hırsız daima ilk → diğer sınıflar anlamsız | Hız avantajı hissedilmez |
| `magic_defense_factor` | 4 | 3–6 | Fiziksel tamamen anlamsız | Büyü fizikseldan farklılaşmaz |
| `dot_rate_burn` | 0.05 | 0.03–0.08 | 3 turda %15 → birleşince overpower | DoT hissedilmez |
| `dot_rate_poison` | 0.04 | 0.03–0.07 | Zehir + debuff kombinasyonu overpower | "Sadece hasar" gibi görünür |
| `warrior_slot3_mult` | 4.0 | 3.0–5.0 | Tek vuruşta çöküm | CD 8 maliyeti karşılanmaz |
| `thief_ambush_hits` | 5 | 3–7 | Crit combo ezici | Kombo hissi kaybolur |
| `thief_ambush_crit_chance` | 0.40 | 0.25–0.60 | Her vuruş kritik → ezici | Randomness heyecan yaratmaz |
| `healer_aura_reduction` | 0.25 | 0.15–0.35 | 2 tur yenilmezlik | Aura hissedilmez |
| `shield_rate` | 0.25 | 0.15–0.40 | Büyücü asla ölmez | Kalkan anlamsız |
| `stat_growth` | 0.08 | 0.05–0.12 | Endgame güç uçurumu | Büyüme hissedilmez |

## Acceptance Criteria

1. **GIVEN** Büyücü (ATK=45), düşman (DEF=40), nötr, crit yok, **WHEN** Büyü Oku kullanılırsa, **THEN** `max(1, 45 - floor(40/4)) = 35` hasar.

2. **GIVEN** Savaşçı (ATK=18), aynı düşman (DEF=40), nötr, crit yok, **WHEN** Kılıç Darbesi kullanılırsa, **THEN** `max(1, 18 - floor(40/2)) = max(1, -2) = 1` hasar (min floor).

3. **GIVEN** Büyücü Slot 1 kullanılırsa, **WHEN** Büyü Patlaması çarpınca, **THEN** hedef Yanma debuffunu alır: 3 tur süre, her tur `max(1, floor(hedef_max_hp × 0.05))` hasar.

4. **GIVEN** Yanma aktif, hedef max_hp=60, **WHEN** 3 tur geçerse, **THEN** toplam DoT = `max(1, floor(60×0.05)) × 3 = 3 × 3 = 9`.

5. **GIVEN** Hırsız (ATK=32) Suikast Fırtınası, düşman (DEF=20), nötr, tüm vuruşlar normal (crit yok), **WHEN** yetenek kullanılırsa, **THEN** her vuruş = `max(1, floor(32×0.8) - 10) = 15`. Toplam = 75.

6. **GIVEN** Hırsız Gölge Adımı kullanılırsa, **WHEN** bir sonraki saldırı yapılırsa (hangi slot), **THEN** crit roll atlanır, `crit_multiplier` kesin uygulanır. Bayrağı tükenir.

7. **GIVEN** Savaşçı Kalkan Ezme boss'a uygulandığında, **WHEN** saldırı çözümlenirse, **THEN** `ATK × 2.0` fiziksel hasar uygulanır, sersemletme uygulanmaz, "bağışık" VFX gösterilir.

8. **GIVEN** Şifacı Büyük İyileştirme (Slot 2), Şifacı max_hp=48 current_hp=20, pet max_hp=30 current_hp=10, **WHEN** kullanılırsa, **THEN** Şifacı +`floor(48×0.20)`=9 HP (→29); pet +`floor(30×0.35)`=10 HP (→20).

9. **GIVEN** Büyü Zırhı kalkan=12 aktifken 30 hasar gelirse, **WHEN** hasar uygulanırsa, **THEN** kalkan 12 emer (sıfırlanır, kalkar), 18 HP'den düşer.

10. **GIVEN** Level 1 hedef HP=18 için Yanma tick = `floor(18×0.05)=0`, **WHEN** DoT tetiklenirse, **THEN** `max(1, 0) = 1` hasar uygulanır (min floor garantisi).

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **Yan sınıf içerikleri (TBD)** — Berserker/Koruyucu/Elementalist/Kaoscu/Gölge/Düellocu/Işık Rahibi/Savaş Rahibi stat bonusları ve yetenek değişimleri. Sonraki tasarım oturumunda.

2. **Pet Pasif Buff Sistemi (TBD)** — Aktif pet oyuncuya DEF, fiziksel ATK, büyü ATK, max HP pasif bonusu verir. Pet/Canavar Veritabanı revizyonuyla birlikte tasarlanacak.

3. **Hasar Hesaplama GDD güncellemesi** — `magic_defense_factor=4` parametresi eklenmeli. Mevcut GDD yalnızca `defense_reduction_factor=2` tanımlıyor.

4. **Oyuncu max level** — MVP öneri: Level 30. Stat scaling buna göre kalibre edilmeli.

5. **Sınıf değiştirme** — Oyuncu sınıf değiştirebilir mi? MVP öneri: hayır (yeni oyun). Tier 2'de değerlendirilebilir.

6. **Yetenek seviyelendirme + loadout sistemi** — Her yetenek Lv1→Lv5 (çarpan artar), açılabilir yetenek havuzu (her sınıf 6-8 yetenek öğrenir), savaş öncesi slot düzeni seçimi. Ayrı bir "Yetenek İlerlemesi / Loadout Sistemi" GDD'si olarak tasarlanacak.
