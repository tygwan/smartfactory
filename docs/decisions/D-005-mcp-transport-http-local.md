# D-005: MCP transport — HTTP local (supersedes D-004's stdio choice)

| | |
|---|---|
| **Status** | Accepted |
| **Date** | 2026-05-07 |
| **Deciders** | coffin; Claude Code (advisory) |
| **Supersedes** | [D-004](D-004-mcp-for-unity-adoption.md) — stdio transport selection only; D-004's adoption decision (use MCP for Unity at all) stands |
| **Superseded by** | — |

## Context

[D-004](D-004-mcp-for-unity-adoption.md) chose stdio transport for the CoplayDev unity-mcp bridge. During M1 D2 work, the stdio configuration was deployed and the bridge process spawned successfully, but `mcpforunity://instances` returned `instance_count: 0` while the Unity Editor's own MCP for Unity panel showed the in-process server as **active**.

Investigation traced the root cause to **filesystem boundary mismatch between WSL2 and Windows-native Unity**:

- The stdio transport relies on a file-based instance discovery mechanism. The Unity package writes its instance metadata to a path on the host filesystem (Windows: `C:\Users\<user>\.unity-mcp\`). The Python bridge process — spawned by Claude Code under WSL2 — reads from `/home/<user>/.unity-mcp/`. The two paths are different filesystems; neither side sees the other's state.
- A direct TCP probe from WSL2 to Windows (`curl 127.0.0.1:8080`) confirmed the connection refused — WSL2's `127.0.0.1` is its own loopback, not Windows-native loopback.
- WSL2 gateway IP (`172.21.16.1`) timeouts at the Windows side because the unity-mcp HTTP server defaults to loopback-only binding (`127.0.0.1`, `localhost`, `::1`).

A working transport was required to continue M1 D2/D3 (UI scaffolding for A3 selection, B3 metadata panel, C2 future-extension label, B2 layer toggles). With the manual round-trip cost compounding under the time box, leaving MCP broken was not an option.

The fix needed to be **a transport choice**, not a discovery patch — the file-based discovery is intrinsic to stdio mode and not configurable from the bridge side.

## Options considered

### Option A — HTTP local with WSL2 mirrored networking (chosen)

Switch the Unity package to its HTTP transport (default `localhost:8080/mcp`). On the Claude Code side, change `.mcp.json` to a URL-based MCP server entry:

```json
{
  "mcpServers": {
    "unity": {
      "type": "http",
      "url": "http://127.0.0.1:8080/mcp"
    }
  }
}
```

Cross the WSL2 ↔ Windows boundary by enabling **mirrored networking** in `~/.wslconfig`:

```ini
[wsl2]
networkingMode=mirrored
```

After `wsl --shutdown`, WSL2's `127.0.0.1:8080` mirrors Windows-native loopback at the same port. The HTTP transport then connects directly without any file discovery.

**Pros**
- Single configuration — no path translation logic required
- mirrored networking is a Microsoft-supported feature on Windows 11 22H2+
- Recovers the full MCP capability surface that D-004 motivated
- HTTP transport is CoplayDev's documented default for Claude Desktop/Cursor/Windsurf

**Cons**
- mirrored networking requires Windows 11 22H2+; older Windows versions need a different fix
- Slightly more permissive than stdio (HTTP listener exposed on loopback) — irrelevant on a single-developer machine but worth noting

### Option B — Bridge stdio with environment-variable path override

Configure the WSL bridge to read instance discovery from a Windows-native path via the `/mnt/c/...` mount.

**Pros**
- Keeps the originally chosen transport

**Cons**
- The CoplayDev bridge does not expose such an environment variable; would require a fork or vendor patch
- Brittle — any path change in the unity-mcp package breaks the override

### Option C — Run Claude Code natively on Windows

Eliminates the WSL2 boundary entirely.

**Pros**
- All transports work without networking workarounds

**Cons**
- The user's existing Claude Code installation is on WSL2; switching mid-M1 would have cost more than the M1 milestone could absorb
- Reasonable as a longer-term migration but not as a mid-milestone fix

### Option D — Continue without MCP

Manual round-trip for all Editor scene mutations.

**Pros**
- Zero infrastructure work

**Cons**
- Eats the time savings that motivated D-004; defeats the portfolio signal toward "Coding Agent를 활용한 개발 경험"
- Already proven costly during M1's first MCP outage

## Decision

**Option A — HTTP local at `http://127.0.0.1:8080/mcp` with WSL2 mirrored networking.** `.mcp.json` updated and committed at `7f1b47b`. The user's `.wslconfig` was updated and `wsl --shutdown` executed; on the new WSL session, MCP for Unity reconnected and all tool calls resumed.

## Consequences

**Unlocks:**
- All MCP tooling for the remainder of M1: scene authoring (`manage_gameobject`, `manage_components`), material edits (`manage_material`), asset creation (`manage_scriptable_object`), and console diagnosis (`read_console`). The B2/V2/A2/C1/C2 work after this decision shipped via batched MCP calls.
- A documented setup procedure for any future contributor on WSL2 — see `~/.wslconfig` mirrored-networking note plus the Unity package side toggle.

**Locks in:**
- `.mcp.json` is a HTTP url-based server; reverting to stdio in the future requires another D-record.
- The project assumes Windows 11 22H2+ for any developer using WSL2. Windows 10 / WSL1 contributors will need an alternative (Option C — run Claude Code natively) and that alternative should be documented when it first matters.

**Cost:**
- One-time user setup: edit `.wslconfig`, `wsl --shutdown`, restart Claude Code session.
- Documentation cost — this D-record + the M1 retrospective entry.

**Open questions:**
- Whether to add a project-local standard at `docs/standards/local/01-mcp-tooling.md` or treat this D-record as sufficient. Defer until a second similar decision arises.
- HTTP transport reliability under long sessions — in M1 the connection was transient-disconnected several times during long-running work; CoplayDev's keep-alive logging suggests this is a known characteristic. Monitor in M2 and only re-decide if it degrades meaningfully.

## References

- Supersedes: [D-004](D-004-mcp-for-unity-adoption.md) (stdio transport choice)
- Related D-records: [D-001](D-001-vision-and-phase-decomposition.md)
- Commit that applied the transport switch: `7f1b47b`
- CoplayDev unity-mcp HTTP docs: <https://github.com/CoplayDev/unity-mcp>
- WSL2 mirrored networking: <https://learn.microsoft.com/en-us/windows/wsl/networking#mirrored-mode-networking>
- aegis 02-decisions standard: <https://github.com/tygwan/aegis/blob/main/standards/02-decisions.md>
