# Düşman AI (Enemy AI)

> **Status**: Approved
> **Author**: user + game-designer, ai-programmer, systems-designer, qa-lead
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Cömert Zindan, Senin Tempon

## Overview

**Düşman AI**, zindan katlarındaki düşman canavarların savaş sırasında nasıl davrandığını belirleyen karar sistemidir. Teknik olarak, her düşman canavar ağırlıklı kurallarla hedef seçer, saldırı zamanlamasını yönetir ve (boss seviyesinde) özel saldırı paternleri uygular — Hasar Hesaplama'ya saldırı girdisi sağlar, Sağlık/Can Sistemi'nden hedef HP oranı okur, Element Sistemi'nden çarpan bilgisi alır. Oyuncu perspektifinden ise düşmanlar "canlı ve tehditkâr" hissetmeli ama asla "adaletsiz" değil. Sistem üç davranış katmanı sunar: **Basit AI** (normal düşmanlar — öngörülebilir ama çeşitli), **Taktik AI** (mini-boss'lar — zayıf hedefi fark eder), ve **Patron AI** (boss'lar — okunabilir saldırı paternleri, AoE yetenekleri). Bu kademelenme, oyuncunun çoğu savaşta rahat hissetmesini ama boss katlarında "dikkat etmeliyim" gerilimini yaşamasını sağlar. Düşman AI her iki savaş modunda (komutan ve otofarm) aynı şekilde çalışır — verimlilik farkı oyuncunun takımının karar kalitesinden gelir, düşmanın davranışından değil.

## Player Fantasy

Oyuncu Düşman AI'ı doğrudan bir "sistem" olarak deneyimlemez — hissettiği şey düşmanların **canlılığı ve tepkiselliğidir**. Normal katlarda düşmanlar hızla eriyip gider ve oyuncu "güçlüyüm, bunları eziyorum" fantezisini yaşar. Mini-boss katlarında hafif bir gerilim — "bu biraz daha akıllı, dikkat edeyim" — ama yine de güçlü hissetmeye devam eder. Boss katlarında ise düşmanın saldırı paterni okunabilir bir ritim oluşturur: "AoE geliyor, iyileşmem lazım" veya "3 turda güçlü saldırı yapacak, savunayım." Bu "öğrendim ve yendim" anı, Challenge estetiğinin doruğudur.

**Çekirdek duygu**: Güç fantezisi + okunabilir tehdit. Oyuncu çoğu zaman ordusunun düşmanları ezmesini izler (Güç Hisset). Boss'larda ise "bu düşman farklı, ama paternini çözdüm" tatmini yaşar. Düşmanlar asla "haksız" hissetmemeli — kayıplar oyuncunun hazırlıksızlığından gelmeli, AI'ın aşırı zekâsından değil.

**Negatif fantazi (kaçınılacak)**: AI asla oyuncunun en zayıf canavarını "hep" hedef almamalı (normal düşmanlar için). "Bu düşman beni okuyor" hissi, casual-friendly tasarıma aykırıdır. Boss'lar bile okunabilir paternlerle saldırmalı — "şok saldırı" değil, "hazırlan, geliyor" hissi.

**Pillar bağlantısı**: "Güç Hisset" — normal düşmanlar hızla düşer, oyuncu güçlü. "Cömert Zindan" — kayıp bile olsa ödül var, AI cezalandırıcı değil. "Senin Tempon" — otofarm'da AI aynı, komutan modunda oyuncu boss paternlerini okuyor.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — AI Davranış Katmanları**

Her düşman canavar aşağıdaki üç AI katmanından birine sahiptir:

| Katman | Kullanım | Hedef Seçimi | Yetenek Kullanımı | Özel Davranış |
|--------|----------|-------------|-------------------|---------------|
| **Basit AI** | Normal düşmanlar (kat 1-9, düşman dalgaları) | Rastgele ağırlıklı | Yok (sadece temel saldırı) | Yok |
| **Taktik AI** | Mini-boss (her 5. kat) | En düşük HP'li hedef | Temel yetenek (cooldown'lu) | Yok |
| **Patron AI** | Boss (her 10. kat) | Akıllı hedef seçimi | Saldırı paternleri + AoE | Faz geçişleri |

**Kural 2 — Basit AI (Normal Düşmanlar)**

Normal düşmanlar aşağıdaki ağırlıklı hedef seçim kuralını kullanır:

1. Oyuncunun hayatta olan canavarlarından birini seç
2. Hedef ağırlıkları:
   - Rastgele hedef: %60
   - En düşük HP oranına sahip hedef: %25
   - En yüksek ATK'lı hedef (tehdit): %15
3. Ağırlıklara göre hedef belirle ve temel saldırı yap
4. Yetenek kullanmaz — sadece normal saldırı

Bu dağılım düşmanların "biraz akıllı" ama öngörülemez hissetmesini sağlar. Oyuncu çoğu zaman rastgele hedeflenir (rahat hisseder), ama bazen zayıf canavarı hedef alınır (hafif gerilim).

**Kural 3 — Taktik AI (Mini-Boss)**

Mini-boss'lar (her 5. katta görünür) daha hedefli davranır:

1. Hedef seçimi:
   - En düşük HP oranına sahip hedef: %50
   - Destekçi arketip (varsa): %30
   - Rastgele: %20
2. Temel saldırı + 1 yetenek (cooldown: 3 tur)
3. Yetenek arketipe göre belirlenir:
   - Saldırgan mini-boss: Güçlü tek hedef saldırı (1.5x ATK) — **fiziksel hasar**
   - Tank mini-boss: Savunma duruşu (2 tur DEF ×2) — hasar yok
   - Büyücü mini-boss: Tüm hedefe yarım AoE (0.5x ATK herkese) — **büyü hasarı** (`magic_defense_factor=4`)

**Kural 4 — Patron AI (Boss)**

Boss'lar (her 10. katta) patern bazlı saldırı sistemi kullanır:

1. **Saldırı Döngüsü (3 turlu):**
   - Tur 1: Normal saldırı
   - Tur 2: Normal saldırı
   - Tur 3: **Güçlü Saldırı** — AoE (tüm hedeflere) veya tek hedef güçlü (boss tipine göre)
   - Döngü tekrar eder
   - Döngü sadece saldırı **tipini** belirler — hedef seçimi her turda aşağıdaki ağırlıklı sistem (madde 3) ile yapılır. AoE turlarında hedef seçimi atlanır (tüm hayatta olan hedeflere uygulanır).
2. **Faz Geçişi**: Boss HP'si %50'nin altına düşünce:
   - Saldırı hızı %20 artar (SPD bonus)
   - Güçlü Saldırı her 2 turda bir olur (3 yerine)
   - Görsel uyarı: Boss kırmızı parlar ("öfke modu")
3. Hedef seçimi:
   - %40 en düşük HP oranı
   - %35 en yüksek ATK (tehdit)
   - %25 rastgele
4. Boss'un özel yeteneği boss arketipine göre belirlenir:
   - AoE: Büyücü arketip (**büyü hasarı**, `magic_defense_factor=4`) / Saldırgan arketip (**fiziksel hasar**, `defense_reduction_factor=2`)
   - Güçlü tek hedef: Tank/Destekçi arketip (**fiziksel hasar**)

**Kural 5 — Düşman Takım Kompozisyonu**

Her zindan katının düşman takımı aşağıdaki kurallara göre oluşturulur:

- Normal katlar: 2-4 düşman canavar, kattaki zindan elementine ağırlıklı
- Mini-boss katları: 1 mini-boss + 1-2 normal düşman
- Boss katları: 1 boss (tek başına)
- Düşman sinerjisi: %15 olasılıkla düşman grubunda 2+ aynı element bulunur (Element Sistemi `enemy_synergy_frequency` tuning knob'u)
- Düşman seviyesi: kat numarası × zorluk çarpanı ile ölçeklenir (formüller Section D'de)
- Düşman güç skalası: normal düşmanlar kat seviyesine göre, mini-boss (orta güç), boss (alan doruk gücü)

**Kural 6 — Element Bilinci**

- **Basit AI**: Element-bilinçli değil. Hedef seçiminde element avantajı/dezavantajı dikkate alınmaz.
- **Taktik AI**: Element-bilinçli değil. Güçlü ama kör.
- **Patron AI**: Kısmi element bilinci — dezavantajlı hedefe saldırmaktan kaçınmaz, ama avantajlı hedefe %10 ek ağırlık verir.

Bu tasarım bilinçlidir: oyuncunun element avantajı her zaman ödüllendirilir ama düşmanın element bilgisi asla cezalandırıcı hissettirmez.

### States and Transitions

Her düşman canavar savaş sırasında aşağıdaki durumlardan birinde bulunur:

| Durum | Açıklama | Tetikleyici | Hedef Durum |
|-------|----------|-------------|-------------|
| **Beklemede** | Sıra düşmanda değil | SPD sırası gelir | Karar |
| **Karar** | Hedef ve aksiyon seçiliyor | AI kuralları çalışır | Saldırı / Yetenek |
| **Saldırı** | Normal saldırı uygulanıyor | Hasar hesaplanır | Beklemede |
| **Yetenek** | Özel yetenek uygulanıyor (Taktik/Patron) | Cooldown başlar | Beklemede |
| **Savunma Duruşu** | DEF artmış, saldırı yapmıyor (Tank mini-boss) | 2 tur sonra | Beklemede |
| **Öfke Modu** | Boss HP < %50, hızlanmış (Patron) | HP eşiği geçilir | Karar (hızlanmış) |
| **Savaş Dışı** | HP = 0, aktif değil | Hasar alır | — (kalıcı) |

Patron faz geçişi: `Karar → Öfke Modu` — HP %50 altına düşünce bir kez tetiklenir, geri dönülemez.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Hasar Hesaplama** | → sağlar / ← okur | Saldırı girdisi (ATK) sağlar; hasar pipeline'ını tetikler; Patron AI hedef seçimi için hasar tahmini okur | `Attack(attackerId, targetId)` → Hasar Hesaplama pipeline'ı; `EstimateDamage(attackerId, targetId)` → int (deterministik tahmin) |
| **Sağlık / Can Sistemi** | ← okur | Hedef HP oranı (hedef seçimi için) | `GetHPRatio(monsterId)` → float |
| **Element Sistemi** | ← okur | Element çarpanı (Patron AI element bilinci için) | `GetElementMultiplier(attackerElement, defenderElement)` |
| **Canavar Veritabanı** | ← okur | Düşman stat'ları, arketip, element | `GetMonsterIdentity()`, `GetBaseStats()` |
| **Savaş Sistemi** | ↔ çift yönlü | Savaş sırası alır, aksiyon sonucu bildirir | `GetTurnOrder()` ←, `ExecuteAction(action)` → |
| **Zindan Keşif** | ← okur | Kat numarası, düşman listesi | `GetFloorEnemies(floorNumber)` → düşman tanımları |
| **Savaş UI** | → sağlar | AI aksiyon bilgisi (animasyon tetiklemesi için) | `OnEnemyAction` event → {actionType, targetId, damage} |

*Specialist agents not consulted — Lean mode. Review manually before production.*

## Formulas

### Formül 1: Düşman Seviye Hesaplama (Enemy Level)

`enemy_level = floor(floor_number × difficulty_multiplier)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Kat numarası | floor_number | int | 1–100+ | Mevcut zindan katı |
| Bölge zorluk çarpanı | difficulty_multiplier | float | 1.0–2.0 | Bölgeye göre ölçekleme (MVP=1.0) |
| Düşman seviyesi | enemy_level | int | 1–200 | Düşmanın efektif seviyesi |

**Çıktı Aralığı**: 1 (Kat 1, MVP) – 200 (Kat 100, zor bölge). MVP kapsamında: 1–10.

**Örnek**: Kat 7, difficulty_multiplier=1.0 → `floor(7 × 1.0)` = **Lv 7**

### Formül 2: Düşman Stat Hesaplama (Enemy Stat)

Düşmanlar oyuncu canavarlarıyla aynı stat dağıtım sistemini kullanır (canavar-veritabani.md). Tek fark: düşman türüne göre stat havuzu ve seviye büyümesi uygulanır. **Düşmanlar her zaman Form A (evrim yok) ve ★0 (yıldız yok) olarak hesaplanır** — evrim ve yıldız güçlendirmesi oyuncuya özel mekaniktir. Bu, oyuncunun güçlendirme yatırımının her zaman karşılığını görmesini sağlar.

`enemy_stat = floor(floor(rarity_pool × archetype_pct) × (1 + growth_rate × (enemy_level - 1)))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Tür stat havuzu | rarity_pool | int | {100, 150, 185} | Düşman türüne göre: Normal=100, Mini-boss=150, Boss=185 |
| Arketip stat yüzdesi | archetype_pct | float | 0.15–0.35 | Canavar Veritabanı arketip tablosu |
| Büyüme oranı | growth_rate | float | 0.02–0.03 | Türe göre (registry: growth_rates) |
| Düşman seviyesi | enemy_level | int | 1–200 | Formül 1'den |
| Düşman bireysel stat | enemy_stat | int | 15–543 | HP/ATK/DEF/SPD (seviyeye göre ölçeklenir) |

**Çıktı Aralığı** (katmanlı):
- **MVP (Kat 1-10)**: 15–~80 (Boss Lv10 max)
- **Lv50 sınırı**: 15–195 (en yüksek arketip%, boss tipi)
- **Lv200 teorik max**: 15–543 (Boss Saldırgan ATK: floor(78 × 6.97))

**Örnek**: Kat 5 mini-boss (Saldırgan, Lv5):
- rarity_pool=150, archetype_pct=0.35 (ATK), growth_rate=0.025
- ATK = `floor(floor(150 × 0.35) × (1 + 0.025 × 4))` = `floor(52 × 1.10)` = **57**

### Formül 3: Ağırlıklı Hedef Seçimi (Target Weight Normalization)

```
weight_i = base_weight_i × condition_i
probability_i = weight_i / Σ(all weight_j)
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel ağırlık | base_weight_i | float | 0–60 | AI katmanına göre (Core Rules tablosu) |
| Koşul bayrak | condition_i | int | {0, 1} | Koşul geçerli mi? (ör: Destekçi yoksa 0) |
| Seçim olasılığı | probability_i | float | 0.0–1.0 | Hedefin seçilme olasılığı |

**Çıktı Aralığı**: Her hedef için 0.0–1.0, toplam her zaman 1.0.

**Patron AI element bilinci ek ağırlığı:**
`adjusted_weight_i = base_weight_i + (has_advantage_i × element_weight_bonus)`

`has_advantage_i` = 1 ise saldıran düşmanın hedef üzerinde element avantajı var, 0 değilse. `element_weight_bonus` = 10.

**Örnek** (Taktik AI, 3 canavar hayatta, destekçi yok):
- base_weights: lowest_HP=50, destekçi=30, random=20
- condition: lowest_HP=1, destekçi=**0**, random=1
- active: 50, 0, 20 → Toplam=70
- P(lowest_HP) = 50/70 = **%71.4**, P(random) = 20/70 = **%28.6**

### Formül 4: Mini-Boss Yetenek Hasarı

Mini-boss yeteneği, standart hasar formülüne arketip çarpanı uygular. `defense_reduction_factor` arketipe göre değişir:

**Saldırgan mini-boss (1.5x tek hedef) — fiziksel hasar:**
`skill_damage = floor(max(1, effective_ATK × 1.5 - floor(target_DEF / 2)) × element_multiplier)`

**Büyücü mini-boss (0.5x AoE, her hedefe ayrı) — büyü hasarı:**
`aoe_damage = floor(max(1, effective_ATK × 0.5 - floor(target_DEF / 4)) × element_multiplier)`

**Tank mini-boss (savunma duruşu, hasar yok):**
`buffed_DEF = effective_DEF × 2` (2 tur sürer)

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif ATK | effective_ATK | int | 15–543 | Mini-boss ATK (stat formula) |
| Hedef DEF | target_DEF | int | 15–499 | Hedef canavarın DEF'i |
| DEF faktörü (fiziksel) | defense_reduction_factor | int | 2 | Saldırgan mini-boss |
| DEF faktörü (büyü) | magic_defense_factor | int | 4 | Büyücü mini-boss |
| Element çarpanı | element_multiplier | float | {0.75, 1.0, 1.5} | Element avantaj/dezavantaj |
| Yetenek çarpanı | skill_multiplier | float | {0.5, 1.5} | Arketip yetenek çarpanı |
| Yetenek hasarı | skill_damage | int | 1–350 | Sonuç (min 1 garantili) |

**Çıktı Aralığı**: 1 – ~350 (Lv50 sınır). MVP'de max ~70.

**Örnek** (Kat 5 Saldırgan mini-boss, Lv5, ATK=57, vs DEF=25, nötr — fiziksel):
`floor(max(1, 57 × 1.5 - floor(25/2)) × 1.0)` = `floor(85.5 - 12)` = **73**

**Örnek** (Kat 5 Büyücü mini-boss, Lv5, ATK=57, vs DEF=25, nötr — büyü):
`floor(max(1, floor(57 × 0.5) - floor(25/4)) × 1.0)` = `floor(28 - 6)` = **22**

### Formül 5: Boss Güçlü Saldırı Hasarı

Boss güçlü saldırısı, normal hasar formülüne güçlü çarpan ekler. `defense_reduction_factor` boss arketipine göre değişir:

**Tek hedef güçlü saldırı — fiziksel hasar (Tank/Destekçi arketip):**
`strong_damage = floor(max(1, effective_ATK × strong_multiplier - floor(target_DEF / 2)) × element_multiplier)`

**AoE güçlü saldırı — fiziksel hasar (Saldırgan arketip):**
`strong_aoe_damage = floor(max(1, effective_ATK × aoe_multiplier - floor(target_DEF / 2)) × element_multiplier)`

**AoE güçlü saldırı — büyü hasarı (Büyücü arketip):**
`strong_aoe_damage = floor(max(1, effective_ATK × aoe_multiplier - floor(target_DEF / 4)) × element_multiplier)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Efektif ATK | effective_ATK | int | 15–543 | Boss ATK (stat formula) |
| Hedef DEF | target_DEF | int | 15–499 | Hedef DEF (oyuncu stat pipeline'ından) |
| DEF faktörü (fiziksel) | defense_reduction_factor | int | 2 | Tank/Destekçi ve Saldırgan arketip |
| DEF faktörü (büyü) | magic_defense_factor | int | 4 | Büyücü arketip |
| Element çarpanı | element_multiplier | float | {0.75, 1.0, 1.5} | Element çarpan |
| Güçlü saldırı çarpanı | strong_multiplier | float | 2.0 | Tek hedef güçlü çarpan |
| AoE çarpanı | aoe_multiplier | float | 0.75 | AoE güçlü çarpan |
| Güçlü hasar | strong_damage | int | 1–550 | Tek hedef sonuç |
| AoE hasar | strong_aoe_damage | int | 1–200 | Hedef başına AoE sonuç |

**Çıktı Aralığı**: Tek hedef 1–~550, AoE fiziksel 1–~200, AoE büyü 1–~215. MVP'de tek hedef max ~148, AoE fiziksel max ~48, AoE büyü max ~54.

**Örnek** (Kat 10 boss, Saldırgan, Lv10, ATK=80, vs DEF=25, nötr — fiziksel):
- Tek hedef: `floor(max(1, 80 × 2.0 - floor(25/2)) × 1.0)` = `floor(160 - 12)` = **148**
- AoE: `floor(max(1, 80 × 0.75 - floor(25/2)) × 1.0)` = `floor(60 - 12)` = **48**

**Örnek** (Kat 10 boss, Büyücü, Lv10, ATK=80, vs DEF=25, nötr — büyü):
- AoE: `floor(max(1, 80 × 0.75 - floor(25/4)) × 1.0)` = `floor(60 - 6)` = **54**

### Formül 6: Boss Öfke Modu Değişiklikleri

Boss HP < %50 altına düşünce "Öfke Modu" aktifleşir:

**Hız bonusu:**
`rage_SPD = floor(base_SPD × (1 + rage_spd_bonus))`

**Saldırı sıklığı:** Güçlü saldırı sıklığı `normal_strong_cd` → `rage_strong_cd`.

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel SPD | base_SPD | int | 15–195 | Boss normal SPD stat'ı |
| Öfke SPD bonusu | rage_spd_bonus | float | 0.20 | %20 hız artışı |
| Öfke SPD | rage_SPD | int | 18–234 | Öfke sonrası SPD |
| Normal güçlü CD | normal_strong_cd | int | 3 | Normal dönem güçlü saldırı sıklığı (tur) |
| Öfke güçlü CD | rage_strong_cd | int | 2 | Öfke dönem güçlü saldırı sıklığı (tur) |
| HP eşiği | hp_threshold | float | 0.50 | Öfke tetikleme eşiği |

**Çıktı Aralığı**: rage_SPD = 18–234. Güçlü saldırı sıklığı: 3 → 2 tur.

**Örnek** (Kat 10 boss, Tank, Lv10, base_SPD=46):
- rage_SPD = `floor(46 × 1.20)` = **55**
- Saldırı paterni: Normal→Normal→Güçlü → Normal→Güçlü→Normal→Güçlü...

## Edge Cases

- **If hedef seçiminde HP oranı eşitliği varsa (iki canavar aynı HP%)**: En düşük HP ağırlığı her ikisine eşit dağıtılır. Birden fazla aday arasında rastgele seçilir — deterministik kırılma kuralı yok. Bu, oyuncunun "düşman hep şunu hedef alıyor" hissetmesini engeller.

- **If hayatta tek oyuncu canavarı kaldıysa**: Tüm hedef seçim ağırlıkları tek hedefe çöker — `probability = 1.0`. AI katmanı fark etmez, seçim otomatik.

- **If Taktik AI'ın destekçi ağırlığı aktifken takımda destekçi yoksa**: `condition_i = 0` ile destekçi ağırlığı sıfırlanır. Kalan ağırlıklar Formül 3'e göre normalize edilir (ör: 50+20=70 → %71.4 lowest HP, %28.6 random). Hata loglanmaz — bu normal bir durum.

- **If mini-boss yeteneği cooldown'dayken sırası gelirse**: Normal saldırı yapar. Yetenek cooldown'ı tur bazlı sayılır — her tur 1 azalır. CD=0 olduğunda yetenek tekrar kullanılabilir.

- **If boss öfke modu tam sıra ortasında tetiklenirse (HP %50 altına düşer)**: Öfke modu **bir sonraki turda** aktifleşir, mevcut tur normal devam eder. Tetikleme anında görsel uyarı (kırmızı parlama) başlar — oyuncuya hazırlanma fırsatı verir. Bu, "Güç Hisset" sütunuyla uyumlu: oyuncu şok edilmez, uyarılır.

- **If boss güçlü AoE saldırısında hedeflerden biri savaş dışıysa**: Savaş dışı hedefler AoE'den etkilenmez. Hasar sadece hayatta olan hedeflere uygulanır — Sağlık/Can Sistemi GDD'sindeki kural ile tutarlı.

- **If düşman stat formülü 0 veya negatif üretirse (veri hatası)**: Minimum 1 olarak clamp edilir. ATK=0 durumunda düşman hasar veremez (hasar pipeline'da min 1 garantisi var ama saldırı gücü sıfıra yakın). Hata loglanır.

- **If difficulty_multiplier 0 veya negatifse (yapılandırma hatası)**: 1.0 olarak fallback. enemy_level hiçbir zaman 0 olamaz — minimum 1. Hata loglanır.

- **If tüm oyuncu canavarları savaş dışıysa ve AI sırası gelirse**: Savaş zaten kaybedilmiştir — Savaş Sistemi TPK (Total Party Kill) akışını tetikler. AI karar döngüsüne girmez.

- **If düşman takımında 2+ aynı element canavar varsa (sinerji aktif)**: Düşmanlar da oyuncu canavarlarıyla aynı sinerji kurallarından yararlanır — Element Sistemi GDD'sindeki sinerji tablosu uygulanır. `enemy_synergy_frequency` (%15) bu durumun ne sıklıkta oluşacağını kontrol eder.

- **If Patron AI'ın element bilinci avantajlı hedef bulamazsa**: `has_advantage_i = 0` tüm hedefler için. Ek ağırlık uygulanmaz, normal hedef seçim ağırlıkları geçerli. Patron AI nötr/dezavantajlı hedeflerden kaçınmaz — sadece avantajlıya hafif eğilim gösterir.

- **If mini-boss Büyücü AoE saldırısında tek hedef kaldıysa**: AoE hasarı tek hedefe tam uygulanır (`0.5x ATK` — azaltma yok). AoE'nin avantajı çoklu hedef; tek hedefe karşı normal saldırıdan zayıf kalır.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hasar Hesaplama** | Sert | `CalculateDamage(attackerId, targetId, damageType)` — AI saldırı kararı sonrası hasar pipeline'ını tetikler; `EstimateDamage(attackerId, targetId)` → int — Patron AI hedef seçiminde deterministik tahmin | Olmadan AI saldırıları hasar üretemez |
| **Oyuncu Sınıf Sistemi** | Yumuşak | Büyücü arketip düşmanlar için `damageType=magic`; diğer arketipler için `damageType=physical` — hasar türü kararı | Olmadan Büyücü düşmanlar yanlış DEF faktörü kullanır |
| **Sağlık / Can Sistemi** | Sert | `GetHPRatio(monsterId)` → float (0.0–1.0) — hedef seçiminde HP oranı; `IsAlive(monsterId)` → bool — hayatta olma kontrolü | Olmadan AI hedef seçimi yapamaz |
| **Element Sistemi** | Yumuşak | `GetElementMultiplier(attackerElement, defenderElement)` → float — Patron AI element-bilinçli hedef seçimi için | Olmadan Patron AI element avantajını görmez, rastgele seçer |
| **Canavar Veritabanı** | Sert | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd}; `GetMonsterIdentity(monsterId)` → {element, archetype, rarity} — düşman stat ve kimlik bilgisi | Olmadan düşman canavarlar oluşturulamaz |
| **Zindan Keşif** | Sert | `GetFloorEnemies(floorNumber)` → düşman listesi tanımları; kat numarası ve bölge bilgisi | Olmadan AI hangi düşmanların savaşacağını bilemez |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Savaş Sistemi** | Sert | `GetEnemyAction(enemyId)` → {actionType, targetId, skillId} — savaş döngüsünde düşman aksiyonu; `OnEnemyAction` event → animasyon tetiklemesi | Olmadan düşmanlar savaşta hiçbir şey yapmaz |
| **Savaş UI** | Yumuşak | `OnEnemyAction` event → {actionType, targetId, damage} — AI aksiyon bilgisi görsel gösterim için | Olmadan düşman aksiyonları gösterilmez ama savaş çalışır |

**Bağımlılık doğası**: 5 upstream'den veri alır (stat, HP, element, düşman listesi, hasar tahmin). Savaş Sistemi'a aksiyon kararı, UI'a görüntüleme bilgisi gönderir. Düşman AI'ın kendisi state tutmaz (boss öfke modu hariç) — her karar bağımsız hesaplanır.

**Bidirectional check**: Hasar Hesaplama GDD'sinde Düşman AI "yumuşak downstream" ✓. Sağlık/Can GDD'sinde Düşman AI "yumuşak downstream" ✓. Element Sistemi GDD'sinde Düşman AI "yumuşak downstream" ✓. Zindan Keşif GDD: ✅ Yazıldı — `GetFloorEnemies` arayüzü doğrulandı.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `difficulty_multiplier` | 1.0 | 0.8–2.0 | Düşmanlar çok güçlü → "Güç Hisset" ihlali | Düşmanlar çok zayıf → hiç tehdit yok |
| `simple_random_weight` | 60 | 40–80 | Basit AI çok rastgele → "aptal" hisseder | Çok öngörülebilir → hep aynı hedefe gider |
| `simple_lowHP_weight` | 25 | 10–40 | Zayıf hedefi çok hedef alır → casual oyuncuyu sinirlendirir | Zayıf canavar hiç hedef alınmaz |
| `simple_highATK_weight` | 15 | 5–25 | Tehdit önceliği çok yapıyor → taktik AI ile fark yok | Tehdit hedefleme yok |
| `tactical_lowHP_weight` | 50 | 30–70 | Mini-boss çok hedefli → "haksız" hissi | Hedefli davranış fark edilmez |
| `tactical_healer_weight` | 30 | 15–50 | Destekçi her zaman hedef → "düşman okuyor" hissi | Destekçi göz ardı edilir |
| `tactical_random_weight` | 20 | 10–40 | Çok rastgele → "aptal" mini-boss | Çok öngörülebilir → sürpriz yok |
| `tactical_skill_cd` | 3 | 2–5 | Yetenek çok sık → mini-boss çok güçlü | Yetenek çok seyrek → normal düşman gibi |
| `boss_lowHP_weight` | 40 | 25–55 | Boss çok hedefli → "adaletsiz" | Hedef seçimi rastgele → tehditkâr değil |
| `boss_highATK_weight` | 35 | 20–50 | Saldırganları hep hedef alır → saldırgan kullanılamaz | Saldırganlar çok güvenli |
| `boss_random_weight` | 25 | 10–40 | Çok rastgele → tehditkâr değil | Çok öngörülebilir |
| `boss_element_weight_bonus` | 10 | 5–20 | Element bilinci çok güçlü → cezalandırıcı | Element bilinci hissedilmez |
| `strong_multiplier` | 2.0 | 1.5–3.0 | Güçlü saldırı tek vuruşta öldürür | Güçlü saldırı fark edilmez |
| `aoe_multiplier` | 0.75 | 0.5–1.0 | AoE çok güçlü → tüm takım erir | AoE çok zayıf |
| `rage_spd_bonus` | 0.20 | 0.10–0.40 | Öfke modu çok hızlı → tepki verilemez | Hız farkı hissedilmez |
| `rage_strong_cd` | 2 | 2–3 | Her tur güçlü saldırı → çok agresif | Normalden farkı yok |
| `hp_threshold` (öfke) | 0.50 | 0.30–0.70 | Öfke çok erken → savaşın çoğu öfke modunda | Çok geç → neredeyse tetiklenmez |
| `enemy_synergy_frequency` | 0.15 | 0.05–0.30 | Düşman sinerjisi çok sık → adaletsiz | Sinerjisi hiç yok |
| `mini_boss_skill_mult` (Saldırgan) | 1.5 | 1.2–2.0 | Tek vuruşta öldürür | Yetenek fark edilmez |
| `mini_boss_aoe_mult` (Büyücü) | 0.5 | 0.3–0.8 | AoE çok güçlü | AoE çok zayıf |
| `mini_boss_def_mult` (Tank) | 2.0 | 1.5–3.0 | Hasar almaz → uzun ve sıkıcı savaş | Savunma duruşu fark edilmez |

**Etkileşim Uyarıları**:
- `difficulty_multiplier` × `rarity_stat_pools` (registry) birlikte düşman güç eğrisini belirler. İkisini aynı anda artırmak zorluk patlar.
- `strong_multiplier` × boss ATK stat'ı birlikte tek vuruş öldürme potansiyelini belirler. Lv10 boss (ATK=80) × 2.0 = 160 hasar — Zayıf Büyücü Lv1 (HP=18) için overkill. Bu beklenen davranış — boss'un güçlü saldırısı tehditkâr olmalı.
- `aoe_multiplier` × `max_team_size` (4) birlikte toplam AoE hasarını belirler: 48 × 4 = 192 toplam (vs tek hedef 148). AoE toplam hasarı tek hedefe yakın ama dağıtık.
- `enemy_synergy_frequency` Element Sistemi GDD'sinde de tanımlı — tek kaynak Element Sistemi, bu GDD referans alır.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Normal düşman saldırısı | Düşman sprite kısa ileri hareket + saldırı çizgisi efekti | 0.4s | MVP |
| Mini-boss yetenek (Saldırgan — güçlü tek hedef) | Kırmızı parıltı + büyük saldırı çizgisi + ekran hafif sarsıntı | 0.6s | MVP |
| Mini-boss yetenek (Büyücü — AoE) | Geniş daire dalga efekti, tüm hedeflere sıçrayan parçacıklar | 0.8s | MVP |
| Mini-boss yetenek (Tank — savunma duruşu) | Mavi kalkan parıltısı, DEF artış ikonu sprite üstünde | 0.5s aktifleşme + 2 tur sürekli | MVP |
| Boss normal saldırı | Düşmandan büyük ve belirgin saldırı animasyonu | 0.5s | MVP |
| Boss güçlü saldırı (tek hedef) | Sarı/turuncu güçlü parıltı + büyük darbe efekti + ekran sarsıntısı | 0.8s | MVP |
| Boss güçlü AoE saldırısı | Ekran çapında dalga + tüm hedeflere eş zamanlı darbe efekti | 1.0s | MVP |
| Boss öfke modu aktifleşme | Boss sprite kırmızı parlar + öfke aura efekti (sürekli) + ekran kısa flash | 1.2s geçiş, sonra sürekli aura | MVP |
| Boss güçlü saldırı ön uyarısı | Tur 2'de hafif parıltı → Tur 3'te "!" uyarı ikonu (boss üstünde) | 0.3s (her turda) | MVP |
| Düşman savaş dışı kalma | Düşman sprite soluklaşma + kaybolma animasyonu | 0.5s | MVP |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Normal düşman saldırısı | Kısa darbe/vuruş sesi | Nötr, hafif | MVP |
| Mini-boss yetenek kullanımı | Yetenek "charge" sesi + güçlü darbe | Tehditkâr ama adil | MVP |
| Boss normal saldırı | Ağır darbe sesi | Güçlü, dikkat çekici | MVP |
| Boss güçlü saldırı | Uzun charge sesi + patlama | Dramatik ama okunabilir | MVP |
| Boss AoE saldırısı | Genişleyen dalga sesi + çoklu darbe | Kapsayıcı, büyük | MVP |
| Boss öfke modu aktifleşme | Derin kükreme/gürültü + ritim değişikliği | Gerilim artışı — "dikkat!" | MVP |
| Boss güçlü saldırı ön uyarısı | Kısa "ding-ding" uyarı sesi (yükselen) | Uyarıcı, acil değil | MVP |
| Düşman savaş dışı | Kısa "düşme" sesi | Tatmin edici ama abartısız | MVP |

> **Asset Spec Flag**: Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:dusuman-ai` çalıştırarak düşman VFX ve ses asset spesifikasyonları üretilebilir.

## UI Requirements

### Savaş Ekranı — Düşman Aksiyon Gösterimi
- Düşmanın saldırı hedefi, saldırı başlamadan önce kısa bir "hedef çizgisi" ile gösterilir (düşmandan hedefe ince ok, 0.2s)
- Mini-boss yetenek kullanımında yetenek adı düşmanın üstünde kısa süre belirir (ör: "Güçlü Saldırı!", "AoE Dalga!", "Savunma Duruşu!")
- Boss saldırı paterni uyarısı: güçlü saldırıdan 1 tur önce boss'un üstünde "!" ikonu — oyuncuya hazırlanma süresi verir
- Boss öfke modu aktifleştiğinde "ÖFKE!" yazısı ekranın ortasında kısa flash (0.5s), ardından boss sprite'ında sürekli kırmızı aura
- Düşman sıra göstergesi: savaş sırası barında düşman ikonları gösterilir (SPD bazlı) — oyuncu hangi düşmanın ne zaman saldıracağını görebilir

### Düşman Bilgi Paneli
- Düşmana dokunulduğunda (long press) bilgi popup: düşman adı, element ikonu, arketip, HP barı, AI katmanı göstergesi (normal/mini-boss/boss)
- Boss'lar için ek bilgi: öfke eşiği göstergesi (HP barında %50 çizgisi), saldırı döngüsü sayacı (mevcut tur/3)
- Minimum dokunma hedefi: 44×44 dp (mobil erişilebilirlik)

### Kat Önizlemesi — Düşman Bilgisi
- Zindan katına girmeden önce düşman sayısı, element dağılımı ve AI katmanı (normal/mini-boss/boss) gösterilir
- Mini-boss ve boss katları özel ikon ile işaretlenir (kılıç ikonu mini-boss, taç ikonu boss)

> **UX Flag — Düşman AI**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te (Pre-Production) `/ux-design` çalıştırarak savaş ekranı düşman aksiyon gösterimi, düşman bilgi paneli ve kat önizlemesi için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Basit AI düşmanı ve oyuncunun 3 canavarı hayatta (A: HP%=100, B: HP%=40, C: ATK=52), **WHEN** hedef seçim ağırlıkları hesaplanırsa, **THEN** P(rastgele) = 60/100 = %60, P(en düşük HP — B) = 25/100 = %25, P(en yüksek ATK — C) = 15/100 = %15.

2. **GIVEN** Taktik AI mini-boss ve 3 canavar hayatta (Tank, Destekçi, Saldırgan), **WHEN** hedef seçim ağırlıkları hesaplanırsa, **THEN** P(en düşük HP) = %50, P(Destekçi) = %30, P(rastgele) = %20.

3. **GIVEN** Taktik AI mini-boss ve 3 canavar hayatta, hiçbiri Destekçi değil, **WHEN** ağırlıklar hesaplanırsa, **THEN** destekçi ağırlığı sıfırlanır, P(en düşük HP) = 50/70 = %71.4, P(rastgele) = 20/70 = %28.6.

4. **GIVEN** Patron AI boss ve 3 canavar, boss'un hiçbirinde element avantajı yok, **WHEN** ağırlıklar hesaplanırsa, **THEN** P(en düşük HP) = %40, P(en yüksek ATK) = %35, P(rastgele) = %25.

5. **GIVEN** Patron AI boss (Ateş) ve 3 canavar: A (Toprak, ATK=35), B (Su, ATK=52), C (Hava, ATK=30), en düşük HP = A, **WHEN** ağırlıklar hesaplanırsa, **THEN** A'nın ağırlığı 40+10(avantaj)=50, toplam=110, P(A) = 50/110 = %45.5.

6. **GIVEN** Kat 5 Saldırgan mini-boss (Lv5, ATK=57), hedef DEF=25, nötr element, **WHEN** 1.5x yetenek kullanırsa, **THEN** hasar = floor(max(1, 57×1.5 - floor(25/2)) × 1.0) = **73**.

7. **GIVEN** Kat 5 Büyücü mini-boss (ATK=57), 3 hedef (DEF=25), nötr, büyü hasarı, **WHEN** 0.5x AoE kullanırsa, **THEN** her hedefe hasar = floor(max(1, floor(57×0.5) - floor(25/4)) × 1.0) = floor(28 - 6) = **22**.

8. **GIVEN** Kat 5 Tank mini-boss (DEF=57), **WHEN** savunma duruşu kullanırsa, **THEN** buffed_DEF = 57 × 2 = **114**, 2 tur sürer, sonra 57'ye döner.

9. **GIVEN** Kat 10 boss (Saldırgan, ATK=80), hedef DEF=25, nötr, **WHEN** 2.0x güçlü tek hedef saldırısı yaparsa, **THEN** hasar = floor(max(1, 80×2.0 - floor(25/2)) × 1.0) = **148**.

10. **GIVEN** Kat 10 boss (Saldırgan, ATK=80), 3 hedef (DEF=25), nötr, **WHEN** 0.75x güçlü AoE yaparsa, **THEN** her hedefe hasar = floor(max(1, 80×0.75 - floor(25/2)) × 1.0) = **48**.

11. **GIVEN** Boss (base_SPD=46, HP > %50), saldırı döngüsü Tur 2'de, **WHEN** bu turda HP %50 altına düşerse, **THEN** öfke modu **bir sonraki turda** aktifleşir, rage_SPD = floor(46 × 1.20) = **55**, güçlü CD: 3→2.

12. **GIVEN** Boss öfke modunda (rage aktif, güçlü CD=2), **WHEN** saldırı döngüsü başlarsa, **THEN** patern: Normal→Güçlü→Normal→Güçlü... (her 2 turda güçlü saldırı).

13. **GIVEN** herhangi AI katmanı, 2 canavar aynı HP oranında, **WHEN** "en düşük HP" ağırlığı uygulanırsa, **THEN** ikisi arasında eşit olasılıkla rastgele seçilir.

14. **GIVEN** herhangi AI katmanı, tek canavar hayatta, **WHEN** hedef seçimi yapılırsa, **THEN** probability = 1.0, AI katmanı fark etmez.

15. **GIVEN** Boss AoE saldırısı, 4 canavardan 2'si savaş dışı, **WHEN** AoE uygulanırsa, **THEN** hasar sadece hayatta olan 2 hedefe uygulanır; savaş dışı canavarlar etkilenmez.

16. **GIVEN** Taktik AI mini-boss, yeteneğini bu tur kullanmış (CD=3), **WHEN** sonraki 2 turda sırası gelirse, **THEN** normal saldırı yapar. 3. turda CD=0 olur ve yetenek tekrar kullanılabilir.

## Open Questions

1. ~~**Boss özel yetenek çeşitliliği**~~: ✅ Çözüldü — Boss arketipine göre belirlenir (Kural 4 güncellemesi). AoE yetenekleri: Büyücü/Saldırgan arketip. Güçlü tek hedef: Tank/Destekçi arketip.

2. **Düşman canavar havuzu (MVP)**: 15-20 canavar MVP'de hem oyuncu hem düşman olarak mı kullanılacak, yoksa düşmanlara özgü canavarlar da olacak mı? → Canavar Veritabanı GDD'si "oyuncu canavarları = düşman canavarları" varsayıyor ama bu açıkça onaylanmadı.

3. **Otofarm'da AI hesaplama**: İdle modda düşman AI tur tur mu hesaplanacak, yoksa sonuç simülasyonu mu yapılacak (sadece kazanma olasılığı → loot)? → Otofarm / Idle Sistemi GDD'sinde tanımlanacak.

4. **Boss yetenek telegraph süresi**: Güçlü saldırıdan 1 tur önce "!" uyarısı yeterli mi, yoksa 2 tur önceden mi uyarılmalı? → Playtest verisi ile belirlenecek (tuning knob olarak eklenebilir).

5. **Arena'da rakip AI (Tier 2+)**: Arena'da oyuncu takımları rakip olarak kullanıldığında, rakip AI hangi katmanı kullanacak? → Arena GDD'sinde tanımlanacak.

6. **Düşman arketip dağılımı**: Normal katlarda düşman canavarların arketip dağılımı (Saldırgan/Tank/Destekçi/Büyücü oranları) ne olmalı? → Zindan Keşif GDD'sinde tanımlanacak.
