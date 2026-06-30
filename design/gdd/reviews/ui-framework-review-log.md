# Review Log: UI Framework

## Review — 2026-06-26 (3rd pass) — Verdict: APPROVED
Scope signal: L
Specialists: ux-designer, ui-programmer, unity-ui-specialist, systems-designer, qa-lead, performance-analyst, game-designer, creative-director
Blocking items: 8 (all resolved in-session) | Recommended: 8 (most resolved in-session)
Summary: Creative-director 8 gerçek blocker tespit etti: ShowModal() API kontratı (data context + bool dönüş + priority), LockTabs reference counting ve otofarm'ın kilit dışı bırakılması (Pillar 3 uyumu), Kural 1 ve AC-NAV-02 çelişkisi, HUD throttle leading/trailing semantik belirsizliği, GOLD_DISPLAY Band 6 float precision bug (V≈999M → "1000M"), GetTopScreen() yanlış dönüş tipi (VisualTreeAsset → VisualElement), safe area "ADR'e bırakıldı" eksikliği (USS padding kararı alındı), ShowFullscreenReveal() API stub yokluğu (Otofarm artık toast kullanmıyor). Tüm 8 blocker in-session revize edildi. 2 ADR (animation driver + UIDocument topology) hâlâ implementasyon öncesi yazılmalı.
Prior verdict resolved: Yes — 3rd pass, all blockers resolved

## Review — 2026-06-25 (2nd pass) — Verdict: NEEDS REVISION (revised in-session, re-review pending)
Scope signal: L
Specialists: ux-designer, ui-programmer, unity-ui-specialist, systems-designer, qa-lead, performance-analyst, game-designer, creative-director
Blocking items: 6 (all resolved in-session) | Recommended: 10+ (most resolved in-session)
Summary: Creative-director 46 ham bulgunu 6 gerçek blocker'a daralttı: LockTabs() deadlock (UnlockTabs() yoktu), dim overlay pickingMode belirsizliği, int overflow (idle RPG için ölümcül — long'a çevrildi), modal soft-lock (ForceCloseActiveModal eklendi), animasyon driver belirsizliği (PrimeTween seçildi), 4-UIDocument batching ADR eksikliği. Tüm 6 blocker in-session revize edildi; 2 ADR blocking olarak işaretlendi (animation driver + UIDocument topology). Re-review yeni oturumda /design-review ile yapılmalı.
Prior verdict resolved: Yes — prior 13 blockers resolved; yeni 6 blocker bu oturumda çözüldü

## Review — 2026-06-25 — Verdict: NEEDS REVISION
Scope signal: L
Specialists: ux-designer, ui-programmer, unity-ui-specialist, systems-designer, qa-lead, performance-analyst, creative-director
Blocking items: 13 (resolved in-session) | Recommended: 20+
Summary: GDD'nin mimari tasarımı (hibrit sekme + stack + modal + toast) oyun türü için doğru ve sağlamdır. Spec'te beş blocker grubu tespit edildi: (1) altın formatı formülü T_K=10.000 ile AC-FMT-03 çelişkisi + C# integer division tuzağı, (2) UIDocument/PanelSettings topolojisi ve sekme stack state davranışı tanımsız, (3) modal kuyruk kontratı drop-oldest vs reject-new çelişkisi + callback tipi belirsizliği, (4) animasyon katmanı Unity 6.3 API'siyle uyumsuz (deprecated transform, USS cubic-bezier overshoot, OS reduced motion), (5) performans AC'leri toplam bütçeyle çakışıyor ve fiziksel olarak imkânsız precondition içeriyor. Tüm 13 blocker in-session revize edildi. Creative director miss: idle oyun türünün "devamsızlıktan-dönüş ödül reveal" akışı framework'te tanımlanmamış — Open Questions'a eklendi.
Prior verdict resolved: No — first review
