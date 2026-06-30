# Concept Prototype Report: Tur Bazlı Savaş

> **Date**: 2026-06-29
> **Prototype Path**: HTML
> **Concept File**: design/gdd/game-concept.md

---

## Hypothesis

Eğer düşman tam ekran kameraya bakıyorsa ve oyuncu cooldown tabanlı yeteneklerle
savaşıyorsa, savaş geleneksel RPG'den daha tehdit edici ve sinematik hissedecek —
oyuncu auto-battle varken bile manuel müdahale etmek isteyecek.

---

## Riskiest Assumption Tested

**En riskli varsayım:** Auto-battle her zaman optimal seçenek olduğunda, oyuncunun
manuel oynaması için bir neden kalmaz — combat monoton hale gelir.

**Sonuç:** Varsayım çürütüldü (olumlu anlamda). Oyuncu auto-battle'ı denemeden önce
manuel oynamayı seçti ve cooldown yönetimini eğlenceli buldu.

---

## Approach

İki savaş sistemi aynı anda test edildi — oyuncu ikisini de oynayıp karşılaştırdı:

**Mod A — Tur Bazlı (test edilen konsept):**
- Düşman tam ekran, kameraya bakıyor, nefes animasyonu
- 3 yetenek: Saldır (her tur), Güçlü Darbe (3 tur CD), İyileş (5 tur CD)
- Auto-battle toggle (açıkken yapay zeka oynar)
- Hasar sayıları, hit flash, düşman saldırısında roar efekti

**Mod B — Aksiyon/MMORPG (karşılaştırma için):**
- Aynı tam ekran düşman sunum
- Gerçek zamanlı sürekli savaş (iki taraf aynı anda vurur)
- 5 buton: Normal (1s), Ağır (5s), Alev Dalgası (12s), Can Potu (5s), Mana Potu (5s)
- Gerçek zamanlı cooldown sayacı overlay'leri

**Path chosen:** HTML
**Reason for path:** Tur bazlı combat timing-sensitive değil; full-screen sunum ve
cooldown kararlarının ilginç olup olmadığı HTML'de test edilebilir.

**Shortcuts taken (intentional):**
- Emoji placeholder görseller (gerçek monster art yok)
- Hardcoded hasar değerleri
- Tek düşman, tek kat
- Ses yok, menü yok, kayıt yok
- Pet sistemi, element avantajı, zindan katları yok

---

## Result

- **Her iki mod da beklentinin üzerinde zevk verdi.** Oyuncu "beklediğimden daha
  fazla zevk verdi" şeklinde ifade etti.
- **Tam ekran düşman "güçlü bir his" uyandırdı.** Konseptin merkezindeki görsel
  hipotez doğrulandı.
- **MMORPG modunda ilk oynamada kayıp** — düşman hasarı biraz yüksek, ama ölümcül
  olmayan zorluk seviyesi. İkinci oynamada kazanılabilir.
- **Kritik bulgu:** MMORPG modu AI video saldırı animasyonlarıyla uyumsuz.
  Düşman her 1.8 saniyede saldırıyor; bu frekansta tam ekran video animasyonu
  çakışır ve kaotik görünür. Tur bazlı modda video animasyonu doğal olarak fit
  oluyor: düşmanın sırası → video oynar → hasar → oyuncunun sırası.
- **Oyuncu MMORPG modunu ilginç bulmasına rağmen tur bazlıya karar verdi** —
  hem eğlence kalitesi hem de teknik uyumluluk (video animasyonları) gerekçesiyle.

---

## Metrics

| Metric | Value |
|--------|-------|
| Path used | HTML |
| Iterations to playable | 2 (ikinci versiyonda MMORPG modu yeniden yazıldı) |
| Prototype duration | ~1 saat |
| Playtesters | 1 (geliştirici) |
| Feel assessment | Tam ekran düşman "güçlü his" yarattı; cooldown kararları eğlenceli; MMORPG daha hızlı ve reaktif |
| Hypothesis verdict | CONFIRMED |

---

## Recommendation: PROCEED

Hipotez doğrulandı: tam ekran düşman + cooldown tabanlı tur savaşı eğlenceli hissettiriyor.
Auto-battle tehlikesine rağmen oyuncu manuel oynamayı seçti. Üstelik bu sistem
projenin AI video animasyon akışıyla (Kling AI) doğrudan uyumlu — saldırı videoları
sıra gelince temiz bir şekilde oynatılabilir. MMORPG modu teknik olarak video
animasyonlarıyla çakışıyor, bu yüzden tur bazlı sistem hem tasarım hem üretim
pipeline'ı açısından doğru seçim.

---

## If Proceeding

**Core tuning values discovered:**
- Düşman hasar bandı 10-22 makul; MMORPG'de 12-22 biraz yüksek hissettirdi
- Cooldown oranları (1 tur / 3 tur / 5 tur) dengeli — Güçlü Darbe için bekleme süresi doğal hissettirdi
- EXP ekranı seviye animasyonu tatmin ediciydi — bu his korunmalı

**Assumptions confirmed:**
- Tam ekran düşman görsel = daha yoğun, sinematik savaş hissi ✓
- Cooldown yönetimi eğlenceli karar döngüsü yaratıyor ✓
- Auto-battle toggle faydalı (oyuncu test etti, tercih etti) ✓

**Assumptions disproved:**
- "MMORPG modu video animasyonlarıyla da çalışabilir" — çalışmaz. Sürekli saldırı
  frekansı video oynatmaya uygun değil. Tur bazlı bu problemi yapısal olarak çözüyor.

**Emergent mechanics:**
- Oyuncu MMORPG modunda can potunu "reaktif" kullandı — bu "tehlike anını takip etme"
  hissi Tur Bazlı modda farklı bir şekilde yakalanabilir (boss saldırısı öncesi uyarı).
- İki mod karşılaştırması oyuncuyu daha bilinçli karar vermeye itti — bu
  yaklaşım (A/B test aynı prototipta) ileride mekanik kararlarında kullanılabilir.

**Video animasyon kararı:**
- Tur bazlı: Düşmanın sırası → Kling AI videosu tam ekran oynar → hasar → oyuncunun sırası. ✓
- MMORPG: Video sürekli overlap olur, çalışmaz. ✗
- Sonuç: Tur bazlı sistem AI video pipeline'ıyla %100 uyumlu.

**Next steps:**
1. GDD revizyonları: `hibrit-savas.md`, `canavar-toplama-evrim.md` — prototype bulgularıyla güncelle
2. `/design-review design/gdd/hibrit-savas.md` — EXP ödül ekranı, tur yapısı doğrula
3. Gerçek Unity projesini oluştur: `prototypes/tur-bazli-savas-concept/` → `src/` değil
4. Kling AI'da ilk 3 yaratık saldırı videosu üret (prototype'taki hasar bandına göre)

---

## Lessons Learned

- **Yaparak kırılan varsayım:** "MMORPG modu da video animasyonlarıyla çalışabilir."
  Pratikte test etmeden bu kısıtı göremezdik.

- **Brainstorm'da çıkmayan sürpriz:** İki modu aynı anda karşılaştırarak test etmek,
  oyuncunun tercihini çok daha net ortaya çıkardı. Tek bir mod test edilseydi
  "iyi mi kötü mü" net bilinemezdi.

- **Farklı test ederdik:** Oyuncu olan geliştirici — taze gözle test eden biri olsaydı
  ilk izlenim daha güçlü data verirdi. Google Play soft launch bu veriyi sağlayacak.

---

> *Prototype code location: `prototypes/tur-bazli-savas-concept/`*
> *Bu kod throwaway. Asla production'a taşıma — sıfırdan yaz.*
