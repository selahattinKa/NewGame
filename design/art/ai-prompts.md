# AI Prompt Rehberi — Canavar Zindanları

> Tüm görseller ve videolar için kullanıma hazır prompt şablonları.
> Araç: Leonardo.ai / Stable Diffusion / Kling AI / Suno AI / ElevenLabs
> Güncellendi: 2026-06-26
>
> **NOT (2026-07-02)**: Element sistemi (Ateş/Su/Toprak/Hava) prototype kapsamından oyun mekaniği olarak tamamen kaldırıldı. Bu dosyadaki "element" başlıkları/gruplamaları artık sadece **görsel tema/palet çeşitliliği** referansıdır — hiçbir gameplay sistemine bağlı değildir. Prompt metinleri (İngilizce "fire element", "water element" vb.) AI görsel aracına verilecek ham metin olduğundan aynen bırakıldı; bunlar sadece görsel stil ipucu, oyun içi mekanik değil.

---

## BÖLÜM 1 — GENEL STİL REHBERİ

Her promptta şu **temel stil bloğunu** kullan. Tutarlılık için zorunlu.

### Görsel Temel Stil Bloğu
```
fantasy mobile RPG card art, highly detailed digital painting,
vibrant colors, professional game illustration, portrait orientation,
sharp focus, dramatic lighting
```

### Kaçınılacak Şeyler (Negative Prompt)
```
blurry, low quality, watermark, text, signature, ugly, deformed,
extra limbs, bad anatomy, poorly drawn, cartoon, anime, chibi,
realistic photo, 3D render, NSFW, multiple creatures, border
```

---

## BÖLÜM 2 — YARATIK / PET KART GÖRSELLERİ

### Tier Renk ve Güç Rehberi

| Tier | Renk | Görsel Ton | Creature Boyutu |
|------|------|-----------|-----------------|
| F | Gri / Gümüş | Sıradan, zayıf, küçük | Mini, basit |
| D | Yeşil | Biraz tehdit edici | Küçük-orta |
| C | Mavi | Orta güç, net özellikler | Orta |
| B | Mor | Güçlü, karanlık hava | Orta-büyük |
| A | Turuncu / Altın | Etkileyici, güçlü aura | Büyük |
| S | Kırmızı / Erguvan | Görkemli, görsel güçler belirgin | Çok büyük |
| SS | Altın / Gökkuşağı | Efsanevi, ışıl ışıl, tanrısal | Devasa |

---

### ŞABLON — Kart Görseli (Şeffaf arka plan için beyaz BG)

```
[YARATIK_ADI], [ELEMENT] element fantasy creature,
[TIER_TANIM], front-facing, looking directly at viewer,
centered composition, isolated on pure white background,
fantasy RPG mobile game card art, highly detailed digital painting,
vibrant [RENK] color palette, dramatic rim lighting,
professional game illustration, portrait orientation 512x768,
sharp focus, no background details, clean edges

Negative: blurry, low quality, watermark, text, multiple creatures,
background scenery, border, frame, bad anatomy, extra limbs
```

---

### TEMA TANIMLAMALARI (görsel çeşitlilik — gameplay mekaniği değil)

**Ateş Elementi:**
```
fire element, flames, burning aura, ember glow, hot colors (red orange yellow),
heat distortion effect, molten scales/fur, fire trails
```

**Su Elementi:**
```
water element, aquatic creature, glowing blue aura, water droplets,
bioluminescent patterns, deep ocean colors (blue teal cyan),
flowing fins, wave effects around body
```

**Toprak Elementi:**
```
earth element, stone armor, crystal growths, mossy texture,
earthy colors (brown green gray), ancient and sturdy appearance,
rocky spikes, nature patterns
```

**Hava Elementi:**
```
air element, wind currents visible around body, feathers or wings,
lightning sparks, silver white purple color palette,
storm clouds forming around creature, floating debris
```

---

### ÖRNEK YARATIKLAR — 4 TEMA × 3 TİER (F/C/S)

> Bu 12 görsel prototype için yeterli. Her birini ayrı ayrı üret.
> Boyut: 512×768 px, PNG, beyaz arka plan → rembg ile şeffaflaştır.

---

#### ATEŞ ELEMENTİ

**[F Tier] Ateş Yavrusu (Ember Pup)**
```
small fire fox puppy creature, F tier weak monster, tiny and cute but slightly
threatening, small flickering flames on tail and ears, ember glow eyes,
reddish orange fur with fire patterns, front-facing, looking at viewer,
isolated on pure white background, fantasy mobile RPG card art,
highly detailed digital painting, vibrant warm colors, 512x768 portrait,
sharp clean edges

Negative: blurry, low quality, large creature, background, border,
multiple creatures, text, watermark
```

**[C Tier] Ateş Tilkisi (Flame Fox)**
```
medium-sized fire fox creature, C tier evolved monster, more powerful
appearance than before, large flames for tail and mane, glowing amber eyes,
orange red fire patterns on dark fur, small horns beginning to form,
front-facing, looking at viewer with intensity, isolated on pure white
background, fantasy mobile RPG card art, highly detailed digital painting,
512x768 portrait, dramatic fire lighting effects

Negative: blurry, low quality, background scenery, border, text, watermark
```

**[S Tier] Ateş Lordu Tilki (Inferno Lord Fox)**
```
massive powerful fire fox beast, S tier legendary evolved monster,
enormous flaming tails (multiple), full body engulfed in controlled flames,
glowing crimson red eyes with intensity, obsidian black fur with lava crack
patterns, golden crown-like horns, radiating intense heat aura,
intimidating and majestic, front-facing, looking directly at viewer,
isolated on pure white background, fantasy mobile RPG card art,
extremely highly detailed digital painting, 512x768 portrait,
cinematic fire effects, awe-inspiring

Negative: blurry, low quality, background, border, text, watermark, weak
```

---

#### SU ELEMENTİ

**[F Tier] Su Kertenkele (Ripple Lizard)**
```
small aquatic lizard creature, F tier weak monster, tiny blue scales,
small water droplets around body, innocent large eyes, simple fins on back,
light blue translucent appearance, front-facing, looking at viewer,
isolated on pure white background, fantasy mobile RPG card art,
highly detailed digital painting, cool blue color palette, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

**[C Tier] Mercan Ejderi (Coral Drake)**
```
medium water dragon creature, C tier evolved monster, vibrant teal and blue
scales, bioluminescent stripe patterns, elegant water fins extended,
glowing blue eyes, small water whirlpool forming around feet,
front-facing, looking at viewer, isolated on pure white background,
fantasy mobile RPG card art, highly detailed digital painting,
512x768 portrait, underwater light caustic effects

Negative: blurry, low quality, background scenery, border, text, watermark
```

**[S Tier] Derin Deniz Leviathan (Abyss Leviathan)**
```
enormous deep sea serpent creature, S tier legendary evolved monster,
massive coiled body fills frame, ancient deep ocean darkness aesthetic,
glowing bioluminescent patterns across entire body, crushing water pressure
aura, multiple rows of razor teeth, void-black and electric blue coloring,
terrifying and majestic, front-facing head looking directly at viewer,
isolated on pure white background, fantasy mobile RPG card art,
extremely highly detailed digital painting, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

---

#### TOPRAK ELEMENTİ

**[F Tier] Taş Kaplumbağa (Pebble Turtle)**
```
small stone shell turtle creature, F tier weak monster, mossy green and gray
rocky appearance, tiny crystal growths on shell, stubby legs, cute but sturdy,
front-facing, looking at viewer, isolated on pure white background,
fantasy mobile RPG card art, highly detailed digital painting,
earthy color palette (brown green gray), 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

**[C Tier] Kristal Golem (Crystal Golem)**
```
medium sized earth golem creature, C tier evolved monster, body made of
living rock and crystal formations, glowing green gems embedded in stone
chest, moss and vines growing on shoulders, heavy powerful stance,
cracked stone texture with inner glow, front-facing, looking at viewer,
isolated on pure white background, fantasy mobile RPG card art,
highly detailed digital painting, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

**[S Tier] Toprak Devası (Titan Earthbreaker)**
```
colossal earth titan creature, S tier legendary evolved monster,
mountain-sized stone body covered in ancient runes, massive crystal spires
growing from back and shoulders, enormous glowing green eyes like emeralds,
ancient tree roots wrapped around stone body, ground cracks beneath weight,
intimidating and ancient, front-facing, looking at viewer,
isolated on pure white background, fantasy mobile RPG card art,
extremely highly detailed digital painting, 512x768 portrait, epic scale

Negative: blurry, low quality, background, border, text, watermark
```

---

#### HAVA ELEMENTİ

**[F Tier] Rüzgar Fareyi (Gust Mouse)**
```
tiny wind mouse creature, F tier weak monster, small fluffy body with
tiny wings, light gray white fur, small lightning sparks around ears,
floating slightly off ground, playful appearance, front-facing,
looking at viewer, isolated on pure white background,
fantasy mobile RPG card art, highly detailed digital painting,
silver white color palette, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

**[C Tier] Fırtına Kartalı (Storm Eagle)**
```
medium storm eagle creature, C tier evolved monster, powerful wingspan
partially visible, feathers crackling with electricity, silver and dark
purple plumage, glowing yellow storm eyes, wind currents visibly swirling
around body, sharp dangerous beak, front-facing, looking at viewer,
isolated on pure white background, fantasy mobile RPG card art,
highly detailed digital painting, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

**[S Tier] Şimşek Ejderi (Thunder Sovereign)**
```
massive thunder dragon creature, S tier legendary evolved monster,
enormous scaled wings creating hurricane winds, entire body crackling
with intense lightning, dark storm cloud forming around head,
electric purple and silver coloring, glowing white storm eyes,
terrifying and godlike, lightning strikes the ground below,
front-facing, looking directly at viewer,
isolated on pure white background, fantasy mobile RPG card art,
extremely highly detailed digital painting, 512x768 portrait

Negative: blurry, low quality, background, border, text, watermark
```

---

## BÖLÜM 3 — SAVAŞ EKRANI GÖRSELLERİ (TAM EKRAN, KAMERAYA BAKIYOR)

> Savaş ekranında yaratık tam ekranı doldurur ve DOĞRUDAN kameraya bakar.
> Boyut: 1080×1920 px (portrait), zindan arka planı dahil (şeffaf gerekmez)

### Şablon — Savaş Ekranı
```
[YARATIK_ADI] monster, [ELEMENT] creature, EXTREME CLOSE UP portrait,
filling entire frame, staring directly into camera with intimidating
threatening expression, about to attack the viewer, dramatic dungeon
lighting from below, dark stone dungeon background with torch light,
motion blur on edges, intense gaze, mouth slightly open showing fangs/teeth,
fantasy RPG mobile game, cinematic quality, 1080x1920 portrait orientation,
extremely detailed, dramatic shadows, "looking at YOU" composition,
first person perspective feel

Negative: blurry, low quality, multiple creatures, peaceful expression,
looking away, small in frame, top-down view, side view
```

### Hazır Savaş Ekranı Promptları (4 tema)

**Ateş — Savaş Ekranı:**
```
Inferno Lord Fox massive fire creature, extreme close up portrait,
filling entire 1080x1920 frame, staring directly into camera,
intense crimson glowing eyes locked on viewer, flames erupting around face,
threatening open mouth showing fire fangs, heat distortion effect,
dark dungeon background with fire glow, dramatic lighting from below,
about to breathe fire at the viewer, cinematic mobile RPG quality,
first person intimidating perspective

Negative: small creature, looking away, peaceful, multiple creatures,
side view, blurry, watermark
```

**Su — Savaş Ekranı:**
```
Abyss Leviathan enormous sea serpent, extreme close up portrait,
filling entire 1080x1920 frame, massive head staring directly at camera,
glowing bioluminescent eyes locked on viewer, rows of sharp teeth visible,
water dripping from enormous jaw, crushing deep ocean pressure aura,
dark wet dungeon stone background with water reflections,
terrifying and inevitable, cinematic mobile RPG quality,
first person intimidating perspective

Negative: small creature, looking away, peaceful, multiple creatures, blurry
```

**Toprak — Savaş Ekranı:**
```
Titan Earthbreaker colossal stone golem, extreme close up portrait,
filling entire 1080x1920 frame, massive stone face looking directly at camera,
enormous glowing emerald eyes focused on viewer, stone fist raised to strike,
ancient runes glowing on forehead, rocks and debris floating around,
dark dungeon background with torchlight on stone walls,
slow unstoppable ancient power, cinematic mobile RPG quality,
first person intimidating perspective

Negative: small creature, looking away, peaceful, multiple creatures, blurry
```

**Hava — Savaş Ekranı:**
```
Thunder Sovereign dragon close up portrait, filling entire 1080x1920 frame,
massive scaled face staring directly into camera with storm fury,
lightning crackling around enormous horns, electric purple glowing eyes,
storm winds visible as motion blur around edges, mouth open showing
crackling electricity between fangs, dark thundercloud background
with lightning flashes illuminating dungeon walls,
cinematic mobile RPG quality, first person intimidating perspective

Negative: small creature, looking away, peaceful, multiple creatures, blurry
```

---

## BÖLÜM 4 — ARKA PLAN / ORTAM GÖRSELLERİ

> Savaş ekranının arka planı, zindan geçiş ekranları için.
> Boyut: 1080×1920 px (portrait), karakter YOK.

### Zindan Ortamları

**Ateş Zindanı:**
```
dark volcanic dungeon interior, portrait orientation 1080x1920,
lava rivers flowing below stone walkway, fire torches on ancient stone walls,
heat haze visible in air, glowing ember ceiling, dramatic orange red lighting,
atmospheric depth of field, NO characters, mobile RPG background art,
highly detailed environment, cinematic quality

Negative: characters, creatures, people, blurry, modern elements
```

**Su Zindanı:**
```
underwater cave dungeon interior, portrait orientation 1080x1920,
bioluminescent plants on wet stone walls, water flooding lower half,
light caustics on ceiling from above water, deep blue teal lighting,
mysterious and deep atmosphere, NO characters, mobile RPG background art,
highly detailed environment, cinematic quality

Negative: characters, creatures, people, blurry, bright lighting
```

**Toprak Zindanı:**
```
ancient stone cave dungeon interior, portrait orientation 1080x1920,
massive crystal formations growing from walls and ceiling, moss covered
ancient stone pillars, green glowing crystal light sources,
deep earth atmosphere, stalactites and stalagmites, NO characters,
mobile RPG background art, highly detailed environment, cinematic quality

Negative: characters, creatures, people, blurry, modern elements, sunlight
```

**Hava Zindanı:**
```
sky fortress dungeon interior, portrait orientation 1080x1920,
open crumbling stone walls revealing stormy sky outside,
lightning flashing through broken ceiling, floating stone platforms,
wind visible as dust and debris, dramatic storm light,
purple and silver atmospheric lighting, NO characters,
mobile RPG background art, highly detailed environment, cinematic quality

Negative: characters, creatures, people, blurry, calm weather, modern
```

**Ana Menü Arka Planı:**
```
epic fantasy dungeon gate entrance, portrait orientation 1080x1920,
massive ancient stone door with glowing runes, torch light on either side,
mysterious fog at ground level, dramatic backlighting through door crack,
fantasy RPG mobile game main menu background, highly detailed,
cinematic quality, NO characters, atmospheric and mysterious

Negative: characters, creatures, people, blurry, modern elements
```

---

## BÖLÜM 5 — KART ÇERÇEVESİ TASARIMI

> Ideogram.ai veya Canva ile üret. PNG, şeffaf merkez (ortası boş).

### Şablon — Kart Çerçevesi
```
fantasy RPG card frame border design, [TİER_RENK] color scheme,
ornate decorative border, magical gemstone corners, portrait card shape
512x768, EMPTY TRANSPARENT CENTER for character portrait,
game UI card frame design, glowing [TİER_RENK] energy lines on border,
[TİER_SEVİYE] quality indicated in corner, dark background around frame,
professional mobile game card design, no character inside, frame only

Negative: character inside frame, full background, blurry, simple border
```

### Tier Başına Çerçeve Promptları

**F Tier — Gri Çerçeve:**
```
F tier fantasy RPG card frame, gray silver color scheme, simple stone carved
border, minimal decoration, small gray gem in each corner, worn and basic
appearance, portrait 512x768 transparent center, mobile game card UI design,
"F" grade indicator in corner, dark gray energy lines

Negative: elaborate decoration, gold, colorful gems, glowing strongly
```

**C Tier — Mavi Çerçeve:**
```
C tier fantasy RPG card frame, blue sapphire color scheme, moderately ornate
border with wave patterns, blue glowing gems in corners, mystical energy lines,
portrait 512x768 transparent center, mobile game card UI design,
"C" grade indicator, medium blue glow effect, polished appearance

Negative: gray, simple, gold, red, overly elaborate
```

**S Tier — Kırmızı/Erguvan Çerçeve:**
```
S tier fantasy RPG card frame, crimson red and deep purple color scheme,
highly ornate decorative border with dragon scale patterns, large glowing
crimson gems in corners and sides, intense energy glow effect,
fire and lightning patterns in border design, portrait 512x768 transparent
center, mobile game card UI design, "S" grade indicator, powerful aura effect,
premium prestigious appearance

Negative: gray, blue, simple, dull, no glow
```

---

## BÖLÜM 6 — SALDIRI ANİMASYONU VİDEOLARI

> Kling AI / Luma AI / Runway ile üret.
> Boyut: 1080×1920 px portrait, süre: 2-3 saniye, loop yapılabilir.
> **Siyah arka plan kullan** → Unity'de Additive blend mode ile şeffaf görünür.

### Şablon — Saldırı Videosu
```
[YARATIK_ADI] monster attacking toward camera, [ELEMENT] attack animation,
creature lunging or striking DIRECTLY AT VIEWER through screen,
[ELEMENT_EFEKT] effect expanding toward camera, dramatic motion blur,
isolated on PURE BLACK background, fantasy RPG game animation,
2-3 second loop, portrait orientation 1080x1920, cinematic slow motion detail,
creature getting LARGER as it approaches camera (zoom toward viewer)

Style: game attack animation, dramatic, impactful, fantasy RPG
```

### Hazır Video Promptları

**Ateş Tilkisi Saldırısı:**
```
Fire fox creature lunging directly at camera with massive fire breath attack,
flames and fire ball expanding toward viewer filling entire frame,
orange red inferno effect growing larger approaching camera,
pure black background, dramatic fire light illumination,
2-3 second attack animation loop, portrait 1080x1920,
fantasy RPG mobile game attack animation, slow motion impact detail
```

**Su Leviathan Saldırısı:**
```
Sea serpent creature striking directly at camera with water blast attack,
massive water surge and wave crashing toward viewer,
bioluminescent blue water explosion expanding to fill frame,
pure black background, underwater light caustics,
2-3 second attack animation loop, portrait 1080x1920,
fantasy RPG mobile game attack animation, slow motion water impact
```

**Toprak Golem Saldırısı:**
```
Stone golem creature slamming massive fist directly toward camera,
rocks and earth debris flying toward viewer filling screen,
green crystal energy shockwave expanding at camera,
pure black background, dust and stone particle effects,
2-3 second attack animation loop, portrait 1080x1920,
fantasy RPG mobile game attack animation, slow motion ground impact
```

**Hava Ejderi Saldırısı:**
```
Thunder dragon creature roaring and releasing lightning bolt directly at camera,
electric purple lightning expanding and filling entire frame toward viewer,
wind and storm energy surging toward screen,
pure black background, electric arc particle effects,
2-3 second attack animation loop, portrait 1080x1920,
fantasy RPG mobile game attack animation, slow motion thunder impact
```

**Evrim Animasyonu (Özel):**
```
Fantasy creature glowing intensely, magical evolution transformation effect,
bright white gold energy particles swirling around creature,
creature transforming radiating power outward toward camera,
pure black background, magical particle burst expanding to fill frame,
satisfying transformation animation 3-4 seconds, portrait 1080x1920,
fantasy RPG evolution ceremony animation, dramatic power reveal
```

---

## BÖLÜM 7 — MÜZİK PROMPTLARI

> Suno AI / Udio ile üret. Format: OGG veya MP3.

### Ana Menü Müziği
```
Epic fantasy mobile RPG main menu music, orchestral with electronic elements,
heroic and adventurous mood, builds anticipation, memorable main theme melody,
no vocals, seamless loop 2-3 minutes, professional game soundtrack quality,
similar to AFK Arena or Summoners War main theme, moderate tempo
```

### Keşif Alanı Müziği
```
Dark dungeon exploration music, tense atmospheric orchestral,
low strings and percussion, mysterious mood, subtle danger feeling,
no vocals, seamless loop 1-2 minutes, mobile RPG dungeon BGM,
tension building but not overwhelming, slow steady beat
```

### Savaş Müziği
```
Intense fantasy battle music, fast tempo orchestral with heavy drums,
epic and energetic, adrenaline pumping combat feel, no vocals,
seamless loop 1-2 minutes, mobile RPG battle BGM, brass and percussion heavy,
similar to dungeon RPG boss fight music, driving rhythm
```

### Boss Savaş Müziği
```
Epic boss battle music, extremely intense orchestral and electronic hybrid,
massive choir vocals allowed, overwhelming power feeling, dramatic tempo,
building intensity, mobile RPG final boss BGM, cinematic quality,
90-120 second seamless loop, dark and epic, strings + brass + heavy drums
```

### Zafer / Ödül Ekranı Müziği
```
Victory fanfare mobile RPG, short satisfying musical sting 5-10 seconds,
triumphant brass melody, reward and accomplishment feeling,
uplifting and celebratory, non-looping, game reward jingle quality,
fantasy RPG reward music, bright and satisfying resolution
```

### Evrim Anı Müziği
```
Magical evolution ceremony music, dramatic orchestral build up then explosion
of triumphant sound, emotional and powerful transformation feeling,
15-20 seconds non-looping, fantasy RPG evolution music,
starts quiet and mysterious then erupts into powerful fanfare,
choir and strings with brass finale
```

### Mağaza / Menü Müziği
```
Peaceful fantasy town market music, light and pleasant orchestral,
flute and harp melody, relaxed browsing mood, no tension, warm feeling,
no vocals, seamless loop 90 seconds, mobile RPG shop BGM,
inviting and comfortable atmosphere
```

---

## BÖLÜM 8 — SES EFEKTİ PROMPTLARI

> ElevenLabs Sound Effects veya Freesound.org ile.

### Yaratık Sesleri

**Ateş Yaratığı Saldırısı:**
```
Fire creature attack sound, aggressive flame breath roar, short 1 second,
fantasy RPG monster, fire crackling and intense roar combined,
threatening and powerful
```

**Su Yaratığı Saldırısı:**
```
Aquatic sea monster attack sound, deep underwater resonant roar,
water surge whoosh, 1 second, threatening deep sea creature,
fantasy RPG monster attack
```

**Toprak Yaratığı Saldırısı:**
```
Earth golem attack sound, heavy stone slam impact, ground shaking,
deep rumbling bass, 1 second, massive heavy creature strike,
fantasy RPG monster attack sound
```

**Hava Yaratığı Saldırısı:**
```
Thunder creature attack sound, electric lightning crack and wind burst,
sharp electric zap with wind whoosh, 1 second, fantasy RPG monster,
storm and thunder attack sound
```

### UI Sesleri

**Level Up:**
```
Level up chime sound effect, satisfying ascending musical ding,
celebratory and rewarding, mobile game level up sound, bright and clear,
0.5 seconds, positive achievement feeling
```

**Evrim:**
```
Creature evolution transformation sound, magical energy buildup then
powerful release burst, 2 seconds, fantasy RPG evolution sound effect,
starts with gentle shimmer then explodes into powerful magical tone
```

**Ödül Ekranı / Item Düşme:**
```
Item drop reward sound, satisfying clink or chime, treasure chest open feeling,
mobile RPG loot sound, pleasant and rewarding, 0.5-1 second
```

**UI Tıklama:**
```
Clean UI button click sound, light tap, mobile game interface click,
satisfying and crisp, 0.1 seconds, modern game UI sound
```

**Hasar Alma:**
```
Hit impact sound, creature taking damage, short sharp impact,
fantasy RPG hit sound, 0.2 seconds, clear and punchy
```

**Boss Girişi:**
```
Boss monster entrance sound, dramatic low frequency rumble building to roar,
3 seconds, intimidating and epic, fantasy RPG boss reveal sound,
screen shaking bass with monster roar
```

---

## BÖLÜM 9 — PROTOTYPE ÜRETIM ÇİZELGESİ

Prototype için minimum gerekli içerik ve üretim sırası:

### 1. Önce Üret (Görsel)
- [ ] 4 tema × 3 tier = **12 kart görseli** (Bölüm 2 promptları)
- [ ] 4 tema savaş ekranı = **4 tam ekran görsel** (Bölüm 3 promptları)
- [ ] 3 zindan arka planı = **3 ortam görseli** (Bölüm 4 promptları)
- [ ] 3 kart çerçevesi (F/C/S) = **3 UI görseli** (Bölüm 5 promptları)

### 2. Sonra Üret (Video)
- [ ] 4 tema saldırı videosu = **4 video** (Bölüm 6 promptları)
- [ ] 1 evrim animasyonu = **1 video** (Bölüm 6 — evrim promptu)

### 3. Son Üret (Ses)
- [ ] Ana menü müziği × 1
- [ ] Zindan müziği × 1
- [ ] Savaş müziği × 1
- [ ] Zafer sesi × 1
- [ ] Evrim sesi × 1
- [ ] UI sesleri (level up, tıklama, hasar) × 3-5

### Toplam Prototype Asset Sayısı
| Tür | Adet |
|-----|------|
| Kart görseli (PNG şeffaf) | 12 |
| Savaş ekranı görseli | 4 |
| Arka plan görseli | 3 |
| Kart çerçevesi (PNG) | 3 |
| Saldırı videosu | 5 |
| Müzik | 4 |
| Ses efekti | 6-8 |
| **TOPLAM** | **~37-39 asset** |

---

## BÖLÜM 10 — İPUÇLARI

### Tutarlılık İçin
- Her yaratık serisinin tüm tier'larını aynı oturumda üret
- "Same creature but more evolved and powerful" ifadesini ekle
- Aynı prompt'u 4-5 kez çalıştır, en iyi çıkanı seç

### Siyah Arka Plan Trick (Video için)
Videoları siyah arka planla üret, Unity'de şu ayarı yap:
`Material → Rendering Mode → Additive`
Siyah pikseller otomatik şeffaf olur, ek araç gerekmez.

### rembg Kurulumu (Ücretsiz, Sınırsız Arka Plan Kaldırma)
```bash
pip install rembg
rembg i girdi.png cikti_seffaf.png
```
Tüm kart görsellerini toplu işlemek için:
```bash
rembg p gorseller/ seffaf_gorseller/
```
