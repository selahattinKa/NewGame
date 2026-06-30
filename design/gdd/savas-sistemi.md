# Savaş Sistemi

> **Status**: Designed
> **Author**: user + game-designer, gameplay-programmer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon, Cömert Zindan

## Overview

**Savaş Sistemi**, oyundaki tüm savaş etkileşimlerini yöneten tur bazlı cooldown savaş döngüsüdür. Savaşa katılan birimler **3 tanedir**: Oyuncu (seçilmiş sınıfı ve ekipmanıyla), Aktif Pet ve Düşman. Her birimin SPD stat'ına göre belirlenen tur sırasında aksiyon alır.

**Oyuncu ana savaşçıdır.** HP'si vardır, hasara maruz kalır ve yenilgi koşulu oyuncunun HP'sinin sıfırlanmasıdır. Düşman oyuncuyu hedef alır. Pet ikincil saldırgandır — her turda bağımsız saldırır ve enerji bazlı yeteneğini kullanır, oyuncunun yanında savaşır. Defeat = Oyuncu HP = 0. Victory = Düşman HP = 0.

Sistem iki mod sunar: **Komutan Modu** (oyuncu yetenek zamanlamasını manuel yönetir, +30% ATK bonusu kazanır) ve **Otofarm Modu** (tüm kararlar otomatik alınır). Her iki modda da her savaş sonu loot düşer — "Cömert Zindan" sütunu gereği eli boş dönen savaş yoktur.

MVP kapsamında tur döngüsü, iki mod, SPD sıralama, pet enerji sistemi, oyuncu sınıf cooldown sistemi, iksir kullanımı, DoT (Yanma/Zehir), temel status efektleri (Sersemletme, Kalkan, DEF Kırma, ATK Zayıflatma) ve savaş sonu ödül dağıtımı yer alır.

## Player Fantasy

Oyuncu savaş sisteminde **güçlü kahraman** fantezisi yaşar. Çekirdek an, kendi sınıf yetenekleriyle düşmanı dövmek — Savaşçı yakın dövüşte eziyorken Büyücü sihir patlamalarıyla hasar yağdırıyor. Pet arkadan ikincil saldırılarla destek veriyor — "ben savaşıyorum, yanımda güçlü bir yardımcım var" hissi.

**Büyüme tatmini**: Aynı düşmanı geçen hafta 8 turda yenerken bu hafta 4 turda yenmek — ekipman takılmış, sınıf leveli artmış, hasar sayıları büyümüş. Pet de daha sert vuruyor. "Ne kadar güçlendik" anı savaşta en belirgin şekilde hissedilir.

**Otofarm fantezisi**: Oyuncu ve pet birlikte otomatik savaşıyor. Geri döndüğünde ödüller seni bekliyor. Ama sen manuel oynarsan %30-40 daha hızlı ve güçlü.

**Negatif fantazi (kaçınılacak)**: Savaş "bekle ve izle" monotonluğuna düşmemeli. Komutan modunda anlamlı karar anları olmalı. Savaşlar çok uzun sürmemeli — 30 saniyede bir savaş (5-8 tur) ideal. Kayıp cezalandırıcı olmamalı — enerji harcanmaz, sadece o aşama loot'u alınamaz.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Savaş Birimleri**

| Birim | Rol | HP | ATK | DEF | SPD | Özellik |
|-------|-----|-----|-----|-----|-----|---------|
| **Oyuncu** | Ana savaşçı | Var (sınıf bazlı) | Var (sınıf + ekipman) | Var (ekipman ağırlıklı) | Var (sınıf + ekipman) | Sınıf yetenekleri, iksir kullanımı |
| **Aktif Pet** | İkincil saldırgan | Var (pet stats) | Var (pet stats + ekipman) | — | Var (pet stats) | Her turda saldırı + enerji yeteneği |
| **Düşman** | Hedef / rakip | Var (SG bazlı) | Var (SG bazlı) | Var (SG bazlı) | Var (SG bazlı) | AI pattern, oyuncuyu hedef alır |

- Düşman **her zaman oyuncuyu hedef alır**.
- Pet hasar almaz (düşmanın hedef aldığı birim yalnızca oyuncu).
- Savaşa girildiğinde oyuncunun HP'si tam doluysa başlar (önceki savaştan kalıntı HP taşınmaz).

**Kural 2 — Savaş Modu Seçimi**

Savaş başlamadan önce oyuncu mod seçer (Keşif Alanı "SAVAŞ" butonuna basılınca):

| Mod | ATK Bonusu | Kontrol | Kullanım |
|-----|-----------|---------|---------|
| **Komutan Modu** | +30% Oyuncu ATK | Manuel — oyuncu yetenek seçer | Aktif oynanış, maksimum güç |
| **Otofarm Modu** | Yok | Otomatik — öncelik sırasına göre | AFK farming |

Bu fark UI'da gösterilmez — Otofarm modunda "−30% ATK" debuff ikonu çıkmaz. Hasar sayıları doğal olarak daha düşük akar.

**Kural 3 — Tur Sırası (Turn Order)**

Her tur başında tüm aktif birimler SPD değerine göre büyükten küçüğe sıralanır.

```
entities = [Oyuncu, Pet, Düşman]
turn_order = entities.sort(by: SPD, descending: true)
// Eşit SPD: Oyuncu > Pet > Düşman (tiebreak sırası)
```

- Tur sırası her tur başında yeniden hesaplanır (SPD değiştiyse güncellenir).
- Bir birim HP = 0 olursa o sıradaki kalan aksiyonları iptal edilir.

**Kural 4 — Oyuncu Aksiyonları**

Oyuncu'nun tur geldiğinde alabileceği aksiyonlar:

| Aksiyon | Koşul | Etki |
|---------|-------|------|
| **Sınıf Yeteneği** (slot 0–3) | CD = 0 | Yeteneğe özgü hasar/efekt; CD başlar |
| **Normal Saldırı** | Her zaman (yedek) | Temel ATK hasarı, CD yok |

- Komutan Modunda oyuncu seçer. Seçilmezse 5 saniye sonra slot 3→2→1→0 önceliğiyle otomatik tetiklenir ("AUTO-SKIP").
- Otofarm Modunda: CD = 0 olan en yüksek slottan tetiklenir; tüm CD doluysa Normal Saldırı.

**Kural 5 — Pet Aksiyonları**

Pet'in tur geldiğinde:

| Aksiyon | Koşul | Etki |
|---------|-------|------|
| **Temel Saldırı** | Her zaman | Pet ATK bazlı hasar |
| **Enerji Yeteneği** | Enerji = 100 | Özel etki + enerji sıfırlanır |

- Pet her turda 25 enerji kazanır (normal saldırı yaparken de).
- 4. turda enerji = 100 → otomatik yetenek. Pet yeteneği oyuncu kontrolünde değil, otomatik ateşlenir.
- Pet hasarı Komutan Modunda artırılmaz — yalnızca oyuncu ATK'ı etkilenir.

**Kural 6 — Düşman Aksiyonları**

Düşmanın tur geldiğinde:

- Düşman AI'sı (Düşman AI GDD) pattern seçer.
- Hedef: **Her zaman oyuncu** (pet'i hedef almaz).
- Hasar `enemy_ATK` ve oyuncunun `effective_DEF` ile hesaplanır (Hasar Hesaplama GDD).

**Kural 7 — İksir Kullanımı**

- İksir butonu savaş UI'ında her zaman görünür (serbest aksiyon — oyuncunun turunu tüketmez).
- Basıldığında Pot Panel açılır: envanterdeki iksirler listelenir.
- Seçilen iksir oyuncunun HP'sini anında iyileştirir.
- Otofarm modunda: Oyuncu HP ≤ %25 iken Büyük İksir → yoksa Küçük İksir otomatik kullanılır.

**Kural 8 — Kazanma / Yenilme Koşulları**

| Koşul | Sonuç |
|-------|-------|
| Düşman HP = 0 | Zafer — Loot + EXP verilir, enerji harcanır |
| Oyuncu HP = 0 | Yenilgi — Enerji harcanmaz, loot alınmaz |
| Oyuncu çekilirse (Retreat) | Çekilme — Enerji harcanmaz |

- Pet HP = 0 olursa: Pet "devre dışı" kalır (savaştan çekilir), battle devam eder — oyuncu yalnız savaşır. Yenilgi koşulu tetiklenmez.
- Savaş bittiğinde oyuncu HP tam dolmaz — bir sonraki savaşa kalıntı HP ile girer. İksir veya dinlenme gerekir.

**Kural 9 — Oyuncu HP Yenileme (Savaş Dışı)**

- Keşif Alanı harita ekranına dönünce HP tam dolmaz.
- HP yenileme yolları: İksir kullanımı (savaş dışında kullanılamaz — yalnızca Dükkan'da satın alınır, savaşta kullanılır), savaşlar arası oturum yenilemesi (oyuncu uygulamayı kapayıp açarsa tam HP ile başlar — mobile sessiz dinlenme).
- Normal aşama savaşları sonrası: HP otomatik %50 yenilenir (yalnızca zafer).
- Mini Boss ve Alan Patronu sonrası: HP tam yenilenir (zafer).

**Kural 10 — Komutan Modu ATK Bonusu (Görünmez)**

```
Komutan Modu: oyuncu_ATK_etkin = floor(base_ATK × 1.30)
Otofarm Modu: oyuncu_ATK_etkin = base_ATK
```

Bu fark hiçbir UI elementinde gösterilmez — "debuff" veya "bonus" ikonu yok. Hasar sayıları doğal olarak farklı akar.

**Kural 11 — Kazanma Bonus Koşulu (Auto-Skip Penaltısı)**

Komutan Modunda oyuncu hiç manuel seçim yapmadan (her aksiyon auto-skip ile tetiklendiyse) tam ödülü alır — penaltı yok. Auto-skip, oyuncunun geç dokunmasından kaynaklanır, oyundan ayrılmaz.

### States and Transitions

```
[Keşif Alanı Haritası]
    └─ "SAVAŞ" butonu → Mod Seçim Ekranı (veya doğrudan savaşa)
        └─ Komutan / Otofarm seçimi → [Savaş Ekranı]

[Savaş Ekranı — Tur Döngüsü]
    Oyuncu Turu:
        ├─ Yetenek seç (Komutan) → hasar / efekt → CD başlar
        ├─ Auto-skip 5s (Komutan) → en hazır slot tetiklenir
        └─ Otofarm → öncelik sırasıyla tetiklenir

    Pet Turu:
        ├─ Temel saldırı → pet ATK hasarı
        └─ Enerji = 100 → Yetenek → enerji sıfır

    Düşman Turu:
        └─ AI pattern → oyuncuya hasar

    İksir (serbest aksiyon):
        └─ Her an basılabilir → Pot Panel → seç → HP iyileşir

[Savaş Sonu]
    ├─ Düşman HP = 0 → Zafer Overlay (loot + EXP)
    ├─ Oyuncu HP = 0 → Yenilgi Overlay ("Enerji harcanmadı")
    └─ Retreat → Çekilme Onay Dialog → Haritaya dön
```

## Formulas

### Formül 1: Tur Sırası

```
turn_order = sort([oyuncu.SPD, pet.SPD, enemy.SPD], descending)
// Eşitlik: Oyuncu > Pet > Düşman
```

### Formül 2: Oyuncu Hasar (Sınıf Yeteneği veya Normal Saldırı)

```
// Oyuncu ATK (ekipman dahil — Ekipman Sistemi GDD'den)
effective_player_ATK = base_class_ATK
    + silah_ATK
    + eldiven_ATK
    + aksesuar_ATK_toplamı   // yüzükler + küpeler + kolye

// Komutan Modu uygulama:
if komutan_modu:
    attacking_ATK = floor(effective_player_ATK × 1.30)
else:
    attacking_ATK = effective_player_ATK

// Hasar Hesaplama GDD formülü:
raw_damage = attacking_ATK × yetenek_carpani   // normal saldırı: çarpan = 1.0
damage_reduction = floor(enemy.DEF × 0.50)
base_damage = max(1, raw_damage - damage_reduction)
final_damage = floor(base_damage × element_multiplier)  // prototipte 1.0
```

### Formül 3: Pet Hasar

```
effective_pet_ATK = pet.base_ATK + pet_silah_ATK + pet_aksesuar_ATK
raw_pet_damage = effective_pet_ATK
damage_reduction = floor(enemy.DEF × 0.50)
pet_damage = max(1, raw_pet_damage - damage_reduction)
```

### Formül 4: Düşman Hasarı (Oyuncuya)

```
// Düşman ATK, SG bazlı (Keşif Alanı + rubber-band ile adjust edilmiş)
effective_enemy_ATK = enemy.base_ATK × rubber_band_factor

effective_player_DEF = base_class_DEF
    + kask_DEF + zirh_DEF + pantalon_DEF
    + eldiven_DEF + bot_DEF
    + aksesuar_DEF_toplamı

damage_reduction = floor(effective_player_DEF × 0.50)
enemy_damage = max(1, effective_enemy_ATK - damage_reduction)
new_player_HP = player.HP - enemy_damage
```

### Formül 5: Pet Enerji Döngüsü

```
pet.energy += 25  // her tur, temel saldırı veya bekleme
if pet.energy >= 100:
    trigger_pet_ability()
    pet.energy = 0
```

### Formül 6: HP Yenileme (Savaş Arası)

```
// Normal aşama zaferi:
player.HP = min(player.max_HP, player.HP + floor(player.max_HP × 0.50))

// Mini Boss / Alan Patronu zaferi:
player.HP = player.max_HP

// Yenilgi / Çekilme:
player.HP değişmez
```

### Formül 7: İksir İyileşmesi

```
// (Ekipman Sistemi GDD ile aynı — oyuncu HP'sine uygulanır)
heal_small = floor(player.max_HP × 0.30)
heal_large = floor(player.max_HP × 0.70)
new_HP = min(player.HP + heal, player.max_HP)
```

**Örnek Savaş (Warrior sınıfı, D tier Silah):**
- Player: HP=250, base_ATK=40, Silah +12 → effective_ATK=52
- Komutan Modu: attacking_ATK = floor(52 × 1.30) = 67
- Enemy DEF=20 → damage_reduction=10 → final_damage=57
- Pet ATK=25 → pet_damage=15 (düşman DEF=20, red=10 → 15)
- Tur toplam hasar: 57 (oyuncu) + 15 (pet) = 72

## Edge Cases

- **If pet HP = 0 olursa**: Savaş devam eder, pet sırası atlanır. Oyuncu yalnız dövüşür. Yenilgi koşulu tetiklenmez. Savaş sonunda pet HP restore edilmez — bir sonraki savaşa petsize girilebilir. (Öneri: Dükkan'da pet için de "iksir" tipi eklenerek pet iyileştirilir — gelecek versiyon.)

- **If oyuncu HP sıfıra inerse pet hâlâ saldırı sırasındaysa**: Oyuncu HP = 0 anında savaş durur. Pet saldırısı beklemede olsa bile yenilgi ekranı açılır.

- **If tüm skill CD'leri doluysa ve 5 saniye geçerse (Komutan Modu)**: Slot 3 → 2 → 1 → 0 sırasıyla CD = 0 olan ilk slot tetiklenir. CD = 0 olan yoksa Normal Saldırı tetiklenir. "AUTO-SKIP" etiketi 0.5 saniye ekranda belirir.

- **If Otofarm modunda tüm iksirler bittiyse**: Oto-iksir tetiklemez, HP ≤ %25'e düşse bile devam eder. Oyuncu yenilirse loot yok, enerji harcanmaz — sessiz yenilgi.

- **If düşman öldürücü darbe vurunca oyuncu HP'si tam sıfıra inerken iksir basılırsa**: Hasar ve iksir aynı "frame"de gelirse: hasar önce işlenir, HP = 0 → yenilgi tetiklenir. İksir iptal olur, sarf edilmez.

- **If pet devre dışıyken (HP = 0) Otofarm devam ederse**: Pet tur sırası atlanır. Oyuncu tüm enerji barı olmadan yavaş dövüşür. Uzun sürer, yenilgi riski artar.

- **If Retreat dialog açıkken düşman öldürücü darbe vurursa**: Dialog kapatılır, yenilgi ekranı açılır.

- **If savaşta aynı tur içinde oyuncu ve düşman aynı anda HP = 0 olursa**: Oyuncu turu önce işlenir — eğer oyuncu son darbesini vurdu ve düşman ölüyorsa Zafer. Düşman aynı turda oyuncuyu öldürdüyse ancak oyuncu daha erken tura sahipse yine Zafer. Tur sırası tie-break belirler.

- **If Normal Savaş aşaması sonrası HP = 15/250 iken devam edilirse**: Bir sonraki savaşa HP = min(15 + floor(250×0.50), 250) = min(140, 250) = 140 ile girilir (zaferde +50% restore). İksir basılmadıysa 140 ile başlanır.

## Dependencies

### Upstream

| Sistem | Veri |
|--------|------|
| **Oyuncu Sınıf Sistemi** | `base_HP`, `base_ATK`, `base_DEF`, `base_SPD`, skill slot tanımları, CD değerleri |
| **Ekipman Sistemi** | `effective_player_ATK`, `effective_player_DEF`, `effective_player_HP` (ekipman bonusları dahil) |
| **Hasar Hesaplama** | Hasar formülü, element_multiplier (prototipte 1.0), DoT, status efektleri |
| **Düşman AI** | Enemy pattern seçimi, hedef belirleme |
| **Pet/Canavar Veritabanı** | Pet `base_ATK`, `base_SPD`, enerji yeteneği tanımı |
| **Ekonomi** | Enerji harcama/iade |
| **Loot / Ödül Sistemi** | Zafer sonrası loot dağıtımı |

### Downstream

| Sistem | Etki |
|--------|------|
| **Savaş UI** | Tur gösterimi, HP barları, skill butonları, iksir paneli |
| **Keşif Alanı** | Savaş sonucu (zafer/yenilgi/çekilme) → aşama kilidi güncelleme |
| **Loot Sistemi** | Zafer → loot trigger |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `commander_atk_bonus` | 0.30 | 0.15–0.45 | Manuel çok baskın, oto değersiz | Manuel avantajı hissedilmez |
| `auto_skip_timer` | 5s | 3s–8s | Sabırsız oyuncular için yavaş | Yanlışlıkla tetikleme |
| `pet_energy_per_turn` | 25 | 20–33 | Pet yeteneği çok sık (her 3 tur) | Pet yeteneği nadiren gelir |
| `normal_stage_hp_restore` | 0.50 | 0.30–0.75 | İksire gerek kalmaz | HP management çok stresli |
| `auto_pot_threshold` | 0.25 | 0.15–0.35 | Oto-mod iksiri erken harcıyor | Pet sık ölüyor |
| `damage_def_reduction_rate` | 0.50 | 0.40–0.60 | Savunma çok önemli, ATK anlamsız | Savunma hiç önemli değil |
| `tiebreak_order` | Oyuncu > Pet > Düşman | — | — | — |

## Acceptance Criteria

1. **GIVEN** SPD: Oyuncu=50, Pet=65, Düşman=40, **WHEN** tur sırası hesaplanırsa, **THEN** sıra: Pet → Oyuncu → Düşman.

2. **GIVEN** Komutan Modu, Oyuncu base_ATK=40, Silah+12 → effective=52, **WHEN** saldırı yapılırsa, **THEN** `attacking_ATK = floor(52 × 1.30) = 67`.

3. **GIVEN** Otofarm Modu, aynı oyuncu, **WHEN** saldırı yapılırsa, **THEN** `attacking_ATK = 52` (bonus yok).

4. **GIVEN** Düşman ATK=30, oyuncu effective_DEF=20, **WHEN** düşman saldırırsa, **THEN** `enemy_damage = max(1, 30 - floor(20×0.50)) = max(1, 20) = 20`.

5. **GIVEN** Oyuncu HP = 20/250, Büyük İksir basıldı, **WHEN** uygulanırsa, **THEN** `new_HP = min(20 + floor(250×0.70), 250) = min(195, 250) = 195`.

6. **GIVEN** Oyuncu HP = 0, **WHEN** kontrol edilirse, **THEN** savaş anında durur, Yenilgi Overlay açılır, "Enerji harcanmadı" gösterilir.

7. **GIVEN** Düşman HP = 0, **WHEN** kontrol edilirse, **THEN** Zafer Overlay açılır, loot ve EXP verilir, enerji 1 harcanır.

8. **GIVEN** Pet HP = 0 iken (pet devre dışı), **WHEN** sıra Pet'e gelirse, **THEN** Pet turu atlanır — oyuncu ve düşman dövüşmeye devam eder.

9. **GIVEN** Komutan Modu'nda 5 saniye boyunca aksiyon seçilmezse, **WHEN** timer dolunca, **THEN** CD = 0 olan en yüksek slot tetiklenir, "AUTO-SKIP" etiketi 0.5s gösterilir.

10. **GIVEN** Otofarm Modu'nda tüm CD'ler doluysa, **WHEN** oyuncu turu gelirse, **THEN** Normal Saldırı tetiklenir.

11. **GIVEN** Pet enerji = 75, tur sonu, **WHEN** pet temel saldırı yaparsa, **THEN** enerji = 100 → yetenek otomatik tetiklenir, enerji = 0.

12. **GIVEN** Normal aşama zaferi sonrası oyuncu HP = 80/300, **WHEN** savaş biter, **THEN** `new_HP = min(80 + floor(300×0.50), 300) = min(230, 300) = 230`.

13. **GIVEN** Mini Boss zaferi, oyuncu HP = 50/300, **WHEN** savaş biter, **THEN** `new_HP = 300` (tam yenileme).

14. **GIVEN** Komutan Modu'nda ATK bonusu +30% aktif, **WHEN** savaş UI'ına bakılırsa, **THEN** "Komutan +30%" debuff/buff ikonu görünmez — sayılar doğal yüksek akar.

15. **GIVEN** Düşman oyuncuya saldırırken oyuncu aynı turda düşmanı öldürüyorsa (oyuncu daha erken tur sırasında), **WHEN** çözümlenirse, **THEN** Zafer tetiklenir — düşmanın sonraki saldırısı iptal edilir.

16. **GIVEN** Retreat dialog açıkken düşman saldırır ve oyuncu HP = 0 olursa, **WHEN** çözümlenirse, **THEN** Dialog kapanır, Yenilgi Overlay açılır.

17. **GIVEN** Otofarm Modu'nda oyuncu HP ≤ %25 ve Büyük İksir mevcut, **WHEN** tur başında kontrol edilirse, **THEN** Büyük İksir otomatik kullanılır.

18. **GIVEN** Oyuncu HP = 0 anı ile iksir kullanımı aynı "frame"de gelirse, **WHEN** çözümlenirse, **THEN** Hasar önce işlenir: HP = 0 → Yenilgi. İksir sarf edilmez.
