# D-001: Vision and 3-phase decomposition

| | |
|---|---|
| **Status** | Accepted |
| **Date** | 2026-05-06 |
| **Deciders** | coffin (project owner); Claude Code (advisory) |
| **Supersedes** | — |
| **Superseded by** | — |

## Context

SmartFactory is a portfolio piece for one specific job application: **이안BIM (iaan.co.kr)**, position **"FAB 3D 배관설계 솔루션 개발자"**. Application deadline is **3 days** from this record's date; interview window is open beyond that.

The project owner initially articulated a broad vision — Unity HDRP dashboard fed by CFD analysis, meta-model surrogate predictions, virtual-and-physical sensor redundancy, and integrated monitoring of E2E flow along piping networks (an *operations* digital twin).

Reading the actual job description revealed a gap. The role's core responsibilities are: (1) piping design core feature development, (2) real-time validation, (3) 3D viewer development. The company's product lineup (DTDesigner Suite, Master Series, Auto Bending) is centered on **piping design / drawing management / change control / automated bending** — i.e. a *design-and-validation* digital twin, not a *simulation-and-monitoring* one. The original vision corresponds more to P·Master / O·Master than to the role being hired.

Without a deliberate decision now, the project would either drift toward the misaligned original vision (impressive but off-target) or shrink to a narrow JD-only build (aligned but unambitious). Both fail the portfolio's purpose.

## Options considered

### Option A — JD-only pivot (design + validation + viewer)

Build a Unity-based piping design tool: pipe authoring (draw/edit), real-time validation (clash detection, geometric rule checks), and a 3D viewer. Drop CFD/sensor/meta-model entirely.

**Pros**
- 1:1 mapping to JD's three responsibilities
- Achievable as a focused vertical slice within 3 days
- Demoable end-to-end in interview ("I draw, system validates, I view")

**Cons**
- Doesn't differentiate from other applicants who read the same JD
- Discards the project owner's stated ambition and domain interest
- No signal of operations / simulation depth (which the company also sells via O·Master / P·Master)

### Option B — Hybrid (A + closing nod to original vision)

Build A as the demonstrable core, then add a slide / diagram / static UI hint indicating where CFD-surrogate-sensor extension would live. No actual operations code.

**Pros**
- Slightly broader narrative than A
- Cheap insurance against "you only did design"

**Cons**
- The "nod" is decorative — it does not run, so it does not demonstrate capability
- Reviewers can see through static aspiration

### Option C — Original vision only (CFD + meta-model + sensors + dashboard)

Build the operations digital twin as articulated.

**Pros**
- Most ambitious; closest to the project owner's intuition

**Cons**
- Mismatched with JD core responsibilities — the demo doesn't show piping design, validation, or a viewer
- CFD integration, ML surrogate training, and sensor stream simulation are each multi-week tracks; combined, they exceed the available time even with high-intensity work
- High risk of producing 4 half-built layers and no demo

### Option A+C — Phased integration (recommended and chosen)

Build A as a *demonstrable core* in M1 (3 days), then extend toward C across M2 and M3 in the post-application / pre-interview window. The integration is natural: a piping model authored in A becomes the geometric input to a CFD surrogate in C; the 3D viewer extends to render virtual + physical sensor overlays. This produces a single end-to-end story spanning the company's full product lineup.

**Pros**
- M1 alone is JD-aligned and demoable; subsequent milestones add depth
- Single data spine (the user-authored piping model) connects all phases — narratively coherent
- Maps to **DTDbuilder + D·Master + Auto Bending + P·Master / O·Master** across phases — covers the company's product surface
- Failed extensions (M2 or M3) do not invalidate M1; the portfolio is shippable at the end of any phase

**Cons**
- Higher discipline cost: each milestone needs its own scope guard and retrospective
- Tempting to over-invest in M2/M3 architecture before M1 demo is locked

## Decision

**Option A+C, with explicit 3-phase decomposition.**

| Phase | Window | Deliverable theme |
|---|---|---|
| **M1** | D-day (3 days) — application 1.0 | Piping design + validation + viewer (JD-aligned vertical slice) + drawing-export *seed* + Figma portfolio page 1.0 + demo video + core retrospective records |
| **M2** | +1 week — pre-interview boost 1 | Drawing-roundtrip (3D ⇄ 2D), full C-layer hints (flow arrows, ONNX mini-surrogate), Figma page 2.0 |
| **M3** | +2~3 weeks — full (c) ambition | Semiconductor-equipment mock-up + auto-routing (path-finding) + CFD comparison demo + Figma page 3.0 + interview slide deck |

CFD will not be implemented from scratch at any phase. M3's "CFD" is a surrogate model trained on either an analytic approximation or a small precomputed dataset, exported via ONNX and inferred inside Unity. This is documented as honest in the demo script ("real CFD pipeline structure shown; surrogate substitutes the expensive solver").

## Consequences

**Unlocks:**
- M1 README can now be written with a concrete scope (see `docs/milestones/M1-piping-vertical-slice/README.md`)
- Implementation can begin without further scope debate
- The portfolio has a credible end-to-end story before any code is written
- Each phase boundary becomes a natural retrospective + demo recording point

**Locks in:**
- Unity + C# as the core stack for M1; Python only enters at M2 (ONNX surrogate)
- The user-authored piping model as the single source of truth that flows through all phases — its data shape (decided in D-002, anticipated) constrains every subsequent phase
- Out-of-scope items per phase boundary; reopening requires a new D-record

**Cost:**
- M1 has zero slack. Any unforeseen friction (Unity build issue, HDRP material problem, input system port) eats stretch goals first, then demoable scope
- Discipline overhead: each phase needs its retrospective and at least one D-record before close

**Open questions** (resolved by future D-records):
- D-002: piping data model (ScriptableObject vs plain C# + JSON serialization)
- D-003: input system strategy for in-editor authoring (new Input System vs legacy via Active Input "Both")
- D-004: drawing export format (raster PNG vs vector SVG)
- D-005: ONNX surrogate training data source (analytic generator vs public dataset) — M2

## References

- Related D-records: — (this is the first substantive D-record after aegis install)
- Related P-records: P-001 (M1 implementation plan, anticipated)
- External sources:
  - Job description: 이안BIM "FAB 3D 배관설계 솔루션 개발자" (image: `/mnt/c/Users/x8333/Desktop/AI_PJT/이안BIM.png`)
  - Company products: <https://www.iaan.co.kr/kr/>
  - aegis 02-decisions standard: <https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md>
- Project memory: `project_vision.md`, `user_context.md`
