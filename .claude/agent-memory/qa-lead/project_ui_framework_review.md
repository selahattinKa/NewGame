---
name: project-ui-framework-review
description: Adversarial AC audit for ui-framework.md GDD — confirmed structural contradictions and untestable ACs as of 2026-06-25
metadata:
  type: project
---

Adversarial QA review of `design/gdd/ui-framework.md` acceptance criteria completed 2026-06-25.

**Why:** Shift-left QA gate — ACs audited before implementation begins to prevent untestable stories reaching dev sprint.

**How to apply:** Before any UI Framework story is moved to In Progress, confirm the affected AC has been corrected per the findings below. Do not approve sprint planning for Logic stories with known AC contradictions.

## Confirmed BLOCKING defects

1. **T_K contradiction**: Formula declares T_K=10,000; AC-FMT-03 tests V=1,000 → "1.0K" (should be "1000"). One must change.
2. **Modal queue policy contradiction**: Edge case = drop-oldest; AC-MOD-05 = reject-new. Mutually exclusive.
3. **AC-FMT-04 comment arithmetic**: Says 1.25 but floor(12500/100)/10 = 12.5. Comment wrong, result correct.
4. **AC-FMT-05 wrong branch formula**: Comment cites floor(V/1000) but correct branch is floor(V/100)/10. Result happens to be identical (50) so runtime behavior is fine; comment misleads implementer.
5. **AC-TOAST-02/03 dual-visibility undefined**: Between t=3.0s–3.4s, A is fading and B is visible simultaneously. No layout/positioning rule defined.
6. **AC-ANIM-06 GIVEN not automatable**: No Unity 6.3 built-in API for OS reduced motion. Requires injectable mock or platform plugin.
7. **AC-PERF-01 precondition physically contradictory**: 3 tab switches × 300ms = 900ms minimum; AC demands this in 500ms without acknowledging snap-interruption dependency.
