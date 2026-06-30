# Hasar Hesaplama (Damage Calculation)

> **Status**: Approved
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Cömert Zindan

## Overview

**Hasar Hesaplama**, savaştaki her saldırının final hasar değerini üreten merkezi hesaplama pipeline'ıdır. Saldıran canavarın ATK stat'ını, savunan canavarın DEF stat'ını, element çarpanını (avantaj/dezavantaj/nötr), sinerji bonuslarını ve kritik vuruş şansını katmanlı bir formülle birleştirerek tek bir tamsayı hasar değeri üretir. Bu değer Sağlık / Can Sistemi'ne `TakeDamage()` aracılığıyla iletilir.

Oyuncu açısından hasar hesaplaması, ekranda uçuşan hasar sayılarıdır — büyük kırmızı sayılar güç fantezisini besler, element avantajlı vuruşlarda sayıların %50 artması "akıllı strateji" tatmini verir, kritik vuruşlar ise "şanslı darbe!" heyecanı yaratır. "Güç Hisset" sütunu gereği oyuncu seviye atladıkça hasar sayıları belirgin şekilde büyümeli; "Cömert Zindan" gereği dezavantajlı element bile anlamlı hasar verebilmeli — hiçbir vuruş sıfır olmamalı.

MVP kapsamında temel ATK-DEF formülü, element çarpanı, sinerji bonusu entegrasyonu, kritik vuruş mekanikliği ve **iki hasar türü** (fiziksel / büyü) yer alır. Fiziksel hasar `defense_reduction_factor=2`, büyü hasarı `magic_defense_factor=4` kullanır — büyü, yüksek DEF hedeflere karşı daha etkilidir. Buff/debuff ve pasif yetenekler Tier 2+'da tanımlanacaktır. DoT (yanma/zehir) Oyuncu Sınıf Sistemi GDD'sinde tanımlanmıştır; bu pipeline'ı tetiklemez, bağımsız flat hasar olarak uygulanır.

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

Saldıranın effective_ATK ve savunanın effective_DEF değerleri, Canavar Veritabanı'ndaki base stat'lara sinerji bonusu uygulanarak elde edilir:

`effective_ATK = floor(base_ATK × (1 + synergy_atk_bonus))`
`effective_DEF = floor(base_DEF × (1 + synergy_def_bonus))`

Sinerji bonusu Element Sistemi GDD'sinde tanımlanmıştır. Hasar Hesaplama bu değerleri hazır alır — sinerji hesaplaması yapmaz.

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
| Savaşçı vs Common Tank | Fiziksel | 18 | 40 | floor(40/2)=20 | max(1,-2)=**1** |
| Büyücü vs Common Tank | Büyü | 45 | 40 | floor(40/4)=10 | **35** |
| Hırsız vs Destekçi | Fiziksel | 32 | 25 | floor(25/2)=12 | **20** |
| Şifacı vs Common Tank | Büyü | 20 | 40 | floor(40/4)=10 | **10** |
| Rare Saldırgan vs Common Tank | Fiziksel | 52 | 35 | floor(35/2)=17 | **35** |

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
| **Canavar Veritabanı** | ← okur | base_ATK, base_DEF (saldıran ve savunan) | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd} |
| **Element Sistemi** | ← okur | Element çarpanı (0.75–1.50), sinerji ATK/DEF bonusu | `GetElementMultiplier(atkElement, defElement)` → float; sinerji bonusu stat çözümleme katmanında |
| **Sağlık / Can Sistemi** | → gönderir | Final hasar değeri | `TakeDamage(targetId, amount)` — HP azaltma Sağlık'ın sorumluluğu |
| **Savaş Sistemi** | ← tetiklenir | Saldırı komutu | Savaş sistemi saldırı sırasını belirler, her saldırıda `CalculateDamage(attackerId, targetId, damageType)` çağırır |
| **Oyuncu Sınıf Sistemi** | ← okur | Saldıran birimin hasar türü | Büyücü/Şifacı → "magic"; Savaşçı/Hırsız → "physical" |
| **Düşman AI** | dolaylı | AI hasar tahminini kullanabilir | `EstimateDamage(attackerId, targetId)` → int (kritik hariç tahmini hasar) |
| **Savaş UI** | → gönderir | Hasar gösterim verileri | `OnDamageDealt` event → {final_damage, was_critical, element_info} |

**Veri akışı özeti**: Canavar Veritabanı + Element Sistemi → giriş sağlar. Savaş Sistemi → hesaplamayı tetikler. Bu sistem → Sağlık'a hasar, UI'a görüntüleme verisi gönderir.

## Formulas

### Formül 1: Temel Hasar (Base Damage)

`base_damage = max(1, effective_ATK - floor(effective_DEF / defense_reduction_factor))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif saldırı | effective_ATK | int | 15–184 | Sinerji dahil ATK |
| Efektif savunma | effective_DEF | int | 15–184 | Sinerji dahil DEF |
| Fiziksel DEF faktörü | defense_reduction_factor | int | 2 | Fiziksel hasar — DEF'in yarısı etkili |
| Büyü DEF faktörü | magic_defense_factor | int | 4 | Büyü hasar — DEF'in çeyreği etkili |
| Temel hasar | base_damage | int | 1–∞ | Minimum 1 garantili |

**Fiziksel örnekler**:
- Common Saldırgan (ATK=35) vs Common Tank (DEF=35) → 35 - floor(35/2) = 35-17 = **18**
- Common Destekçi (ATK=17) vs Rare Tank (DEF=52) → 17 - floor(52/2) = -9 → **1** (min clamp)

**Büyü örnekler**:
- Büyücü (ATK=45) vs Common Tank (DEF=40) → 45 - floor(40/4) = 45-10 = **35**
- Şifacı (ATK=20) vs Common Tank (DEF=40) → 20 - floor(40/4) = 20-10 = **10**

**Aynı ATK/DEF, iki tür karşılaştırması** (ATK=35, DEF=40):
- Fiziksel: 35 - 20 = **15**
- Büyü: 35 - 10 = **25** — yüksek DEF hedefe karşı %67 daha fazla

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
Legendary C Saldırgan (ATK=154) + 4'lü Ateş sinerji (+20% ATK) → effective_ATK = floor(154 × 1.20) = 184
vs Common Büyücü (DEF=17), avantajlı element, kritik vuruş:
1. base_damage = 184 - floor(17/2) = 184 - 8 = 176
2. element_damage = floor(176 × 1.5) = 264
3. crit_damage = floor(264 × 2.0) = 528
4. final_damage = max(1, 528) = **528**

**Tam pipeline örneği — Worst case**:
Common Destekçi (ATK=17), sinerji yok vs Rare Tank (DEF=52) + 3'lü sinerji (+10% DEF), dezavantajlı element:
1. effective_DEF = floor(52 × 1.10) = 57
2. base_damage = max(1, 17 - floor(57/2)) = max(1, 17 - 28) = max(1, -11) = 1
3. element_damage = floor(1 × 0.75) = 0
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
| **Canavar Veritabanı** | Sert | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd} — ATK ve DEF kullanılır | Olmadan hasar hesaplanamaz |
| **Element Sistemi** | Sert | `GetElementMultiplier(atkElement, defElement)` → float; `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus} | Olmadan element ve sinerji farkı yok |
| **Oyuncu Sınıf Sistemi** | Sert | Saldıran birimin `damageType` ("physical"/"magic") — sınıfa göre belirlenir | Olmadan magic/physical ayrımı yapılamaz |

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
- `defense_reduction_factor` × Canavar Veritabanı'ndaki arketip DEF yüzdeleri birlikte Tank'ın efektif dayanıklılığını belirler. Factor=2.0 ile Tank (DEF=35) 17.5 azaltır; Factor=3.0 ile sadece 11.7 azaltır.
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

1. **GIVEN** Common Saldırgan (ATK=35) vs Common Tank (DEF=35), nötr element, **WHEN** hasar hesaplanırsa, **THEN** base_damage = 35 - 17 = 18, element_damage = 18, final = 18 (kritik yoksa).

2. **GIVEN** Rare Saldırgan (ATK=52) vs Common Destekçi (DEF=25), avantajlı element, **WHEN** hasar hesaplanırsa, **THEN** base = 52-12=40, element = floor(40×1.5)=60.

3. **GIVEN** Common Destekçi (ATK=17) vs Rare Tank (DEF=52), dezavantajlı element, **WHEN** hasar hesaplanırsa, **THEN** base = max(1, 17-26)=1, element = floor(1×0.75)=0, final = max(1, 0) = 1.

4. **GIVEN** herhangi bir saldırı, **WHEN** final hasar hesaplanırsa, **THEN** sonuç asla 0'dan küçük olamaz — minimum 1.

5. **GIVEN** crit_chance=0.10, **WHEN** 1000 saldırı simüle edilirse, **THEN** kritik oranı %8–12 arasında (istatistiksel tolerans).

6. **GIVEN** element_damage=27, **WHEN** kritik vuruş gerçekleşirse, **THEN** crit_damage = floor(27×2.0) = 54.

7. **GIVEN** saldıran savaş dışı (HP=0), **WHEN** saldırı sırası gelirse, **THEN** CalculateDamage çağrılmaz.

8. **GIVEN** ATK=0 (veri hatası), **WHEN** hasar hesaplanırsa, **THEN** base_damage = max(1, 0-DEF/2) = 1, hata loglanır.

9. **GIVEN** DEF=0, **WHEN** hasar hesaplanırsa, **THEN** base_damage = ATK (tam hasar, azaltma yok).

10. **GIVEN** Legendary C Saldırgan (ATK=154) + sinerji (+20%) vs Common Büyücü (DEF=17), avantajlı, kritik, **WHEN** tam pipeline çalışırsa, **THEN** final = max(1, floor(floor(176×1.5)×2.0)) = 528.

11. **GIVEN** hasar hesaplanınca, **WHEN** OnDamageDealt event yayınlanırsa, **THEN** event final_damage, was_critical, element_info alanlarını içerir.

12. **GIVEN** EstimateDamage çağrılırsa, **WHEN** sonuç döndürülürse, **THEN** kritik vuruş dahil edilmez — deterministik sonuç.

13. **GIVEN** Büyücü (ATK=45) vs Common Tank (DEF=40), nötr, crit yok, **WHEN** büyü hasarı hesaplanırsa, **THEN** `max(1, 45 - floor(40/4)) = max(1, 35) = 35`.

14. **GIVEN** aynı senaryo fiziksel hasar olsaydı, **THEN** `max(1, 45 - floor(40/2)) = max(1, 25) = 25`. Büyü **%40 daha fazla** hasar verir.

## Open Questions

1. **Buff/debuff sistemi (Tier 2+)**: ATK/DEF geçici değişiklikleri (DEF kırma, ATK zayıflatma) formül pipeline'ına nasıl eklenir? → Oyuncu Sınıf Sistemi GDD'sinde durum etkileri tanımlandı; pipeline entegrasyonu Savaş Sistemi revizyonunda.

2. **AoE (alan hasarı) mekanikliği**: Büyücü Element Fırtınası her hedefe bağımsız pipeline çalıştırır (oyuncu-sinif-sistemi.md Kural 3). Genel AoE kural detayları Savaş Sistemi revizyonunda.

3. ~~**DoT (hasar zaman içinde)**~~ **ÇÖZÜLDÜ**: Yanma (Büyücü) ve Zehir (Hırsız) DoT bu pipeline'dan bağımsızdır — `floor(max_hp × dot_rate)` flat hasar, DEF bypass. Oyuncu Sınıf Sistemi GDD'sinde tanımlı (Kural 4, Formül 2).

4. ~~**Yetenek hasarı**~~ **ÇÖZÜLDÜ**: ATK'ya çarpan eklenir (`effective_ATK × skill_multiplier`). Yetenek Sistemi GDD'sinde (Formül 3) ve Oyuncu Sınıf Sistemi GDD'sinde (Kural 3) tanımlı. Bu pipeline `damageType` parametresiyle çalışır.
