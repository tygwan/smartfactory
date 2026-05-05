# P-001: M1 implementation plan

| | |
|---|---|
| **Status** | Open |
| **Owner** | coffin |
| **Started** | 2026-05-06 |
| **Completed** | — |
| **Milestone** | [M1-piping-vertical-slice](../milestones/M1-piping-vertical-slice/README.md) |
| **Related D-records** | [D-001](../decisions/D-001-vision-and-phase-decomposition.md), [D-002](../decisions/D-002-piping-data-model.md), [D-003](../decisions/D-003-input-system-strategy.md) |
| **Verification** | V-001 (reserved) |

## Goal

Reach a self-demonstrating M1 deliverable inside 3 calendar days (~50 high-intensity working hours): the M1 working scene at `Assets/Project/Scenes/M1_PipingAuthor.unity` lets the operator author piping with two-click placement, validates it in real time, renders it through a self-authored 3D viewer with an analytic pressure gradient, exports a top-down PNG drawing, and produces a 1–2 minute demo video. By close, the GitHub repository contains all code, the Figma portfolio page 1.0 embeds the video, and `retrospective.md` + `V-001` are written. Application 1.0 ships.

## Non-goals

- Drawing 2D → 3D round-trip regeneration — deferred to M2
- ONNX mini-surrogate training and Unity inference — M2
- Flow-direction arrows / particle visualization — M2
- Equipment mock-up prefabs, auto-routing, CFD-comparison — M3
- Real CFD solver integration — never (substituted by surrogate per D-001)
- Production build (.exe) packaging — out of project scope per AGENTS.md
- Beyond-M1 Figma pages — M2 / M3 add their own pages

## Steps

### Phase D1 (~16–18 h) — Foundation + first authoring

1. **Pipe data model** — `Assets/Project/Scripts/Data/PipeData.cs` (`[Serializable]` struct: id, start, end, diameter, material) + `PipeNetworkAsset.cs` (ScriptableObject with `List<PipeData>` + `OnChanged` event). Output: two `.cs` files + one starter `.asset` instance under `Assets/Project/Data/`.
2. **Author camera + input map** — `Assets/Project/Scripts/Viewer/AuthorCameraController.cs` (orbit + fly + raycast click) backed by `Assets/Project/Input/AuthorActions.inputactions`. Output: camera moves in the M1 scene; clicks produce world-space rays.
3. **A1 two-click pipe placement** — first click stores start, second click commits a new `PipeData` to the network asset. Subscribes to `OnChanged` to instantiate `PipeView` GameObjects. Output: pipe appears in scene immediately on second click.
4. **`PipeView` mesh component** — `Assets/Project/Scripts/Viewer/PipeView.cs` builds a cylinder mesh between start/end with the chosen diameter. Output: visible cylinder, geometry updates if `PipeData` is mutated.

### Phase D2 (~18–20 h) — Validation + viewer + visual hint

5. **A2 auto-elbow** — at intersection of two pipes, insert a short connector `PipeData` variant (or simply a sphere prefab matching diameter). Output: two pipes meeting look continuous.
6. **A3 selection + edit panel** — click a `PipeView` → side panel shows `PipeData` fields (diameter, material) and writes back via `PipeNetworkAsset`. Output: in-scene pipe edits round-trip through the data layer.
7. **V1 clash detection** — pure function over `PipeNetworkAsset` returning `List<(int a, int b)>` clashing pipe pairs; subscribed to `OnChanged`; offending `PipeView`s swap to a clash material; right-side alert panel lists pairs. Output: real-time red highlight + alert.
8. **B2 layer toggles** — UI buttons toggle visibility of: external factory backdrop, authored piping, clash highlights. Output: any combination of the three layers visible.
9. **B3 metadata panel** — clicking a pipe surfaces diameter / material / length / virtual pressure (analytic). Output: side panel updates per click.
10. **C1 pressure gradient material** — Shader Graph with a `_PressureT` float (0 → 1) mapped to a blue→red ramp; per-pipe `MaterialPropertyBlock` set from analytic `ΔP ∝ L / D⁵` normalized across the network. Output: pipes colored by virtual pressure.
11. **C2 future-extension label** — small in-scene UI overlay: *"this gradient is replaced by an ONNX-exported CFD surrogate in M3"*. Output: label visible in the demo recording.

### Phase D3 (~14–16 h) — Stretch + portfolio

12. **A4 stretch — T-junction or spline branch** — extend `PipeData` with a junction variant; click on existing pipe + new endpoint creates a branch. Output: branching networks possible.
13. **V2/V3 stretch — bend radius + slope rules** — additional pure-function validators feeding the same alert panel. Output: more rule types covered.
14. **Drawing seed — top-down PNG export** — Editor menu item creates an orthographic top-down `RenderTexture` and writes `Assets/Project/Exports/M1_top.png`. Output: one PNG per export.
15. **Demo video (1–2 min)** — Unity Recorder or OBS; voiceover or text overlays mapping each segment to a JD responsibility. Output: `M1_demo.mp4`.
16. **Figma page 1.0** — hero shot + JD-responsibility-mapping table + architecture diagram + embedded video. Output: Figma URL.
17. **V-001 + retrospective** — `docs/verifications/V-001-m1-vertical-slice.md` checklists all M1 scope items; `docs/milestones/M1-piping-vertical-slice/retrospective.md` captures friction, decisions, and M2 follow-ups. Output: two `.md` files.
18. **M1 close** — flip M1 README status to Closed; update AGENTS.md "Out of scope" for M2 context; final commit + push.

## Risks and mitigations

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| HDRP Shader Graph learning curve eats D2 time | med | med | If `MaterialPropertyBlock` + Shader Graph stalls > 1h, fall back to per-pipe material instances with `Color.Lerp` — visually equivalent for the demo |
| Auto-elbow geometry math at A2 looks wrong | med | low | Substitute with a sphere of matching diameter at the intersection — reads as elbow at viewer distance |
| Clash detection becomes O(N²) per frame | low | med | Subscribe to `OnChanged` only (event-driven per D-002); never run from `Update()` |
| External asset auto-mutates again on Unity restart | high | low | Already observed twice; `git checkout --` before each commit; if persistent, add a one-line D-record acknowledging Unity's HDRP material auto-migration |
| ~50 h estimate underruns reality | med | high | Cut order: A4 → V3 → V2 → C2 → drawing-export precision (raster only, no SVG). Demo video and Figma never cut |
| Figma page craft eats more than 0.5 day | med | med | Time-box: one page, 4 sections (hero, responsibilities, architecture, video). No design-system polish in M1 |

## Done criterion

- [ ] All M1 README scope items checked, dropped explicitly, or moved to M2 with reasoning
- [ ] M1 working scene runs in Editor Play mode without exceptions; manual flow A1 → V1 → B3 → C1 demonstrable
- [ ] One `M1_top.png` drawing export produced from the operator-authored network
- [ ] `M1_demo.mp4` (≤ 2 min) recorded
- [ ] Figma page 1.0 published; URL captured in `retrospective.md`
- [ ] `V-001-m1-vertical-slice.md` written, all checks signed off or annotated as dropped
- [ ] `docs/milestones/M1-piping-vertical-slice/retrospective.md` written with M2 follow-ups
- [ ] M1 README Status flipped to Closed
- [ ] All work pushed to `origin/main`

## Verification plan

V-001 is a manual + scripted verification. Manual: open `M1_PipingAuthor.unity`, perform the demo flow once on camera (this becomes the demo video itself, killing two birds), and screenshot each scope item as evidence. Scripted: run a minimal `EditMode` test (`Assets/Project/Tests/`) that constructs a `PipeNetworkAsset` programmatically and asserts (a) `PipeData` round-trips through `JsonUtility`, (b) `ValidationRules.DetectClashes` flags an intentionally-overlapping pair. The test code itself is portfolio evidence of TDD signal per JD's preference.

## Revision log

- 2026-05-06: P-001 opened. Initial plan based on D-001 / D-002 / D-003 and M1 README scope.
