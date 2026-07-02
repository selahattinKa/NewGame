# Canavar Toplama ve Evrim (Monster Collection & Evolution)

> **Status**: Revised — **Re-Review Bekliyor** (2026-07-02, re-review turu 3 — tam `/design-review`, temiz context: game-designer, systems-designer, economy-designer, qa-lead, creative-director verdict: NEEDS REVISION → aynı oturumda revize edildi, bkz. `reviews/canavar-toplama-evrim-review-log.md`). 3. tur, 2. turun (aynı gün) Canavar Güçlendirme birleşmesini doğruladı ve şunları güçlendirdi: `canavar-veritabani.md` Kural 6 çelişkisi "kırıntı"dan **onaylı blocker**'a yükseltildi (Open Questions #6), Kural 5/Formül 3'ün giriş-sayısı tablosu "provisional" olarak işaretlendi, `ekonomi.md`'nin stale altın-evrim modeli ve idle-altın/Formül 4 sink uyumsuzluğu flag edildi (yeni Dependencies notu), Kural 11'e Kural 10 ile simetrik bir race-condition edge case eklendi, mock `total_pokedex_entries` test sabiti DO-NOT-SHIP adlandırma kuralına bağlandı, AC#33 eklendi, AC#16/#18 test edilebilirliği sıkılaştırıldı, S/SS'e özel VFX/Audio "doruk an" beat'i eklendi. **Bu revizyonlar henüz bağımsız re-review'dan geçmedi** — implementasyondan önce temiz context'te `/design-review canavar-toplama-evrim` tekrar çalıştırılmalı. Kalan açık maddeler: Pokédex elmas bütçesi VE giriş-sayısı tablosu artık ikili bağımlılıkla `canavar-veritabani.md`'nin hem nihai roster'ına (Open Questions #2) HEM DE Kural 6 form-gate çözümüne (Open Questions #6, onaylı blocker) bağlı; `loot-odul-sistemi.md`/`kesif-alani.md` arayüz güncellemeleri kendi revizyon oturumlarını bekliyor (Open Questions #7-8); `ekonomi.md`'nin stale altın-evrim modeli ve idle-altın dengesi kendi revizyon oturumunu bekliyor (YENİ, Dependencies → Çapraz bağımlılık notları).
> **Author**: user + game-designer, systems-designer, economy-designer, qa-lead, creative-director
> **Last Updated**: 2026-07-02
> **Implements Pillar**: Topla Hepsini, Güç Hisset

## Overview

**Canavar Toplama ve Evrim**, oyuncunun canavar koleksiyonunun yaşam döngüsünü yöneten sistemdir: canavarların kazanılması, koleksiyona eklenmesi, envanterinin yönetilmesi ve koleksiyon ilerlemesinin takibi. Loot / Ödül Sistemi'nden `OnMonsterDropped` sinyaliyle yeni canavar instance'ları oluşturur (Canavar Veritabanı şablonlarından), bunları oyuncunun genişletilebilir envanterinde depolar ve Canavar Güçlendirme'ye instance verilerini sağlar. Pokédex tarzı bir kayıt defteri tüm canavar türlerini takip eder — sahip olunmayanlar kilitli siluet olarak görünür, tamamlama yüzdesi ve keşif ödülleri koleksiyon genişletme motivasyonu yaratır.

Oyuncu perspektifinden bu sistem, "Topla Hepsini" sütununun kalbidir: zindandan yeni bir canavar kazanmak, koleksiyon defterinde yeni bir giriş açmak, tamamlama yüzdesinin yükselmesini görmek ve nadir bir keşfin heyecanını yaşamak. Kopya canavarlar çöp değil — yıldız birleştirme malzemesi veya altına satış kaynağıdır. Envanter yönetimi stratejik bir katman ekler: sınırlı ama genişletilebilir kapasite, oyuncuyu hangi canavarları tutacağı konusunda bilinçli seçimler yapmaya yönlendirir.

MVP kapsamında 15-20 canavar türü, başlangıç envanter kapasitesi (elmasla genişletilebilir), Pokédex kayıt defteri, canavar satma mekaniği, temel koleksiyon tamamlama ödülleri ve Loot→Instance→Güçlendirme pipeline'ı tanımlanır.

## Player Fantasy

Canavar Toplama ve Evrim'de oyuncu **iki katmanda** fantezi yaşar:

**Doğrudan deneyim — "Koleksiyoncu Lord"**: Oyuncunun en heyecanlı anı, zindanda yeni bir canavar düştüğünde koleksiyon defterinde kilitli bir silueti açma anıdır. "Bu canavarı daha önce görmedim!" keşfi, Pokédex'in 30 yıldır kanıtladığı tamamlama dürtüsünü tetikler — %73 tamamlandı, %74'e çıkmak için bir canavar daha lazım. Her yeni keşif koleksiyonu bir adım büyütür, koleksiyon büyüdükçe tamamlama ödülleri açılır ve "hepsini topla" fantezisi kendini besleyen bir döngüye dönüşür. Nadir bir canavar düşmesi — Epik veya Efsanevi — oyunun doruk anlarından biridir: "Bunu buldum!" heyecanı.

Kopya canavarlar bile değerlidir — ama iki yol eşit değildir. **Asıl değer yolu yıldız birleştirmedir** (Kural 11): kopya, canavarın kalıcı stat tavanını yükseltir, "bu kopya boşa gitmedi, canavarımı güçlendirdi" hissini somut şekilde üretir. Satış (Kural 7) kasıtlı olarak düşük tutulur (bkz. Formül 2 denge notu) — bu bir yatırım getirisi değil, envanter temizliği kolaylığıdır. Koleksiyon motivasyonu kopya düşmelerinde canlı kalıyorsa bunun sebebi fusion'dır, altın değil.

**Dürüstlük notu (design-review 2026-07-02, game-designer bulgusu)**: Tier-Zinciri Pokédex modelinde (Kural 5) completion% artışının büyük kısmı, oyun ilerledikçe zaten sahip olunan canavarları bir üst kademeye evrimleştirmekten (Mastery — "bu canavarı geliştirdim") gelir, yeni tür bulmaktan (Discovery — "bunu ilk kez görüyorum") değil. Bu bilinçli bir tasarım: Pokédex çubuğu hem keşfi hem ustalığı ölçer, ikisi ayrı UI izleyicileri olarak sunulmaz (bkz. UI Requirements — Pokédex görünümü). Nadirlik ile tek-tür-toplam-ödül ters orantılıdır (SS düşüşü 1 giriş/15 elmas, F düşüşü 3 kademe/45 elmas) — bu da bilinçli bir seçim: ödül grind uzunluğuyla orantılıdır, düşüş nadirliğiyle değil. Nadir bir düşüşün "doruk an" hissi güç/nadirlik ekseninden gelir (bkz. yukarıdaki paragraf), Pokédex-elmas ekseninden değil.

**Dolaylı altyapı — "Stratejik Yönetici"**: Envanter yönetimi oyuncuyu bilinçli seçimler yapmaya yönlendirir: sınırlı kapasite, hangi canavarları tutup hangilerini satacağına karar vermeyi gerektirir. Bu seçimler stratejik derinlik katar — tam envanterde yeni bir Nadir canavar düşmesi "kimi göndereyim?" kararını tetikler. Genişletilebilir kapasite elmas sink'i olarak çalışırken, oyuncuya "daha büyük koleksiyonun gücü" hissini verir.

**Çekirdek duygu**: Keşif heyecanı + tamamlama tatmini. Her zindan girişi potansiyel bir yeni keşif, her keşif koleksiyonun bir adım büyümesi, her tamamlama ödülü somut güçlenme.

**Negatif fantezi (kaçınılacak)**: "Envanter dolu, hiçbir şey yapamıyorum" çıkmazı — loot düştüğünde beklemeye alma mekanizması bunu önler. "50 zindan boyunca hep aynı F tier" monotonluğu — Loot GDD'nin pity ve nadirlik artışı bu sorunu zaten ele alır.

**Pillar bağlantısı**: "Topla Hepsini" — Pokédex tamamlama dürtüsü, keşif ödülleri. "Güç Hisset" — her toplanan canavar potansiyel güce dönüşür (yıldız, takım genişlemesi). "Cömert Zindan" — her zindan çıkışı koleksiyona en az bir katkı bırakır.

*`creative-director` + `game-designer` konsültasyonu: `/design-review` 2026-07-02, tam mod — bkz. Kural 4.3 (S/SS muafiyeti) ve Kural 5 (Pokédex modeli) player-fantasy gerekçeleri.*

## Detailed Rules

### Core Rules

**Kural 1 — Canavar Kazanma Yolları (REVİZE — design-review 2026-07-02)**

**Düzeltme**: Önceki sürüm bu tabloyu `zindan-kesif.md`'ye (Zindan Loot / Boss Drop) bağlıyordu. `systems-index.md`'nin 2026-06-30 pivotuyla **Zindan Keşif Sistemi Tier 2 özel etkinliğe düşürüldü**, MVP'nin ana canavar kazanım kaynağı artık **Keşif Alanı Sistemi** (`kesif-alani.md`, #13). Tablo buna göre güncellendi:

| Kaynak | Tetikleyici | Detay | Kapsam |
|--------|-------------|-------|--------|
| **Keşif Alanı Loot** | Aşama temizleme (normal) | `kesif-alani.md` loot tablosu — Loot Sistemi Kural 3'e göre canavar düşme şansı ×1.3 çarpanıyla uygulanır | MVP |
| **Keşif Alanı İlk Temizleme** | Mini Boss / Alan Patronu aşamasının ilk temizlenmesi | Garantili canavar (C/B tier, alana özel) — bkz. `kesif-alani.md` | MVP |
| **Zindan Keşif Loot/Boss** | Zindan Keşif özel etkinliği (kat temizleme / boss) | Loot GDD Kural 3-6: %15 base oran, pity ile %45'e kadar + boss %1-5 | Tier 2 |

İleride eklenebilecek kaynaklar (Tier 2+): koleksiyon milestone ödülleri, arena ödülleri, etkinlik ödülleri.

**Cross-file not**: `kesif-alani.md`'nin kendi Downstream tablosu şu an "Canavar Toplama"yı ayrı bir satır olarak listelemiyor (yalnızca Loot/Ödül Sistemi üzerinden dolaylı bağlanıyor) ve Pokédex "keşfedildi" (düşman olarak karşılaşma, bkz. Kural 5) sinyalini (`OnEnemyEncountered` eşdeğeri) tanımlamıyor — bu sinyal MVP'de şu an yalnızca Tier 2 `zindan-kesif.md`'de var. Bu, `kesif-alani.md`'nin kendi revizyon oturumunda kapatılması gereken bir arayüz boşluğudur (bkz. Dependencies).

**Kural 2 — Canavar Instance Oluşturma**

**(REVİZE — design-review 2026-07-02, F-D-C-B-A-S-SS model hizalaması)** Bir canavar düştüğünde Loot sistemi `OnMonsterDropped(monsterId, tier)` sinyali gönderir — `tier` parametresi, Loot sisteminin nadirlik rulosunun ürettiği başlangıç kademesidir (F-D-C-B-A-S-SS, bkz. `loot-odul-sistemi.md` Kural 10 pity sistemi). **Cross-file not**: `loot-odul-sistemi.md`'nin mevcut arayüz tanımı (`OnMonsterDropped(monsterId)`, tek parametre) bu `tier` parametresini içermiyor — bu, o dosyanın kendi revizyon oturumunda eklenmesi gereken bir arayüz güncellemesidir.

Bu sistem şu adımları uygular:

1. Canavar Veritabanı'ndan tür şablonunu çek: `GetMonsterDefinition(monsterId)`
2. Benzersiz `instance_id` ata (UUID)
3. Instance verilerini başlat:

| Alan | Başlangıç Değeri | Kaynak |
|------|-------------------|--------|
| `instance_id` | UUID | Bu sistem |
| `template_id` | monsterId | Loot sinyali |
| `tier` | Loot sinyalinden gelen başlangıç kademe (F/D/C/B/A/S/SS) | Loot sinyali |
| `level` | 1 | Sabit — bkz. `level-deneyim-sistemi.md` Kural 5 |
| `xp` | 0 | Sabit |
| `banked_xp` | 0 | Sabit — bkz. `level-deneyim-sistemi.md` Kural 5 |
| `lifetime_pet_level` | 0 | Sabit — bkz. `level-deneyim-sistemi.md` Kural 6 |
| `star_rank` | 0 | Sabit |
| `acquired_date` | timestamp | Bu sistem |
| `is_locked` | false | Sabit |
| `is_favorite` | false | Sabit |

**Düzeltme notu**: Önceki sürümdeki `evolution_stage` (Form A/B/C modeli) alanı kaldırıldı — bkz. Kural 5'in F-D-C-B-A-S-SS tier-zinciri revizyonu. `pity_counter` alanı da kaldırıldı: pity, canavar instance'ına değil Loot / Ödül Sistemi'nin kendi rolling state'ine ait bir kavramdır (bkz. `loot-odul-sistemi.md` Kural 10, `SaveLootState()`) — bu alanın instance şemasında bulunması bir kapsam hatasıydı.

4. Koleksiyon defterinde bu **(tür, tier)** çifti ilk kez keşfedildiyse → `first_discovery` flag'i + keşif ödülü (bkz. Kural 5/6 — her tier basamağı ayrı bir Pokédex girişidir)
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
2. Aktif pet slotunda seçili canavar envanter slotu kullanır (ayrı slot yok)
3. Kilitlenen canavarlar (`is_locked = true`) satılamaz ve otomatik satıştan muaftır
4. Favori canavarlar (`is_favorite = true`) listede üste sabitlenir ve satış uyarısı alır

**Kural 4 — Envanter Dolu Durumu**

Envanter kapasitesi doluyken yeni canavar düşerse:

1. Canavar **bekleme alanına** (pending buffer) eklenir
2. Bekleme alanı kapasitesi: 10 canavar (envanter dışı, geçici) — **istisna**: S/SS tier canavarlar bu 10 slotluk kapasiteye dahil edilmez, ayrı ve sınırsız bir "kalıcı bekleme" alt-alanında tutulur (bkz. madde 3)
3. Bekleme süresi tier'e göre değişir:
   - F / D: **7 gün**
   - C / B / A: **14 gün** + push notification gönderilir ("Nadir canavarınız beklemede — [X] gün kaldı!")
   - **S / SS: SÜRESİZ — asla otomatik satılmaz (REVİZE, design-review 2026-07-02, creative-director kararı)**. Push notification bir defaya mahsus gönderilir ("Efsanevi canavarınız kalıcı beklemede — envanterde yer açın!"), tekrarlanmaz (spam önleme). Bu tier oyuncu manuel olarak yer açana kadar sonsuza dek bekler.
4. Süre dolduğunda bekleme alanındaki ilgili canavar otomatik satılır (altına çevrilir) — **S/SS bu maddenin kapsamı dışındadır (madde 3)**
5. Bekleme alanı (10 slotluk F-A kapasitesi) da doluysa: yeni canavar otomatik satılır ve altın olarak eklenir (loot raporu "envanter dolu — satıldı" notu gösterir) — **S/SS tier canavarlar bu otomatik satıştan HER ZAMAN muaftır**: kalıcı bekleme alt-alanına eklenir, 10 slotluk kapasiteyi etkilemez ve asla zorla satılmaz (bkz. Edge Cases)
6. Oyuncu bilgilendirilir: "Envanter dolu! [X] canavar beklemede — yer aç veya sat."
7. **Kullanım kısıtı (REVİZE — design-review 2026-07-02, game-designer/economy-designer anlaşmazlığının orta-yol çözümü)**: Kalıcı bekleme alt-alanındaki bir S/SS instance'ı, envantere taşınana kadar Kural 10 (Evrim Yürütme), Kural 11 (Yıldız Sistemi) veya aktif takım seçimi için kullanılamaz — bu üç eylemin hepsi "envanterde" durumunu gerektirir. Bu, süresiz biriktirmeye küçük bir fırsat maliyeti ekler (game-designer'ın "kimi göndereyim? gerilimi kayboluyor" bulgusuna kısmi yanıt) ve satış hâlâ mümkün olduğundan economy-designer'ın "sınırsız birikim → baskın geç-oyun altın musluğu" riskini tamamen ortadan kaldırmaz — bu risk bilerek açık bırakılıyor, bkz. Tuning Knobs → S/SS Satış Riski notu.

**Denge notu (design-review 2026-07-02)**: S/SS muafiyeti "Cömert Zindan" pillar'ıyla uyum için eklendi — nadir bir kazanımın sessizce kaybolması negatif fantezi yaratır (bkz. Player Fantasy). Kalıcı bekleme alt-alanı sınırsız olduğundan, teorik olarak çok sayıda S/SS canavar birikebilir; bu, envanter genişletme elmas sink'i için ek bir motivasyon kaynağıdır (istenmeyen bir sonuç değil). **Anlaşmazlık notu**: game-designer bu muafiyeti bilinçli bir "kimi göndereyim?" stratejik-gerilim sulandırması olarak görüyor (madde 7'nin kullanım kısıtı bunu kısmen telafi eder); economy-designer bunu bir ekonomi riski olarak görüyor (S/SS hiç otomatik satılmadığından süresiz birikip manuel satışla baskın bir geç-oyun altın kaynağına dönüşebilir — bkz. Formül 2 denge notu). Her iki görüş de geçerli, kullanıcı madde 7'nin orta-yol kısıtını onayladı ama tam çözüm değil — Tuning Knobs'ta izleme notu olarak bırakılıyor.

**Kural 5 — Koleksiyon Defteri (Pokédex)**

Tüm canavar türleri ve evrim formları koleksiyon defterinde listelenir:

| Durum | Görünüm | Bilgi |
|-------|---------|-------|
| **Keşfedilmedi** | Kilitli siluet, "???" isim | Sadece arketip/nadirlik ipucu |
| **Keşfedildi** (sahip değil) | Gri ikon, isim gösterilir | Temel bilgi: arketip, nadirlik |
| **Sahip (PERMANENT)** | Tam renkli ikon | Tüm bilgi: stat, yetenek, lore. Bu durum kalıcıdır — satış sırasında geri alınmaz |

Keşif koşulları:
- Bir canavarı **kazanmak** (loot'tan almak, herhangi bir tier'de) → o (tür, tier) çiftinin otomatik keşfi + "Sahip" (kalıcı)
- Bir canavara karşı **savaşmak** (düşman olarak görmek) → o (tür, tier) çiftinin otomatik keşfi (siluet → gri)

**Tier Zinciri Girişleri (REVİZE — design-review 2026-07-02, F-D-C-B-A-S-SS model hizalaması)**:

Eski "Form A/B/C" modeli (tür başına sabit 3 giriş) kaldırıldı — `canavar-veritabani.md` Kural 4'ün nadirlik/tier zincirine uyumlu hale getirildi. Her canavar türü, kendi başlangıç kademesinden (F/D/C/B/A/S) terminal SS'e kadar **her tier basamağı için ayrı bir Pokédex girişi** sayılır:

| Başlangıç Kademesi | Zincir | Giriş Sayısı |
|---------------------|--------|---------------|
| F | F→D→C | 3 |
| D | D→C→B | 3 |
| C | C→B→A | 3 |
| B | B→A→S→SS | 4 |
| A | A→S→SS | 3 |
| S | S→SS | 2 |
| SS | SS (terminal) | 1 |

Bir tür için toplam giriş sayısı, o türün Canavar Veritabanı'ndaki başlangıç kademesine göre yukarıdaki tablodan belirlenir (bkz. Formül 3).

⚠️ **PROVISIONAL (design-review 2026-07-02, 3. tur re-review)**: Yukarıdaki giriş sayısı tablosu, `canavar-veritabani.md` Kural 6'nın tier-içi Form 1-3 gate'inin kaldırılacağı/yeniden çerçevelenceği varsayımına dayanır (bkz. Open Questions #6, artık onaylı blocker). Eğer o gate canlı kalırsa bu tablo tier başına 2-3× eksik sayım yapıyor olabilir. `canavar-veritabani.md` revizyonu tamamlanana kadar bu tablo ve ona bağlı Formül 3 çıktıları nihai kabul edilmemeli.

**Keşif/Sahiplik mantığı**: Bir instance bir tier'a evrimleştiğinde (level-deneyim-sistemi.md Kural 6'daki tier-up), o instance'ın YENİ tier'ı için (tür, yeni-tier) Pokédex girişi de otomatik "Sahip Olundu" durumuna geçer — evrim, o basamak için ayrı bir keşif+sahiplik ödülü tetikler (bkz. Kural 6). Bir türün "keşfedilmiş" sayılması için EN AZ BİR (tür, tier) girişinin keşfedilmiş olması yeterlidir; "tamamen sahip olunmuş" sayılması için türün ulaşabileceği TÜM tier girişlerinin sahiplenilmiş olması gerekir (UI'da tür kartı üstünde "3/4 tier" gibi bir alt-ilerleme göstergesiyle sunulabilir — bkz. UI Requirements).

**Kural 6 — Koleksiyon Ödülleri**

İki katmanlı ödül sistemi:

**a) Keşif Ödülleri** (her yeni (tür, tier) girişi keşfedildiğinde — REVİZE, design-review 2026-07-02)

| Olay | Ödül |
|------|------|
| Yeni (tür, tier) girişi keşfedildi (ilk karşılaşma o kademede) | 5 elmas |
| Yeni (tür, tier) girişi sahip olundu (ilk kazanım veya ilk evrimle o kademeye ulaşım) | 10 elmas + 500 altın |

**Not**: Tier Zinciri modeli altında (Kural 5) bu ödüller artık tür başına birden fazla kez tetiklenebilir — bir tür F'den SS'e evrimleştikçe her basamak (F, D, C, ...) kendi keşif+sahiplik ödülünü verir. Bu, toplam Pokédex elmas bütçesini eski sabit-3-form varsayımından farklılaştırır — **kesin toplam bütçe, Canavar Veritabanı'nın nihai tür-başına-başlangıç-kademesi dağılımı netleşene kadar hesaplanamaz** (bkz. Tuning Knobs → Diamond Budget notu ve Open Questions #2).

**b) Milestone Ödülleri** (koleksiyon tamamlama yüzdesine göre — REVİZE, design-review 2026-07-02)

Milestone eşikleri artık sabit tür sayısına değil, **Formül 3'ün ürettiği `completion_pct`** değerine bağlıdır (derived `total_pokedex_entries` — bkz. Formül 3). Bu, Formül 3 ile bu tablo arasındaki eski çelişkiyi (60 vs 20 denominatör) ortadan kaldırır — tek bir sayaç, tek bir formül:

| Milestone | Koşul | Ödül |
|-----------|-------|------|
| %25 | `completion_pct >= 25` | 50 elmas + 5.000 altın |
| %50 | `completion_pct >= 50` | 100 elmas + 15.000 altın + 1 XP İksiri (Büyük) |
| %75 | `completion_pct >= 75` | 200 elmas + 30.000 altın + 3 XP İksiri (Büyük) |
| %100 | `completion_pct >= 100` | 500 elmas + 100.000 altın + özel başlık: "Canavar Lordu" |

Milestone ödülleri tek seferlik — ikinci kez verilemez.

**Milestone tetikleme zamanlaması**: Milestone kontrolü **her yeni (tür, tier) girişi kazanımında** çalışır (`unique_owned_entries` artışı, bkz. Formül 3). Satış sonrası kontrol gerekmez — milestone'lar tek seferlik olduğundan geri alınmaz. `completion_pct` UI'da anlık güncellenir ama milestone tetikleyici sadece kazanımdadır.

**Kural 7 — Canavar Satma**

Üç satış modu:

| Mod | Açıklama |
|-----|----------|
| **Tek satış** | Envanter listesinden canavar seç → onayla → altın kazan |
| **Toplu satış** | Birden fazla canavar seç (checkbox) → toplu onayla → toplam altın |
| **Otomatik satış filtresi** | Kurallar belirle, eşleşen düşen canavarlar otomatik satılır |

Satış fiyatları (registry'den — `monster_sell_value`):
| Tier | Satış Fiyatı |
|------|-------------|
| F | 100 altın |
| D | 200 altın |
| C | 400 altın |
| B | 700 altın |
| A | 1.200 altın |
| S | 2.000 altın |
| SS | 3.500 altın |

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
| Tier filtresi | F / D / C / Kapalı | Kapalı |
| Yıldız filtresi | ★0 / ★1 altı / Kapalı | Kapalı |
| Güvenlik kilidi | B ve üstü tier otomatik satış filtresinden muaf | Açık (kapatılamaz) |

Otomatik satış kuralları:
1. Filtre **sadece yeni düşen** canavarlara uygulanır — mevcut envantere dokunmaz
2. **B, A, S ve SS tier canavarlar otomatik satış filtresinden muaftır** — güvenlik kilidi. **(Düzeltme — design-review 2026-07-02, systems-designer'ın dosya-içi çelişki bulgusu)**: Bekleme alanı süresi dolduğunda B/A tier'lar 14 gün sonra satılır (bkz. Kural 4.3). **S/SS bu satıştan tamamen muaftır — süresizdir, asla otomatik satılmaz** (Kural 4.3). Önceki sürümdeki "S/SS 30 gündür" notu, S/SS'in süresiz-bekleme haline getirilmesinden önceki bir modele aitti ve hatalıydı; kaldırıldı.
3. Otomatik satılan canavarlar loot raporunda "otomatik satıldı: +[X] altın" notu ile gösterilir
4. Filtre oyuncu tarafından açılıp kapatılabilir
5. İlk kez keşfedilen (Pokédex'e yeni eklenen) canavar otomatik satılmaz — ilk kopya her zaman koleksiyona eklenir

**Kural 9 — Canavar Kilitleme**

Değerli canavarları yanlışlıkla satmaktan korumak için:
1. Oyuncu herhangi bir canavarı "kilitli" olarak işaretleyebilir
2. Kilitli canavarlar: satılamaz, yıldız birleştirmede malzeme olamaz, otomatik satıştan muaf
3. Kilitleme/kilidi kaldırma ücretsiz ve anlık
4. Takıma eklenen canavarlar otomatik kilitlenmez (oyuncunun tercihi)

**Kural 10 — Evrim Yürütme (Tier Yükseltme) (REVİZE — 2026-07-02, element sistemi kaldırıldı)**

Bu sistem, `canavar-guclendirme.md`'nin (Deprecated) evrim yürütme sorumluluğunu devralır. `level-deneyim-sistemi.md` Kural 5/6'nın kararı gereği **seviye tavanına ulaşmak evrim için TEK gate'tir** — canavar-guclendirme.md'nin eski başarı-oranı/pity mekaniği (rastgele evrim başarısızlığı) bu kararla çelişir ve kaldırılmıştır: evrim artık **deterministik**tir, gereksinimler karşılandığında her zaman başarılı olur.

**Revizyon notu**: Element sistemi prototype kapsamından tamamen kaldırıldığı için (kullanıcı kararı, bkz. `systems-index.md` Deprecated listesi) eski element bazlı evrim malzemesi (Ateş Taşı/Kristali/Özü) kaldırıldı. Yerine, kullanıcıyla tartışılıp netleştirilmiş **canavar+altın (+ üst tier'larda Evrim Taşı)** modeli geldi (aşağıda).

Evrim koşulları:
1. `IsEvolutionEligible(instanceId)` `true` dönmeli (`level-deneyim-sistemi.md` Kural 5 — pet kendi tier'ının seviye tavanına ulaşmış olmalı, ör. F tier Lv10)
2. Oyuncu gerekli evrim malzemesine sahip olmalı (aşağıdaki tablo — canavar+altın bazlı, element eşleşmesi YOKTUR)
3. Oyuncu "Evrimleştir" butonuna basar → malzeme tüketilir (canavarlar `ConsumeInstance()` ile yok edilir, altın `SpendGold()` ile düşer, üst tier'larda Evrim Taşı da tüketilir), tier bir üst kademeye geçer (F→D→C→B→A→S→SS), instance `level=1`, `xp=0` olur (`level-deneyim-sistemi.md` Kural 6), `lifetime_pet_level` **sıfırlanmaz**
4. Evrim arketipi **değiştirmez**; yeni tier'ın base stat havuzu (`canavar-veritabani.md` Kural 4 tablosu) otomatik devreye girer — ayrı bir "evrim stat bonusu" formülüne gerek yoktur (eski %40 çarpanı, tier-başına-stat-havuzu modeliyle gereksizleşti)
5. Evrim, yeni tier için ilgili Pokédex girişini "Sahip Olundu" durumuna geçirir (bkz. Kural 5/6)
6. `star_rank` evrimde **korunur** (yıldız ve tier bağımsız eksenlerdir, bkz. Kural 11)

**Evrim Malzemesi Gereksinimleri:**

| Geçiş | Malzeme | Miktar |
|-------|---------|--------|
| F→D | Tier'i F olan herhangi 4 canavar + altın | 4 canavar |
| D→C | Tier'i D olan herhangi 4 canavar + altın | 4 canavar |
| C→B | Tier'i C olan herhangi 4 canavar + altın | 4 canavar |
| B→A | Tier'i B olan herhangi 4 canavar + altın **+ Evrim Taşı** | 4 canavar + taş |
| A→S | Tier'i A olan herhangi 4 canavar + altın **+ Evrim Taşı** | 4 canavar + taş |
| S→SS | Tier'i S olan herhangi 4 canavar + altın **+ Evrim Taşı** | 4 canavar + taş |

**Not**: Tüketilen 4 canavar **aynı tür olmak zorunda değildir** — sadece tüketilecek canavarın tier'ı, geçişin kaynak tier'ıyla eşleşmelidir (ör. F→D için tier'i F olan herhangi 4 canavar, türleri farketmez). Bu, kasıtlı bir tasarım kararıdır: Kural 11'in yıldız birleştirme malzemesi **aynı tür** kopya gerektirir — evrim malzemesi bilerek **tür-bağımsız** tutuldu ki iki sistem aynı kopya havuzu için rekabet etmesin (bir F-tier kopya ya yıldıza ya evrime harcanır, ikisi arasında rekabet yerine düşük değerli farklı türden F-tier'ları "evrim yakıtı" olarak kullanma teşviki sağlanır).

**Evrim Taşı**: Element'siz, generic bir malzeme — Keşif Alanı/zindan loot droşundan kazanılır, tier'e göre düşme oranı nadirleşir (kesin oranlar `loot-odul-sistemi.md`'nin kapsamındadır). Altın miktarı ve Evrim Taşı loot oranları henüz dengelenmedi — `loot-odul-sistemi.md` ile birlikte bir sonraki balance oturumunda kalibre edilmeli (bkz. Tuning Knobs).

**Kural 11 — Yıldız Sistemi (Ascension) (YENİ — design-review 2026-07-02, Canavar Güçlendirme birleşmesi)**

Aynı canavarın kopyalarını birleştirerek stat tavanını kalıcı olarak yükseltir. `canavar-guclendirme.md`'nin (Deprecated) Kural 7'sinden devralınmıştır, nadirlik kademeleri Common-Legendary yerine F-D-C-B-A-S-SS olarak güncellenmiştir.

| Yıldız | Gereken Kopya (kümülatif) | Stat Bonusu |
|--------|---------------------------|-------------|
| ★1 | 1 | +5% |
| ★2 | 3 | +10% |
| ★3 | 6 | +15% |
| ★4 | 10 | +22% |
| ★5 (max) | 15 | +30% |

Yıldız kuralları:
1. Kopya canavar **aynı tür** olmalı (aynı `template_id`) — tier'i farketmez (bir F-tier kopya, D-tier'a evrimleşmiş ana canavarı yükseltebilir, çünkü ikisi de aynı türden başlar; bkz. Edge Cases)
2. Birleştirilen kopya `ConsumeInstance(instanceId)` ile tüketilir (Kural 2/Dependencies)
3. Yıldız bonusu tüm statlara çarpımsal uygulanır: `final_stat = StatAtLevel(monsterId, level) × (1 + star_bonus)` — `StatAtLevel` girdisi `level-deneyim-sistemi.md`'den, base stat `canavar-veritabani.md`'den gelir; bu sistem yalnızca `star_bonus` çarpanını ekler
4. Yıldız, evrimden bağımsızdır — evrim yıldızı resetlemez (Kural 10 madde 7), yıldız evrimi gerektirmez
5. Altın maliyeti: Formül 4 (Yıldız Altın Maliyeti)
6. ★5'e ulaşmış bir canavarın fazladan kopyası birleştirme için seçilemez — normal canavar olarak koleksiyona eklenir, satılabilir (bkz. Edge Cases)
7. Kilitli veya takımdaki canavarlar birleştirme malzemesi olamaz (Kural 3/9 ile tutarlı)

### States and Transitions

Canavar instance yaşam döngüsü:

| Durum | Tetikleyici | Sonraki Durum | Sahip Sistem |
|-------|-------------|---------------|--------------|
| **Düşme** | Loot sistemi `OnMonsterDropped` | **Envanterde** veya **Beklemede** | Bu sistem |
| **Beklemede** | Envanter dolu | **Envanterde** (yer açılınca) veya **Satıldı** (7 gün sonra) | Bu sistem |
| **Envanterde** | Instance oluşturuldu, envantere eklendi | **Takımda** / **Güçlendirildi** / **Satıldı** / **Birleştirildi** | Bu sistem |
| **Takımda** | Oyuncu takıma ekledi | **Savaşta** | Takım Kurma |
| **Güçlendirildi** | Seviye artışı (Level/Deneyim Sistemi) / evrim (Kural 10) / yıldız artışı (Kural 11) | **Envanterde** (daha güçlü) | Level/Deneyim Sistemi (seviye), Bu sistem (evrim, yıldız) |
| **Birleştirildi** | Yıldız birleştirme malzemesi olarak kullanıldı (Kural 11) | **Yok edildi** (tüketildi) | Bu sistem |
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
| **Canavar Veritabanı** | ← okur | Tür şablonu (id, arketip, nadirlik, evrim yolu) + tier-başına base stat havuzu | `GetMonsterDefinition(monsterId)` |
| **Loot / Ödül** | ← alır | Yeni canavar sinyali + evrim malzemesi drop tanımı | `OnMonsterDropped(monsterId)` → instance oluşturma |
| **Level / Deneyim Sistemi** | ← okur | Seviye/XP değeri, evrim uygunluk bayrağı, `StatAtLevel` girdisi | `GetPetLevel(instanceId)`, `IsEvolutionEligible(instanceId)` (Kural 10) |
| **Ekonomi** | → çağırır | Satış altını ekleme, envanter genişletme elması çekme, yıldız yükseltme altını çekme (Kural 11/Formül 4) | `GrantGold(amount)`, `SpendGems(amount)`, `SpendGold(amount)` |
| **Savaş Sistemi** | ← sorgular | Mevcut canavar listesi, aktif pet slot instance'ı, nihai stat (level+star birleşik) | `GetOwnedMonsters(filters?)`, `IsActivePet(instanceId)`, `GetFinalStats(instanceId)` |
| **Koleksiyon UI** | → sağlar | Envanter listesi, Pokédex durumu, kapasite bilgisi, güçlendirme durumu (evrim/yıldız) | `GetInventory()`, `GetPokedexStatus()`, `GetCapacity()`, `GetEnhancementInfo(instanceId)` |
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

### Formül 2: Güçlendirilmiş Canavar Satış Fiyatı (REVİZE — design-review 2026-07-02)

**Revizyon gerekçesi**: `level-deneyim-sistemi.md` canavar seviye atlamayı tamamen altınsız hale getirdi (savaş XP'si + XP İksiri ile, hiçbir gram altın harcanmadan). Önceki `sell_level_bonus=0.02` katsayısı, seviyeyi altınla satın almanın bir maliyeti olduğu (ve satışın bu maliyeti kısmen geri ödediği) varsayımıyla kalibre edilmişti — o denge artık geçerli değil. **Kullanıcı netleştirmesi (2026-07-02)**: Seviyelenme hâlâ gerçek bir yatırımdır — sadece altın değil, oyuncunun zaman/emek yatırdığı XP İksirleri ve Keşif Alanı/zindan ilerlemesiyle kazanılır. Bu yüzden level bonusu tamamen kaldırılmıyor, **küçültülerek** korunuyor: yatırım hâlâ satışta hafifçe yansır, ama tek başına anlamlı bir "bedava seviye atlat → kârına sat" döngüsü oluşturamayacak kadar küçük.

`sell_price = floor(base_sell_value × (1 + level × sell_level_bonus + star_rank × sell_star_bonus))`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel satış fiyatı | base_sell_value | int | 100–3.500 | Registry: `monster_sell_value` (F-SS, bkz. Kural 7 tablosu) |
| Seviye | level | int | 1–tier_cap | Canavar seviyesi — **tier-relative cap** (bkz. `level-deneyim-sistemi.md` Kural 5: F=10, D=15, C=20, B=25, A=30, S=35, SS=40). Önceki sürümde hatalı şekilde sabit 1–50 aralığı kullanılıyordu. |
| Seviye bonus katsayısı | sell_level_bonus | float (tuning knob) | 0.01 | **Düşürüldü (eski: 0.02)** — bkz. yukarıdaki revizyon gerekçesi |
| Yıldız sırası | star_rank | int | 0–5 | Canavar yıldızı |
| Yıldız bonus katsayısı | sell_star_bonus | float (tuning knob) | 0.10 | Değişmedi |
| Satış fiyatı | sell_price | int | 101–6.650 | Sonuç altın |

**Çıktı Aralığı**: 101 (F Lv1 ★0) – 6.650 (SS Lv40 ★5 — kendi tier tavanında, mutlak maksimum). **Düzeltme**: Önceki sürümde bu bölümde iç çelişkili üç farklı üst sınır (1.750 / 8.750 iki kez) veriliyordu — tek, doğrulanmış üst sınır 6.650'dir.

**Örnek satışlar (tier-relative level cap'lerle yeniden hesaplandı):**

| Canavar | Level (tier cap) | Yıldız | Hesaplama | Satış Fiyatı |
|---------|-------|--------|-----------|-------------|
| F Lv1 ★0 | 1 | 0 | 100 × (1 + 0.01 + 0) | **101** |
| F Lv10 ★0 (cap) | 10 | 0 | 100 × (1 + 0.10 + 0) | **110** |
| C Lv20 ★2 (cap) | 20 | 2 | 400 × (1 + 0.20 + 0.20) | **560** |
| B Lv25 ★5 (cap) | 25 | 5 | 700 × (1 + 0.25 + 0.50) | **1.225** |
| A Lv30 ★5 (cap) | 30 | 5 | 1.200 × (1 + 0.30 + 0.50) | **2.160** |
| SS Lv40 ★5 (cap, mutlak maks.) | 40 | 5 | 3.500 × (1 + 0.40 + 0.50) | **6.650** |

**Denge notu**: Satış fiyatları kasıtlı olarak düşük — canavar satma kaynak kazanımının ana yolu olmamalı. Ana altın kaynağı Keşif Alanı/zindan loot'udur. Satış envanter yönetimi aracıdır. Level bonusunun küçültülmesiyle, tam tier tavanında (en yüksek yatırım) bile satış fiyatı base değerin ×1.90'ını geçmez — önceki ×2.5 tavanına göre belirgin şekilde daha az kârlı.

**Bilinen sınırlama (design-review 2026-07-02, systems-designer bulgusu — belgelenip düzeltilmedi, kullanıcı kararı)**: B tier ve üstünde formül küçük bir sınır ihlali üretir — maksimize edilmiş (Lv cap, ★5) bir alt-tier canavar, taze düşen (Lv1, ★0) bir üst-tier canavardan daha pahalıya satılabilir (ör. B Lv25★5=1.225 altın > A Lv1★0=1.212 altın; fark tier yükseldikçe büyür, S Lv35★5=3.700 > SS Lv1★0=3.535). Mutlak fark küçüktür (₺13-165) ve düzeltmek `sell_level_bonus`/`sell_star_bonus`/`base_sell_value` tablosunun geniş kapsamlı yeniden dengelenmesini gerektirir — bu revizyonda ertelendi, gelecekteki bir balance-tuning oturumunda ele alınmalı. **Tolere edilen davranış notu (3. tur)**: Bu, QA tarafından bug olarak açılmamalı — bkz. Acceptance Criteria'da AC#11'in altındaki not. **UI notu**: Envanter listesi satış-fiyatına göre sıralanabiliyorsa, bu sınır ihlali oyuncuya tuhaf görünebilir ("neden maxlanmış B'im taze A'mdan üstte?") — Koleksiyon UI GDD'sinde ele alınmalı, bu dosyanın kapsamı dışında.

**Satış-yoluyla-altın riski (design-review 2026-07-02 — 2. tur economy-designer bulgusu, 3. tur re-review'da somutlaştırıldı)**: S/SS canavarlar Kural 4.3 gereği hiçbir zaman otomatik satılmadığından kalıcı bekleme alt-alanında sınırsız birikebilir. Bir SS Lv40★5 kopyasının manuel satışı 6.650 altın verir.

**Güncelleme (3. tur, economy-designer)**: 2. turda bu risk "`kesif-alani.md`'nin somut altın/saat oranları belirlenmeden doğrulanamaz" denerek ertelenmişti. Bu erteleme gerekçesi artık geçersiz — `ekonomi.md`'nin MEVCUT idle-altın formülüyle (orta-oyun `idle_gold_per_minute=25` → 24 saat = 27.000 altın) çapraz doğrulama yapılabiliyor: bir SS Lv40★5'in satışı (6.650 altın) idle gelirin yalnızca **~6-15 dakikasına** eşit. **Gerçek risk yeniden çerçevelendi**: sorun "S/SS'in nadiren satılıp baskın kaynağa dönüşmesi" değil — **tüm altın ekonomisi (idle + kat ilerlemesi) zaten bu dosyanın sink'lerinden çok daha büyük**; S/SS satışı bu mevcut enflasyonu yalnızca marjinal olarak büyütüyor (bkz. Formül 4 sonrası not ve Tuning Knobs). Bu artık "izlenmeli" değil, mevcut verilerle bugün ölçülebilir bir bulgu — ama düzeltmesi bu dosyanın değil, `ekonomi.md`'nin idle-altın dengesinin kapsamındadır (bkz. Dependencies → Çapraz bağımlılık notları, yeni madde).

### Formül 3: Koleksiyon Tamamlama Yüzdesi (REVİZE — design-review 2026-07-02)

**Revizyon gerekçesi**: Önceki sürüm `total_species_count=60`'ı sabit varsayıyordu (20 tür × 3 form). Bu hem Kural 6b milestone tablosuyla (20 payda) hem de F-D-C-B-A-S-SS tier zincirinin gerçek (türe göre 1-4 arası değişen) uzunluğuyla çelişiyordu (bkz. design-review bulgu game-designer#1/#2, systems-designer#3, qa-lead#1). Bu revizyonda hem denominatör hem tetikleme mantığı Kural 5'in Tier Zinciri modeline hizalandı.

`completion_pct = floor(unique_owned_entries / total_pokedex_entries × 100)`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Benzersiz sahip olunan giriş | unique_owned_entries | int | 0–total | Pokédex'te en az bir kez "Sahip Olundu" durumuna geçmiş **(tür, tier)** çiftleri (permanent, bkz. Kural 5) |
| Toplam Pokédex girişi | total_pokedex_entries | int | **Türetilmiş (sabit değil)** | `Σ` tüm tanımlı türler için `ChainLength(türün başlangıç kademesi)` — bkz. Kural 5 zincir-uzunluğu tablosu (F/D/C=3, B=4, A=3, S=2, SS=1). Canavar Veritabanı'ndan `GetTotalPokedexEntryCount()` ile okunur, bu GDD'de hardcode edilmez. |
| Tamamlama yüzdesi | completion_pct | int | 0–100 | Floor ile tam sayı |

**Çıktı Aralığı**: 0% – 100%

**Not**: "Sahip olunan" = Pokédex'te "Sahip Olundu" kalıcı durumuna geçmiş (en az bir kez o (tür, tier) kademesinde bir instance kazanılmış — ya doğrudan loot'tan ya da evrimle o kademeye ulaşarak). Satılan/evrimle geçilmiş kademeler sayılmaya DEVAM EDER (kalıcı). Pokédex "Keşfedildi" durumu ayrı izlenir, completion_pct'ye dokunmaz — yalnızca "Sahip Olundu" (permanent) sayılır.

**Sayısal örnek gerektiriyor**: Bu formülün somut bir sayısal `total_pokedex_entries` değeri (eski sabit "60" gibi), Canavar Veritabanı'nın MVP 15-20 türünün başlangıç-kademe dağılımı netleşmeden hesaplanamaz — bu dağılım bu GDD'nin kapsamı dışındadır (`canavar-veritabani.md`'nin sorumluluğu). ⚠️ **UYARI — bu bir gerçek değer DEĞİLDİR, koda kopyalanmamalı** (economy-designer bulgusu, 3. tur qa-lead/systems-designer tarafından "gerçek bir landmine riski" olarak doğrulandı): Eğer MVP'nin 20 türü şöyle dağılırsa — 5×F(3), 5×D(3), 4×C(3), 3×B(4), 2×A(3), 1×S(2) → `total_pokedex_entries = 5×3+5×3+4×3+3×4+2×3+1×2 = 15+15+12+12+6+2 = 62`. Oyuncu bu dağılımda 46 girişe sahip olsa: `floor(46/62×100) = %74` → henüz %75 milestone'a ulaşmamış. **Gerçek toplam, `canavar-veritabani.md`'nin nihai tür roster'ı kilitlendiğinde yeniden hesaplanmalı (bkz. Open Questions #2), VE `canavar-veritabani.md` Kural 6'nın Form-gate çelişkisi çözüldükten sonra (bkz. Open Questions #6, artık onaylı blocker) — bu tablo iki ayrı bağımlılığa bağlıdır.** Bu sayı belirlenene kadar Pokédex ödül ekonomisinin (elmas bütçesi) implementasyona alınmaması gerekir. **Kod-koruma notu (3. tur)**: AC#7/#8'deki test sabiti `MOCK_TOTAL_POKEDEX_ENTRIES__DO_NOT_SHIP` adlandırma kuralına tabidir — bu dosyadaki "62" örneği de dahil, hiçbir yerde çıplak sayısal literal olarak prod koduna geçmemelidir.

### Formül 4: Yıldız Altın Maliyeti (YENİ — design-review 2026-07-02, Canavar Güçlendirme birleşmesi, Kural 11)

`star_gold_cost = base_star_cost × star_rank × tier_cost_multiplier`

**Değişkenler:**
| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel yıldız maliyeti | base_star_cost | int (tuning knob) | 500 | ★1, F tier maliyeti |
| Yıldız sırası | star_rank | int | 1–5 | Yükseltilen yıldız |
| Tier maliyet çarpanı | tier_cost_multiplier | float (tuning knob) | 1.0–3.0 | F=1.0, D=1.3, C=1.8, B=2.4, A=2.8, S=3.2, SS=3.6 (eski Common-Legendary tablosunun F-SS'e taşınmış hali) |
| Yıldız maliyeti | star_gold_cost | int | 500–9.000 | Altın maliyeti (tek yıldız işlemi) |

**Çıktı Aralığı**: 500 (F tier ★1) – 9.000 (SS tier ★5: 500×5×3.6=9.000, mutlak maksimum tek işlem maliyeti).

**Maliyet tablosu (kümülatif, ★1→★5):**

| Tier | ★1 | ★2 | ★3 | ★4 | ★5 | Toplam |
|------|-----|-----|-----|-----|-----|--------|
| F | 500 | 1.000 | 1.500 | 2.000 | 2.500 | 7.500 |
| D | 650 | 1.300 | 1.950 | 2.600 | 3.250 | 9.750 |
| C | 900 | 1.800 | 2.700 | 3.600 | 4.500 | 13.500 |
| B | 1.200 | 2.400 | 3.600 | 4.800 | 6.000 | 18.000 |
| A | 1.400 | 2.800 | 4.200 | 5.600 | 7.000 | 21.000 |
| S | 1.600 | 3.200 | 4.800 | 6.400 | 8.000 | 24.000 |
| SS | 1.800 | 3.600 | 5.400 | 7.200 | 9.000 | 27.000 |

**Örnek**: C tier canavar ★3'e yükseltilirse: `500 × 3 × 1.8 = 2.700` altın.

**Denge notu**: Yıldız yükseltme, `level-deneyim-sistemi.md`'nin "canavar ilerlemesi altınsız" kararından **istisnadır** — çünkü kaynağı savaş XP'si değil, kopya canavar + altın kombinasyonudur (Ekipman/Dükkan Sistemi'nin altın sink'ine ek bir sink, çakışma değil). Yüksek-tier ★5 maliyeti (SS: 27.000 altın toplam) endgame altın sink'i olarak tasarlanmıştır.

**Ölçek uyumsuzluğu (YENİ — design-review 2026-07-02, 3. tur, systems-designer bulgusu)**: `tier_cost_multiplier` F→SS arasında yalnızca 3.6× ölçekleniyor (1.0→3.6) ama Kural 7'nin satış tablosu F→SS arasında 35× ölçekleniyor (100→3.500). Sonuç: bir maxlanmış SS kopyasının satışı (6.650 altın, Formül 2) kendi tam SS ★1-5 yıldız yolunun (27.000 altın) **~%24.6'sını** finanse ederken, bir maxlanmış F kopyasının satışı (110 altın) kendi F yıldız yolunun (7.500 altın) yalnızca **~%1.5'ini** finanse ediyor — SS-tier yıldız ilerlemesi kopya-satışıyla kendi kendini finanse etme açısından F-tier'den **~16× daha kolay**. Bu, yukarıdaki "Satış-yoluyla-altın riski" notunu güçlendiren somut, ölçülebilir bir bulgu — mekanik bu turda değiştirilmedi (kapsam `ekonomi.md`'nin altın dengesine bağlı), ama izleme notuna eklendi (bkz. Tuning Knobs).

## Edge Cases

- **If envanter dolu ve bekleme alanı da doluyken (10/10 beklemede, F-D-C-B-A tier) yeni canavar düşerse**: **(REVİZE, design-review 2026-07-02 — systems-designer'ın "hangi canavar satılır belirsiz" bulgusu)** Yeni canavar F-A tier ise, otomatik satılır ve altına çevrilir; loot raporunda "envanter + bekleme dolu — otomatik satıldı: +[X] altın" notu gösterilir. **Yeni canavar S/SS tier ise**: hiçbir zaman bu şekilde satılmaz — ayrı, sınırsız "kalıcı bekleme" alt-alanına eklenir (bkz. Kural 4.2-4.5), 10 slotluk kapasiteyi hiç etkilemez. Bekleme alanının kendisi (10/10 F-A) hiçbir zaman "yer açmak için" tahliye edilmez — yeni gelen canavar direkt satılır, mevcut bekleyenler yerinde kalır. Bu senaryoda F/D/C/B/A tier'ların hepsi teorik olarak bekleme alanında bulunabilir (C/B/A süresi 14 gün olduğundan erken tahliye olasılığı düşüktür, ama garanti değildir).

- **If bekleme alanı (10/10, F-A) dolu ve süresi dolan bir canavar birden fazla adayla eşleşirse (ör. hem F hem D tier süresi aynı anda dolmuşsa)**: **(REVİZE, design-review 2026-07-02 — FIFO netleştirmesi)** Süresi dolan TÜM canavarlar aynı giriş turunda satılır (tier'e bakılmaksızın, sadece "süresi dolmuş mu" kontrolü yapılır — bkz. Kural 4.3/AC#21). Bekleme alanı doluyken yeni bir canavar için yer açmak amacıyla erken tahliye YAPILMAZ — yer açma yalnızca doğal süre dolumuyla veya oyuncunun manuel "Envantere Al"/"Sat" işlemiyle gerçekleşir (bkz. Kural 4.4/16. AC, FIFO burada geçerlidir: en eski `acquired_date`'e sahip canavar önce envantere taşınır).

- **If oyuncu tüm canavarlarını satmaya çalışırsa (1 canavar kalana kadar)**: Son canavar satılamaz. "Sat" butonu devre dışı kalır. "Koleksiyonda en az 1 canavar bulunmalı" mesajı gösterilir.

- **If otomatik satış filtresi açıkken ilk kez keşfedilen bir F tier canavar düşerse**: İlk kopya otomatik satılmaz — Pokédex'e eklenir ve koleksiyona girer. Aynı türün sonraki kopyaları filtre kuralına tabidir.

- **If oyuncu otomatik satış filtresini "F" olarak ayarlamışken tüm F tier canavarlarını satmışsa ve yeni F tier canavar düşerse**: Canavar zaten otomatik satılır. Pokédex'te "sahip olundu" durumu korunur ama tamamlama yüzdesinden düşer (aktif instance yok).

- **If canavar bekleme alanında süre dolunca ama oyuncu o sürede hiç giriş yapmadıysa**: Timer oyuncu girişinde hesaplanır (offline süre dahil). Giriş anında süresi dolmuş canavarlar satılır (F/D: 7 gün, C/B: 14 gün), altın eklenir ve bildirim gösterilir. C/B canavar satıldıysa özel bildirim: "Nadir canavarınız bekleme süresinde satıldı — [X] altın kazandınız."

- **If yıldız birleştirme için seçilen kopya canavar bekleme alanındaysa**: Bekleme alanındaki canavarlar birleştirme malzemesi olarak seçilebilir — bu onları bekleme alanından kaldırır ve tüketir. Slot açma gerekmez.

- **If oyuncu envanter genişletme satın alırken elması yetersizse**: "Satın Al" butonu devre dışı. Mevcut elmas ve gereken elmas gösterilir. "Elmas kazan" yönlendirmesi sunulur.

- **If Pokédex %100 tamamlama ödülü alındıktan sonra yeni canavar türleri eklenirse (içerik güncellemesi)**: Tamamlama yüzdesi yeniden hesaplanır (ör. 20/22 = %90). Yeni milestone'lar tanımlanmaz — mevcut milestone'lar sabit kalır. İleride Tier 2+ güncellemesinde yeni milestone'lar eklenebilir.

- **If koleksiyon tamamlama yüzdesi tam milestone eşiğinde ama oyuncu önceki milestone'u almamışsa**: Tüm karşılanmış ve alınmamış milestone'lar sırayla verilir. %50'yi atlayıp %75'e ulaşılırsa, hem %50 hem %75 ödülü verilir.

- **If aynı anda iki loot roll aynı tür canavar düşürürse (boss + normal roll)**: Her ikisi de ayrı instance olarak oluşturulur — 2 adet aynı canavar koleksiyona eklenir. Envanter 2 slot kullanır.

- **If oyuncu kilitli bir canavarı toplu satış seçimine dahil etmeye çalışırsa**: Kilitli canavarlar seçilemez (checkbox devre dışı). Seçim listesinde kilitli ikon gösterilir.

- **If favoriye alınmış canavar otomatik satış filtresine uysa bile**: Favori canavarlar otomatik satıştan muaftır — filtre kuralı override edilir.

- **If envanter tam 20/20 iken oyuncu bir canavarı yıldız birleştirmeye verirse (19/20 olur) ve ardından bekleme alanında canavar varsa**: Bekleme alanından en eski canavar otomatik olarak envantere taşınır. FIFO sırası uygulanır (bu, yıldız birleştirmenin tetiklediği slot açılması dahil AC#16'nın kapsadığı genel "slot açılırsa" senaryosunun bir örneğidir).

- **[REVİZE — 2026-07-02, Kural 10] Evrim malzemesi (canavar/altın/Evrim Taşı) yetersizse**: Evrimleştir butonu devre dışı kalır. UI gereken malzemeyi ve mevcut miktarı gösterir (ör. "Tier F canavar ×4 gerekli, elinizde 2" veya "Evrim Taşı ×1 gerekli, elinizde 0").

- **[YENİ — design-review 2026-07-02, Kural 10] `IsEvolutionEligible` `true` dönerken evrim anında malzeme kontrolü başarısız olursa (ör. eşzamanlı başka bir işlemde malzeme tüketildiyse)**: İşlem temiz şekilde reddedilir — hiçbir malzeme tüketilmez, hiçbir state değişmez, pet seviye tavanında beklemeye devam eder. Bu, `level-deneyim-sistemi.md`'nin daha önce bu dosyaya devrettiği açık uçlu edge case'i kapatır.

- **[YENİ — design-review 2026-07-02, 3. tur, Kural 11, systems-designer bulgusu — Kural 10'un race-condition kapsamındaki asimetri]** **Aynı kopya canavar veya aynı ana canavar hedefiyle eşzamanlı iki "Birleştir" işlemi gönderilirse (ör. çift dokunma veya ağ gecikmesiyle tekrar gönderim)**: İşlem, Kural 10'daki malzeme-kontrolü mantığıyla aynı prensiple ele alınır — `ConsumeInstance(instanceId)` ve `SpendGold(star_gold_cost)` tek bir atomik transaction olarak ele alınır (ikisi birden başarılı olur veya ikisi de geri alınır, kısmi durum asla kalıcı olmaz). İkinci istek, ilk işlem tamamlandıktan sonra devreye girerse: hedef kopya `instance_id` artık mevcut olmadığından ("zaten tüketildi") temiz şekilde reddedilir — "Bu canavar zaten kullanıldı" mesajı gösterilir, ikinci kez altın çekilmez, `star_rank` ikinci kez artmaz.

- **[YENİ — design-review 2026-07-02, Kural 11] Oyuncu ★5 canavarın fazladan kopyasını kazanırsa**: Kopya normal bir canavar olarak koleksiyona eklenir (kendi instance_id'si, kendi envanter slotu) — birleştirme için seçilemez (maks yıldıza ulaşılmış). Fazla kopya normal şekilde satılabilir veya kilitlenebilir.

- **[YENİ — design-review 2026-07-02, Kural 4.7] Kalıcı beklemedeki bir S/SS canavar Güçlendirme (evrim/yıldız) işlemine veya takıma eklenmeye çalışılırsa**: İşlem engellenir — "Önce envantere alın" uyarısı gösterilir. Envanterde slot açılıp canavar manuel taşındıktan sonra bu işlemler tekrar mümkün olur.

*`systems-designer` konsültasyonu: `/design-review` 2026-07-02, tam mod.*

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetMonsterDefinition(monsterId)` — tür şablonu (kimlik, stat, arketip, nadirlik, evrim yolu, tier-başına base stat havuzu) | Olmadan instance oluşturulamaz, evrim/yıldız stat hesabı yapılamaz |
| **Level / Deneyim Sistemi** | Sert | `IsEvolutionEligible(instanceId)` — pet tier level tavanı kontrolü, `StatAtLevel` girdisi (Kural 10/11) | Olmadan evrim tetiklenemez, yıldız bonus formülü çalışmaz |
| **Loot / Ödül** | Sert | `OnMonsterDropped(monsterId, tier)` — canavar kazanım sinyali. **Düzeltme (design-review 2026-07-02)**: `tier` parametresi eklendi (F-D-C-B-A-S-SS başlangıç kademesi); önceki tek-parametreli imza F-SS nadirlik rulosunun ürettiği kademeyi taşımıyordu. `loot-odul-sistemi.md`'nin kendi arayüz tanımı bu parametreyi henüz yansıtmıyor — o dosyanın revizyonunda eklenmelidir. | Olmadan canavar kazanma kaynağı yok |
| **Ekonomi** | Sert | `GrantGold(amount)` — satış altını. `SpendGems(amount)` — envanter genişletme. `SpendGold(amount)` — yıldız yükseltme (Kural 11/Formül 4). | Olmadan satış, genişletme ve yıldız yükseltme çalışmaz |
| **Kaydetme/Yükleme** | Sert | `SaveCollectionState()` / `LoadCollectionState()` — tüm instance verisi, Pokédex, otomatik satış kuralları | Olmadan ilerleme kaybolur |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Keşif Alanı Sistemi** | Sert | `OnEnemyEncountered(monsterId, tier)` — Pokédex keşfi (düşman olarak karşılaşma). **YENİ (design-review 2026-07-02)**: `kesif-alani.md` artık MVP'nin ana canavar kazanım kaynağı (bkz. Kural 1) ama bu sinyali kendi Downstream tablosunda tanımlamıyor — bu, `kesif-alani.md`'nin kendi revizyon oturumunda kapatılması gereken bir arayüz boşluğudur. | ⚠️ MVP'de bu sinyalin somut bir kaynağı yok — bloklayıcı değil (loot yoluyla "Sahip Olundu" akışı çalışır) ama "Keşfedildi" (savaşarak karşılaşma, henüz sahip olunmadan) akışı MVP'de tetiklenemez |
| **Zindan Keşif** | Yumuşak *(Tier 2)* | `OnEnemyEncountered(monsterId, tier)` — aynı sinyal, Tier 2 özel etkinlik içeriğinden | `zindan-kesif.md` artık Tier 2 (systems-index 2026-06-30 pivotu) — MVP'de zorunlu değil |
| **Koleksiyon / Envanter UI** | Sert | `GetInventory()`, `GetPokedexStatus()`, `GetCapacity()` — envanter ve Pokédex verisi | UI verileri bu sistemden gelir |
| **İlerleme Döngüleri** | Yumuşak | Koleksiyon tamamlama yüzdesi, milestone tetikleyicileri | İlerleme metrikleri |
| **Savaş Sistemi** | Yumuşak | `GetOwnedMonsters(filters?)` — aktif pet seçim havuzu | Pet seçim arayüzü |
| **Level / Deneyim Sistemi** | Sert (çift yönlü, bkz. Upstream) | `IsEvolutionEligible(instanceId)` — pet tier level tavanı kontrolü (level-deneyim-sistemi.md Kural 5) | **Çözüldü (design-review 2026-07-02)**: Bu GDD artık F-D-C-B-A-S-SS tier modelini benimsiyor ve evrim transaction'ının kendisini (materyal kontrolü, deterministik yürütme) Kural 10'da tanımlıyor — `IsEvolutionEligible` arayüzü tüketilmeye hazır. Kalan bağımlılık: `canavar-veritabani.md`'nin tier-içi form gate'i (Kural 6) hâlâ `level-deneyim-sistemi.md`'nin "seviye tavanı = tek gate" kararıyla çelişiyor — bu, `canavar-veritabani.md`'nin kendi revizyon oturumunda kapatılmalı (Open Questions #6), bu dosyanın kendi kapsamını bloklamıyor. |

### Çapraz bağımlılık notları

- Canavar Veritabanı bu sistemi downstream olarak listeliyor ✅
- **Canavar Güçlendirme (`canavar-guclendirme.md`) — DEPRECATED (design-review 2026-07-02)**: Bu sistem bu dosyaya birleştirildi (Kural 10/11). `canavar-guclendirme.md` dosyasının kendisi Deprecated olarak işaretlendi (bkz. o dosyanın güncellenmiş Status satırı) — systems-index.md'nin "birleşti" notuyla artık tutarlı.
- Loot / Ödül bu sistemi downstream olarak listeliyor ✅
- **Keşif Alanı Sistemi (`kesif-alani.md`) GDD**: Mevcut (Status: Designed) ama bu dosyayı/Pokédex-keşif sinyalini downstream olarak listelemiyor — **stale/eksik, `kesif-alani.md`'nin revizyonunda kapatılmalı** (design-review 2026-07-02 bulgusu)
- Zindan Keşif GDD: Mevcut, ama artık Tier 2 (systems-index 2026-06-30 pivotu) — **düzeltme**: önceki sürümde "henüz yazılmadı" deniyordu, bu artık doğru değil
- **Koleksiyon UI GDD (`koleksiyon-envanter-ui.md`)**: **Düzeltme (design-review 2026-07-02) — stale referans giderildi**: Önceki sürüm "henüz yazılmadı" diyordu; dosya artık mevcut (systems-index #18, Status: Designed)
- **Ekonomi GDD (`ekonomi.md`) — YENİ (design-review 2026-07-02, 3. tur, economy-designer bulgusu)**: `ekonomi.md` hâlâ eski "altınla seviye/evrim" modelini taşıyor (`GetLevelUpCost(level, rarity)` arayüzü, "C tier 1. evrimi → 2.500 altın döner" örneği — satır ~127/163/390, kendi AC#11) — bu doğrudan bu dosyanın Kural 10'undaki "Altın maliyeti YOKTUR" kararıyla çelişiyor. Bu dosya evrim-altın-maliyetini kapsam dışı bırakmakta haklı (Kural 10.3 net), ama `ekonomi.md`'nin kendisi bu nedenle stale/iç-çelişkili kalıyor — **`ekonomi.md`'nin kendi revizyon oturumunda kapatılmalı**. Ayrıca `ekonomi.md`'nin idle-altın formülü (orta-oyun `idle_gold_per_minute=25`) bu dosyanın Formül 4 yıldız sink'ini (SS ★5 = 27.000 altın = 1 günlük idle geliri) önemsizleştiriyor — bkz. Formül 4 sonrası "Ölçek uyumsuzluğu" notu ve Tuning Knobs → S/SS satış riski. `ekonomi.md`'nin idle-altın dengesi gözden geçirilmeden bu dosyanın altın sink'leri (Formül 4, Kural 7) anlamlı şekilde test edilemez.

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `base_inventory_capacity` | 20 | 15–30 | Başlangıçta çok geniş → genişletme motivasyonu yok | Çok dar → erken frustrasyon, sürekli satma zorunluluğu |
| `inventory_expansion_size` | 5 | 3–10 | Her genişletme çok değerli → ilk genişletme elmas sink çok güçlü | Genişletme anlamsız → motivasyon düşer |
| `base_expansion_cost` | 50 | 25–100 | İlk genişletme pahalı → yeni oyuncu frustrasyonu | İlk genişletme bedava hissettirir → elmas sink yok |
| `max_inventory_capacity` | 60 | 40–100 | Çok geniş → envanter yönetimi gereksiz | Çok dar → endgame koleksiyon sınırlı |
| `max_expansion_count` | 8 | 5–12 | Çok fazla → üstel maliyet anlamsız | Çok az → kapasite artışı yetersiz |
| `pending_buffer_size` | 10 | 5–20 | Bekleme çok geniş → envanter yönetimi gevşer | Bekleme çok dar → canavar kaybı riski artar |
| `pending_expiry_days` | 7 (F/D tier) | 3–14 | Çok uzun → oyuncu umursamaz | Çok kısa → geri dönüş süresi yetersiz |
| `pending_expiry_days_rare` | 14 (C/B/A tier) | 7–30 | Çok uzun → bekleme alanı tıkanır | Çok kısa → nadir canavar kaybı riski |
| *(kaldırıldı)* ~~`pending_expiry_days_epic` (S/SS, eski: 30)~~ | — | — | **REVİZE (design-review 2026-07-02)**: S/SS tier artık süre aşımından tamamen muaf (kalıcı bekleme, bkz. Kural 4.3) — bu knob'a gerek kalmadı. | |
| `discovery_gem_reward` | 5 | 2–10 | Keşif ödülü çok cömert → elmas enflasyonu | Keşif hissedilmez |
| `first_owned_gem_reward` | 10 | 5–25 | İlk sahiplik çok cömert → elmas enflasyonu | Heyecan düşük |
| `first_owned_gold_reward` | 500 | 200–1.000 | — | — |
| `milestone_25_gems` | 50 | 25–100 | Milestone çok cömert → ekonomi bozulur | Milestone motivasyonu düşer |
| `milestone_100_gems` | 500 | 250–1.000 | — | — |
| `sell_level_bonus` | 0.01 *(düşürüldü, eski: 0.02)* | 0.005–0.02 *(daraltıldı, eski: 0.01–0.05)* | Güçlü canavar satmak çok karlı → altınsız-seviyeleme + satış döngüsü kâr üretir (bkz. Formül 2 revizyon gerekçesi) | Seviye farkı satışta hissedilmez |
| `sell_star_bonus` | 0.10 | 0.05–0.20 | Yıldızlı canavar satmak çok karlı → yıldız amacı kayar | Yıldız farkı satışta hissedilmez |
| `evolution_material_costs` | Kural 10 tablosu (4 canavar + altın her geçişte; B→A/A→S/S→SS'de ek Evrim Taşı) | — | Malzeme çok fazla → evrim grind duvarı | Malzeme çok az → evrim değersiz hisseder |
| `base_star_cost` | 500 | 200–1.000 | Yıldız çok pahalı → kopya sink zaten yeterli | Yıldız bedava → altın sink yok |
| `tier_cost_multiplier` (yıldız) | F=1.0 .. SS=3.6 | 1.0–4.0 aralığında ölçeklenir | Üst tier yıldız aşırı pahalı → SS canavarlar hiç yıldızlanmaz | Tier farkı yıldız maliyetinde hissedilmez |
| `star_bonus_5` (max yıldız stat bonusu) | 0.30 | 0.20–0.50 | ★5 çok baskın → ★5'siz yarışılamaz | ★5 hedeflemeye değmez |

**Etkileşim Uyarıları**:
- `base_inventory_capacity` × `inventory_expansion_size` × `base_expansion_cost` birlikte envanter genişletme ekonomisini belirler. Kapasiteyi artırmak genişletme ihtiyacını azaltır.
- **Diamond Budget Balance (REVİZE — design-review 2026-07-02, 2. tur)**: Önceki sürümdeki sabit rakamlar (60 form × 15 elmas = 900, toplam ~1.750-1.950 elmas — üç farklı iç tutarsız hesap) kaldırıldı. Pokédex elmas bütçesi artık Formül 3'ün `total_pokedex_entries` türetilmiş değerine bağlıdır ve türlerin başlangıç-kademe dağılımı netleşmeden (Canavar Veritabanı'nın nihai roster'ı) **kesin bir sayı olarak hesaplanamaz** — bkz. Open Questions #2. Genel formül: `toplam_pokedex_elmas = total_pokedex_entries × (discovery_gem_reward + first_owned_gem_reward) + Σ(milestone_gems)`. **Bu sayı belirlenene kadar Pokédex ödül ekonomisi (elmas bütçesi) implementasyona alınmamalıdır** (economy-designer bulgusu — bu belirsizlik dokümantasyon eksikliği değil, gerçek bir denetlenebilirlik açığıdır: QA milestone tetiklemelerini gerçek değerlerle doğrulayamaz, ekonomi dashboard'ları elmas/saat hesaplayamaz).
  - **Genişletme sink'i tarafı (sabit, değişmedi)**: Full expansion sink = 12.750 elmas (8 genişletme).
  - **Musluk tarafı — DÜZELTİLMİŞ HESAP (design-review 2026-07-02, 2. tur, economy-designer)**: Önceki "~42 ay / 300-400 elmas/ay" projeksiyonu geçersizdi — arena ödüllerini (Tier 2, MVP'de YOK — `ekonomi.md` ile çapraz doğrulandı) içeriyordu. MVP-only tekrarlı kaynaklarla (günlük giriş 7 elmas/gün + haftalık görev 50 elmas/hafta) yeniden hesaplandı: `7×9 gün/ay ≈ 63/ay + 50×4.3 hafta/ay ≈ 200/ay = ~263 elmas/ay`. Buna göre 12.750 elmas'a ulaşmak `12.750 / 263 ≈ 48.5 ay` sürer (önceki 42 aya göre ~%15 daha uzun). **Bu rakam da illüstratiftir** — Pokédex ödülleri (yukarıdaki gerçek `total_pokedex_entries` bekleyen kısım) bu hesaba dahil değildir; gerçek toplam musluk hem bu 263/ay hem de nihai Pokédex bütçesinin toplamı olacaktır.
- `pending_buffer_size` × `pending_expiry_days` birlikte canavar kaybı riskini belirler (yalnızca F-A tier için — S/SS artık bu riskten muaf, bkz. Kural 4.3). İkisini düşürmek agresif envanter yönetimi zorunluluğu yaratır.
- `sell_level_bonus` × `sell_star_bonus` × max level (tier-relative, en yükseği SS=40) × max star (5) birlikte maximum satış çarpanını belirler: base × (1 + 40×0.01 + 5×0.10) = base × 1.90 **(düzeltildi, eski: ×2.5)**. SS max = 3.500 × 1.90 = 6.650 altın. Ekonomi GDD altın kaynaklarıyla çakışmamalı.
  - **S/SS satış riski — GÜNCELLENMİŞ ÇERÇEVE (design-review 2026-07-02, 3. tur, economy-designer + creative-director)**: 2. turda bu risk "kesif-alani.md verisi bekleniyor" denerek ertelenmişti; bu erteleme gerekçesi artık geçersiz. `ekonomi.md`'nin mevcut idle-altın formülüyle çapraz doğrulandığında: SS ★5 toplam yıldız maliyeti (27.000 altın, Formül 4) tam olarak **1 günlük orta-oyun idle geliri**ne eşit (aktif oyun gerektirmeden); bir SS Lv40★5 kopyasının satışı (6.650 altın) idle gelirin **~6-15 dakikasına** eşit. **Gerçek sorun S/SS'in nadiren satılması değil — mevcut idle-altın musluğu bu dosyanın tüm altın sink'lerinden (satış, yıldız yükseltme) çok daha büyük.** Formül 4'ün F→SS ölçeklenmesi (3.6×) Kural 7'nin satış-tablosu ölçeklenmesinden (35×) çok daha dar olduğundan, SS-tier yıldız yolu kopya-satışıyla kendi kendini finanse etmede F-tier'den ~16× daha kolay (bkz. Formül 4 sonrası not) — bu asimetri riski büyütüyor. **Düzeltme bu dosyanın kapsamında değil**: asıl dengesizlik `ekonomi.md`'nin idle-altın oranlarında — o dosya flag edildi (bkz. Dependencies → Çapraz bağımlılık notları). Bu dosyada mekanik değiştirilmedi, ama izleme notu artık soyut değil, somut sayılarla güncellendi.
- `evolution_material_costs` × Loot/Ödül'ün Evrim Taşı düşme oranı (üst tier'larda) birlikte bir tier'de kalma süresini belirler — level-deneyim-sistemi.md'nin XP eğrisiyle senkron olmalı (biri diğerini fazla bekletmemeli).
- `base_star_cost` × `tier_cost_multiplier` × max star (5) birlikte yıldız altın sink'inin toplam büyüklüğünü belirler (F: 7.500 – SS: 27.000 altın). Ekipman/Dükkan sink'leriyle birlikte toplam altın talebini oluşturur, aşırı toplam oyuncuyu "hiçbir şeye yetişemiyorum" hissine sokabilir.

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Yeni canavar keşfi (Pokédex) | Siluet açılma animasyonu — karanlık siluetten renkli ikona geçiş + ışık efekti | MVP |
| İlk sahiplik (yeni tür kazanımı) | Pokédex giriş açılma + "YENİ!" damgası + nadirlik renginde aura | MVP |
| İlk sahiplik — S/SS tier (YENİ, design-review 2026-07-02, 3. tur, game-designer bulgusu) | Yukarıdakine ek: tam ekran flaş + yavaşlatılmış kart döndürme + ekran-çapı ışık patlaması — diğer tüm tier'lerden görsel olarak ayırt edilir, "doruk an" fantezisini yalnızca prose'da değil sunumda da taşır | MVP |
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
| İlk sahiplik | Keşif fanfarı + nadirlik bazlı jingle (F=kısa, ... B=epik, **S/SS=tam orkestral "doruk an" stingeri, diğer tüm nadirliklerden belirgin şekilde ayrı** — YENİ, design-review 2026-07-02, 3. tur, game-designer bulgusu: önceki tabloda jingle skalası B'de duruyordu, S/SS'e özel bir beat tanımlı değildi) | MVP |
| Canavar satma | Metalik "clink" + yumuşak "swoosh" | MVP |
| Toplu satış | Hızlanan "clink-clink-clink" + toplam "ka-ching" | MVP |
| Milestone ödülü | Kutlama fanfarı (2-3 sn) + konfeti sesi | MVP |
| Envanter genişletme | Kilit açılma sesi + genişleme "whoosh" | MVP |
| Bekleme uyarısı | Yumuşak bildirim tonu | MVP |
| Canavar kilitleme | Kısa "klik" sesi | MVP |

> 📌 **Asset Spec** — Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:canavar-toplama-evrim` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- **Koleksiyon ana ekranı**: İki tab — "Envanter" (sahip olunan canavarlar) ve "Pokédex" (tüm türler). Üst barda: kapasite göstergesi (ör. 17/20), tamamlama yüzdesi (%85), sıralama/filtreleme butonları.
- **Envanter listesi**: Grid görünüm (4×5 ızgara). Her canavar kartı: portre, nadirlik çerçevesi rengi, seviye, yıldız göstergesi. Kilitli canavarlar kilit ikonu gösterir. Favori canavarlar kalp ikonu + üste sabitlenir.
- **Pokédex görünümü**: Grid tüm türleri gösterir. Keşfedilmemişler siyah siluet + "???". Keşfedilmişler gri ikon + isim. Sahip olunanlar tam renkli. Tamamlama barı üstte.
- **Canavar detay ekranı**: Envanterdeki canavarı tıklayınca açılır. Portre (büyük), isim, arketip, nadirlik. Statlar (HP/ATK/DEF/SPD) barlarla. Alt butonlar: "Takıma Ekle", "Güçlendir" (→ Güçlendirme ekranı), "Sat", "Kilitle/Aç", "Favori". Lore metni alt bölümde.
- **Güçlendirme ekranı (YENİ — design-review 2026-07-02, Canavar Güçlendirme birleşmesi)**: 2 tab — "Evrim" ve "Yıldız" (eski Seviye tab'ı kaldırıldı, seviye artık tamamen pasif/otomatik — bkz. `level-deneyim-sistemi.md`). Evrim tab'ı: mevcut tier → hedef tier görseli + malzeme gereksinim listesi (✅/❌) + "Evrimleştir" butonu (deterministik, başarı oranı göstergesi YOK — eski pity/şans mekaniği kaldırıldı). Yıldız tab'ı: 5 yıldız göstergesi + "Birleştir" butonu + kopya seçim listesi (takımdakiler/kilitliler gri) + altın maliyeti (Formül 4) + stat bonus preview.
- **Satış ekranı**: Tek satış: detay ekranından "Sat" → onay dialogu + satış fiyatı gösterilir. Toplu satış: envanter listesinde "Toplu Sat" modu → checkbox seçimi → toplam fiyat + "Sat" butonu.
- **Otomatik satış ayarları**: Ayarlar ekranında veya envanter ekranında dişli ikonu → filtre kuralları: tier dropdown (Kapalı/F/D) + yıldız dropdown (Kapalı/★0/★1 altı). "C/B tier asla satılmaz" güvenlik notu sabit gösterilir.
- **Envanter genişletme**: Envanter ekranında kapasite barı yanında "+" butonu → genişletme dialogu: mevcut kapasite, yeni kapasite, elmas maliyeti, "Satın Al" butonu.
- **Bekleme alanı**: Envanter ekranında sarı uyarı bandı: "[X] canavar beklemede — [Y] gün kaldı". Tıklayınca bekleme listesi açılır. Her canavar: portre + isim + kalan gün + "Envantere Al" (slot varsa) veya "Sat" butonu.
- **Milestone ödül ekranı**: Pokédex'te milestone barı üzerinde ödül ikonları. Alınmamış milestone'lar parlayarak dikkat çeker. Tıklayınca ödül içeriği popup'ta gösterilir.
- **Minimum dokunma hedefi**: 44×44 dp tüm butonlar

> 📌 **UX Flag — Canavar Toplama ve Evrim**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak koleksiyon ekranı, Pokédex, envanter yönetimi, satış akışı ve bekleme alanı için UX spec oluşturulmalı.

## Acceptance Criteria

> **Not (design-review 2026-07-02)**: Aşağıdaki AC'ler F-D-C-B-A-S-SS tier modeline ve tier-relative instance şemasına (Kural 2/5) hizalandı. Değişmeyenler olduğu gibi bırakıldı; değişenler **REVİZE** etiketiyle işaretlendi. 4 yeni AC (23-26) daha önce test kapsamı dışında kalan senaryoları kapatıyor (qa-lead bulgusu, 1. tur). 6 ek yeni AC (27-32) Canavar Güçlendirme birleşmesini ve 2. tur qa-lead bulgularını kapatıyor (2. tur).
>
> **Fixture düzeltmesi (design-review 2026-07-02, 2. tur — qa-lead bulgusu)**: `canavar-veritabani.md` Kural 1'in şema örneği `fire-striker-infernalclaw`'ı `base_rarity=C` olarak tanımlıyor (bu monster henüz kilitlenmiş bir MVP roster girişi değil, yalnızca şema örneğidir — bkz. `canavar-veritabani.md` Open Question #1). AC#1/#5/#6, önceki turda bu canonik örnekle çelişen bir F-tier varsayımı kullanıyordu (F→D geçişi bu monster için geçersiz bir zincirdi). Aşağıdaki AC'ler C-tier başlangıcıyla (C→B→A zinciri, `canavar-veritabani.md` Kural 4) tutarlı hale getirildi.

1. **[REVİZE — 2. tur, fixture düzeltmesi]** **GIVEN** Loot sistemi bir canavar düşürür, **WHEN** `OnMonsterDropped("fire-striker-infernalclaw", "C")` çağrılırsa, **THEN** yeni instance oluşturulur: tier="C", level=1, xp=0, banked_xp=0, lifetime_pet_level=0, star_rank=0 ve envantere eklenir; (fire-striker-infernalclaw, C) Pokédex girişi "Sahip Olundu" durumuna geçer.

2. **GIVEN** oyuncunun envanteri 20/20 dolu, **WHEN** yeni canavar düşerse, **THEN** canavar bekleme alanına eklenir ve "Envanter dolu!" bildirimi gösterilir.

3. **GIVEN** bekleme alanında 7 günden fazla bekleyen F tier canavar var, **WHEN** oyuncu giriş yaparsa, **THEN** canavar otomatik satılır (100 altın) ve bildirim gösterilir.

4. **[REVİZE]** **GIVEN** bekleme alanı 10/10 dolu (F-A tier) ve envanter 20/20 dolu, **WHEN** yeni **F-A tier** canavar düşerse, **THEN** canavar anında otomatik satılır ve loot raporunda not gösterilir. **(Bkz. AC#26 için S/SS tier istisnası.)**

5. **[REVİZE]** **GIVEN** oyuncu daha önce "fire-striker-infernalclaw" türüyle **C tier'da** hiç karşılaşmamış, **WHEN** bu türle C tier olarak savaşırsa (düşman olarak), **THEN** (fire-striker-infernalclaw, C) Pokédex girişi "keşfedildi" durumuna geçer (gri ikon, isim görünür) ve 5 elmas ödülü verilir. **Not**: Aynı türün farklı bir tier'ı (ör. F) daha önce keşfedilmiş olsa bile, her (tür,tier) çifti kendi keşif olayını bağımsız tetikler.

6. **[REVİZE — 2. tur, fixture düzeltmesi]** **GIVEN** oyuncu ilk kez "fire-striker-infernalclaw" türünü **B tier'a evrimleyerek** kazanır (önceden C tier'da sahipti, `canavar-veritabani.md` Kural 4'e göre C max=A, dolayısıyla C→B geçerli bir zincir adımı), **WHEN** Kural 10'daki evrim tamamlanırsa, **THEN** (fire-striker-infernalclaw, B) Pokédex girişi "sahip olundu" durumuna geçer (tam renkli) ve 10 elmas + 500 altın ödülü verilir — (fire-striker-infernalclaw, C) girişi de kalıcı olarak "sahip olundu" kalmaya devam eder (silinmez).

7. **[REVİZE — 2. tur, test edilebilirlik düzeltmesi; 3. tur, mock-koruma sıkılaştırıldı]** **GIVEN** bir test fixture'ı `MOCK_TOTAL_POKEDEX_ENTRIES__DO_NOT_SHIP = 62` (mock değer — gerçek roster kilitlenene kadar bu, `canavar-veritabani.md`'nin nihai dağılımıyla değiştirilecek bir yer tutucudur, bkz. Formül 3) ile enjekte edilmiş ve oyuncunun `unique_owned_entries`'i 15'ten 16'ya çıkarak `62 × 0.25 = 15.5` eşiğini ilk kez geçmiş, **WHEN** tamamlama kontrolü yapılırsa, **THEN** %25 milestone ödülü tetiklenir: 50 elmas + 5.000 altın. **Test harness notu (3. tur, qa-lead + systems-designer bulgusu)**: Bu bir unit test'tir — `total_pokedex_entries` mock/enjekte edilmeli, gerçek roster verisi beklenmemeli. Test sabiti **`MOCK_...__DO_NOT_SHIP` işaretleyicisiyle adlandırılmalıdır** (yalnızca "62" değil) — bu, sayının yanlışlıkla `MonsterDatabaseConfig` gibi bir prod dosyasına kopyalanmasını isim düzeyinde engeller. Mümkünse bu literal'in `tests/unit/` dışında görünmesini engelleyen bir CI/lint kuralı eklenmesi önerilir.

8. **[REVİZE — 2. tur, test edilebilirlik düzeltmesi; 3. tur, mock-koruma sıkılaştırıldı]** **GIVEN** aynı mock fixture (`MOCK_TOTAL_POKEDEX_ENTRIES__DO_NOT_SHIP = 62`) ve oyuncunun `unique_owned_entries`'i 62'ye ulaşmış (tüm mock girişlere sahip), **WHEN** tamamlama kontrolü yapılırsa, **THEN** %100 milestone ödülü: 500 elmas + 100.000 altın + "Canavar Lordu" başlığı. **Test harness notu**: Bu da unit test'tir, mock fixture gerektirir (bkz. AC#7'deki DO-NOT-SHIP adlandırma kuralı) — gerçek roster kilitlendiğinde bu iki AC gerçek `total_pokedex_entries` değeriyle bir entegrasyon testiyle tekrarlanmalıdır (bkz. Open Questions #2/#6).

9. **[REVİZE]** **GIVEN** F tier Lv10 (tier cap) ★0 canavar, **WHEN** satılırsa, **THEN** satış fiyatı = floor(100 × (1 + 10×0.01 + 0)) = **110 altın**.

10. **[REVİZE]** **GIVEN** C tier Lv20 (tier cap) ★2 canavar, **WHEN** satılırsa, **THEN** satış fiyatı = floor(400 × (1 + 20×0.01 + 2×0.10)) = **560 altın**.

11. **GIVEN** envanterde 1 canavar kalmış, **WHEN** oyuncu satmaya çalışırsa, **THEN** işlem engellenir ve "Son canavar satılamaz" mesajı gösterilir.

> **Non-AC / tolere edilen davranış notu (YENİ — design-review 2026-07-02, 3. tur, qa-lead bulgusu)**: Formül 2'nin "Bilinen sınırlama" notunda belgelenen tier-sınır ihlali (ör. B Lv25★5=1.225 altın > A Lv1★0=1.212 altın satış fiyatı üretmesi) **beklenen davranıştır — QA tarafından bug olarak açılmamalıdır**. Bu, bu revizyonda bilinçli olarak düzeltilmedi (kullanıcı kararı, bkz. Formül 2), gelecekteki bir balance-tuning oturumunu bekliyor.

12. **GIVEN** kilitli canavar, **WHEN** oyuncu satmaya veya yıldız birleştirme malzemesi yapmaya çalışırsa, **THEN** işlem engellenir.

13. **GIVEN** otomatik satış filtresi "F" ayarlı, **WHEN** ilk kez keşfedilen F tier canavar düşerse, **THEN** otomatik satılmaz — koleksiyona eklenir.

14. **GIVEN** otomatik satış filtresi "F" ayarlı, **WHEN** daha önce keşfedilmiş F tier canavar düşerse (kopya), **THEN** otomatik satılır ve loot raporunda "otomatik satıldı: +100 altın" gösterilir.

15. **GIVEN** 3. envanter genişletme satın alınacak, **WHEN** oyuncu genişletme butonuna basarsa, **THEN** maliyet = 50 × 2² = **200 elmas**, kapasite 30→35 olur.

16. **[REVİZE — 2. tur, kapsam netleştirmesi; 3. tur, qa-lead bulgusu ile kapsam daha da netleştirildi]** **GIVEN** oyuncu envanter 19/20, bekleme alanında 2 canavar var, **WHEN** envanterde 1 slot açılırsa (satış, yıldız birleştirme malzemesi olarak tüketim (Kural 11) veya başka bir yolla — kaynak farketmez), **THEN** bekleme alanından en eski canavar otomatik envantere taşınır (FIFO). **Kapsam notu (3. tur)**: Bu FIFO yalnızca 10 slotluk F-A bekleme alanını kapsar — kalıcı S/SS bekleme alt-alanı (Kural 4.3) bu otomatik taşımaya HİÇBİR ZAMAN dahil değildir, S/SS yalnızca oyuncunun manuel işlemiyle envantere taşınır (bkz. Kural 4.7, AC#32/#33).

17. **GIVEN** favori canavar, otomatik satış filtresi "F" ayarlı, **WHEN** F tier favori canavar düşerse, **THEN** otomatik satılmaz — favori muafiyeti uygulanır.

18. **[REVİZE — 2. tur, sıra netleştirmesi; 3. tur, qa-lead bulgusuyla test edilebilir hale getirildi]** **GIVEN** oyuncu %50 milestone almamış ama %75'e ulaşmış, **WHEN** milestone kontrolü yapılırsa, **THEN** hem %50 hem %75 ödülleri verilir VE ödül-verme event log'unda %50 event'i %75 event'inden önce timestamp'lenir (artan sırayla verildiğinin test edilebilir kanıtı — iki ödülün de ekonomiye ulaştığını doğrulamak tek başına yeterli değildir, sıra event log ile doğrulanır).

19. **GIVEN** oyuncu 8. genişletmeyi tamamlamış (60/60 kapasite), **WHEN** genişletme butonuna basarsa, **THEN** buton devre dışıdır ve "Maksimum kapasiteye ulaştınız" mesajı gösterilir.

20. **GIVEN** bekleme alanında C tier canavar 13 gündür bekliyor, **WHEN** oyuncu giriş yaparsa, **THEN** canavar henüz satılmaz (14 gün süresi dolmamış) ve push notification gönderilir: "Nadir canavarınız beklemede — 1 gün kaldı!"

21. **[REVİZE — sınır netleştirildi]** **GIVEN** bekleme alanında F tier canavar tam **7 gündür** (`elapsed_days >= 7`) ve C tier canavar 5 gündür bekliyor, **WHEN** oyuncu giriş yaparsa, **THEN** F tier canavar otomatik satılır (`elapsed_days >= expiry_days` eşitliği de satışı tetikler — Kural 3'teki level-atlama sınır kuralıyla tutarlı), C tier canavar beklemede kalır (14 gün süresi dolmamış).

22. **[REVİZE — S/SS artık süresiz]** **GIVEN** bekleme alanında S tier canavar 90+ gündür bekliyor, **WHEN** oyuncu giriş yaparsa, **THEN** canavar **hiçbir zaman** otomatik satılmaz (Kural 4.3, S/SS istisnası) — bir kerelik "Efsanevi canavarınız kalıcı beklemede" bildirimi zaten gönderilmiş olduğundan tekrar gönderilmez, canavar bekleme alanında kalmaya devam eder.

23. **[YENİ]** **GIVEN** aynı savaş turunda iki ayrı loot rulosu aynı türde canavar düşürür (ör. normal roll + boss roll), **WHEN** her iki `OnMonsterDropped` sinyali işlenirse, **THEN** iki bağımsız `instance_id` oluşturulur, envanterden 2 slot kullanılır, her ikisi de Pokédex'te aynı (tür,tier) girişine "sahip olundu" katkısı yapar (giriş yalnızca bir kez işaretlenir, ikinci instance yalnızca envanter/koleksiyon sayısını artırır).

24. **[YENİ]** **GIVEN** oyuncunun elması, seçilen envanter genişletmesinin maliyetinden az, **WHEN** oyuncu "Satın Al" butonuna basmaya çalışırsa, **THEN** buton devre dışıdır, mevcut elmas ve gereken elmas gösterilir, işlem gerçekleşmez.

25. **[YENİ]** **GIVEN** kilitli bir canavar, **WHEN** oyuncu toplu satış modunda seçim listesini açarsa, **THEN** kilitli canavarın checkbox'ı devre dışıdır ve kilit ikonu gösterilir — seçime dahil edilemez.

26. **[YENİ]** **GIVEN** envanter 20/20 dolu ve bekleme alanı (F-A, 10 slot) da dolu, **WHEN** yeni bir **S veya SS tier** canavar düşerse, **THEN** canavar otomatik satılmaz — ayrı "kalıcı bekleme" alt-alanına eklenir (10 slotluk kapasiteyi etkilemez), oyuncuya "Efsanevi canavarınız kalıcı beklemede" bildirimi gösterilir.

27. **[YENİ — 2. tur, qa-lead bulgusu]** **GIVEN** bir kopya canavar bekleme alanındayken (envanter dolu olduğu için beklemede), **WHEN** oyuncu bu instance'ı ana canavarına yıldız birleştirme malzemesi olarak seçerse, **THEN** instance bekleme alanından kaldırılır ve tüketilir (`ConsumeInstance`), ana canavarın `star_rank`'ı +1 artar — envanterde slot açılması gerekmez, bekleme-alanı-özel bir işlem akışı yoktur.

28. **[REVİZE — 2026-07-02, element sistemi kaldırıldı, Kural 10 malzeme modeli güncellendi]** **GIVEN** F tier canavar Lv10'da (tier cap, `IsEvolutionEligible=true`) ve oyuncunun envanterinde tier'i F olan 4 canavar (herhangi tür) + yeterli altını var, **WHEN** oyuncu "Evrimleştir" butonuna basarsa, **THEN** 4 canavar `ConsumeInstance()` ile tüketilir, altın düşer, canavar D tier'a geçer, `level=1`, `xp=0`, `lifetime_pet_level` korunur (artmaya devam eder, sıfırlanmaz), (tür, D) Pokédex girişi "Sahip Olundu" olur.

29. **[REVİZE — 2026-07-02, element sistemi kaldırıldı, Kural 10 malzeme modeli güncellendi]** **GIVEN** F tier canavar Lv10'da ve oyuncunun envanterinde yalnızca 3 tane F-tier canavar var (4 gerekli), **WHEN** evrim ekranı açılırsa, **THEN** "Evrimleştir" butonu devre dışıdır, "Tier F canavar ×4 gerekli, elinizde 3" gösterilir.

30. **[YENİ — 2. tur, Kural 11 birleşmesi]** **GIVEN** C tier canavar ★0, oyuncunun envanterinde aynı türden 1 kopyası daha var ve 2.700 altını var, **WHEN** oyuncu kopyayı ★1 birleştirmeye seçip onaylarsa, **THEN** `star_gold_cost = 500 × 1 × 1.8 = 900` altın harcanır, kopya tüketilir, ana canavar `star_rank=1` olur, statlar `× 1.05` bonusuyla yeniden hesaplanır.

31. **[YENİ — 2. tur, Kural 11 birleşmesi]** **GIVEN** bir canavar zaten ★5 (max), **WHEN** oyuncu aynı türden yeni bir kopya kazanırsa, **THEN** kopya normal bir instance olarak koleksiyona eklenir (kendi `instance_id`'si, kendi envanter slotu), birleştirme seçim listesinde bu ana canavar için "MAX" gösterilir ve kopya birleştirme hedefi olarak seçilemez.

32. **[YENİ — 2. tur, Kural 4.7 orta-yol kısıtı]** **GIVEN** kalıcı beklemede bir SS tier canavar (envanter dolu olduğu için hiç taşınmamış), **WHEN** oyuncu bu instance'ı yıldız birleştirmede ana canavar olarak veya takıma eklemek için seçmeye çalışırsa, **THEN** işlem engellenir, "Önce envantere alın" uyarısı gösterilir.

33. **[YENİ — 3. tur, qa-lead bulgusu — AC#32'nin eksik yarısını kapatır]** **GIVEN** AC#32'deki gibi engellenmiş bir SS instance'ı, **WHEN** oyuncu envanterde slot açıp bu instance'ı manuel olarak envantere taşırsa, **THEN** Kural 4.7'nin kısıtı kalkar — evrim (Kural 10), yıldız birleştirme (Kural 11) ve takıma ekleme işlemlerinin hepsi artık "Önce envantere alın" engeli olmadan başarıyla tamamlanır.

*`qa-lead` konsültasyonu: `/design-review` 2026-07-02, tam mod (1., 2. ve 3. tur).*

## Open Questions

1. **Başlangıç canavarı**: MVP'de oyuncu ilk canavarını nasıl alır? Tutorial/Onboarding (Tier 2) olmadan ilk zindan girişinde garanti F tier canavar düşürülebilir. → Tutorial GDD'sinde netleşecek.

2. **[REVİZE — design-review 2026-07-02, 2. tur] Koleksiyon milestone/keşif ödül dengeleme**: Pokédex elmas bütçesi artık `total_pokedex_entries`'in (Formül 3) türetilmiş, roster-bağımlı değerine dayanıyor — sabit bir rakam vermek mümkün değil. BLOCKER (güncellenmiş kapsam): (a) `canavar-veritabani.md`'nin 15-20 türün başlangıç-kademe (F/D/C/B/A/S) dağılımını netleştirmesi gerekiyor ki `total_pokedex_entries` hesaplanabilsin — **bu sayı belirlenene kadar Pokédex ödül ekonomisi implementasyona alınmamalı** (bkz. Tuning Knobs → Diamond Budget Balance); (b) Ekonomi GDD'nin MVP-kapsamındaki tekrarlı kaynakları çapraz doğrulandı ve **düzeltilmiş musluk rakamı yazıldı**: ~263 elmas/ay (arena hariç, Tier 2), 12.750 elmas'lık genişletme sink'ine ulaşmak **~48.5 ay** sürer (önceki geçersiz "~42 ay" rakamının yerine, bkz. Tuning Knobs). Pokédex ödülleri bu 48.5 aylık hesaba dahil değildir (roster bağımlılığı nedeniyle). → `canavar-veritabani.md` roster revizyonu tamamlandığında Formül 3 + Tuning Knobs'taki illüstratif rakamlar gerçek değerle güncellenmeli. **(c) YENİ (3. tur)**: Bu bağımlılık artık ikili — roster dağılımının netleşmesi TEK BAŞINA yeterli değil, `canavar-veritabani.md` Kural 6'nın form-gate çelişkisi de çözülmeli (bkz. Open Questions #6, bu turda onaylı blocker'a yükseltildi), aksi halde Kural 5'in giriş-sayısı tablosu (dolayısıyla `total_pokedex_entries`) yanlış temelden hesaplanmış olabilir.

3. ~~**Evrim formu ve Pokédex** — eski "Form A/B/C" çözümü~~ **SÜPERSEDE EDİLDİ (design-review 2026-07-02)**: Bu maddenin önceki çözümü (20 tür × 3 form = 60, `total_species_count=60`) F-D-C-B-A-S-SS tier pivotundan ÖNCEki bir modele aitti ve OQ#6'nın işaret ettiği çelişkinin asıl kaynağıydı. **Yeni çözüm**: Pokédex artık (tür, ulaşılan tier) çiftlerini ayrı giriş sayıyor — bkz. Kural 5 "Tier Zinciri Girişleri" ve Formül 3. Giriş sayısı türden türe değişir (başlangıç kademesine göre 1-4 arası), sabit değildir. Bu madde artık kapalı; kesin sayısal toplam yalnızca OQ#2'nin roster-bağımlılığına kalıyor.

4. **Otomatik satış geri alma**: Otomatik satılan canavar geri alınabilir mi? (Geri alma penceresi 30 sn gibi) → MVP'de yok, Tier 2+'da değerlendirilebilir.

5. **Envanter sıralama seçenekleri**: Nadirlik, seviye, güç, kazanım tarihi — hangi sıralama seçenekleri MVP'de olmalı? RECOMMENDED (Systems Design Review): Minimum = nadirlik, seviye. → UI GDD'sinde detaylandırılacak.

6. **[KISMEN ÇÖZÜLDÜ, KALAN KISIM ONAYLI BLOCKER'A YÜKSELTİLDİ — design-review 2026-07-02, 3. tur (re-review)] F-D-C-B-A-S-SS tier revizyonu**: Bu dosyanın KENDİ modeli artık `level-deneyim-sistemi.md` ile hizalı (bkz. Kural 2/5, Formül 3 — eski Form A/B/C + nadirlik modeli kaldırıldı). **`canavar-guclendirme.md` kolu ÇÖZÜLDÜ (kullanıcı kararı, 2. tur)**: O sistem bu dosyaya birleştirildi — Evrim Yürütme Kural 10'da (deterministik, pity/başarı-oranı yok), Yıldız Sistemi Kural 11'de (F-D-C-B-A-S-SS nadirlik çarpanlarıyla) tanımlı. `canavar-guclendirme.md` artık Deprecated işaretli.

   **Kalan madde — 3. tur re-review'da "kırıntı"dan ONAYLI BLOCKER'a yükseltildi (game-designer + systems-designer + creative-director yakınsaması)**: `canavar-veritabani.md` Kural 6, tier-içi "Form 1-3" evrim aşamasını hâlâ CANLI olarak tanımlıyor (`evolved_pool = floor(base_pool × 1.40^(form-1))`, max form'a ulaşınca tier-up açılıyor) — bu hem `level-deneyim-sistemi.md`'nin "sadece seviye tavanı = tek gate" kararıyla HEM DE bu dosyanın Kural 10'undaki deterministik-evrim modeliyle çelişiyor. Bu artık salt bir "form kavramı kaldırılmalı mı" sorusu değil — **iki somut, kanıtlanmış sonucu var**:
   1. `canavar-veritabani.md`'nin Interactions tablosu, Kural 6'nın `GetGrowthRates()`/`GetEvolutionTarget()` arayüzlerini hâlâ Deprecated `canavar-guclendirme.md`'ye bağlı gösteriyor — yani bu dosyanın Kural 10.5'in dayandığı `GetMonsterDefinition()` "tier-başına base stat havuzu" arayüzü, upstream'de **henüz dokümante edilmiş şekliyle mevcut değil**.
   2. Eğer Kural 6'nın Form 1-3 gate'i canlı kalırsa, Kural 5'in giriş-sayısı tablosu (F=3, B=4, vb.) tier başına **2-3× eksik sayım** yapıyor olabilir — bu, Formül 3'ün `total_pokedex_entries` hesabını sayısal bir bekleme değil, yapısal bir geçersizlik riski haline getirir.

   **Bu GDD'nin bu turdaki aksiyonu**: Kural 5'in giriş-sayısı tablosu ve Formül 3, aşağıda **"provisional — canavar-veritabani.md revizyonuna bağlı"** notuyla işaretlendi (bkz. Kural 5 ve Formül 3). Bu dosyanın kendi kapsamı bu nedenle bloklanmıyor (mantık, arayüz sözleşmesi tutarlı tanımlı) — ama implementasyon, `canavar-veritabani.md` Kural 6 çözülmeden bu iki bölümün nihai sayılarını temel alamaz.
   → Sıradaki adım: `/design-review canavar-veritabani.md` (artık onaylı blocker, ertelenemez).

7. **[YENİ — design-review 2026-07-02] Keşif Alanı Sistemi arayüz boşluğu**: `kesif-alani.md` (MVP ana canavar kazanım kaynağı, bkz. Kural 1) kendi Downstream tablosunda Canavar Toplama'yı listelemiyor ve Pokédex-keşif sinyalini (`OnEnemyEncountered(monsterId, tier)`) tanımlamıyor. → `kesif-alani.md`'nin kendi revizyon oturumunda eklenmeli.

8. **[YENİ — design-review 2026-07-02] Loot sinyali arayüz güncellemesi**: `loot-odul-sistemi.md`'nin `OnMonsterDropped(monsterId)` imzası, bu GDD'nin artık gerektirdiği `tier` parametresini içermiyor (bkz. Kural 2, Dependencies). → `loot-odul-sistemi.md`'nin kendi revizyon oturumunda arayüz güncellenmeli.
