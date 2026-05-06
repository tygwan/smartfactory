# M1 — Piping vertical slice (application 1.0)

| | |
|---|---|
| **Status** | Closed |
| **Opened** | 2026-05-06 |
| **Closed** | 2026-05-07 |
| **Owner** | coffin |

## Goal

Ship a self-demonstrating Unity HDRP scene where a user authors piping in 3D, the system validates it in real time against geometric rules, and a 3D viewer renders both the piping model and a placeholder visual signal for future operations data. The milestone closes when the demo is recordable as a 1–2 minute video, the Figma portfolio page 1.0 embeds it, and the GitHub repository contains the code, retrospective records, and a runnable Unity build (or at minimum a reproducible scene). This is the application-deadline deliverable; M2 and M3 extend it toward the full A+C vision per [D-001](../../decisions/D-001-vision-and-phase-decomposition.md).

## Scope

Mark each item as it is verified by a V-record.

**A. Piping authoring**
- [x] A1. Two-click line pipe creation (start/end points → straight segment with diameter, material props)
- [x] A2. Auto-elbow insertion at intersection of two pipes (sphere fallback per P-001 risk § cut-order)
- [x] A3. Click pipe → editable Inspector panel (diameter, material) — **plus** Edit-mode Scene authoring window (PipeSceneAuthoringWindow), unplanned addition that aligns directly with JD §1
- [ ] A4 (stretch). T-junction / 3-way branch — **dropped** per cut order; carried to M2 (see retrospective § Next-milestone follow-ups)

**A. Real-time validation**
- [x] V1. Clash detection — segment-segment distance against (R₁ + R₂ + clearance), red material swap, alert panel
- [~] V2 (stretch). Sharp-bend detection — pure function `ValidationRules.DetectSharpBends` shipped; UI surface deferred to M2
- [ ] V3 (stretch). Slope direction rule — **dropped** per cut order; carried to M2

**A. 3D viewer**
- [x] B1. Orbit + fly camera — `AuthorCameraController` written fresh against the new Input System (per D-003)
- [~] B2. Layer toggles — `LayerToggleController` + `PipeView.ShowClashHighlights` shipped; Toggle GameObject UI scene wiring deferred to user action at M1 close (V-001 Follow-up #1)
- [x] B3. Click pipe → metadata panel (diameter, material, length, virtual pressure)

**C. Visual hint of future extension**
- [x] C1. Pipe color = analytic pressure-drop estimate (`ΔP ∝ L / D⁵`) via `MaterialPropertyBlock` + metallic surface
- [x] C2. UI label `FutureNoteText` explicitly stating M3 surrogate replacement plan

**Drawing export seed**
- [x] One-way 3D → 2D wireframe export — `DrawingExportTool` with two MenuItems (full + pipes-only), top-down ortho 1024×1024 PNG. SVG and round-trip remain M2.

**Portfolio artifacts**
- [~] Figma portfolio page 1.0 — outline written at `docs/portfolio/M1-portfolio-outline.md`; Figma file build is a user follow-up (V-001 Follow-up #3)
- [~] Demo video 1–2 minutes — recording script written at `docs/portfolio/M1-demo-video-script.md`; OBS capture is a user follow-up (V-001 Follow-up #2)
- [x] Retrospective written at `retrospective.md`
- [x] V-001 written at `../../verifications/V-001-m1-vertical-slice.md`

Legend: `[x]` shipped; `[~]` partially shipped (code in place, surface/wiring/recording follow-up); `[ ]` dropped per cut order, carried to M2.

## Out of scope

The following are explicitly deferred per [D-001](../../decisions/D-001-vision-and-phase-decomposition.md):

- ONNX mini-surrogate training and Unity inference (→ M2 / C4)
- Flow-direction arrows, particle / flow-line visualization (→ M2 / C3)
- Drawing 2D → 3D round-trip regeneration (→ M2)
- Semiconductor equipment mock-up prefabs (→ M3)
- Auto-routing / path-finding from start point to equipment (→ M3)
- Real CFD simulator integration (never — substituted by surrogate per D-001)
- Production build distribution (.exe packaging, signing) — out of project scope per AGENTS.md
- Multi-scene additive loading (single-scene M1; revisit if M3 needs it)
- Replacing external asset code under `Assets/UnityFactorySceneHDRP/` beyond the minimal CameraMove port — see D-003

## Inherited from M0

M0 was an implicit setup phase (greenfield project bootstrap, aegis discipline install, AGENTS/CLAUDE fill). It did not produce a formal retrospective; lessons are absorbed directly:

- Working directory and HDRP template are stable; FactorySceneSample.unity is a known-good entry point — use as backdrop for authored piping
- Active Input Handling needs to be set to "Both" or new Input System exclusively before M1 begins (touched on by D-003)
- aegis discipline is installed and dual-source AGENTS/CLAUDE convention is enforced

## Open questions

| # | Question | Resolved by |
|---|---|---|
| 1 | Piping data model — ScriptableObject per pipe vs plain C# class + JSON serialization | [D-002](../../decisions/D-002-piping-data-model.md) |
| 2 | Input system strategy — port CameraMove.cs to new Input System, or keep "Both" mode and write our own author-mode controls separately | [D-003](../../decisions/D-003-input-system-strategy.md) |
| 3 | Drawing export format for the seed — raster PNG vs vector SVG | Resolved inside P-001 — raster PNG only for M1; SVG carried to M2 |
| 4 | Validation execution model — per-frame Update polling vs event-driven on edit | Resolved inside P-001 — event-driven via PipeNetworkAsset.OnChanged |
| 5 | MCP for Unity transport — stdio vs HTTP local | [D-004](../../decisions/D-004-mcp-for-unity-adoption.md) chose stdio; superseded mid-milestone by [D-005](../../decisions/D-005-mcp-transport-http-local.md) (HTTP local) |

## Reserved record numbers

| Type | Range | Notes |
|---|---|---|
| D-records | D-002 ... D-005 | Data model, input system, drawing export, validation execution model (if needed) |
| P-records | P-001 ... P-002 | P-001 = M1 implementation plan; P-002 = portfolio artifact production (Figma + video) |
| V-records | V-001 ... V-005 | One per scope group: A authoring, A validation, B viewer, C visual hint, drawing export + portfolio |

## Plan summary

- P-001: M1 implementation plan — sequenced task list across A / V / B / C / drawing-export. Written immediately after D-002 lands.
- P-002: Portfolio artifact production (Figma + demo video). Written on D2 once the demo is recordable.

## References

- Previous milestone: M0 (implicit setup; no retrospective produced — see *Inherited from M0* above)
- Constraining D-records: [D-001](../../decisions/D-001-vision-and-phase-decomposition.md)
- Project memory: `~/.claude/projects/-mnt-c-Users-x8333-Desktop-AI-PJT-unity-SmartFactory/memory/project_vision.md`
- Job description (image): `/mnt/c/Users/x8333/Desktop/AI_PJT/이안BIM.png`
- Company website: <https://www.iaan.co.kr/kr/>

---

## Closing checklist (run at milestone close)

- [ ] All scope items verified (V-records exist for each group)
- [ ] All open questions resolved by D-records (D-002, D-003, D-004 minimum)
- [ ] Retrospective written at `retrospective.md`
- [ ] Each V-record's follow-ups: either listed in retrospective `§ Next-milestone follow-ups` or explicitly dropped
- [ ] `AGENTS.md` "Out of scope" updated for M2 context
- [ ] Status above changed to **Closed**

## Abandonment checklist (when closing as Abandoned)

- [ ] Retrospective written, focused on *why this milestone was wrong* and *what M1' should look like instead*
- [ ] D-record explaining abandonment and superseding D-001's M1 scope
- [ ] Status above changed to **Abandoned**
- [ ] Any V-records produced before abandonment kept
