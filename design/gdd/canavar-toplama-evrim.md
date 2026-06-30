# Canavar Toplama ve Evrim (Monster Collection & Evolution)

> **Status**: Designed
> **Author**: user + game-designer
> **Last Updated**: 2026-06-25
> **Implements Pillar**: Topla Hepsini, Güç Hisset

## Overview

**Canavar Toplama ve Evrim**, oyuncunun canavar koleksiyonunun yaşam döngüsünü yöneten sistemdir: canavarların kazanılması, koleksiyona eklenmesi, envanterinin yönetilmesi ve koleksiyon ilerlemesinin takibi. Loot / Ödül Sistemi'nden `OnMonsterDropped` sinyaliyle yeni canavar instance'ları oluşturur (Canavar Veritabanı şablonlarından), bunları oyuncunun genişletilebilir envanterinde depolar ve Canavar Güçlendirme'ye instance verilerini sağlar. Pokédex tarzı bir kayıt defteri tüm canavar türlerini takip eder — sahip olunmayanlar kilitli siluet olarak görünür, tamamlama yüzdesi ve keşif ödülleri koleksiyon genişletme motivasyonu yaratır.

Oyuncu perspektifinden bu sistem, "Topla Hepsini" sütununun kalbidir: zindandan yeni bir canavar kazanmak, koleksiyon defterinde yeni bir giriş açmak, tamamlama yüzdesinin yükselmesini görmek ve nadir bir keşfin heyecanını yaşamak. Kopya canavarlar çöp değil — yıldız birleştirme malzemesi veya altına satış kaynağıdır. Envanter yönetimi stratejik bir katman ekler: sınırlı ama genişletilebilir kapasite, oyuncuyu hangi canavarları tutacağı konusunda bilinçli seçimler yapmaya yönlendirir.

MVP kapsamında 15-20 canavar türü, başlangıç envanter kapasitesi (elmasla genişletilebilir), Pokédex kayıt defteri, canavar satma mekaniği, temel koleksiyon tamamlama ödülleri ve Loot→Instance→Güçlendirme pipeline'ı tanımlanır.

## Player Fantasy

Canavar Toplama ve Evrim'de oyuncu **iki katmanda** fantezi yaşar:

**Doğrudan deneyim — "Koleksiyoncu Lord"**: Oyuncunun en heyecanlı anı, zindanda yeni bir canavar düştüğünde koleksiyon defterinde kilitli bir silueti açma anıdır. "Bu canavarı daha önce görmedim!" keşfi, Pokédex'in 30 yıldır kanıtladığı tamamlama dürtüsünü tetikler — %73 tamamlandı, %74'e çıkmak için bir canavar daha lazım. Her yeni keşif koleksiyonu bir adım büyütür, koleksiyon büyüdükçe tamamlama ödülleri açılır ve "hepsini topla" fantezisi kendini besleyen bir döngüye dönüşür. Nadir bir canavar düşmesi — Epik veya Efsanevi — oyunun doruk anlarından biridir: "Bunu buldum!" heyecanı.

Kopya canavarlar bile değerlidir: yıldız birleştirme malzemesi olarak güce, satıldığında altına dönüşür. "Bu kopya boşa gitmedi" hissi, koleksiyon motivasyonunu kopya düşmelerinde bile canlı tutar.

**Dolaylı altyapı — "Stratejik Yönetici"**: Envanter yönetimi oyuncuyu bilinçli seçimler yapmaya yönlendirir: sınırlı kapasite, hangi canavarları tutup hangilerini satacağına karar vermeyi gerektirir. Bu seçimler stratejik derinlik katar — tam envanterde yeni bir Nadir canavar düşmesi "kimi göndereyim?" kararını tetikler. Genişletilebilir kapasite elmas sink'i olarak çalışırken, oyuncuya "daha büyük koleksiyonun gücü" hissini verir.

**Çekirdek duygu**: Keşif heyecanı + tamamlama tatmini. Her zindan girişi potansiyel bir yeni keşif, her keşif koleksiyonun bir adım büyümesi, her tamamlama ödülü somut güçlenme.

**Negatif fantezi (kaçınılacak)**: "Envanter dolu, hiçbir şey yapamıyorum" çıkmazı — loot düştüğünde beklemeye alma mekanizması bunu önler. "50 zindan boyunca hep aynı Common" monotonluğu — Loot GDD'nin pity ve nadirlik artışı bu sorunu zaten ele alır.

**Pillar bağlantısı**: "Topla Hepsini" — Pokédex tamamlama dürtüsü, keşif ödülleri. "Güç Hisset" — her toplanan canavar potansiyel güce dönüşür (yıldız, takım genişlemesi). "Cömert Zindan" — her zindan çıkışı koleksiyona en az bir katkı bırakır.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Canavar Kazanma Yolları**

MVP'de canavarlar iki kaynaktan kazanılır:

| Kaynak | Tetikleyici | Detay |
|--------|-------------|-------|
| **Zindan Loot** | Normal kat temizleme | Loot GDD Kural 3: %15 base oran, pity ile %45'e kadar |
| **Boss Drop** | Boss katı temizleme | Loot GDD Kural 4-6: normal canavar %35 + boss kendisi %1-5 |

İleride eklenebilecek kaynaklar (Tier 2+): koleksiyon milestone ödülleri, arena ödülleri, etkinlik ödülleri. MVP'de bu kaynaklar yoktur.

**Kural 2 — Canavar Instance Oluşturma**

Bir canavar düştüğünde Loot sistemi `OnMonsterDropped(monsterId)` sinyali gönderir. Bu sistem şu adımları uygular:

1. Canavar Veritabanı'ndan tür şablonunu çek: `GetMonsterDefinition(monsterId)`
2. Benzersiz `instance_id` ata (UUID)
3. Instance verilerini başlat:

| Alan | Başlangıç Değeri | Kaynak |
|------|-------------------|--------|
| `instance_id` | UUID | Bu sistem |
| `template_id` | monsterId | Loot sinyali |
| `level` | 1 | Sabit |
| `current_xp` | 0 | Sabit |
| `evolution_stage` | 1 (Form A) | Sabit |
| `star_rank` | 0 | Sabit |
| `pity_counter` | 0 | Sabit |
| `acquired_date` | timestamp | Bu sistem |
| `is_locked` | false | Sabit |
| `is_favorite` | false | Sabit |

4. Koleksiyon defterinde bu tür ilk kez keşfedildiyse → `first_discovery` flag'i + keşif ödülü
5. Envantere ekle (kapasite kontrolü sonrası)

**Kural 3 — Envanter Sistemi**

| Parametre | Değer | Açıklama |
|-----------|-------|----------|
| Başlangıç kapasitesi | 20 slot | Yeni oyuncu |
| Slot başına canavar | 1 | Nadirlik/seviye farketmez |
| Genişletme boyutu | +5 slot / genişletme | Sabit artış |
| Genişletme maliyeti | Kademeli artan (Formül 1) | Elmas ile ödenir |
| Maksimum genişletme | 8 kez | Hard cap |
| Maksimum kapasite | 60 slot | 20 + (8 × 5) |

Envanter kuralları:
1. Her canavar instance'ı 1 slot kaplar — nadirlik, seviye veya evrim aşaması farketmez
2. Takımda aktif olan canavarlar envanter slotu kullanır (ayrı takım slotu yok)
3. Kilitlenen canavarlar (`is_locked = true`) satılamaz ve otomatik satıştan muaftır
4. Favori canavarlar (`is_favorite = true`) listede üste sabitlenir ve satış uyarısı alır

**Kural 4 — Envanter Dolu Durumu**

Envanter kapasitesi doluyken yeni canavar düşerse:

1. Canavar **bekleme alanına** (pending buffer) eklenir
2. Bekleme alanı kapasitesi: 10 canavar (envanter dışı, geçici)
3. Bekleme süresi nadirliğe göre değişir:
   - Common / Uncommon: **7 gün**
   - Rare / Epic / Legendary: **14 gün** + push notification gönderilir ("Nadir canavarınız beklemede — [X] gün kaldı!")
4. Süre dolduğunda bekleme alanındaki ilgili canavar otomatik satılır (altına çevrilir)
5. Bekleme alanı da doluysa: yeni canavar otomatik satılır ve altın olarak eklenir (loot raporu "envanter dolu — satıldı" notu gösterir)
6. Oyuncu bilgilendirilir: "Envanter dolu! [X] canavar beklemede — yer aç veya sat."

**Kural 5 — Koleksiyon Defteri (Pokédex)**

Tüm canavar türleri ve evrim formları koleksiyon defterinde listelenir:

| Durum | Görünüm | Bilgi |
|-------|---------|-------|
| **Keşfedilmedi** | Kilitli siluet, "???" isim | Sadece element rengi ipucu |
| **Keşfedildi** (sahip değil) | Gri ikon, isim gösterilir | Temel bilgi: element, arketip, nadirlik |
| **Sahip (PERMANENT)** | Tam renkli ikon | Tüm bilgi: stat, yetenek, lore. Bu durum kalıcıdır — satış sırasında geri alınmaz |

Keşif koşulları:
- Bir canavarı **kazanmak** (loot'tan almak) → otomatik keşif + "Sahip" (kalıcı)
- Bir canavara karşı **savaşmak** (düşman olarak görmek) → otomatik keşif (siluet → gri)

**Evrim formları**: Her canavar türü (ör. "Fire Striker") 3 ayrı Pokédex giriş olarak sayılır: Form A, Form B, Form C. Form C'ye ulaşmak, o tür için "Sahip" koşulunu karşılar. Tüm formlar ayrı ayrı satın alınıp koleksiyona eklenmişse veya Form A'ya sahipsen, o türü "keşfedilmiş" olarak işaretlersin.

**Kural 6 — Koleksiyon Ödülleri**

İki katmanlı ödül sistemi:

**a) Keşif Ödülleri** (her yeni tür keşfedildiğinde):

| Olay | Ödül |
|------|------|
| Yeni tür keşfedildi (ilk karşılaşma) | 5 elmas |
| Yeni tür sahip olundu (ilk kazanım) | 10 elmas + 500 altın |

**b) Milestone Ödülleri** (koleksiyon tamamlama yüzdesine göre):

| Milestone | Koşul (MVP: 20 tür) | Ödül |
|-----------|---------------------|------|
| %25 | 5 tür sahip | 50 elmas + 5.000 altın |
| %50 | 10 tür sahip | 100 elmas + 15.000 altın + 1 XP İksiri (Büyük) |
| %75 | 15 tür sahip | 200 elmas + 30.000 altın + 3 XP İksiri (Büyük) |
| %100 | 20 tür sahip | 500 elmas + 100.000 altın + özel başlık: "Canavar Lordu" |

Milestone ödülleri tek seferlik — ikinci kez verilemez.

**Milestone tetikleme zamanlaması**: Milestone kontrolü **her yeni benzersiz tür kazanımında** çalışır (unique_owned_count artışı). Satış sonrası kontrol gerekmez — milestone'lar tek seferlik olduğundan geri alınmaz. Completion_pct UI'da anlık güncellenir ama milestone tetikleyici sadece kazanımdadır.

**Kural 7 — Canavar Satma**

Üç satış modu:

| Mod | Açıklama |
|-----|----------|
| **Tek satış** | Envanter listesinden canavar seç → onayla → altın kazan |
| **Toplu satış** | Birden fazla canavar seç (checkbox) → toplu onayla → toplam altın |
| **Otomatik satış filtresi** | Kurallar belirle, eşleşen düşen canavarlar otomatik satılır |

Satış fiyatları (registry'den — `monster_sell_value`):
| Nadirlik | Satış Fiyatı |
|----------|-------------|
| Common | 100 altın |
| Uncommon | 200 altın |
| Rare | 400 altın |
| Epic | 700 altın |
| Legendary | 1.000 altın |

Güçlendirilmiş canavarlar bonus fiyatla satılır:
`sell_price = base_sell_value × (1 + level × 0.02 + star_rank × 0.10)`

Satış kısıtlamaları:
1. Son canavar satılamaz (minimum 1 canavar koleksiyonda kalmalı)
2. Takımda aktif canavar satılamaz (önce takımdan çıkar)
3. Kilitli canavar satılamaz (önce kilidi kaldır)
4. Favoriler satılırken ek onay istenir: "Bu canavar favorinizde! Emin misiniz?"

**Kural 8 — Otomatik Satış Filtresi**

Oyuncu yapılandırılabilir kurallar belirler:

| Parametre | Seçenekler | Varsayılan |
|-----------|-----------|-----------|
| Nadirlik filtresi | Common / Uncommon / Kapalı | Kapalı |
| Yıldız filtresi | ★0 / ★1 altı / Kapalı | Kapalı |
| Güvenlik kilidi | Rare+ otomatik satış filtresinden muaf | Açık (kapatılamaz) |

Otomatik satış kuralları:
1. Filtre **sadece yeni düşen** canavarlara uygulanır — mevcut envantere dokunmaz
2. **Rare, Epic, Legendary canavarlar otomatik satış filtresinden muaftır** — güvenlik kilidi. (Not: bekleme alanı süresi dolduğunda Rare+ canavarlar da satılabilir — bkz. Kural 4.3. Bekleme süresi 14 güne uzatılmıştır.)
3. Otomatik satılan canavarlar loot raporunda "otomatik satıldı: +[X] altın" notu ile gösterilir
4. Filtre oyuncu tarafından açılıp kapatılabilir
5. İlk kez keşfedilen (Pokédex'e yeni eklenen) canavar otomatik satılmaz — ilk kopya her zaman koleksiyona eklenir

**Kural 9 — Canavar Kilitleme**

Değerli canavarları yanlışlıkla satmaktan korumak için:
1. Oyuncu herhangi bir canavarı "kilitli" olarak işaretleyebilir
2. Kilitli canavarlar: satılamaz, yıldız birleştirmede malzeme olamaz, otomatik satıştan muaf
3. Kilitleme/kilidi kaldırma ücretsiz ve anlık
4. Takıma eklenen canavarlar otomatik kilitlenmez (oyuncunun tercihi)

### States and Transitions

Canavar instance yaşam döngüsü:

| Durum | Tetikleyici | Sonraki Durum | Sahip Sistem |
|-------|-------------|---------------|--------------|
| **Düşme** | Loot sistemi `OnMonsterDropped` | **Envanterde** veya **Beklemede** | Bu sistem |
| **Beklemede** | Envanter dolu | **Envanterde** (yer açılınca) veya **Satıldı** (7 gün sonra) | Bu sistem |
| **Envanterde** | Instance oluşturuldu, envantere eklendi | **Takımda** / **Güçlendirildi** / **Satıldı** / **Birleştirildi** | Bu sistem |
| **Takımda** | Oyuncu takıma ekledi | **Savaşta** | Takım Kurma |
| **Güçlendirildi** | Seviye/evrim/yıldız artışı | **Envanterde** (daha güçlü) | Canavar Güçlendirme |
| **Birleştirildi** | Yıldız birleştirme malzemesi olarak kullanıldı | **Yok edildi** (tüketildi) | Canavar Güçlendirme |
| **Satıldı** | Oyuncu sattı / otomatik satış / bekleme süresi doldu | **Yok edildi** (altın kazanıldı) | Bu sistem |

**Keşif durumu** (Pokédex — tür bazlı, instance bazlı değil):

| Durum | Tetikleyici | Kalıcı |
|-------|-------------|--------|
| **Keşfedilmedi** | Oyuncu bu türle hiç karşılaşmadı | — |
| **Keşfedildi** | Düşman olarak savaşıldı | Evet |
| **Sahip Olundu** | Instance kazanıldı (en az bir kez) | Evet |

**Not**: Sahip olunma durumu kalıcıdır — bir türü satsan bile Pokédex'te "sahip olundu" olarak kalır. Tamamlama yüzdesi "mevcut sahip olunan benzersiz türler / toplam türler" olarak hesaplanır, satılan türler tamamlama yüzdesinden düşer.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | Tür şablonu (id, element, arketip, nadirlik, evrim yolu) | `GetMonsterDefinition(monsterId)` |
| **Loot / Ödül** | ← alır | Yeni canavar sinyali | `OnMonsterDropped(monsterId)` → instance oluşturma |
| **Canavar Güçlendirme** | → sağlar | Instance verileri (level, xp, stage, star) | `GetMonsterInstance(instanceId)` |
| **Canavar Güçlendirme** | ← çağrılır | Yıldız birleştirmede kopya tüketimi | `ConsumeInstance(instanceId)` → instance yok edilir |
| **Ekonomi** | → çağırır | Satış altını ekleme, envanter genişletme elması çekme | `GrantGold(amount)`, `SpendGems(amount)` |
| **Takım Kurma** | ← sorgular | Mevcut canavar listesi, aktif takımdaki instance'lar | `GetOwnedMonsters(filters?)`, `IsInTeam(instanceId)` |
| **Koleksiyon UI** | → sağlar | Envanter listesi, Pokédex durumu, kapasite bilgisi | `GetInventory()`, `GetPokedexStatus()`, `GetCapacity()` |
| **Kaydetme/Yükleme** | ↔ persist | Tüm instance verileri, Pokédex durumu, otomatik satış kuralları | `SaveCollectionState()` / `LoadCollectionState()` |
| **Zindan Keşif** | ← alır | Düşman canavar karşılaşma sinyali (Pokédex keşfi için) | `OnEnemyEncountered(monsterId)` → keşif kaydı |

## Formulas

### Formül 1: Envanter Genişletme Maliyeti

`expansion_cost = base_expansion_cost × 2 ^ (expansion_count - 1)`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel genişletme maliyeti | base_expansion_cost | int | 50 | İlk genişletme elmas maliyeti |
| Genişletme sayısı | expansion_count | int | 1–8 | Kaçıncı genişletme (hard cap) |
| Genişletme maliyeti | expansion_cost | int | 50–6.400 | Elmas maliyeti |

**Çıktı Aralığı**: 50 (ilk genişletme) – 6.400 (8. ve son genişletme). Hard cap: maksimum 8 genişletme = 60 slot.

**Maliyet tablosu:**

| Genişletme | Slot | Kümülatif Slot | Maliyet (Elmas) | Kümülatif Maliyet |
|-----------|------|----------------|-----------------|-------------------|
| 1 | +5 | 25 | 50 | 50 |
| 2 | +5 | 30 | 100 | 150 |
| 3 | +5 | 35 | 200 | 350 |
| 4 | +5 | 40 | 400 | 750 |
| 5 | +5 | 45 | 800 | 1.550 |
| 6 | +5 | 50 | 1.600 | 3.150 |
| 7 | +5 | 55 | 3.200 | 6.350 |
| 8 | +5 | 60 | 6.400 | 12.750 |

**Denge notu**: 60 slota ulaşmak (8 genişletme) toplamda 12.750 elmas. MVP'de 20 canavar türü var, 60 slot yeterli kopya + ana koleksiyon + yıldız birleştirme malzemesi için. İlk 5 genişletme (45 slot, 1.550 elmas) çoğu oyuncunun doğal tavanı olacaktır.

**Örnek**: 3. genişletme → 50 × 2² = **200 elmas** → 35 slot.

### Formül 2: Güçlendirilmiş Canavar Satış Fiyatı

`sell_price = floor(base_sell_value × (1 + level × 0.02 + star_rank × 0.10))`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel satış fiyatı | base_sell_value | int | 100–1.000 | Registry: `monster_sell_value` |
| Seviye | level | int | 1–50 | Canavar seviyesi |
| Yıldız sırası | star_rank | int | 0–5 | Canavar yıldızı |
| Satış fiyatı | sell_price | int | 102–2.500 | Sonuç altın |

**Çıktı Aralığı**: 102 (Common Lv1 ★0) – 2.500 (Legendary Lv50 ★5)

**Örnek satışlar:**

| Canavar | Level | Yıldız | Hesaplama | Satış Fiyatı |
|---------|-------|--------|-----------|-------------|
| Common Lv1 ★0 | 1 | 0 | 100 × (1 + 0.02 + 0) | **102** |
| Common Lv10 ★0 | 10 | 0 | 100 × (1 + 0.20) | **120** |
| Rare Lv25 ★2 | 25 | 2 | 400 × (1 + 0.50 + 0.20) | **680** |
| Legendary Lv50 ★5 | 50 | 5 | 1.000 × (1 + 1.00 + 0.50) | **2.500** |

**Denge notu**: Satış fiyatları kasıtlı olarak düşük — canavar satma kaynak kazanımının ana yolu olmamalı. Ana altın kaynağı zindan loot'udur. Satış envanter yönetimi aracıdır.

### Formül 3: Koleksiyon Tamamlama Yüzdesi

`completion_pct = floor(unique_owned_count / total_species_count × 100)`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Benzersiz sahip olunan tür | unique_owned_count | int | 0–total | Pokédex'te en az bir kez "Sahip Olundu" durumuna geçmiş türler (permanent) |
| Toplam tür sayısı | total_species_count | int | 60 (MVP) | Canavar Veritabanı toplam tür × evrim formları (20 tür × 3 form A/B/C) |
| Tamamlama yüzdesi | completion_pct | int | 0–100 | Floor ile tam sayı |

**Çıktı Aralığı**: 0% – 100%

**Not (REVISED)**: "Sahip olunan" = Pokédex'te "Sahip Olundu" kalıcı durumuna geçmiş (en az bir kez bir instance kazanılmış). Satılan türler sayılmaya DEVAM EDER. Satış sırasında o tür bir kez daha kazanılmadığı sürece completion_pct sabit kalır. Pokédex "Keşfedildi" durumu ayrı izlenir, completion_pct'ye dokunmaz — yalnızca "Sahip Olundu" (permanent) sayılır.

**Örnek**: 45 tür keşfedildi ve 45 tür sahip (form olsun veya olmasın) / 60 toplam = floor(75) = **%75** → milestone ödülü tetiklenir. Oyuncu sonra 5 Rare türünü satsa: tamamlama hâlâ **%75** (45 still "ever-owned").

## Edge Cases

- **If envanter dolu ve bekleme alanı da doluyken (10/10 beklemede) yeni canavar düşerse**: Yeni canavar otomatik satılır, altına çevrilir. Loot raporunda "envanter + bekleme dolu — otomatik satıldı: +[X] altın" notu gösterilir. Rare+ canavarlar da bu durumda otomatik satılır — ancak Rare+ canavarların bekleme süresi 14 gün olduğundan (Kural 4.3), bu duruma düşme olasılığı düşüktür. Bekleme alanında Rare+ canavar varken alan doluysa, önce Common/Uncommon canavarlar (7 gün süresi daha kısa olanlar) satılarak yer açılır.

- **If oyuncu tüm canavarlarını satmaya çalışırsa (1 canavar kalana kadar)**: Son canavar satılamaz. "Sat" butonu devre dışı kalır. "Koleksiyonda en az 1 canavar bulunmalı" mesajı gösterilir.

- **If otomatik satış filtresi açıkken ilk kez keşfedilen bir Common canavar düşerse**: İlk kopya otomatik satılmaz — Pokédex'e eklenir ve koleksiyona girer. Aynı türün sonraki kopyaları filtre kuralına tabidir.

- **If oyuncu otomatik satış filtresini "Common" olarak ayarlamışken tüm Common canavarlarını satmışsa ve yeni Common düşerse**: Canavar zaten otomatik satılır. Pokédex'te "sahip olundu" durumu korunur ama tamamlama yüzdesinden düşer (aktif instance yok).

- **If canavar bekleme alanında süre dolunca ama oyuncu o sürede hiç giriş yapmadıysa**: Timer oyuncu girişinde hesaplanır (offline süre dahil). Giriş anında süresi dolmuş canavarlar satılır (Common/Uncommon: 7 gün, Rare+: 14 gün), altın eklenir ve bildirim gösterilir. Rare+ canavar satıldıysa özel bildirim: "Nadir canavarınız bekleme süresinde satıldı — [X] altın kazandınız."

- **If yıldız birleştirme için seçilen kopya canavar bekleme alanındaysa**: Bekleme alanındaki canavarlar birleştirme malzemesi olarak seçilebilir — bu onları bekleme alanından kaldırır ve tüketir. Slot açma gerekmez.

- **If oyuncu envanter genişletme satın alırken elması yetersizse**: "Satın Al" butonu devre dışı. Mevcut elmas ve gereken elmas gösterilir. "Elmas kazan" yönlendirmesi sunulur.

- **If Pokédex %100 tamamlama ödülü alındıktan sonra yeni canavar türleri eklenirse (içerik güncellemesi)**: Tamamlama yüzdesi yeniden hesaplanır (ör. 20/22 = %90). Yeni milestone'lar tanımlanmaz — mevcut milestone'lar sabit kalır. İleride Tier 2+ güncellemesinde yeni milestone'lar eklenebilir.

- **If koleksiyon tamamlama yüzdesi tam milestone eşiğinde ama oyuncu önceki milestone'u almamışsa**: Tüm karşılanmış ve alınmamış milestone'lar sırayla verilir. %50'yi atlayıp %75'e ulaşılırsa, hem %50 hem %75 ödülü verilir.

- **If aynı anda iki loot roll aynı tür canavar düşürürse (boss + normal roll)**: Her ikisi de ayrı instance olarak oluşturulur — 2 adet aynı canavar koleksiyona eklenir. Envanter 2 slot kullanır.

- **If oyuncu kilitli bir canavarı toplu satış seçimine dahil etmeye çalışırsa**: Kilitli canavarlar seçilemez (checkbox devre dışı). Seçim listesinde kilitli ikon gösterilir.

- **If favoriye alınmış canavar otomatik satış filtresine uysa bile**: Favori canavarlar otomatik satıştan muaftır — filtre kuralı override edilir.

- **If envanter tam 20/20 iken oyuncu bir canavarı yıldız birleştirmeye verirse (19/20 olur) ve ardından bekleme alanında canavar varsa**: Bekleme alanından en eski canavar otomatik olarak envantere taşınır. FIFO sırası uygulanır.

*`systems-designer` not consulted — Lean mode. Review manually before production.*

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetMonsterDefinition(monsterId)` — tür şablonu (kimlik, stat, element, arketip, nadirlik, evrim yolu) | Olmadan instance oluşturulamaz |
| **Canavar Güçlendirme** | Sert | `GetEnhancedStats(instanceId)` — güçlendirilmiş stat değerleri. `ConsumeInstance(instanceId)` — yıldız birleştirmede kopya tüketimi. `GetEvolutionRequirements(monsterId)` — evrim koşulları. | Olmadan güçlendirme pipeline'ı çalışmaz |
| **Loot / Ödül** | Sert | `OnMonsterDropped(monsterId)` — canavar kazanım sinyali. Nadirlik seçimi Loot tarafından yapılır. | Olmadan canavar kazanma kaynağı yok |
| **Ekonomi** | Sert | `GrantGold(amount)` — satış altını. `SpendGems(amount)` — envanter genişletme. | Olmadan satış ve genişletme çalışmaz |
| **Kaydetme/Yükleme** | Sert | `SaveCollectionState()` / `LoadCollectionState()` — tüm instance verisi, Pokédex, otomatik satış kuralları | Olmadan ilerleme kaybolur |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Zindan Keşif** | Sert | `OnEnemyEncountered(monsterId)` — Pokédex keşfi. Canavar kazanım akışı zindan içinde tetiklenir. | Zindan canavar keşfi bu sisteme bağlı |
| **Koleksiyon / Envanter UI** | Sert | `GetInventory()`, `GetPokedexStatus()`, `GetCapacity()` — envanter ve Pokédex verisi | UI verileri bu sistemden gelir |
| **İlerleme Döngüleri** | Yumuşak | Koleksiyon tamamlama yüzdesi, milestone tetikleyicileri | İlerleme metrikleri |
| **Takım Kurma** | Yumuşak | `GetOwnedMonsters(filters?)` — mevcut canavar listesi | Takım oluşturma havuzu |

### Çapraz bağımlılık notları

- Canavar Veritabanı bu sistemi downstream olarak listeliyor ✅
- Canavar Güçlendirme bu sistemi downstream olarak listeliyor ✅
- Loot / Ödül bu sistemi downstream olarak listeliyor ✅
- Zindan Keşif GDD: Henüz yazılmadı — `OnEnemyEncountered` sinyali burada tanımlandı
- Koleksiyon UI GDD: Henüz yazılmadı — arayüz tanımları burada

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `base_inventory_capacity` | 20 | 15–30 | Başlangıçta çok geniş → genişletme motivasyonu yok | Çok dar → erken frustrasyon, sürekli satma zorunluluğu |
| `inventory_expansion_size` | 5 | 3–10 | Her genişletme çok değerli → ilk genişletme elmas sink çok güçlü | Genişletme anlamsız → motivasyon düşer |
| `base_expansion_cost` | 50 | 25–100 | İlk genişletme pahalı → yeni oyuncu frustrasyonu | İlk genişletme bedava hissettirir → elmas sink yok |
| `max_inventory_capacity` | 60 | 40–100 | Çok geniş → envanter yönetimi gereksiz | Çok dar → endgame koleksiyon sınırlı |
| `max_expansion_count` | 8 | 5–12 | Çok fazla → üstel maliyet anlamsız | Çok az → kapasite artışı yetersiz |
| `pending_buffer_size` | 10 | 5–20 | Bekleme çok geniş → envanter yönetimi gevşer | Bekleme çok dar → canavar kaybı riski artar |
| `pending_expiry_days` | 7 (Common/Uncommon) | 3–14 | Çok uzun → oyuncu umursamaz | Çok kısa → geri dönüş süresi yetersiz |
| `pending_expiry_days_rare` | 14 (Rare+) | 7–30 | Çok uzun → bekleme alanı tıkanır | Çok kısa → nadir canavar kaybı riski |
| `discovery_gem_reward` | 5 | 2–10 | Keşif ödülü çok cömert → elmas enflasyonu | Keşif hissedilmez |
| `first_owned_gem_reward` | 10 | 5–25 | İlk sahiplik çok cömert → elmas enflasyonu | Heyecan düşük |
| `first_owned_gold_reward` | 500 | 200–1.000 | — | — |
| `milestone_25_gems` | 50 | 25–100 | Milestone çok cömert → ekonomi bozulur | Milestone motivasyonu düşer |
| `milestone_100_gems` | 500 | 250–1.000 | — | — |
| `sell_level_bonus` | 0.02 | 0.01–0.05 | Güçlü canavar satmak çok karlı → güçlendirme+sat döngüsü | Seviye farkı satışta hissedilmez |
| `sell_star_bonus` | 0.10 | 0.05–0.20 | Yıldızlı canavar satmak çok karlı → yıldız amacı kayar | Yıldız farkı satışta hissedilmez |

**Etkileşim Uyarıları**:
- `base_inventory_capacity` × `inventory_expansion_size` × `base_expansion_cost` birlikte envanter genişletme ekonomisini belirler. Kapasiteyi artırmak genişletme ihtiyacını azaltır.
- **Diamond Budget Balance (CRITICAL)**: Pokédex'ten toplam elmas kazanımı = 60 tür × (5 keşif + 10 sahiplik) + milestone'lar (50+100+200+500) = ~1.950 elmas. Full expansion sink = 12.750 elmas (8 genişletme). **Tekrarlı kaynaklar gerekli**: Ekonomi GDD'de tanımlanan günlük giriş (7/gün), haftalık görevler (50/hafta), arena ödülleri (20-50/hafta) ile birlikte ortalama 300-400 elmas/ay kazanılır. 60 slota ulaşmak (12.750 elmas) ~42 ay alır — bu uygun bir endgame target'ıdır. Önemli: ilk 5 genişletme (1.550 elmas) MVP expansion için yeterli olmalıdır.
- `discovery_gem_reward` × `first_owned_gem_reward` × toplam tür sayısı (60 form dahil) birlikte Pokédex'ten toplam elmas kazanımını belirler (MVP: 60 form × 15 elmas = 900 elmas + milestone'lar 850 = toplam ~1.750 elmas). Ekonomi GDD tekrarlı kaynaklarıyla dengelenmeli.
- `pending_buffer_size` × `pending_expiry_days` birlikte canavar kaybı riskini belirler. İkisini düşürmek agresif envanter yönetimi zorunluluğu yaratır.
- `sell_level_bonus` × `sell_star_bonus` × max level (50) × max star (5) birlikte maximum satış fiyatını belirler: base × (1 + 1.0 + 0.5) = base × 2.5. Ekonomi GDD altın kaynaklarıyla çakışmamalı.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Yeni canavar keşfi (Pokédex) | Siluet açılma animasyonu — karanlık siluetten renkli ikona geçiş + ışık efekti | MVP |
| İlk sahiplik (yeni tür kazanımı) | Pokédex giriş açılma + "YENİ!" damgası + nadirlik renginde aura | MVP |
| Canavar envantere ekleme | Kart kayma animasyonu + yumuşak parlama | MVP |
| Canavar satma | Canavar kartı küçülme + altın sikke parçacıkları | MVP |
| Toplu satış | Ardışık kart küçülme cascade + toplam altın popup | MVP |
| Otomatik satış bildirimi | Küçük altın ikonu + "otomatik satıldı" metin animasyonu | MVP |
| Milestone ödülü | Tam ekran kutlama — konfeti + nadirlik renginde ışık patlaması + ödül listesi cascade | MVP |
| Envanter genişletme | Envanter ızgarası genişleme animasyonu + yeni slotlar parlama | MVP |
| Bekleme alanı uyarısı | Titreyen zarf ikonu + kırmızı bildirim noktası | MVP |
| Canavar kilitleme | Kilit ikonu yerleşme animasyonu + "klik" efekti | MVP |

### Audio Gereksinimleri

| Olay | Ses | Öncelik |
|------|-----|---------|
| Yeni canavar keşfi | Gizemli açılış tonu + "shimmer" (1 sn) | MVP |
| İlk sahiplik | Keşif fanfarı + nadirlik bazlı jingle (Common=kısa, Legendary=epik) | MVP |
| Canavar satma | Metalik "clink" + yumuşak "swoosh" | MVP |
| Toplu satış | Hızlanan "clink-clink-clink" + toplam "ka-ching" | MVP |
| Milestone ödülü | Kutlama fanfarı (2-3 sn) + konfeti sesi | MVP |
| Envanter genişletme | Kilit açılma sesi + genişleme "whoosh" | MVP |
| Bekleme uyarısı | Yumuşak bildirim tonu | MVP |
| Canavar kilitleme | Kısa "klik" sesi | MVP |

> 📌 **Asset Spec** — Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:canavar-toplama-evrim` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- **Koleksiyon ana ekranı**: İki tab — "Envanter" (sahip olunan canavarlar) ve "Pokédex" (tüm türler). Üst barda: kapasite göstergesi (ör. 17/20), tamamlama yüzdesi (%85), sıralama/filtreleme butonları.
- **Envanter listesi**: Grid görünüm (4×5 ızgara). Her canavar kartı: portre, element ikonu, nadirlik çerçevesi rengi, seviye, yıldız göstergesi. Kilitli canavarlar kilit ikonu gösterir. Favori canavarlar kalp ikonu + üste sabitlenir.
- **Pokédex görünümü**: Grid tüm türleri gösterir. Keşfedilmemişler siyah siluet + "???". Keşfedilmişler gri ikon + isim. Sahip olunanlar tam renkli. Tamamlama barı üstte.
- **Canavar detay ekranı**: Envanterdeki canavarı tıklayınca açılır. Portre (büyük), isim, element, arketip, nadirlik. Statlar (HP/ATK/DEF/SPD) barlarla. Alt butonlar: "Takıma Ekle", "Güçlendir" (→ Güçlendirme ekranı), "Sat", "Kilitle/Aç", "Favori". Lore metni alt bölümde.
- **Satış ekranı**: Tek satış: detay ekranından "Sat" → onay dialogu + satış fiyatı gösterilir. Toplu satış: envanter listesinde "Toplu Sat" modu → checkbox seçimi → toplam fiyat + "Sat" butonu.
- **Otomatik satış ayarları**: Ayarlar ekranında veya envanter ekranında dişli ikonu → filtre kuralları: nadirlik dropdown (Kapalı/Common/Uncommon) + yıldız dropdown (Kapalı/★0/★1 altı). "Rare+ asla satılmaz" güvenlik notu sabit gösterilir.
- **Envanter genişletme**: Envanter ekranında kapasite barı yanında "+" butonu → genişletme dialogu: mevcut kapasite, yeni kapasite, elmas maliyeti, "Satın Al" butonu.
- **Bekleme alanı**: Envanter ekranında sarı uyarı bandı: "[X] canavar beklemede — [Y] gün kaldı". Tıklayınca bekleme listesi açılır. Her canavar: portre + isim + kalan gün + "Envantere Al" (slot varsa) veya "Sat" butonu.
- **Milestone ödül ekranı**: Pokédex'te milestone barı üzerinde ödül ikonları. Alınmamış milestone'lar parlayarak dikkat çeker. Tıklayınca ödül içeriği popup'ta gösterilir.
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar

> 📌 **UX Flag — Canavar Toplama ve Evrim**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak koleksiyon ekranı, Pokédex, envanter yönetimi, satış akışı ve bekleme alanı için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** Loot sistemi bir canavar düşürür, **WHEN** `OnMonsterDropped("fire-striker-infernalclaw")` çağrılırsa, **THEN** yeni instance oluşturulur: level=1, xp=0, evolution_stage=1, star_rank=0 ve envantere eklenir.

2. **GIVEN** oyuncunun envanteri 20/20 dolu, **WHEN** yeni canavar düşerse, **THEN** canavar bekleme alanına eklenir ve "Envanter dolu!" bildirimi gösterilir.

3. **GIVEN** bekleme alanında 7 günden fazla bekleyen Common canavar var, **WHEN** oyuncu giriş yaparsa, **THEN** canavar otomatik satılır (100 altın) ve bildirim gösterilir.

4. **GIVEN** bekleme alanı 10/10 dolu ve envanter 20/20 dolu, **WHEN** yeni canavar düşerse, **THEN** canavar anında otomatik satılır ve loot raporunda not gösterilir.

5. **GIVEN** oyuncu daha önce "fire-striker-infernalclaw" türüyle hiç karşılaşmamış, **WHEN** bu türle savaşırsa (düşman olarak), **THEN** Pokédex'te bu tür "keşfedildi" durumuna geçer (gri ikon, isim görünür) ve 5 elmas ödülü verilir.

6. **GIVEN** oyuncu ilk kez "fire-striker-infernalclaw" türü bir canavar kazanır, **WHEN** instance oluşturulursa, **THEN** Pokédex'te "sahip olundu" durumuna geçer (tam renkli) ve 10 elmas + 500 altın keşif ödülü verilir.

7. **GIVEN** oyuncu 5 benzersiz tür sahip (MVP: %25), **WHEN** tamamlama kontrolü yapılırsa, **THEN** %25 milestone ödülü tetiklenir: 50 elmas + 5.000 altın.

8. **GIVEN** oyuncu 20 benzersiz tür sahip (MVP: %100), **WHEN** tamamlama kontrolü yapılırsa, **THEN** %100 milestone ödülü: 500 elmas + 100.000 altın + "Canavar Lordu" başlığı.

9. **GIVEN** Common Lv10 ★0 canavar, **WHEN** satılırsa, **THEN** satış fiyatı = floor(100 × (1 + 10×0.02 + 0)) = **120 altın**.

10. **GIVEN** Rare Lv25 ★2 canavar, **WHEN** satılırsa, **THEN** satış fiyatı = floor(400 × (1 + 25×0.02 + 2×0.10)) = **680 altın**.

11. **GIVEN** envanterde 1 canavar kalmış, **WHEN** oyuncu satmaya çalışırsa, **THEN** işlem engellenir ve "Son canavar satılamaz" mesajı gösterilir.

12. **GIVEN** kilitli canavar, **WHEN** oyuncu satmaya veya yıldız birleştirme malzemesi yapmaya çalışırsa, **THEN** işlem engellenir.

13. **GIVEN** otomatik satış filtresi "Common" ayarlı, **WHEN** ilk kez keşfedilen Common canavar düşerse, **THEN** otomatik satılmaz — koleksiyona eklenir.

14. **GIVEN** otomatik satış filtresi "Common" ayarlı, **WHEN** daha önce keşfedilmiş Common canavar düşerse (kopya), **THEN** otomatik satılır ve loot raporunda "otomatik satıldı: +100 altın" gösterilir.

15. **GIVEN** 3. envanter genişletme satın alınacak, **WHEN** oyuncu genişletme butonuna basarsa, **THEN** maliyet = 50 × 2² = **200 elmas**, kapasite 30→35 olur.

16. **GIVEN** oyuncu envanter 19/20, bekleme alanında 2 canavar var, **WHEN** envanterde 1 slot açılırsa, **THEN** bekleme alanından en eski canavar otomatik envantere taşınır (FIFO).

17. **GIVEN** favori canavar, otomatik satış filtresi "Common" ayarlı, **WHEN** Common favori canavar düşerse, **THEN** otomatik satılmaz — favori muafiyeti uygulanır.

18. **GIVEN** oyuncu %50 milestone almamış ama %75'e ulaşmış, **WHEN** milestone kontrolü yapılırsa, **THEN** hem %50 hem %75 ödülleri sırayla verilir.

19. **GIVEN** oyuncu 8. genişletmeyi tamamlamış (60/60 kapasite), **WHEN** genişletme butonuna basarsa, **THEN** buton devre dışıdır ve "Maksimum kapasiteye ulaştınız" mesajı gösterilir.

20. **GIVEN** bekleme alanında Rare canavar 13 gündür bekliyor, **WHEN** oyuncu giriş yaparsa, **THEN** canavar henüz satılmaz (14 gün süresi dolmamış) ve push notification gönderilir: "Nadir canavarınız beklemede — 1 gün kaldı!"

21. **GIVEN** bekleme alanında Common canavar 7+ gündür ve Rare canavar 5 gündür bekliyor, **WHEN** oyuncu giriş yaparsa, **THEN** Common canavar otomatik satılır (süresi dolmuş), Rare canavar beklemede kalır (14 gün süresi dolmamış).

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **Başlangıç canavarı**: MVP'de oyuncu ilk canavarını nasıl alır? Tutorial/Onboarding (Tier 2) olmadan ilk zindan girişinde garanti Common canavar düşürülebilir. → Tutorial GDD'sinde netleşecek.

2. **Koleksiyon milestone ödül dengeleme**: Pokédex milestone ödülleri (toplam ~1.150 elmas) + tekrarlı elmas kaynakları Ekonomi GDD'deki toplam elmas kazanım bütçesiyle tutarlı mı? BLOCKER: Ekonomi GDD'nin şu tekrarlı kaynaklarını tanımlaması gerekir: günlük giriş bonusu, haftalık görevler, ödünç al sistemi, arena ödülleri. Expansion sınkı (%20-30) bu kaynakların elde edilebilir olmasına bağlı. → Ekonomi GDD Blocker #3 revizyon.

3. ~~**Evrim formu ve Pokédex** — RESOLVED~~ **CLOSED**: Evrimleşen canavarlar (Form A→B→C) Pokédex'te **ayrı giriş**. Bunu baştan belirtiyoruz: 20 tür × 3 form = 60 Pokédex giriş. total_species_count=60. Formlar bağımsız olarak sahip alınabilir; "Tür keşfedildi" = herhangi bir formu açmış olmak; "Tür sahip" = en az bir form instance'ı kazanılmış (kalıcı). Milestone thresholds ve Formula 3 güncellenmiştir.

4. **Otomatik satış geri alma**: Otomatik satılan canavar geri alınabilir mi? (Geri alma penceresi 30 sn gibi) → MVP'de yok, Tier 2+'da değerlendirilebilir.

5. **Envanter sıralama seçenekleri**: Element, nadirlik, seviye, güç, kazanım tarihi — hangi sıralama seçenekleri MVP'de olmalı? RECOMMENDED (Systems Design Review): Minimum = nadirlik, seviye, element. → UI GDD'sinde detaylandırılacak.
