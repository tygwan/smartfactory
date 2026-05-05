# D-003: Input system strategy

| | |
|---|---|
| **Status** | Accepted |
| **Date** | 2026-05-06 |
| **Deciders** | coffin; Claude Code (advisory) |
| **Supersedes** | — |
| **Superseded by** | — |

## Context

The HDRP template enabled the new **Input System Package** as the active input handler. The external asset under `Assets/UnityFactorySceneHDRP/Player/Scripts/CameraMove.cs` reads `UnityEngine.Input` (legacy API), causing per-frame `InvalidOperationException` in Play mode. A temporary workaround set Active Input Handling to **Both**.

This must become a permanent decision before M1 begins because:
- A1–A4 (pipe authoring) requires custom author-mode camera + click-to-place input — built on whichever input layer we commit to
- AGENTS.md treats `Assets/UnityFactorySceneHDRP/` as read-only by default — depending on its `CameraMove.cs` is a soft violation that needs explicit acknowledgement or rejection
- The portfolio narrative benefits from input handling that is "our code, new Input System" rather than "external script wrapped in compatibility mode"

## Options considered

### Option A — Keep external `CameraMove.cs`, ship as-is via Active Input "Both"

Use the existing `Player.prefab` from the external asset as the camera in the M1 working scene. Author-mode controls (clicking points, selecting pipes) layer on top via the legacy API.

**Pros**
- Zero implementation cost for the camera layer
- Camera works immediately

**Cons**
- Permanent dependency on external code we cannot point to as our own work
- Mixed input APIs (legacy + new) in a single project — confusing for the portfolio reader
- Author-mode click handling on legacy API is a step backwards relative to JD-aligned modern patterns

### Option B — Active Input "Both" + self-authored controls, do not use external `Player.prefab` in our scene (chosen)

Keep Active Input Handling at "Both" (the external asset's compile and runtime code remain valid in its own scenes), but in our M1 working scene at `Assets/Project/Scenes/M1_PipingAuthor.unity` we instantiate **only** our own camera + authoring rig. The external `Player.prefab` and `CameraMove.cs` are never referenced from our scene; we write a fresh `AuthorCameraController` against the new Input System (`InputAction` based).

**Pros**
- External asset stays untouched (read-only honored)
- All input code in our project is unified on the new Input System — clean portfolio story
- The custom author camera is on the critical path for A1 (two-click pipe placement) anyway — building it costs no *additional* time over Option C
- Active Input "Both" is a passive setting; it does not force us to use the legacy API anywhere we own

**Cons**
- ~1.5 hour upfront cost for `AuthorCameraController` (orbit + fly + raycast for click)
- Project carries a "Both" setting that future readers may briefly question (mitigated by this D-record)

### Option C — Revert Active Input to new Input System exclusively

Revert `ProjectSettings.asset` to "Input System Package (New)" only. External asset's legacy code will throw exceptions if its scenes are opened, but those scenes are not part of our build path.

**Pros**
- Cleanest single-mode setting

**Cons**
- External demo scenes (`FactorySceneSample.unity`, etc.) become broken in Play mode — losing the ability to inspect them as references during development
- Not justified by any benefit beyond aesthetic purity

## Decision

**Option B**: Active Input Handling stays "Both"; in our M1 working scene we use only self-authored controls written against the new Input System; the external `Player.prefab` / `CameraMove.cs` are not instantiated in our scene.

## Consequences

**Unlocks:**
- `AuthorCameraController` becomes the foundation for A1–A4 pipe authoring (orbit/fly camera + click raycast doubles as pipe-endpoint placement)
- `Assets/Project/Scripts/Authoring/` and `Assets/Project/Scripts/Viewer/` can be entirely greenfield code — directly demonstrable as the applicant's own work
- External factory scene remains usable as a reference / sandbox without breaking

**Locks in:**
- All input handling we author uses `InputAction` / `InputActionAsset` (new Input System); no `UnityEngine.Input.*` calls in our codebase
- The M1 scene `Assets/Project/Scenes/M1_PipingAuthor.unity` does **not** include the external `Player.prefab`

**Cost:**
- ~1.5 hours in P-001 to write `AuthorCameraController` + an `InputActionAsset` for author mode (orbit, fly, click, F-toggle)

**Open questions:**
- Specific `InputAction` map shape (binding scheme, keyboard vs mouse layering) — resolved inside P-001, not D-record material
- Whether to add gamepad bindings — out of scope for M1; revisit if a deliverable requires it

## References

- Related D-records: [D-001](D-001-vision-and-phase-decomposition.md), [D-002](D-002-piping-data-model.md)
- Related P-records: P-001 (M1 implementation plan, anticipated)
- Unity Input System docs: <https://docs.unity3d.com/Packages/com.unity.inputsystem@latest>
- aegis 02-decisions standard: <https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md>
