# V-001: M1 vertical slice

| | |
|---|---|
| **Date** | 2026-05-07 |
| **Verifier** | coffin |
| **Plan** | [P-001](../plans/P-001-m1-implementation.md) |
| **Milestone** | [M1-piping-vertical-slice](../milestones/M1-piping-vertical-slice/README.md) |
| **Related D-records** | [D-001](../decisions/D-001-vision-and-phase-decomposition.md), [D-002](../decisions/D-002-piping-data-model.md), [D-003](../decisions/D-003-input-system-strategy.md), [D-004](../decisions/D-004-mcp-for-unity-adoption.md) |

## Hypothesis

The M1 vertical slice — pipe authoring, real-time clash validation, 3D viewer with metadata + analytic pressure gradient, and a top-down drawing seed export — is end-to-end demonstrable in `M1_PipingAuthor.unity`, runs without exceptions, and produces the artifacts P-001's done criterion enumerates. The implementation should be visibly aligned with the JD's three core responsibilities (배관 설계 / 실시간 검증 / 3D 뷰어).

## Method

Verification combined live MCP-driven inspection of the Unity scene against the M1 README scope checklist with the verifier's interactive smoke tests in Play mode and Edit mode. No automated test suite was integrated in M1 — that is captured in Findings below.

- **Compile check**: `mcp__unity__read_console` with type filter `error` returned 0 entries on the final build.
- **Authoring (Play mode)**: PipeAuthoringTool was wired to AuthorCameraController and PipeNetwork.asset. Two-click placement worked end-to-end (cyan first-point marker → second click commits a `PipeData`). Selection of a placed pipe surfaced its metadata in PipeInspectorPanel.
- **Authoring (Edit mode)**: SmartFactory → Pipe Scene Authoring opened a working EditorWindow. With Authoring Active toggled on, two clicks in the Scene view added a pipe to the assigned PipeNetwork.asset and the pipe rendered immediately via the PipeView's [ExecuteAlways] reconciliation.
- **Validation V1**: Verifier authored two intersecting pipes; both swapped to PipeClash material and AlertCanvas top-right text rendered "[!] N clashes detected" listing the offending pair indices. Removing/separating the pipes returned the pipes to gradient and the alert to "[OK] No clashes detected".
- **Auto-elbow A2**: With the network containing 4 endpoint-bucket matches, `find_gameobjects search="PipeJoint"` returned 4 spheres of localScale ≈ max(diameter) × 1.35.
- **Pressure gradient C1**: Visual inspection — pipes rendered with cool-gray-blue → warm-gray-red lerp by L/D⁵ relative magnitude, after the metallic surface polish (Metallic 0.9, Smoothness 0.7).
- **Future-extension label C2**: `FutureNoteText` rendered the M3 surrogate caveat at the bottom-left of AlertCanvas.
- **Drawing seed export §14**: `DrawingExportTool` Editor menu items registered. Live invocation via `execute_menu_item` failed in this session because Unity Editor was backgrounded and skipped script reload; the code is in place and the menu becomes invokable as soon as the verifier focuses the Editor.

## Result

- All A1–A3 / V1 / B1–B3 / C1–C2 scope items demonstrated working in the Editor.
- A2 auto-elbow demonstrated working (4 PipeJoint instances confirmed via MCP).
- Drawing seed §14 — code and menu registered; PNG generation verified by inspection of code paths only (manual menu invocation pending Editor focus).
- A4 T-junction stretch — not implemented (cut per P-001 risk § cut-order).
- V2 sharp-bend — pure function `ValidationRules.DetectSharpBends` implemented; not yet wired to a UI surface (`OnSharpBendsUpdated` event not added).
- V3 slope rule — not implemented (cut per P-001 risk § cut-order).
- B2 layer toggles — `LayerToggleController` and `PipeView.ShowClashHighlights` shipped; UI scene wiring (Toggle GameObjects + field assignment) deferred to verifier's manual UI build.
- Demo video §15 — depends on user-side OBS capture; not yet recorded at V-001 close.
- Figma page §16 — outline written at `docs/portfolio/M1-portfolio-outline.md`; the Figma file itself depends on user-side build.
- M1 retrospective §17 — written at `docs/milestones/M1-piping-vertical-slice/retrospective.md` alongside this V-record.

## Conclusion

**Partially verified.** The functional vertical slice (authoring + validation + viewer + visual hint + drawing export code) is demonstrable end-to-end. Three deliverables remain user-actioned at the time of this record: B2 toggle UI wiring, demo video recording, and the Figma file build. None are blocked technically; all carry forward into the milestone close as user follow-ups, not as outstanding code work.

## Deviations

| # | Planned | Actual | Reason |
|---|---|---|---|
| 1 | A4 T-junction stretch | Not implemented | P-001 risk § cut-order; demoability and portfolio artifacts prioritized |
| 2 | V3 slope rule stretch | Not implemented | Same cut-order |
| 3 | V2 sharp-bend with UI | Pure function only | UI scene wiring deferred; cut at the same boundary as V3 in priority |
| 4 | MCP for Unity stdio (per D-004) | Switched to HTTP local mid-M1 (commit `7f1b47b`) | stdio file-based discovery failed across WSL2 ↔ Windows-native Unity; HTTP local with WSL2 mirrored networking unblocked |
| 5 | First-pipe creation only via Play mode | Edit-mode Scene authoring tool added (PipeSceneAuthoringWindow) | JD §1 alignment: CAD/BIM-style Edit-mode click loop is a stronger demo than Play-only and was within reach |
| 6 | Single pipe material | PipeStandard material polished (Metallic 0/0.5 → 0.9/0.7) + pressure gradient color desaturation | Earlier shading read as "fluorescent paint"; metallic finish unifies visually with existing fab pipework |

## Findings

1. **WSL2 ↔ Windows MCP friction** — Severity: med. The CoplayDev unity-mcp file-based instance discovery does not bridge the WSL2/Windows-native filesystem boundary. HTTP local on `127.0.0.1:8080/mcp` works only after `wsl --shutdown` with `[wsl2] networkingMode=mirrored` in `.wslconfig`. Captured in commit `7f1b47b`'s body and now formally noted here for M2 retrospective input.
2. **Unity background-mode does not reload changed scripts** — Severity: low. While the user is focused away from the Unity Editor, newly created Editor scripts (DrawingExportTool) do not register their MenuItems; `execute_menu_item` returns "invalid or context-dependent". Workaround: focus the Editor (Alt+Tab) once after each Editor-script change.
3. **Pressure gradient relies on HDRP/Lit `_BaseColor` MaterialPropertyBlock override** — Severity: low. Worked first try but is HDRP-pipeline dependent. M2 should consider Shader Graph or a per-pipe material instance fallback for pipelines other than HDRP/Lit.
4. **No automated tests in M1** — Severity: med. P-001's done criterion mentions an EditMode test (`JsonUtility` round-trip + `DetectClashes` against a known overlap). The harness was not set up; the validation rules are pure static functions and the test is straightforward to add. M2 to wire `Assets/Project/Tests/` with `.asmdef` and a starter EditMode test.
5. **External asset auto-mutation noise** — Severity: low. `Assets/UnityFactorySceneHDRP/.../{Glass.mat, Line_Acryl.mat, VolumeProfile.asset}` were re-touched by Unity each session. Reset before each commit kept the diff clean. M2 candidate: add a one-line D-record acknowledging Unity's HDRP material auto-migration if it persists.
6. **TextMeshPro fallback** — Severity: trivial. `LiberationSans SDF` lacks `⚠`/`✓`/`Δ`. ASCII substitution unblocked the alert UI; the inspector panel still uses `Δ` because the font does cover Greek capital Delta in fallback range. Watch for further glyph misses in M2.

## Follow-ups

| # | Description | Owner | Target | Status |
|---|---|---|---|---|
| 1 | Wire B2 toggle UI (3 Toggle GameObjects under AlertCanvas + LayerToggleController serialized fields) | coffin | M1 close (manual) | open |
| 2 | Record demo video and capture link in retrospective | coffin | M1 close | open |
| 3 | Build Figma page 1.0 from `docs/portfolio/M1-portfolio-outline.md` | coffin | M1 close | open |
| 4 | EditMode test scaffolding (`Assets/Project/Tests/` + `.asmdef`) with `DetectClashes` and `JsonUtility` tests | coffin | M2 P-002 | open |
| 5 | Wire V2 sharp-bend results into a UI panel (or extend AlertCanvas list) | coffin | M2 | open |
| 6 | Implement V3 slope-direction rule | coffin | M2 | open |
| 7 | A4 T-junction / spline branch authoring | coffin | M2 (or M3 stretch) | open |
| 8 | D-005 record formalizing HTTP local MCP transport (supersedes D-004's stdio choice) | coffin | M1 close or M2 open | open |
| 9 | Drawing 2D ⇄ 3D round-trip (drawing seed → 3D regeneration) | coffin | M2 §14 | open |

## References

- Related V-records: — (V-001 is the first verification record)
- Related D-records: D-001, D-002, D-003, D-004
- Commits (selected): `dfc56d2 → a727427` (commit hashes spanning bootstrap → material polish; full trail at https://github.com/tygwan/smartfactory/commits/main)
