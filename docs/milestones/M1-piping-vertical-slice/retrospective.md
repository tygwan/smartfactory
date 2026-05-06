# M1 — Retrospective

| | |
|---|---|
| **Milestone** | M1 — Piping vertical slice (application 1.0) |
| **Period** | 2026-05-06 ~ 2026-05-07 |
| **Author** | coffin |
| **V-records covered** | [V-001](../../verifications/V-001-m1-vertical-slice.md) |
| **D-records produced** | [D-001](../../decisions/D-001-vision-and-phase-decomposition.md), [D-002](../../decisions/D-002-piping-data-model.md), [D-003](../../decisions/D-003-input-system-strategy.md), [D-004](../../decisions/D-004-mcp-for-unity-adoption.md) |

## Inputs

- [x] V-001 reviewed — § Deviations, § Findings, § Follow-ups
- [x] P-001 reviewed — cut-order applied (A4, V3 deferred); no plan abandonment
- [x] All M1 D-records (D-001 → D-004) referenced
- [x] Memory entries (`project_vision.md`, `user_context.md`) consulted; no contradiction with executed work

## What worked

### aegis discipline framed every decision

The choice to install aegis at M0 and immediately produce D-001 (the vision + 3-phase decomposition) before writing any C# turned out to be the best single decision of the milestone. When the JD analysis of 이안BIM's role revealed the gap between the user's original vision (CFD-driven operations twin) and the company's actual product line (design + validation + viewer tools), D-001 absorbed that pivot cleanly: the original ambition was preserved as M2/M3 scope while M1 narrowed to vertical-slice JD-aligned work. Without that framing the project would have spent D1 building the wrong system.

### MCP for Unity collapsed the round-trip cost

Once HTTP local transport was in place (after the WSL2 stdio dead end), Canvas/UI scaffolding, component wiring, and material property edits became single-call automations instead of multi-minute manual round-trips. Concrete impact: A3 selection + B3 metadata panel were wired end-to-end in one batch_execute call rather than the half-dozen "open Inspector → drag asset → save scene" cycles that would have eaten D2.

### Editor-mode Scene authoring window emerged unscripted

`PipeSceneAuthoringWindow` was not in the original P-001 plan — it surfaced when the user asked "scene 상태에서 pipe 어떻게 배치할 수 있어?" mid-D2. Implementing it cost ~30 minutes and produced the strongest direct match to JD §1 (배관 설계 핵심 기능). The lesson: the cleanest portfolio signals tend to emerge from honest user feedback during demoable iterations, not from upfront planning.

## What to improve

### Test scaffolding skipped

P-001's done criterion called for an EditMode test (JsonUtility round-trip + DetectClashes). Setting up `Assets/Project/Tests/` with `.asmdef` was deferred under time pressure and never made it back. Validation rules are pure static functions — the test would have taken under an hour and would have made TDD/DDD signal in the JD's preferred-experience clause concrete. Next time: scaffold tests on D1 alongside the data layer, before any feature code.

### Time-budgeted "stretch" items remained on the roadmap

A4 T-junction, V3 slope rule, and V2 UI surfacing all stayed in P-001 as ostensible D2/D3 work even after it was clear they wouldn't ship. Honest plan revision (strikethrough in P-001 §Steps + Revision log entry) was skipped. Next milestone: cut visibly when the cut happens, not after the fact in the retrospective.

### External asset re-touch noise

Three external HDRP material/profile files were re-touched by Unity at every session start. Each session ate a `git checkout --` reset before staging. A one-line D-record acknowledging Unity's auto-migration (or pinning the `Assets/UnityFactorySceneHDRP/` folder via `git update-index --skip-worktree`) would have removed the friction earlier. The repeated reset is a smell that an undocumented decision is hiding.

## Next-milestone follow-ups

### Drawing 2D ⇄ 3D round-trip

The M1 drawing seed (top-down PNG export) is one-way. M2's headline feature is the round-trip: SVG vector export from the existing PipeNetworkAsset, then SVG re-import that reconstructs `PipeData` entries. This is the deliverable that maps to **이안BIM's D·Master / C·Master** product surface and is the largest single portfolio uplift for M2.

### ONNX mini-surrogate (C4)

Train a tiny MLP in Python (PyTorch) on a handful of analytic pressure-drop samples (Hagen-Poiseuille for now, then a small precomputed CFD set if available) and export to ONNX. Replace the analytic `L/D⁵` placeholder in `PipeView.AnalyticPressureDrop` with `ONNX inference`. M3 swaps the surrogate's training data for real CFD output and adds the comparison demo.

### B2 UI scene wiring (carried from V-001 §Follow-ups)

`LayerToggleController` and `PipeView.ShowClashHighlights` ship from M1, but the actual Toggle GameObjects + field assignment did not happen in the scene before close. This is mechanical and small (≤ 15 minutes once the user is in front of the Editor); appropriate to fold into M1's tail rather than M2's head.

### V2 sharp-bend UI surface

`ValidationRules.DetectSharpBends` is a pure function with no consumer. Either extend AlertCanvas to enumerate sharp bends alongside clashes (one new TMP_Text + an `OnSharpBendsUpdated` event on PipeView) or roll it into a generic `ValidationAlertPanel`. Grouping with the V3 slope rule below makes a coherent M2 mini-feature.

### V3 slope rule + A4 T-junction

Both deferred per cut order. M2 is a natural home: V3 ties into the drawing roundtrip (drainage direction must survive 3D ⇄ 2D), and A4's branching topology is necessary before M3's auto-routing can produce realistic networks.

### D-005 record: formalize MCP HTTP local

D-004 chose stdio. Field test forced HTTP local during M1 (commit `7f1b47b`). The transport switch was logged operationally but no formal D-record exists. M1 close or M2 open should produce **D-005: MCP transport — HTTP local (supersedes D-004's stdio choice)** with the WSL2 mirrored-networking prerequisite documented. Without it, future readers will not understand why `.mcp.json` looks the way it does.

### Explicitly dropped (not carried forward)

- **CFD solver implementation** — never planned in M1, never planned in M2/M3. Substituted by surrogate at every phase per D-001. Out of scope permanently; will not return.
- **Demo VR/AR build** — explicitly out of scope per AGENTS.md "Out of scope (permanent)" section.
- **Multi-user collaboration on the same scene** — explicitly out of scope; portfolio-only project.

## Standards refinements (optional)

- AGENTS.md "Out of scope" should be updated for M2 context as part of M1 close (move M2 items out of "Deferred to M2" into actual milestone scope; introduce M3 deferrals into the "Deferred to M3" group).
- Consider a project-local convention at `docs/standards/local/01-mcp-tooling.md` documenting the WSL2 mirrored networking prerequisite. Optional; D-005 may suffice.

## References

- Milestone README: [M1-piping-vertical-slice](README.md)
- V-001: [m1-vertical-slice](../../verifications/V-001-m1-vertical-slice.md)
- Standards touched: none
- Portfolio outline: [M1-portfolio-outline](../../portfolio/M1-portfolio-outline.md)
