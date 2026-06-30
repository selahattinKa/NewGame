# Takım Kurma (Team Building)

> **Status**: Approved
> **Author**: user + game-designer
> **Last Updated**: 2026-06-25
> **Implements Pillar**: Topla Hepsini, Güç Hisset, Senin Tempon

## Overview

**Takım Kurma**, oyuncunun koleksiyonundaki canavarlardan 4'ünü seçerek savaş takımı oluşturduğu stratejik hazırlık sistemidir. Veri katmanı olarak, seçili canavarların kimlik bilgilerini (element, arketip, stat'lar) Canavar Veritabanı'ndan alır ve Element Sistemi'nin sinerji hesaplamalarını tetikleyerek takımın efektif güç profilini belirler — Hibrit Savaş Sistemi bu profili girdi olarak kullanır. Oyuncu perspektifinden ise her içerik öncesinde "hangi canavarlarla gideyim?" sorusu oyunun en stratejik karar noktasıdır: ateş zindanına su ağırlıklı takım, buz zindanına ateş takımı, arenaya özel savunma kompozisyonu. Oyuncu farklı içerikler için birden fazla takım preset'i kaydeder ve tek dokunuşla aktif takımını değiştirir; arenaya girerken kayıtlı takım otomatik yüklenir. Bu sistem olmadan element sinerjileri ve arketip çeşitliliği dekoratif kalır, koleksiyon "en güçlü 4'ü seç" düz bir güç yarışına dönüşür ve oyuncunun stratejik ifade alanı tamamen kaybolur.

## Player Fantasy

Takım Kurma'da oyuncunun yaşadığı fantazi **koleksiyoner gururu ile keşif sevincinin** birleşimidir. Çekirdek an, yeni bir canavar kazanıp güçlendirdikten sonra takım ekranını açıp "bu tam buraya uyar!" farkındalığıdır — Su takımına eksik olan o Nadir Su Destekçisi düştüğünde, onu yerine koymak bir puzzle parçasının yerine oturması kadar tatmin edicidir.

Sistem bilinçli olarak hafif tutulmuştur: oyuncu saatlerini versin ama kafasını yormasın. Element avantajı basit ve sezgisel (ateşe karşı su), sinerji bonusu otomatik hesaplanıp ekranda gösterilir, "en güçlü takımı öner" butonu bir dokunuşla optimal takımı kurar. Strateji derinliği isteyene açıktır ama zorunlu değildir — yanlış takımla girmek cezalandırıcı değil, sadece doğru takımla girmek belirgin şekilde ödüllendirici.

**Koleksiyoner fantazisi**: Takım preset ekranı, oyuncunun "Ateş Zindanı Takımım", "Buz Zindanı Takımım", "Arena Takımım" diye farklı vitrinler oluşturmasını sağlar. Her preset bir gurur koleksiyonudur — en güçlü canavarlarını sergiler. Yeni bir Efsanevi canavar kazandığında ilk dürtüsü "bunu hangi takımıma koyayım?" olmalıdır.

**Keşif katmanı**: Oyuncu zaman zaman beklenmedik sinerji keşfeder — 3 aynı element canavarla denediğinde "+15% ATK bonusu" belirince "vay, bu iyi!" anı yaşar. Ama bu keşif zorunlu değildir. Oyun hiç sinerji düşünmeden de eğlenceli; sinerjileri keşfeden biraz daha iyi oynuyor.

**Negatif fantazi (kaçınılacak)**: Takım kurma asla "yanlış kurdum, kaybettim" frustrasyonu yaratmamalı. Yanlış element takımıyla bile zindan geçilebilmeli — sadece daha yavaş ve biraz daha az loot. "Optimal takım kurmalıyım" baskısı olmamalı — bu rahat ve akıcı bir deneyim, hardcore min-max değil. Anti-pillar ile tutarlı: "NOT Karmaşık strateji bulmacası."

**Pillar bağlantısı**: "Topla Hepsini" — her yeni canavar takım olasılıklarını genişletir. "Güç Hisset" — güçlendirilmiş takımın toplam güç skoru büyür. "Senin Tempon" — preset sistemiyle rahat yönetim, istenirse tek dokunuşla otomatik öneri.

*`creative-director` not consulted — Lean mode. Review manually before production.*

## Detailed Rules

### Core Rules

**Kural 1 — Takım Kompozisyonu**

- Bir takımda minimum 1, maksimum 4 canavar bulunur
- Aynı canavar aynı takımda birden fazla slotta bulunamaz (benzersiz)
- Aynı canavar farklı preset'lerde yer alabilir — kısıtlama yok
- Takımda slot sırası (1–4) formasyon pozisyonunu belirler (savaş dizilimi — detaylar Hibrit Savaş GDD'sinde)
- Takım boş bırakılamaz — en az 1 canavar olmadan savaş başlatılamaz

**Kural 2 — Canavar Seçimi**

- Oyuncu koleksiyonundaki sahip olduğu tüm canavarlardan seçim yapar
- Seçim ekranında canavarlar varsayılan olarak güç skoruna göre sıralı gösterilir
- Filtreleme seçenekleri: element (Ateş/Su/Toprak/Hava), arketip (Saldırgan/Tank/Destekçi/Büyücü), nadirlik
- Sıralama seçenekleri: güç skoru, seviye, nadirlik, element
- Bir canavar seçildiğinde ilk boş slot'a yerleşir (sol→sağ sırasıyla: slot 1, 2, 3, 4). Tüm slotlar doluysa seçim yapılamaz — önce bir canavar çıkarılmalı
- Tekrar dokunulunca canavar slottan çıkarılır
- Canavar bir slottan başka bir slota taşınabilir (sürükle-bırak veya slot'a dokunup hedef slot'a dokunma). Hedef slot doluysa iki canavarın yeri takas edilir (swap)
- Seviye veya güç kısıtlaması yok — sahip olunan her canavar seçilebilir

**Kural 3 — Takım Preset Sistemi**

- Oyuncu başlangıçta **3 ücretsiz preset slot'una** sahiptir
- Ek slotlar elmas harcayarak açılır (maliyet Tuning Knobs'ta tanımlı)
- Her preset'e özel isim verilebilir (varsayılan: "Takım 1", "Takım 2", "Takım 3")
- Preset seçimi tek dokunuşla: tüm slotlar anında güncellenir
- Arena'ya girerken aktif preset otomatik yüklenir
- Preset'teki bir canavar satılır/kaybolursa o slot boşalır — preset silinmez, eksik slot gösterilir

**Preset Veri Yapısı** (Kaydetme/Yükleme serializasyonu için):
```
TeamPreset {
  preset_id: int          // 1–max_preset_slots
  preset_name: string     // max 20 karakter, varsayılan "Takım [N]"
  slots: [monsterId | null] × max_team_size  // sıralı, null = boş slot
}
ActiveTeamState {
  active_preset_id: int   // aktif preset referansı
}
```

**Kural 4 — Otomatik Öneri (Auto-Suggest)**

"En İyi Takımı Öner" butonu tek dokunuşla takım kurar:

1. **Bağlam tespiti**: Zindan girişindeyse → girilecek katın düşman element kompozisyonunu al (sadece bir sonraki kat, tüm zindan değil). Genel ekrandaysa → bağlamsız mod. Kat arası düzenlemede → bir sonraki katın düşmanları baz alınır.
2. **Canavar skorlama** (zindan bağlamı): Her sahip olunan canavar için:
   - Temel skor = canavarın toplam güç skoru
   - Düşman elementine avantajlıysa → skor × 1.5
   - Düşman elementine dezavantajlıysa → skor × 0.75
   - Nötrse → skor × 1.0
3. **Canavar skorlama** (bağlamsız mod): Skor = toplam güç skoru (element bonusu yok)
4. Skor'a göre azalan sırala, ilk 4'ü seç
5. Sonucu takım slot'larına yerleştir ve oyuncuya göster

Algoritma basit ve şeffaftır — "neden bu takım?" sorusunun cevabı sezgisel olarak anlaşılabilir. Oyuncu öneriyi kabul edebilir veya değiştirebilir.

> **Tasarım notu — sinerji farkındalığı kasıtlı olarak yoktur**: Algoritma sinerji bonusunu hesaba katmaz. Sinerji farkındalığı (3 aynı element tercih etme) eklenseydi algoritma daha "akıllı" ama daha az şeffaf olurdu — oyuncu "neden güçsüz Su canavarımı önerdi?" sorusuna sezgisel cevap veremezdi. Sinerji keşfi oyuncunun kendi keşif alanı olarak bırakılmıştır (Player Fantasy: "keşif katmanı"). İleride Tier 2+ olarak sinerji-bilinçli öneri modu eklenebilir (Open Questions #4).

**Kural 5 — Takım Güç Skoru (Team Power Score)**

Takımın genel gücünü tek bir sayıyla gösterir:

`team_power = Σ (her canavarın effective_HP + effective_ATK + effective_DEF + effective_SPD)`

- Effective stat'lar sinerji bonusu dahil hesaplanır
- Güç skoru takım değişikliği anında güncellenir
- Sinerji aktifleşince güç skorunun artışı yeşil renkte vurgulanır
- Formül detayları Section D'de tanımlanır

**Kural 6 — Zindan Güç Eşiği Göstergesi**

Zindan girişinde, takım güç skoru yanında zindanın "Önerilen Minimum Güç" değeri gösterilir:

| Takım Gücü / Önerilen Güç | Gösterge | Mesaj |
|----------------------------|----------|-------|
| ≥ 100% | Yeşil | "Hazırsın!" |
| %70–99 | Sarı | "Zorlu olabilir" |
| < %70 | Kırmızı | "Çok güçlü düşmanlar!" |

Gösterge sadece öneridir — oyuncu her zaman girebilir. Güç karşılaştırması Zindan Keşif GDD Kural 9 (zorluk ölçeklemesi) ve Düşman AI GDD (düşman stat formülü) ile hesaplanır.

**Kural 7 — Kat Arası Takım Düzenleme**

Game concept'e göre "Kat aralarında takım düzenleme fırsatı" vardır:

- Zindan katları arasında (savaş dışında) oyuncu takımını düzenleyebilir
- Savaş sırasında takım kilitlidir — değişiklik yapılamaz
- Kat arası düzenleme tam takım ekranını açar (preset seçimi dahil)
- Sinerji bonusu yeni takım kompozisyonuna göre yeniden hesaplanır

### States and Transitions

| Durum | Açıklama | Tetikleyici | Hedef Durum |
|-------|----------|-------------|-------------|
| **Boş** | Hiç canavar seçilmemiş | Canavar seçilir / Preset yüklenir | Kısmi veya Tam |
| **Kısmi** | 1–3 canavar seçili | 4. canavar eklenir | Tam |
| **Kısmi** | 1–3 canavar seçili | "Savaş Başlat" | Kilitli |
| **Tam** | 4 canavar seçili | "Savaş Başlat" | Kilitli |
| **Tam** | 4 canavar seçili | Canavar çıkarılır | Kısmi |
| **Kilitli** | Savaş devam ediyor | Kat tamamlanır | Düzenlenebilir |
| **Kilitli** | Savaş devam ediyor | Zindan tamamlanır/terk edilir | Tam/Kısmi |
| **Düzenlenebilir** | Kat arası düzenleme aktif | "Sonraki Kat" butonu | Kilitli |

Preset yükleme: Boş, Kısmi, Tam veya Düzenlenebilir durumlarında preset tek dokunuşla tüm slotları günceller.

### Interactions with Other Systems

| Sistem | Yön | Veri Akışı | Arayüz |
|--------|-----|-----------|--------|
| **Canavar Veritabanı** | ← okur | Canavar kimliği: element, arketip | `GetMonsterIdentity(monsterId)` |
| **Canavar Güçlendirme** | ← okur | Effective stats (seviye + evrim + yıldız) | `GetEffectiveStats(monsterId)` → {hp, atk, def, spd} |
| **Element Sistemi** | ← okur | Sinerji bonusu hesaplama | `CalculateSynergy(teamElements[])` → {atk_bonus, def_bonus, spd_bonus} |
| **Ekonomi** | ← okur | Elmas bakiyesi (preset slot açma) | `CanAfford(cost)`, `SpendDiamonds(amount)` |
| **Hibrit Savaş Sistemi** | → sağlar | Savaş takımı | `GetActiveTeam()` → [{monsterId, slot, effective_stats, element}] |
| **Zindan Keşif** | ↔ çift yönlü | Düşman elementleri (okur), aktif takım (sağlar) | `GetFloorEnemyElements()` ← / `GetActiveTeam()` → |
| **Arena** | → sağlar | Kayıtlı arena takımı | `GetArenaTeam()` → preset |
| **Canavar Toplama ve Evrim** | ← okur | Sahip olunan canavarlar listesi | `GetOwnedMonsters()` → list |
| **Kaydetme / Yükleme** | ↔ persist | Preset verisi kaydetme/yükleme | Serialize/Deserialize preset data |

*Specialist agents not consulted — Lean mode. Review manually before production.*

## Formulas

### Formül 1: Takım Güç Skoru (team_power)

Takımın genel gücünü tek bir tamsayı olarak gösterir:

`team_power = Σ (her canavar i için) monster_power_i`

`monster_power_i = effective_HP_i + effective_ATK_i + effective_DEF_i + effective_SPD_i`

Effective stat'lar sinerji bonusu dahil:
```
effective_ATK = floor(pipeline_ATK × (1 + synergy_atk_bonus))
effective_DEF = floor(pipeline_DEF × (1 + synergy_def_bonus))
effective_SPD = floor(pipeline_SPD × (1 + synergy_spd_bonus))
effective_HP  = pipeline_HP  (sinerji HP'ye uygulanmaz)
```

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Pipeline stat | pipeline_STAT | int | 15–499 | `final_stat_pipeline` çıktısı (seviye + evrim + yıldız) |
| Sinerji ATK bonusu | synergy_atk_bonus | float | 0.00–0.20 | Element Sistemi sinerji tablosu |
| Sinerji DEF bonusu | synergy_def_bonus | float | 0.00–0.15 | Element Sistemi sinerji tablosu |
| Sinerji SPD bonusu | synergy_spd_bonus | float | 0.00–0.10 | Element Sistemi sinerji tablosu |
| Canavar gücü | monster_power | int | 100–~1590 | Tek canavarın 4 effective stat toplamı (sinerji dahil) |
| Takım gücü | team_power | int | 100–~6360 | Tüm takımın toplam gücü |

**Çıktı Aralığı**: 100 (tek Common Lv1, sinerji yok) – ~6360 (4× Legendary Lv50 Form C ★5, tam 4'lü sinerji)

**Örnek 1 — Erken oyun** (4 Common Lv1, farklı elementler, sinerji yok):
- Saldırgan: 20+35+15+30 = 100
- Tank: 30+15+35+20 = 100
- Destekçi: 28+17+25+30 = 100
- Büyücü: 18+35+17+30 = 100
- **team_power = 400**

**Örnek 2 — Orta oyun** (3 Ateş + 1 Su, Rare Lv20 Form B):
- Pipeline stat toplam per monster ≈ 310 (150 × 1.4 × 1.475)
- 3 Ateş sinerji: +15% ATK, +10% DEF → Ateş canavarları ≈ 340
- Su canavar (tek, sinerji yok) ≈ 310
- **team_power ≈ 3×340 + 310 = 1330**

**Örnek 3 — Geç oyun** (4 Legendary Lv50 Form C ★5, 4'lü sinerji):
- Pipeline stat toplam per monster ≈ 1415
- 4'lü sinerji: +20% ATK, +15% DEF, +10% SPD
- Effective per monster ≈ 1580
- **team_power ≈ 4×1580 = 6320**

### Formül 2: Otomatik Öneri Skoru (fit_score)

`fit_score = monster_power × element_context_multiplier`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Canavar gücü | monster_power | int | 60–1716 | 4 stat toplamı (sinerji hariç — sinerji takım oluştuktan sonra hesaplanır) |
| Element bağlam çarpanı | element_context_multiplier | float | 0.75–1.50 | Canavarın elementinin düşman elementine karşı avantajı |
| Uyum skoru | fit_score | float | 45–2574 | Sıralama skoru |

**Element bağlam çarpanı**:
- Düşman elementine avantajlı → 1.50
- Düşman elementine dezavantajlı → 0.75
- Nötr → 1.00
- Bağlamsız mod (zindan dışı) → 1.00

**Birden fazla düşman elementi varsa**: Girilecek katın tüm düşmanlarının çoğunluk elementine göre hesaplanır. Eşitlik durumunda nötr (1.0). Kapsam her zaman bir sonraki kattır — tüm zindan bazlı değildir.

**Örnek**: Ateş zindanı (düşmanlar çoğunluk Ateş):
- Su Saldırgan (power=310): fit_score = 310 × 1.5 = **465** ← en iyi
- Toprak Tank (power=320): fit_score = 320 × 0.75 = **240** ← dezavantajlı
- Hava Büyücü (power=300): fit_score = 300 × 1.0 = **300** ← nötr

### Formül 3: Preset Slot Maliyeti (preset_slot_cost)

`preset_slot_cost = base_preset_cost × 2 ^ (slot_index - 4)`

| Değişken | Sembol | Tip | Aralık | Açıklama |
|----------|--------|-----|--------|----------|
| Temel maliyet | base_preset_cost | int | 50 | İlk ücretli slotun maliyeti |
| Slot indeksi | slot_index | int | 4–10 | Açılacak slotun numarası (1-3 ücretsiz) |
| Slot maliyeti | preset_slot_cost | int | 50–3200 | Elmas cinsinden maliyet |

**Maliyet Tablosu**:

| Slot | Maliyet (Elmas) | Kümülatif |
|------|-----------------|-----------|
| 4 | 50 | 50 |
| 5 | 100 | 150 |
| 6 | 200 | 350 |
| 7 | 400 | 750 |
| 8 | 800 | 1550 |

**Çıktı Aralığı**: 50 (4. slot) – 3200 (10. slot, pratik üst sınır)

İlk ek slotlar ucuz ve erişilebilir; geç slotlar koleksiyoner oyuncular için lüks.

*`systems-designer` consulted — agent connection failed, formulas designed from existing GDD data. Review manually before production.*

## Edge Cases

- **If oyuncunun koleksiyonunda hiç canavar yoksa**: Takım kurma ekranı erişilemez. "Henüz canavarın yok!" mesajıyla zindan/arena engellenir. *(İlk oyun akışında tutorial bir canavar verir — bu durum pratikte sadece veri hatası.)*

- **If oyuncunun sadece 1 canavarı varsa**: 1 kişilik takımla savaş başlatılabilir. Sinerji bonusu yok. Güç skoru düşük ama engelleme yok — erken oyun deneyimi kısıtlanmamalı.

- **If preset'teki 4 canavardan 2'si satılmış/kaybolmuşsa**: Preset silinmez — 2 dolu + 2 boş slot gösterilir. Oyuncu eksik slotları manuel doldurur veya Auto-Suggest kullanır. Boş slotlu preset ile savaş başlatılabilir (Kural 1: min 1 canavar).

- **If aynı canavar aynı takımda 2 slota yerleştirilmeye çalışılırsa**: Duplicate engeli — canavar mevcut slotundan çıkarılır ve hedef slota taşınır (Kural 2'deki swap davranışı).

- **If Auto-Suggest çalıştırıldığında oyuncunun 4'ten az canavarı varsa**: Sahip olunan tüm canavarlar seçilir (1, 2 veya 3). Boş slotlar bırakılır.

- **If zindan bağlamında tüm düşmanlar farklı elementlerdeyse (element çoğunluğu yok)**: element_context_multiplier = 1.0 (nötr). Auto-Suggest saf güç skoruna göre seçer.

- **If takım güç skoru zindan önerilen gücünün çok altındaysa (< %30)**: Kırmızı uyarı gösterilir ama giriş engellenmez. "Cömert Zindan" sütunu: oyuncu her zaman deneyebilir.

- **If oyuncu kat arası düzenlemede tüm canavarları çıkarırsa (0 canavar)**: "Sonraki Kat" butonu devre dışı kalır. "En az 1 canavar seç" uyarısı. Zindanı terk etme seçeneği sunulur.

- **If preset ismi boş bırakılırsa**: Varsayılan isim kullanılır ("Takım [N]"). Boş string kaydedilmez.

- **If preset ismi çok uzunsa**: Maksimum 20 karakter. Aşan kısım kesilir.

- **If elmas bakiyesi preset slot açmaya yetmiyorsa**: "Yeterli elmas yok" mesajı. Slot açma butonu devre dışı (soluk). Elmas satın alma ekranına yönlendirme sunulur.

- **If savaş sırasında bir canavar HP=0'a düşerse**: Takım güç skoru güncellenmez — savaş başındaki değer kalır. *(Element Sistemi edge case ile tutarlı: sinerji bonusu da yeniden hesaplanmaz.)*

- **If oyuncu tüm preset'lere aynı takımı kaydederse**: Geçerli. Kısıtlama yok — oyuncunun tercihi.

*`systems-designer` not consulted — Lean mode. Review manually before production.*

## Dependencies

### Upstream (Bu sistem neye bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Canavar Veritabanı** | Sert | `GetMonsterIdentity(monsterId)` → element, arketip | Olmadan canavar seçimi yapılamaz |
| **Canavar Güçlendirme** | Sert | `GetEffectiveStats(monsterId)` → {hp, atk, def, spd} | Olmadan güç skoru hesaplanamaz |
| **Element Sistemi** | Sert | `CalculateSynergy(teamElements[])` → bonuslar | Olmadan sinerji bonusu yok |
| **Ekonomi** | Yumuşak | `CanAfford(cost)`, `SpendDiamonds(amount)` | Olmadan preset slot açma çalışmaz, temel takım kurma etkilenmez |
| **Koleksiyon (Canavar Toplama)** | Sert | `GetOwnedMonsters()` → liste | Olmadan seçilebilecek canavar yok |

### Downstream (Bu sisteme bağlı)

| Sistem | Tip | Arayüz | Kritiklik |
|--------|-----|--------|-----------|
| **Hibrit Savaş Sistemi** | Sert | `GetActiveTeam()` → [{monsterId, slot, effective_stats, element}] | Savaş takımsız başlatılamaz |
| **Zindan Keşif** | Sert | `GetActiveTeam()` → takım / ← `GetFloorEnemyElements()` | Zindan girişi takım gerektirir; kat önizleme çift yönlü |
| **Arena (Asenkron PvP)** | Sert | `GetArenaTeam()` → preset | Arena takımsız çalışmaz (Tier 2) |
| **Savaş UI** | Yumuşak | Takım bilgisi (canavar isimleri, stat'lar, element) | Görüntüleme |
| **Kaydetme / Yükleme** | Sert | Preset verisi serialize/deserialize | Preset'ler oturumlar arası kalıcı olmalı |

**Çift yönlü bağımlılık**: Zindan Keşif ↔ Takım Kurma. Zindan Keşif düşman element bilgisini sağlar (Auto-Suggest için), Takım Kurma aktif takımı sağlar (savaş başlatma için).

## Tuning Knobs

| Knob | Değer | Güvenli Aralık | Çok Yüksekse | Çok Düşükse |
|------|-------|----------------|-------------|-------------|
| `max_team_size` | 4 | 3–6 | Sinerji tablosu genişler, dengeleme zorlaşır | Taktik derinlik azalır, sinerji anlamsız |
| `free_preset_slots` | 3 | 2–5 | Elmas sink azalır, monetizasyon etkisi düşer | Oyuncu farklı içerikler için takım kaydedemez, UX kötüleşir |
| `base_preset_cost` | 50 | 25–200 | Ek slot'lar erişilemez → oyuncu frustrasyonu | Elmas sink çok ucuz → monetizasyon değeri düşer |
| `preset_cost_exponent_base` | 2 | 1.5–3 | Geç slot'lar astronomik pahalı | Maliyet artışı hissedilmez |
| `max_preset_slots` | 10 | 6–15 | Arayüz karmaşıklaşır | Koleksiyoner oyuncular tatmin olmaz |
| `preset_name_max_length` | 20 | 10–30 | Arayüzde taşma riski | İfade özgürlüğü kısıtlı |
| `power_threshold_green` | 1.00 | 0.80–1.20 | "Hazırsın" çok kolay → yanıltıcı | "Hazırsın" çok zor → gereksiz stres |
| `power_threshold_yellow` | 0.70 | 0.50–0.90 | Sarı uyarı çok geç → sürpriz zorluk | Sarı uyarı çok erken → gereksiz endişe |
| `autosuggest_advantage_multiplier` | 1.50 | 1.25–2.00 | Element avantajı öneri'de çok baskın → güçsüz ama avantajlı canavar seçilir | Element avantajı önemsizleşir → saf güç seçimi |
| `autosuggest_disadvantage_multiplier` | 0.75 | 0.50–0.90 | Dezavantajlı canavar asla önerilmez | Dezavantaj farkı yok — rastgele seçim gibi |

**Etkileşim Uyarıları**:
- `max_team_size` değiştirilirse Element Sistemi sinerji tablosu da güncellenmeli (mevcut tablo 4 canavar varsayar)
- `autosuggest_advantage_multiplier` ve `autosuggest_disadvantage_multiplier` değerleri Element Sistemi'ndeki `element_advantage_multiplier` (1.50) ve `element_disadvantage_multiplier` (0.75) ile aynı tutulmalı — tutarsızlık oyuncuyu şaşırtır
- `power_threshold_green` ve `power_threshold_yellow` Zindan Keşif GDD'sindeki zorluk eğrisiyle kalibre edilmeli

## Visual/Audio Requirements

### VFX Gereksinimleri

| Olay | VFX | Süre | Öncelik |
|------|-----|------|---------|
| Canavar slot'a yerleştirildiğinde | Kart hafif parlama + slot'a "oturma" animasyonu | 0.3s | MVP |
| Canavar slot'tan çıkarıldığında | Kart soluklaşma + slot boşalma | 0.2s | MVP |
| Sinerji aktifleştiğinde | Aynı element canavarlar arasında element renginde bağlantı çizgisi + sinerji ikonu belirme | 0.5s | MVP |
| Sinerji seviye artışında (2→3→4) | Bağlantı çizgisi kalınlaşır, parlama yoğunlaşır | 0.3s | Nice-to-have |
| Güç skoru artışı (sinerji ile) | Sayı yeşil renkte yukarı kayarak güncellenir | 0.4s | MVP |
| Auto-Suggest tamamlandığında | 4 slot'a hızlı seri "tık-tık-tık-tık" yerleşme animasyonu | 0.8s | MVP |
| Preset yüklendiğinde | Slot'lar aynı anda fade-swap ile güncellenir | 0.3s | MVP |
| Güç eşiği — yeşil | Güç skoru çevresi yeşil parlama | sürekli | MVP |
| Güç eşiği — sarı | Güç skoru çevresi sarı nabız | sürekli | MVP |
| Güç eşiği — kırmızı | Güç skoru çevresi kırmızı nabız (daha hızlı) | sürekli | MVP |

### İkon Gereksinimleri

| İkon | Boyut | Kullanım Yeri |
|------|-------|---------------|
| Element ikonu (×4) | 32×32 px | Canavar kartı üzerinde (Element Sistemi GDD'sinden referans) |
| Avantaj/dezavantaj okları | 24×24 px | Zindan bağlamında canavar kartı köşesi |
| Sinerji bağlantı ikonu | 40×40 px | Takım paneli — aktif sinerji göstergesi |
| Preset slot ikonu | 48×48 px | Preset seçim panelinde |
| Auto-Suggest butonu ikonu | 48×48 px | "Öner" butonu (yıldız/sihirli değnek motifi) |
| Kilit ikonu | 24×24 px | Kilitli preset slotlarında (elmas ile aç) |

### Audio Gereksinimleri

| Olay | Ses Türü | Ton | Öncelik |
|------|----------|-----|---------|
| Canavar slot'a yerleşme | Kısa "tok" ses — kart masaya oturma hissi | Pozitif, tatmin edici | MVP |
| Canavar slot'tan çıkarma | Hafif "swoosh" | Nötr | MVP |
| Sinerji aktifleşme | Melodic chime — artan nota (Element Sistemi audio ile aynı) | Ödüllendirici | MVP |
| Auto-Suggest tamamlanma | Hızlı 4'lü "tık" serisi + final "ta-da" | Eğlenceli, hızlı | MVP |
| Preset yükleme | Kısa "click" | Nötr, hızlı | MVP |
| Güç skoru artışı | Yükselen ton (sayı büyüdükçe) | Güç hissi | Nice-to-have |
| Preset slot açma (elmas) | Kilit açılma sesi + parlama | Ödüllendirici | MVP |

## UI Requirements

### Takım Kurma Ana Ekranı
- Ekranın üst yarısı: 4 takım slotu (yatay dizi), her slot 80×100 dp
- Her slot: canavar portresi, element ikonu (sol üst), güç skoru (alt), nadirlik çerçeve rengi
- Boş slot: noktalı çerçeve + "+" ikonu
- Slot'lar arası sinerji bağlantı çizgisi (aynı element canavarlar arasında)
- Slot dizisinin altında: takım güç skoru (büyük font) + sinerji bonus özeti

### Canavar Seçim Paneli
- Ekranın alt yarısı: kaydırılabilir canavar listesi (grid — 4 sütun)
- Her canavar kartı: 80×100 dp, portre + element ikonu + güç skoru + nadirlik çerçeve
- Zindan bağlamında: avantaj/dezavantaj oku kartın köşesinde (↑ yeşil, ↓ kırmızı)
- Üst kısımda filtre/sıralama barı: element filtreleri (4 ikon toggle), arketip filtre, sıralama dropdown
- Seçili canavar: kart parlak çerçeve + slot numarası rozeti

### Preset Yönetim Paneli
- Ekranın sol kenarında veya üst kısımda: yatay preset tab'ları
- Her tab: preset ismi (düzenlenebilir) + dokunarak aktifleştir
- Kilitli slotlar: kilit ikonu + elmas maliyet etiketi
- Aktif preset: vurgulanmış çerçeve

### Auto-Suggest Butonu
- Canavar seçim panelinin sağ üst köşesinde: "Öner" butonu (yıldız ikonu + metin)
- Tek dokunuşla 4 slot dolar, mevcut seçim üzerine yazar
- Zindan bağlamında buton etiketi: "Zindana Göre Öner"

### Zindan Güç Eşiği Göstergesi
- Zindan girişinde: takım güç skoru yanında "/" ile önerilen güç (ör: "1330 / 1500")
- Renk kodlu (yeşil/sarı/kırmızı) + mesaj ("Hazırsın!" / "Zorlu olabilir" / "Çok güçlü düşmanlar!")
- "Savaş Başlat" butonu her zaman aktif — renk sadece bilgilendirme

### Kat Arası Düzenleme Ekranı
- Tam takım kurma ekranı açılır (aynı layout)
- Üst kısımda: "Kat [N] Tamamlandı — Takımını Düzenle" başlığı
- "Sonraki Kata Geç" butonu (en az 1 canavar seçiliyken aktif)

**Minimum Dokunma Hedefi**: Tüm butonlar, kartlar ve ikonlar minimum 44×44 dp dokunma alanı (mobil erişilebilirlik — technical-preferences.md ile tutarlı).

> **UX Flag — Takım Kurma**: Bu sistem kapsamlı UI gereksinimleri içeriyor. Phase 4'te (Pre-Production) `/ux-design` çalıştırarak takım kurma ekranı için UX spec oluşturulmalı — stories `design/ux/takim-kurma.md`'yi referans almalı, GDD'yi doğrudan değil.

## Acceptance Criteria

1. **GIVEN** oyuncunun koleksiyonunda 4+ canavar, **WHEN** takım kurma ekranı açılırsa, **THEN** canavarlar güç skoruna göre sıralı listelenir ve 4 slot boş gösterilir.

2. **GIVEN** 4 farklı element canavar (Ateş, Su, Toprak, Hava) seçili, **WHEN** güç skoru hesaplanırsa, **THEN** team_power = Σ(HP+ATK+DEF+SPD) sinerji bonusu yok (her elementten 1).

3. **GIVEN** 3 Ateş + 1 Su canavar seçili, **WHEN** sinerji hesaplanırsa, **THEN** 3 Ateş canavar +15% ATK ve +10% DEF alır, Su canavar bonus almaz, güç skoru buna göre güncellenir.

4. **GIVEN** 4 aynı element canavar seçili, **WHEN** sinerji hesaplanırsa, **THEN** tüm 4 canavar +20% ATK, +15% DEF, +10% SPD alır ve güç skoru yeşil artış gösterir.

5. **GIVEN** zaten slot 2'de bulunan canavar, **WHEN** oyuncu aynı canavarı slot 3'e sürüklerse, **THEN** canavar slot 2'den çıkar ve slot 3'e taşınır (duplicate engeli).

6. **GIVEN** Ateş zindanı girişi (düşmanlar çoğunluk Ateş), **WHEN** "En İyi Takımı Öner" basılırsa, **THEN** Su canavarlar öncelikli seçilir (fit_score = power × 1.5).

7. **GIVEN** oyuncunun 2 canavarı var, **WHEN** Auto-Suggest basılırsa, **THEN** 2 canavar seçilir, 2 slot boş kalır.

8. **GIVEN** 3 ücretsiz preset dolu, **WHEN** 4. slot açma butonu basılırsa, **THEN** 50 elmas maliyeti gösterilir. Onaylanırsa elmas düşer ve slot açılır.

9. **GIVEN** 5. slot açma, **WHEN** maliyet hesaplanırsa, **THEN** 100 elmas (50 × 2^1).

10. **GIVEN** preset'e "Ateş Takımı" ismi verilmiş ve 4 canavar kaydedilmiş, **WHEN** farklı bir ekrandan preset seçilirse, **THEN** tek dokunuşla 4 slot güncellenir ve isim gösterilir.

11. **GIVEN** preset'teki 1 canavar satılmış, **WHEN** preset yüklenirse, **THEN** 3 dolu + 1 boş slot gösterilir, preset silinmez.

12. **GIVEN** takım güç skoru 400, zindan önerilen güç 500, **WHEN** zindan girişi açılırsa, **THEN** sarı gösterge ve "Zorlu olabilir" mesajı gösterilir (400/500 = %80, %70–99 aralığı).

13. **GIVEN** takım güç skoru 200, zindan önerilen güç 600, **WHEN** zindan girişi açılırsa, **THEN** kırmızı gösterge gösterilir ama "Savaş Başlat" butonu aktif kalır (giriş engellenmez).

14. **GIVEN** zindan kat 3 tamamlanmış, **WHEN** kat arası ekran açılırsa, **THEN** takım düzenleme aktif — canavar ekle/çıkar/preset değiştir mümkün.

15. **GIVEN** savaş devam ediyor, **WHEN** oyuncu takım değiştirmeye çalışırsa, **THEN** işlem engellenir — takım kilitli.

16. **GIVEN** elmas bakiyesi 30, **WHEN** 4. slot açma (50 elmas) denenirse, **THEN** "Yeterli elmas yok" mesajı ve buton devre dışı.

17. **GIVEN** oyuncunun 0 canavarı var, **WHEN** takım kurma ekranı açılmaya çalışılırsa, **THEN** ekran engellenir ve "Henüz canavarın yok!" mesajı gösterilir.

*`qa-lead` not consulted — Lean mode (agent connection failed). Review manually before production.*

## Open Questions

1. **Arena takım formatı**: Arena'da ayrı savunma ve saldırı takımı mı olacak, yoksa tek takım mı? → Arena (Asenkron PvP) GDD'sinde tanımlanacak (Tier 2).

2. **Kat arası düzenleme sıklığı**: Her kat arasında mı, yoksa her 5. kat gibi belirli aralıklarla mı? → Zindan Keşif Sistemi GDD'sinde tanımlanacak.

3. **Takım tarihçesi**: Oyuncu en son hangi takımla hangi zindanı geçtiğini görebilmeli mi? → UX kararı, düşük öncelik.

4. **Auto-Suggest gelişmiş modu**: İleride arketip dengesi (1 Tank + 1 Destekçi + 2 Saldırgan gibi) de gözetmeli mi? → Tier 2+ geliştirmesi olarak değerlendirilecek.

5. **Preset paylaşımı**: Oyuncular preset'lerini sosyal medyada veya oyun içi paylaşabilmeli mi? → Tier 3+ sosyal özellik.
