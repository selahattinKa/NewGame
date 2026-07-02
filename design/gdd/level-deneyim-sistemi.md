# Level / Deneyim Sistemi (Level / Experience System)

> **Status**: Revised — Implementation Blocked (2026-07-01, 3 tur `/design-review`. 1. tur: NEEDS REVISION → iksir→altın açığı, kayıt şeması, registry senkronu, 3 invariant pseudocode, AC #12 bölünmesi, Toplam Seviye göstergesi çözüldü. 2. tur yeniden inceleme: 6 blocker doğrulandı AMA pseudocode düzeltmesi kendi içinde YENİ bir blocker (`banked_progress` tanımsız terimi) doğurdu + save/load atomikliğinin AC'si eksik bulundu. 3. tur: bu 2 yeni blocker + 5 önerilen madde çözüldü. Kalan 3 blocker hâlâ başka dosyaların revizyonuna bağlı — bkz. Open Questions #1, #6, #7)
> **Author**: user + game-designer, systems-designer, economy-designer, qa-lead, creative-director
> **Last Updated**: 2026-07-01
> **Implements Pillar**: Güç Hisset
> **Creative Director Review (CD-GDD-ALIGN)**: Consulted for `/design-review` Phase 3b synthesis (2026-07-01) — see `design/gdd/reviews/level-deneyim-sistemi-review-log.md`

## Overview

Level / Deneyim Sistemi, savaş sonunda kazanılan EXP değerini oyuncu karakteri ve aktif pet için kalıcı bir güç artışına dönüştüren temel ilerleme katmanıdır. Şu anda `CombatManager.GenerateReward()` savaş sonunda 200-280 arası rastgele bir EXP sayısı üretiyor ve `BattleRewardScreen` bunu animasyonla gösteriyor, ama bu değer hiçbir kalıcı state'e yazılmıyor — bu sistem tam olarak bu boşluğu dolduruyor. Oyuncu ve pet, aynı savaş sonu ödül ekranından beslenen ama tamamen bağımsız iki XP/level sayacına sahiptir: oyuncu karakteri sınıfından bağımsız kendi seviyesinde büyür (mevcut `StatAtLevel` formülünü artık gerçek, biriken bir level değerine bağlar), pet ise kendi tier'ı (F-D-C-B-A-S-SS) içinde seviye atlar ve bir tier'ın level tavanına ulaşmak bir sonraki tier'a evrimleşmenin ön şartlarından biri haline gelir. Oyuncu açısından sistem tamamen otomatik ve pasiftir — her savaş sonunda görünür bir EXP bar dolar, level atlandığında belirgin bir "LEVEL UP!" anı yaşanır ve karakterin/petin sayıları büyür; oyuncunun aktif bir kararı gerekmez, sadece sonucu izler ve güçlendiğini hisseder.

## Player Fantasy

Oyuncu bu sistemde "Ben büyüyorum" fantazisini yaşar — her savaş sonu EXP barının dolmasını izlemek, çubuk taşıp "LEVEL UP!" anına ulaştığında beklenti-ödül döngüsünü tetikler. Bu, oyunun "Güç Hisset" sütununun en somut kanıtı: level arttıkça HP/ATK/DEF/SPD sayıları büyür, aynı düşmanlar daha kolay düşer, oyuncu geri dönüp baktığında "birkaç kat önce zor gelen düşman şimdi tek vuruşta gidiyor" hissini yaşar.

Pet tarafında bu fantezi ikiye katlanır: pet'in level tavanına yaklaştığını görmek ("2 level kaldı, sonra evrimleşebilir") Pet Evrim Sistemi'nin "bir kat daha" psikolojisiyle birleşir — level barı doldukça evrim anına biraz daha yaklaşıldığı hissi oluşur.

Negatif fantezi (kaçınılacak): Level atlamanın fark edilmemesi — sessiz, görsel/işitsel geri bildirimi olmayan bir sayı artışı "Güç Hisset" sütununu zedeler. Level barının çok yavaş dolması da grind hissi yaratır ("Cömert Zindan"a aykırı).

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — İki Bağımsız Sayaç**
Oyuncu karakteri (`player_level`, `player_xp`) ve aktif pet instance'ı (`pet_level`, `pet_xp` — instance'a ait, kaydedilir) tamamen ayrı sayaçlardır. Sadece aktif pet XP kazanır; envanterdeki diğer petler kazanmaz.

**Kural 2 — XP Kaynağı**
Oyuncu karakteri **sadece savaş zaferinden** XP kazanır (`CombatManager.GenerateReward()` sadece victory'de ürettiği `ExpGained` değeri tam olarak eklenir). Aktif pet ise iki kaynaktan XP kazanır: (a) savaş zaferi — aynı `ExpGained` değeri pete de tam olarak eklenir (paylaşılmaz); (b) **XP İksirleri** — Keşif Alanı ve zindan loot'undan düşen tüketilebilir item'lar (Mini/Small/Medium/Large/Giant — bkz. `loot-odul-sistemi.md`), oyuncu tarafından manuel olarak **sadece aktif pete** uygulanır, anında XP ekler. XP İksirleri **hiçbir koşulda gerçek para (IAP) ile satın alınamaz** — yalnızca loot veya rewarded ad ödülü üzerinden kazanılır. **Kısıt notu (design-review 2026-07-01)**: Bu sınır `magaza-ui.md`'yi de bağlar — Tier 2 IAP entegrasyonu (henüz tanımlanmadı) elmas→iksir dönüşümü gibi bir bypass İÇERMEMELİDİR; `magaza-ui.md`'nin ileride yapılacak Tier 2 revizyonu bu kısıtı açıkça kontrol etmelidir.

**Hedef kısıtı (design-review 2026-07-01, blocker düzeltmesi)**: Aktif pet **SS tier Lv40'ta** (mutlak son tavan — bir daha evrimleşemez) ise, XP İksiri kullanım ekranında geçerli bir hedef olarak **listelenmez/seçilemez**. Gerekçe: bu pet için kazanılacak tek şey Formül 2'nin altına çevirme mekanizmasıdır (bkz. Kural 5, Formül 2) ve bunun potion'la (tek seferde çok büyük XP değerleriyle) beslenmesi orantısız bir altın enjeksiyonuna yol açar (design-review 2026-07-01 bulgusu — bkz. AC #27). SS Lv40 dışındaki tüm tavan durumlarında (F/D/C/B/A/S tier tavanı, evrim materyali bekleniyor) potion hedef olarak seçilebilir ve normal şekilde bankalanır (gold dönüşümü YOKTUR — sadece savaş XP'si SS Lv40 ve oyuncu Lv30'da altına çevrilir, bkz. Formül 2).

**Kural 3 — Level Atlama**
Biriken XP, o level'ın eşiğine **ulaşınca veya geçince** (`birikmiş_xp >= XP_required(level)`) level atlanır — tam eşitlik (ör. Lv1'de tam 200 XP kazanmak) da level atlatır, sadece eşiğin üzerine çıkmak şart değildir (design-review 2026-07-01, systems-designer'ın sınır belirsizliği bulgusu düzeltildi). Taşan XP bir sonraki level'a devreder (carry-over, kaybolmaz). Tek bir savaş ödülü birden fazla level atlatabilir (ör. büyük XP iksiri).

**Kayıt/yükleme bütünlüğü (design-review 2026-07-01, blocker düzeltmesi — qa-lead'in "multi-level-up sırasında save/quit" bulgusu)**: Tek bir XP grant'ının tetiklediği ardışık level-atlama zinciri (yukarıdaki Algoritma bölümüne bakınız) **atomik**tir — zincir, herhangi bir kayıt noktası (autosave, uygulama arka plana alma) gerçekleşmeden ÖNCE, tek bir senkron işlem içinde tamamen belleğe uygulanır. Bir kayıt asla zincirin ortasında (ör. Lv6 uygulanmış, Lv7 uygulanmamış) yakalanamaz; disk'e yazılan `level/xp/banked_xp` değerleri her zaman zincirin son (tamamlanmış) durumunu yansıtır.

**Kural 4 — Level Atlamanın Sonucu**
Level arttığında ilgili `StatAtLevel(baseStat, level)` çağrısı yeni level ile yeniden hesaplanır. ATK/DEF/SPD doğrudan bu yeni değere ayarlanır. HP ise orantılı ölçeklenir: `new_current_hp = floor((current_hp / old_max_hp) × new_max_hp)`. Örnek: %80 dolulukla (HP=80/MaxHP=100) level atlayan bir birimin yeni MaxHP'si 120 ise, yeni current HP = floor(0.80 × 120) = 96 olur.

**Netleştirme (design-review 2026-07-01)**: Bu formül "asla tam heal vermez" anlamına gelmez — HP oranını korur. Eğer birim level atlamadan önce HP'si tamdıysa (current_hp = old_max_hp, oran=1.0), sonuç matematiksel olarak tam heal ile aynıdır: `new_current_hp = floor(1.0 × new_max_hp) = new_max_hp`. Bu beklenen ve doğru davranıştır — kural sadece dolu OLMAYAN HP'nin level atlamayla tamamlanmadığını garanti eder, her zaman kısmi kalacağını değil.

**Kural 5 — Pet Level Tavanı (Tier Başına)**

| Tier | F | D | C | B | A | S | SS |
|------|---|---|---|---|---|---|----|
| Level Tavanı | 10 | 15 | 20 | 25 | 30 | 35 | 40 |

Pet, kendi tier'ının tavanına ulaşınca "Evrime Hazır" bayrağı alır (Pet Evrim Sistemi bu bayrağı + evrim materyalini birlikte kontrol eder). Tavana ulaşıldıktan sonra fazla XP kazanılırsa **biriktirilir, kaybolmaz** (evrimden sonra otomatik kullanılır).

**Karar (design-review 2026-07-01)**: Pet'in bir üst tier'a geçmesi için TEK geçerli şart, kendi tier'ının seviye tavanına ulaşmaktır. `canavar-veritabani.md` Kural 6'daki ayrı "tier içi form gelişimi" (Form 1→2→3, evrim malzemesiyle ilerleyen ayrı bir gate) bu modelle çelişir ve bu GDD'nin kapsamı dışındadır — `canavar-veritabani.md`'nin kendi revizyon oturumunda bu form-aşaması gate'i kaldırılmalı veya tier atlamasının (bu GDD'nin sağladığı sinyal) doğrudan bir sonucu olacak şekilde yeniden yazılmalıdır (bkz. Open Questions #6).

**Uygulama notu (tavan taşması sabitlemesi)**: `XP_required(L)` yalnızca `L < tier_cap` için tanımlıdır (Formüller bölümü). Level-atlama döngüsü, `level == tier_cap`'e ulaştığı anda **kesin olarak durmalıdır** — bu noktadan sonra gelen TÜM XP (savaş veya iksir kaynaklı, miktarı fark etmeksizin) bankalanır (yukarıdaki paragraf), döngü asla `XP_required(tier_cap)` veya üstünü hesaplamaya çalışmamalıdır. Bir pet, evrim gerçekleşmeden `pet_level > tier_cap` durumuna asla geçemez — bu, implementasyon için sert bir invariant'tır (bkz. AC #24).

**Algoritma (design-review 2026-07-01, blocker düzeltmesi — systems-designer'ın "prose'da kalan hard-stop, off-by-one riski" bulgusu)**: Guard, XP tüketilmeden ÖNCE kontrol edilir — `XP_required(tier_cap)` asla çağrılmaz:

```
function ApplyXP(pet, incoming_xp):
    remaining = incoming_xp
    while remaining > 0:
        if pet.level >= pet.tier_cap:
            pet.banked_xp += remaining      # Kural 5 — bankala, ASLA XP_required(tier_cap) çağrılmaz
            remaining = 0
            break
        threshold = XP_required(pet.level)  # yalnızca level < tier_cap iken çağrılır
        needed = threshold - pet.xp
        if remaining >= needed:
            pet.level += 1
            pet.xp = 0
            pet.lifetime_pet_level += 1     # Kural 6 — evrimde ASLA sıfırlanmaz
            remaining -= needed
        else:
            pet.xp += remaining
            remaining = 0
    # döngü sonunda: pet.level asla tier_cap'i aşmaz (invariant, AC #24)
```

Bu fonksiyon, tek bir XP grant'ının (savaş ödülü veya iksir) tetiklediği TÜM ardışık level-atlamalarını senkron ve bölünmez şekilde işler — bkz. Kural 3'ün "kayıt/yükleme bütünlüğü" notu (aşağıda).

**Kural 6 — Evrim Sonrası Sıfırlama**
Pet bir üst tier'a evrimleştiğinde `pet_level=1`, `pet_xp=0` olur (+ Kural 5'teki biriken fazla XP otomatik uygulanır). Taban stat zaten tier atlamasıyla yükselir (mevcut tier stat tablosu), level 1'den yeniden büyüme başlar.

**Toplam Seviye / kalıcı sayaç (design-review 2026-07-01, game-designer'ın "evrim düşüş hissi" bulgusu — recommended, bu revizyonda dahil edildi)**: `pet_level`'ın evrimde 1'e sıfırlanması yalnızca tier-içi sayaçtır. Bunun yanında `lifetime_pet_level` adında kalıcı bir sayaç tutulur — pet instance'ın yaşam boyu kazandığı HER level atlamada (tier fark etmeksizin, evrim öncesi/sonrası dahil) +1 artar ve **evrimde asla sıfırlanmaz**. Bu sayaç Koleksiyon/Envanter UI'da ve LEVEL UP/evrim anında "Toplam Seviye: N" olarak gösterilir (bkz. UI Requirements) — amacı, evrimin görünürdeki `pet_level=1` sıfırlamasının bir "güç kaybı" gibi hissettirmesini önlemek; oyuncu her zaman monoton artan tek bir sayı görür. `lifetime_pet_level` persist edilen bir alandır (bkz. Dependencies).

**Kural 7 — Oyuncu Level Tavanı**
Oyuncu karakteri max Level 30'a ulaşır (`oyuncu-sinif-sistemi.md`'deki açık soruyu kapatır). Tavandaysa fazla XP kazanılırsa Formül 2'deki (`xp_overflow_gold_formula`) sink'e yönlendirilir (altına çevrilir).

**Kural 8 — XP İksiri Aşırı Doldurma Koruması (REVIZE — design-review 2026-07-01)**

Pet evrimleştiğinde level'ı 1'e sıfırlandığı için (Kural 6), XP İksirleri oyuncuya evrim sonrası hızlı toparlanma imkanı sağlar. Ancak sabit XP değerli iksirler (`loot-odul-sistemi.md`: Mini=25, Small=100, Medium=500, Large=2000, Giant=10000) tier'lar arasında orantısız güce sahiptir — aynı Giant iksir, F tier'ın tamamına yakınını (kümülatif ~6.628 XP'nin ~%151'i) tek başına bitirebilirken, SS tier'ında (~144.258 XP) bunun sadece ~%7'sini karşılar. Sabit level-eşiği kilidi (eski Kural 8) bu tier-ölçekli dengesizliği çözmez, sadece tier-içi erken kullanımı engeller.

**Yeni mekanik — Tek Uygulamada Yüzdesel Sınır**:

`remaining_tier_xp` = hedef pet'in mevcut biriken XP'si dahil, kendi tier tavanına (cap) ulaşması için gereken toplam kalan XP (Formül 1'in kümülatif tablosundan hesaplanır).

`max_applicable_xp = floor(remaining_tier_xp × overfill_cap_ratio)`, `overfill_cap_ratio = 0.5` (tuning knob).

- Eğer iksirin XP değeri ≤ `max_applicable_xp` ise: iksir normal şekilde uygulanır, onay istenmez (mevcut akış, Kural 3).
- Eğer iksirin XP değeri > `max_applicable_xp` ise: Onay ekranı açılır — "Bu iksir [Pet Adı]'nın mevcut tier'ı için çok güçlü — yalnızca [max_applicable_xp] XP uygulanacak, kalan [iksir_xp − max_applicable_xp] XP kaybolacak. Devam edilsin mi?" Oyuncu onaylarsa: iksir tüketilir, yalnızca `max_applicable_xp` kadarı uygulanır (Kural 3 sıralı level-atlama mantığıyla), kalan kısım **kaybolur** (bankalanmaz — Kural 5'teki tavan-bankalama mekanizmasından farklıdır, çünkü pet burada henüz tavanda değildir). Oyuncu onaylamazsa: iksir tüketilmez, işlem iptal edilir.
- **İstisna (design-review 2026-07-01, 3. tur — systems-designer'ın "remaining_tier_xp çok küçükken dejenere dialog" bulgusu)**: Eğer `max_applicable_xp == 0` ise (pet zaten doğal olarak tavana çok yakın), yukarıdaki onay ekranı hiç GÖSTERİLMEZ — bu iksir, o hedef pet için UI'da baştan seçilebilir bir hedef olarak sunulmaz (gri/devre dışı, bkz. UI Requirements). "0 XP uygulanacak, kaybolacak" gibi anlamsız bir onay asla oyuncuya gösterilmez.

Bu kural **tüm iksir boyutlarına eşit şekilde uygulanır** (Mini/Small/Medium/Large/Giant) — artık boyuta göre ayrı bir kilit/muafiyet yoktur. Küçük iksirler (Mini/Small) düşük XP taşıdığından bu sınırı pratikte nadiren aşar; bu yüzden eski "Mini/Small her zaman kullanılabilir" istisnasına ayrıca gerek kalmamıştır, ama kural artık hepsine tutarlı şekilde uygulanır.

`max_applicable_xp` **her zaman** `remaining_tier_xp`'nin altında kalır (ratio ≤ 1.0 olduğu sürece), bu nedenle bu kısıtlama tek başına bir peti tavana ulaştıramaz — tavana ulaşma her zaman Kural 5'in bankalama mantığına düşer, üst üste birkaç uygulama veya savaş XP'siyle birlikte olur.

**Öncelik sırası — Kural 8 vs Kural 5 (design-review 2026-07-01, blocker düzeltmesi — systems-designer'ın "Formül 8 tek başına okununca Kural 5'le çelişiyor" bulgusu)**: Kural 8'in kontrolü, `remaining_tier_xp > 0` OLDUĞU zaman geçerlidir. `remaining_tier_xp == 0` ise (pet zaten kendi tier tavanında, evrim materyali bekliyor) Kural 8 hiç devreye girmez — akış doğrudan Kural 5'in bankalama mantığına gider:

**Düzeltme (design-review 2026-07-01, 3. tur — systems-designer + economy-designer'ın bağımsız yakaladığı yeni blocker)**: Önceki sürümde bu pseudocode `pet.banked_progress` adında tanımsız bir terim kullanıyordu — bu, `pet.banked_xp` (farklı bir anlamı olan, tavan-sonrası taşma alanı) ile karıştırılabilir ve bir implementer yanlışlıkla `banked_xp`'yi yerine koyarsa Kural 8'in %50 sınırı HER taze-olmayan pet için sessizce bozulurdu. Aşağıdaki pseudocode artık `XPInvestedThisTier(pet)` adında, açıkça tanımlanmış TÜRETİLMİŞ bir değer kullanıyor (persisted bir alan değil):

```
function ApplyPotion(pet, potion_xp):
    if pet.tier == "SS" and pet.level == 40:
        return REJECTED  # UI zaten bu hedefi listelemez — bkz. Kural 2 hedef kısıtı

    remaining_tier_xp = CumulativeXPToCap(pet) - XPInvestedThisTier(pet)
    # XPInvestedThisTier(pet) = Σ_{i=1}^{pet.level-1} XP_required(i) + pet.xp
    #   — TÜRETİLMİŞ bir değerdir (level/xp'den hesaplanır), AYRI bir persisted alan DEĞİLDİR.
    #   pet.banked_xp İLE KARIŞTIRILMAMALIDIR: banked_xp yalnızca pet ZATEN tavandayken
    #   (level==tier_cap) biriken tavan-sonrası taşma XP'sidir (Kural 5); XPInvestedThisTier
    #   ise pet HENÜZ tavanda DEĞİLKEN bu tier içinde şu ana kadar harcanmış toplam XP'dir.
    if remaining_tier_xp == 0:
        # pet tavanda, evrim bekliyor — Kural 8 ATLANIR, doğrudan Kural 5
        pet.banked_xp += potion_xp
        return APPLIED_FULL

    max_applicable_xp = floor(remaining_tier_xp * overfill_cap_ratio)
    if potion_xp <= max_applicable_xp:
        ApplyXP(pet, potion_xp)             # Kural 3 algoritması, onay istenmez
        return APPLIED_FULL
    else:
        if not user_confirms(potion_xp, max_applicable_xp):
            return CANCELLED                # iksir tüketilmez
        ApplyXP(pet, max_applicable_xp)     # kalan (potion_xp - max_applicable_xp) kaybolur
        return APPLIED_PARTIAL
```

Bu, Formül 2'nin (altına çevirme) bu akışın HİÇBİR adımında çağrılmadığını netleştirir — potion kaynaklı XP hiçbir zaman doğrudan altına çevrilmez, ya seviye atlatır ya bankalanır. Altına çevirme sadece savaş zaferi kaynaklı XP için, sadece oyuncu Lv30'da çalışır (bkz. Formül 2 "Ne zaman uygulanır").

### States and Transitions

| Durum | Tetikleyici | Sonraki Durum |
|-------|-------------|----------------|
| Oyuncu: Level N (Aktif) | Zafer → XP eklenir | Level N+1 (eşik aşılırsa) veya aynı level |
| Oyuncu: Level 30 (Max) | XP kazanılır | Sink'e yönlendirilir, level sabit |
| Pet: Level N / Tier T | Zafer → XP eklenir | Level N+1 veya aynı level |
| Pet: Level=cap(T) | "Evrime Hazır" | Evrim (materyal + oyuncu onayı) → Level 1 / Tier T+1 |

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı |
|--------|-----|-----------|
| Savaş Sistemi / BattleReward | ← alır | `ExpGained` değeri — zafer sonu üretilen XP |
| Oyuncu Sınıf Sistemi | → sağlar | `GetPlayerLevel()` — `StatAtLevel` artık bu değeri kullanır (şu an sabit 1) |
| Pet/Canavar Veritabanı | → sağlar | `GetPetLevel(instanceId)` — `MonsterData.StatAtLevel` bu değeri kullanır |
| Pet Evrim Sistemi | → sağlar | `IsEvolutionEligible(instanceId)` — level tavanı kontrolü |
| Kaydetme/Yükleme | ↔ persist | `player_level/xp`, her pet instance'ın `level/xp/banked_xp/lifetime_pet_level` alanları |
| Savaş UI (BattleRewardScreen) | → sağlar | EXP bar animasyonu + "LEVEL UP!" olayı tetikleyici |

## Formulas

### Formül 1: XP-Seviye Eğrisi (`xp_to_next_level_formula`)

`XP_required(L) = floor(xp_base + xp_growth × (L - 1) + xp_quad × (L - 1)²)`

| Değişken | Tip | Değer/Aralık | Açıklama |
|----------|-----|------|----------|
| `L` | int | Oyuncu 1-29 / Pet 1-(tier_cap-1) | Mevcut seviye |
| `xp_base` | int (sabit) | 200 | Lv1→2 eşiği |
| `xp_growth` | int (sabit) | 120 | Seviye başına doğrusal artış |
| `xp_quad` | float (sabit) | 2.5 | Seviye başına karesel artış (geç seviyede dikleşme) |

**Çıktı Aralığı**: 200 (Lv1→2) – 8.370 (pet SS tier Lv39→40, en yüksek eşik). Oyuncu ve pet AYNI formül ve sabitleri paylaşır — sadece `L`, tier'e göre farklı cap'te durur ve evrimde 1'e resetlenir.

**Örnek**: Lv10'da (L=10, x=9): `floor(200 + 120×9 + 2.5×81) = floor(200+1080+202.5) = 1.482` XP gerekli.

**Sağlama** (~240 XP/savaş ortalama): Lv10 → ~28 savaş (1 dungeon run'ından az). Lv30 (oyuncu tavanı) → ~264-369 savaş (~7-9 tam run, ~1-4 hafta casual oyuncu için) — "haftalar, aylar değil" hedefiyle örtüşüyor.

**Tier bazlı kümülatif tablo (pet)**:

| Tier | Cap | Son seviye atlama eşiği | Tier'i baştan bitirmek için kümülatif XP |
|---|---|---|---|
| F | 10 | 1.320 | 6.628 |
| D | 15 | 2.182 | 15.764 |
| C | 20 | 3.170 | 29.588 |
| B | 25 | 4.282 | 48.724 |
| A | 30 | 5.520 | 73.798 |
| S | 35 | 6.882 | 105.434 |
| SS | 40 | 8.370 | 144.258 |

**Düzeltme notu**: Kümülatif değerler, her seviyenin bağımsız olarak floor edilmiş XP eşiğinin toplamıdır (Formül 1'in gerçek oyun-içi davranışıyla birebir — bkz. AC #6). Önceki taslaktaki değerler yuvarlanmamış ara toplamların floor'undan hesaplanmıştı ve 2-10 XP kadar yüksekti.

### Formül 2: Tavan Taşma → Altın (`xp_overflow_gold_formula`)

`gold_from_overflow = floor(xp_gained × xp_to_gold_rate)`

| Değişken | Tip | Değer/Aralık | Açıklama |
|----------|-----|------|----------|
| `xp_gained` | int | 200-280 (savaş ödülü) | Tavandayken kazanılan XP (tamamı taşma sayılır) |
| `xp_to_gold_rate` | int (sabit, tuning knob) | 2 | XP→altın dönüşüm oranı |

**Ne zaman uygulanır**: Sadece oyuncu Lv30'daysa VEYA pet SS tier Lv40'taysa (her ikisi de "gidecek yer yok" durumu — bkz. Kural 7 ve Kural 5'in SS istisnası). Diğer tüm pet tier'larında (F-S) taşan XP Kural 5 gereği bankalanır, altına çevrilmez.

**Geçiş anı netleştirmesi (design-review 2026-07-01, 3. tur — systems-designer'ın nice-to-have bulgusu)**: Bir pet TEK bir savaş zaferiyle Lv39'dan SS Lv40'a geçerse, o aynı XP grant'ının kalan kısmı (Kural 5 Algoritma'sının cap-branch'i) `banked_xp`'ye eklenir. SS terminal bir tier olduğundan (bir daha evrimleşemez), bu küçük kalıntı normalde hiç kullanılamayacak şekilde `banked_xp`'de sonsuza dek beklerdi. Bunu önlemek için: bir pet bu geçiş sırasında SS Lv40'a ulaştığı anda, o transaction'da oluşan kalıntı `banked_xp` da AYNI ANDA Formül 2 ile altına çevrilir (tıpkı sonraki tam savaşlarda olacağı gibi) — `banked_xp`, SS Lv40'ta hiçbir zaman kalıcı olarak sıfırdan farklı bir değerde takılı kalmaz.

**Kaynak kısıtı (design-review 2026-07-01, blocker düzeltmesi — economy-designer + systems-designer'ın "Dev iksir → 20.000 altın tek seferde" bulgusu)**: `xp_gained` girdisi bu formülde **yalnızca savaş zaferi kaynaklı XP** olabilir (`ExpGained`, 200-280 aralığı). XP İksirleri bu formüle **hiçbir koşulda** doğrudan girdi olamaz — çünkü (a) XP İksirleri zaten yalnızca aktif pete uygulanır (Kural 2), (b) pet SS tier Lv40'taysa (bu formülün pet tarafındaki tek tetikleyici durumu) UI potion hedef seçiminde bu peti listelemez (Kural 2 hedef kısıtı), dolayısıyla bir iksirin SS Lv40 bir pete ulaşması mekanik olarak imkânsızdır. Bu, önceki taslakta potion'ın da bu formüle (SS Lv40 durumunda) girebildiği ve tek uygulamada `floor(10.000×2)=20.000` altın gibi orantısız bir tutar üretebildiği açığı kapatır — çıktı aralığı (400-560) bu kısıtla birlikte her zaman geçerlidir.

**Çıktı Aralığı**: 400-560 altın/savaş (tavandayken). `floor_gold_formula`'nın 4-5. kat karşılığına yakın — "güzel bonus", jackpot değil.

**Varsayım netleştirmesi (design-review 2026-07-01)**: Bu "güzel bonus, jackpot değil" değerlendirmesi, günde ~10-15 savaş (ortalama casual oturum) varsayımına dayanır: 10-15 savaş/gün × 400-560 altın = ~4.000-8.400 altın/gün — idle altın tavanının (24 saatte ~54.000 altın, `ekonomi.md` Formül 4) küçük bir kısmı, dolayısıyla trivial değil ama jackpot da değil. Bu oran çok daha yüksek günlük savaş sayılarında (ör. 100+/gün, agresif otofarm kullanıcıları) orantısını kaybedebilir — bu durumda idle geliriyle karşılaştırıldığında baskın hale gelebilir. Balance testing aşamasında gerçek günlük savaş sayısı dağılımıyla doğrulanmalı.

**Not**: Bu, `canavar-guclendirme.md`'deki eski altın-XP satın alma sisteminin YERİNE geçer (o sistem deprecated edilecek — bkz. Dependencies ve Open Questions).

## Edge Cases

- **If pet tier'ının level tavanına ulaşmışsa ve zafer sonu yeni XP kazanılırsa**: Formül 2 devreye girmez (SS hariç) — kazanılan XP tamamen bankalanır (Kural 5), pet'in biriken XP'si artar ama level atlamaz. Evrim gerçekleştiğinde bu birikim otomatik olarak yeni tier'ın Lv1→2 eşiğine uygulanır.

- **If oyuncu Lv30'dayken veya pet SS tier Lv40'tayken zafer kazanılırsa**: Kazanılan XP'nin tamamı Formül 2 ile altına çevrilir, level/xp state değişmez.

- **If tek bir ödül (ör. Giant XP iksiri) birden fazla level atlatacak kadar büyükse**: Level atlamaları sırayla işlenir, ama stat/HP-oranı ölçeklemesi (Kural 4) sadece son duruma göre bir kez uygulanır — ara adımlarda yuvarlama sapması birikmez.

- **If bir iksirin XP değeri, hedef pet'in `max_applicable_xp` sınırını aşarsa (Kural 8, REVIZE)**: Onay ekranı açılır, uygulanacak/kaybolacak miktarlar gösterilir. Oyuncu onaylarsa iksir tüketilir ve yalnızca sınır kadarı uygulanır, kalan kaybolur. Onaylamazsa iksir tüketilmez.

- **If bir iksir (savaş XP'si de dahil), pet'i tam tier tavanına (`level == tier_cap`) getirecek veya aşacak kadar büyükse**: Level-atlama döngüsü `level == tier_cap`'e ulaştığı anda durur; bu noktadan sonraki TÜM XP (kaynağı fark etmeksizin) Kural 5 gereği bankalanır. Pet, evrim gerçekleşmeden `tier_cap`'in üstüne asla çıkamaz (bkz. Kural 5 uygulama notu, AC #24).

- **If pet zaten tavandayken (banked XP > 0) bir XP İksiri uygulanırsa**: Bu senaryo yalnızca F/D/C/B/A/S tier tavanları için geçerlidir (evrim materyali bekleniyor) — **SS tier Lv40'a XP İksiri hiç uygulanamaz** çünkü UI o pet'i hedef olarak listelemez (Kural 2 hedef kısıtı, design-review 2026-07-01). F-S tier tavanlarında Kural 8'in yüzdesel sınırı devreye girmez çünkü `remaining_tier_xp = 0` — iksirin tamamı doğrudan Kural 5'in bankalama mantığına gider (bankalanır, altına ÇEVRİLMEZ — Formül 2'ye potion asla girmez, bkz. Formül 2 Kaynak kısıtı). Kural 8 yalnızca pet HENÜZ tavanda değilken, tek bir uygulamanın tier'ı erken bitirmesini sınırlar.

- **If aktif pet savaşlar arasında değiştirilirse**: Yeni aktif pet o andan itibaren XP kazanır; önceki aktif pet'in level/xp'i olduğu yerde donar (instance'a bağlı, kaybolmaz).

- **If pet "Evrime Hazır" bayrağını taşırken oyuncu evrim materyali biriktirmemişse**: Level tavanında beklemeye devam eder, fazla XP bankalanmaya devam eder — evrim engellenmiş olması level ilerlemesini durdurmaz, sadece tier atlamasını erteler.

- **If `IsEvolutionEligible(instanceId)` `true` dönerken evrim işlemi TAM O ANDA materyal kontrolünde başarısız olursa (race condition)**: **(BLOCKED — design-review 2026-07-01, 3. tur, qa-lead'in bulgusu, AC #12b ile aynı desende ayrıca takip ediliyor)** Bu senaryonun kesin davranışı (evrim işlemi temiz şekilde reddedilir mi, bayrak durumu bozulur mu, vb.) bu GDD'nin kapsamı DIŞINDADIR — materyal kontrolü ve evrim transaction'ının kendisi `canavar-toplama-evrim.md`'nin sorumluluğundadır (bkz. Interactions with Other Systems: Pet Evrim Sistemi `IsEvolutionEligible` + evrim sonrası reset çağrısı). Bu GDD yalnızca seviye-tavanı bayrağını (`true`/`false`) sağlar; bayrak `true` iken materyal eksikse ne olacağı `canavar-toplama-evrim.md`'nin F→SS revizyonunda tanımlanmalı ve test edilmelidir (Open Question #6 ile aynı blocker'a bağlıdır).

- **If bir pet Lv1'ken (yeni evrimleşmiş veya yeni kazanılmış) bir sonraki savaşta yenilgi alınırsa**: Yenilgi XP vermez (Kural 2, sadece zafer) — level/xp değişmez, ceza da yoktur ("kayıp/ceza yok" anti-sütununa uygun).

- **If aktif pet, savaş zaferle bitmesine rağmen savaş sırasında baygın düşmüşse (current_hp=0)**: (design-review 2026-07-01, systems-designer bulgusu) Baygınlık, zaferin sonucunu değiştirmez — pet zafer XP'sini tam olarak alır (Kural 2), gerekirse level de atlar. HP oranlı ölçekleme formülü (Kural 4) `floor((0/old_max_hp) × new_max_hp) = 0` verir — pet level atlasa bile baygın kalmaya devam eder; baygın bir pet'in savaş öncesi/sonrası ne zaman iyileştiği Sağlık/Can Sistemi'nin (`saglik-can-sistemi.md`) kapsamındadır, bu GDD sadece level-up anındaki HP hesabından sorumludur.

- **If iki farklı pet aynı gün evrimleşip ikisi de level tavanına aynı anda ulaşırsa**: Her instance bağımsız kendi state'ini taşıdığından çakışma olmaz — aynı anda birden fazla pet "Evrime Hazır" olabilir, sırayla değerlendirilir.

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| Savaş Sistemi / CombatManager | Sert | `GenerateReward()` → `ExpGained` | Olmadan XP kaynağı yok |
| Loot / Ödül Sistemi | Sert | XP İksiri item drop'ları (Mini/Small/Medium/Large/Giant) | Olmadan ikinci XP kaynağı yok |
| Kaydetme/Yükleme | Sert | `player_level/xp`, her pet instance `level/xp`, `banked_xp` **ve `lifetime_pet_level`** persist | Olmadan ilerleme kaybolur. **Düzeltme (design-review 2026-07-01)**: `banked_xp` (Kural 5) önceki sürümde persist listesinde eksikti — "kaybolmaz" garantisi bu alanın kalıcı olmasını gerektirir. **Blocker kapatıldı (design-review 2026-07-01, 2. tur)**: `kaydetme-yukleme.md`'nin kayıt şeması bu dördünü (player_level, player_xp, banked_xp, lifetime_pet_level) içermiyordu — bu revizyonda `kaydetme-yukleme.md` da güncellendi (bkz. o dosyanın `monster_collection` ve oyuncu karakteri alanları). |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| Oyuncu Sınıf Sistemi | Sert | `GetPlayerLevel()` → `StatAtLevel()` girdisi | Şu an sabit 1 kullanıyor, bu GDD gerçek değeri sağlar |
| Pet/Canavar Veritabanı | Sert | `GetPetLevel(instanceId)` → `MonsterData.StatAtLevel()` girdisi | Aynı şekilde |
| Pet Evrim Sistemi | Sert | `IsEvolutionEligible(instanceId)` + evrim sonrası reset çağrısı | Evrim tetiklemesi bu bayrağa bağlı |
| Ekonomi | Yumuşak | `xp_overflow_gold_formula` çıktısı `GrantGold()` çağırır | Sadece tavan durumunda çalışır |
| Savaş UI (BattleRewardScreen) | Sert | EXP bar animasyonu + LEVEL UP olayı tetikleyici | Sunum katmanı |

### Çapraz Bağımlılık / Deprecation Notları

- **`canavar-guclendirme.md` Kural 2 (altınla XP satın alma, düz max level 50) bu GDD ile çelişiyor — DEPRECATED edilmeli.** Bu GDD onun yerini alıyor.
- **Karar (design-review 2026-07-01) — Altın açığı çözümü**: `level_up_cost` altın harcama noktası TAMAMEN emekliye ayrılır — canavar seviye atlama artık tamamen altınsız (yalnızca savaş XP'si + XP İksiri). Bu, `ekonomi.md` Kural 3'teki "Canavar seviye atlama → level_up_cost" satırının kaldırılmasını gerektirir. Boşalan altın harcama ihtiyacı, zaten MVP kapsamında olan **Ekipman Sistemi** (`ekipman-sistemi.md`) ve **Dükkan Sistemi** (`dukkan.md`) tarafından karşılanır — bu iki sistem kendi altın sink'lerine sahiptir ve bu GDD'nin kapsamı dışındadır. `registry/entities.yaml`'daki `level_up_cost_formula` ve `growth_rates` alanları (şu an `status: active`) deprecated işaretlenmeli — bu, ayrı bir Ekonomi/Canavar Güçlendirme revizyon oturumunda yapılmalıdır (bu GDD'nin kendisi registry'yi düzenlemez, sadece kararı burada belgeler).
- **Düzeltme**: Önceki sürümde bu bölümde `growth_rates` sabitinin (registry, kaynak: `canavar-guclendirme.md`, nadirliğe göre 0.02–0.03 — `StatAtLevel()`'ın kullandığı per-level STAT büyüme çarpanı) bu GDD'nin `xp_base/xp_growth/xp_quad` sabitleriyle (XP eşik eğrisi — level atlamak için gereken XP miktarı) değiştirileceği yazılıydı. Bu hatalıydı: ikisi ayrı formüllere ait — biri stat büyümesini, diğeri XP eşiğini belirler; biri diğerinin yerine geçemez. `growth_rates`'in deprecation durumu tamamen Open Question #1'deki `canavar-guclendirme.md` retrofit oturumuna bağlıdır — bu GDD `growth_rates`'e dokunmaz.
- **`xp_potion_values`** (registry, kaynak: `loot-odul-sistemi.md`: 25/100/500/2000/10000) yeni eğriyle tam orantılı değil — bu GDD'nin kapsamı dışında, `loot-odul-sistemi.md`'nin ileride revize edilmesi gerekiyor (Open Questions'ta not düşülüyor).
- **Pet Evrim Sistemi (`canavar-toplama-evrim.md`) bu GDD'nin varsaydığı arayüzü henüz sağlamıyor.** Kural 5/6 ve `IsEvolutionEligible(instanceId)`, F-D-C-B-A-S-SS tier tavanına dayalı bir evrim modeli varsayıyor; ancak `canavar-toplama-evrim.md` hâlâ eski 3-form (Form A/B/C) + nadirlik modelini kullanıyor ve bu arayüzü tanımlamıyor. Bu GDD implement edilmeden önce `canavar-toplama-evrim.md`'nin F→SS revizyonu tamamlanmalı (bkz. systems-index.md Next Steps: `/design-review canavar-toplama-evrim`, ve Open Question #6).
- **YENİ (design-review 2026-07-01) — `canavar-veritabani.md` ile üçüncü bir çelişki bulundu.** `canavar-veritabani.md` Kural 6 (Status: Revised, zaten F-D-C-B-A-S-SS kullanıyor) tier atlaması için AYRI bir "tier içi form gelişimi" şartı tanımlıyor (Form 1→2→3, evrim malzemesiyle ilerleyen, seviyeden bağımsız bir gate). Bu, bu GDD'nin Kural 5/6'sıyla (seviye tavanı = tek gate) doğrudan çelişiyor. **Karar**: Seviye tavanı tek geçerli evrim tetikleyicisidir (yukarıdaki Kural 5/6 notuna bakınız) — `canavar-veritabani.md`'nin kendi revizyon oturumunda Form-aşaması gate'i kaldırılmalı veya bu GDD'nin sağladığı seviye-tavanı sinyaline bağlanacak şekilde yeniden yazılmalıdır. Bu, implement öncesi ayrı bir blocker'dır (bkz. Open Question #6).

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `xp_base` | 200 | 100-400 | İlk level atlama yavaş → erken oyun hook'u zayıflar | Çok hızlı → eşik hissi kaybolur |
| `xp_growth` | 120 | 60-200 | Orta-geç seviyeler arası fark çok açılır → grind hissi | Eğri düz kalır → büyüme hissi zayıflar |
| `xp_quad` | 2.5 | 1.0-5.0 | Geç seviyeler aşırı dikleşir → cap'e ulaşmak aylar sürer | Eğri neredeyse doğrusal → cap'e çok hızlı ulaşılır |
| `xp_to_gold_rate` | 2 | 1-5 | Tavan sonrası altın ana kaynakları gölgede bırakır | Tavan sonrası ödül hissedilmez |
| `pet_tier_level_caps` | F=10,D=15,C=20,B=25,A=30,S=35,SS=40 | ±5 her tier'da | Tier içinde çok uzun kalınır → evrim motivasyonu düşer | Tier çok hızlı biter → evrim materyali darboğazı öne çıkar |
| `overfill_cap_ratio` (eski adı `potion_lock_threshold_ratio` — design-review 2026-07-01'de anlamı değişti; **registry `entities.yaml` bu revizyonda güncellendi**, bkz. Dependencies) | 0.5 | 0.3-0.7 | Kısıtlama gevşer → büyük iksirler tek uygulamada tier'ı aşırı hızlı bitirir | Kısıtlama sıkılaşır → büyük iksirler sürekli kısmi uygulanır/kaybolan XP oranı artar, oyuncu hayal kırıklığı yaşar |
| `player_level_cap` | 30 | 20-50 | Oyuncu çok uzun süre büyür → MVP için fazla içerik gerekir | Tavan erken gelir → geç oyunda büyüme hissi biter |

**Etkileşim Uyarıları**:
- `xp_base` × `xp_growth` × `xp_quad` birlikte tüm eğriyi belirler; birini değiştirmek diğerleriyle yeniden dengelenmeli (Formül 1 tablosu yeniden hesaplanmalı).
- `pet_tier_level_caps` × xp eğrisi birlikte "bir tier'ı bitirme süresi"ni belirler — bu süre, Pet Evrim Sistemi'nin evrim materyali drop oranıyla senkron olmalı (aksi halde biri diğerini bekler).
- `xp_to_gold_rate` × `floor_gold_formula` birlikte tavan sonrası altın ile normal zindan altınının karşılaştırmasını belirler; oran çok yüksek olursa tavandaki oyuncular zindan yapmadan zenginleşir.

## Visual/Audio Requirements

### VFX Gereksinimleri

- Level Up (oyuncu veya pet): Kısa ışık patlaması + portre üzerinde parlama, büyük "LEVEL UP!" yazısı, statlar eski değerden yeniye sayılı olarak artar (HP/ATK/DEF/SPD).
- Savaş sonu EXP bar dolumu: Altın/yeşil renkli bar dolarken sayı sayacı eşliğinde artar (ör. 200→283).
- "Evrime Hazır" bayrağı: Pet kartı çerçevesi nabız gibi parlar + küçük "!" rozeti.
- XP İksiri kullanımı: Şişe kırılma/içilme animasyonu + anında bar sıçraması.
- Tavan sonrası altın dönüşümü: EXP bar yerine küçük altın sikke ikonu + "+X altın" popup.

### Audio Gereksinimleri

- Level Up: Kısa yükselen fanfar (0.5-1sn) — oyuncu ve pet aynı sesi paylaşır, pet için pitch biraz daha tiz.
- EXP bar dolumu: Hafif "tık tık" sayaç sesi.
- Evrime Hazır bildirimi: Kısa "ding".
- XP İksiri kullanımı: Şişe sesi + yutma efekti.

> 📌 **Asset Spec** — Visual/Audio gereksinimleri tanımlandı. Art bible onaylandıktan sonra `/asset-spec system:level-deneyim-sistemi` çalıştırarak per-asset spesifikasyonlar üretilebilir.

## UI Requirements

- Savaş sonu ödül ekranında (BattleRewardScreen) EXP barın yanında hem oyuncu hem pet için ayrı "Lv.N" etiketi gösterilir.
- Level atlanırsa ekranda "LEVEL UP! Lv.N → Lv.N+1" satırı + eski→yeni stat karşılaştırması (HP/ATK/DEF/SPD) gösterilir.
- Koleksiyon/Envanter ekranında her pet kartına küçük bir XP bar + level göstergesi eklenir; tavana ulaşan pet kartında "Evrime Hazır" rozeti görünür.
- **YENİ (design-review 2026-07-01) — Toplam Seviye göstergesi**: Pet kartında, tier-içi `pet_level`'ın yanında küçük ve sürekli bir "Toplam Seviye: N" (`lifetime_pet_level`) etiketi gösterilir — bu sayı evrimde SIFIRLANMAZ, sadece monoton artar. Evrim anındaki "LEVEL UP"/evrim ekranında da bu sayı görünür kalır, böylece `pet_level=1`'e sıfırlanma anı bir güç kaybı gibi hissettirmez (game-designer'ın "düşüş hissi" bulgusuna karşı önlem).
- **YENİ (design-review 2026-07-01) — Bankalanmış XP göstergesi**: Pet tavana ulaştığında (level=tier_cap), XP barının altında/yanında küçük bir "Bankalanan: +X XP" sayacı gösterilir ve her zafer sonrası bu sayaç görünür şekilde artar (sayı animasyonuyla) — böylece tavan sonrası savaşlar da görünür bir ilerleme hissi verir, "sessiz/fark edilmeyen ilerleme" olumsuz fantezisi önlenir.
- XP İksiri kullanım ekranı: envanterden iksir seç → hedef pet seç → önizleme (mevcut XP + iksir XP, level atlarsa öngörü) → onayla. **REVIZE**: İksirin XP değeri hedefin `max_applicable_xp` sınırını aşarsa (Kural 8), "Bu pet için çok güçlü — yalnızca [X] XP uygulanacak, [Y] XP kaybolacak. Devam?" onay dialogu gösterilir (buton gri/devre dışı DEĞİL — oyuncu bilinçli olarak devam edebilir). **YENİ (design-review 2026-07-01, blocker düzeltmesi)**: Hedef pet seçim listesi, SS tier Lv40'taki (mutlak son tavan) petleri hiç göstermez — bkz. Kural 2 hedef kısıtı, AC #27. **YENİ (design-review 2026-07-01, 3. tur — game-designer'ın "SS40 sessizce kayboluyor" bulgusu)**: SS tier Lv40'taki pet, Koleksiyon/Envanter kartında ve iksir hedef listesinde sessizce dışlanmak yerine belirgin bir "MAX" rozeti alır ("Evrimleştir Hepsini" tamamlayıcılığını onaylayan olumlu bir işaret) — bu, sınırlamayı bir eksiklik değil bir başarı göstergesi olarak çerçeveler.
- **YENİ (design-review 2026-07-01, 3. tur — systems-designer'ın "remaining_tier_xp çok küçükken 0 XP uygulanacak dialogu" bulgusu)**: Eğer `max_applicable_xp == 0` ise (pet doğal olarak tavana çok yakın, ör. `remaining_tier_xp=1`), Kural 8'in onay dialogu ASLA gösterilmez — bunun yerine o iksir, o hedef pet için envanterde gri/devre dışı görünür (kısa bir tooltip: "Bu pet zaten tavana çok yakın — bu iksir gerekmiyor"). Bu, "0 XP uygulanacak, kaybolacak" gibi anlamsız bir onay isteğini önler; pet zaten normal savaş XP'siyle doğal olarak tavana ulaşacaktır.
- Minimum dokunma hedefi: 44×44 dp tüm butonlar.

> 📌 **UX Flag — Level / Deneyim Sistemi**: Bu sistem UI gereksinimleri içeriyor. Phase 4'te `/ux-design` çalıştırarak savaş ödül ekranı ve koleksiyon kartı XP göstergesi için UX spec oluşturulmalı.

## Acceptance Criteria

1. **GIVEN** oyuncu karakteri Lv10, **WHEN** `XP_required(10)` hesaplanırsa, **THEN** sonuç `floor(200 + 120×9 + 2.5×81) = 1.482` olmalı (Formül 1, oyuncu tarafı).

2. **GIVEN** aktif pet F tier Lv9 (tier tavanının bir altındaki, `XP_required`'ın tanımlı olduğu son seviye — cap=10'un kendisi için `XP_required` tanımsızdır), **WHEN** `XP_required(9)` hesaplanırsa, **THEN** sonuç `floor(200 + 120×8 + 2.5×64) = 1.320` olmalı (Formül 1, pet tarafı — Formüller bölümündeki F tier kümülatif tablosuyla eşleşir).

3. **GIVEN** oyuncu Lv3, mevcut biriken XP 300 (Lv3→4 eşiği = 450), **WHEN** savaş zaferi 200 XP kazandırırsa (toplam 500), **THEN** oyuncu Lv4'e yükselir ve 50 XP bir sonraki eşiğe (Lv4→5) devreder — kayıp yok.

4. **GIVEN** oyuncu Lv9→Lv10 seviye atlarsa, **WHEN** level-up işlenirse, **THEN** ATK/DEF/SPD `StatAtLevel(baseStat, 10)` ile yeniden hesaplanıp doğrudan yeni değere ayarlanır (orantılı ölçekleme HP'ye özgüdür, diğer statlara uygulanmaz).

5. **GIVEN** pet Lv7'de HP=80/MaxHP=100 (%80 dolu) iken Lv8'e yükselirse ve yeni MaxHP=120 olursa, **WHEN** level-up işlenirse, **THEN** yeni current HP = `floor(0.80 × 120) = 96` olur — tam heal (120) verilmez.

6. **GIVEN** aktif pet F tier Lv5'te, 0 biriken XP (`remaining_tier_xp` = 720+862+1010+1162+1320 = 5.074, `max_applicable_xp` = floor(5.074×0.5) = 2.537), **WHEN** Large XP İksiri (2000 XP, `max_applicable_xp`'nin altında olduğundan onay istenmeden doğrudan uygulanır) uygulanırsa, **THEN** sıralı olarak Lv6 (720 XP harcanır) ve Lv7'ye (862 XP harcanır) yükselir, kalan 418 XP Lv7→8 eşiğine devreder (720+862+418=2.000 ✓); stat/HP-oranı yeniden hesaplaması yalnızca son durumda (Lv7) bir kez tetiklenir, Lv6 için ayrı bir ara hesaplama tetiklenmez.

7. **GIVEN** oyuncunun envanterinde aktif olmayan bir Pet B varken aktif Pet A ile savaşa girilirse, **WHEN** savaş zaferi kazanılırsa, **THEN** yalnızca Pet A'nın XP/level'ı artar; Pet B'nin level/xp değerleri değişmeden kalır.

8. **GIVEN** savaş zaferi `ExpGained=240` üretirse, **WHEN** ödül uygulanırsa, **THEN** hem oyuncu hem aktif pet 240 XP'nin tamamını ayrı ayrı alır (paylaştırılmaz, bölünmez).

9. **GIVEN** bir savaş yenilgiyle sonuçlanırsa, **WHEN** ödül hesaplanırsa, **THEN** ne oyuncu ne de aktif pet XP kazanır; level/xp durumu ve altın (taşma dönüşümü dahil) değişmeden kalır.

10. **GIVEN** XP İksiri kullanım ekranı açıkken, **WHEN** oyuncu hedef olarak kendi karakterini seçmeye çalışırsa, **THEN** oyuncu karakteri geçerli bir hedef olarak listelenmez/seçilemez — yalnızca aktif pet seçilebilir hedeftir.

11. **GIVEN** mağaza/IAP satın alma ekranı, **WHEN** oyuncu XP İksiri (Mini/Small/Medium/Large/Giant) ararsa, **THEN** hiçbiri gerçek para karşılığı satın alınabilir bir ürün olarak listelenmez — yalnızca loot düşmesi veya ödüllü reklam akışından elde edilir.

12a. **(REVIZE — design-review 2026-07-01, 2. tur, qa-lead'in "AC #12 çalıştırılamaz" bulgusuna karşı bölünmüş, ŞİMDİ ÇALIŞTIRILABİLİR)** **GIVEN** bu GDD'nin kendi `pet_level`/`tier_cap` state'i (mock/bağımsız, `canavar-toplama-evrim.md`'ye bağlı olmadan) pet_level=tier_cap durumuna getirilirse, **WHEN** eligibility-bayrağı hesaplanırsa (yalnızca "`pet_level == tier_cap` mi?" kontrolü — bu GDD'nin Kural 5'in TAMAMEN sorumlu olduğu kısmı), **THEN** bayrak `true` döner, evrim materyali sahipliğinden tamamen bağımsızdır. Bu birim testi `IsEvolutionEligible`'ın gerçek implementasyonuna bağlı değildir — yalnızca bu GDD'nin ürettiği bayrak mantığını doğrular.

12b. **(BLOCKED — entegrasyon AC'si, ayrı takip edilir)** **GIVEN** aktif pet F tier Lv10'a (tier tavanı) ulaşırsa, **WHEN** gerçek `canavar-toplama-evrim.md::IsEvolutionEligible(instanceId)` çağrılırsa, **THEN** 12a'daki bayrak mantığını kullanarak `true` döner. **Test edilebilirlik notu (design-review 2026-07-01)**: Bu AC, `canavar-toplama-evrim.md`'nin henüz sağlamadığı bir arayüze bağımlıdır (Open Question #6, BLOCKER) — o dosyanın F→SS revizyonu tamamlanmadan bu AC çalıştırılamaz ve story'nin Definition of Done'ından **ayrı** takip edilmelidir (12a'nın geçmesi, bu story'yi Done yapmaya yeter; 12b üst-bağımlılık dosyası revize olunca ayrıca kapatılır).

13. **GIVEN** aktif pet F tier Lv10'da (tavanda, SS değil), bankalanmış XP=0, **WHEN** savaş zaferi 250 XP kazandırırsa, **THEN** pet_level 10'da sabit kalır, bankalanmış XP 250'ye çıkar, Formül 2 (altına çevirme) tetiklenmez.

14. **GIVEN** aynı pet tavanda bankalanmış XP=250 iken evrim materyali henüz toplanmamışsa, **WHEN** ek bir savaş zaferi 260 XP daha kazandırırsa, **THEN** bankalanmış XP kümülatif olarak 510'a çıkar, level sabit kalır, "Evrime Hazır" bayrağı true kalmaya devam eder (evrimin ertelenmesi level ilerlemesini durdurmaz).

15. **GIVEN** oyuncu Lv30'da (tavan), **WHEN** savaş zaferi 250 XP kazandırırsa, **THEN** player_level/xp değişmez, altın `floor(250×2) = 500` kadar artar (Formül 2, oyuncu tavanı — Kural 7).

16. **GIVEN** aktif pet SS tier Lv40'ta (tavan, tier zinciri sonu), **WHEN** savaş zaferi 280 XP kazandırırsa, **THEN** pet_level/xp değişmez, altın `floor(280×2) = 560` kadar artar (Formül 2, pet SS istisnası — Kural 5).

17. **GIVEN** aktif pet F tier Lv10'da evrim onaylanıp D tier'a geçerse, **WHEN** evrim işlemi tamamlanırsa, **THEN** pet_level=1 ve pet_xp=0 olarak sıfırlanır (bankalanmış XP'nin uygulanmasından önceki taban durum).

18. **GIVEN** evrim anında bankalanmış XP=250 taşınıyorsa, **WHEN** evrim tamamlanıp bankalanmış XP otomatik uygulanırsa, **THEN** yeni tier'da Lv1→2 eşiği (200) aşılır ve pet aynı evrim işlemi içinde anında Lv2'ye çıkar, 50 XP bir sonraki eşiğe devreder (ayrı bir savaşa gerek yok).

19. **GIVEN** Pet A ve Pet B aynı gün kendi tier tavanlarına ulaşırsa, **WHEN** Pet A evrimleştirilirse, **THEN** Pet B'nin level/xp/bankalanmış XP durumu tamamen etkilenmeden kalır (instance bağımsızlığı).

20. **GIVEN** aktif pet Pet A'dan Pet B'ye değiştirilirse, **WHEN** bu değişiklikten sonra bir savaş kazanılırsa, **THEN** yalnızca Pet B'nin XP/level'ı güncellenir; Pet A'nın level/xp'i değişiklik anındaki değerinde donar.

21. **GIVEN** aktif pet D tier Lv1'de (yeni evrimleşmiş, 0 biriken XP; `remaining_tier_xp` = D tier kümülatif toplamı = 15.764, `max_applicable_xp` = floor(15.764×0.5) = 7.882), **WHEN** oyuncu Giant XP İksiri (10.000 XP) uygulamaya çalışırsa, **THEN** onay dialogu açılır: "Yalnızca 7.882 XP uygulanacak, 2.118 XP kaybolacak. Devam edilsin mi?" — oyuncu onaylamazsa iksir tüketilmez.

22. **GIVEN** aynı pet D tier Lv10'a ulaşırsa (`remaining_tier_xp` = 1.482+1.650+1.822+2.000+2.182 = 9.136, `max_applicable_xp` = floor(9.136×0.5) = 4.568), **WHEN** oyuncu Medium XP İksiri (500 XP, `max_applicable_xp`'nin çok altında) uygulamaya çalışırsa, **THEN** onay istenmeden doğrudan uygulanır — hiçbir buton devre dışı bırakılmaz (Kural 8, REVIZE).

23. **GIVEN** aktif pet D tier Lv1'de (yukarıdaki gibi, `max_applicable_xp` = 7.882), **WHEN** oyuncu Mini XP İksiri (25 XP) uygulamaya çalışırsa, **THEN** işlem onay istenmeden her zaman izin verilir — küçük iksirler pratikte sınırı aşmadığından ayrı bir muafiyet kuralına gerek yoktur, ama kısıtlama mekanik olarak TÜM boyutlara eşit uygulanır (Kural 8, REVIZE).

24. **GIVEN** pet F tier'dan D tier'a evrimleşir ve bankalanmış XP=20.000 (varsayımsal olarak çok uzun bekleme sonucu birikmiş), **WHEN** evrim tamamlanıp bankalanmış XP yeni Lv1 D-tier pet'e otomatik uygulanırsa, **THEN** sıralı level-atlama D tier kümülatif toplamını (15.764 XP) tüketerek Lv15'e (D tier tavanı) ulaşır, kalan 4.236 XP pet_level'ı D tavanının ÜSTÜNE çıkarmadan yeniden bankalanır ve pet anında D→C için "Evrime Hazır" bayrağını alır (Kural 5 uygulama notu — tavan taşması hard-stop invariantı, hangi kaynaktan gelirse gelsin ve miktarı ne olursa olsun geçerlidir).

25. **GIVEN** bir pet tavanda (level=tier_cap) bankalanmış XP=510 ile beklerken, **WHEN** oyun kaydedilip yeniden yüklenirse, **THEN** `banked_xp=510` değeri değişmeden geri yüklenir (persist edilen alan, bkz. Dependencies).

26. **GIVEN** aktif pet F tier Lv10'da (tavanda, bankalanmış XP=0), **WHEN** oyuncu bu pete Large XP İksiri (2000 XP) uygularsa, **THEN** Kural 8'in yüzdesel sınırı devreye girmez (`remaining_tier_xp=0` çünkü pet zaten tavanda) — iksirin tamamı doğrudan Kural 5'in bankalama mantığına gider, bankalanmış XP 2.000'e çıkar, level sabit kalır.

27. **(YENİ — design-review 2026-07-01, 2. tur, blocker düzeltmesi)** **GIVEN** aktif pet SS tier Lv40'ta (mutlak son tavan), **WHEN** oyuncu XP İksiri kullanım ekranını açıp hedef pet seçmeye çalışırsa, **THEN** bu pet hedef listesinde görünmez/seçilemez — hiçbir iksir bu pete uygulanamaz, dolayısıyla Formül 2'ye potion kaynaklı XP asla giremez (Kural 2 hedef kısıtı, Formül 2 Kaynak kısıtı).

28. **(YENİ — design-review 2026-07-01, 2. tur, game-designer'ın "evrim düşüş hissi" bulgusuna karşı önlem)** **GIVEN** aktif pet F tier Lv10'da (`lifetime_pet_level`=10 diyelim — F tier'da Lv1'den Lv10'a toplam 9 level atlamış, `lifetime_pet_level` başlangıç değeri 1 + 9 = 10) evrim onaylanıp D tier Lv1'e sıfırlanırsa ve otomatik bankalanmış XP uygulamasıyla D tier Lv2'ye çıkarsa, **WHEN** evrim işlemi tamamlanırsa, **THEN** `pet_level` görünürde 1'den 2'ye döner (Kural 6) AMA `lifetime_pet_level` asla geriye gitmez veya sıfırlanmaz — evrim + otomatik level atlama sonrası `lifetime_pet_level = 11` olur (bir önceki değer + evrim-sonrası tek level atlama), Koleksiyon/Envanter UI'da "Toplam Seviye: 11" olarak gösterilir.

29. **(YENİ — design-review 2026-07-01, 3. tur, blocker düzeltmesi — qa-lead'in "atomiklik kuralının AC'si yok" bulgusu)** **GIVEN** AC #6'daki senaryo (F tier Lv5, Large XP İksiri 2000 XP, sıralı olarak Lv6→Lv7'ye çıkıp 418 XP devretmesi gereken çok-adımlı zincir), **WHEN** zincir belleğe tam uygulandıktan HEMEN SONRA (Kural 3'ün "kayıt/yükleme bütünlüğü" garantisi gereği, zincir başlamadan bitene kadar hiçbir kayıt noktası araya giremez) oyun kaydedilip yeniden yüklenirse, **THEN** yüklenen state her zaman zincirin TAM SONUÇLANMIŞ halini yansıtır (`pet_level=7`, `pet_xp=418`) — asla ara bir durum (ör. `pet_level=6` ile donmuş) olarak yüklenmez. Bu, Kural 3'ün atomiklik iddiasını bağımsız olarak test eden ilk AC'dir (önceki AC #25 yalnızca durgun-durum `banked_xp` kalıcılığını test ediyordu, zincir kesintisini değil).

*`qa-lead` consulted — Lean mode (Section H mandatory spawn).*

## Open Questions

1. **canavar-guclendirme.md revizyonu (BLOCKER, KARAR VERİLDİ — design-review 2026-07-01)** — Kural 2'deki altınla XP satın alma / düz max level 50 sistemi kaldırılmalı, bu GDD'ye referans vermeli. **Karar**: `level_up_cost` altın sink'i tamamen emekliye ayrılır; boşalan altın harcama ihtiyacı Ekipman Sistemi + Dükkan Sistemi tarafından karşılanır (bkz. Dependencies → Çapraz Bağımlılık notları). Registry'de `growth_rates` ve `level_up_cost_formula` deprecated işaretlenmeli, `ekonomi.md` Kural 3'teki ilgili satır kaldırılmalı. → Karar bu GDD'de verildi, uygulama (dosya/registry düzenlemesi) ayrı bir `/design-system retrofit canavar-guclendirme.md` + Ekonomi GDD revizyon oturumunda yapılmalı. **Risk notu (design-review 2026-07-01, 2. tur — economy-designer)**: Ekipman/Dükkan sistemlerinin altın sink boyutunun bu boşluğu gerçekten karşıladığı henüz doğrulanmadı — bu, kapanmış bir karardan çok, `ekipman-sistemi.md`/`dukkan.md` ile koordinasyon gerektiren açık bir risk olarak izlenmelidir.

2. **xp_potion_values oranları güncel değil (ciddiyeti azaldı — design-review 2026-07-01)** — Mevcut değerler (25/100/500/2000/10000) yeni eğriyle orantılı değil; ölçüm F tier'da Giant'ın kümülatif tier XP'sinin ~%151'i, SS tier'da ~%7'si olduğunu gösterdi (~22x fark). Kural 8'in yeni yüzdesel-sınır mekaniği (design-review 2026-07-01) bu dengesizliğin OYUNCUYA YANSIYAN etkisini zaten sınırlıyor (bir uygulama asla tier'ın %50'sinden fazlasını veremez) — ama isimlendirme sorunu kalıyor: bir "Dev İksir" erken tier'larda sıklıkla kısmi uygulanıp XP kaybına yol açabilir, bu da "Dev" isminin vaat ettiğiyle çelişebilir. → `loot-odul-sistemi.md` revizyonunda potion değerlerinin tier'a göre ölçeklenmesi (sabit yerine oransal) değerlendirilmeli; bu artık bloke edici değil, iyileştirme fırsatı.

3. **Otofarm/Idle Sistemi etkileşimi belirsiz** — İdle savunma sonuçları da bu XP hattından mı besleniyor, yoksa ayrı bir mekanizma mı? → `otofarm-idle.md` ile çapraz kontrol gerekiyor.

4. ~~**Registry güncellemesi (Phase 5)**~~ **RESOLVED (2026-07-01, 2. tur güncellemesiyle birlikte)** — `xp_to_next_level_formula`, `xp_overflow_gold_formula` ve ilişkili sabitler (`xp_base`, `xp_growth`, `xp_quad`, `xp_to_gold_rate`, `pet_tier_level_caps`, `player_level_cap`, `overfill_cap_ratio` — eski adı `potion_lock_threshold_ratio`, 2. turda yeniden adlandırıldı) registry'ye eklendi/güncellendi.

5. ~~**systems-index.md güncellemesi**~~ **RESOLVED** — Bu sistem #34 olarak eklendi, Pet Evrim Sistemi (#8) ve Oyuncu Sınıf Sistemi (#10) satırlarına bu GDD dependency olarak eklendi.

6. **Pet Evrim Sistemi (`canavar-toplama-evrim.md`) F→SS revizyonu (BLOCKER, KARAR VERİLDİ — design-review 2026-07-01)** — Bu GDD'nin Kural 5/6'sı ve `IsEvolutionEligible(instanceId)` arayüzü, `canavar-toplama-evrim.md`'nin henüz sağlamadığı bir F-D-C-B-A-S-SS tier-tavanlı evrim modeli varsayıyor (o dosya hâlâ eski Form A/B/C + nadirlik modelini kullanıyor). **Karar**: Bu GDD'nin seviye-tavanı modeli (Kural 5/6) TEK geçerli evrim tetikleyicisidir. → `/design-review canavar-toplama-evrim` ile ayrı bir revizyon oturumunda ele alınmalı; bu GDD'nin implementasyonu ondan önce başlayamaz.

7. **YENİ (BLOCKER, KARAR VERİLDİ) — `canavar-veritabani.md` ile üçüncü çelişki**: `canavar-veritabani.md` Kural 6 (zaten Revised, F-D-C-B-A-S-SS kullanıyor) tier atlaması için ayrı bir "tier içi form gelişimi" (Form 1→2→3, malzeme bazlı, seviyeden bağımsız) gate'i tanımlıyor — bu GDD'nin seviye-tavanı modeliyle çelişiyor. **Karar**: Seviye tavanı tek geçerli tetikleyicidir; `canavar-veritabani.md`'nin Form-aşaması gate'i kaldırılmalı veya tier atlamasının bir sonucu (ör. görsel/stat bonusu) olacak şekilde yeniden çerçevelenmeli, ayrı bir evrim şartı OLMAMALI. → `canavar-veritabani.md`'nin kendi revizyon oturumunda ele alınmalı (aynı oturumda `canavar-toplama-evrim.md` ile birlikte değerlendirilmesi önerilir, zira ikisi de aynı evrim modelini etkiliyor).

8. **YENİ (design-review 2026-07-01, 2. tur — game-designer, NON-BLOCKER, disagreement kaydı — 3. turda pet tarafı eklendi)** — Oyuncu karakteri Lv30 tavanına ulaştıktan sonra (dokümanın kendi matematiğine göre 1-4 hafta içinde ulaşılabilir) hiçbir yeni büyüme ekseni yok; tüm gelecek XP kalıcı olarak altına dönüşür. game-designer bunu Blocking olarak işaretledi ("sürekli büyüme" pilar'ına aykırı, aylarca sürecek bir post-cap deneyim için); creative-director bunu bu GDD'nin kapsamı dışında bir canlı-servis yol haritası konusu olarak değerlendirdi ve pozisyonunu 3. turda da korudu — büyüme oyuncudan pet F→SS zincirine (144k+ XP, evrim materyaliyle geçitli) kayıyor, "büyüme sıfırlanıyor" değil "büyüme yer değiştiriyor." Bu GDD'de bir çözüm önerilmiyor — ileride bir prestij/mastery/alt-seviye sistemi ihtiyacı olarak `/roadmap` veya ayrı bir GDD'de değerlendirilebilir. **YENİ (design-review 2026-07-01, 3. tur — game-designer)**: Pet SS tier Lv40 da (oyuncu Lv30 gibi) mutlak bir "gidecek yer yok" durumu — bir daha büyüyemez, kazandığı XP sadece Formül 2 ile önemsiz miktarda altına dönüşür. Bu durum önceden yalnızca oyuncu tarafı için tartışılıyordu; aynı yol haritası notu SS40 pet'ler için de geçerlidir.

9. **YENİ (design-review 2026-07-01, 2. tur — game-designer, NON-BLOCKER)** — Kural 8'in yüzdesel sınırı `remaining_tier_xp`'ye bağlı olduğundan, optimal oyun tarzı büyük iksirleri taze bir tier'ın Lv1'ine kadar biriktirmektir (cap'i maksimize etmek için). Bu "hoarding" davranışı Edge Cases'te ele alınmadı, bu revizyona dahil edilmedi — ileride bir UX/ekonomi notu olarak değerlendirilebilir (ör. iksirlerin süre sınırlı olması veya erken kullanım teşviki).

10. **YENİ (design-review 2026-07-01, 3. tur — economy-designer, görünürlük için Formüller bölümünden taşındı, NON-BLOCKER)** — Formül 2'nin "Varsayım netleştirmesi" paragrafı, yüksek günlük savaş hacminde (100+/gün, agresif otofarm kullanıcıları) tavan-sonrası altın taşmasının idle altın tavanına (~54.000/24s) yaklaşabileceğini ve "güzel bonus, jackpot değil" varsayımının bu hacimde bozulabileceğini not ediyor. Bu risk daha önce yalnızca Formüller bölümünün içinde gömülüydü — takip edilebilirlik için buraya da taşındı. → Balance testing aşamasında gerçek günlük savaş sayısı dağılımıyla doğrulanmalı; gerekirse `xp_to_gold_rate`'e ek olarak günlük bir taşma-altını tavanı eklenmesi değerlendirilebilir.
