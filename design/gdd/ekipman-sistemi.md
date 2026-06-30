# Ekipman Sistemi

> **Status**: Designed
> **Author**: user + systems-designer, economy-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Fantezisi, Hep Bir Şey Var

## Overview

**Ekipman Sistemi**, oyuncunun ve aktif petinin donanım slotlarına çeşitli tier'larda silah, zırh ve aksesuar takabildiği, ayrıca savaş içinde kullanılmak üzere iksir (pot) taşıyabildiği kalıcı güç büyüme sistemidir. Ekipman istatistikleri savaş hesaplamalarına doğrudan yansır. Prototipte F/D/C/B olmak üzere 4 tier ekipman bulunur. İksirler sarfedilebilir tüketici (consumable) olarak ayrı bir slot'ta taşınır.

Ekipmanlar iki yolla kazanılır: Dükkan'dan altınla satın alınır veya Keşif Alanı aşaması temizlemelerinden item olarak düşer. Her karakter (Oyuncu + aktif Pet) kendi ekipman setini bağımsız olarak taşır.

## Player Fantasy

Savaştan döndükten sonra düşen bir Zırh'ı pet'ine takmak — ve bir sonraki savaşta pet'in kayda değer ölçüde daha sağlam durması. "Seviyesi bitti ama ekipmanla büyümeye devam ediyor" hissi.

Oyuncunun kendi silahını takmak ve Komutan Modu'ndaki hasar artışının eskisinden güçlü hissettirmesi — "ben de güçlendim" fantezisi. Oyuncu HP bağışık olduğundan hayatta kalmak değil, pet'e güç vermek için savaşıyor.

İksir basarken zamanlama kararı: %15 HP'deyken pot mu kullanayım, düşman bu turda ölebilir mi? Bu küçük ama anlamlı bir gerilim anı.

**Negatif fantezi (kaçınılacak)**: Ekipman olmadan ilerlemenin imkânsız hissettirmesi. Ekipman "güç çarpanı" değil "güç eki" olmalı — iyi oyuncu ekipman olmadan da ilerleyebilmeli, ama ekipmanla daha kolay ve tatmin edici.

## Detailed Rules

### Core Rules

**Kural 1 — Ekipman Slotları**

Her karakterin bağımsız ekipman slotları vardır:

| Karakter | Slot | Tip | Stat Etkisi |
|----------|------|-----|-------------|
| **Oyuncu** | Silah | Kılıç/Asa/Yay tipi | Komutan Modu ATK çarpanını artırır |
| **Oyuncu** | Aksesuar | Yüzük/Kolye | Pet'e pasif düz stat bonusu verir (her zaman aktif) |
| **Pet** | Silah | Pençe/Dişler/Kanat vb. | Pet ATK + |
| **Pet** | Zırh | Zırh/Kabuk/Bant vb. | Pet DEF + ve Pet HP + |
| **Pet** | Aksesuar | Taş/Mühür | Pet SPD + ve Pet ATK + |

Toplam: Oyuncu 2 slot, Pet 3 slot → 5 aktif ekipman yuvası.

**Kural 2 — Ekipman Tierleri ve Stat Tablosu**

**Pet Silahı** (ATK+):
| Tier | ATK Bonusu |
|------|-----------|
| F | +5 |
| D | +12 |
| C | +25 |
| B | +40 |

**Pet Zırhı** (DEF+ ve HP+):
| Tier | DEF Bonusu | HP Bonusu |
|------|-----------|----------|
| F | +5 | +30 |
| D | +12 | +70 |
| C | +22 | +130 |
| B | +35 | +200 |

**Pet Aksesuar** (SPD+ ve ATK+):
| Tier | SPD Bonusu | ATK Bonusu |
|------|-----------|-----------|
| F | +2 | +3 |
| D | +5 | +7 |
| C | +9 | +13 |
| B | +14 | +22 |

**Oyuncu Silahı** (Komutan ATK çarpan bonusu):
| Tier | Ek Çarpan | Toplam Çarpan |
|------|----------|--------------|
| F | +0.05 | ×1.35 |
| D | +0.10 | ×1.40 |
| C | +0.18 | ×1.48 |
| B | +0.28 | ×1.58 |

**Oyuncu Aksesuar** (Pet'e pasif düz stat+):
| Tier | Pet ATK+ | Pet DEF+ | Pet HP+ |
|------|---------|---------|--------|
| F | +5 | 0 | 0 |
| D | +12 | 0 | 0 |
| C | +20 | +15 | 0 |
| B | +30 | +25 | +50 |

**Kural 3 — Ekipman Takma / Çıkarma Kuralları**

- Ekipman yalnızca Koleksiyon → Pet Detay ekranı veya Oyuncu Profil ekranından takılır/çıkarılır.
- Savaş sırasında ekipman değiştirilemez.
- Bir slota yeni ekipman takıldığında eski ekipman otomatik envantere döner (kaybolmaz).
- Her tier ekipman, herhangi bir pette kullanılabilir — pet tipine kısıtlama yoktur.
- Ekipmanın başka Pete devri serbesttir: söküp diğerine tak.

**Kural 4 — İksirler (Consumable)**

Prototipte 2 iksir tipi vardır:

| İksir | İyileşme | Taşıma Limiti | Kullanım |
|-------|---------|--------------|---------|
| Küçük İksir | Pet Max HP'nin %30'u | 10 adet | Savaş içi aksiyon |
| Büyük İksir | Pet Max HP'nin %70'i | 5 adet | Savaş içi aksiyon |

- İksirler envanterde stacklenir (aynı tipten toplu taşıma).
- Savaş dışında kullanılamaz.
- Aktif pet'e uygulanır — tüm pette değil, yalnızca o an savaşan pet.

**Kural 5 — Savaş İçi İksir Kullanımı**

- Savaş UI'ında Aksiyon Çubuğu'nun sağında küçük iksir butonu (şişe ikonu, 44dp).
- Basıldığında "Pot Panel" açılır: Küçük İksir ve Büyük İksir satırları (adet gösterir).
- İksir seçilir → pet HP anında iyileşir → iksir adedi 1 azalır.
- İksir kullanmak oyuncunun turunu tüketmez (serbest aksiyon).
- Oto-savaş modunda: pet HP ≤ %25'e düştüğünde önce Büyük İksir, yoksa Küçük İksir otomatik kullanılır.
- Oto-savaş modunda iksir yoksa: kullanılmaz, devam eder.
- Savaş bittikten sonra kullanılan iksirler geri gelmez (sarf edildi).

**Kural 6 — Ekipman Kazanım Yolları**

1. **Dükkan**: Altınla satın alma (günlük dönen stok).
2. **Keşif Alanı drop**: Aşama temizlemelerinde Loot Sistemi tarafından belirlenen % ihtimalle düşer.
   - Normal aşama: %10 iksir, %5 F/D ekipman
   - Şampiyon: %20 iksir, %10 D/C ekipman
   - Mini Boss: %25 ekipman (D–C tier), %30 iksir
   - Alan Patronu: B tier ekipman garantili (1 adet), +50% iksir

**Kural 7 — Envanter Yönetimi**

- Ekipman ve iksirler ayrı inventory bölmelerinde tutulur.
- Ekipman envanter limiti: 50 slot (prototip).
- İksir taşıma limiti Kural 4'te belirtilmiştir.
- Envanter dolduğunda yeni item düşmez — "Envanter Dolu" toast gösterilir. Dükkan'da satış yapılabilir.
- Dükkan'a satış: tüm ekipmanlar Dükkan'da altın karşılığı satılabilir (Kural tablosu Dükkan GDD'de).

### States and Transitions

```
[Koleksiyon / Pet Detay Ekranı]
    ├─ Slot'a dokun → slot seçilir
    └─ Ekipman seç → Onayla → [Ekipman Takıldı]
        └─ Eski ekipman varsa → envantere döndürüldü

[Savaş]
    ├─ İksir butonu basılır → [Pot Panel]
    │   ├─ İksir seçilir → HP anında iyileşir → panel kapanır
    │   └─ İptal → panel kapanır
    └─ Oto-mod → HP ≤ %25 → otomatik iksir

[Keşif Alanı — Aşama Sonu]
    └─ Loot ekranında ekipman kartı belirir → envantere eklenir
```

## Formulas

### Formül 1: Pet Efektif Statlar (Ekipman Dahil)

```
effective_ATK = base_ATK + pet_weapon_ATK + pet_accessory_ATK + player_accessory_ATK
effective_DEF = base_DEF + pet_armor_DEF + player_accessory_DEF
effective_HP  = base_HP  + pet_armor_HP  + player_accessory_HP
effective_SPD = base_SPD + pet_accessory_SPD
```

Bu değerler Hasar Hesaplama GDD'sine (effective_ATK, effective_DEF) ve Savaş Sistemi'ne (effective_HP, effective_SPD) beslenir.

### Formül 2: Komutan ATK (Oyuncu Silahı Dahil)

```
player_weapon_bonus = ekipman yoksa 0.0 | tier bazlı değer (Kural 2 tablosu)
total_cmd_multiplier = 1.30 + player_weapon_bonus
commander_ATK = floor(effective_ATK × total_cmd_multiplier)
```

**Örnek — D tier Oyuncu Silahı:**
`total_cmd_multiplier = 1.30 + 0.10 = 1.40`
`effective_ATK = 35 → commander_ATK = floor(35 × 1.40) = 49`

**Örnek — B tier Oyuncu Silahı:**
`total_cmd_multiplier = 1.30 + 0.28 = 1.58`
`effective_ATK = 35 → commander_ATK = floor(35 × 1.58) = 55`

### Formül 3: İksir İyileşme Miktarı

```
heal_small = floor(pet_max_HP × 0.30)
heal_large = floor(pet_max_HP × 0.70)

// Sınır: HP, max'ı aşamaz
new_HP = min(current_HP + heal_amount, pet_max_HP)
```

**Örnek**: Pet max HP = 300, mevcut HP = 60 (≤%25 eşiği)
`heal_small = floor(300 × 0.30) = 90 → new_HP = min(60 + 90, 300) = 150`
`heal_large = floor(300 × 0.70) = 210 → new_HP = min(60 + 210, 300) = 270`

### Formül 4: Ekipman Statlı Hasar Hesabı Entegrasyonu

```
// Hasar Hesaplama GDD'si effective_ATK ve effective_DEF kullanır.
// Bu formül, ekipman dahil effective değerlerin nasıl üretildiğini gösterir.

// Komutan Modunda:
attacking_ATK = commander_ATK = floor(effective_ATK × total_cmd_multiplier)

// Oto-farm Modunda:
attacking_ATK = effective_ATK  // Komutan bonusu yok
```

### Formül 5: Ekipman Drop Şansı (Keşif Alanı Entegrasyonu)

```
// Loot Sistemi'ne ek tablo (prototip):
base_equip_drop = stage_type'a göre (Kural 6)
roll = random(0.0, 1.0)
if roll < base_equip_drop:
    dropped_tier = stage_type'a göre tier seç
    dropped_slot = random(SILAH, ZIRH, AKSESUAR)  // Pet ekipmanı
    dropped_item = GetEquipmentByTierAndSlot(dropped_tier, dropped_slot)
```

## Edge Cases

- **If pet değiştirilirse (gelecek sürümde çoklu pet varsa)**: Eski pette takılı ekipman onu takılı kalır. Yeni aktif pet kendi slotlarıyla gelir. Ekipman otomatik transfer olmaz.

- **If iksir kullanıldığında pet zaten tam HP'deyse**: İksir yine de kullanılır (sarf edilir). UI'da "Pet HP Zaten Dolu — Yine de kullan?" onay dialogu gösterilir.

- **If oto-mod iksir tetiklerken tam HP'deyse**: Otomatik tetikleme gerçekleşmez — %25 eşiği HP'den düşük olunca tetiklenir, tam HP değil.

- **If envanter dolu ve aşama loot'u ekipman içeriyorsa**: Ekipman envantere eklenmez, "Envanter Dolu" toast. Oyuncu iksirle alabilmek için dükkan'da satmalı. İksir stacklendiğinden limit ayrı kontrol edilir.

- **If iksir envanter limiti doluysa ve aşama iksir drop'u gelirse**: Limit aşılırsa eklenmez, toast. Dükkan'da mevcut iksirler kullanılmalı.

- **If ekipman takılıyken o slot için daha yüksek tier ekipman drop edilirse**: Otomatik değiştirme olmaz — oyuncu manuel olarak değiştirmelidir. İleri versiyon: "Daha güçlü ekipman düştü!" bildirim.

- **If savaşta iksir paneli açıkken savaş sona ererse** (düşman son tur öldü): Panel otomatik kapanır, iksir kullanılmaz. Zafer ekranına geçilir.

- **If oyuncu slotu boşsa (ekipman yoksa)**: Slot görsel olarak boş ("─") gösterilir. Commander ATK multiplier 1.30 (temel değer), oyuncu aksesuar bonusu 0.

- **If aynı anda iki slota aynı item takılabilir mi**: Hayır — bir ekipman yalnızca bir slota takılabilir. Sürükleyip ikinci slota bırakmak, önceki slottan çıkarır ve yenisine takar.

- **If F tier ekipman savaşın ortasında çıkarılabilir mi**: Savaş sırasında ekipman değiştirilemez (Kural 3) — bu edge case geçerli değil; değiştirme girişimi buton disabled ile engellenir.

## Dependencies

### Upstream

| Sistem | Veri / Arayüz |
|--------|--------------|
| **Savaş Sistemi** | `effective_ATK`, `effective_DEF`, `effective_HP`, `effective_SPD` okur — ekipman bonusları bu değerlere eklenir |
| **Hasar Hesaplama** | `commander_ATK` formülüne `total_cmd_multiplier` beslenir |
| **Loot / Ödül Sistemi** | Düşen ekipmanı envantere ekler (`AddEquipmentToInventory(item)`) |
| **Ekonomi** | İksir alımında altın düşer (`SpendGold(amount)`) |
| **Kaydetme / Yükleme** | Ekipman slotları, envanter, iksir adetleri persist edilir |

### Downstream

| Sistem | Etki |
|--------|------|
| **Savaş UI** | İksir butonunu ve pot paneli render eder |
| **Koleksiyon / Envanter UI** | Ekipman slotlarını Pet Detay'da gösterir, equip/unequip tetikler |
| **Dükkan** | Ekipman ve iksir satışı envantere ekleme ve altın harcama üzerinden çalışır |
| **Keşif Alanı** | Drop formülüne slot ve tier parametreleri beslenir |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `pet_weapon_atk_B` | 40 | 30–55 | Late game ekipman > leveling | Ekipman hissedilmez |
| `pet_armor_hp_B` | 200 | 150–280 | Boss'lar trivial | Zırh işe yaramaz hisseder |
| `player_weapon_bonus_B` | 0.28 | 0.20–0.40 | Commander çok baskın | Oyuncu silahı anlamsız |
| `player_acc_atk_B` | 30 | 20–45 | Aksesuar > pet silahı | Bağlantı hissedilmez |
| `heal_small_ratio` | 0.30 | 0.20–0.40 | İksir çok güçlü, boss kolay | Küçük iksir işe yaramaz |
| `heal_large_ratio` | 0.70 | 0.55–0.85 | Yenilmezlik hissi | Büyük iksir tercih edilmez |
| `auto_pot_hp_threshold` | 0.25 | 0.15–0.35 | Oto-mod çok tasarruflu iksir kullanır | İksir geç kullanılır, pet ölür |
| `equip_inventory_limit` | 50 | 30–100 | Envanter yönetimi gereksiz | Oyuncu zorlanır |
| `potion_small_stack_max` | 10 | 5–20 | İksir zaten hiç bitmez | Kıyı savaşında yetersiz |
| `potion_large_stack_max` | 5 | 3–10 | Boss'lar anlamsız | Nadide iksir mantıklı |
| `normal_stage_equip_drop` | 0.05 | 0.02–0.12 | Loot yağmuru, dükkan gereksiz | Oyuncu ekipman kazanamaz |
| `boss_equip_drop_guaranteed` | 1 adet B | sabit | — | Patron ödülü değersizleşir |

## Acceptance Criteria

1. **GIVEN** Pet Detay ekranı açık, **WHEN** "Silah" slotuna D tier bir item takılırsa, **THEN** pet effective_ATK +12 artar, savaştaki hasarda bu artış yansır.

2. **GIVEN** B tier Oyuncu Silahı takılı, **WHEN** Komutan Modu aktifse, **THEN** `commander_ATK = floor(effective_ATK × 1.58)` hesaplanır (örn. ATK=35 → 55).

3. **GIVEN** Oyuncu Aksesuarı C tier (ATK+20, DEF+15), **WHEN** herhangi bir savaşta, **THEN** pet effective_ATK 20 artar, effective_DEF 15 artar — her iki modda da (Komutan + Oto).

4. **GIVEN** pet HP = 60/300 (%20), **WHEN** Büyük İksir kullanılırsa, **THEN** `heal_large = floor(300 × 0.70) = 210`, yeni HP = `min(60+210, 300) = 270`.

5. **GIVEN** pet HP = 280/300 (dolu değil), **WHEN** Büyük İksir kullanılırsa, **THEN** `new_HP = min(280+210, 300) = 300` — max HP'yi aşmaz.

6. **GIVEN** pet HP tam dolu iken iksir butonuna basılırsa, **WHEN** iksir seçilirse, **THEN** "Pet HP Zaten Dolu — Yine de kullan?" onay dialogu açılır.

7. **GIVEN** oto-savaş modu aktif, pet HP = %24 (eşiğin altı), envanterde Büyük İksir var, **WHEN** pet hasarı aşılırsa, **THEN** sistem otomatik Büyük İksir kullanır, iksir adedi 1 azalır.

8. **GIVEN** oto-savaş modu aktif, Büyük İksir yok, Küçük İksir var, pet HP ≤ %25, **WHEN** tetiklenirse, **THEN** Küçük İksir kullanılır.

9. **GIVEN** oto-savaş modu aktif, her iki iksir de yok, pet HP ≤ %25, **WHEN** tetiklenirse, **THEN** iksir kullanılmaz, savaş devam eder.

10. **GIVEN** savaş içinde iksir paneli açık, **WHEN** düşman o turda ölürse (zafer), **THEN** panel otomatik kapanır, iksir harcanmaz, zafer ekranı açılır.

11. **GIVEN** mevcut slotta D tier Zırh varken C tier Zırh takılmak istenirse, **WHEN** oyuncu onaylarsa, **THEN** D tier Zırh envantere döner, C tier Zırh slota takılır.

12. **GIVEN** ekipman takılıyken savaş başlarsa, **WHEN** savaş içinde ekipman butonu var mı kontrol edilirse, **THEN** ekipman slotları disabled (değiştirilemez), savaş bitince aktifleşir.

13. **GIVEN** envanter 50/50 dolu iken aşama ekipman düşürürse, **WHEN** loot ekranı gösterilirse, **THEN** "Envanter Dolu" toast — ekipman envantere eklenmez.

14. **GIVEN** iksir stack limiti 10/10 doluyken dükkan iksir satın almak istenirse, **WHEN** satın alma butonuna basılırsa, **THEN** "İksir Envanteri Dolu (10/10)" uyarısı gösterilir, işlem iptal.

15. **GIVEN** oyuncu slotu tamamen boşken (ekipman yok), **WHEN** Komutan Modu aktifse, **THEN** `total_cmd_multiplier = 1.30`, `player_accessory_ATK = 0` — Formül 1 sıfır ek verir.

16. **GIVEN** Alan Patronu temizlenirse, **WHEN** loot ekranı açılırsa, **THEN** B tier ekipman (rastgele slot: Silah, Zırh veya Aksesuar) garantili olarak envantere eklenir.

17. **GIVEN** Küçük İksir stack 1 adetteyken kullanılırsa, **WHEN** kullanılırsa, **THEN** adet 0 olur, iksir butonu "Küçük İksir" satırı gri (disabled) gösterilir.

18. **GIVEN** Pet Aksesuar C tier (SPD+9, ATK+13) takılıyken, **WHEN** savaş tur sırası hesaplanırsa, **THEN** `effective_SPD = base_SPD + 9`, tur sırası buna göre düzenlenir.

19. **GIVEN** B tier Pet Zırh (DEF+35, HP+200) takılıyken, **WHEN** savaş başlarsa, **THEN** pet max HP = `base_HP + 200`, savaştaki HP barı bu değeri yansıtır.

20. **GIVEN** ekipman satışı yapılmak istenirse (dükkan üzerinden), **WHEN** F tier ekipman satılırsa, **THEN** Ekipman Sistemi `RemoveEquipment(itemId)` çağrısı yapar, Dükkan GDD'de tanımlı altın ödülü verilir.
