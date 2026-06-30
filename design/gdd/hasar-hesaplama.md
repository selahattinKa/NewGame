# Hasar Hesaplama (Damage Calculation)

> **Status**: Revised
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Cömert Zindan

> ⚠️ **PROTOTYPE KAPSAM NOTU**: `element_multiplier` prototipte sabit **1.0** — element
> avantaj/dezavantaj hesabı yapılmaz. `GetElementMultiplier()` her zaman 1.0f döner.
> Pipeline adımı 3 (element_damage satırı) prototipte `element_damage = base_damage`
> olarak basitleşir. Tam avantaj sistemi MVP+'da etkinleştirilir.

## Overview

**Hasar Hesaplama**, savaştaki her saldırının final hasar değerini üreten merkezi hesaplama pipeline'ıdır. Saldıran tarafın effective_ATK'sını, savunan tarafın effective_DEF'ini, element çarpanını ve kritik vuruş şansını katmanlı bir formülle birleştirerek tek bir tamsayı hasar değeri üretir. Bu değer Sağlık / Can Sistemi'ne `TakeDamage()` aracılığıyla iletilir.

Savaşta **3 olası saldırı yönü** vardır:

| Saldıran | Savunan | Hasar Hedefi | ATK Kaynağı |
|----------|---------|-------------|-------------|
| **Oyuncu** | Düşman | Düşman HP | Sınıf base + Ekipman (Silah, Eldiven, Aksesuarlar) |
| **Pet** | Düşman | Düşman HP | Pet base_ATK + Pet Ekipman |
| **Düşman** | Oyuncu | Oyuncu HP | SG bazlı enemy_ATK (Keşif Alanı rubber-band ile) |

Oyuncu ana savaşçıdır — düşman oyuncuyu hedef alır. Defeat = Oyuncu HP = 0.

MVP kapsamında temel ATK-DEF formülü, element çarpanı (prototipte 1.0), kritik vuruş mekanikliği ve **iki hasar türü** (fiziksel / büyü) yer alır. Fiziksel hasar `defense_reduction_factor=2`, büyü hasarı `magic_defense_factor=4` kullanır — büyü, yüksek DEF hedeflere karşı daha etkilidir. Sinerji bonusu kaldırıldı (takım yok, tek pet). DoT (yanma/zehir) Oyuncu Sınıf Sistemi GDD'sinde tanımlanmıştır; bu pipeline'ı tetiklemez, bağımsız flat hasar olarak uygulanır.

## Player Fantasy

Oyuncu hasar hesaplamasını iki katmanda deneyimler. **Yüzeyde**, ekranda uçuşan hasar sayıları savaşın ritmini ve enerjisini oluşturur. Büyük kırmızı sayılar "güçlüyüm" hissini anında iletir; element avantajıyla %50 daha büyük sayılar görmek "doğru strateji seçtim" tatminidir. Kritik vuruşta sayının ikiye katlanması ve özel VFX patlaması — "vay, şanslı darbe!" heyecanı.

**Çekirdek duygu**: Yıkıcı güç ve stratejik ustalık. Oyuncu canavarının saldırısını izlerken "şu düşmanı kaç vuruşta düşürebilirim?" hesabını yapar — ve güçlendirme sonrası aynı düşmanı daha az vuruşta devirmek somut ilerleme kanıtıdır. Bir Tank canavarın düşük ama istikrarlı hasarı vs. bir Saldırganın yüksek ama savunmasız hasarı — arketip farkı hasar sayılarında hissedilir.

**Büyüme tatmini**: Seviye atladıkça ATK büyür, hasar sayıları gözle görülür şekilde artar. Eskiden 5 vuruşta düşen düşman artık 2 vuruşta düşer — "ne kadar güçlendim" anı. Evrim ile stat havuzu %40 artınca hasar farkı dramatik olur.

**Negatif fantazi (kaçınılacak)**: Sıfır hasar asla görülmemeli — dezavantajlı element + yüksek savunma durumunda bile minimum 1 hasar garantisi. "Hiçbir şey yapamıyorum" frustrasyonu, "Güç Hisset" sütununu doğrudan ihlal eder. DEF asla hasarı tamamen sıfırlamamalı — azaltmalı ama ortadan kaldırmamalı.

**Pillar bağlantısı**: "Güç Hisset" — büyüyen hasar sayıları gücün en görünür ifadesi. "Cömert Zindan" — dezavantajlı element bile anlamlı hasar verir, cezalandırıcı değil. "Senin Tempon" — otofarm'da hasar otomatik hesaplanır, komutan modunda oyuncu element avantajını bilinçli kullanır.

## Detailed Rules

### Core Rules

**Kural 1 — Hasar Pipeline'ı (Hesaplama Sırası)**

Her saldırı aşağıdaki sırayla hesaplanır:

```
1. Stat Çözümleme  →  effective_ATK, effective_DEF (sinerji dahil)
2. Temel Hasar      →  base_damage = max(1, effective_ATK - floor(effective_DEF / 2))
3. Element Çarpanı  →  element_damage = floor(base_damage × element_multiplier)
4. Kritik Vuruş     →  crit_roll → floor(element_damage × crit_multiplier) veya element_damage
5. Final Clamp      →  final_damage = max(1, result)
6. Uygulama         →  TakeDamage(targetId, final_damage) → Sağlık/Can Sistemi
```

Her adım sıralıdır — bir önceki adımın çıktısı sonrakine giriş olur.

**Kural 2 — Stat Çözümleme (Adım 1)**

effective_ATK ve effective_DEF, saldıran/savunan tarafın kimliğine göre farklı kaynaklardan derlenir. Hasar Hesaplama bu değerleri hazır alır — kaynak sistemler tarafından önceden hesaplanmış gelir.

**Oyuncu saldırırken (Oyuncu → Düşman):**
```
effective_ATK = base_class_ATK
    + silah_ATK + eldiven_ATK
    + yuzuk1_ATK + yuzuk2_ATK + kupe1_ATK + kupe2_ATK

// Komutan Modu:
attacking_ATK = floor(effective_ATK × 1.30)
// Otofarm Modu:
attacking_ATK = effective_ATK
```

**Pet saldırırken (Pet → Düşman):**
```
effective_pet_ATK = pet.base_ATK + pet_silah_ATK + pet_aksesuar_ATK
attacking_ATK = effective_pet_ATK   // Komutan Modu bonusu pet'e uygulanmaz
```

**Düşman saldırırken (Düşman → Oyuncu):**
```
attacking_ATK = enemy.base_ATK × rubber_band_factor
// rubber_band_factor: Keşif Alanı GDD — player_SG/stage_SG, clamp(0.70, 1.30)
```

**Oyuncu savunurken (DEF):**
```
effective_DEF = base_class_DEF
    + kask_DEF + zirh_DEF + pantalon_DEF + eldiven_DEF + bot_DEF
    + kolye_DEF + yuzuk1_DEF + yuzuk2_DEF
```

**Düşman savunurken (DEF):**
```
effective_enemy_DEF = enemy.base_DEF × rubber_band_factor
```

Ekipman bonusları Ekipman Sistemi GDD'sinde (Formül 1) tanımlıdır. Komutan Modu çarpanı Savaş Sistemi GDD'sinde (Formül 2) tanımlıdır.

**Kural 3 — Temel Hasar (Adım 2)**

`base_damage = max(1, effective_ATK - floor(effective_DEF / defense_reduction_factor))`

`defense_reduction_factor` hasar türüne göre belirlenir:

| Hasar Türü | defense_reduction_factor | Kullananlar | Etki |
|------------|--------------------------|-------------|------|
| **Fiziksel** | 2 | Savaşçı, Hırsız, fiziksel pet arketipleri | DEF'in yarısı hasar azaltır |
| **Büyü** | 4 | Büyücü, Şifacı, büyü pet arketipleri | DEF'in çeyreği hasar azaltır — zırha daha az takılır |

- DEF asla hasarı tamamen sıfırlamaz — `max(1, ...)` garantisi
- Hasar türü saldıran birimin sınıfı/arketipi tarafından belirlenir; savunanın tipi fark etmez
- `CalculateDamage()` çağrısında `damageType` parametresi geçirilir

| Senaryo | Tür | ATK | DEF | DEF/factor | Base Damage |
|---------|-----|-----|-----|------------|-------------|
| Savaşçı (ekipman yok) → Düşman | Fiziksel | 40 | 30 | floor(30/2)=15 | **25** |
| Savaşçı (D Silah+20) → Düşman | Fiziksel | 60 | 30 | floor(30/2)=15 | **45** |
| Büyücü → Düşman (yüksek DEF) | Büyü | 55 | 40 | floor(40/4)=10 | **45** |
| Savaşçı → Düşman (yüksek DEF) | Fiziksel | 40 | 80 | floor(80/2)=40 | max(1,-0)=**1** |
| Düşman → Oyuncu (DEF=35) | Fiziksel | 50 | 35 | floor(35/2)=17 | **33** |
| Düşman → Oyuncu (B Zırh, DEF=90) | Fiziksel | 50 | 90 | floor(90/2)=45 | **5** |
| Pet → Düşman | Fiziksel | 30 | 25 | floor(25/2)=12 | **18** |

**Kural 4 — Element Çarpanı (Adım 3)**

`element_damage = floor(base_damage × element_multiplier)`

Element çarpanı Element Sistemi GDD'sinden alınır: `GetElementMultiplier(attackerElement, defenderElement)`.

| Etkileşim | Çarpan | Örnek (base=18) |
|-----------|--------|-----------------|
| Avantajlı | 1.5x | floor(18 × 1.5) = 27 |
| Nötr | 1.0x | 18 |
| Dezavantajlı | 0.75x | floor(18 × 0.75) = 13 |

**Kural 5 — Kritik Vuruş (Adım 4)**

Her saldırıda kritik vuruş şansı kontrol edilir:

`crit_roll = random(0.0, 1.0) < crit_chance`

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| `crit_chance` | 0.10 | %10 sabit şans (MVP'de stat'a bağlı değil) |
| `crit_multiplier` | 2.0 | Kritik hasar = normal hasar × 2 |

- Kritik vuruş element çarpanından **sonra** uygulanır
- `crit_damage = floor(element_damage × crit_multiplier)`
- Kritik vuruş olmazsa `crit_damage = element_damage`
- Kritik vuruş VFX ve ses efekti tetikler (Visual/Audio bölümünde)

**Kural 6 — Final Hasar Clamp (Adım 5)**

`final_damage = max(1, crit_damage)`

Her koşulda minimum 1 hasar garantisi. Bu kural "Güç Hisset" sütununun temel güvencesidir — hiçbir saldırı boşa gitmez.

**Kural 7 — Hasar Uygulama (Adım 6)**

Final hasar değeri Sağlık / Can Sistemi'ne `TakeDamage(targetId, final_damage)` aracılığıyla iletilir. Hasar Hesaplama, HP azaltmayı yapmaz — sadece sayıyı üretir.

Aynı anda Savaş UI'a hasar bilgisi gönderilir:
```
OnDamageDealt → {
  attackerId, targetId,
  final_damage, base_damage,
  element_multiplier, was_critical,
  was_advantage, was_disadvantage
}
```

### States and Transitions

Hasar Hesaplama durum makinesi değildir — stateless bir hesaplama pipeline'ıdır. Her saldırı bağımsız hesaplanır; önceki saldırılardan taşınan durum yoktur.

Tek "durum" kavramı: kritik vuruş olasılık dağılımı. MVP'de bu sabit %10'dur ve önceki vuruşlardan etkilenmez (her vuruş bağımsız).

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Oyuncu Sınıf Sistemi** | ← okur | Oyuncunun `base_class_ATK`, `base_class_DEF`, hasar türü (physical/magic) | `GetPlayerBaseStats()` → {atk, def, hp, spd}; sınıf hasar türü |
| **Ekipman Sistemi** | ← okur | Tüm ekipman bonusları (effective_ATK ve effective_DEF hesabına girer) | `GetEffectivePlayerStats()` → {effective_ATK, effective_DEF, effective_HP} |
| **Pet/Canavar Veritabanı** | ← okur | Pet `base_ATK`, `base_DEF` (koleksiyondaki pet statları) | `GetPetBaseStats(petId, level)` → {atk, def, spd} |
| **Keşif Alanı** | ← okur | Düşman `enemy_ATK`, `enemy_DEF` (rubber-band ile ayarlanmış) | `GetEnemyStats(stageId)` → {atk, def, hp, spd} |
| **Element Sistemi** | ← okur | Element çarpanı (prototipte sabit 1.0) | `GetElementMultiplier(atkElement, defElement)` → float |
| **Sağlık / Can Sistemi** | → gönderir | Final hasar değeri | `TakeDamage(targetId, amount)` — HP azaltma Sağlık'ın sorumluluğu |
| **Savaş Sistemi** | ← tetiklenir | Saldırı komutu | `CalculateDamage(attackerId, targetId, damageType)` her saldırıda çağırılır |
| **Düşman AI** | dolaylı | AI hasar tahmini | `EstimateDamage(attackerId, targetId)` → int (kritik hariç) |
| **Savaş UI** | → gönderir | Hasar gösterim verileri | `OnDamageDealt` event → {final_damage, was_critical, element_info} |

**Veri akışı özeti**: Oyuncu Sınıf Sistemi + Ekipman Sistemi (oyuncu statları), Pet Veritabanı (pet statları), Keşif Alanı (düşman statları) → giriş sağlar. Bu sistem → Sağlık'a hasar, UI'a görüntüleme gönderir.

## Formulas

### Formül 1: Temel Hasar (Base Damage)

`base_damage = max(1, effective_ATK - floor(effective_DEF / defense_reduction_factor))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif saldırı | effective_ATK | int | 1–∞ | Oyuncu: sınıf base + ekipman; Pet: base_ATK + pet ekipman; Düşman: SG bazlı |
| Efektif savunma | effective_DEF | int | 1–∞ | Oyuncu: sınıf base + ekipman; Düşman: SG bazlı |
| Fiziksel DEF faktörü | defense_reduction_factor | int | 2 | Fiziksel hasar — DEF'in yarısı etkili |
| Büyü DEF faktörü | magic_defense_factor | int | 4 | Büyü hasar — DEF'in çeyreği etkili |
| Temel hasar | base_damage | int | 1–∞ | Minimum 1 garantili |

**Fiziksel örnekler — Oyuncu → Düşman**:
- Savaşçı (effective_ATK=52, D Silah dahil) → Düşman (DEF=30): 52 - floor(30/2) = 52-15 = **37**
- Savaşçı Komutan Modu (attacking_ATK=67) → Düşman (DEF=30): 67 - 15 = **52**
- Savaşçı (ATK=40) → Yüksek DEF Düşman (DEF=90): 40 - floor(90/2) = 40-45 = -5 → **1** (min clamp)

**Büyü örnekler — Büyücü → Düşman**:
- Büyücü (effective_ATK=55) → Düşman (DEF=40): 55 - floor(40/4) = 55-10 = **45**
- Şifacı (ATK=35) → Düşman (DEF=40): 35 - floor(40/4) = 35-10 = **25**

**Düşman → Oyuncu**:
- Düşman (ATK=50) → Oyuncu (effective_DEF=35): 50 - floor(35/2) = 50-17 = **33**
- Düşman (ATK=50) → Oyuncu B Zırh (effective_DEF=90): 50 - floor(90/2) = 50-45 = **5**

**Pet → Düşman**:
- Pet (effective_pet_ATK=42, D tier Silah dahil) → Düşman (DEF=25): 42 - floor(25/2) = 42-12 = **30**

**Aynı ATK (55), iki hasar türü, yüksek DEF düşman (DEF=60)**:
- Fiziksel: 55 - floor(60/2) = 55-30 = **25**
- Büyü: 55 - floor(60/4) = 55-15 = **40** — yüksek DEF hedefe karşı %60 daha fazla

### Formül 2: Element Hasarı

`element_damage = floor(base_damage × element_multiplier)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel hasar | base_damage | int | 1–∞ | Formül 1'den |
| Element çarpanı | element_multiplier | float | 0.75–1.50 | Element Sistemi'nden |
| Element hasarı | element_damage | int | 1–∞ | Floor ile yuvarlanır |

**Çıktı Aralığı**: 0 (min base × dezavantaj: floor(1 × 0.75) = 0 — Formül 4'te final min 1'e clamp edilir) ile 276 (floor(184 × 1.50))

**Örnek**: base_damage=18, avantajlı → floor(18 × 1.5) = floor(27) = **27**
**Örnek**: base_damage=18, dezavantajlı → floor(18 × 0.75) = floor(13.5) = **13**

### Formül 3: Kritik Vuruş

`final_after_crit = was_critical ? floor(element_damage × crit_multiplier) : element_damage`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Element hasarı | element_damage | int | 1–∞ | Formül 2'den |
| Kritik şansı | crit_chance | float | 0.10 | Sabit %10 (MVP) |
| Kritik çarpanı | crit_multiplier | float | 2.0 | Sabit 2x (MVP) |
| Kritik mi? | was_critical | bool | true/false | random < crit_chance |
| Kritik sonrası hasar | final_after_crit | int | 1–∞ | Floor ile yuvarlanır |

**Çıktı Aralığı**: element_damage (kritik değil) ile element_damage × 2 (kritik)

**Örnek**: element_damage=27, kritik → floor(27 × 2.0) = **54**
**Örnek**: element_damage=27, kritik değil → **27**

### Formül 4: Final Hasar (Tüm Pipeline)

`final_damage = max(1, final_after_crit)`

**Tam pipeline örneği — Best case**:
Savaşçı Komutan Modu, effective_ATK=80 → attacking_ATK = floor(80 × 1.30) = 104
vs Düşman (DEF=20), avantajlı element, kritik vuruş:
1. base_damage = 104 - floor(20/2) = 104 - 10 = 94
2. element_damage = floor(94 × 1.5) = 141
3. crit_damage = floor(141 × 2.0) = 282
4. final_damage = max(1, 282) = **282**

**Tam pipeline örneği — Worst case**:
Oyuncu (ATK=32, ekipman yok) vs Yüksek DEF Düşman (DEF=90), dezavantajlı element, kritik yok:
1. base_damage = max(1, 32 - floor(90/2)) = max(1, 32 - 45) = max(1, -13) = 1
2. element_damage = floor(1 × 0.75) = 0
3. crit_damage = 0 (kritik yok)
4. final_damage = max(1, 0) = **1**

### Formül 5: Hasar Tahmini (AI için)

`estimated_damage = max(1, floor((effective_ATK - floor(effective_DEF / 2)) × element_multiplier))`

Kritik vuruş dahil edilmez — deterministik tahmin. Düşman AI hedef seçimi için kullanır.

## Edge Cases

- **If effective_ATK == effective_DEF/2 (hasar tam 0)**: `max(1, ...)` garantisi devreye girer → 1 hasar.

- **If saldıran savaş dışıysa (HP=0)**: Saldırı gerçekleşmez — Savaş Sistemi savaş dışı canavarı saldırı sırasından çıkarır. Hasar Hesaplama çağrılmaz.

- **If savunan savaş dışıysa (HP=0)**: Hedef geçersiz — saldırı iptal, Savaş Sistemi yeni hedef seçer. Hasar Hesaplama çağrılmaz.

- **If element çarpanı × base_damage floor sonucu 0 olursa**: Formül 4'teki `max(1, ...)` ile 1'e clamp edilir. (ör: base_damage=1, dezavantaj 0.75x → floor(0.75) = 0 → final 1)

- **If kritik vuruş + element avantajı + sinerji birlikte çok yüksek hasar üretirse**: Hasar cap uygulanmaz — oyun cömert güç fantezisi hedefliyor. Oyuncu güçlüyse büyük sayılar görsün. Dengeleme stat havuzları ve düşman HP ile yapılır, hasar cap ile değil.

- **If ATK veya DEF 0 veya negatifse (veri hatası)**: 0 olarak işlenir. ATK=0 → base_damage = max(1, 0 - DEF/2) = 1. DEF=0 → base_damage = ATK (tam hasar). Hata loglanır.

- **If aynı anda birden fazla hedef varsa (AoE saldırı — ileride)**: MVP'de tüm saldırılar tek hedeftir. AoE mekanikliği Savaş Sistemi'nde tanımlanacak; bu durumda her hedef için pipeline bağımsız çalışır.

- **If crit_chance 0 veya negatifse**: Kritik vuruş asla gerçekleşmez. Formül normal devam eder.

- **If crit_chance >= 1.0 ise**: Her vuruş kritik. Geçerli — bazı buff/pasifler bunu tetikleyebilir (Tier 2+).

- **If savunanın DEF'i saldıranın ATK'sının 2 katından fazlaysa**: base_damage = max(1, ATK - DEF/2). DEF/2 > ATK olduğunda negatif sonuç → 1'e clamp. Bu durum nadirlik/seviye farkında meşru olabilir (Common vs Legendary).

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Oyuncu Sınıf Sistemi** | Sert | `GetPlayerBaseStats()` → {base_class_ATK, base_class_DEF, damageType ("physical"/"magic")} | Olmadan oyuncu statları ve hasar türü alınamaz |
| **Ekipman Sistemi** | Sert | `GetEffectivePlayerStats()` → {effective_ATK, effective_DEF, effective_HP} | Olmadan ekipman bonusları hesaba katılamaz |
| **Pet/Canavar Veritabanı** | Sert | `GetPetBaseStats(petId, level)` → {base_ATK, base_DEF, element} | Olmadan pet saldırı statları alınamaz |
| **Keşif Alanı** | Sert | `GetEnemyStats(stageId)` → {enemy_ATK, enemy_DEF, enemy_HP} — rubber-band uygulanmış | Olmadan düşman statları alınamaz |
| **Element Sistemi** | Sert | `GetElementMultiplier(atkElement, defElement)` → float | Olmadan element çarpanı hesaplanamaz |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Sağlık / Can Sistemi** | Sert | `TakeDamage(targetId, amount)` — final hasar iletilir | Olmadan hasar uygulanamaz |
| **Savaş Sistemi** | Sert | `CalculateDamage(attackerId, targetId, damageType)` — savaş döngüsünde çağırılır | Olmadan savaş mekaniği çalışmaz |
| **Düşman AI** | Yumuşak | `EstimateDamage(attackerId, targetId)` — hedef seçimi tahmini | AI daha akıllı hedef seçer; olmadan rastgele |
| **Savaş UI** | Yumuşak | `OnDamageDealt` event → hasar gösterim verileri | Olmadan hasar sayıları gösterilemez |

**Bağımlılık doğası**: İki upstream'den stat ve çarpan alır. Sağlık'a hasar, Savaş'a tetik, UI'a görüntüleme gönderir. Stateless pipeline — çift yönlü bağımlılık yok.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `defense_reduction_factor` (fiziksel) | 2 | 1.5–3.0 | Tank arketipi değersiz | Düşük ATK fiziksel birimler sürekli 1 hasar yapar |
| `magic_defense_factor` (büyü) | 4 | 3–6 | Fiziksel tamamen anlamsız (neden DEF yatırımı?) | Büyü fizikseldan farklılaşmaz |
| `crit_chance` | 0.10 | 0.05–0.20 | Çok sık kritik → "normal" vuruş anlamsız, hasar öngörülemez | Kritik çok nadir → sistem yokmuş gibi hisseder |
| `crit_multiplier` | 2.0 | 1.5–3.0 | Kritik çok güçlü → savaşlar şansa kalır | Kritik tatmin edici hissedilmez |
| `min_damage` | 1 | 1 | Sabit — 0'dan büyük olmalı | Sabit — 0 hasar "Güç Hisset" ihlali |

**Etkileşim Uyarıları**:
- `defense_reduction_factor` × Ekipman Sistemi'ndeki DEF değerleri birlikte oyuncunun efektif dayanıklılığını belirler. Factor=2.0 ile Oyuncu (effective_DEF=60) 30 azaltır; Factor=3.0 ile sadece 20 azaltır.
- `crit_chance` × `crit_multiplier` birlikte ortalama hasar çarpanını belirler: 1 + (0.10 × 1.0) = **1.10** (ortalamanın %10 üstünde). İkisini aynı anda artırmak hasar varyansını patlatabilir.
- Element Sistemi'ndeki `element_advantage_multiplier` (1.50) × `crit_multiplier` (2.0) birlikte best-case hasar çarpanını belirler: 1.5 × 2.0 = **3.0** (normal hasarın 3 katı). Bu üst sınır dengeleme testlerinde kontrol edilmeli.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Normal hasar | Beyaz hasar sayısı yukarı uçar | 0.5s | MVP |
| Element avantajlı hasar | Hasar sayısı %30 büyük + element renginde + "Etkili!" yazısı | 0.8s | MVP |
| Element dezavantajlı hasar | Hasar sayısı %20 küçük + gri + "Etkisiz..." yazısı | 0.5s | MVP |
| Kritik vuruş | Hasar sayısı 2x büyük + sarı/altın renk + yıldız patlaması VFX + ekran hafif sarsıntısı | 1.0s | MVP |
| Minimum hasar (1) | Küçük, soluk hasar sayısı | 0.3s | Nice-to-have |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Normal hasar | Kısa darbe sesi | Nötr | MVP |
| Element avantajlı hasar | Parlak "zing" + element efekti | Ödüllendirici | MVP |
| Kritik vuruş | Ağır darbe + cam kırılması hissi | Güçlü, tatmin edici | MVP |
| Minimum hasar (1) | Hafif "tink" | Zayıf ama var | Nice-to-have |

## UI Requirements

### Hasar Sayıları (Floating Damage Numbers)
- Hasar sayıları canavar sprite'ının üstünde floating text olarak gösterilir
- Normal hasar: beyaz, 24pt font
- Kritik hasar: altın/sarı, 36pt font, yıldız ikonu
- Element avantajlı: element renginde, 30pt font, "Etkili!" alt yazı
- Element dezavantajlı: gri, 20pt font, "Etkisiz..." alt yazı
- Birden fazla hasar aynı anda gösterilirse, sayılar üst üste binmemeli — hafif offset ile dağıtılır
- Minimum dokunma hedefi gerekli değil — hasar sayıları interaktif değil, salt görüntüleme

> **UX Flag — Hasar Hesaplama**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te (Pre-Production) `/ux-design` çalıştırarak savaş ekranı hasar gösterimi için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Savaşçı (base_class_ATK=40, ekipman yok), fiziksel, nötr element vs Düşman (DEF=30), **WHEN** hasar hesaplanırsa, **THEN** base_damage = 40 - 15 = 25, element_damage = 25, final = 25 (kritik yoksa).

2. **GIVEN** Savaşçı (D Silah+20, effective_ATK=60) vs Düşman (DEF=25), avantajlı element, **WHEN** hasar hesaplanırsa, **THEN** base = 60-12=48, element = floor(48×1.5)=72.

3. **GIVEN** Oyuncu (ATK=30, fiziksel) vs Yüksek DEF Düşman (DEF=80), dezavantajlı element, **WHEN** hasar hesaplanırsa, **THEN** base = max(1, 30-40)=1, element = floor(1×0.75)=0, final = max(1, 0) = 1.

4. **GIVEN** herhangi bir saldırı, **WHEN** final hasar hesaplanırsa, **THEN** sonuç asla 0'dan küçük olamaz — minimum 1.

5. **GIVEN** crit_chance=0.10, **WHEN** 1000 saldırı simüle edilirse, **THEN** kritik oranı %8–12 arasında (istatistiksel tolerans).

6. **GIVEN** element_damage=27, **WHEN** kritik vuruş gerçekleşirse, **THEN** crit_damage = floor(27×2.0) = 54.

7. **GIVEN** saldıran savaş dışı (HP=0), **WHEN** saldırı sırası gelirse, **THEN** CalculateDamage çağrılmaz.

8. **GIVEN** ATK=0 (veri hatası), **WHEN** hasar hesaplanırsa, **THEN** base_damage = max(1, 0-DEF/2) = 1, hata loglanır.

9. **GIVEN** DEF=0, **WHEN** hasar hesaplanırsa, **THEN** base_damage = ATK (tam hasar, azaltma yok).

10. **GIVEN** Savaşçı Komutan Modu (effective_ATK=80 → attacking_ATK=104) vs Düşman (DEF=20), avantajlı element, kritik vuruş, **WHEN** tam pipeline çalışırsa, **THEN** base=94, element=floor(94×1.5)=141, crit=floor(141×2.0)=282, final = **282**.

11. **GIVEN** hasar hesaplanınca, **WHEN** OnDamageDealt event yayınlanırsa, **THEN** event final_damage, was_critical, element_info alanlarını içerir.

12. **GIVEN** EstimateDamage çağrılırsa, **WHEN** sonuç döndürülürse, **THEN** kritik vuruş dahil edilmez — deterministik sonuç.

13. **GIVEN** Büyücü (effective_ATK=45) vs Düşman (DEF=40), nötr element, crit yok, **WHEN** büyü hasarı hesaplanırsa, **THEN** `max(1, 45 - floor(40/4)) = max(1, 35) = 35`.

14. **GIVEN** aynı ATK=45 ve Düşman DEF=40, fiziksel hasar olsaydı, **THEN** `max(1, 45 - floor(40/2)) = max(1, 25) = 25`. Büyü **%40 daha fazla** hasar verir.

## Open Questions

1. **Buff/debuff sistemi (Tier 2+)**: ATK/DEF geçici değişiklikleri (DEF kırma, ATK zayıflatma) formül pipeline'ına nasıl eklenir? → Oyuncu Sınıf Sistemi GDD'sinde durum etkileri tanımlandı; pipeline entegrasyonu Savaş Sistemi revizyonunda.

2. **AoE (alan hasarı) mekanikliği**: Büyücü Element Fırtınası her hedefe bağımsız pipeline çalıştırır (oyuncu-sinif-sistemi.md Kural 3). Genel AoE kural detayları Savaş Sistemi revizyonunda.

3. ~~**DoT (hasar zaman içinde)**~~ **ÇÖZÜLDÜ**: Yanma (Büyücü) ve Zehir (Hırsız) DoT bu pipeline'dan bağımsızdır — `floor(max_hp × dot_rate)` flat hasar, DEF bypass. Oyuncu Sınıf Sistemi GDD'sinde tanımlı (Kural 4, Formül 2).

4. ~~**Yetenek hasarı**~~ **ÇÖZÜLDÜ**: ATK'ya çarpan eklenir (`effective_ATK × skill_multiplier`). Yetenek Sistemi GDD'sinde (Formül 3) ve Oyuncu Sınıf Sistemi GDD'sinde (Kural 3) tanımlı. Bu pipeline `damageType` parametresiyle çalışır.
