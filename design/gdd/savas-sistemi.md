# Savaş Sistemi

> **Status**: Revised
> **Author**: user + game-designer, gameplay-programmer, systems-designer
> **Last Updated**: 2026-06-30
> **Implements Pillar**: Güç Hisset, Senin Tempon, Cömert Zindan

## Overview

**Savaş Sistemi**, oyundaki tüm savaş etkileşimlerini yöneten tur bazlı cooldown savaş döngüsüdür. Savaşa katılan birimler **3 tanedir**: Oyuncu (seçilmiş sınıfıyla), Aktif Pet ve Düşman. Her birimin SPD stat'ına göre belirlenen tur sırasında aksiyon alır. Oyuncu sınıf yeteneklerini kullanır (4 slot, cooldown bazlı), pet kendi aktif yeteneğini kullanır (enerji bazlı, her 4 turda bir). Düşman yalnızca peti hedef alır — oyuncu hasar almaz. Defeat = pet HP = 0. Victory = düşman HP = 0.

Sistem iki mod sunar: **Komutan Modu** (oyuncu yetenek zamanlamasını kontrol eder, +10% ATK bonusu kazanır) ve **Otofarm Modu** (tüm kararlar otomatik alınır). Her iki modda da her savaş sonu loot düşer — "Cömert Zindan" sütunu gereği eli boş dönen savaş yoktur.

MVP kapsamında tur döngüsü, iki mod, SPD sıralama, pet enerji sistemi, oyuncu sınıf cooldown sistemi, DoT (Yanma/Zehir), temel status efektleri (Sersemletme, Kalkan, DEF Kırma, ATK Zayıflatma) ve savaş sonu ödül dağıtımı yer alır.

## Player Fantasy

Oyuncu savaş sisteminde **güçlü komutan** fantezisi yaşar. Çekirdek an, petinin otomatik savaştığını izlerken doğru anda yetenek butonuna basıp düşmanı element avantajlı bir saldırıyla devirmektir — "benim zamanlamam fark yarattı" hissi. Komutan modunda oyuncu petinin generalıdır: peti ne zaman saldıracağını yönetmez (otomatik), ama kritik anlarda müdahale eder — düşük HP'li pete iyileştirme gönderir, boss'un güçlü saldırısı öncesi savunma yeteneği tetikler, element avantajlı saldırı zamanlar.

**Büyüme tatmini**: Aynı düşmanı geçen hafta 8 turda yenerken bu hafta 4 turda yenmek — hasar sayıları büyümüş, pet HP barı daha az kıpırdıyor. "Ne kadar güçlendik" anı savaş sisteminde en belirgin şekilde hissedilir.

**Otofarm fantezisi**: Petin senin yerine savaşıyor. Geri döndüğünde ödüller seni bekliyor. "İmparatorluğum bensiz bile çalışıyor" — ama sen müdahale edince %20-30 daha iyi çalışıyor.

**Negatif fantazi (kaçınılacak)**: Savaş asla "bekle ve izle" monotonluğuna düşmemeli. Komutan modunda anlamlı müdahale anları olmalı. Savaşlar çok uzun sürmemeli — 30 saniyede bir savaş (5-8 tur) ideal. Kayıp bile cezalandırıcı olmamalı — kaybetsen bile enerji harcanmaz, sadece o aşamadan loot alamazsın.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Savaş Akışı (Combat Flow)**

Her savaş üç aşamadan oluşur:

```
Savaş Öncesi (Pre-Combat) → Aktif Savaş (Combat) → Savaş Sonu (Post-Combat)
```

1. **Savaş Öncesi**: Aktif pet yüklenir (`GetActivePet()`). Düşman Keşif Alanı'ndan alınır (`GetStageEnemy()`). Mod seçimi yapılır (varsayılan: son kullanılan mod). Tüm birimler `current_hp = max_hp`, `energy = 0` ile başlar. Oyuncu'nun cooldown'ları sıfırlanır.

2. **Aktif Savaş**: Tur döngüsü başlar (Kural 2-3). Döngü, düşman savaş dışı (kazanma) veya pet savaş dışı (kaybetme) olana kadar devam eder. Oyuncu istediği zaman çekilebilir.

3. **Savaş Sonu**:

| Sonuç | Loot | Enerji Maliyeti | Ceza | HP |
|-------|------|-----------------|------|----|
| **Kazanma** | Tam ödül (Loot/Ödül Sistemi) | Harcanır | Yok | Pet tam iyileşir |
| **Kaybetme** | Yok | Harcanmaz | Yok | Pet tam iyileşir |
| **Çekilme** | Yok | Harcanmaz | Yok | Pet tam iyileşir |

**Kural 2 — Tur Sıralama Sistemi (SPD-Based Turn Order)**

Her raunt'ta 3 birim SPD stat'ına göre azalan sırayla hareket eder:

1. **Oyuncu SPD** = sınıfa göre sabit değer (sınıf sistemi GDD'sinde tanımlı)
2. **Pet SPD** = pet'in effective SPD stat'ı
3. **Düşman SPD** = düşmanın SPD stat'ı

Sıralama kuralları:
- En yüksek SPD → ilk sıra
- SPD eşitliğinde: Oyuncu → Pet → Düşman öncelik sırası
- Raunt başında savaş dışı birimler sıradan çıkarılır

**Oyuncu'nun turu**: Yetenek kullanır. Oyuncu hasar almaz — düşman yalnızca peti hedef alır.
**Pet'in turu**: Saldırı yapar veya pet yeteneği kullanır.
**Düşman'ın turu**: Peti saldırır. (Oyuncu immune.)

**Kural 3 — Birim Tur Fazları**

Her birimin turu 5 fazda işler:

```
1. DoT Tick          → Aktif DoT'lar uygulanır (Yanma/Zehir — Kural 10)
2. Pasif Rejenerasyon → max(1, floor(max_hp × 0.02)) HP iyileşme (pet ve düşman için)
3. Enerji Birikimi   → energy += energy_per_turn (25) — yalnızca pet için
4. Aksiyon Seçimi    → Aksiyon belirlenir (mod'a ve birime göre)
5. Aksiyon Yürütme   → Hasar/iyileşme/buff uygulama + animasyon
```

- Oyuncu turu için Faz 2 ve 3 atlanır (oyuncunun HP ve enerjisi yoktur)
- Faz 1'de birim HP=0'a düşerse savaş dışı kalır, kalan fazlar atlanır
- Faz 4: Oyuncu → yetenek slot seçimi. Pet → saldırı/yetenek. Düşman → AI kararı. Sersemletme aktifse Faz 4 atlanır (oyuncu ve düşman için — pet için de uygulanabilir)
- Cooldown sayacı (oyuncu sınıf yetenekleri) Faz 5 sonunda 1 azalır

**Kural 4 — Komutan Modu (Commander Mode)**

Oyuncunun aktif katılımla savaşa müdahale ettiği mod.

**Flat ATK Bonusu**: Pete +%30 ATK:
`commander_ATK = floor(effective_ATK × 1.30)`

DEF, SPD, HP'ye bonus yok — sadece ATK. Mod değiştirilirse bonus bir sonraki turdan düşer.

**Oyuncu Müdahale Noktaları**:

| Müdahale | Mekanik | Verimlilik Etkisi |
|----------|---------|-------------------|
| **Yetenek Zamanlaması** | Yetenek butonuna basarak optimal anda kullan | Boss saldırısı öncesi savunma, düşük HP'de iyileştirme, hasar penceresinde güçlü saldırı |
| **Savaş Hızı** | 1x / 2x / 3x toggle | Hesaplamaya etki yok, animasyon hızı |

**Yetenek bekleme**: Slot 1-3 CD dolunca buton aktifleşir. Oyuncu butona basmadıkça yetenek kullanılmaz — Slot 0 (CD0) otomatik devreye girer. Oyuncu yeteneği stratejik olarak "doğru an" için saklayabilir.

**Kural 5 — Otofarm Modu (Auto-Farm Mode)**

Tüm kararlar otomatik, oyuncu müdahalesi yok.

**ATK Bonusu**: Yok. Peti effective_ATK kullanır. Bu fark UI'da gösterilmez — oyuncu bir "debuff" görmez, savaş sayıları doğal olarak daha düşük akar.

**Otomatik Yetenek Kullanımı (Pet)**: Energy=100 olduğu anda yetenek hemen kullanılır.

**Otomatik Slot Önceliği (Oyuncu)**: Slot 3 açıksa → Slot 3. Slot 2 → Slot 1 → Slot 0.

**Verimlilik Farkı (Komutan vs Otofarm)**:

| Fark Kaynağı | Komutan Avantajı | Tahmini Etki |
|-------------|------------------|-------------|
| Flat +30% ATK bonusu | Her pet saldırısında | +30% |
| Yetenek zamanlaması | Optimal an bekleme | +10-20% |
| **Toplam** | | **~40-50%** |

**Kural 6 — Pet Yetenek Sistemi (MVP)**

Pet arketipine göre 1 aktif yetenek taşır.

**Enerji Mekanizması**:
- Savaş başlangıcı: `energy = 0`
- Her tur başı (pet turu): `energy += energy_per_turn` (25)
- Yetenek eşiği: `energy >= energy_threshold` (100)
- Kullanım sonrası: `energy = 0`
- Enerji 100'ü aşmaz

**Arketip Yetenekleri**:

| Arketip | Yetenek Adı | Etki | Hedef | Çarpan/Oran | Hasar Türü |
|---------|-------------|------|-------|-------------|-----------|
| **Saldırgan** | Güçlü Vuruş | Yüksek hasar | Düşman | ATK × 2.0 | Fiziksel (`defense_reduction_factor=2`) |
| **Tank** | Koruma Duruşu | Kendi DEF'ini artırır | Kendisi | DEF × 2.0, 2 tur | — |
| **Destekçi** | İyileştirme | Peti iyileştirir | Pet (kendisi) | max_hp × 0.20 | — |
| **Büyücü** | Element Dalgası | Düşmana büyü hasarı | Düşman | ATK × 0.75 | Büyü (`magic_defense_factor=4`) |

**Not**: Keşif Alanı'nda her aşamada 1 düşman vardır. Büyücü'nün "AoE" etiketi MVP'de yalnızca tek hedefe uygulanır (gelecekte çok düşman içeren boss aşamalarında anlam kazanabilir).

**Kural 7 — Oyuncu Sınıf Yetenek Sistemi (Cooldown)**

Oyuncu sınıfı (Savaşçı/Büyücü/Hırsız/Şifacı), pet yetenek sisteminden bağımsız 4 yetenek slotuna sahiptir.

**Slot yapısı**:

| Slot | Cooldown | Açıklama |
|------|----------|----------|
| Slot 0 | CD0 | Her tur kullanılabilir — temel saldırı / temel eylem |
| Slot 1 | CD3 | 3 tur bekleme — orta güç yetenek |
| Slot 2 | CD5 | 5 tur bekleme — güçlü yetenek |
| Slot 3 | CD8 | 8 tur bekleme — ultimate |

**Cooldown mekanizması**:
- Savaş başlangıcı: tüm CD'ler 0 (tüm yetenekler açık)
- Yetenek kullanıldığında: `current_cd = slot_cd` set edilir
- Her oyuncu turu sonu: `current_cd = max(0, current_cd - 1)`
- Kullanılabilir koşul: `current_cd == 0`

**Komutan modunda seçim**:
- Oyuncu kendi turunda kullanılabilir slotlardan birini seçer
- Seçilmezse Slot 0 (CD0) otomatik kullanılır
- Butonlar: yalnızca `current_cd == 0` olanlar aktif

**Otofarm modunda öncelik**: Slot 3 → Slot 2 → Slot 1 → Slot 0.

**Sınıf yetenek tablosu (özet)** — ayrıntılar Oyuncu Sınıf Sistemi GDD'sinde:

| Sınıf | Slot 0 | Slot 1 (CD3) | Slot 2 (CD5) | Slot 3 (CD8) |
|-------|--------|--------------|--------------|--------------|
| Savaşçı | Normal saldırı | Sersemletme + hasar | DEF Kırma + Kalkan | AoE (boss kitleri için) |
| Büyücü | Büyü saldırısı | Hafif büyü | Ağır büyü + Yanma DoT | Güçlü büyü |
| Hırsız | Normal saldırı | Zehir + hasar | Kaçınma duruşu | Çoklu vuruş combo |
| Şifacı | Hafif büyü | Pete İyileştirme | Pete Diriliş | ATK buff + Kesin Kritik |

---

**Kural 8 — DoT Sistemi (Damage over Time)**

DoT efektleri hasar pipeline'ından bağımsızdır — DEF'i bypass eder, doğrudan HP düşürür.

**DoT türleri**:

| Tip | Kaynak | Oran | Süre | Uygulama |
|-----|--------|------|------|----------|
| **Yanma** | Büyücü Slot 2 | max_hp × 0.05 / tur | 3 tur | Hedef birim tur başı (Faz 1) |
| **Zehir** | Hırsız Slot 1 | max_hp × 0.04 / tur | 4 tur | Hedef birim tur başı (Faz 1) |

**Uygulama kuralları**:
- `dot_damage = max(1, floor(target_max_hp × dot_rate))`
- DoT, rejenerasyondan (Faz 2) önce gelir
- Aynı tipten DoT stack'lenmez: tekrar uygulanırsa süre yenilenir
- Farklı tipler (Yanma + Zehir) aynı anda aktif olabilir
- DoT düşmana da pette de uygulanabilir (kaynağa bağlı)

**Boss bağışıklığı**: Boss Sersemletme'ye bağışıklıdır. DoT'a bağışıklık yok.

---

**Kural 9 — Status Efektleri**

| Efekt | Kaynak | Etki | Süre | Boss Bağışıklığı |
|-------|--------|------|------|-----------------|
| **Sersemletme** | Savaşçı Slot 1 | Hedef Faz 4'ü atlar | 1 tur | Evet |
| **DEF Kırma** | Savaşçı Slot 2 | Hedef DEF × 0.70 | 2 tur | Hayır |
| **ATK Zayıflatma** | Şifacı Slot 3 | Hedef ATK × 0.80 | 2 tur | Hayır |
| **Kalkan** | Savaşçı Slot 2 | Hasar absorbe eder (max_hp × 0.25) | 3 tur veya dolana dek | Hayır |
| **Kesin Kritik** | Şifacı Slot 3 | Sonraki saldırı garantili kritik | 1 kullanım | Hayır |
| **Hasar Azaltma** | Savaşçı Slot 1 | Alınan hasar × 0.75 | 2 tur | Hayır |

**Uygulama kuralları**:
- Aynı tip efekt stack'lenmez — tekrar uygulanırsa süre yenilenir
- Farklı efektler aynı anda aktif olabilir
- Kalkan aktifken gelen hasar önce kalkanı tüketir
- `shield_hp = floor(target_max_hp × 0.25)`; hasar aştığında kalan HP'ye gider
- Status süresi: her UnitTurnEnd'de 1 azalır (etkilenen birimin turunda)

---

**Kural 10 — Mod Geçişi**

- Savaş öncesinde mod seçilir (varsayılan: son kullanılan)
- Savaş sırasında mod toggle edilebilir (tek dokunuş)
- Değişiklik bir sonraki turdan geçerli
- Komutan → Otofarm: ATK bonusu düşer, yetenek butonları kaybolur
- Otofarm → Komutan: ATK bonusu aktifleşir, yetenek butonları belirir
- Cooldown sayaçları mod değişiminden etkilenmez

**Kural 11 — Savaş Hız Kontrolü**

| Hız | Tur Süresi | Kullanım |
|-----|-----------|----------|
| 1x | ~2 sn/tur | İlk savaşlar |
| 2x | ~1 sn/tur | Normal oyun |
| 3x | ~0.5 sn/tur | Farming |

Hız yalnızca animasyon süresini etkiler — hesaplama değişmez. Tercih kaydedilir.

### States and Transitions

**Üst Düzey Savaş Durumları**

| Durum | Açıklama | Giriş | Çıkış |
|-------|----------|-------|-------|
| **PreCombat** | Pet yükleme, mod seçimi | Savaş başlatma | → Combat |
| **Combat** | Aktif tur döngüsü | PreCombat tamamlanınca | → Victory / Defeat / Retreat |
| **Victory** | Düşman HP=0 | Son düşman düşünce | → PostCombat |
| **Defeat** | Pet HP=0 | Pet düşünce | → PostCombat |
| **Retreat** | Oyuncu çekildi | "Çekil" butonu | → PostCombat |
| **PostCombat** | Loot dağıtımı, iyileşme | Victory/Defeat/Retreat | → Keşif Alanı'na döner |

```
PreCombat → Combat ──→ Victory  ──→ PostCombat
                   ├──→ Defeat   ──→ PostCombat
                   └──→ Retreat  ──→ PostCombat
```

**Combat İçi Tur Döngüsü**

```
RoundStart → [Oyuncu, Pet, Düşman SPD sırasıyla]:
  UnitTurnStart → DoTPhase → RegenPhase → EnergyPhase → DecisionPhase → ActionPhase → ResolutionPhase → UnitTurnEnd
→ RoundEnd → (kazanma/kaybetme kontrolü) → RoundStart (veya Victory/Defeat)
```

| Faz | Ne Olur |
|-----|---------|
| **RoundStart** | Sıra listesi SPD'ye göre güncellenir, savaş dışı birimler çıkarılır |
| **DoTPhase** | Aktif DoT'lar uygulanır; HP=0 ise savaş dışı |
| **RegenPhase** | Pasif rejenerasyon (pet ve düşman — oyuncuya uygulanmaz) |
| **EnergyPhase** | energy += 25 (yalnızca pet turu) |
| **DecisionPhase** | Mod'a göre aksiyon belirlenir; Sersemletme aktifse atlanır |
| **ActionPhase** | Saldırı/yetenek animasyonu + hasar hesaplama |
| **ResolutionPhase** | HP güncelleme, savaş dışı kontrolü |
| **UnitTurnEnd** | CD sayaçları -1, status süreleri -1, sonraki birime geç |
| **RoundEnd** | Victory/Defeat kontrolü |

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Pet Sistemi** | ← okur | Aktif pet (ID, effective stats, element, arketip) | `GetActivePet()` → {petId, effective_stats, element, archetype} |
| **Canavar Veritabanı** | ← okur | Pet kimliği, yetenek tanımı | `GetMonsterIdentity(petId)` → {element, archetype}; `GetSkillDef(archetype)` |
| **Canavar Güçlendirme** | ← okur | Pipeline stat çıktısı | `GetEffectiveStats(petId)` → {hp, atk, def, spd} |
| **Element Sistemi** | ← okur | Element çarpanı | `GetElementMultiplier(atkElement, defElement)` → float |
| **Hasar Hesaplama** | → tetikler | Saldırı komutu → hasar değeri | `CalculateDamage(attackerId, targetId, damageType)` → int |
| **Oyuncu Sınıf Sistemi** | ← okur | Oyuncunun damageType, SPD değeri, slot yetenekleri | Savaş sistemi sınıf verisini buradan alır |
| **Sağlık / Can Sistemi** | ↔ çift yönlü | HP durumu okur; hasar/iyileşme uygular | `IsAlive(id)`, `GetCurrentHP(id)`, `TakeDamage(id, amount)`, `Heal(id, amount)`, `FullHeal(petId)` |
| **Düşman AI** | ← okur | Düşman aksiyon kararı | `GetEnemyAction(enemyId)` → {actionType, targetId, skillId} |
| **Loot / Ödül Sistemi** | → tetikler | Savaş sonucu → loot | `DistributeLoot(battleResult, stageNumber)` |
| **Keşif Alanı** | ↔ çift yönlü | Düşman alır; savaş sonucu bildirir | `GetStageEnemy(stageId)` ←; `OnBattleComplete(result)` → |
| **Savaş UI** | → sağlar | Tüm savaş durumu, animasyon tetiklemeleri | `OnTurnStart`, `OnActionExecuted`, `OnBattleEnd`, `OnModeChanged` events |

## Formulas

### Formül 1: Komutan ATK Bonusu

`commander_ATK = floor(effective_ATK × (1 + commander_atk_bonus))`

| Değişken | Değer | Açıklama |
|----------|-------|----------|
| effective_ATK | 15–600 | Pet'in pipeline ATK çıktısı |
| commander_atk_bonus | 0.30 | Sabit %30 |

**Örnek — F tier Pet (effective_ATK=35)**:
→ commander_ATK = floor(35 × 1.30) = **45** (+10 ATK)

**Örnek — B tier Pet Lv20 (effective_ATK=117)**:
→ commander_ATK = floor(117 × 1.30) = **152** (+35 ATK)

### Formül 2: Saldırgan Yeteneği — Güçlü Vuruş

```
1. boosted_ATK = floor(ATK_source × 2.0)
2. def_reduction = floor(target_DEF / 2)
3. base_damage = max(1, boosted_ATK - def_reduction)
4. element_damage = floor(base_damage × element_multiplier)
5. skill_damage = was_crit ? floor(element_damage × 2.0) : element_damage
```

**Örnek (Komutan)**: F tier Saldırgan (ATK=45) vs F tier Düşman (DEF=35), nötr, crit yok
→ boosted=90, def_red=17, base=73, final=**73**

### Formül 3: Büyücü Yeteneği — Element Dalgası

```
1. boosted_ATK = floor(ATK_source × 0.75)
2. def_reduction = floor(target_DEF / 4)   ← büyü hasarı DEF'i yarım etkiler
3. base_damage = max(1, boosted_ATK - def_reduction)
4. skill_damage = floor(base_damage × element_multiplier)
```

**Örnek**: F tier Büyücü (ATK=38) vs Düşman (DEF=35), nötr
→ boosted=28, def_red=floor(35/4)=8, base=20, final=**20**

### Formül 4: Tank Yeteneği — Koruma Duruşu

`buffed_DEF = floor(effective_DEF × 2.0)` — 2 tur sürer

**Örnek**: F tier Tank (DEF=35) → buffed_DEF=70 → gelen hasar dramatik düşer

### Formül 5: Destekçi Yeteneği — İyileştirme

`heal_amount = floor(pet_max_hp × 0.20)`

### Formül 6: Enerji Progresyonu

`turns_to_ability = ceil(100 / 25) = 4 tur`

Pet her 4 turda bir yeteneğini kullanabilir.

### Formül 7: Savaş Süresi Tahmini

`estimated_turns = ceil(enemy_HP / (pet_DPS + player_skill_DPS))`

**Hedef**: 5-8 tur (30 saniyede bir savaş @ 2x hız).

### Formül 8: DoT Hasarı

`dot_damage_per_tick = max(1, floor(target_max_hp × dot_rate))`

| DoT | Oran | Süre | Toplam (F tier max_hp≈60) |
|-----|------|------|--------------------------|
| Yanma | 0.05 | 3 tur | ~9 hasar |
| Zehir | 0.04 | 4 tur | ~9 hasar |

### Formül 9: Cooldown Yönetimi

`is_available = (current_cd == 0)`

Savaş başlangıcında tüm CD'ler 0 → ilk turda tüm yetenekler açık.

## Edge Cases

- **If pet HP=0 olursa**: Defeat anında tetiklenir. Oyuncu turu gelmişse iptal edilir. PostCombat'a geçilir, enerji harcanmaz.

- **If oyuncu turunda hiçbir slot açık değilse**: Mümkün değil — Slot 0 (CD0) her zaman açıktır.

- **If komutan modunda oyuncu yetenek butonuna basmazsa**: Slot 0 otomatik kullanılır. Slot 1-3 açıksa enerji/CD boşa gitmez — oyuncu sonraki turunda kullanabilir.

- **If pet energy=100'deyken pet savaş dışı kalırsa**: Enerji kaybolur. Savaş sonu sıfırlanır.

- **If savaş sırasında mod değiştirilirse**: Değişiklik bir sonraki turdan geçerli. Mevcut tur eski modla tamamlanır.

- **If Tank Koruma Duruşu aktifken pet tekrar yetenek kullanırsa**: Süre yenilenir (2 tura sıfır). Stack'lenmez.

- **If oyuncu "Çekil" butonuna PreCombat'ta basarsa**: Savaş başlamaz, enerji harcanmaz.

- **If savaş 20+ tur sürerse**: Devam eder — tur limiti yok. Denge sorunu olarak loglanır.

- **If DoT uygulanmış düşman DoTPhase'de savaş dışı kalırsa**: Victory tetiklenir, kalan fazlar atlanır.

- **If Yanma aktifken hedefe tekrar Yanma uygulanırsa**: Süre 3 tura yenilenir, hasar oranı değişmez.

- **If Sersemletme boss'a uygulanırsa**: Stun uygulanmaz (bağışıklık). Yetenekten gelen hasar normal uygulanır.

- **If Kalkan aktifken gelen hasar kalkan kapasitesinden büyükse**: Kalan hasar HP'ye gider. `hp_damage = total_damage - shield_remaining`.

- **If uygulama arka plana atılırsa (mobil)**: Savaş durumu kaydedilir. Geri dönünce kaldığı yerden devam. Otofarm modundaysa arka planda tamamlanır.

## Dependencies

### Upstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Pet Sistemi** | Sert | `GetActivePet()` | Olmadan pet yüklenemez |
| **Canavar Güçlendirme** | Sert | `GetEffectiveStats(petId)` | Olmadan stat pipeline çıktısı alınamaz |
| **Canavar Veritabanı** | Sert | `GetMonsterIdentity(petId)`, `GetSkillDef(archetype)` | Olmadan pet kimliği ve yetenek bilinmez |
| **Oyuncu Sınıf Sistemi** | Sert | Oyuncunun SPD, damageType, slot yetenekleri | Olmadan oyuncu turu çalışmaz |
| **Element Sistemi** | Sert | `GetElementMultiplier(atkEl, defEl)` | Olmadan element farkı yok |
| **Hasar Hesaplama** | Sert | `CalculateDamage(attackerId, targetId, damageType)` | Olmadan hasar üretilemez |
| **Sağlık / Can Sistemi** | Sert | `IsAlive`, `TakeDamage`, `Heal`, `FullHeal` | Olmadan savaş döngüsü çalışmaz |
| **Düşman AI** | Sert | `GetEnemyAction(enemyId)` | Olmadan düşman aksiyon alamaz |
| **Keşif Alanı** | Sert | `GetStageEnemy(stageId)` | Olmadan düşman tanımı alınamaz |

### Downstream

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Loot / Ödül Sistemi** | Sert | `DistributeLoot(battleResult, stageNumber)` | Olmadan ödül dağıtılamaz |
| **Savaş UI** | Sert | `OnTurnStart`, `OnActionExecuted`, `OnBattleEnd`, `OnModeChanged` | Olmadan savaş görüntülenemez |
| **Keşif Alanı** | Sert | `OnBattleComplete(result)` | Olmadan aşama ilerlemesi güncellenmez |
| **Otofarm / Idle Sistemi** | Sert | Savaş simülasyonu (basitleştirilmiş) | İdle modda savaş sonuçlarını simüle eder |

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `commander_atk_bonus` | 0.30 | 0.15–0.40 | Komutan çok güçlü → otofarm anlamsız | Fark hissedilmez |
| `energy_per_turn` | 25 | 15–50 | Yetenek çok sık | Yetenek çok seyrek → monotonluk |
| `energy_threshold` | 100 | 50–200 | Yetenek çok sık | Oyuncu yetenek göremeden savaş biter |
| `skill_atk_multiplier` | 2.0 | 1.5–3.0 | Tek vuruşta öldürür | Normal saldırıdan zar zor iyi |
| `skill_aoe_multiplier` | 0.75 | 0.50–1.0 | Büyücü overpowered | Büyücü değersiz |
| `skill_def_multiplier` | 2.0 | 1.5–3.0 | Hasar almaz | Tank değersiz |
| `skill_buff_duration` | 2 | 1–4 | Sürekli kalkan | Anlık etki |
| `healer_skill_rate` | 0.20 | 0.15–0.25 | Ölümsüzlük | İyileşme anlamsız |
| `yanma_rate` | 0.05 | 0.03–0.10 | DoT tek başına öldürür | Fark edilmez |
| `yanma_duration` | 3 | 2–5 | Ezici kümülatif | Çok kısa |
| `zehir_rate` | 0.04 | 0.02–0.08 | Hırsız overpowered | Motivasyon yok |
| `zehir_duration` | 4 | 3–6 | Boss erir | Değersiz |
| `shield_rate` | 0.25 | 0.15–0.40 | Ölümsüzlük | Anlamsız |
| `def_break_mult` | 0.70 | 0.50–0.85 | DEF tamamen anlamsız | Fark edilmez |
| `atk_weaken_mult` | 0.80 | 0.60–0.90 | Düşman saldırısı sıfır | Hissedilmez |

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Öncelik |
|------|-----|---------|
| Savaş başlangıcı | Pet giriş (soldan) + düşman belirme (sağdan) | MVP |
| Tur sırası göstergesi | Aktif birimin çevresinde parlak çerçeve | MVP |
| Pet enerji %100 | Enerji barı dolma + yetenek butonu pulse | MVP |
| Saldırgan — Güçlü Vuruş | Büyük slash + element darbe dalgası + ekran sarsıntısı | MVP |
| Tank — Koruma Duruşu | Kalkan oluşma (mavi-altın) + 2 tur sürekli aura | MVP |
| Destekçi — İyileştirme | Yeşil ışınlar + "+X HP" text | MVP |
| Büyücü — Element Dalgası | Element renginde genişleyen dalga | MVP |
| Komutan modu aktif | Altın komutan rozeti + pet çevresinde hafif aura | MVP |
| Otofarm modu aktif | Gri/mavi çark ikonu + "AUTO" | MVP |
| Kazanma | Altın ışık patlaması + "ZAFER!" + loot belirme | MVP |
| Kaybetme | Ekran kararma + "YENİLDİN" + "Tekrar Dene" | MVP |

## UI Requirements

### Savaş Ekranı Ana Layout

- **Üst bölüm**: Düşman (sprite + HP barı + element ikonu + CP göstergesi)
- **Orta bölüm**: Savaş alanı — animasyonlar, hasar sayıları, VFX
- **Alt bölüm**: Pet (sprite + HP barı + enerji barı + element ikonu)
- **Ekran alt kenarı**: 4 oyuncu yetenek butonu + 1 pet yetenek göstergesi (komutan modunda)
- **Ekran sol üst**: Mod toggle (Komutan/Otofarm)
- **Ekran sağ üst**: Hız butonu (1x/2x/3x) + Çekil butonu

### Yetenek Butonları (Komutan Modu)

- **4 oyuncu sınıf butonu** (Slot 0-3), ekran altında yatay dizi
- **1 pet yetenek göstergesi** (enerji radial fill) — pet turu gelince pulse
- CD dolmamış slotlar soluk, CD=0 olanlar parlak
- Minimum dokunma hedefi: 64×64 dp

### Savaş Sonu Ekranı

- **Kazanma**: "ZAFER!" + EXP kazanımı + loot listesi + "Devam" butonu
- **Kaybetme**: "Tekrar Dene" + "Çekil" butonu (enerji harcanmadığı belirtilir)

## Acceptance Criteria

1. **GIVEN** savaş başlatıldığında, **WHEN** PreCombat tamamlanırsa, **THEN** pet current_hp=max_hp ve energy=0, tüm oyuncu CD'leri=0.

2. **GIVEN** Pet SPD=30, Oyuncu SPD=25, Düşman SPD=20, **WHEN** ilk raunt başlarsa, **THEN** tur sırası: Pet→Oyuncu→Düşman.

3. **GIVEN** SPD eşitliğinde (Pet SPD=30, Düşman SPD=30), **WHEN** sıra belirlenirse, **THEN** Pet önce hareket eder.

4. **GIVEN** komutan modunda F tier Saldırgan Pet (effective_ATK=35), **WHEN** saldırırsa, **THEN** commander_ATK = floor(35×1.30) = 45 kullanılır.

5. **GIVEN** otofarm modunda aynı pet, **WHEN** saldırırsa, **THEN** effective_ATK = 35 kullanılır (commander bonusu yok). ATK farkı UI'da gösterilmez.

6. **GIVEN** pet energy=75, **WHEN** pet turu enerji fazı çalışırsa, **THEN** energy = 100, yetenek göstergesi pulse başlar.

7. **GIVEN** pet energy=100, komutan modu, **WHEN** oyuncu pet yetenek butonuna basarsa, **THEN** yetenek kullanılır, energy = 0.

8. **GIVEN** pet energy=100, komutan modu, **WHEN** oyuncu basmazsa, **THEN** normal saldırı yapılır, energy = 100 kalır.

9. **GIVEN** pet energy=100, otofarm modu, **WHEN** pet turu gelirse, **THEN** yetenek anında kullanılır, energy = 0.

10. **GIVEN** Saldırgan Güçlü Vuruş, komutan modu (ATK=45) vs Düşman (DEF=35), nötr, crit yok, **WHEN** kullanılırsa, **THEN** boosted=90, base=90-17=73, final=**73**.

11. **GIVEN** Büyücü Element Dalgası (ATK=38) vs Düşman (DEF=35), nötr, **WHEN** kullanılırsa, **THEN** boosted=28, def_red=8, final=**20**.

12. **GIVEN** Tank Koruma Duruşu (DEF=35), **WHEN** kullanılırsa, **THEN** buffed_DEF=70, 2 tur sürer. 3. turda DEF=35'e döner.

13. **GIVEN** Destekçi (max_hp=60, current_hp=30), **WHEN** yetenek kullanılırsa, **THEN** heal=floor(60×0.20)=12, current_hp=42.

14. **GIVEN** düşman HP=0, **WHEN** son hasar gelirse, **THEN** Victory tetiklenir, loot dağıtılır, pet tam HP'ye döner.

15. **GIVEN** pet HP=0, **WHEN** son hasar gelirse, **THEN** Defeat tetiklenir, loot yok, enerji harcanmaz, pet tam HP'ye döner.

16. **GIVEN** savaş devam ederken, **WHEN** oyuncu "Çekil" basarsa, **THEN** savaş biter, loot yok, enerji harcanmaz.

17. **GIVEN** komutan modu aktif, **WHEN** oyuncu mod toggle'a basarsa, **THEN** bir sonraki turdan otofarm aktif, ATK bonusu düşer.

18. **GIVEN** Büyücü Slot 2 Yanma DoT uygulandı (düşman max_hp=60), **WHEN** düşman turu gelirse, **THEN** DoTPhase'de max(1, floor(60×0.05))=**3 hasar**, 3 tur devam eder.

19. **GIVEN** Hırsız Slot 1 Zehir DoT (düşman max_hp=60), **WHEN** 4 tur boyunca DoTPhase çalışırsa, **THEN** max(1, floor(60×0.04))=**2 hasar/tur**, toplam 8 hasar.

20. **GIVEN** Savaşçı Slot 1 stun (normal düşman), **WHEN** düşman turu gelirse, **THEN** DecisionPhase atlanır. Sonraki turda normal.

21. **GIVEN** Savaşçı Slot 1 boss'a stun, **WHEN** uygulanırsa, **THEN** stun atlanır (bağışıklık), hasar normal.

22. **GIVEN** Kalkan aktif (pet max_hp=60, shield_hp=15), 20 hasar geldi, **WHEN** uygulanırsa, **THEN** 15 hasar kalkanı tüketir, kalan 5 hasar HP'ye gider.

23. **GIVEN** oyuncu Slot 3 (CD8) savaş başında kullanır, **WHEN** kullanılırsa, **THEN** kullanılır, current_cd=8 set edilir.

24. **GIVEN** otofarm modunda Slot 2 ve Slot 3 aynı anda açık, **WHEN** oyuncu turu gelirse, **THEN** Slot 3 kullanılır.

*`qa-lead` not consulted — Lean mode. Review manually before production.*

## Open Questions

1. **Oyuncu SPD değeri**: Oyuncu sınıfına göre sabit mi, yoksa ekipmanla artıyor mu? → Oyuncu Sınıf Sistemi GDD'sinde tanımlanacak.

2. **Düşman hedef seçimi**: Düşman her zaman peti mi hedefler? Yoksa oyuncuya da saldırabilir mi (oyuncunun da HP'si olması durumunda)? → MVP'de düşman yalnızca peti hedefler. Oyuncunun HP'si yok.

3. **Pet savaş dışı kalınca oyuncu devam eder mi?** → MVP'de hayır. Pet düşünce = Defeat. Oyuncu tek başına savaşamaz.

4. **Çoklu düşman (gelecek içerik)**: Keşif Alanı boss aşamalarında yardımcı düşmanlar eklenirse (örn. mini-boss + 2 yardımcı), hedef seçimi ve Büyücü AoE yeniden tasarlanmalı. → Tier 2+ kararı.

5. **Pasif yetenekler**: Her arketipe 1 pasif yetenek planlanıyor mu? → Tier 2+ genişletmesi.
