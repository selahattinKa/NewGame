# Game Concept: Canavar Zindanları (Monster Dungeons)

*Created: 2026-06-23*
*Status: Draft*

---

## Elevator Pitch

> Sınıfını seç, yanına en güçlü peti al ve zindanlara dal. Tur bazlı savaşta yetenek cooldownlarını yönet ya da oto-savaş butonuna bas — her savaş sonu EXP kazanırsın, seviyeni ve petini büyütürsün. Yaratıkları kart olarak topla, F'den SS'e evrimleştir ve mağazadan hem kendine hem petine efsane kostümler al.

---

## Core Identity

| Aspect | Detail |
| ---- | ---- |
| **Genre** | 2D Tur Bazlı RPG + Pet Toplama + Zindan Crawler |
| **Platform** | Mobile (iOS / Android) |
| **Target Audience** | Mid-core mobil oyuncular, koleksiyon/RPG severler (bkz. Player Profile) |
| **Player Count** | Tek oyunculu (Arena: asenkron PvP) |
| **Session Length** | 5-30 dakika (aktif), idle otofarm arası |
| **Monetization** | Hibrit: IAP (kostüm/özel pet — gerçek para) + Reklam geliri (interstitial/rewarded ad) |
| **Orientation** | Portrait (dikey) — tek elle oynanabilir, 9:16 / 19.5:9 |
| **Estimated Scope** | Prototype → Google Play soft launch (2-3 hafta); tam MVP (3-4 ay ek) |
| **Comparable Titles** | AFK Arena, Monster Warlord, Summoners War, Brown Dust 2, Puzzle & Dragons |

---

## Core Fantasy

İki kadim ırk arasında süregelen savaşta seçtiğin tarafın en güçlü savaşçısısın. Yanında sadece tek bir pet var — ama o pet F'den başlayıp SS'e evrimleşen, her adımda görünüşü değişen, savaş alanında video animasyonuyla saldıran bir güç. Sınıfını seç, dalını geliştir, karakter kartını koleksiyonuna ekle — ve zindan katlarını tek tek fethederek EXP kazan, seviyeni yükselt, petini evrimleştir.

Savaş ekranında sen yoksun — sadece düşman var. Yaratıklar tam ekran, doğrudan sana bakıyor, seni tehdit ediyor. Sen ve petin arka planda savaşıyorsunuz ama ekranda yalnızca düşman görünüyor: bu daha yoğun, daha tehdit edici, daha sinematik bir his. Tur bazlı savaşta yetenek cooldownlarını yönet ya da oto-savaş butonuna bas — savaş biter, EXP ekranı açılır. Tüm bunlar tek elinle, dikey ekranda, her yerde oynanır.

---

## Unique Hook

Summoners War gibi pet toplama ve evrim sistemi, **AMA AYRICA** sıfır sprite animasyonu: her yaratık/pet AI ile üretilmiş kart görseli olarak sunuluyor, saldırı animasyonları AI video — sanat bütçesi yok, içerik üretim hızı maksimum. Üstüne 2 ırk hikaye çatışması her etkinliğin lore çapasını veriyor. Hem kart koleksiyonu heyecanı hem tur bazlı RPG derinliği, hem de idle konfor — tek oyunda üç tatmin.

---

## Player Experience Analysis (MDA Framework)

### Target Aesthetics (What the player FEELS)

| Aesthetic | Priority | How We Deliver It |
| ---- | ---- | ---- |
| **Sensation** (sensory pleasure) | 5 | Tam ekran düşman (bize bakıyor), AI video saldırı animasyonları, EXP kazanım efektleri, evrim parlaması |
| **Fantasy** (make-believe, role-playing) | 1 | Sınıf + pet kombinasyonu fantezisi — seçtiğin ırk, geliştirdiğin sınıf |
| **Narrative** (drama, story arc) | 3 | 2 ırk hikaye çatışması, etkinliklerde lore derinleşmesi |
| **Challenge** (obstacle course, mastery) | 3 | Boss savaşları, cooldown yönetimi, tur bazlı taktik derinlik |
| **Fellowship** (social connection) | 6 | Arena sıralaması karşılaştırma, ırk etkinlik rekabeti |
| **Discovery** (exploration, secrets) | 2 | Yeni zindan bölgeleri, SS evrimi ulaşabilen pet keşfi, gizli katlar |
| **Expression** (self-expression, creativity) | 4 | Sınıf dalı seçimi, kostüm kombinasyonları, pet + oyuncu görsel uyumu |
| **Submission** (relaxation, comfort zone) | 2 | Oto-savaş modu, idle otofarm, stressiz loot toplama |

### Key Dynamics (Emergent player behaviors)

- Oyuncular farklı sınıf dalı + pet kombinasyonları deneyerek sinerji keşfedecek
- "Bir kat daha" psikolojisi ile zindan oturumları planlanandan uzun sürecek
- Oyuncular geri döndüğünde otofarm birikimlerini heyecanla kontrol edecek
- Petleri F'den SS'e evrimleştirme yolculuğu uzun vadeli bağlılık yaratacak
- Hangi petlerin SS'e ulaşabildiğini keşfetmek koleksiyon cazibesini artıracak
- Kostüm ve silah sistemi oyuncunun kendi "görsel kimliğini" oluşturmasına imkân verecek

### Core Mechanics (Systems we build)

1. **Tur Bazlı Savaş Sistemi** — Cooldown tabanlı yetenekler; oto-savaş buton toggle; savaş sonu ödül ekranı (EXP + düşen eşyalar: elbise, silah, takı, pet kartı); savaş görünümü: düşman tam ekran kameraya bakıyor — oyuncu ve pet görünmüyor
2. **Pet Evrim Sistemi** — Zindanlardan pet kazanma; F→D→C→B→A→S→SS evrimi (EXP + item gerekli, görünüş değişiyor, her pet SS'e ulaşamaz)
3. **Oyuncu Sınıf Sistemi** — 4 ana sınıf × 3 dal; sınıf yükseltme kartı (seviye sıfırlanmaz)
4. **Zindan Keşif Sistemi** — Katlı zindanlar, düşman dalgaları, boss savaşları, kat bazlı EXP/milestone ödülleri
5. **Kostüm / Elbise Sistemi** — Oyuncu + pet kostüm/silah/zırh; F-D-C-B-A-S-SS dereceli ekipman
6. **IAP + Reklam Mağazası** — IAP: gerçek para ile kostüm/özel pet; Rewarded Ad: reklam izle → enerji/bonus; Interstitial: kat geçişlerinde
7. **Otofarm / Idle Sistemi** — Aktif savaşmadan arka planda EXP + ilerleme; geri döndüğünde ShowFullscreenReveal

---

## Player Motivation Profile

### Primary Psychological Needs Served

| Need | How This Game Satisfies It | Strength |
| ---- | ---- | ---- |
| **Autonomy** (freedom, meaningful choice) | Sınıf + dal seçimi, oto-savaş/manuel toggle, zindan seçimi, kostüm özelleştirme | Core |
| **Competence** (mastery, skill growth) | Güç büyümesi sayılarla görünür, boss'ları yenme, pet evrim başarısı, arena sıralaması | Core |
| **Relatedness** (connection, belonging) | Irk etkinlik rekabeti, arena sıralaması karşılaştırma, koleksiyon paylaşımı | Supporting |

### Player Type Appeal (Bartle Taxonomy)

- [x] **Achievers** (goal completion, collection, progression) — Pet koleksiyonu tamamlama, SS evrimi ulaşma, kostüm seti tamamlama
- [x] **Explorers** (discovery, understanding systems, finding secrets) — Yeni zindan bölgeleri keşfetme, hangi petlerin SS'e ulaşabildiğini bulma, sınıf dal sinerjileri
- [ ] **Socializers** (relationships, cooperation, community) — Sınırlı: asenkron PvP ve ırk etkinlikleri
- [x] **Killers/Competitors** (domination, PvP, leaderboards) — Arena sıralaması, ırk etkinlik ligi, sıralama tablosu

### Flow State Design

- **Onboarding curve**: İlk 10 dakikada ırk ve sınıf seçilir, başlangıç peti verilir, ilk zindan katı çok kolay, loot cömert düşer. Oyuncu güçlü hisseder hemen. Oto-savaş butonu 2. katta tanıtılır, yetenek sistemi 3. katta açılır.
- **Difficulty scaling**: Zindan katları kademeli olarak zorlaşır ama oyuncu gücü daha hızlı büyür — oyuncu hep "biraz önde" hisseder. Boss katları zorluk sıçraması yapar ama cömert ödülle telafi eder.
- **Feedback clarity**: Hasar sayıları ekranda uçuşur, güçlenme animasyonları belirgin, loot sandığı açılışı tatmin edici. "GÜÇLÜ!" hissi her katta pekiştirilir.
- **Recovery from failure**: Zindan başarısızlığı eşya kaybettirmez — sadece o kattan loot alamazsın. Takımı güçlendirip tekrar dene. Ceza yok, sadece ödül yok.

---

## Core Loop

### Moment-to-Moment (30 seconds)

**Savaş görünümü**: Düşman tam ekran — doğrudan kameraya bakıyor, saldırı yaparken AI video animasyonu oynatılıyor. Oyuncu ve pet ekranda yok; savaş yeteneği butonları ekranın alt kısmında yer alıyor.

**Manuel Modda**: Düşman tam ekranda seni bekliyor → cooldownu dolan yeteneği seç → tetikle (hasar efekti düşmana çarpar) → düşmanın saldırı animasyonu oynar → düşman devrilir → **ödül ekranı**: kazanılan EXP + o savaşta düşen eşyalar (elbise, silah, takı, pet kartı vb.) listeleniyor.

**Oto-Savaş Modunda**: Buton aktif → cooldownlar dolunca otomatik saldırı seçilir → düşman animasyonları oynar → kat biter → aynı ödül ekranı görünür.

Her iki modda da: ödül ekranı savaş sırasında değil, savaş bittikten sonra açılır — savaş esnasında ekrana eşya saçılmıyor, görünüm temiz kalıyor.

### Short-Term (5-15 minutes)

Bir zindan seansı: 5-10 kattan oluşan bir zindan bloğu.
- Her katta 2-3 düşman grubu (tur bazlı savaş) → her savaş sonu ödül ekranı: EXP + düşen eşyalar
- Kat tamamlama bonusu: normal savaşlardan daha cömert ödül ekranı (daha nadir eşyalar / pet kartı şansı artar)
- Her 5. katta mini boss → daha yüksek EXP + garantili nadir ödül (A/S tier item veya pet kartı)
- Kat aralarında pet değiştirme ve level/evrim durumu gözden geçirme fırsatı

**"Bir kat daha" psikolojisi**: Sıradaki milestone ödülünü gösterme — "3 kat daha kazanırsam peti C tier'a çıkarma materyali geliyor" dürtüsü.

### Session-Level (30-120 minutes)

```
Oyuna giriş
├─ Otofarm birikimlerini topla — EXP birikimleri (ShowFullscreenReveal)
├─ Günlük görevleri kontrol et
├─ Zindan keşfi (ana aktivite — manuel veya oto-savaş)
│   └─ Savaş sonu ödül ekranı: EXP + eşyalar (elbise/silah/takı/pet kartı)
├─ Pet yönetimi (EXP kontrolü, evrim yapma, aktif pet seçimi)
├─ Mağaza — yeni kostüm veya ekipman satın alma (IAP)
├─ Arena savaşları (sınıf + pet kombinasyonunu test et)
└─ Günlük görevlerden ödüller topla
```

**Doğal durma noktası**: Zindan enerjisi veya arena hakları tükendiğinde. Otofarm'ı aç ve çık.

### Long-Term Progression

| Zaman Dilimi | İlerleme Ekseni |
|---|---|
| Günlük | Günlük görevler + otofarm birikimleri — her giriş ödüllendirici |
| Haftalık | Yeni zindan bölgeleri açma, arena ligi yükselme |
| Aylık | Sezon etkinlikleri, özel canavarlar, turnuvalar |
| Sürekli | Canavar koleksiyonu genişleme + güç büyümesi — "bitmeyecek bir şey" |

### Retention Hooks

- **Curiosity**: Kilitli zindan bölgeleri, SS'e ulaşabilen gizli petler, "bu pet SS'de nasıl görünür?", ödül ekranında ne düşecek?
- **Investment**: F'den evrimleştirilen SS pet, sınıf dalı ilerlemesi, koleksiyon + mağazadan alınan kostüm kombinasyonları
- **Social**: Arena ligi rekabeti, ırk etkinlik sıralaması, koleksiyon karşılaştırma
- **Mastery**: Tur bazlı cooldown yönetimi, element avantajı, sınıf + pet sinerji kurma

---

## Game Pillars

### Pillar 1: Cömert Zindan (Generous Loot)
Her zindan girişi ödüllendirici hissetmeli — oyuncu asla eli boş dönmemeli.

*Design test*: "Bu değişiklik oyuncunun eline geçen loot miktarını azaltır mı? Evet ise → yapma."

### Pillar 2: Evrimleştir Hepsini (Evolve 'Em All)
Geniş pet koleksiyonu ve F→SS evrim yolculuğu oyunun kalbi — her pet benzersiz, her evrim adımı görsel ve duygusal bir ödül.

*Design test*: "Bu pet mevcut bir petin kopyası mı? Evet ise → benzersiz bir evrim görünümü veya SS yeteneği ekle. Bu evrim görsel olarak tatmin edici mi? Hayır ise → AI art'ı yenile."

### Pillar 3: Senin Tempon (Play Your Way)
Aktif oynayan da, meşgul olan da ilerlemeli — oyuncu kendi ritmini seçebilmeli.

*Design test*: "Bu özellik sadece aktif oyuncuyu mı ödüllendiriyor? Evet ise → pasif alternatif ekle (daha az verimli olsa bile)."

### Pillar 4: Güç Hisset (Power Fantasy)
Oyuncu sürekli güçlendiğini hissetmeli — sayılar büyümeli, düşmanlar daha kolay düşmeli, ordun daha heybetli görünmeli.

*Design test*: "Bu değişiklik oyuncuyu daha güçsüz hissettirir mi? Evet ise → telafi mekanizması ekle."

### Pillar 5: Hep Bir Şey Var (Always Something To Do)
Günlük görevler, etkinlikler, yeni zindanlar — oyuncu her girişte yapacak bir şey bulsun.

*Design test*: "Oyuncu 5 dakika içinde yapacak şey bulamaz mı? Evet ise → içerik döngüsü ekle."

### Anti-Pillars (What This Game Is NOT)

- **NOT Cezalandırıcı grind**: "Cömert Zindan" sütununu zedeler — 100 kez aynı zindanı dönmek eğlenceli değil
- **NOT Pay-to-win**: IAP yalnızca kozmetik (kostüm, görsel) ve kolaylık (özel pet/enerji) satar — savaş gücü IAP'a bağlı değil, EXP ve evrime bağlı. Reklam izleme isteğe bağlı, ceza yok.
- **NOT Karmaşık strateji bulmacası**: Oyun rahat ve akıcı olmalı — oto-savaş var, tur bazlı savaş 10 dakika düşündürmeyecek
- **NOT Kayıp/ceza sistemi**: Zindan kaybetmek eşya kaybettirmemeli — sadece ödül kazanamamak yeterli ceza
- **NOT Çoklu takım kurma**: Tek bir aktif pet; derinlik "hangi peti" sorusundan değil, "nasıl evrimleştiririm" ve "hangi sınıf dalı" sorularından gelir

---

## Farklılaştırıcı Özellikler

> Bu özellikler rakip oyunlarda **bulunmuyor**. Oyunun kimliğini ve tutma gücünü doğrudan etkiliyor.
> Her biri için tasarım notu ve sistem referansı verilmiştir.

### 1. Düşman Kameraya Bakıyor (Zaten Mevcut — Temel Farklılaştırıcı)
Savaş ekranında düşman tam ekran, doğrudan oyuncuya bakıyor. Oyuncu ve pet görünmüyor. Rakip oyunların hiçbirinde bu perspektif yok. AI video animasyonu kameraya doğru saldırı yönünde üretiliyor — "ekrandan çıkıyor" hissi.

*Sistem*: Savaş Sistemi | *Öncelik*: Prototype

---

### 2. Yaşayan Kart Portreleri
Koleksiyon ekranındaki pet kartları statik değil — çok hafif 2-3 frame döngü animasyonu: gözler kırpıyor, aura titreşiyor, kuyruk sallıyor. AI video araçlarıyla üretmek ucuz. Rakip oyunlarda tüm kartlar tamamen statik.

*Neden fark yaratır*: Koleksiyon "canlı" hissettiriyor — oyuncular ekran görüntüsü alıp paylaşıyor.
*Sistem*: Yaşayan Kart Portreleri | *Öncelik*: MVP

---

### 3. İntikam Sistemi
Oyuncuyu öldüren düşman "İntikam Listesi"ne düşüyor. Güçlenip o düşmanı yenince özel **"İNTİKAM ALINDI"** animasyonu ve bonus ödül. Yenilgi artık ceza değil, motivasyon.

*Neden fark yaratır*: Rakip oyunlarda yenilgi = sıkıcı tekrar. Burada yenilgi = geri dönme sebebi.
*Sistem*: İntikam Sistemi | *Öncelik*: MVP

---

### 4. Aranıyor Tahtası
Her gün rastgele 1 yaratık "Aranıyor" ilanıyla çıkıyor — 24 saat içinde zindanda bulup yenersen özel ödül. Ertesi gün yeni yaratık.

*Neden fark yaratır*: Günlük girişi zorunlu kılmadan merak yaratıyor. "Bugün kim aranıyor?" bildirimi açtırıyor.
*Sistem*: Aranıyor Tahtası | *Öncelik*: MVP

---

### 5. Pet Sadakat Sistemi
Aynı pet ile ne kadar çok savaşılırsa o kadar "sadakat" puanı birikiyor. Belirli eşiklerde: kart üzerinde özel rozet, saldırı animasyonunda küçük kişisel dokunuş, özel tepki.

*Neden fark yaratır*: "Bu benim petim" duygusunu yaratıyor — oyuncu o peti değiştirmek istemiyor. Retention'ın ta kendisi.
*Sistem*: Pet Sadakat Sistemi | *Öncelik*: Tier 2

---

### 6. Canavar Kütüphanesi (Codex)
Yendiğin her yaratının sayfası açılıyor: kısa lore metni, zayıf noktaları, hangi zindanda bulunduğu, kaç kez yenildiği. Tamamlamacı oyuncular için sürekli bir hedef.

*Neden fark yaratır*: AFK Arena ve Summoners War'da bu yok. Sıfıra yakın geliştirme maliyeti ama saatler harcatıyor.
*Sistem*: Canavar Kütüphanesi | *Öncelik*: Tier 2

---

### 7. Irk Savaşı Canlı Sıralaması
İki ırk arasındaki haftalık savaş gerçek zamanlı skor tablosunda görünüyor. Önde olan ırka o hafta pasif buff. Her oyuncunun katkısı sıralamaya yansıyor.

*Neden fark yaratır*: Arkadaşın olmasa bile bir tarafa ait hissettiriyor. Topluluk rekabeti her hafta yeniden başlıyor.
*Sistem*: Irk Etkinlik Sistemi (genişletilmiş) | *Öncelik*: Tier 2

---

## Inspiration and References

| Reference | What We Take From It | What We Do Differently | Why It Matters |
| ---- | ---- | ---- | ---- |
| Monster Warlord | Geniş canavar koleksiyonu, toplama bağımlılığı | Pet evrim F→SS görsel; AI kart formatı, sprite animasyonu yok | Koleksiyon modelinin mobilde işlediğini kanıtlıyor |
| AFK Arena | Idle savaş sistemi, güçlenme döngüsü | Tur bazlı savaş + manuel/oto toggle; tek pet, çok sınıf | Idle + RPG hibridinin uzun vadeli tutma gücünü kanıtlıyor |
| Summoners War | Element sistemi, canavar evrimi, tier sistemi | IAP kozmetik önce; EXP bazlı ilerleme, item düşmüyor | Canavar toplama + arena modelinin 10 yıl dayanabildiğini kanıtlıyor |
| Brown Dust 2 | Tur bazlı RPG, karakter kostüm sistemi | AI üretimli görseller; çok daha basit savaş mekanikleri | Kostüm + RPG kombinasyonunun retensiyon değerini kanıtlıyor |
| Puzzle & Dragons | Pet + seviye evrim zinciri, dungeon crawler | Video animasyon saldırılar; dokunmatik optimizasyon önde | Pet evrim döngüsünün uzun vadeli bağımlılık yarattığını kanıtlıyor |

**Non-game inspirations**: Pokémon animesi (evrim anının duygusallığı), gacha kültürü (nadir kart çekiş tatmini), koleksiyon hobisi psikolojisi (tamamlanmamış seti tamamlama dürtüsü), Dragon Ball (evrim → görsel dönüşüm → güç artışı döngüsü).

---

## Lokalizasyon Stratejisi

### Hedef Pazarlar ve Diller

| Öncelik | Dil | Kapsadığı Pazar | Neden Öncelikli | ARPU |
|---------|-----|----------------|-----------------|------|
| ✅ Mevcut | Türkçe | Türkiye | Geliştirici pazarı | Orta |
| 1 | **İngilizce** | ABD, UK, Avustralya, Kanada, Hindistan | Zorunlu — tüm uluslararası erişim | Yüksek |
| 2 | **Portekizce (BR)** | Brezilya | 215M kişi, mobil RPG sever | Orta |
| 3 | **İspanyolca (LATAM)** | Meksika, Kolombiya, Arjantin... | 400M+ kişi tek pakette | Orta |
| 4 | **Endonezce** | Endonezya | 270M kişi, koleksiyon RPG'nin kalbi | Orta-Düşük |
| 5 | **Filipince (Tagalog)** | Filipinler | Güçlü mobil oyun kültürü | Orta-Düşük |
| 6 | **Tayca** | Tayland | Koleksiyon RPG çok popüler | Orta |
| 7 | **Vietnamca** | Vietnam | Hızlı büyüme, genç demografik | Düşük |
| 8 | **Rusça** | Rusya, Ukrayna, Kazakistan | Büyük pazar, RPG sever | Orta |
| 9 | **Arapça** | S.Arabistan, BAE, Mısır | Çok yüksek ARPU — RTL dikkat ⚠️ | Çok Yüksek |
| 10 | **Almanca** | Almanya, Avusturya, İsviçre | Avrupa'nın en büyük pazarı | Yüksek |
| 11 | **Fransızca** | Fransa + Batı Afrika | Hem Avrupa hem Afrika | Orta-Yüksek |
| 12 | **Geleneksel Çince** | Tayvan, Hong Kong | Koleksiyon oyunu seviyor | Yüksek |

### Aşamalı Lokalizasyon Planı

**Prototype (Google Play ilk çıkış):**
Türkçe + İngilizce

**MVP sonrası (ilk gelir gelince):**
+ Portekizce (BR) + İspanyolca (LATAM) + Endonezce

**Tier 2 (pazar verisi sonrası):**
+ Filipince + Tayca + Vietnamca + Rusça

**Tier 3 (yatırım gerektirir):**
+ Arapça (RTL — UI framework yeniden düzenleme gerekebilir)
+ Almanca + Fransızca + Geleneksel Çince

### Kaçınılacak Pazarlar (Şimdilik)

- 🇯🇵 **Japonya** — En yüksek ARPU ama kalite standardı çok yüksek; AI art bu pazarda işe yaramaz
- 🇨🇳 **Çin** — Ayrı app store, lisans gerektiriyor, çok zorlu süreç
- 🇮🇳 **Hindistan (Hindi)** — 1.4 milyar kişi ama IAP neredeyse yok; İngilizce zaten büyük kısmını kapsıyor

### Teknik Notlar

- **Arapça**: Sağdan sola (RTL) metin yönü — UI Framework'te tüm layout'lar ters çevrilmeli; ayrı geliştirme efor gerektirir
- **Genel**: Unity'de `Localization` paketi (Unity Localization) kullanılmalı — string table + font atlas per dil
- **Font**: Her dil için ayrı font gerekebilir (Tayca, Vietnamca, Arapça özel karakter setleri)
- **Kültürel uyum**: Bazı yaratık tasarımları belirli pazarlarda hassas olabilir (Orta Doğu için şeytan/iblis figürleri dikkat)

---

## Target Player Profile

| Attribute | Detail |
| ---- | ---- |
| **Age range** | 16-35 |
| **Gaming experience** | Casual → Mid-core |
| **Time availability** | 5-30 dakika aktif oyun, gün boyu idle — yolda, molada, yatmadan önce |
| **Platform preference** | Mobil (iOS/Android) |
| **Current games they play** | AFK Arena, Monster Warlord, Summoners War, Brown Dust 2, Raid: Shadow Legends |
| **What they're looking for** | Güçlü hissetme, cömert ödüller, pet evrim tatmini, görsel kostüm özelleştirme |
| **What would turn them away** | Aşırı grind, pay-to-win, karmaşık kontroller, ceza mekanikleri, tek tip görsel |

---

## Visual Identity Anchor

> *AD-CONCEPT-VISUAL atlandı (Lean mod) — detaylı görsel yön `/art-bible` ile belirlenecek.*

**Ön yönlendirme** (brainstorm'dan çıkarılan):
- 2D sanat stili, AI üretimi görseller — kart formatı (koleksiyon) + tam ekran savaş görseli
- **Savaş perspektifi**: Düşman/yaratık kameraya bakıyor, tam dikey ekranı dolduruyor — oyuncu ve pet görünmüyor. "Bana saldırıyor" hissi.
- Canavar tasarımları çeşitli ve renkli — her element farklı renk paleti; kameraya bakış açısı AI prompt'ta zorunlu
- Mobil dostu temiz UI; yetenek butonları ekranın alt kısmında
- Zindan ortamları atmosferik ama karanlık değil — "güçlü kahraman" hissi, "hayatta kalma korkusu" değil

---

## Technical Considerations

| Consideration | Assessment |
| ---- | ---- |
| **Recommended Engine** | Unity 6.3 LTS — mobil için en güçlü indie motor, C#, URP, geniş ekosistem |
| **Key Technical Challenges** | İlk Unity projesi (öğrenme eğrisi), AI görsel tutarlılığı, AI video saldırı entegrasyonu, AdMob/Unity Ads entegrasyonu |
| **Screen Orientation** | Portrait only — `Screen.orientation = ScreenOrientation.Portrait`; safe area (notch + home indicator) UI Framework'te halledildi |
| **Art Style** | 2D kart formatı — AI üretimi görseller (Midjourney/SDXL); saldırı animasyonları AI video (Runway/Kling); portrait layout için dikey kart tasarımı |
| **Art Pipeline Complexity** | Düşük (AI üretimi) — sprite animasyonu YOK; video oynatım Unity VideoPlayer ile |
| **Audio Needs** | Orta — savaş efektleri, EXP kazanım sesi, arka plan müziği; video ses entegrasyonu |
| **Networking** | İlk aşamada yok; arena ve ırk etkinlikleri için basit client-server (sonraki katmanlarda) |
| **Monetization SDK** | Google AdMob (rewarded + interstitial) + Google Play Billing Library (IAP); her ikisi de prototype'a entegre edilmeli |
| **Content Volume** | Prototype: 10-15 pet (F/C/S tier), 4 sınıf (1 dal), 1 zindan, 5 kat. Tam vizyon: 50+ pet (SS dahil), 10+ bölge |
| **Procedural Systems** | Zindan kat düzenleri prosedürel olabilir (prototype'ta sabit, sonra prosedürel) |
| **AI Art Notes** | Her pet için 7 görsel (F/D/C/B/A/S/SS tier) — **kart formatı** (koleksiyon ekranı için). Savaş ekranı için **düşman görseli kameraya bakıyor** — AI ile üretirken "front-facing, looking at camera, threatening" prompt şartı. Video animasyonu: saldırı başına 1 klip (2-4 sn), kameraya doğru saldırıyor gibi (düşman ekrandan "çıkıyor" hissi). Prototype'ta 3 tier (F/C/S). |

---

## Risks and Open Questions

### Design Risks
- Manuel/oto-savaş dengesi: oto-savaş çok güçlüyse kimse manuel oynamaz; cooldown tasarımı tatmin edici olmalı
- EXP-only ödül modeli: "ne kazandım?" hissi yeterince tatmin edici olmalı — milestone ödülleri bu boşluğu kapatmalı
- IAP dengeleme: kozmetik-only IAP tutarsa iyi, ama oyuncu "güçlü olmak için para gerekiyor" hissine kapılmamalı
- 7 tier içerik üretimi: F-D-C-B-A-S-SS her pet için 7 görsel çok büyük — prototype'ta 3 tier başla

### Technical Risks
- İlk Unity projesi — öğrenme eğrisi prototype süresini 2-3x uzatabilir
- AI video entegrasyonu (VideoPlayer): çözünürlük, format ve mobil performans sorunları
- AI üretimi görseller arasında tier tutarlılığı (aynı pet farklı tierlerde tanınabilir olmalı)
- Idle sistemi (arka plan hesaplama) mobilde pil/performans sorunları yaratabilir
- Google Play IAP (Billing Library) + AdMob entegrasyonu — iki ayrı SDK, test süreci gerekir
- Rewarded ad frekansı yanlış ayarlanırsa oyuncu deneyimi bozulur — interstitial'lar kat geçişinde, zorla değil

### Market Risks
- Mobil RPG pazarı rekabetçi — fark: portrait/tek el + hızlı prototype + market test
- IAP dönüşüm oranı düşük olabilir — reklam geliri bunu dengeleyebilir (hibrit model avantajı)
- Organik keşfedilebilirlik düşük olabilir — pet evrim videoları sosyal içerik potansiyeli

### Scope Risks
- Tam vizyon (200+ canavar, PvP, etkinlikler) ilk oyun için çok büyük — MVP disiplini kritik
- "Bir özellik daha" dürtüsü en büyük düşman — anti-sütunlara sadık kal

### Open Questions
- Oto-savaş ile manuel savaş ne kadar farklı sonuç vermeli? → MVP prototipinde test et
- Kaç element yeterli? → 4 elementle başla, oyuncu feedback'ine göre genişlet
- Arena asenkron mu yoksa gerçek zamanlı mı? → MVP'de asenkron, feedback'e göre karar
- AI video saldırı formatı: MP4 mi (daha uyumlu) yoksa WebM/alpha-channel mı? → Unity VideoPlayer test gerekir
- Sınıf dalı seçimi kalıcı mı yoksa maliyetli geri dönüşe izin verecek mi? → GDD'de netleştirilecek
- IAP içeriği: sadece kozmetik mi, yoksa "enerji satın alma" gibi konfor IAP'ları da olacak mı? → prototype sonrası market verisine göre karar

---

## MVP Definition

**Core hypothesis**: "Tur bazlı savaşta sınıfını ve petini yönetmek, EXP kazanmak, zindan geçmek ve pet evrimleştirmek eğlenceli mi? Oyuncular IAP satın alır mı?"

**Required for MVP (Google Play soft launch öncesi)**:
1. 2 ırk seçimi (başlangıçta; lore çatışması bağlamıyla)
2. 4 oyuncu sınıfı (dal sistemi Tier 2'ye ertelendi — prototype için tek dal)
3. 10-15 pet (F-C-S tier; her pet 3 AI görsel; evrim EXP + item gerektiriyor)
4. Tur bazlı savaş — cooldown tabanlı yetenekler; oto-savaş toggle; savaş sonu EXP ekranı
5. 1 zindan bölgesi, 10 kat, 1 boss
6. Savaş sonu ödül ekranı: EXP + düşen eşyalar (elbise, silah, takı, pet kartı) — savaş sırasında ekrana saçılmıyor, ödül ekranında listeleniyor
7. Temel IAP mağazası (2-3 kostüm paketi + 1-2 özel pet — Google Play Billing)
8. Temel reklam entegrasyonu (AdMob: rewarded ad → enerji/bonus; interstitial → kat geçişi)
9. Otofarm + ShowFullscreenReveal EXP geri dönüş ödülü

**Explicitly NOT in MVP**:
- 3 sınıf dalı (sadece 1 dal per sınıf — Tier 2)
- Arena / PvP sistemi
- Günlük görevler ve etkinlikler
- Irk etkinlik sistemi
- A-B-D-SS tier pet görseli (F/C/S ile başla)
- Dünya boss'u
- Pet takası
- 2+ zindan bölgesi

### Scope Tiers

| Tier | Content | Features | Timeline |
| ---- | ---- | ---- | ---- |
| **Prototype → Google Play** | 10-15 pet (F/C/S), 4 sınıf (1 dal), 2 ırk, 1 zindan, 10 kat | Tur bazlı savaş + oto, EXP sistemi, IAP (2-3 ürün) + AdMob rewarded/interstitial, portrait | 2-3 hafta |
| **MVP: Tam Döngü** | 20-30 pet (F-D-C-B-A-S), 4 sınıf × 3 dal, tam kostüm sistemi | + Sınıf dalları, tam mağaza, milestone ödül sistemi genişletildi | +4-6 hafta |
| **Tier 2: Rekabet** | 40+ pet (SS tier dahil), 2 zindan bölgesi | + Arena (AI rakip), günlük görevler, ırk etkinlikleri | +4-6 hafta |
| **Tier 3: Derinlik** | 100+ pet, 5+ zindan bölgesi | + Sınıf prestige, sezon etkinlikleri, ırk rekabet ligi | +2-3 ay |

Her katman tek başına **oynanabilir bir ürün** — hiçbir noktada yarım kalmış hissetmez.

---

## Next Steps

- [ ] `/setup-engine` — Unity motorunu yapılandır, versiyon referans dokümanlarını oluştur
- [ ] `/art-bible` — Görsel kimlik spesifikasyonu oluştur (GDD'lerden ÖNCE)
- [ ] `/design-review design/gdd/game-concept.md` — Konsept dokümanını doğrula
- [ ] `creative-director` ile vizyon tartışması — sütun ince ayarı
- [ ] `/map-systems` — Konsepti bağımsız sistemlere ayrıştır, bağımlılıkları haritalandır
- [ ] `/design-system [system]` — Her sistem için ayrı GDD yaz (bağımlılık sırasına göre)
- [ ] `/create-architecture` — Master mimari planını oluştur
- [ ] `/architecture-decision (×N)` — Her mimari karar için ADR yaz
- [ ] `/architecture-review` — TR registry ve İzlenebilirlik Matrisi oluştur
- [ ] `/gate-check` — Üretime geçiş öncesi faz kapısı doğrulaması
