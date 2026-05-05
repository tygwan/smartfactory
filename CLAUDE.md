# AGENTS.md — SmartFactory

## Project context

Unity HDRP-based piping design / validation / 3D viewer tool, with a phased extension toward operations-side digital twin. Portfolio piece for one specific job application — see [D-001](docs/decisions/D-001-vision-and-phase-decomposition.md) for the full rationale, scope split (M1 vertical slice → M2 boost → M3 full ambition), and what is deliberately substituted (real CFD solver → ONNX surrogate). The `UnityFactorySceneHDRP` asset (factory environment + worker character) is reused as backdrop for authored piping, not as the primary subject. Currently entering **M1 (piping vertical slice)** with a 3-day application deadline.

## Stack

- **Language(s):** C#
- **Runtime / framework:** Unity 6000.4.4f1 LTS, HDRP 17.4.0
- **Package manager / build:** Unity Package Manager (UPM); Unity Editor build pipeline
- **Test framework:** Unity Test Framework (planned, not yet integrated)

## Reading order

When starting a fresh session, read in this order:

1. This file (`AGENTS.md` or `CLAUDE.md` — they are identical)
2. `README.md` (TBD — to be written during M1)
3. `docs/decisions/` — the active D-records (start with [D-001](docs/decisions/D-001-vision-and-phase-decomposition.md), the load-bearing scope decision)
4. [`docs/milestones/M1-piping-vertical-slice/README.md`](docs/milestones/M1-piping-vertical-slice/README.md) — current milestone scope, deliverables checklist, and open questions
5. Main scene at `Assets/UnityFactorySceneHDRP/Scene_Factory/FactorySceneSample.unity` — backdrop for authored piping; treat as read-only per AGENTS conventions

<!-- aegis:orchestration:begin -->

## Orchestration — when to do what

Use this lookup before reaching for a standard or template. Standards live in the aegis repo; link out to them as needed.

| Situation / what user says | aegis standard | aegis template | Produce in this project |
|---|---|---|---|
| First-day setup / convention change | [01-conventions](https://github.com/tygwan/aegis/blob/main/standards/01-conventions.md) | `AGENTS.template.md` | `AGENTS.md` (= `CLAUDE.md`) |
| "X vs Y" / structural choice | [02-decisions](https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md) | `D-record.template.md` | `docs/decisions/D-{NNN}-{slug}.md` |
| Before non-trivial work (>1 day or >3 sub-tasks) | [03-plans](https://github.com/tygwan/aegis/blob/main/standards/03-plans.md) | `Plan-record.template.md` | `docs/plans/P-{NNN}-{slug}.md` |
| End of meaningful work | [04-verification](https://github.com/tygwan/aegis/blob/main/standards/04-verification.md) | `V-record.template.md` | `docs/verifications/V-{NNN}-{slug}.md` |
| Routine commit | [05-changes](https://github.com/tygwan/aegis/blob/main/standards/05-changes.md) | (commit format) | git commit |
| Phase opens | [06-milestones](https://github.com/tygwan/aegis/blob/main/standards/06-milestones.md) | `Milestone-README.template.md` | `docs/milestones/M{N}-{slug}/README.md` |
| Phase closes (Closed or Abandoned) | [06-milestones](https://github.com/tygwan/aegis/blob/main/standards/06-milestones.md) + [07-learn-from-friction](https://github.com/tygwan/aegis/blob/main/standards/07-learn-from-friction.md) | `Retrospective.template.md` | `docs/milestones/M{N}-{slug}/retrospective.md` |
| External narrative (portfolio) | [07-learn-from-friction](https://github.com/tygwan/aegis/blob/main/standards/07-learn-from-friction.md) | `Portfolio-entry.template.md` | {project-specific location} |
| Same friction recurred | [07-learn-from-friction](https://github.com/tygwan/aegis/blob/main/standards/07-learn-from-friction.md) | (memory or standards refinement) | memory file or D-record |

<!-- aegis:orchestration:end -->

## Active conventions

### Language

- Chat collaboration: Korean
- Authored documents (markdown, code comments): English

### Records

This project follows aegis discipline:

- Structural decisions → `docs/decisions/D-{NNN}-{slug}.md`
- Non-trivial work → plan record at `docs/plans/P-{NNN}-{slug}.md`
- Verified work → `docs/verifications/V-{NNN}-{slug}.md`
- Milestone closures → retrospective at `docs/milestones/M{N}-{slug}/retrospective.md`

### Commits

Per aegis [05-changes](https://github.com/tygwan/aegis/blob/main/standards/05-changes.md):

- Format: `type(scope): summary`
- Imperative mood ("add", not "added")
- One logical change per commit
- Body explains *why*, references related D/P/V records

### Project-specific standards (optional)

None yet. When project-specific rules emerge beyond aegis's seven standards (e.g. Unity asset organization, scene structure conventions, C# style beyond defaults), place them at `docs/standards/local/{NN}-{slug}.md` and link here. Per aegis [01-conventions § Project-specific standards](https://github.com/tygwan/aegis/blob/main/standards/01-conventions.md).

## Working style

- **Foundation decisions** (architecture, dependency policy, milestone scope): present alternatives with tradeoffs grounded in project constraints; the user picks before execution. Take the time needed.
- **Execution after rules are set**: proceed briskly with short user confirmations ("진행", "권장 진행", "X로 진행"); no per-step option prompts.
- **Auto-OK**: editing files, compile/test verification, small commits, refactoring within a single file, reading the codebase.
- **Confirmation-required**: destructive git operations (`reset --hard`, force push, branch deletion), dependency upgrades or additions, the first D-record on a new topic, anything that modifies external assets under `Assets/UnityFactorySceneHDRP/` (we treat that asset as read-only by default).
- **Trajectory reporting**: when iterating on multi-round fixes (e.g. compile errors, warnings), report progress as a trajectory ("25 errors → 8 warnings → 0 new") so it is clear whether the loop is converging.

<!-- aegis:discipline:begin -->

## Discipline reminders

- A structural decision without a D-record is a future bug.
- A V-record's findings must either feed the next milestone or be explicitly dropped — never silently disappear.
- `AGENTS.md` and `CLAUDE.md` must stay identical. Edit one, mirror to the other, in the same commit.
- When two standards seem to conflict, propose a refinement via D-record; do not silently work around.

<!-- aegis:discipline:end -->

## Out of scope (current milestone — M1 piping vertical slice)

Items deliberately deferred. Revisit and update this list when M1 closes. See [`docs/milestones/M1-piping-vertical-slice/README.md`](docs/milestones/M1-piping-vertical-slice/README.md) and [D-001](docs/decisions/D-001-vision-and-phase-decomposition.md) for the phase decomposition (M1 → M2 → M3).

**Permanently out of scope** (no current milestone targets these):

- Production / standalone build distribution (.exe packaging, signing, installer)
- VR / AR support
- Multi-user collaboration / networked sessions
- Real-time external data ingestion from real fab sensors / MES / SCADA — substituted by surrogate per D-001
- Implementing a CFD solver from scratch — substituted by ONNX surrogate (planned for M2/M3)

**Deferred to M2** (post-application, pre-interview boost):

- Drawing 2D → 3D round-trip regeneration
- Flow-direction arrows / particle / flow-line visualization (C3)
- ONNX mini-surrogate training in Python + Unity inference (C4)

**Deferred to M3** (full A+C ambition per D-001):

- Semiconductor equipment mock-up prefabs
- Auto-routing / path-finding from start point to equipment
- CFD-comparison demo (best-route selection from surrogate output)

**Conditional in M1** (touched only via D-records):

- Modifying `Assets/UnityFactorySceneHDRP/` external asset code — minimal port of `CameraMove.cs` to new Input System is allowed only if D-003 decides so; otherwise treat the asset as read-only

## Memory

Claude Code auto-memory lives at `~/.claude/projects/-mnt-c-Users-x8333-Desktop-AI-PJT-unity-SmartFactory/memory/` (auto-created per session). Use it for user/feedback/project/reference memories per the auto-memory system; keep `MEMORY.md` as a concise index, not a content store.
