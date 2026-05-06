# SmartFactory

> Unity HDRP 기반 fab 배관 설계 / 실시간 검증 / 3D 뷰어 — 이안BIM **FAB 3D 배관설계 솔루션 개발자** 포지션 지원용 portfolio 프로젝트.

3일 vertical slice (M1) → 도면 round-trip + ONNX 기반 CFD surrogate (M2) → equipment mock-up + auto-routing + CFD 비교 (M3) 의 단계적 확장. 전체 비전과 단계 분해는 [D-001](docs/decisions/D-001-vision-and-phase-decomposition.md) 참조.

## Status

- **M1 (piping vertical slice)** — Closed 2026-05-07. [retrospective](docs/milestones/M1-piping-vertical-slice/retrospective.md), [V-001](docs/verifications/V-001-m1-vertical-slice.md).
- **M2 / M3** — open at user discretion; scope candidates listed in `AGENTS.md` "Out of scope (M2 candidates)".

## What's in M1

| 직무 책무 (이안BIM JD) | 구현 |
|---|---|
| 배관 설계 핵심 기능 | Play-mode 두 점 클릭 authoring · Edit-mode Scene view authoring (PipeSceneAuthoringWindow, CAD-style) · auto-elbow sphere fittings · ScriptableObject 기반 단일 데이터 소스 |
| 실시간 검증 | V1 clash detection (segment-segment 거리 + material swap + alert UI) · V2 sharp-bend detection (joint angle threshold) |
| 3D 뷰어 | 자체 작성 AuthorCameraController (new Input System) · 클릭 selection + metadata panel · analytic ΔP pressure gradient (M3에서 ONNX surrogate로 대체될 placeholder) · top-down PNG 도면 export |

핵심 설계 결정: [D-002](docs/decisions/D-002-piping-data-model.md) (데이터 모델), [D-003](docs/decisions/D-003-input-system-strategy.md) (입력 시스템), [D-004](docs/decisions/D-004-mcp-for-unity-adoption.md) + [D-005](docs/decisions/D-005-mcp-transport-http-local.md) (Coding Agent + MCP integration).

## Tech stack

- **Unity 6000.4.4f1 LTS** + **HDRP 17.4.0**
- **C#** (Unity Test Framework — M2에서 EditMode tests 추가 예정)
- **Coding Agent**: Claude Code + [CoplayDev unity-mcp](https://github.com/CoplayDev/unity-mcp) (HTTP local transport — D-005)
- **Unity 패키지**: Input System 1.19 · Splines 2.7 · Cinemachine 3.1 · ProBuilder 6.0 · VFX Graph 17.4 · TextMeshPro

## Reading order

1. **`AGENTS.md`** = **`CLAUDE.md`** — 작업 컨벤션 (둘은 byte-identical, aegis 01-conventions)
2. 이 README
3. **`docs/decisions/`** — D-records (D-001부터 시작)
4. **`docs/milestones/M1-piping-vertical-slice/README.md`** — M1 scope + checklist + closing notes
5. 메인 씬 `Assets/Project/Scenes/M1_PipingAuthor.unity` (Edit 모드에서 즉시 시각화 — `PipeView`의 `[ExecuteAlways]` 덕분)

## Run

1. Unity 6000.4.4f1 LTS Editor로 프로젝트 열기 (HDRP 17.4 자동 import)
2. Project 창에서 `Assets/Project/Scenes/M1_PipingAuthor.unity` 더블클릭
3. (선택) **SmartFactory → Pipe Scene Authoring** 메뉴 → 윈도우 열림 → `PipeNetwork.asset` 슬롯 채우고 Authoring Active ON → Scene view 클릭 두 번으로 파이프 추가
4. (선택) Play 모드 진입 → 우클릭 + WASD 카메라 이동 + 좌클릭 두 번으로 파이프 추가
5. (선택) **SmartFactory → Export Top-Down PNG** → `Assets/Project/Exports/M1_top.png` 생성

## Coding Agent collaboration

이 프로젝트는 Claude Code와 사람이 협업한 portfolio. 모든 의사결정은 [aegis discipline](https://github.com/tygwan/aegis)의 D / P / V / retrospective 레코드로 박제. git log + `docs/` 트리만 봐도 의사결정 트레일 추적 가능.

## Out of scope (영구)

- Production / standalone build distribution
- VR / AR
- Multi-user collaboration
- 실제 fab 센서 / MES / SCADA 연동 — surrogate로 대체
- CFD solver from scratch — surrogate로 대체

## Repository

GitHub: <https://github.com/tygwan/smartfactory>

## Portfolio

- M1 portfolio outline: [`docs/portfolio/M1-portfolio-outline.md`](docs/portfolio/M1-portfolio-outline.md)
- M1 demo video script: [`docs/portfolio/M1-demo-video-script.md`](docs/portfolio/M1-demo-video-script.md)
- FigJam architecture diagram: <https://www.figma.com/board/uKyPifbPYrBtcSytRyeRIw>
