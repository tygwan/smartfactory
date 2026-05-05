# M1 — Piping vertical slice (application 1.0)

| | |
|---|---|
| **Status** | Open |
| **Opened** | 2026-05-06 |
| **Closed** | — |
| **Owner** | coffin |

## Goal

Ship a self-demonstrating Unity HDRP scene where a user authors piping in 3D, the system validates it in real time against geometric rules, and a 3D viewer renders both the piping model and a placeholder visual signal for future operations data. The milestone closes when the demo is recordable as a 1–2 minute video, the Figma portfolio page 1.0 embeds it, and the GitHub repository contains the code, retrospective records, and a runnable Unity build (or at minimum a reproducible scene). This is the application-deadline deliverable; M2 and M3 extend it toward the full A+C vision per [D-001](../../decisions/D-001-vision-and-phase-decomposition.md).

## Scope

Mark each item as it is verified by a V-record.

**A. Piping authoring**
- [ ] A1. Two-click line pipe creation (start/end points → straight segment with diameter, material props)
- [ ] A2. Auto-elbow insertion at intersection of two pipes
- [ ] A3. Click pipe → editable Inspector panel (diameter, material)
- [ ] A4 (stretch). T-junction / 3-way branch, or spline-based curved routing

**A. Real-time validation**
- [ ] V1. Clash detection — distance between two pipes < `(R₁ + R₂ + clearance)` highlights both red, alert in side panel
- [ ] V2 (stretch). Minimum bend radius / elbow angle violation
- [ ] V3 (stretch). Slope direction rule (drainage flow guarantee)

**A. 3D viewer**
- [ ] B1. Orbit + fly camera (port `CameraMove.cs` to new Input System, or fresh implementation)
- [ ] B2. Layer toggles — background fab / authored piping / clash highlights
- [ ] B3. Click pipe → metadata panel (diameter, material, length, virtual pressure)

**C. Visual hint of future extension**
- [ ] C1. Pipe color = analytic pressure-drop estimate (`ΔP ∝ L / D⁵`) rendered as material gradient
- [ ] C2. UI label / tooltip explicitly stating: "this gradient is replaced by an ONNX-exported CFD surrogate in M3"

**Drawing export seed**
- [ ] One-way 3D → 2D wireframe export (PNG raster, top-down + side projection minimum); SVG vector is stretch — full round-trip is M2

**Portfolio artifacts**
- [ ] Figma portfolio page 1.0 (one page: hero shot + responsibility-mapping table + architecture diagram + embedded video)
- [ ] Demo video 1–2 minutes (screencap + voiceover or text overlays)
- [ ] Retrospective at `retrospective.md` covering decisions, friction, and what M2 should inherit

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
| 1 | Piping data model — ScriptableObject per pipe vs plain C# class + JSON serialization | D-002 (before A1 implementation) |
| 2 | Input system strategy — port CameraMove.cs to new Input System, or keep "Both" mode and write our own author-mode controls separately | D-003 (before B1 implementation) |
| 3 | Drawing export format for the seed — raster PNG vs vector SVG | D-004 (before drawing-export task) |
| 4 | Validation execution model — per-frame Update polling vs event-driven on edit | (resolved inside P-001; promote to D-record only if non-obvious) |

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
