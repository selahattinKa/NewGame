# Canavar Veritabanı (Monster Database)

> **Status**: Designed
> **Author**: user + game-designer, systems-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Topla Hepsini, Güç Hisset

## Overview

**Canavar Veritabanı**, oyundaki tüm canavar türlerinin merkezi veri deposudur. Her canavarın temel istatistikleri (HP, saldırı, savunma, hız), element aidiyeti (ateş/su/toprak/hava), arketip sınıfı (saldırgan/tank/destekçi/büyücü), nadirlik kademesi (yaygın→efsanevi) ve evrim yolu bu sistemde tanımlanır. Veritabanı salt-okunur bir referans katmanıdır — canavarları değiştirmez, sadece "bir canavar nedir?" sorusuna tek kaynak cevap verir.

Oyuncu bu sistemi doğrudan görmez, ancak koleksiyondaki her canavarın benzersiz hissetmesi, güçlendirme sırasında stat büyümesinin tatmin edici olması ve element sinerjilerinin keşfedilebilir olması bu veritabanının zenginliğine bağlıdır. Savaş, güçlendirme, loot, takım kurma ve evrim sistemlerinin tamamı Canavar Veritabanı'nın tanımlarını tüketir; hiçbiri canavar verisini kendisi üretmez.

Prototype kapsamında 10-15 canavar türü (4 element × 4 arketip matrisinden seçilen), **4 nadirlik kademesi (F-D-C-B)** ve temel stat yapısı tanımlanır. A, S ve SS kademeleri Google Play soft-launch verisi sonrasında MVP güncellemesiyle eklenecektir. Tam vizyonda 200+ canavar türüne ölçeklenebilir bir şema hedeflenir.

> **Kapsam notu**: Bu veritabanı **toplanabilir pet canavarlarını** (oyuncunun koleksiyonuna aldığı varlıklar) kapsar. F/D/C/B tier sistemi yalnızca bu petler ve ekipman için geçerlidir. Keşif Alanı'nda savaşılan **düşman canavarlarının** tier'ı yoktur — güçleri **SG (Saldırı Gücü)** ile ölçülür.

## Player Fantasy

Oyuncu Canavar Veritabanı ile doğrudan etkileşmez — hissettiği şey bu veritabanının mümkün kıldığı **çeşitlilik ve derinliktir**. Koleksiyonu taradığında her canavarın istatistikleri, element kimliği ve nadirlik seviyesi farklıdır; hiçbiri diğerinin kopyası hissetmez. Yeni bir canavar kazandığında stat kartını incelediğinde "bu ne yapabilir?" merakı uyanır. Takımını kurarken element sinerjilerini keşfeder, güçlendirme sırasında büyüyen sayılar tatmin verir.

Bu sistemin başarısı sessizdir: çalıştığında oyuncu fark etmez, başarısız olduğunda (tekdüze canavarlar, anlamsız stat farkları, kırık sinerjiler) tüm oyun sığ hisseder.

**Pillar bağlantısı**: "Topla Hepsini" — her canavarın benzersiz ve değerli hissetmesi bu veritabanının şemasına bağlıdır. "Güç Hisset" — stat büyümesinin tatmin edici eğrisi burada tanımlanır.

## Detailed Rules

### Core Rules

**Kural 1 — Canavar Kimlik Şeması**

Her canavar türü aşağıdaki sabit kimlik alanlarıyla tanımlanır:

| Alan | Tip | Açıklama | Örnek |
|------|-----|----------|-------|
| `id` | string | Benzersiz tanımlayıcı (kebab-case) | `fire-striker-infernalclaw` |
| `display_name` | string | Oyuncuya gösterilen isim | "Cehennem Pençesi" |
| `element` | enum | Ateş / Su / Toprak / Hava | `fire` |
| `archetype` | enum | Saldırgan / Tank / Destekçi / Büyücü | `striker` |
| `base_rarity` | enum | F / D / C / B (prototype); A/S/SS MVP sonrası | `C` |
| `evolution_stage` | int | Mevcut evrim aşaması (1, 2 veya 3) | `1` |
| `evolves_to` | string\|null | Evrimleştiği canavarın id'si | `fire-striker-infernalclaw-2` |
| `evolves_from` | string\|null | Evrimleştiği kaynak id | `null` |
| `description` | string | Lore/flavor metni | "Volkanik derinliklerden..." |
| `visual_asset_id` | string | Art bible asset referansı | `char_monster_fire_striker_infernalclaw_rare` |

**Kural 2 — Temel İstatistikler (Base Stats)**

Her canavar türü 4 temel istatistiğe sahiptir (seviye 1 değerleri):

| Stat | Kısaltma | Açıklama | Kullanıcı Etkisi |
|------|----------|----------|------------------|
| **Can Puanı** | HP | Canavarın dayanıklılığı | Sıfıra düşünce savaş dışı kalır |
| **Saldırı** | ATK | Verilen hasarın temel çarpanı | Hasar Hesaplama'ya giriş |
| **Savunma** | DEF | Alınan hasarı azaltma | Hasar Hesaplama'ya giriş |
| **Hız** | SPD | Saldırı sırası ve sıklığı | Yüksek hız = önce ve daha sık saldırı |

**Kural 3 — Arketip Stat Dağılımı**

Toplam stat havuzunun yüzdesel dağılımı:

| Arketip | HP% | ATK% | DEF% | SPD% | Kimlik |
|---------|-----|------|------|------|--------|
| **Saldırgan** | 20% | 35% | 15% | 30% | Cam top — hızlı ve yıkıcı, kırılgan |
| **Tank** | 30% | 15% | 35% | 20% | Dağ — yavaş ama yıkılmaz |
| **Destekçi** | 28% | 17% | 25% | 30% | Dengeli — takımı ayakta tutar |
| **Büyücü** | 18% | 35% | 17% | 30% | Güçlü ama savunmasız |

**Kural 4 — Nadirlik Kademeleri**

> **Prototype kapsamı**: F-D-C-B (4 tier). A, S, SS Google Play soft-launch sonrası MVP güncellemesiyle eklenecek.

| Kademe | Stat Havuzu (Lv1) | Max Evrim | Düşme Çarpanı | Oyuncu Beklentisi |
|--------|-------------------|-----------|---------------|-------------------|
| **F** (Yaygın) | 100 | 2 (A→B) | 1.0x | Başlangıç takımı |
| **D** (Seyrek) | 120 | 2 (A→B) | 0.5x | İlk güçlenme hissi |
| **C** (Nadir) | 150 | 3 (A→B→C) | 0.15x | Takımın omurgası |
| **B** (Epik) | 185 | 3 (A→B→C) | 0.04x | Endgame gücü — prototype boss tier |

*MVP sonrası eklenecek: A (215, 0.01x), S (250, 0.005x), SS (300, 0.001x)*

**Tasarım notu**: F ve D tier canavarlar yapısal olarak C+ seviyesine ulaşamaz (F B max=140 < C A=150). Bu genre standardıdır (gacha fodder pattern): F/D canavarlar erken oyun takımı filler'ı ve yıldız yükseltme (star-up) birleştirme malzemesi olarak tasarlanmıştır. "Topla Hepsini" pillar'ı bu kademeler için "koleksiyon kaydı tamamlama" ve "güçlendirme malzemesi" değeri üzerinden sağlanır, endgame viability değil.

**Kural 5 — Element Etkileşim Matrisi**

Döngüsel avantaj: Ateş → Toprak → Hava → Su → Ateş

| Saldıran \ Savunan | Ateş | Su | Toprak | Hava |
|---------------------|------|-----|--------|------|
| **Ateş** | 1.0x | 0.75x | 1.5x | 1.0x |
| **Su** | 1.5x | 1.0x | 1.0x | 0.75x |
| **Toprak** | 0.75x | 1.0x | 1.0x | 1.5x |
| **Hava** | 1.0x | 1.5x | 0.75x | 1.0x |

Avantajlı: 1.5x hasar. Dezavantajlı: 0.75x. Nötr/Aynı: 1.0x.

**Kural 6 — Evrim Sistemi**

- F ve D: 2 aşama (Form A → Form B)
- C ve B: 3 aşama (Form A → Form B → Form C)
- Evrim element ve arketipi değiştirmez — sadece statları, görseli ve yetenek setini güçlendirir
- Her evrim aşaması stat havuzunu %40 artırır (Nadir Lv1: A=150, B=210, C=294)
- Evrim gereksinimleri Canavar Güçlendirme GDD'sinde tanımlanacak

**Kural 7 — Yetenek Slotları**

- **Slot 1 (Temel Yetenek)**: Doğuştan. Element temasıyla bağlantılı ama her canavara özgü — aynı elementteki iki canavar farklı Slot 1 yeteneğine sahiptir.
- **Slot 2 (Evrim Yeteneği)**: Form B'de açılır. Arketip temasıyla bağlantılı ama her canavara özgü.

Her canavar mekanik olarak benzersizdir: aynı element+arketip+nadirlikte iki canavar bile farklı yetenek setlerine sahiptir. Yetenek detayları (hasar, efekt, cooldown) Savaş Sistemi GDD'sinde tanımlanacak.

### States and Transitions

Veritabanı salt-okunur referanstır — kendi durum geçişi yoktur. Canavar türünün yaşam döngüsü diğer sistemler tarafından tetiklenir:

| Durum | Tetikleyici | Hedef | Sahip Sistem |
|-------|-------------|-------|--------------|
| **Tanımsız** | Oyun yüklendiğinde tanımlar yüklenir | **Tanımlı** | Veritabanı |
| **Tanımlı** (salt-okunur) | Oyuncu bu türden canavar kazanır | **Koleksiyonda** (instance) | Canavar Toplama |
| **Koleksiyonda** | Seviye atlatır | **Güçlendirilmiş** | Canavar Güçlendirme |
| **Koleksiyonda** | Evrim gereksinimleri karşılanır | **Evrimleşmiş** | Canavar Güçlendirme |

**Önemli ayrım**: Veritabanı **tür tanımlarını** (template) tutar. Oyuncunun **canavar instance'ları** (seviye, XP) Canavar Toplama/Güçlendirme'de yönetilir.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Element Sistemi** | ← okur | Element etkileşim çarpanları | Veritabanı elementi bildirir; Element Sistemi çarpanı hesaplar |
| **Hasar Hesaplama** | → sağlar | ATK, DEF base stat | `GetBaseStats(monsterId, level)` → {hp, atk, def, spd} |
| **Sağlık/Can** | → sağlar | Max HP | `GetBaseStats()` üzerinden HP |
| **Canavar Güçlendirme** | → sağlar | Büyüme oranları, evrim yolu | `GetGrowthRates(monsterId)`, `GetEvolutionTarget(monsterId)` |
| **Loot/Ödül** | → sağlar | Nadirlik ve düşme çarpanı | `GetRarityWeight(monsterId)` |
| **Takım Kurma** | → sağlar | Element, arketip, slot | `GetMonsterIdentity(monsterId)` |
| **Canavar Toplama** | → sağlar | Tam tür tanımı | `GetMonsterDefinition(monsterId)` |
| **Koleksiyon UI** | → sağlar | Görüntüleme verileri | display_name, description, visual_asset_id, stats |
| **Savaş UI** | → sağlar | Savaş stat referansı | Anlık HP, ATK, DEF, SPD |

Veri akışı tek yönlüdür — veritabanı runtime'da immutable'dır.

## Formulas

### Formül 1: Stat Havuzu Dağıtımı

`individual_stat = floor(total_stat_pool * archetype_percentage)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Toplam stat havuzu | total_stat_pool | int | 100–225 | Nadirlik kademesine göre (Kural 4) |
| Arketip yüzdesi | archetype_percentage | float | 0.15–0.35 | Arketip tablosundan (Kural 3) |
| Bireysel stat | individual_stat | int | 15–78 | Sonuç — floor ile yuvarlanır |

**Çıktı Aralığı**: 15 ile 78. Max non-HP stat = floor(225 × 0.35) = 78.

**Örnek**: Nadir Saldırgan Lv1 → HP=31, ATK=52, DEF=22, SPD=45 (havuz=150)

**Yuvarlama Kuralı**: `hp_final = floor(pool × hp%) + (pool - Σfloor(pool × stat%_i))`. Tüm statlar floor ile yuvarlanır, kalan fark HP'ye eklenir. Toplam her zaman pool'a eşittir.

### Formül 2: Evrim Stat Artışı

`evolved_stat_pool = floor(base_stat_pool * (1 + evolution_bonus) ^ (stage - 1))`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel stat havuzu | base_stat_pool | int | 100–225 | Nadirlik Form A havuzu |
| Evrim bonusu | evolution_bonus | float | 0.40 | Sabit %40 artış |
| Aşama | stage | int | 1–3 | Evrim aşaması |
| Evrimleşmiş havuz | evolved_stat_pool | int | 100–441 | Sonuç |

**Çıktı Aralığı**: 100 (Yaygın A) ile 441 (Efsanevi C: 225 × 1.4² = 441)

**Örnek**: Nadir → A=150, B=210, C=294

Evrimleşmiş stat dağılımı aynı arketip yüzdeleriyle (Kural 3) yapılır — evrim arketip dağılımını değiştirmez, sadece havuzu büyütür.

### Formül 3: Element Hasar Çarpanı

`element_multiplier = ELEMENT_MATRIX[attacker_element][defender_element]`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Saldıran elementi | attacker_element | enum | {fire, water, earth, air} | Saldıranın elementi |
| Savunan elementi | defender_element | enum | {fire, water, earth, air} | Savunanın elementi |
| Çarpan | element_multiplier | float | 0.75–1.50 | Hasar çarpanı |

**Çıktı**: 0.75 (dezavantaj), 1.0 (nötr), 1.5 (avantaj)

Bu çarpan Hasar Hesaplama GDD'sine giriş olarak aktarılır.

## Edge Cases

- **If yuvarlama sonucu stat toplamı havuzu aşarsa**: Her stat bağımsız floor, fark HP'ye eklenir. Toplam asla havuzun üstüne çıkamaz.

- **If aynı id'ye sahip iki canavar tanımı yüklenirse**: İkinci reddedilir, hata loglanır. İlk tanım geçerli.

- **If evolves_to var olmayan id'ye işaret ederse**: Evrim butonu devre dışı. Hata loglanır. Oyuncu bilgilendirilmez.

- **If element değeri 4 geçerli değer dışındaysa**: Canavar nötr olarak işlenir (tüm çarpanlar 1.0x). Hata loglanır.

- **If stat havuzu 0 veya negatifse**: Tüm statlara minimum 1 uygulanır. Hata loglanır.

- **If aynı element + arketip + nadirlikte birden fazla canavar varsa**: Geçerli. Her canavarın kendine özgü yetenek seti vardır (Kural 7) — stat dağılımı aynı olsa da mekanik farklılık yetenekler üzerinden sağlanır.

- **If evrim zincirinde döngü varsa (A→B→A)**: Yükleme sırasında döngü tespiti, zincir kesilir (evolves_to null), hata loglanır.

- **If oyuncu sahip olmadığı canavarın bilgisini sorgularsa**: Geçerli — veritabanı tüm tür tanımlarını açar. Sahiplik bilgisi Canavar Toplama'da.

## Dependencies

### Upstream (Bu sistem neye bağlı)

Yok — Foundation katmanı, sıfır bağımlılık.

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| Sağlık / Can | Sert | `GetBaseStats()` → HP | Olmadan çalışamaz |
| Hasar Hesaplama | Sert | `GetBaseStats()` → ATK, DEF | Olmadan çalışamaz |
| Canavar Güçlendirme | Sert | `GetGrowthRates()`, `GetEvolutionTarget()` | Olmadan çalışamaz |
| Loot / Ödül | Sert | `GetRarityWeight()` | Düşme oranları burada |
| Takım Kurma | Sert | `GetMonsterIdentity()` | Element/arketip bilgisi |
| Canavar Toplama | Sert | `GetMonsterDefinition()` | Yeni canavar oluşturma |
| Koleksiyon UI | Yumuşak | display_name, description, visual_asset_id | Görüntüleme |
| Savaş UI | Yumuşak | Stat referansı | Görüntüleme |

Tüm bağımlılıklar tek yönlü — veritabanı runtime'da immutable.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `evolution_bonus` | 0.40 | 0.25–0.60 | Evrimli canavarlar çok güçlü → eskiler anlamsız | Evrim tatmin etmez |
| `rarity_stat_pool_common` | 100 | 80–120 | Yaygınlar çok güçlü → nadirlik farkı anlamsız | Yaygınlar işe yaramaz |
| `rarity_stat_pool_uncommon` | 120 | 100–140 | — | — |
| `rarity_stat_pool_rare` | 150 | 130–180 | — | — |
| `rarity_stat_pool_epic` | 185 | 160–220 | — | — |
| `rarity_stat_pool_legendary` | 225 | 200–280 | Legendary çok baskın → PvP meta tek boyutlu | "Efsanevi" hissetmez |
| `element_advantage_multiplier` | 1.50 | 1.25–2.00 | Element seçimi çok kritik | Element önemsizleşir |
| `element_disadvantage_multiplier` | 0.75 | 0.50–0.90 | Dezavantaj çok ağır | Dezavantaj hissedilmez |
| `archetype_stat_percentages` | Kural 3 | ±5% her oran | Arketipler benzeşir | Aşırı uzmanlaşma |

**Etkileşim Uyarısı**: `evolution_bonus` × `rarity_stat_pool_legendary` birlikte Efsanevi Form C gücünü belirler (225 × 1.4² = 441). İkisini aynı anda artırmak güç eğrisini kırabilir.

## Acceptance Criteria

1. **GIVEN** Nadir Saldırgan tanımı, **WHEN** `GetBaseStats("fire-striker-infernalclaw", 1)` çağrılırsa, **THEN** HP=31, ATK=52, DEF=22, SPD=45 döner (toplam=150).

2. **GIVEN** Yaygın canavar Form A (havuz=100), **WHEN** Form B'ye evrimleşirse, **THEN** stat havuzu = 140.

3. **GIVEN** Efsanevi canavar (havuz=225), **WHEN** Form C'ye evrimleşirse, **THEN** stat havuzu = 441.

4. **GIVEN** Ateş saldırgan, **WHEN** Toprak savunana saldırırsa, **THEN** element çarpanı = 1.5x.

5. **GIVEN** Su canavar, **WHEN** Hava savunana saldırırsa, **THEN** element çarpanı = 0.75x.

6. **GIVEN** Aynı element (Ateş vs Ateş), **WHEN** çarpan sorgulanırsa, **THEN** 1.0x döner.

7. **GIVEN** Saldırgan arketip havuz=100, **WHEN** stat dağıtılırsa, **THEN** HP=20, ATK=35, DEF=15, SPD=30 (toplam=100).

8. **GIVEN** evrim döngüsü (A→B→A), **WHEN** veritabanı yüklenirse, **THEN** döngü tespit edilir, bağlantı null yapılır, hata loglanır.

9. **GIVEN** geçersiz element değeri, **WHEN** stat sorgulanırsa, **THEN** tüm çarpanlar 1.0x, hata loglanır.

10. **GIVEN** MVP seti (15-20 tür), **WHEN** tümü yüklenirse, **THEN** benzersiz id'ler, her element ≥1 canavar, her arketip ≥1 canavar.

11. **GIVEN** runtime veritabanı, **WHEN** değişiklik denenirse, **THEN** işlem reddedilir (immutability).

12. **GIVEN** aynı id'ye sahip iki canavar tanımı, **WHEN** veritabanı yüklenirse, **THEN** ikinci reddedilir, hata loglanır, ilk tanım geçerli kalır.

13. **GIVEN** evolves_to alanı var olmayan id'ye işaret eden canavar, **WHEN** evrim kontrolü yapılırsa, **THEN** evrim butonu devre dışı kalır ve hata loglanır.

14. **GIVEN** stat havuzu 0 veya negatif olan canavar tanımı, **WHEN** stat dağıtılırsa, **THEN** tüm statlara minimum 1 uygulanır ve hata loglanır.

15. **GIVEN** Yaygın canavar, **WHEN** evolution_stage sorgulanırsa, **THEN** max_stage = 2 (Form A→B). VE Nadir canavar için max_stage = 3 (Form A→B→C).

16. **GIVEN** Tank arketip havuz=100, **WHEN** stat dağıtılırsa, **THEN** HP=30, ATK=15, DEF=35, SPD=20 (toplam=100, yuvarlama kaybı yok).

17. **GIVEN** Büyücü arketip havuz=225 (Legendary), **WHEN** stat dağıtılırsa, **THEN** HP=floor(225×0.18)+2=42, ATK=floor(225×0.35)=78, DEF=floor(225×0.17)=38, SPD=floor(225×0.30)=67 (toplam=225, yuvarlama kaybı=2→HP'ye).

## Open Questions

1. **MVP canavar listesi**: 4 element × 4 arketip = 16 kombinasyon. 15-20 canavar hedefiyle hangi kombinasyonlar MVP'de yer alacak? → Canavar tasarımı sırasında belirlenir.

2. **Evrim malzeme gereksinimleri**: Evrim için gereken kaynaklar (altın, özel malzeme, aynı canavar kopyası?) → Canavar Güçlendirme GDD'sinde tanımlanacak.

3. **Yetenek detayları**: Slot 1 ve Slot 2 yeteneklerinin spesifik etkileri → Savaş Sistemi GDD'sinde tanımlanacak.

4. **Canavar isimleri ve lore**: display_name ve description değerleri → Narrative/Writer aşamasında doldurulacak.
