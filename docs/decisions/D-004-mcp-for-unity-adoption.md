# D-004: MCP for Unity adoption

| | |
|---|---|
| **Status** | Accepted (transport choice partially superseded) |
| **Date** | 2026-05-06 |
| **Deciders** | coffin; Claude Code (advisory) |
| **Supersedes** | — |
| **Superseded by** | [D-005](D-005-mcp-transport-http-local.md) — transport selection only; the adoption decision (use MCP for Unity at all) stands |

## Context

M1 is in flight. After D1 §1–§4 (data model, author camera, authoring tool, pipe view) and §7 (V1 clash detection) landed, the remaining D2/D3 work shifts from pure scripting toward **Unity Editor scene manipulation**: Canvas + UGUI panel setup, component wiring with serialized-field assignments, material creation and shader graph authoring, prefab construction, layer toggling, and capture pipelines for the demo video. Each of these is a multi-step Editor click sequence.

Two friction points compound under the M1 time box:

1. **Round-trip cost.** The current loop is: agent writes a script → user opens Unity → user manually creates Canvas/components/wires fields → user reports back → agent commits. Each round costs minutes of focus and tokens of context for the verbal Editor walkthrough. UI work (which the M1 deliverable cannot skip) is the worst offender — a single panel needs Canvas + Text + Component + multiple field assignments, all of which are mechanical.
2. **Console error surface.** When something doesn't compile or behaves wrong in Play mode, the user has to copy/paste the Console contents back into chat. We've already done this twice (CameraMove input conflict, focus issue). Direct console access would compress these debugging cycles dramatically.

Independent of friction, two business signals also point toward MCP adoption:

- The job posting at 이안BIM lists **"Coding Agent를 활용한 개발 경험 (Claude Code, Cursor 등)"** in 우대사항. The company also offers Claude Code subscriptions and operates an in-house LLM server. Demonstrably operating Claude Code with Unity-side automation is a direct portfolio signal that maps to the company's stated dev environment.
- The CoplayDev/unity-mcp project exposes 32+ tools (`manage_scene`, `manage_script`, `manage_gameobject`, `manage_asset`, `manage_material`, `manage_physics`, `read_console`, `execute_menu_item`, `batch_execute`, etc.) — direct coverage of the operations M1's remaining work needs.

## Options considered

### Option A — stdio transport via project-level `.mcp.json` (chosen)

Register the MCP server in `.mcp.json` at the project root with the `uvx`-managed bridge process. Claude Code spawns the bridge per session; the bridge talks to the Unity Editor's in-process listener.

**Pros**
- Per-project scope — only this repository activates Unity tooling, no leakage into unrelated sessions
- Claude Code manages the bridge lifecycle automatically — no separate server to keep alive
- `uvx` handles Python venv isolation transparently — no ambient pollution
- File is committable, so the configuration is part of the repo's reproducible state — anyone cloning gets the same agent capability

**Cons**
- Requires Python 3.10+ and `uv` on the developer machine
- Bridge process is per-Claude-Code-session — slightly slower cold start than a long-lived HTTP server (negligible at human cadence)

### Option B — HTTP local transport

Unity package launches an HTTP server on `localhost:8080`; Claude Code (or any MCP client) connects to it.

**Pros**
- Server stays warm across sessions and clients
- Multiple clients (Cursor, Claude Desktop, etc.) can share the same Unity instance

**Cons**
- User must manage server lifetime manually — adds an "is the server up?" failure mode
- Less hermetic — the same port is shared if another tool collides

### Option C — HTTP remote transport

Run the bridge on another machine and tunnel.

**Pros**
- Useful for distributed teams sharing one Unity instance

**Cons**
- Solves no problem present in this single-developer portfolio project
- Adds network configuration surface for zero gain

### Option D — Decline MCP for Unity

Continue the manual round-trip pattern.

**Pros**
- Zero new dependency

**Cons**
- M1's UI-heavy D2/D3 phase is exactly where the round-trip cost compounds
- Forfeits a 우대사항-aligned portfolio signal that costs ~5 minutes to obtain
- Already paid setup cost twice in informal Q&A (Input System debugging, focus issue) that MCP would have collapsed

## Decision

**Option A** — stdio transport via project-level `.mcp.json`.

## Consequences

**Unlocks:**
- Scene manipulation automation: Canvas creation, component addition, serialized-field wiring, prefab setup — all from the agent side
- Material and shader graph asset creation directly from script
- Console error inspection without copy/paste — agent reads it directly via `read_console`
- `batch_execute` for compound operations: ten Editor actions in one tool call instead of ten round-trips, which collapses both wall-clock and tokens
- The portfolio narrative gains a concrete "Coding Agent + Unity MCP collaboration" axis aligned with the JD's preferred-experience clause

**Locks in:**
- Python 3.10+ and `uv` are required tooling on any machine running this project
- `.mcp.json` is committed and treated as project-level configuration; modifying transport later is a D-record amendment, not a casual edit
- Activation requires a Claude Code session restart after this commit

**Cost:**
- One-time setup: install `uv` if absent, restart Claude Code session, verify `mcp__unity__*` tools appear in the deferred tool list
- Token-side cost is bounded: tool descriptions contribute to the system prompt at session start (one-time), per-call responses are no larger than the manual walkthroughs they replace

**Open questions:**
- Token-usage measurement under realistic M1 work — is `batch_execute` saving the predicted 10–100x in our actual usage pattern? Resolved by retrospective metrics rather than a D-record.
- Whether `manage_scene` / `manage_gameobject` responses will need filter parameters to avoid dumping large hierarchies; observe in M1 use, refine via local convention or a follow-up D-record if it bites

## References

- Related D-records: [D-001](D-001-vision-and-phase-decomposition.md) (vision and JD context), [D-002](D-002-piping-data-model.md), [D-003](D-003-input-system-strategy.md)
- Related P-records: P-001 (M1 implementation plan — D2/D3 work expected to be the primary beneficiary)
- External: <https://github.com/CoplayDev/unity-mcp>
- Job description: 이안BIM "FAB 3D 배관설계 솔루션 개발자" — 우대사항 § "Coding Agent를 활용한 개발 경험 (Claude Code, Cursor 등)"
- aegis 02-decisions standard: <https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md>
