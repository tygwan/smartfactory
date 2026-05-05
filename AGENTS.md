# AGENTS.md — {project name}

> Conventions for any AI agent (Claude Code, Codex CLI, future agents) and any human contributor working on this project. **Remove this blockquote after filling in.**
>
> **Important**: After filling in, copy this entire file to `CLAUDE.md` at the project root with **identical content**. Both files must stay synchronized — that is what makes both Claude Code and Codex see the same conventions. See [`aegis/standards/01-conventions.md`](https://github.com/tygwan/aegis/blob/main/standards/01-conventions.md) "Two files, identical content" for the rationale.

## Project context

{One paragraph: what this project is, who it serves, current stage. State enough that a fresh agent session can orient itself in 30 seconds.}

## Stack

> Adapt rows to your project's reality. Drop rows that don't apply (a CLI tool may not have persistence; a library may not have a runtime/framework distinct from its language).

- **Language(s):** {e.g. TypeScript, Python, Rust, Go}
- **Runtime / framework:** {e.g. Node 20, Next.js 15, Django 5}
- **Package manager / build:** {e.g. pnpm 10, uv, cargo, gradle}
- **Persistence / infra:** {e.g. Postgres 16, Redis, S3 — drop if N/A}
- **Test framework:** {e.g. vitest, pytest, cargo test}

## Reading order

When starting a fresh session, read in this order:

1. This file (`AGENTS.md` or `CLAUDE.md` — they are identical)
2. `README.md`
3. `docs/decisions/` — the active D-records, especially the most recent
4. {project-specific: e.g. the architecture spec, the current milestone README}

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

- Chat collaboration: {e.g. Korean, English}
- Authored documents (markdown, code comments): {e.g. English}

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

{If your project needs additional rules beyond aegis's seven (e.g. API design conventions, library publication standards), place them at `docs/standards/local/{NN}-{slug}.md` and reference them here. Per aegis [01-conventions § Project-specific standards](https://github.com/tygwan/aegis/blob/main/standards/01-conventions.md).}

- {e.g. `docs/standards/local/01-api-design.md` — REST API conventions}

## Working style

- {Pacing preference: e.g. "for foundation decisions, take time and ground choices in business constraints; for execution after rules are set, work briskly without per-step option prompts"}
- {Auto-OK actions: e.g. "editing files, running tests, refactoring within a file"}
- {Confirmation-required actions: e.g. "destructive git operations, dependency upgrades, deploys"}

<!-- aegis:discipline:begin -->

## Discipline reminders

- A structural decision without a D-record is a future bug.
- A V-record's findings must either feed the next milestone or be explicitly dropped — never silently disappear.
- `AGENTS.md` and `CLAUDE.md` must stay identical. Edit one, mirror to the other, in the same commit.
- When two standards seem to conflict, propose a refinement via D-record; do not silently work around.

<!-- aegis:discipline:end -->

## Out of scope (current milestone)

{List of items deliberately deferred. Update this list at every milestone open.}

- {item 1}
- {item 2}

## Memory (optional)

{Pointer to project-specific persistent memory if used, e.g. Claude Code auto-memory at `~/.claude/projects/-path-to-project/memory/`.}
