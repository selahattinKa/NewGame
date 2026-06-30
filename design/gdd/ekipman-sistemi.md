# Ekipman Sistemi

> **Status**: Designed
> **Author**: user + systems-designer, economy-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Fantezisi, Hep Bir Şey Var

## Overview

**Ekipman Sistemi**, oyuncunun 11 ekipman slotuna çeşitli tier'larda donanım takabildiği, petinin ise 2 ekipman slotuna sahip olduğu kalıcı güç büyüme sistemidir. Oyuncu ana savaşçı olduğundan ekipman sisteminin ağırlığı oyuncu tarafındadır. Ekipman istatistikleri savaş hesaplamalarına doğrudan yansır.

Prototipte F/D/C/B olmak üzere 4 tier ekipman bulunur. İksirler (pot) sarfedilebilir tüketici (consumable) olarak ayrı bir slotta taşınır ve yalnızca savaş içinde kullanılır. Ekipmanlar Dükkan'dan altınla satın alınır veya Keşif Alanı aşama temizlemelerinden drop edilir.

## Player Fantasy

Savaştan döndükten sonra düşen Zırh'ı takmak, HP barının görünür biçimde büyüdüğünü görmek — "bu artık farklı bir oyuncu." 11 slot dolduğunda oyuncu tüm vücudunu donatmış, klasik RPG fantezisini yaşamış hisseder.

Yüzük, küpe, kolye gibi aksesuar slotları hem estetik hem de güç hissi verir — "bu yüzüğü özel bir boss'tan düşürdüm, o yüzden kolye ile birlikte takıyorum" RPG hikayeleri. Her slot anlamlı.

**Negatif fantezi (kaçınılacak)**: Slotların çoğunun boş kalması. F tier de olsa slot dolu olmalı — oyuncunun ilk günden tüm slotlara bir şeyler takabilmesi gerekir. Dükkan F tier ekipmanları ucuz tutar bunun için.

## Detailed Rules

### Core Rules

**Kural 1 — Oyuncu Ekipman Slotları (11 Slot)**

| Slot | Tür | Birincil Stat | İkincil Stat |
|------|-----|--------------|-------------|
| **Kask** | Baş | DEF + | HP + |
| **Zırh** | Göğüs | DEF ++ | HP + |
| **Pantalon** | Bacak | DEF + | SPD + |
| **Eldiven** | El | ATK + | — |
| **Bot** | Ayak | SPD + | DEF + |
| **Silah** | Silah | ATK ++ | — |
| **Yüzük 1** | Aksesuar | ATK + veya DEF + | — |
| **Yüzük 2** | Aksesuar | ATK + veya DEF + | — |
| **Küpe 1** | Aksesuar | SPD + veya ATK + | — |
| **Küpe 2** | Aksesuar | SPD + veya ATK + | — |
| **Kolye** | Aksesuar | HP + veya DEF + | — |

Aksesuar slotları (Yüzük ×2, Küpe ×2, Kolye) her item için bağımsızdır — aynı item iki slota takılamaz.

**Kural 2 — Pet Ekipman Slotları (2 Slot)**

Pet ikincil saldırgan olduğundan daha az slot:

| Slot | Tür | Birincil Stat |
|------|-----|--------------|
| **Pet Silah** | Saldırı | Pet ATK + |
| **Pet Aksesuar** | Yardımcı | Pet SPD + veya Yetenek Bonusu |

**Kural 3 — Tier Stat Tablosu — Oyuncu**

**Kask (DEF+ / HP+):**
| Tier | DEF | HP |
|------|-----|----|
| F | +4 | +20 |
| D | +10 | +50 |
| C | +18 | +95 |
| B | +28 | +150 |

**Zırh (DEF++ / HP+):**
| Tier | DEF | HP |
|------|-----|----|
| F | +8 | +30 |
| D | +20 | +75 |
| C | +35 | +140 |
| B | +55 | +220 |

**Pantalon (DEF+ / SPD+):**
| Tier | DEF | SPD |
|------|-----|-----|
| F | +5 | +2 |
| D | +12 | +5 |
| C | +22 | +9 |
| B | +34 | +14 |

**Eldiven (ATK+):**
| Tier | ATK |
|------|-----|
| F | +5 |
| D | +13 |
| C | +24 |
| B | +38 |

**Bot (SPD+ / DEF+):**
| Tier | SPD | DEF |
|------|-----|-----|
| F | +3 | +3 |
| D | +7 | +8 |
| C | +13 | +14 |
| B | +20 | +22 |

**Silah (ATK++):**
| Tier | ATK |
|------|-----|
| F | +8 |
| D | +20 |
| C | +38 |
| B | +60 |

**Yüzük ×2 (ATK+ veya DEF+ — her item farklı stat verebilir):**
| Tier | ATK (saldırı yüzüğü) | DEF (koruma yüzüğü) |
|------|---------------------|---------------------|
| F | +4 | +4 |
| D | +10 | +10 |
| C | +19 | +19 |
| B | +30 | +30 |

**Küpe ×2 (SPD+ veya ATK+):**
| Tier | SPD (hız küpesi) | ATK (güç küpesi) |
|------|-----------------|-----------------|
| F | +2 | +3 |
| D | +5 | +8 |
| C | +9 | +15 |
| B | +14 | +24 |

**Kolye (HP+ veya DEF+):**
| Tier | HP (can kolyesi) | DEF (koruma kolyesi) |
|------|-----------------|---------------------|
| F | +25 | +5 |
| D | +60 | +12 |
| C | +110 | +22 |
| B | +175 | +35 |

**Kural 4 — Tier Stat Tablosu — Pet**

**Pet Silah (ATK+):**
| Tier | ATK |
|------|-----|
| F | +5 |
| D | +12 |
| C | +25 |
| B | +40 |

**Pet Aksesuar (SPD+ veya Yetenek Bonusu):**
| Tier | SPD | Yetenek Bonusu |
|------|-----|---------------|
| F | +2 | — |
| D | +5 | — |
| C | +9 | Enerji dolum +5/tur |
| B | +14 | Enerji dolum +10/tur |

**Kural 5 — Ekipman Takma / Çıkarma Kuralları**

- Ekipman yalnızca Oyuncu Profil ekranı (oyuncu slotları) veya Koleksiyon → Pet Detay ekranından (pet slotları) takılır/çıkarılır.
- Savaş sırasında ekipman değiştirilemez — slotlar disabled görünür.
- Bir slota yeni ekipman takıldığında eski ekipman otomatik envantere döner (kaybolmaz).
- Aynı item iki farklı slota (örn. iki Yüzük slotuna) takılamaz — item benzersiz bağlanır.

**Kural 6 — İksirler (Consumable)**

Prototipte 2 iksir tipi:

| İksir | İyileşme | Taşıma Limiti | Hedef |
|-------|---------|--------------|-------|
| Küçük İksir | Oyuncu Max HP %30 | 10 adet | Oyuncu HP |
| Büyük İksir | Oyuncu Max HP %70 | 5 adet | Oyuncu HP |

- Savaş dışında kullanılamaz.
- Oto-modda: oyuncu HP ≤ %25 → Büyük İksir önce, yoksa Küçük İksir.

**Kural 7 — Ekipman Kazanım Yolları**

1. **Dükkan**: Altınla satın alma (günlük dönen stok — slot tipi önceden bilinmez).
2. **Keşif Alanı drop**: Aşama temizlemesinde % ihtimalle düşer.
   - Normal aşama: %5 F/D ekipman, %10 iksir
   - Şampiyon: %10 D/C ekipman, %20 iksir
   - Mini Boss: %20 D/C ekipman, %30 iksir
   - Alan Patronu: B tier ekipman (rastgele slot — oyuncu veya pet) garantili + %50 iksir

**Kural 8 — Envanter Yönetimi**

- Ekipman envanteri: 50 slot.
- İksir taşıma limiti ayrı (Kural 6).
- Envanter dolduğunda yeni item gelmez — "Envanter Dolu" toast, Dükkan'da satış yapılmalı.
- Satış fiyatları: Dükkan GDD'de tanımlıdır (alış fiyatının %25'i).
- Takılı ekipman satış listesinde görünmez — önce çıkarılmalı.

### States and Transitions

```
[Oyuncu Profil / Pet Detay]
    ├─ Slot'a dokun → Envanter listesi açılır (o slot tipine uygun filtreli)
    └─ Item seç → Onayla → Ekipman takıldı (eski varsa → envantere döndü)

[Savaş]
    ├─ İksir butonu → Pot Panel → seç → oyuncu HP iyileşir
    └─ Ekipman slot butonu → disabled (savaşta değiştirilemez)

[Keşif Alanı — Aşama Sonu Loot Ekranı]
    └─ Ekipman kartı belirir → envantere eklenir
```

## Formulas

### Formül 1: Oyuncu Efektif Statlar (Tüm Ekipman Dahil)

```
effective_ATK = base_class_ATK
    + silah_ATK + eldiven_ATK
    + yuzuk1_ATK + yuzuk2_ATK
    + kupe1_ATK + kupe2_ATK

effective_DEF = base_class_DEF
    + kask_DEF + zirh_DEF + pantalon_DEF
    + eldiven_DEF + bot_DEF
    + kolye_DEF + yuzuk1_DEF + yuzuk2_DEF

effective_HP = base_class_HP
    + kask_HP + zirh_HP
    + kolye_HP

effective_SPD = base_class_SPD
    + pantalon_SPD + bot_SPD
    + kupe1_SPD + kupe2_SPD
```

Boş slotlar için +0 kullanılır.

### Formül 2: Tam Set Oyuncu — B Tier Örneği

```
// Varsayımsal Warrior sınıfı base stats: HP=200, ATK=40, DEF=15, SPD=45
// B tier tam set:
effective_ATK  = 40 + 60(silah) + 38(eldiven) + 30(yuzuk1-atk) + 30(yuzuk2-atk)
               + 24(kupe1-atk) + 24(kupe2-atk) = 246

effective_DEF  = 15 + 28(kask) + 55(zirh) + 34(pantalon) + 0(eldiven-def)
               + 22(bot) + 35(kolye-def) + 0(yuzuk-def) = 189

effective_HP   = 200 + 150(kask) + 220(zirh) + 175(kolye) = 745

effective_SPD  = 45 + 14(pantalon) + 20(bot) + 14(kupe1) + 14(kupe2) = 107
```

### Formül 3: Komutan Modu ATK Uygulama

```
attacking_ATK = floor(effective_ATK × 1.30)   // Komutan
attacking_ATK = effective_ATK                  // Oto
```

### Formül 4: İksir İyileşmesi

```
heal_small = floor(effective_HP × 0.30)
heal_large = floor(effective_HP × 0.70)
new_HP = min(current_HP + heal, effective_HP)
```

**Örnek**: effective_HP = 300, mevcut HP = 60
`heal_large = floor(300 × 0.70) = 210 → new_HP = min(270, 300) = 270`

### Formül 5: Pet Efektif ATK

```
effective_pet_ATK = pet.base_ATK + pet_silah_ATK
effective_pet_SPD = pet.base_SPD + pet_aksesuar_SPD
pet_energy_per_turn = 25 + pet_aksesuar_energy_bonus  // C tier: +5, B tier: +10
```

### Formül 6: Ekipman Satış Fiyatı

```
sell_price = floor(buy_price × 0.25)
```

## Edge Cases

- **If aynı slot tipi için iki item takılmaya çalışılırsa (iki Silah)**: Oyuncunun yalnızca bir Silah slotu var — var olan çıkarılıp yenisi takılır veya iptal edilir. Yüzük 1 ve Yüzük 2 birbirinden bağımsız — farklı item takılabilir.

- **If iki Yüzük slotuna aynı item takılmak istenirse**: Sistem reddeder: "Bu item zaten Yüzük 1'de takılı." Her item yalnızca bir slota bağlanabilir.

- **If ekipman takılıyken savaş başlarsa**: Slotlar savaş süresince kilitlenir (disabled). Savaş bitince kilid açılır.

- **If envanter doluyken Alan Patronu'ndan B tier ekipman düşerse**: "Envanter Dolu" toast — item gelmez. Oyuncu loot ekranında uyarılır, Dükkan'a gidip yer açabilir (o aşamaya yeniden girmesi gerekir).

- **If iksir stack tam doluyken dükkan'dan daha fazla almak istenirse**: Max limit UI'da gösterilir, "İksir Envanteri Dolu" uyarısı.

- **If pet HP = 0 iken pet ekipman çıkarılmak istenirse**: Koleksiyon ekranından her zaman takılıp çıkarılabilir. Pet HP = 0 ekipman erişimini engellemez.

- **If Büyük İksir savaşta kullanılırken oyuncu tam HP'deyse**: Onay dialog: "Oyuncu HP Zaten Dolu — Yine de kullan?" İksir sarf edilir.

- **If tüm ekipman slotları boşsa**: effective_ATK = base_class_ATK, tüm ekipman bonusları 0. Savaş yine çalışır — ekipmansız da oynanabilir, ama zorlanır.

- **If bir slot tipi için envanterde hiç uyumlu item yoksa**: Slot seçim ekranı "Bu slot için envanterde ekipman yok — Dükkan'a git" mesajı gösterir.

## Dependencies

### Upstream

| Sistem | Veri / Arayüz |
|--------|--------------|
| **Savaş Sistemi** | `effective_ATK`, `effective_DEF`, `effective_HP`, `effective_SPD` okur |
| **Hasar Hesaplama** | Formüllere ekipman dahil efektif statlar beslenir |
| **Loot / Ödül Sistemi** | Düşen ekipmanı envantere ekler |
| **Ekonomi** | Dükkan alım/satım altın işlemleri |
| **Kaydetme / Yükleme** | Tüm slotlar ve envanter persist edilir |
| **Oyuncu Sınıf Sistemi** | `base_HP`, `base_ATK`, `base_DEF`, `base_SPD` — ekipman bu değerlere eklenir |
| **Pet/Canavar Veritabanı** | Pet `base_ATK`, `base_SPD` — pet ekipmanı bu değerlere eklenir |

### Downstream

| Sistem | Etki |
|--------|------|
| **Savaş UI** | İksir butonunu render eder, oyuncu stat barlarını gösterir |
| **Koleksiyon / Envanter UI** | Pet ekipman slotlarını Pet Detay'da gösterir |
| **Oyuncu Profil UI** | Oyuncu 11 slot ekranını gösterir |
| **Dükkan** | Ekipman satış/alış için envanter arayüzü sağlar |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `silah_ATK_B` | +60 | +45–+80 | Savaşçı olmayan sınıflar anlamsız | Silah hissedilmez |
| `zirh_DEF_B` | +55 | +40–+75 | Boss'lar trivial | Zırh işe yaramaz |
| `zirh_HP_B` | +220 | +160–+300 | Uzun savaşlar rutin | HP değerli değil |
| `bot_SPD_B` | +20 | +14–+28 | Hız çok dominant | Bot tercih edilmez |
| `kolye_HP_B` | +175 | +130–+240 | Kolye > Zırh | Kolye anlamsız |
| `heal_small_ratio` | 0.30 | 0.20–0.40 | İksir çok güçlü | Küçük iksir işe yaramaz |
| `heal_large_ratio` | 0.70 | 0.55–0.85 | Boss'lar trivial | Büyük iksir tercih edilmez |
| `auto_pot_threshold` | 0.25 | 0.15–0.35 | Oto-mod iksiri erken tüketir | İksir geç, oyuncu ölür |
| `equip_inventory_limit` | 50 | 30–100 | Yönetim anlamsız | Sık temizlik stresi |
| `boss_equip_drop_pool` | Oyuncu veya Pet slotu (random) | — | Belirli slot her zaman | Belirli slot hiç gelmez |
| `normal_stage_equip_drop_rate` | 0.05 | 0.02–0.12 | Dükkan gereksiz | Oyuncu hiç ekipman kazanamaz |

## Acceptance Criteria

1. **GIVEN** Warrior base_ATK=40, B tier Silah (+60) ve B tier Eldiven (+38) takılı, **WHEN** effective_ATK hesaplanırsa, **THEN** `effective_ATK ≥ 138` (diğer ATK slotları da varsa daha yüksek).

2. **GIVEN** Komutan Modu aktif, effective_ATK=138, **WHEN** saldırı yapılırsa, **THEN** `attacking_ATK = floor(138 × 1.30) = 179`.

3. **GIVEN** effective_HP = 400, mevcut HP = 80, Büyük İksir basıldı, **WHEN** uygulanırsa, **THEN** `new_HP = min(80 + floor(400 × 0.70), 400) = min(360, 400) = 360`.

4. **GIVEN** Pet Aksesuar C tier (SPD+9, enerji +5/tur), **WHEN** savaşta, **THEN** `pet_energy_per_turn = 25 + 5 = 30` → yetenek her ~3.3 turda bir gelir.

5. **GIVEN** Yüzük 1 slotuna item A takılı iken Yüzük 2'ye de item A takılmak istenirse, **THEN** "Bu item zaten Yüzük 1'de takılı" hatası, işlem iptal.

6. **GIVEN** Silah slotuna D tier Silah takılıyken C tier Silah takılmak istense, **WHEN** onaylanırsa, **THEN** D tier Silah envantere döner, C tier slota takılır.

7. **GIVEN** Savaş başladıktan sonra ekipman slotlarına dokunulursa, **WHEN** slot butonuna basılırsa, **THEN** slotlar disabled görünür, değiştirilemez.

8. **GIVEN** Tüm 11 oyuncu slotu boş, **WHEN** savaş başlarsa, **THEN** effective_ATK = base_class_ATK, effective_DEF = base_class_DEF, effective_HP = base_class_HP — savaş çalışır.

9. **GIVEN** Alan Patronu temizlendi, envanterde yer var, **WHEN** loot açılırsa, **THEN** B tier ekipman (oyuncu veya pet slotlarından birinde — random) garantili envantere eklenir.

10. **GIVEN** Envanter 50/50 dolu iken ekipman drop gelirse, **WHEN** loot ekranı açılırsa, **THEN** "Envanter Dolu" toast — item eklenmez.

11. **GIVEN** İksir stack Küçük İksir = 10/10 (tam dolu), **WHEN** dükkan'dan alınmak istenirse, **THEN** "İksir Envanteri Dolu (10/10)" uyarısı, işlem iptal.

12. **GIVEN** B tier tam set takılı (Formül 2 örneği), **WHEN** statlar gösterilirse, **THEN** effective_ATK = 246, effective_DEF = 189, effective_HP = 745, effective_SPD = 107 (Warrior base'i ile).

13. **GIVEN** Takılı ekipman satış listesinde aranırsa, **WHEN** liste açılırsa, **THEN** takılı ekipmanlar görünmez — yalnızca envanterdeki serbest itemlar listelenir.

14. **GIVEN** F tier Kask takılı (DEF+4, HP+20) ve F tier Zırh takılı (DEF+8, HP+30), **WHEN** effective_DEF ve effective_HP hesaplanırsa, **THEN** DEF'e +12 ve HP'ye +50 eklenir (diğer boş slotlar +0).

15. **GIVEN** Pet Aksesuar B tier (SPD+14, enerji +10/tur), **WHEN** savaşta, **THEN** `pet_energy_per_turn = 35` → yetenek her ~2.9 turda bir gelir.
