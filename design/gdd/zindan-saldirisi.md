# Zindan Saldırısı Sistemi (Dungeon Assault)

> **Status**: Designed (YENİ — 2026-07-02, kullanıcıyla tasarlandı)
> **Author**: user + game-designer
> **Last Updated**: 2026-07-02
> **Implements Pillar**: Hep Bir Şey Var, Senin Tempon

> **Ayrılma notu (2026-07-02)**: Bu sistem, eski `zindan-kesif.md`'nin yerini alır — o dosya `kesif-alani.md`'ye (gerçek implementasyona göre) birleştirildiği için silindi. "Zindan Saldırısı" kavramsal olarak **tamamen yeni ve ayrı** bir sistemdir — Keşif Alanı'nın ana ilerleme döngüsünden bağımsız, günlük tekrarlanan, tek-kaynak-odaklı bir yan içerik. İsimlendirme kasıtlı: hem "Keşif Alanı" hem eski "Zindan Keşif" adında "keşif" geçtiği için karışıyordu — bu sistem "saldırı" (assault/raid) temalı, farklı bir fantezi sunuyor.

## Overview

**Zindan Saldırısı**, oyuncunun günlük olarak tekrarlayabileceği, tek bir kaynağa odaklanmış kısa saldırı görevleridir. İki türü vardır: **EXP Zindanı** ve **Altın Zindanı**. Her tür kendi başına 5 kattan oluşan, tek dalgalı, hızlı bir "koşu"dur (run) — oyuncu 5 katı sırayla temizlemeye çalışır, herhangi bir katta kaybederse koşu biter (o ana kadar temizlenen katların ödülü elde tutulur, kalıcıdır).

Her zindan türünün **günde 3 giriş hakkı** vardır (Keşif Alanı'nın 100 enerjisinden tamamen bağımsız bir kaynak — enerji harcamaz). Giriş hakları her gün sıfırlanır. Zorluk, oyuncunun Keşif Alanı'ndaki en yüksek temizlediği aşamaya göre ölçeklenir — böylece oyuncu ilerledikçe Zindan Saldırısı da anlamlı zorlukta kalır.

Ödül modeli kasıtlı olarak **tek kaynaklı**: EXP Zindanı'ndan yalnızca EXP, Altın Zindanı'ndan yalnızca altın çıkar — canavar, ekipman veya diğer kaynaklar düşmez. Oyuncu ihtiyacına göre net bir seçim yapar ("EXP'e ihtiyacım var" → EXP Zindanı'na gir).

MVP kapsamında 2 zindan türü (EXP, Altın), her biri 5 kat/1 dalga, günde 3+3 giriş hakkı yer alır. İleride (Tier 2+) farklı temalı/farklı kaynaklı ek zindan türleri eklenebilir (bkz. Open Questions).

## Player Fantasy

Oyuncu Zindan Saldırısı'nda **hızlı vur-kaç akıncısı** fantezisi yaşar — Keşif Alanı'nın "yavaş fetih" temposunun aksine, bu kısa, odaklı, günlük bir rutindir. "Bugün EXP'e mi altına mı ihtiyacım var?" kararı oturumun başlangıç noktasıdır.

**Günlük rutin tatmini**: 3+3 giriş hakkı, günlük check-in'i doğal olarak teşvik eder — "günlük saldırı haklarımı boşa harcamayayım" hissi. Bu, "Hep Bir Şey Var" pillar'ının günlük ritim ayağıdır.

**Net kaynak kontrolü**: Karma loot tablolarının aksine (Keşif Alanı gibi), oyuncu tam olarak ne kazanacağını bilerek girer — belirsizlik yok, sadece "ne kadar" belirsiz (kaç kat temizleyebilecek).

**Negatif fantezi (kaçınılacak)**: Zindan Saldırısı, Keşif Alanı'nın yerini almamalı — kısa, tamamlayıcı bir rutin olarak kalmalı. Günde 6 giriş (3+3) × 5 kat × ~30sn/kat ≈ 15 dakikalık toplam süre hedeflenir, bu "bir oturumda bitirilen görev listesi" hissi vermeli, ikinci bir ana döngü değil.

**Pillar bağlantısı**: "Hep Bir Şey Var" — günlük giriş hakları her gün yeni bir "yapılacak" yaratır. "Senin Tempon" — kısa, odaklı, oturuma sığan bir aktivite.

## Detailed Rules

### Core Rules

**Kural 1 — Zindan Türleri**

| Tür | Ödül Kaynağı | Kat Sayısı | Dalga/Kat |
|-----|---------------|-------------|-----------|
| EXP Zindanı | Yalnızca EXP (oyuncu) | 5 | 1 |
| Altın Zindanı | Yalnızca altın | 5 | 1 |

**Kural 2 — Giriş Hakkı**

| Parametre | Değer |
|-----------|-------|
| Günlük giriş hakkı (tür başına) | 3 |
| Toplam günlük giriş (2 tür) | 6 |
| Sıfırlanma | Her gün (yerel gece yarısı — kesin saat TBD, bkz. Tuning Knobs) |
| Enerji maliyeti | **Yok** — Keşif Alanı'nın enerji havuzundan tamamen bağımsız |
| Giriş hakkı devri | Kullanılmayan haklar ertesi güne devretmez |

**Kural 3 — Koşu (Run) Yapısı**

1. Oyuncu bir zindan türü seçip giriş hakkı harcar (1 giriş hakkı = 1 koşu).
2. Koşu, Kat 1'den başlar, sırayla Kat 5'e kadar ilerler — her kat 1 dalga, tek düşman grubu.
3. Bir kat kazanılınca hemen bir sonraki kata geçilir (Keşif Alanı'ndaki gibi dalgalar arası bekleme yoktur — kat geçişleri de aynı 1.5s geçiş mantığıyla anlık akar).
4. HP/durum efektleri katlar arası **korunur** (Keşif Alanı Kural 3 ile aynı prensip).
5. Herhangi bir katta kaybedilirse koşu **biter** — kalan katlar oynanmaz. O koşuda **önceden temizlenen katların ödülü kalıcıdır** (kaybedilmez).
6. 5. kat da temizlenirse koşu "Tam Temizleme" ile biter — ekstra bir bonus **yoktur** (MVP'de sade tutuluyor, bkz. Open Questions).
7. Koşu bitince (başarı ya da başarısızlıkla) oyuncu zindan seçim ekranına döner. Yeni bir koşu için yeni bir giriş hakkı gerekir.

**Kural 4 — Zorluk Ölçekleme**

Zindan Saldırısı'nın zorluğu, oyuncunun Keşif Alanı'ndaki **en yüksek temizlediği aşamaya** göre ölçeklenir — ayrı bir zorluk eğrisi icat edilmez, mevcut ilerleme yansıtılır:

`effective_floor = highest_cleared_kesif_alani_floor + zindan_kat_no`

`highest_cleared_kesif_alani_floor`, `kesif-alani.md`'deki `DungeonManager.Floors[]` durumundan okunur (en yüksek `Cleared` aşama numarası, hiç aşama temizlenmediyse 0).

Düşman statları, `kesif-alani.md` Formül 5 ile **aynı formülle** hesaplanır (ayrı bir formül icat edilmez):

```
HP  = floor(20 + effective_floor × 8)
ATK = floor(10 + effective_floor × 3)
DEF = floor(8  + effective_floor × 2)
SPD = 20 (sabit)
```

Boss dalgası çarpanı (×2.5) **uygulanmaz** — Zindan Saldırısı'nda boss/şampiyon kat ayrımı yoktur, 5 kat da aynı formülle, sadece `zindan_kat_no` arttıkça (1→5) `effective_floor` büyüdüğü için doğal olarak zorlaşır.

**Kural 5 — Ödül Hesabı**

Her temizlenen kat, kendi `effective_floor` değerine göre ödül verir (Formül 1/2). Koşu bitince (başarı ya da başarısızlıkla), o ana kadar temizlenen katların ödülleri **toplanıp** oyuncuya verilir.

## Formulas

### Formül 1: Altın Zindanı — Kat Başı Altın Ödülü

`gold_reward(zindan_kat) = floor(150 × effective_floor)`

`effective_floor = highest_cleared_kesif_alani_floor + zindan_kat` (Kural 4)

**Örnek**: `highest_cleared_kesif_alani_floor = 8`, zindan Kat 3 temizlendi → `effective_floor = 11` → `gold_reward = floor(150×11) = 1.650` altın (bu kata özel — koşu toplamı, temizlenen tüm katların toplamıdır).

### Formül 2: EXP Zindanı — Kat Başı EXP Ödülü

`exp_reward(zindan_kat) = floor(50 × effective_floor)`

Bu EXP **oyuncunun kendi seviyesine** uygulanır (`level-deneyim-sistemi.md`'deki oyuncu XP formülüyle aynı havuza eklenir). Pet XP'sine etkisi yoktur (bkz. Open Questions — pet'e de mi uygulansın tartışılmadı).

**Örnek**: `highest_cleared_kesif_alani_floor = 8`, zindan Kat 3 temizlendi → `effective_floor = 11` → `exp_reward = floor(50×11) = 550` EXP.

### Formül 3: Koşu Toplam Ödülü

`run_total = Σ(reward(zindan_kat)) için kat = 1..son_temizlenen_kat`

**Örnek** (Altın Zindanı, `highest_cleared_kesif_alani_floor=8`, oyuncu Kat 1-3'ü temizleyip Kat 4'te kaybetti):
- Kat 1: effective=9 → floor(150×9)=1.350
- Kat 2: effective=10 → floor(150×10)=1.500
- Kat 3: effective=11 → floor(150×11)=1.650
- **Koşu toplamı: 4.500 altın** (Kat 4-5 ödülü verilmez, koşu orada bitti)

## Edge Cases

- **If oyuncunun günlük giriş hakkı 0'sa**: İlgili zindan türüne giriş engellenir, "Bugünkü giriş hakların doldu — yarın tekrar dene" mesajı. Diğer zindan türü etkilenmez (haklar birbirinden bağımsız).

- **If oyuncu hiç Keşif Alanı aşaması temizlememişse (`highest_cleared_kesif_alani_floor = 0`)**: `effective_floor = zindan_kat_no` (1-5) — zindan hâlâ oynanabilir, sadece en düşük zorluk seviyesinde.

- **If oyuncu Kat 1'de kaybederse (hiç kat temizlemeden)**: Koşu biter, ödül verilmez (Σ boş küme = 0), giriş hakkı yine de harcanmıştır — geri iade edilmez.

- **If oyuncu koşu ortasında (ör. Kat 3'te) uygulamadan çıkarsa**: Koşu durumu kalıcılaştırılmaz — bu bir "oturum içi" akıştır, Keşif Alanı'nın kalıcı `FloorState`'inden farklı olarak zindan koşusu save/load kapsamında değildir. Oyuncu geri döndüğünde koşu kaybedilmiş sayılır, o ana kadarki ödül verilmez, giriş hakkı harcanmış kalır. *(Sert bir kural — kullanıcıyla netleştirilmeli, bkz. Open Questions.)*

- **If `highest_cleared_kesif_alani_floor` oyuncu Keşif Alanı'nda ilerledikçe artarsa (koşu sırasında değil, günler içinde)**: Zorluk formülü her yeni koşuda güncel değeri okur — önceki koşuların zorluğu etkilenmez, sadece gelecek koşular yeni zorlukla başlar.

- **If oyuncu her iki zindan türünü de aynı gün art arda oynarsa**: Giriş hakları bağımsızdır — EXP Zindanı'nı 3 kez, Altın Zindanı'nı 3 kez, toplam 6 koşu aynı gün yapılabilir.

## Dependencies

### Upstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Keşif Alanı Sistemi** | Sert | `highest_cleared_kesif_alani_floor` (en yüksek `Cleared` aşama) — zorluk girdisi | Olmadan zorluk hesaplanamaz |
| **Oyuncu Sınıf Sistemi** | Sert | Taban stat + pet/ekipman bonusu (Keşif Alanı Kural 6 ile aynı `BuildPlayer` mantığı) | Olmadan oyuncu birimi kurulamaz |
| **Savaş Sistemi** | Sert | Kat savaşı yürütme | Olmadan koşu çalışmaz |
| **Level / Deneyim Sistemi** | Sert (EXP Zindanı için) | Oyuncu XP havuzuna ekleme | Olmadan EXP Zindanı ödülü uygulanamaz |
| **Ekonomi** | Sert (Altın Zindanı için) | `AddGold()` | Olmadan Altın Zindanı ödülü uygulanamaz |
| **Kaydetme/Yükleme** | Sert | Günlük giriş hakkı sayacı + son sıfırlama zamanı | Olmadan giriş hakları kalıcı olmaz |

### Downstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Zindan Saldırısı UI** *(yazılmadı)* | Sert | Zindan seçim ekranı, giriş hakkı göstergesi, koşu sonuç ekranı | UI verileri bu sistemden gelir |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|-----------------|---------------|--------------|
| `daily_entries_per_type` | 3 | 1–5 | Günlük rutin çok uzun sürer | Zindan anlamsız hissettirir |
| `floors_per_run` | 5 | 3–8 | Koşu çok uzun, "hızlı vur-kaç" hissi kaybolur | Koşu çok kısa, ödül azlığı hissedilir |
| `gold_reward_coefficient` | 150 | 100–250 | Altın Zindanı, Keşif Alanı'ndan daha karlı hale gelir → ana döngüyü gölgeler | Altın Zindanı'na girmek anlamsız |
| `exp_reward_coefficient` | 50 | 30–100 | EXP Zindanı seviye atlamayı çok hızlandırır | EXP Zindanı'na girmek anlamsız |
| `daily_reset_time` | Yerel gece yarısı (TBD — UTC mi yerel mi netleşmedi) | — | — | — |

**Etkileşim Uyarısı**: `gold_reward_coefficient` × Keşif Alanı'nın Formül 1 katsayısı (100) birlikte karşılaştırılmalı — Zindan Saldırısı'nın altın verimi Keşif Alanı'ndan belirgin şekilde düşük kalmalı (bu bir "ek/tamamlayıcı" rutin, ana altın kaynağı değil), aksi halde oyuncular Keşif Alanı'nı bırakıp yalnızca Zindan Saldırısı farm eder.

## Acceptance Criteria

1. **GIVEN** oyuncunun EXP Zindanı günlük giriş hakkı 3/3, **WHEN** bir koşu başlatırsa, **THEN** hak 2/3'e düşer, Altın Zindanı hakları etkilenmez (3/3 kalır).

2. **GIVEN** oyuncunun EXP Zindanı günlük giriş hakkı 0, **WHEN** giriş denerse, **THEN** giriş engellenir, "Bugünkü giriş hakların doldu" mesajı.

3. **GIVEN** `highest_cleared_kesif_alani_floor = 8`, **WHEN** zindan Kat 3'e girilirse, **THEN** `effective_floor = 11`, düşman HP=`floor(20+11×8)=108`.

4. **GIVEN** Altın Zindanı, `highest_cleared_kesif_alani_floor=8`, oyuncu Kat 1-3'ü temizleyip Kat 4'te kaybeder, **WHEN** koşu biterse, **THEN** toplam ödül = 1.350+1.500+1.650 = **4.500 altın**, Kat 4-5 ödülü verilmez.

5. **GIVEN** oyuncu Kat 1'de kaybeder, **WHEN** koşu biterse, **THEN** ödül = 0, giriş hakkı yine de harcanmıştır (iade edilmez).

6. **GIVEN** oyuncu hiç Keşif Alanı aşaması temizlememiş (`highest_cleared_kesif_alani_floor=0`), **WHEN** zindan Kat 1'e girerse, **THEN** `effective_floor = 1`, en düşük zorlukta oynanabilir.

7. **GIVEN** EXP Zindanı, oyuncu tüm 5 katı temizler, **WHEN** koşu biterse, **THEN** Formül 3'e göre 5 katın toplam EXP'i oyuncunun seviyesine eklenir, ekstra "tam temizleme" bonusu verilmez.

8. **GIVEN** gün değişir (günlük sıfırlama zamanı geçer), **WHEN** oyuncu oyunu açarsa, **THEN** her iki zindan türünün giriş hakları 3/3'e sıfırlanır.

## Open Questions

1. **Koşu ortasında uygulamadan çıkma davranışı**: Şu anki taslak kural "koşu kalıcılaştırılmaz, kaybedilmiş sayılır" diyor — bu sert bir kayıp riski. Kullanıcıyla netleştirilmeli: koşu ortasında bırakılıp sonra devam edilebilmeli mi (Keşif Alanı'nın kalıcı aşama durumuna benzer şekilde), yoksa mevcut "tek oturumluk koşu" modeli mi korunmalı?

2. **EXP Zindanı'nın pet'e etkisi**: Formül 2, EXP'yi yalnızca oyuncuya uyguluyor. Pet'in kendi seviyelenmesi (`level-deneyim-sistemi.md`) bu sistemden hiç faydalanmıyor — bu kasıtlı mı, yoksa ileride "Pet EXP Zindanı" gibi ayrı bir 3. tür mü eklenmeli?

3. **Tam temizleme bonusu**: MVP'de 5 katın hepsi temizlenince ekstra bir ödül yok — Tier 2'de bir "mükemmel koşu" bonusu eklenmeli mi?

4. **Ek zindan türleri (Tier 2+)**: Kullanıcı "zindanlar farklı konseptlerde olacak" dedi — prototype'ta yalnızca EXP + Altın var. Hangi ek türler (ör. Ekipman Zindanı, Evrim Taşı Zindanı) ve ne zaman eklenecek? → Sonraki tasarım oturumunda.

5. **Günlük sıfırlama saati**: Yerel gece yarısı mı, sabit bir UTC saati mi? Diğer günlük sistemlerle (varsa) tutarlı olmalı.
