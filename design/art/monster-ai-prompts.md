# Canavar AI Görsel Prompt Seti — Prototype (F–D–C–B)

**Hedef araçlar:**
- Kart görseli: **Google ImageFX** (imagesfx.google.com)
- Animasyonlar: **Google Flow** (labs.google/flow) — Veo 3

**Prototype tier kapsamı:** F, D, C, B (A/S/SS MVP sonrası eklenir)

---

## Animasyon Clip Seti — Her Canavar İçin 5 Clip

| Dosya | Süre | Ne zaman oynar |
|-------|------|----------------|
| `idle.mp4` | 2–3 sn (loop) | Her zaman arka planda |
| `attack_light.mp4` | 2 sn | Normal Saldırı kullanılınca |
| `attack_heavy.mp4` | 2 sn | Ağır Saldırı kullanılınca |
| `hit_normal.mp4` | 1–2 sn | Canavar normal hasar alınca |
| `hit_heavy.mp4` | 1–2 sn | Canavar ağır hasar alınca |

**Flow genel kurallar:**
- Her prompt ayrı bir Flow session'ı — tek videoda birleştirme
- Clip sonunda canavar idle pozisyonuna dönmeli (geçişler yumuşak olsun)
- Süreyi Flow arayüzünde manuel ayarla
- Ses ipucunu her zaman ekle — boş bırakırsan Flow rastgele ses koyar
- **Arka plan:** Her promptun sonuna ekle: `Pure solid black background throughout.`

**ImageFX genel kurallar:**
- Doğal dil cümleleri — virgüllü keyword listesi çalışmaz
- En boy oranı: **2:3 portrait** seç
- Her promptun sonuna ekle: `Pure solid black background. Dark fantasy digital painting style, cinematic lighting, highly detailed.`
- Görselde arka plan çıkmazsa şunu da ekle: `Do not add any scenery, environment, or background details. Only the creature on a pure black background.`

---

## Tier F

---

### 1. Gölge Sıçanı
**Tip:** Saldırgan | **Tier:** F

#### ImageFX — Kart Görseli
```
A scraggly dark rat creature shown in a bust portrait, facing directly at the viewer.
Its fur is patchy and unkempt with faint wisps of shadow barely clinging to it.
The eyes are small and dull violet, not glowing brightly. It has small crooked fangs
and scrawny clawed forepaws. The overall look is weak and unremarkable, like a
common dungeon pest with a slight supernatural edge. Pure solid black background. Do not make the creature look
powerful, epic, or majestic. Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, cinematic lighting, highly detailed.
```

#### Flow — idle
```
Close-up bust shot of a scraggly shadowy rat creature facing the camera. It twitches
its nose slowly and its ears flick once. The faint shadow wisps on its fur drift
lazily. Its small violet eyes blink once. The creature is otherwise still. The clip
loops naturally from end back to start. Camera holds still.
Pure solid black background throughout.
Sound: faint ambient sewer dripping, occasional small skitter. 2-3 seconds.
```

#### Flow — attack_light
```
Close-up bust shot of a scraggly shadowy rat creature facing the camera. It suddenly
lunges its head forward and snaps its jaws at the camera, then quickly pulls back to
idle position. The movement is fast and twitchy. Shadow wisps briefly flare out
during the snap, then settle. Camera holds still.
Pure solid black background throughout.
Sound: a quick wet snap followed by a small hiss. 2 seconds.
```

#### Flow — attack_heavy
```
Close-up bust shot of a scraggly shadowy rat creature facing the camera. It crouches
low, shadow wisps on its fur swirling and expanding. It then launches its whole body
forward toward the camera with both claws extended, violet eyes flashing bright.
It pulls back to idle. Camera pushes in slightly during the lunge.
Pure solid black background throughout.
Sound: a building low shriek that cuts into a sharp claw scrape. 2 seconds.
```

#### Flow — hit_normal
```
Close-up bust shot of a scraggly shadowy rat creature facing the camera. It suddenly
flinches backward and to the side as if struck, head tilting away. Shadow wisps scatter
briefly. It recovers and returns to facing the camera. Camera holds still.
Pure solid black background throughout.
Sound: a small pained squeak. 1-2 seconds.
```

#### Flow — hit_heavy
```
Close-up bust shot of a scraggly shadowy rat creature facing the camera. A heavy
impact sends it reeling hard backward, shadow wisps exploding outward in all
directions. It shakes its head and slowly straightens back up, violet eyes dimmer
than before. Camera shakes once on impact.
Pure solid black background throughout.
Sound: a sharp crack followed by a pained yelp. 2 seconds.
```

---

### 2. Kil Golem
**Tip:** Tank | **Tier:** F

#### ImageFX — Kart Görseli
```
A small clay golem shown in a bust portrait, facing the viewer directly. Its body is
made of rough wet dark clay with visible cracks and patches of crumbling dry clay.
Two dim amber pinpoints glow faintly from its roughly-formed face where eyes would be.
Its fists are oversized and clumsy-looking. The creature looks primitive and stubborn
rather than powerful. Pure solid black background. Do not make it look large, imposing, or menacing.
Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, cinematic lighting, highly detailed.
```

#### Flow — idle
```
Medium close-up of a small clay golem facing the camera. Its body slowly rises and
falls with a heavy breathing-like motion. Small clay fragments occasionally crumble
and fall from its surface. The amber eye-glow pulses very faintly and slowly.
The clip loops naturally. Camera holds still.
Pure solid black background throughout.
Sound: low ambient rumble, occasional small crumble of dry clay. 2-3 seconds.
```

#### Flow — attack_light
```
Medium close-up of a small clay golem facing the camera. It raises one oversized fist
and brings it down in a simple downward punch, then returns to idle position. The
motion is slow and clumsy. A small clay chunk breaks loose on impact. Camera holds still.
Pure solid black background throughout.
Sound: a dull wet thud. 2 seconds.
```

#### Flow — attack_heavy
```
Medium close-up of a small clay golem facing the camera. It pulls both fists back,
amber eye-glow intensifying. It slams both fists forward simultaneously toward the
camera. Clay chunks scatter from its arms during the slam. It slowly pulls back to
idle. Camera shakes on impact.
Pure solid black background throughout.
Sound: a deep resonant clay slam, like a boulder hitting mud. 2 seconds.
```

#### Flow — hit_normal
```
Medium close-up of a small clay golem facing the camera. It rocks backward slightly
from an impact, a small crack forming on its chest. It steadies itself and faces
forward again. Amber eye-glow flickers once. Camera holds still.
Pure solid black background throughout.
Sound: a dull thwack against clay. 1-2 seconds.
```

#### Flow — hit_heavy
```
Medium close-up of a small clay golem facing the camera. A heavy blow sends it
lurching sideways. A large chunk of clay breaks off its shoulder and falls. It slowly
rights itself, a new crack glowing faintly orange. Camera shakes hard on impact.
Pure solid black background throughout.
Sound: a loud crack of clay splitting, pieces scattering. 2 seconds.
```

---

## Tier D

---

### 3. Buz Büyücüsü
**Tip:** Büyücü | **Tier:** D

#### ImageFX — Kart Görseli
```
An ice sorcerer shown in a bust portrait, facing the viewer directly with cold pale
blue glowing eyes. The robes are made of crystallized ice and frost. Sharp ice shards
float in orbit around the body. Frost breath is faintly visible in the cold air.
The skin is pale with blue-tinted veins and an elaborate ice crown sits on the head.
Pure solid black background. The expression is calculating and menacing.
Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, cinematic lighting, highly detailed.
```

#### Flow — idle
```
Close-up bust shot of an ice sorcerer facing the camera. Ice shards orbit the body
slowly and steadily. Frost breath drifts upward in a thin stream. The pale blue eyes
glow with a calm, cold light. The ice crown shimmers faintly. The clip loops naturally.
Camera holds still.
Pure solid black background throughout.
Sound: soft ambient wind, faint crystalline chime from orbiting ice shards. 2-3 seconds.
```

#### Flow — attack_light
```
Close-up bust shot of an ice sorcerer facing the camera. The sorcerer flicks one hand
forward. A single ice shard breaks from orbit and shoots toward the camera.
The sorcerer's expression remains cold and unmoved. The hand returns to rest.
Camera holds still.
Pure solid black background throughout.
Sound: a sharp crystalline crack and a brief whoosh. 2 seconds.
```

#### Flow — attack_heavy
```
Close-up bust shot of an ice sorcerer facing the camera. The sorcerer raises both
hands slowly, all orbiting ice shards spiraling inward toward the palms. The eyes
flash bright white-blue. The sorcerer thrusts both palms forward, releasing a burst
of ice crystals and frost toward the camera. Hands return to idle. Camera pushes in
slowly during the charge, then shakes on release.
Pure solid black background throughout.
Sound: a building crystalline hum erupting into a sharp ice blast. 2 seconds.
```

#### Flow — hit_normal
```
Close-up bust shot of an ice sorcerer facing the camera. The sorcerer recoils
slightly from an impact, head tilting back. One orbiting ice shard is knocked out
of orbit and falls. The sorcerer straightens, expression briefly showing irritation.
Camera holds still.
Pure solid black background throughout.
Sound: a sharp impact, a single ice shard shattering. 1-2 seconds.
```

#### Flow — hit_heavy
```
Close-up bust shot of an ice sorcerer facing the camera. A powerful hit throws the
sorcerer hard to one side. Several orbiting ice shards shatter and scatter. Frost
breath is disrupted. The sorcerer slowly rights themselves, eyes flashing with anger.
Camera shakes on impact.
Pure solid black background throughout.
Sound: a heavy blow, multiple ice shards exploding, a sharp cold exhale. 2 seconds.
```

---

## Tier C

---

### 4. Gölge Kurdu
**Tip:** Saldırgan | **Tier:** C

#### ImageFX — Kart Görseli
```
A large shadow wolf shown in a bust portrait, staring directly at the viewer with
piercing amber eyes. The fur is made of living shadows and dark smoke tendrils.
The body appears semi-transparent and ethereal while the snarling face is solid and
detailed. Faint lightning crackles silently within the shadowy fur. Pure solid black background. The expression is predatory and dangerous.
Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, cinematic lighting, highly detailed.
```

#### Flow — idle
```
Close-up bust shot of a shadow wolf facing the camera. Shadow tendrils drift slowly
off its fur. Its amber eyes scan left and right once, then settle back on the camera.
Its lips pull back slightly in a permanent low snarl. Faint silent lightning flickers
once in the fur. The clip loops naturally. Camera holds still.
Pure solid black background throughout.
Sound: low ambient forest wind, a very faint distant growl. 2-3 seconds.
```

#### Flow — attack_light
```
Close-up bust shot of a shadow wolf facing the camera. It snaps its jaws forward
once, fast and sharp, spectral fangs briefly visible. Shadow tendrils flare out
slightly on the snap. It immediately returns to idle snarl. Camera holds still.
Pure solid black background throughout.
Sound: a sharp supernatural snap. 2 seconds.
```

#### Flow — attack_heavy
```
Close-up bust shot of a shadow wolf facing the camera. It lowers its head, shadow
tendrils expanding and writhing aggressively, amber eyes flashing bright. It lunges
its entire upper body forward toward the camera, jaws wide open showing full spectral
fangs, a burst of shadow erupting outward. It snaps back to idle position.
Camera pushes in fast during lunge.
Pure solid black background throughout.
Sound: a deep resonant growl building into a vicious supernatural snarl. 2 seconds.
```

#### Flow — hit_normal
```
Close-up bust shot of a shadow wolf facing the camera. An impact jolts its head to
one side. Shadow tendrils scatter briefly. It shakes its head and locks its amber
eyes back on the camera, snarl deepening. Camera holds still.
Pure solid black background throughout.
Sound: a solid impact thud, a brief growl of pain. 1-2 seconds.
```

#### Flow — hit_heavy
```
Close-up bust shot of a shadow wolf facing the camera. A powerful blow sends it
lurching hard backward. Shadow tendrils explode outward from the impact. The amber
eyes go dim for a moment. The wolf slowly pulls back into frame, visibly shaken,
a low dangerous growl rising. Camera shakes on impact.
Pure solid black background throughout.
Sound: a heavy crack of impact, shadow energy dispersing with a hiss, then a low
threatening growl. 2 seconds.
```

---

### 5. Demir Golem
**Tip:** Tank | **Tier:** C

#### ImageFX — Kart Görseli
```
A towering iron golem shown in a bust portrait, looking down at the viewer with
glowing teal rune eyes. The body is made of ancient cracked iron plates held
together by glowing arcane runes carved into the metal. Moss and stone grow in
the crevices between plates. The massive stone fists are partially visible at the
bottom of the frame. Pure solid black background. The creature looks imposing and ancient.
Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, cinematic lighting, highly detailed.
```

#### Flow — idle
```
Medium close-up of an iron golem facing the camera. The teal runes on its body pulse
slowly in a steady rhythm like a heartbeat. Small stone fragments float upward off
its shoulders and drift. The massive chest rises and falls very slightly. The clip
loops naturally. Camera holds still.
Pure solid black background throughout.
Sound: deep low ambient hum from the runes, distant stone settling. 2-3 seconds.
```

#### Flow — attack_light
```
Medium close-up of an iron golem facing the camera. One massive fist rises from below
frame and delivers a slow but heavy forward punch, runes on that arm flashing gold.
The fist returns. The golem's expression does not change. Camera holds still.
Pure solid black background throughout.
Sound: heavy metal whoosh, a deep resonant impact. 2 seconds.
```

#### Flow — attack_heavy
```
Medium close-up of an iron golem facing the camera. All runes across its body begin
pulsing rapidly from teal to bright gold. Both massive fists rise into frame from
below. Stone fragments lift off its surface and float. It slams both fists downward
and forward simultaneously. Camera shakes hard on impact.
Pure solid black background throughout.
Sound: deep stone grinding, rising rune hum, then a ground-shaking metal slam. 2 seconds.
```

#### Flow — hit_normal
```
Medium close-up of an iron golem facing the camera. It absorbs an impact with barely
a reaction — head tilts back slightly, one rune on the chest flickers. It resettles
immediately. Camera holds still.
Pure solid black background throughout.
Sound: a metallic clang that rings out and fades. 1-2 seconds.
```

#### Flow — hit_heavy
```
Medium close-up of an iron golem facing the camera. A powerful hit rocks it backward.
A crack appears across one iron chest plate, the rune inside it sputtering orange
before restabilizing to teal. Stone fragments scatter from the impact. It slowly
straightens, runes pulsing faster than before. Camera shakes hard on impact.
Pure solid black background throughout.
Sound: a massive metallic crash, sparks, a deep groan of bending iron. 2 seconds.
```

---

## Tier B

---

### 6. Ateş Ejderi
**Tip:** Saldırgan | **Tier:** B

#### ImageFX — Kart Görseli
```
A massive fire dragon shown in a bust portrait, facing directly at the viewer.
The scales are crimson and obsidian black with rivers of lava glowing between them.
The eyes burn with molten orange inner fire. Smoke rises from the nostrils.
The wings are partially visible behind the head. Pure solid black background. The creature fills most of the frame and radiates power.
Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, epic cinematic lighting, highly detailed.
```

#### Flow — idle
```
Close-up bust shot of a fire dragon facing the camera. Lava rivers between its scales
glow and pulse slowly. Thin smoke drifts steadily from its nostrils. Its molten
orange eyes burn with a steady inner fire. Embers float upward past the frame.
The clip loops naturally. Camera holds still.
Pure solid black background throughout.
Sound: deep ambient low rumble, crackling embers, slow heavy breathing. 2-3 seconds.
```

#### Flow — attack_light
```
Close-up bust shot of a fire dragon facing the camera. It opens its jaws and releases
a quick sharp burst of fire that flares toward the camera, then closes its mouth.
Scales flare briefly with heat during the burst. It returns to idle immediately.
Camera holds still.
Pure solid black background throughout.
Sound: a sharp ignition crack and a quick roar of flame. 2 seconds.
```

#### Flow — attack_heavy
```
Close-up bust shot of a fire dragon facing the camera. It inhales deeply, lava rivers
on its scales glowing intensely brighter. Its jaw opens wide revealing white-hot fire
building in its throat. Eyes shift from orange to blinding white. It releases a
sustained column of fire toward the camera that fills the frame. Camera pushes in
slowly during the charge, then shakes on release.
Pure solid black background throughout.
Sound: a deep rumbling inhale building into a roaring sustained eruption of flame. 2 seconds.
```

#### Flow — hit_normal
```
Close-up bust shot of a fire dragon facing the camera. An impact lands on its side —
its head turns toward the hit briefly, scales rattling. It turns back to face the
camera, lava glow pulsing sharply once as if in anger. Camera holds still.
Pure solid black background throughout.
Sound: a heavy impact against scales, a low threatening growl. 1-2 seconds.
```

#### Flow — hit_heavy
```
Close-up bust shot of a fire dragon facing the camera. A devastating hit snaps its
head hard to one side. Several scales crack and flare orange along the impact line.
Lava rivers surge brighter. It slowly turns its head back to face the camera, eyes
now blazing intense white. Camera shakes violently on impact.
Pure solid black background throughout.
Sound: a thunderous impact, scale cracking, a deep enraged roar. 2 seconds.
```

---

### 7. Kan Vampiri
**Tip:** Saldırgan | **Tier:** B

#### ImageFX — Kart Görseli
```
A vampire lord shown in a bust portrait, facing directly at the viewer with a cold
smile revealing sharp fangs and glowing crimson eyes. The figure wears a black
armored coat with blood-red lining. Dark wings are partially spread behind.
Crimson blood droplets float slowly in orbit around the figure. The face is pale
and aristocratic with an expression of cruel elegance. Pure solid black background. Do not add any scenery, environment, or background details.
Dark fantasy digital painting style, epic cinematic lighting, highly detailed.
```

#### Flow — idle
```
Close-up bust shot of a vampire lord facing the camera. Crimson blood droplets orbit
slowly around the figure. The crimson eyes glow with a steady cold light. The cape
shifts slightly as if in a faint breeze. The cold smile is fixed and permanent.
The clip loops naturally. Camera holds still.
Pure solid black background throughout.
Sound: distant gothic wind, very faint heartbeat-like pulse. 2-3 seconds.
```

#### Flow — attack_light
```
Close-up bust shot of a vampire lord facing the camera. It reaches one hand forward
toward the camera with unnatural speed, then retracts it just as fast. One blood
droplet from the orbit follows the hand movement and snaps back. The smile widens
slightly for just a moment. Camera holds still.
Pure solid black background throughout.
Sound: a sharp rush of air, a faint hiss. 2 seconds.
```

#### Flow — attack_heavy
```
Close-up bust shot of a vampire lord facing the camera. The orbiting blood droplets
begin spinning faster, glowing brighter. The vampire tilts its head with a slow cold
smile, then suddenly lunges the entire upper body forward, cape flaring wide, eyes
blazing crimson, fangs prominent. Blood droplets surge forward with it.
It snaps back to position. Camera zooms in on the eyes during the buildup, then
shakes on the lunge.
Pure solid black background throughout.
Sound: eerie silence, then a sudden explosive rush of wind and a supernatural hiss. 2 seconds.
```

#### Flow — hit_normal
```
Close-up bust shot of a vampire lord facing the camera. An impact causes it to step
back slightly, the cold smile faltering for just a moment. One blood droplet orbit
is disrupted and breaks away. The smile returns immediately, now with a dangerous
edge. Camera holds still.
Pure solid black background throughout.
Sound: a solid impact, a brief sharp exhale. 1-2 seconds.
```

#### Flow — hit_heavy
```
Close-up bust shot of a vampire lord facing the camera. A powerful blow sends it
reeling backward, wings flaring out involuntarily to catch balance. Several orbiting
blood droplets scatter and dissipate. It straightens slowly, the cold smile gone —
replaced by open fury, eyes blazing intense crimson. Camera shakes on impact.
Pure solid black background throughout.
Sound: a heavy impact, wings snapping open, a low vicious snarl. 2 seconds.
```

---

## Stil Tutarlılığı Notları

- **Arka plan:** Her zaman karanlık, unsura uygun renk vurgusuyla (ateş=turuncu kor, hava=mor/mavi, toprak=yeşil/kahve, su=buz mavisi)
- **Boyut hissi:** B ve C tier büyük ve görkemli; F ve D tier daha küçük ve mütevazı
- **Hit animasyonlarında:** Canavar çerçeveden tamamen çıkmamalı — hasar hissedilmeli ama canavar görünür kalmalı
- **Clip sonu:** Her clip sonunda canavar idle pozisyonuna dönmeli — Flow'da "returns to idle position" ifadesini koru

## Tier Renk Paleti (UI bağlamı için)

| Tier | Ana Renk | İkincil Renk |
|------|----------|-------------|
| F | #808080 (Gri) | #A0A0A0 |
| D | #FFFFFF (Beyaz) | #E0E0E0 |
| C | #4CAF50 (Yeşil) | #81C784 |
| B | #2196F3 (Mavi) | #64B5F6 |

## Üretim Sırası (Önerilen)

1. **Ateş Ejderi (B)** → oyunun "yüzü", ilk oluştur, tüm 5 clip'i yap, stil rehberi olarak kullan
2. **Gölge Sıçanı (F)** → F tier görsel ve animasyon dilini kur
3. **Kil Golem (F)** → F tier ikinci referans
4. **Gölge Kurdu (C)** → C tier orta güç hissi
5. **Demir Golem (C)** → tank animasyon dili
6. **Kan Vampiri (B)** → B tier ikinci boss
7. **Buz Büyücüsü (D)** → D tier son
