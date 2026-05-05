# D-002: Piping data model

| | |
|---|---|
| **Status** | Accepted |
| **Date** | 2026-05-06 |
| **Deciders** | coffin; Claude Code (advisory) |
| **Supersedes** | — |
| **Superseded by** | — |

## Context

Per [D-001](D-001-vision-and-phase-decomposition.md), the user-authored piping model is the single source of truth flowing through every milestone:

- M1 — pipe authoring + clash validation + 3D viewer + analytic pressure gradient
- M2 — drawing 2D ⇄ 3D round-trip; ONNX surrogate inference using the same pipe geometry
- M3 — auto-routing produces piping; CFD-comparison demo evaluates piping

Choosing the wrong storage shape now blocks M2 round-trip (the 3D model must serialize cleanly to and from a 2D drawing) and makes M3 auto-routing painful (the routing algorithm needs to *produce* this same data shape from scratch). The decision must land before any A1-A4 implementation begins.

## Options considered

### Option A — `ScriptableObject` per pipe (or a single `ScriptableObject` holding `List<PipeData>`)

Each pipe is described by a `PipeData` SO field set (start/end points, diameter, material, metadata). A `PipeView` MonoBehaviour on a scene GameObject references the SO. JSON IO via `JsonUtility.ToJson` / `FromJson` against the SO's serializable shape.

**Pros**
- Unity-native: Inspector edits work out of the box; asset references survive scene reloads
- Trivial JSON serialization → drawing export and surrogate input handoff are one-liners
- Clean separation: data lives in `Assets/_Project/Data/`, views live in scene → M2 round-trip means "deserialize JSON → create/update SO assets → views regenerate"
- M3 auto-routing produces a `List<PipeData>` directly, no scene marshalling

**Cons**
- Boilerplate cost (one SO class + a small editor for batch ops)
- One asset file per pipe can clutter folders at scale (mitigation: store all pipes in one `PipeNetworkAsset` SO with `List<PipeData>` — used as the default shape; per-pipe SO only if a specific pipe needs versioned overrides)

### Option B — Plain C# class + custom JSON serialization

Pure POCO domain model, no Unity types. Custom JSON IO. Scene GameObjects read from a runtime-held `List<PipeData>` and rebuild views on change.

**Pros**
- Cleanest domain — testable as pure functions, matches JD's TDD/DDD preference signal
- Zero Unity coupling at the domain layer

**Cons**
- No Inspector editing of pipes during M1 — must build a custom UI just to author a single pipe, eating D1
- Boilerplate burden is highest of the three options — must write Unity-side sync, custom JSON, and the editor experience all from scratch
- Wins (testability, decoupling) only pay off in M2/M3; M1 sees only the cost

### Option C — MonoBehaviour component holds the data directly

`PipeBehaviour` MonoBehaviour stores diameter, material, start/end as serialized fields on the GameObject itself. No separate data asset.

**Pros**
- Fastest to set up — zero abstraction
- Everything visible in Hierarchy / Inspector immediately

**Cons**
- Data is fused to scene → exporting requires walking the GameObject tree, deserializing each component's fields
- M2 round-trip becomes ugly: rebuild the *scene* from JSON rather than rebuild the *data*, then regenerate views
- M3 auto-routing must instantiate GameObjects to produce output — tight coupling between algorithm and scene
- This option saves ~2 hours in M1 and costs ~10 hours across M2 + M3

## Decision

**Option A — `ScriptableObject`-based piping data, with `PipeNetworkAsset` (SO holding `List<PipeData>`) as the default container; per-pipe SO escape hatch only if a real need surfaces.**

## Consequences

**Unlocks:**
- M2 drawing round-trip lands as `JSON ⇄ PipeNetworkAsset ⇄ scene views` — three layers, each responsibility clean
- M3 auto-routing is a pure function `(start, equipment, constraints) → PipeNetworkAsset` — no scene dependency in the algorithm
- Validation rules (clash, bend radius, slope) are pure functions on `PipeData` / `PipeNetworkAsset` — directly unit-testable, matching JD's TDD signal
- Pipe authoring tools edit `PipeNetworkAsset` and emit a change event; views subscribe and update — no per-frame polling

**Locks in:**
- Adding new per-pipe attributes (e.g. wall thickness, insulation) means evolving `PipeData`'s serializable fields and migrating existing assets — incremental cost is low because there is only one network asset in M1
- The pipe geometry primitive is a `(startPoint, endPoint, diameter)` line segment plus auto-elbow at intersections; non-cylindrical fittings (T-junction, reducer) join as additional `PipeData` variants flagged by an enum — handled when A4 stretch goal is implemented

**Cost:**
- ~1 hour boilerplate at start of D1: define `PipeData` (Serializable), `PipeNetworkAsset` (ScriptableObject), `PipeView` (MonoBehaviour referencing the network asset and an index)
- One change event channel (`PipeNetworkAsset.OnChanged`) wired to validation + view systems

**Open questions:**
- Pipe ID generation — GUID vs incrementing int. Resolved inside P-001 implementation plan; promote to D-record only if it has cross-milestone impact
- Coordinate system — world space vs local-to-equipment. Default world space for M1; revisit at M3 when equipment mock-ups appear

## References

- Related D-records: [D-001](D-001-vision-and-phase-decomposition.md) (vision and phases that depend on this data shape)
- Related P-records: P-001 (M1 implementation plan, anticipated next)
- aegis 02-decisions standard: <https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md>
- Unity ScriptableObject docs: <https://docs.unity3d.com/Manual/class-ScriptableObject.html>
