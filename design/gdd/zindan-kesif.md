# Zindan Keşif Sistemi (Dungeon Exploration System)

> **Status**: Designed
> **Author**: user + game-designer
> **Last Updated**: 2026-06-24
> **Implements Pillar**: Cömert Zindan, Hep Bir Şey Var, Güç Hisset

## Overview

**Zindan Keşif Sistemi**, oyunun ana aktivite döngüsünü oluşturan yapıdır: oyuncunun canavar takımını göndereceği katlı zindanları tanımlar, her kattaki düşman gruplarını yapılandırır, kat ilerleme mantığını yönetir ve savaş-loot-ilerleme akışını orkestre eder. Mekanik olarak sistem, bölge → zindan → kat hiyerarşisinde çalışır: her bölge kendine ait düşman havuzu, görsel tema ve zorluk eğrisine sahiptir; her zindan bölgenin alt kümesidir; her kat 1 düşman grubu + 1 savaş + 1 loot rulosu içerir. Her 5. kat boss katıdır — daha güçlü düşman, daha cömert ödül, boss canavarı düşme şansı. Kat girişi enerji maliyetlidir (2 enerji/kat), kazanılan savaşlar katı temizler ve bir sonrakini açar; kaybedilen savaşlar enerji harcamaz ve cezasızdır.

Oyuncu perspektifinden Zindan Keşif, "bir kat daha" psikolojisinin motoru ve keşif heyecanının kaynağıdır. Oyuncu zindana girdiğinde her kat onu ödüllendiren bir meydan okumadır: düşman grubunu yen, loot topla, bir sonraki kata ilerle. Bir sonraki katın potansiyel ödülünü görme dürtüsü oturumu doğal olarak uzatır. İlk kez temizlenen katlar ekstra elmas ödülü verir; daha önce geçilen katlar tekrar edilebilir (farm). Boss katları zorluk sıçraması + cömert ödül denklemiyle "doruk an" yaratır. Sistem, oyuncunun gücüne göre katlar arası zorluk ölçeklemesiyle "her zaman bir sonraki zorluk var" hissini korurken, "Cömert Zindan" gereği eli boş döndüren bir kat asla yoktur.

MVP kapsamında 1 zindan bölgesi (10 kat, 2 boss katı: Kat 5 ve Kat 10), bölge canavar havuzu, kat düşman grubu yapılandırması, ilerleme state machine'i, enerji maliyet kontrolü, ilk temizleme ödülleri ve tekrar oynama döngüsü tanımlanır. İleride (Tier 2+) ek bölgeler, prosedürel kat düzenleri, evrim zindanları ve özel etkinlik zindanları eklenecektir.

## Player Fantasy

Zindan Keşif Sistemi'nde oyuncu **fetih komutanı** fantezisi yaşar. Çekirdek an, bir sonraki katın kapısının açılması ve "bu sefer ne çıkacak?" merakıdır — yeni bir düşman grubu, belki nadir bir canavar, kesinlikle cömert bir ödül. Her temizlenen kat bilinmeyen topraklarda bir adım daha ilerlemek, her boss yenilgisi bir bölgenin fethidir. Oyuncu zindan haritasına baktığında geçmiş katlarını temizlenmiş topraklar olarak görür — "buralar benim" sahiplik hissi.

**"Bir kat daha" dürtüsü**: Oyunun en güçlü retention mekaniği bu sistemde yaşar. Bir katı temizledikten sonra bir sonraki katın potansiyel ödülü ekranda gösterilir — "belki bu katta o Nadir canavar düşer." Bu dürtü oturumu 5 dakikadan 30 dakikaya uzatır. Oyuncu durmayı bilinçli olarak seçmeli — oyun onu durdurmak yerine "bir tane daha" diye cezbetmeli.

**İlerleme tatmini**: Daha önce 8 turda geçilen Kat 3, şimdi 3 turda geçiliyor. Oyuncu zindan tekrarında bu farkı somut olarak hisseder — güçlendirme çalışmış, ordusu büyümüş. Boss'u ilk seferde yenemeyip takımı güçlendirip geri dönüp ezmek, oyunun en tatmin edici anlarından biri.

**Keşif heyecanı**: Her yeni katta karşılaşılan düşman canavar türleri Pokédex'e kaydedilir. İlk kez görülen bir canavar "bu ne?" merakı uyandırır. Yeni bir bölge açıldığında (Tier 2+) tamamen farklı bir düşman havuzu ve görsel tema — her bölge yeni bir macera hissi.

**Negatif fantezi (kaçınılacak)**: "Duvar" — oyuncunun günlerce aynı katta kalması. Zorluk ölçeklemesi oyuncu gücünün biraz gerisinde kalmalı — oyuncu "biraz önde" hissetmeli. "Boş tekrar" — aynı katları farm ederken sıkılma. Boss katları ve nadir canavar düşme şansı tekrarı anlamlı kılmalı. "Enerji cezası" — enerjisi bitti diye oturamama. Enerji geri dönüş hızı (1/5dk) makul sınır koyar ama cezalandırıcı hissettirmemeli.

**Pillar bağlantısı**: "Cömert Zindan" — her kat ödüllendirici, kaybetmek bile cezasız. "Hep Bir Şey Var" — her zaman bir sonraki kat, bir sonraki boss, bir sonraki bölge. "Güç Hisset" — geçmiş katların kolaylaşması güçlenmeyi somutlaştırır.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Zindan Hiyerarşisi**

Oyun dünyası üç katmanlı bir yapıda organize edilir:

| Katman | Tanım | MVP Kapsamı |
|--------|-------|-------------|
| **Bölge** | En üst düzey alan — kendine ait düşman havuzu, görsel tema, arka plan sanatı, müzik | 1 bölge: "Karanlık Orman" |
| **Zindan** | Bölge içindeki oynanabilir birim — katlara bölünmüş dikey yapı | 1 zindan: 10 kat |
| **Kat** | Tek bir savaş karşılaşması — düşman dalgaları + loot | 10 kat (Kat 1-10) |

Bölge → Zindan → Kat ilişkisi ileride genişletmeye açıktır (Tier 2+: ek bölgeler, bölge başına birden fazla zindan).

**Kural 2 — Kat İlerleme (Lineer)**

1. Oyuncu Kat 1'den başlar — Kat 1 her zaman açıktır
2. Bir katı temizlemek (tüm dalgaları yenmek) bir sonraki katı açar
3. İlerleme lineerdir: Kat N'i geçmeden Kat N+1'e girilemez
4. İlerleme kalıcıdır — oturum arası kaybolmaz (Kaydetme/Yükleme)
5. Oyuncu en son açtığı kata kadar istediği katı seçip tekrar oynayabilir (farm)
6. Kat seçimi zindan harita ekranından yapılır (Zindan Harita UI)

**Kural 3 — Kat Yapısı (Dalga Sistemi)**

Her kat 2-3 düşman dalgasından oluşur:

| Kat Tipi | Dalga Sayısı | Dalga Yapısı |
|----------|-------------|-------------|
| **Normal Kat** | 2 dalga | Dalga 1: 2-3 düşman. Dalga 2: 3-4 düşman. |
| **Boss Katı** (her 5. kat) | 3 dalga | Dalga 1: 2-3 düşman. Dalga 2: 3-4 düşman. Dalga 3: 1 Boss + 1-2 yardımcı. |

Dalga akışı:
1. Savaş başlar → ilk dalga düşmanları yüklenir
2. Dalga yenildiğinde (tüm düşmanlar savaş dışı) → kısa geçiş animasyonu → sonraki dalga yüklenir
3. Takım HP ve enerji **dalgalar arası korunur** — iyileşme yok, enerji sıfırlanmaz
4. Tüm dalgalar temizlendiğinde → kat temizlendi → loot dağıtımı
5. Herhangi bir dalgada tüm oyuncu canavarları savaş dışı kalırsa → kat başarısız
6. Oyuncu herhangi bir dalgada çekilebilir → kat başarısız, dalga arası çekil veya savaş içi çekil

**HP ve Enerji Dalgalar Arası**: Takım canavarlarının HP'si dalgalar arasında sıfırlanmaz — Dalga 1'de aldıkları hasar Dalga 2'ye taşınır. Enerji birikimi de taşınır. Bu mekanik Boss katlarında stratejik derinlik yaratır: Dalga 1-2'de enerji biriktirip Boss dalgasında yetenek kullanma.

**Kural 4 — Enerji Maliyeti**

| İşlem | Maliyet | Koşul |
|-------|---------|-------|
| Kata giriş (ilk kez veya tekrar) | 2 enerji | Enerji ≥ 2 |
| Sweep (süpürme) | 2 enerji | Enerji ≥ 2, kat daha önce temizlenmiş |
| Kaybetme | 0 (enerji iade) | Enerji harcanmaz |
| Çekilme | 0 (enerji iade) | Enerji harcanmaz |

Enerji Ekonomi GDD'den: 100 maks, 1/5dk yenilenme, ~8.3 saat tam dolum. 100 enerji = 50 kat kapasitesi.

**Kural 5 — Sweep (Süpürme) Mekaniği**

Daha önce temizlenmiş katları tek butonla geçme:

| Parametre | Değer |
|-----------|-------|
| Koşul | Kat daha önce temizlenmiş olmalı |
| Enerji maliyeti | Normal giriş ile aynı (2 enerji) |
| Loot | Normal loot tablosu uygulanır (aynı oranlar) |
| Süre | Anlık — savaş animasyonu atlanır |
| Toplu sweep | Birden fazla kat seçilebilir ("10 kat sweep" gibi) |
| Kısıtlama | Boss katları sweep edilemez — boss savaşı her zaman oynanmalı |

Sweep loot'u normal loot'la aynıdır — "Cömert Zindan" gereği sweep eden oyuncu cezalandırılmamalı. Boss katı kısıtlaması komutan modu etkileşimini korur ve boss savaşlarını anlamlı tutar.

**Kural 6 — Boss Katı Kuralları**

| Parametre | Değer |
|-----------|-------|
| Boss katı sıklığı | Her 5. kat (Kat 5, 10, 15...) |
| Boss sayısı | 1 boss + 1-2 yardımcı düşman |
| Boss türü | Kat 5: Mini-Boss, Kat 10: Alan Patronu (MVP) |
| Boss düşme | Loot GDD Kural 6: B Tier=%5, A Tier=%3, S/SS Tier=%1 |
| Boss altını | Normal kat × 3 (boss_gold_multiplier = 3.0) |
| Sweep | Boss katı sweep edilemez |

Boss savaşı tüm oyuncular için "doruk an" — sweep atlatılamaz, komutan modu bonus burada en değerli.

**AI katmanı eşleştirmesi** (Düşman AI GDD terminolojisi): Bu dokümanda "boss katı" her 5. katı ifade eder. Düşman AI GDD'de Kat 5 boss'u **mini-boss (Taktik AI)**, Kat 10 boss'u **boss (Patron AI)** olarak sınıflandırılır. Implementasyonda AI katmanı seçimi boss türüne göre yapılır: Mini-Boss = Taktik AI, Alan Patronu = Patron AI.

**Kural 7 — Düşman Grubu Oluşturma**

Her dalgadaki düşmanlar şu parametrelerle belirlenir:

1. **Düşman sayısı**: Dalga tipine göre (Kural 3)
2. **Düşman türü**: Bölge canavar havuzundan ağırlıklı rastgele seçim
3. **Düşman nadirliği**: Kat numarasına göre ölçeklenir:

| Kat Aralığı | Normal Düşman Gücü | Boss Türü |
|-------------|-------------------|----------|
| Kat 1-3 | Düşük | — |
| Kat 4 | Düşük + %30 Orta | — |
| Kat 5 (Boss) | Orta | Mini-Boss |
| Kat 6-8 | Orta + %20 Yüksek | — |
| Kat 9 | Orta + %40 Yüksek | — |
| Kat 10 (Boss) | Yüksek | Alan Patronu |

4. **Düşman seviyesi**: Düşman AI GDD'den — `floor(floor_number × difficulty_multiplier)`. MVP'de difficulty_multiplier = 1.0 (lineer).

**Kural 8 — İlk Temizleme Ödülleri**

Bir kat ilk kez temizlendiğinde normal loot'a ek olarak:

| Kat | İlk Temizleme Ödülü |
|-----|---------------------|
| Normal kat (1-4, 6-9) | 5 elmas |
| Boss katı (Kat 5) | 50 elmas |
| Son boss katı (Kat 10) | 100 elmas + özel başarım |

İlk temizleme ödülleri tek seferlik — `first_clear` flag'i ile kontrol edilir. Loot GDD Kural 9 ile tutarlı.

**Kural 9 — Zorluk Ölçeklemesi**

Zorluk oyuncu güçlendirmesinin biraz gerisinde kalmalı — "biraz önde" hissi:

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| difficulty_multiplier | 1.0 (MVP) | Düşman seviyesi = kat numarası |
| Enerji harcaması | 2/kat (sabit) | Tüm katlar eşit maliyet |
| Düşman sayısı artışı | Kat 1: 4 toplam → Kat 10: 8 toplam (boss dahil) | Kademeli artış |

Dengeleme prensibi: Game Concept'ten — "Zorluk kademeli olarak zorlaşır ama oyuncu gücü daha hızlı büyür — oyuncu hep 'biraz önde' hisseder. Boss katları zorluk sıçraması yapar ama cömert ödülle telafi eder."

**Kural 10 — Zindan Oturumu Akışı**

Bir zindan oturumu şu adımlardan oluşur:

```
1. Zindan Harita ekranı → Kat seç (veya en son açılan kat)
2. Enerji kontrolü (≥ 2?) → Yetersizse giriş engellenir
3. Kat Giriş → Düşman grubu oluştur (Kural 7)
4. Dalga 1 savaşı → Savaş Sistemi'ne aktar
   → Kazanma: Dalga 2'ye geç (HP/enerji korunur)
   → Kaybetme/Çekilme: Kat başarısız → adım 8
5. Dalga 2 savaşı → Savaş Sistemi'ne aktar
   → Kazanma: Boss katıysa Dalga 3'e geç; normal katsa adım 7
   → Kaybetme/Çekilme: Kat başarısız → adım 8
6. [Boss katı] Dalga 3 savaşı → Savaş Sistemi'ne aktar
   → Kazanma: adım 7
   → Kaybetme/Çekilme: Kat başarısız → adım 8
7. Kat temizlendi:
   → 2 enerji harcanır
   → Loot dağıtımı (Loot Sistemi)
   → İlk temizlemeyse → first_clear ödülü
   → Bir sonraki kat açılır (ilk kez temizlemeyse yok)
   → Pokédex keşif güncellemesi
   → FullHeal(teamId) — tüm takım tam HP'ye döner (Sağlık/Can Kural 6)
   → "Devam" / "Tekrar" / "Çık" seçimi
8. Kat başarısız:
   → Enerji harcanmaz
   → Loot yok
   → "Tekrar Dene" / "Çık" seçimi
```

### States and Transitions

**Kat Durumları**

| Durum | Açıklama | Giriş Tetikleyici | Çıkış |
|-------|----------|-------------------|-------|
| **Locked** | Kilitli — henüz açılmamış | Başlangıç durumu (Kat 2+) | → Unlocked (önceki kat temizlenince) |
| **Unlocked** | Açık — girilebilir, henüz temizlenmemiş | Önceki kat temizlendi | → InProgress (oyuncu girince) |
| **InProgress** | Aktif — oyuncu bu katta savaşıyor | Oyuncu kata girdi | → Cleared / Failed |
| **Cleared** | Temizlendi — tekrar oynanabilir / sweep | Tüm dalgalar yenildi | → InProgress (tekrar oynama) |
| **Failed** | Başarısız — oyuncu kaybetti veya çekildi | TPK veya çekilme | → InProgress (tekrar deneme) |

```
Locked → Unlocked → InProgress ──→ Cleared ──→ InProgress (tekrar)
                                └──→ Failed  ──→ InProgress (tekrar deneme)
```

**Zindan Oturum Durumları**

| Durum | Açıklama | Giriş | Çıkış |
|-------|----------|-------|-------|
| **MapView** | Zindan harita ekranı — kat seçimi | Ana menüden "Zindan" | → FloorEntry / Sweep |
| **FloorEntry** | Kat giriş — düşman grubu oluşturma | Kat seçildi + enerji yeterli | → WaveCombat |
| **WaveCombat** | Dalga savaşı — Savaş Sistemi aktif | Düşman grubu hazır | → WaveTransition / FloorCleared / FloorFailed |
| **WaveTransition** | Dalga arası geçiş — sonraki dalga yükleniyor | Dalga yenildi, sonraki dalga var | → WaveCombat |
| **FloorCleared** | Kat temizlendi — loot dağıtımı + sonuç ekranı | Son dalga yenildi | → MapView / FloorEntry (devam) |
| **FloorFailed** | Kat başarısız — tekrar dene ekranı | TPK veya çekilme | → MapView / FloorEntry (tekrar) |
| **Sweep** | Süpürme — anlık loot hesaplama | Temizlenmiş kat + sweep seçildi | → MapView (loot sonucu) |

```
MapView → FloorEntry → WaveCombat ──→ WaveTransition → WaveCombat
                                   ├──→ FloorCleared → MapView / FloorEntry
                                   └──→ FloorFailed  → MapView / FloorEntry
       → Sweep → MapView
```

**Bölge İlerleme Durumları (Tier 2+ genişletme)**

| Durum | Açıklama | Koşul |
|-------|----------|-------|
| **Locked** | Bölge kilitli | Önceki bölge tamamlanmamış |
| **Available** | Açık, oynanabilir | Önceki bölge tamamlandı |
| **Completed** | Tüm katlar temizlendi | Son kat (boss) temizlendi |

MVP'de tek bölge her zaman Available durumundadır.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Savaş Sistemi** | ↔ çift yönlü | Düşman listesi sağlar ←; savaş sonucu alır → | `GetFloorEnemies(floorNumber, waveIndex)` → [{monsterId, level}]; `OnBattleComplete(result)` ← {outcome, turnsUsed} |
| **Loot / Ödül** | → tetikler | Kat bilgisi sağlar; loot hesaplamasını tetikler | `GetCurrentFloorInfo()` → {floor, type, region, boss_id}; `DistributeFloorLoot(floorNumber, floorType, regionId)` |
| **Canavar Toplama** | → tetikler | Düşman karşılaşma sinyali (Pokédex keşfi) | `OnEnemyEncountered(monsterId)` — her dalgadaki düşmanlar için |
| **Ekonomi** | ← okur + → çağırır | Enerji kontrolü ve harcama | `HasEnergy(amount)` → bool; `SpendEnergy(amount)`; `GetCurrentEnergy()` → int |
| **Düşman AI** | ← okur | Düşman konfigürasyonu, seviye hesaplama | `GetEnemyConfig(monsterId, level)` → stat set; `enemy_level = floor(floor_number × difficulty_multiplier)` |
| **Canavar Veritabanı** | ← okur | Bölge canavar havuzu | `GetRegionMonsterPool(regionId)` → [monsterId[]] |
| **Takım Kurma** | ← okur | Aktif takım kontrolü | `GetActiveTeam()` — savaş öncesi takım yükleme |
| **Kaydetme/Yükleme** | ↔ persist | Kat durumları, first_clear flagları, en yüksek kat | `SaveDungeonState()` / `LoadDungeonState()` |
| **Zindan Harita UI** | → sağlar | Kat durumları, bölge bilgisi, ilerleme verisi | `GetFloorStates()` → [{floor, status, firstClear}]; `GetRegionInfo()` → {name, theme} |
| **Otofarm / Idle** | → sağlar | En yüksek temizlenmiş kat, bölge bilgisi | `GetHighestClearedFloor()` → int; idle hesaplama için referans |

**Veri akışı özeti**: Bu sistem savaş öncesi hazırlık (düşman oluşturma, enerji kontrolü) ve savaş sonrası sonuç işleme (loot tetikleme, ilerleme güncelleme) arasında orkestratör rolü üstlenir. Savaş Sistemi gerçek savaşı yönetir; bu sistem savaşın bağlamını (hangi kat, hangi düşmanlar, ne ödül) tanımlar.

**Bidirectional check**:
- Savaş Sistemi GDD: `GetFloorEnemies` ← ve `OnBattleComplete` → tanımlı ✅
- Loot GDD: `GetCurrentFloorInfo` tanımlı ✅
- Canavar Toplama GDD: `OnEnemyEncountered` tanımlı ✅
- Ekonomi GDD: Enerji sistemi tanımlı ✅
- Düşman AI GDD: `enemy_level_formula` tanımlı ✅
- Zindan Harita UI: henüz yazılmadı *(provisional — Not Started)*
- Otofarm GDD: ✅ Yazıldı — `GetHighestClearedFloor` arayüzü doğrulandı

*Specialist agents not consulted — Lean mode. Review manually before production.*

## Formulas

### Formül 1: Kat Düşman Seviyesi (Referans)

Düşman AI GDD'sinde tanımlıdır — bu sistem tetikler:

`enemy_level = floor(floor_number × difficulty_multiplier)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Kat numarası | floor_number | int | 1–10 (MVP) | Mevcut kat |
| Zorluk çarpanı | difficulty_multiplier | float | 1.0 (MVP) | Bölge bazlı ölçekleme |
| Düşman seviyesi | enemy_level | int | 1–10 | Sonuç |

**Çıktı Aralığı**: 1 (Kat 1) – 10 (Kat 10), MVP'de lineer.

**Örnek**: Kat 7 → floor(7 × 1.0) = **Lv7 düşmanlar**

### Formül 2: Kat Toplam Düşman Sayısı

`total_enemies = wave_1_count + wave_2_count [+ wave_3_count]`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Dalga 1 sayısı | wave_1_count | int | 2–3 | Sabit: erken katlar 2, geç katlar 3 |
| Dalga 2 sayısı | wave_2_count | int | 3–4 | Sabit: erken katlar 3, geç katlar 4 |
| Dalga 3 sayısı (boss) | wave_3_count | int | 2–3 | Boss + 1-2 yardımcı |
| Toplam düşman | total_enemies | int | 5–10 | Kat başı toplam |

**Kat başı tablo:**

| Kat | Dalga 1 | Dalga 2 | Dalga 3 | Toplam |
|-----|---------|---------|---------|--------|
| 1 | 2 | 3 | — | **5** |
| 2-3 | 2 | 3 | — | **5** |
| 4 | 3 | 3 | — | **6** |
| 5 (Boss) | 2 | 3 | 3 (1B+2) | **8** |
| 6-7 | 3 | 3 | — | **6** |
| 8-9 | 3 | 4 | — | **7** |
| 10 (Boss) | 3 | 4 | 3 (1B+2) | **10** |

### Formül 3: Kat Başı Beklenen Toplam Altın

Ekonomi GDD'den referans:

`floor_gold = base_gold_per_floor × floor_number × difficulty_multiplier`

Normal katlarda + boss katlarında 3x çarpan:

| Kat | Altın Hesaplama | Beklenen Altın |
|-----|-----------------|----------------|
| 1 | 100 × 1 × 1.0 | **100** |
| 2 | 100 × 2 × 1.0 | **200** |
| 3 | 100 × 3 × 1.0 | **300** |
| 4 | 100 × 4 × 1.0 | **400** |
| 5 (Boss) | 100 × 5 × 1.0 × 3 | **1.500** |
| 6 | 100 × 6 × 1.0 | **600** |
| 7 | 100 × 7 × 1.0 | **700** |
| 8 | 100 × 8 × 1.0 | **800** |
| 9 | 100 × 9 × 1.0 | **900** |
| 10 (Boss) | 100 × 10 × 1.0 × 3 | **3.000** |
| **Toplam** | | **8.500** |

Bu tablo Loot GDD Formül 9 ile tutarlı (Beklenen Loot / 10 kat: ~8.500 altın).

### Formül 4: İlk Temizleme Elmas Ölçeklemesi

`first_clear_gems = base_first_clear_gems × floor_tier_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel ilk temizleme elması | base_first_clear_gems | int | 5 | Normal kat temel ödülü |
| Kat tier çarpanı | floor_tier_multiplier | int | 1–20 | Normal=1, Boss(5)=10, Son Boss(10)=20 |
| İlk temizleme elması | first_clear_gems | int | 5–100 | Sonuç |

**MVP tablosu:**

| Kat | Hesaplama | Elmas |
|-----|-----------|-------|
| 1-4 | 5 × 1 | **5** (×4 = 20) |
| 5 (Boss) | 5 × 10 | **50** |
| 6-9 | 5 × 1 | **5** (×4 = 20) |
| 10 (Son Boss) | 5 × 20 | **100** |
| **Toplam** | | **190 elmas** |

### Formül 5: Sweep Oturum Verimi

`sweep_session_value = Σ(selected_floors) [floor_gold + random_loot_expected_value]`

Sweep normal loot tablosunu kullanır — verimlilikte fark yoktur. Tek fark süre: sweep anlık, savaş 30sn-2dk/kat.

**10 kat tam sweep (Kat 1-10, boss hariç):**
- Altın: 100+200+300+400+600+700+800+900 = 4.000 altın (8 normal kat)
- Boss katları sweep edilemez → oyuncu 2 boss savaşı yapar (4.500 altın)
- Toplam: **8.500 altın** (savaşla aynı)
- Süre farkı: sweep 8 kat ~5 saniye vs savaş 8 kat ~4-8 dakika

## Edge Cases

- **If oyuncunun enerjisi 1 kaldıysa (2'den az) ve kata girmek isterse**: Giriş engellenir. "Enerji yetersiz — 2 enerji gerekli" mesajı + enerji yenilenme zamanlayıcısı gösterilir. Elmas ile enerji yenileme seçeneği sunulur.

- **If oyuncu Dalga 1'i yenip Dalga 2'de tüm canavarları kaybederse (boss katı)**: Kat başarısız sayılır. Enerji harcanmaz. Dalga 1 loot'u verilmez — loot yalnızca kat tamamen temizlendiğinde verilir. "Tekrar Dene" seçeneğinde Dalga 1'den başlanır.

- **If oyuncu zaten temizlediği bir katı tekrar oynayıp kaybederse**: Kat durumu "Cleared" kalır — tekrar kayıpla Unlocked'a düşmez. Enerji harcanmaz, loot yok.

- **If oyuncu son katı (Kat 10) temizledikten sonra "Devam" seçerse**: Bir sonraki kat yoktur (MVP'de 10 kat sınır). "Tüm katları temizlediniz! Yeni bölge yakında..." mesajı gösterilir. Oyuncu harita ekranına döner, mevcut katları tekrar oynayabilir veya sweep yapabilir.

- **If boss katında oyuncu Dalga 2'yi bitirip Dalga 3 (boss) öncesinde çekilirse**: Kat başarısız. Dalga 1-2 loot'u verilmez. Enerji harcanmaz.

- **If toplu sweep sırasında enerji yarıda biterse (ör. 5 kat seçildi ama 4 kat için enerji var)**: Sweep başlamadan enerji kontrolü yapılır. Yetersiz enerji varsa uyarı: "X kat için yeterli enerji — Y kat eksik." Oyuncu mevcut enerjiyle devam edebilir veya iptal edebilir.

- **If sweep sırasında canavar düşer ve envanter doluysa**: Normal kurallar uygulanır — canavar bekleme alanına eklenir (Canavar Toplama GDD Kural 4). Sweep loot raporu beklemeye alınan canavarları gösterir.

- **If oyuncu ilk kez bir katı temizlerken aynı anda karşılaştığı düşmanlardan yeni tür keşfederse**: Hem first_clear elmas ödülü hem Pokédex keşif ödülü (5 elmas karşılaşma + 10 elmas sahiplik) verilir. Ödüller bağımsızdır, birikir.

- **If boss katında boss yenildiğinde yardımcı düşmanlar hâlâ hayattaysa**: Savaş devam eder — boss özel bir "savaş sonu" tetikleyicisi değildir. Tüm düşmanlar savaş dışı olmalı. Boss'un yardımcıları bağımsız birimlerdir.

- **If difficulty_multiplier ileride değiştirilirse (bölge bazlı)**: Düşman seviyesi yeniden hesaplanır. Daha önce temizlenmiş katların durumu değişmez — zorluk yalnızca giriş anında hesaplanır.

- **If oyuncu sweep ile boss katını seçmeye çalışırsa**: Seçim engellenir — boss katı sweep listesinde devre dışı (gri, seçilemez). "Boss katları sweep edilemez" tooltip gösterilir.

- **If Dalga 1 yenildikten sonra uygulama arka plana atılırsa (mobil)**: Dalga arası durum kaydedilir. Uygulama geri döndüğünde Dalga 2 başlar. Uzun süre arka planda kalınırsa (>30dk) oturum iptal edilir, kat başarısız, enerji harcanmaz.

- **If aynı kattaki farklı dalgalarda aynı canavar türü çıkarsa**: Normal durum — bölge havuzundan rastgele seçim. Her düşman karşılaşması Pokédex'i güncelleyebilir ama zaten keşfedilmişse tekrar ödül verilmez.

- **If oyuncu 100 enerjiyle 50 kat sweep yapmak isterse**: Toplu sweep 50 kat × 2 enerji = 100 enerji, 100 enerji yeterli. Tüm katlar sweep edilir (boss katları hariç → gerçekte 40 normal kat + boss katlarında savaş gerekli). Oyuncu bunu planlı yapabilmeli.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Savaş Sistemi** | Sert | `OnBattleComplete(result)` — savaş sonucu bildirimi | Olmadan kat sonucu belirlenemez |
| **Loot / Ödül** | Sert | `DistributeFloorLoot(floorNumber, floorType, regionId)` — loot hesaplama ve dağıtım | Olmadan kat ödülü verilemez |
| **Canavar Toplama** | Yumuşak | `OnEnemyEncountered(monsterId)` — Pokédex keşif kaydı | Olmadan keşif çalışmaz ama zindan ilerler |
| **Ekonomi** | Sert | `HasEnergy(amount)`, `SpendEnergy(amount)` — enerji kontrolü ve harcama | Olmadan kata giriş kontrol edilemez |
| **Düşman AI** | Sert | `GetEnemyConfig(monsterId, level)` — düşman stat yapılandırması | Olmadan düşman grubu oluşturulamaz |
| **Canavar Veritabanı** | Sert | `GetRegionMonsterPool(regionId)` — bölge canavar havuzu | Olmadan düşman türleri belirlenemez |
| **Takım Kurma** | Sert | `GetActiveTeam()` — aktif takım verisi | Olmadan savaşa girilecek takım bilinmez |
| **Sağlık / Can Sistemi** | Sert | `FullHeal(teamId)` — kat sonu tam iyileşme; dalgalar arası HP korunur (Sağlık/Can Kural 6) | Olmadan HP yönetimi çalışmaz |
| **Kaydetme/Yükleme** | Sert | `SaveDungeonState()` / `LoadDungeonState()` — ilerleme persist | Olmadan kat durumları ve first_clear kaybolur |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Savaş Sistemi** | Sert | `GetFloorEnemies(floorNumber, waveIndex)` — kat düşman listesi | Savaşta düşman grubu bu sistemden gelir |
| **Zindan Harita UI** | Sert | `GetFloorStates()`, `GetRegionInfo()` — harita verisi | UI verileri bu sistemden gelir *(GDD henüz yazılmadı — Not Started)* |
| **Otofarm / Idle** | Sert | `GetHighestClearedFloor()` — idle hesaplama referansı | Idle ilerlemesi en yüksek kata bağlı |
| **Loot / Ödül** | Sert | `GetCurrentFloorInfo()` — kat bilgisi loot tablosu seçimi için | Loot tablosu seçimi kat bilgisine bağlı |

**Çift yönlü**: Savaş Sistemi ↔ Zindan Keşif (düşman listesi sağlar ←, savaş sonucu alır →). Loot ↔ Zindan Keşif (kat bilgisi sağlar →, loot tablosu uygular ←).

**Bidirectional check**:
- Savaş Sistemi GDD: Zindan Keşif upstream + downstream olarak listelenmiş ✅
- Loot GDD: Zindan Keşif upstream olarak listelenmiş ✅
- Canavar Toplama GDD: Zindan Keşif downstream olarak listelenmiş ✅
- Sağlık/Can GDD: Zindan Keşif downstream olarak listelenmiş ✅
- Ekonomi GDD: Enerji sistemi tanımlı ✅
- Düşman AI GDD: enemy_level_formula tanımlı ✅
- Zindan Harita UI: henüz yazılmadı *(Not Started)*
- Otofarm GDD: ✅ Yazıldı — arayüzler doğrulandı

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `energy_per_floor` | 2 | 1–5 | Oturum çok kısa → frustrasyon, enerji hızla biter | Oturum çok uzun → enerji anlamsız, doğal durma noktası yok |
| `difficulty_multiplier` | 1.0 | 0.5–2.0 | Katlar çok zor → oyuncu duvar görür, "biraz önde" bozulur | Katlar çok kolay → meydan okuma yok, sıkılma |
| `boss_floor_interval` | 5 | 3–10 | Boss çok sık → özel hissetmez | Boss çok seyrek → uzun süre doruk an yok |
| `wave_count_normal` | 2 | 1–3 | Kat çok uzun → mobil oturum sınırını aşar | Tek dalga = savaş çok kısa, HP taşıma stratejisi yok |
| `wave_count_boss` | 3 | 2–4 | Boss katı çok uzun, frustrasyon | Boss katı normal kattan farklı hissettirmez |
| `base_first_clear_gems` | 5 | 2–10 | Elmas enflasyonu | İlk temizleme heyecanı düşük |
| `boss_first_clear_gems` | 50 | 25–100 | Boss elması çok cömert → ekonomi bozulur | Boss ödülü heyecansız |
| `final_boss_first_clear_gems` | 100 | 50–200 | Son boss çok cömert | Son boss ödülü yetersiz |
| `sweep_enabled` | true | true/false | — | Sweep kapalı → farm sıkıcı hale gelir |
| `boss_sweep_allowed` | false | true/false | Boss sweep açıksa boss savaşı anlamsızlaşır | — (false uygun) |
| `max_floors_per_region` | 10 | 5–50 | İçerik gereksinimi artar, dengeleme zorlaşır | Bölge çok kısa, keşif hissi yok |
| `wave_1_enemy_count_early` | 2 | 1–3 | Erken katlar zor | Erken katlar çok kolay |
| `wave_2_enemy_count_late` | 4 | 3–5 | Geç katlar çok zor | Düşman sayısı artışı hissedilmez |
| `background_timeout_minutes` | 30 | 10–60 | Arka planda çok uzun bekleme → stale state riski | Kısa mola bile oturumu iptal eder |

**Etkileşim Uyarıları**:
- `energy_per_floor` × `max_energy` (registry=100) birlikte oturum uzunluğunu belirler: 100/2=50 kat vs 100/5=20 kat. Enerji maliyetini artırmak oturum kapasitesini düşürür.
- `boss_floor_interval` × `max_floors_per_region` birlikte bölge başına boss sayısını belirler: 10/5=2 boss. Interval'ı düşürmek daha fazla boss = daha fazla boss loot.
- `difficulty_multiplier` Düşman AI GDD'deki `enemy_level_formula`'yı doğrudan etkiler. Değiştirirken Loot GDD'deki beklenen loot/10 kat tablosunu da kontrol et.
- `base_first_clear_gems` × `max_floors_per_region` = bölge başına ilk temizleme elması. MVP: 8×5 + 50 + 100 = 190 elmas. Canavar Toplama GDD Pokédex elmaslarıyla (toplam ~1.150) birlikte Ekonomi elmas bütçesine uymalı.
- `wave_count_normal` × ortalama savaş süresi (30sn-2dk) = kat süresi. 2 dalga × ~45sn = ~1.5dk/kat. Mobil oturum hedefi 5-15 dk = ~3-10 kat, uyumlu.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Zindan giriş | Karanlık kapı açılma animasyonu + bölge renginde ışık patlaması | 1.5s | MVP |
| Kat geçiş (dalga arası) | Kısa ekran geçiş efekti — düşmanlar sağdan kayarak çıkar, yeni dalga sağdan girer | 0.8s | MVP |
| Kat temizleme | Altın parlama + "KAT TEMİZLENDİ" yazısı + loot ikonları belirme | 1.5s | MVP |
| Boss katı giriş | Ekran kararma + dramatik aydınlanma + boss silüet belirme + boss isim kartı | 2.0s | MVP |
| Boss yenilgisi | Ekran sarsıntısı + boss patlama efekti + altın yağmuru + "BOSS YENİLDİ!" yazısı | 2.5s | MVP |
| İlk temizleme ödülü | Yıldız ikonu parlama + "İLK TEMİZLEME!" damgası + elmas animasyonu | 1.5s | MVP |
| Sweep başlangıcı | Hızlı ileri sarma efekti — kat ikonları hızlıca geçer | 0.3s/kat | MVP |
| Sweep sonucu | Toplu loot özet ekranı — altın sayacı + ikon listesi | 2.0s | MVP |
| Zindan haritası — temizlenmiş kat | Parlak ikon, yıldız işareti, açık renk | Sürekli | MVP |
| Zindan haritası — kilitli kat | Koyu gri ikon, kilit ikonu, siluet | Sürekli | MVP |
| Zindan haritası — mevcut kat (en yüksek açık) | Parlayan çerçeve + pulse animasyonu | Sürekli pulse | MVP |
| Enerji yetersiz uyarısı | Enerji barı kırmızı flash + "Enerji yetersiz" popup | 1.0s | MVP |
| Yeni bölge açılma (Tier 2+) | Haritada yeni alan belirme + keşif müziği + ışık yayılma efekti | 3.0s | Tier 2 |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Zindan giriş | Ağır kapı açılma + yankılı ortam sesi | Gerilim, macera | MVP |
| Kat geçiş (dalga arası) | Kısa geçiş swoosh + yeni düşman giriş sesi | Ritimli, hızlı | MVP |
| Kat temizleme | Zafer akordu (kısa, 1sn) + loot toplama sesi | Tatmin, başarı | MVP |
| Boss katı giriş | Dramatik trompet + düşük bas rumble | Tehdit, heyecan | MVP |
| Boss yenilgisi | Epik zafer fanfarı (2-3sn) + patlama + altın yağmuru | Kutlama, başarı | MVP |
| İlk temizleme ödülü | Yıldız kazanma chime + elmas tıngırtısı | Ödüllendirici | MVP |
| Sweep | Hızlı "whoosh" + toplu loot ses efekti | Verimli, hızlı | MVP |
| Enerji yetersiz | Düşük ton "buzz" + engel sesi | Yumuşak engel, cezalandırıcı değil | MVP |
| Zindan ambiyansı | Bölge bazlı ortam müziği döngüsü (Karanlık Orman: gizemli, düşük tempo) | Atmosferik | MVP |

> **Asset Spec Flag**: Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:zindan-kesif` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- **Zindan Harita Ekranı**: Dikey tower/kule formatı — katlar alttan üste sıralı. Her kat daire veya kare ikon: temizlenmiş=parlak+yıldız, kilitli=gri+kilit, açık=renkli+girilebilir, boss katı=büyük+taç ikonu. Oyuncunun mevcut konumu (en yüksek açık kat) vurgulanır.
- **Kat bilgi popup**: Kata dokunulduğunda popup — kat numarası, kat tipi (Normal/Boss), beklenen düşman nadirliği, ilk temizleme durumu (temizlendi/temizlenmedi), "Giriş" / "Sweep" / "Geri" butonları.
- **Enerji göstergesi**: Ekran üstünde enerji barı (mevcut/maks) + yenilenme zamanlayıcısı. Barın yanında "+" butonu (elmas ile yenileme).
- **Dalga göstergesi**: Savaş sırasında ekran üstünde "Dalga 1/2" veya "Dalga 2/3" göstergesi. Dalga arası geçişte kısa animasyon.
- **Sweep butonu**: Temizlenmiş katlarda "Sweep" butonu aktif. Boss katlarında devre dışı (gri). Toplu sweep: "Toplu Sweep" butonu → kat seçimi listesi → toplam enerji maliyeti + "Başlat" butonu.
- **Sweep sonuç ekranı**: Toplu loot özeti — her kat için kısa satır + toplam altın, toplam canavar, toplam malzeme.
- **Kat sonuç ekranı**: Kat temizleme sonrası — "Devam" (sonraki kat), "Tekrar" (aynı kat), "Çık" (haritaya dön) butonları. İlk temizlemeyse "İLK TEMİZLEME!" vurgusu + elmas ödülü.
- **Kat başarısız ekranı**: "Tekrar Dene" + "Çık" butonları. "Enerji harcanmadı" bildirimi. Takım güç önerisi: "Takımını güçlendir!" yönlendirmesi.
- **Bölge seçimi (Tier 2+)**: Yatay bölge haritası — bölge ikonları soldan sağa. Kilitli bölgeler siluet. Seçilen bölge zindan haritasını açar.
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar. Kat ikonları minimum 48×48 dp.

> **UX Flag — Zindan Keşif Sistemi**: Bu sistem kapsamlı UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak zindan harita ekranı, kat bilgi popup, sweep akışı ve bölge seçimi için UX spec oluşturulmalı — stories `design/ux/zindan-harita.md`'yi referans almalı.

## Acceptance Criteria

1. **GIVEN** oyuncu Kat 1'i henüz temizlememiş, **WHEN** zindan haritasını açarsa, **THEN** Kat 1 "Unlocked" durumunda ve girilebilir, Kat 2-10 "Locked" durumunda.

2. **GIVEN** oyuncu Kat 3'ü temizlemiş, **WHEN** zindan haritasını açarsa, **THEN** Kat 1-3 "Cleared", Kat 4 "Unlocked", Kat 5-10 "Locked".

3. **GIVEN** oyuncunun enerjisi 2, **WHEN** Kat 1'e girerse, **THEN** giriş kabul edilir. Kat temizlenirse enerji 0'a düşer.

4. **GIVEN** oyuncunun enerjisi 1 (2'den az), **WHEN** herhangi bir kata girmeye çalışırsa, **THEN** giriş engellenir ve "Enerji yetersiz" mesajı gösterilir.

5. **GIVEN** oyuncu normal katta (Kat 3), **WHEN** kata girerse, **THEN** 2 dalga düşman grubu oluşturulur: Dalga 1 (2-3 düşman) + Dalga 2 (3-4 düşman).

6. **GIVEN** oyuncu boss katında (Kat 5), **WHEN** kata girerse, **THEN** 3 dalga oluşturulur: Dalga 1 (2-3 normal) + Dalga 2 (3-4 normal) + Dalga 3 (1 Boss + 1-2 yardımcı).

7. **GIVEN** oyuncu Dalga 1'i yenmiş (boss katı), **WHEN** Dalga 2 başlarsa, **THEN** takım canavarlarının HP'si ve enerjisi Dalga 1'den korunur — sıfırlanmaz.

8. **GIVEN** oyuncu Dalga 2'de tüm canavarlarını kaybederse, **WHEN** sonuç hesaplanırsa, **THEN** kat başarısız, enerji harcanmaz, loot yok, "Tekrar Dene" seçeneği gösterilir.

9. **GIVEN** oyuncu bir katı temizlerse, **WHEN** loot hesaplanırsa, **THEN** 2 enerji harcanır, Loot Sistemi tetiklenir ve loot dağıtılır.

10. **GIVEN** oyuncu Kat 3'ü ilk kez temizlerse, **WHEN** loot dağıtılırsa, **THEN** normal loot + 5 elmas first_clear ödülü verilir.

11. **GIVEN** oyuncu Kat 5 boss'unu ilk kez yenerse, **WHEN** loot dağıtılırsa, **THEN** normal loot + 50 elmas first_clear ödülü verilir.

12. **GIVEN** oyuncu Kat 10 son boss'unu ilk kez yenerse, **WHEN** loot dağıtılırsa, **THEN** normal loot + 100 elmas + özel başarım verilir.

13. **GIVEN** oyuncu daha önce temizlediği Kat 3'ü tekrar oynayıp kaybederse, **WHEN** sonuç hesaplanırsa, **THEN** kat durumu "Cleared" kalır, enerji harcanmaz.

14. **GIVEN** oyuncu temizlenmiş normal katı (Kat 3) sweep seçerse, **WHEN** sweep yapılırsa, **THEN** 2 enerji harcanır, normal loot tablosu uygulanır, savaş atlanır, anlık sonuç gösterilir.

15. **GIVEN** oyuncu temizlenmiş boss katını (Kat 5) sweep seçmeye çalışırsa, **WHEN** kat seçiminde, **THEN** boss katı sweep listesinde devre dışıdır ve seçilemez.

16. **GIVEN** oyuncu 5 katı toplu sweep seçer ve 10 enerjisi var, **WHEN** sweep başlatılırsa, **THEN** 5 × 2 = 10 enerji harcanır, 5 kat loot'u toplu hesaplanır.

17. **GIVEN** oyuncu Kat 7 düşman grubunda yeni canavar türüyle karşılaşırsa, **WHEN** savaş başlarsa, **THEN** `OnEnemyEncountered` tetiklenir ve Pokédex güncellenir.

18. **GIVEN** Kat 5 boss katında düşman grubu oluşturulurken, **WHEN** boss türü belirlenirse, **THEN** boss Mini-Boss türünde olur (Kural 7 tablosu).

19. **GIVEN** Kat 10 boss katında, **WHEN** düşman grubu oluşturulursa, **THEN** normal düşmanlar Yüksek güçte, boss Alan Patronu türünde olur.

20. **GIVEN** oyuncu son katı (Kat 10) temizlemiş, **WHEN** "Devam" seçerse, **THEN** "Tüm katları temizlediniz!" mesajı gösterilir ve harita ekranına döner.

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **Evrim Zindanı**: Loot GDD'de "Evrim Zindanı" (Evrim Taşı farm) referans ediliyor. Bu özel zindan tipi MVP'de mi yoksa Tier 2+'da mı? Eğer MVP'deyse dalga yapısı ve loot tablosu nasıl farklılaşır? → Mevcut karar: MVP dışı (Tier 2+).

2. **Prosedürel kat düzenleri**: Game Concept'te "MVP'de sabit, sonra prosedürel" diyor. MVP'de düşman grubu yapısı sabit mi (her oyunda aynı düşmanlar) yoksa bölge havuzundan rastgele mi seçiliyor? → Mevcut tasarım: düşman türleri bölge havuzundan rastgele, sayı ve nadirlik konfigürasyonla sabit.

3. ~~**Sweep pity etkileşimi**~~: ✅ Çözüldü — Sweep edilen katlarda pity counter normal katla aynı etkiyi yapar. Counter artar, canavar düşerse sıfırlanır. Sweep'in avantajı yalnızca süre (anlık vs savaş süresi). Loot GDD'ye yansıtılmalı.

4. **Kat arası takım düzenleme**: Takım Kurma GDD "kat arası düzenleme var" diyor. Bu durum dalga arası mı yoksa kat arası mı geçerli? → Mevcut tasarım: dalga arası düzenleme yok (HP/enerji korunur), kat arası düzenleme var (kat sonuç ekranından).

5. ~~**Arka plan kat tamamlama**~~: ✅ Çözüldü — Otofarm / Idle Sistemi GDD'sinde tanımlandı (Approved). Aktif savaş sırasında arka plana geçiş bu GDD'de `background_timeout_minutes=30` ile yönetilir; idle farm ayrı sistem.

6. **Difficulty_multiplier bölge bazlı ölçekleme**: Tier 2+'da farklı bölgeler farklı difficulty_multiplier kullanabilir mi? (ör. Bölge 2: 1.5x) → Evet, yapı buna uygun tasarlandı. Detaylar Tier 2+ genişletmesinde.
