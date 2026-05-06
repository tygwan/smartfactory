# M1 demo video — recording script

> Target length: 90–120 seconds.
> Recorder: OBS Studio (already installed).
> Output: `M1_demo.mp4` placed under `docs/portfolio/` (or uploaded as a GitHub release asset and linked from the Figma page).
> Audio: optional voiceover in Korean; if no voiceover, use on-screen text overlays for each segment label.

## Pre-recording setup

1. Unity Editor focused, scene `Assets/Project/Scenes/M1_PipingAuthor.unity` loaded.
2. Hierarchy: `Player` and `Animation Sample` deactivated; `Line` / `Controller` / `Prop` reactivated; AlertCanvas children visible.
3. `PipeNetwork.asset` either fresh-cleared (Pipe Authoring window → Clear Network) or pre-populated with one demonstrative network. Both work; cleared looks cleaner and lets the build narrate itself.
4. Open `Window → Layouts → 2 by 3` (or any layout where Scene + Game + Console + Hierarchy + Inspector are all visible).
5. OBS Studio: source = Display Capture or Window Capture (Unity Editor only). Resolution 1920×1080. Bitrate 6000–8000 kbps.

## Segment plan

```
0:00 ─ 0:08   Opening title  (~8s)
0:08 ─ 0:25   Edit-mode authoring (CAD loop)  (~17s)
0:25 ─ 0:40   Real-time validation (V1 clash) (~15s)
0:40 ─ 0:55   3D viewer + metadata + gradient (~15s)
0:55 ─ 1:10   Drawing seed (top-down PNG export) (~15s)
1:10 ─ 1:30   Roadmap card (M2 / M3) (~20s)
```

## Segment 1 — Opening title (0:00–0:08)

- Black title card or Unity splash → fade.
- On-screen text: **"SmartFactory"** then **"Unity HDRP Fab Piping — M1 Vertical Slice"**.
- Voiceover (Korean): *"이안BIM 'FAB 3D 배관설계 솔루션 개발자' 포지션 지원용 포트폴리오. M1 vertical slice 데모입니다."*

## Segment 2 — Edit-mode authoring (0:08–0:25) ⟶ JD §1 mapping

- Unity menu: **SmartFactory → Pipe Scene Authoring** → window opens.
- Pipe Network slot already filled. Toggle **Authoring Active** ON.
- Move cursor to Scene view; click two points on the fab floor → first pipe rendered, mid-air joint not yet (only one pipe).
- Click two more points → second pipe; **PipeJoint** sphere appears at the shared endpoint (auto-elbow demo).
- On-screen text overlay: **"배관 설계 (Edit-mode authoring + auto-elbow)"**.
- Voiceover: *"Scene view에서 직접 클릭으로 배관을 그립니다. 만나는 지점은 자동으로 elbow fitting을 생성합니다."*

## Segment 3 — Real-time validation (0:25–0:40) ⟶ JD §2 mapping

- Add a third pipe that intersects an existing pipe.
- Both intersecting pipes flip red instantly (clash material).
- AlertCanvas top-right reads `[!] 1 clash detected\n  • #2(xxxxxx) ↔ #0(xxxxxx)` in red.
- Move the new pipe end so it's no longer intersecting (Inspector edit OR delete + redraw).
- Alert returns to green `[OK] No clashes detected`.
- On-screen text: **"실시간 검증 (clash detection, event-driven)"**.
- Voiceover: *"파이프 추가/이동 시 즉시 검증. 충돌 페어가 빨간색으로 표시되고 alert에 기록됩니다."*

## Segment 4 — 3D viewer + metadata + gradient (0:40–0:55) ⟶ JD §3 mapping

- Click an existing pipe in Scene view.
- Inspector panel (좌상단) updates: id, diameter, material, length, virtual ΔP.
- Toggle Authoring Active OFF; demonstrate orbit/fly camera if in Play mode (right-click + WASD).
- Highlight the cool→warm pressure gradient on the network.
- On-screen text: **"3D 뷰어 + analytic pressure gradient"**.
- Voiceover: *"파이프를 클릭하면 metadata가 표시됩니다. 색상 그라디언트는 analytic ΔP — M3에서 ONNX CFD surrogate로 대체될 placeholder입니다."*

## Segment 5 — Drawing seed (0:55–1:10) ⟶ M2 bridge

- Unity menu: **SmartFactory → Export Top-Down PNG**.
- Brief delay; Console prints `[Drawing] Top-down PNG exported: Assets/Project/Exports/M1_top.png`.
- Cut to file explorer or Project window showing `M1_top.png`.
- Open the PNG (double click).
- On-screen text: **"도면 export — M2에서 SVG round-trip으로 확장"**.
- Voiceover: *"3D 모델을 평면도 PNG로 추출. M2에서는 SVG 양방향 round-trip을 추가합니다."*

## Segment 6 — Roadmap card (1:10–1:30)

- Static slide / overlay listing:

  ```
  M1 (3 days)   ✓ Authoring + Validation + Viewer + Drawing seed
  M2 (+1 week)  Drawing roundtrip + ONNX surrogate + flow viz
  M3 (+2~3wk)   Equipment mock-up + Auto-routing + CFD comparison
  ```

- Voiceover: *"이번 M1은 vertical slice. M2/M3에서 도면 round-trip과 CFD surrogate, auto-routing까지 확장합니다. 이안BIM 제품 라인 — DTDbuilder, D·Master, Auto Bending, P·Master — 전체 표면을 단계적으로 커버하는 구조입니다."*

- Final card: **"GitHub: github.com/tygwan/smartfactory"** + Figma URL placeholder.

## Post-recording

1. Trim opening/closing dead air.
2. Export at 1080p, target file size ≤ 30 MB if possible (for GitHub release asset). Otherwise host on Vimeo.
3. Drop the URL into `M1-portfolio-outline.md` Production checklist + Figma page hero CTA.
4. Update `retrospective.md` Follow-up #2 status to "done" with the URL.

## Fallback if any clip fails

- If clash demo doesn't trigger reliably (e.g. raycast misses): pre-build a 4-pipe network with one intentional clash and start the recording with the network already present. Skip Segment 2's "build from empty"; keep Segment 3.
- If drawing export menu doesn't register: focus the Editor briefly, hit Ctrl+R (Refresh), then retry. If the menu still misses, fall back to executing the export from the Inspector via right-click on PipeNetwork.asset (custom Editor item, M2 candidate).
- If recording exceeds 2 minutes: cut Segment 5 (drawing export) entirely. M1 Figma page can show the exported PNG as a still instead.
