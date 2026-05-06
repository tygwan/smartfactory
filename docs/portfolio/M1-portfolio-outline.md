# SmartFactory — M1 Portfolio Outline

> Source-of-truth outline for the Figma portfolio page 1.0. Updated as the demo recording lands and Figma is built.
> Target audience: 이안BIM (FAB 3D 배관설계 솔루션 개발자) hiring reviewers.

## Page 1.0 structure

### 1. Hero — first 3 seconds

- **Headline (Korean)**: SmartFactory — Unity HDRP fab piping authoring + real-time validation + 3D viewer
- **One-line subtitle (Korean)**: 이안BIM 채용 포트폴리오. 3일 vertical slice; M2/M3 단계로 도면 round-trip + CFD surrogate 확장.
- **Hero visual**: 1 still from the demo video — pipe network glowing with pressure gradient, clash pair flashing red, AlertCanvas overlay visible top-right
- **CTA**: ▶ Demo video (1–2 min, embedded)

### 2. Responsibility mapping (JD → deliverable)

Single table, no decoration. Three rows, three columns.

| 이안BIM JD 책무 | M1 deliverable | M2/M3 확장 |
|---|---|---|
| 배관 설계 핵심 기능 | Two-click authoring (Play mode) + Edit-mode Scene view authoring (CAD-style window) + auto-elbow sphere fittings + ScriptableObject single-source-of-truth | M2 도면 ⇄ 3D round-trip / M3 auto-routing |
| 실시간 검증 | V1 clash detection (segment-segment Eberly distance, event-driven, material swap) + V2 sharp-bend detection (joint angle threshold) | V3 slope rule / extra fab code rules |
| 3D 뷰어 | Self-authored AuthorCameraController (new Input System) + clickable selection + metadata panel + analytic pressure gradient overlay + future-extension label + top-down PNG drawing seed | C3 flow arrows / C4 ONNX surrogate inference / M3 CFD comparison |

### 3. Architecture diagram (single column, full-width)

**FigJam diagram (auto-generated, claim into team)**:
<https://www.figma.com/board/uKyPifbPYrBtcSytRyeRIw?utm_source=claude&utm_content=edit_in_figjam>

Open the link, claim/copy into `tg yoon의 팀`, then export the cleaned-up version into the Figma page below. Reference layout (ASCII) follows for offline reading:

```
                ┌─────────────────────────────────┐
                │  PipeNetworkAsset (SO)          │  ← single source of truth
                │  List<PipeData> + OnChanged     │
                └────────────┬────────────────────┘
                             │
        ┌────────────────────┼────────────────────────┐
        │                    │                        │
   Authoring               Validation                Viewer
   ─────────               ──────────                ──────
   • Play-mode             • DetectClashes           • PipeView (cylinder mesh)
     PipeAuthoringTool       (segment dist)            + auto-elbow sphere
   • Edit-mode             • DetectSharpBends        • PipeInspectorPanel
     PipeSceneAuthoring-     (joint angle)             (selection + metadata)
     Window                  pure functions          • ClashAlertPanel
   • Selection click                                  • pressure gradient
     hits PipeViewItem                                  (analytic)
                                                     • DrawingExportTool
                                                       (top-down PNG)
        │                                                 │
        └─────────────── M2 boundary ─────────────────────┘
                                │
                                ▼
        Drawing 2D ⇄ 3D round-trip (deferred)
                                │
                                ▼
        ONNX CFD surrogate inference (M3)
                                │
                                ▼
        Auto-routing + CFD comparison demo (M3)
```

### 4. Decision trail (column of compact cards)

For each, link to the D-record on GitHub.

- **D-001** Vision and 3-phase decomposition — A+C integrated
- **D-002** Piping data model — ScriptableObject + JSON
- **D-003** Input system — new Input System "Both"; self-authored controls
- **D-004** Adopting MCP for Unity — stdio (later HTTP local; M1 retrospective will capture WSL2/Windows friction)
- (P-001) M1 implementation plan — D1/D2/D3 schedule, cut-order applied during execution

The hiring company explicitly lists "Coding Agent를 활용한 개발 경험 (Claude Code, Cursor 등)" as preferred experience — this section is direct alignment evidence.

### 5. Engineering hygiene (compact bullets, light callout)

- aegis discipline: `docs/decisions/`, `docs/plans/`, `docs/verifications/`, `docs/milestones/M1-…/retrospective.md`
- TDD-friendly: validation rules are pure static functions over `IReadOnlyList<PipeData>`
- DDD-friendly: domain model lives in `Assets/Project/Scripts/Data/` (ScriptableObject + plain serializable struct), no Unity coupling at the data layer
- Reproducible env: `.mcp.json` committed, package manifest pinned
- Single git history with descriptive commits; commit count and trail visible at https://github.com/tygwan/smartfactory

### 6. M2/M3 roadmap (one slim column)

- **M2 (+1 week)** — drawing roundtrip 3D ⇄ 2D, ONNX mini-surrogate (Python), flow arrows
- **M3 (+2-3 weeks)** — equipment mock-up, auto-routing (A* / RRT), CFD-comparison demo
- Each phase ends with a portfolio update, retrospective, and git tag

## Build notes

- Page format: single-column, ~1280 wide, mobile-friendly fallback
- Color palette aligned with the demo material gradient (cool gray / muted blue / muted red-orange)
- Typography: sans-serif body, monospaced code blocks
- Embed: demo MP4 hosted on GitHub release or Vimeo; URL captured in `retrospective.md`
- Logo / personal brand: TBD (sender's preference)

## Production checklist (run before publishing)

- [ ] Demo video recorded (≤ 2 min, captioned segments mapping to each JD responsibility)
- [ ] Hero still extracted from video and color-graded to match palette
- [ ] All D-record/P-record/V-record/retrospective links verified to point at the correct commit hashes
- [ ] GitHub repo public + README pointing at this page
- [ ] Figma share link permissions set correctly (public read-only)
