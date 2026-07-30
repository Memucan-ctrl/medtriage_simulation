# MedTriage Cardiac Arrest Integration Handoff

## Implemented

A scene-independent integration package was implemented. No Simulation scene, model, XR Origin, Build Settings, MainMenu, TaskCatalog, package manifest, or API secret was modified.

### Shared foundation

- `Assets/_Shared/Scripts/Data/ScoreCategories.cs`
- `Assets/_Shared/Scripts/Data/EventLogEntry.cs`
- `Assets/_Shared/Scripts/Data/CriticalErrorRule.cs`
- `Assets/_Shared/Scripts/Data/CategoryWeighting.cs`
- `Assets/_Shared/Scripts/Data/ScenarioDefinition.cs`
- `Assets/_Shared/Scripts/Managers/ScoringCalculator.cs`
- `Assets/_Shared/Scripts/Managers/TaskManager.cs`

### Simulation integration

- `Assets/_Simulation/Scripts/CardiacArrestSceneController.cs`
- `Assets/_Simulation/Scripts/Interactions/DefibrillatorController.cs`
- `Assets/_Simulation/Scripts/Networking/DebriefCoachingService.cs`

All implemented scripts passed Unity structural validation with zero errors and warnings. Unity reported no C# compiler errors.

## Existing integration contract

The implementation reuses `TaskResult`, `SessionManager.CurrentTaskId`, `SessionManager.RecordProgressAsync`, `SessionManager.ReturnToMenu`, `CloudSaveManager`, and `TaskCatalog`.

Flow:

`MainMenu -> SessionManager.LoadTask -> Simulation -> TaskManager.Complete -> optional AI proxy -> Cloud Save -> Debrief -> ReturnToMenu`

## Canonical identifiers

- Task and Scenario ID: `cardiac_arrest_01`
- Scene: `Simulation_CardiacArrest01`
- Display name: `In-Hospital Cardiac Arrest`

The TaskCatalog already contains this entry. Do not create a duplicate.

## Scoring behaviour

- Gemini never grades the trainee.
- Only categories with logged events are assessed.
- Unassessed categories are omitted instead of receiving an automatic 100.
- Composite weights normalize across assessed categories.
- Critical errors use stable IDs.
- `TaskManager.Complete()` creates at most one result.

## AI security

`DebriefCoachingService` calls a secure backend proxy only. Never place a Gemini API key in Unity, Inspector fields, source code, Git history, documentation, or a client build. Leave the proxy URL blank until the backend is deployed. Proxy failure uses a local coaching fallback and does not block Cloud Save or debrief.

Expected proxy response:

```json
{
  "summary": "Coaching feedback",
  "strengths": ["..."],
  "improvements": ["..."],
  "disclaimer": "Educational feedback only."
}
```

## Simulation-team wiring

1. Create `Assets/_Simulation/Scenes/Simulation_CardiacArrest01.unity`.
2. Add a Camera, Directional Light, and the agreed XR Origin or desktop test rig.
3. Add `TaskManager` to an empty GameObject.
4. Create `Assets > Create > Medtriage > Scenario Definition`.
5. Set ScenarioId to `cardiac_arrest_01`; clinically review rhythm, reversible cause, timing, weights, and critical-error wording.
6. Add `CardiacArrestSceneController`; assign the ScenarioDefinition and TaskManager.
7. Leave the proxy URL blank until the secure backend is deployed.
8. Add the defibrillator prop and `DefibrillatorController`; wire Charge, Call Clear, and Shock.
9. Add final patient, team, CPR, rhythm, medication, animation, audio, and HUD systems.
10. Add the scene to Build Settings only when ready.
11. Test from Bootstrap, not directly from the Simulation scene.

## Recommended critical-error IDs

- `no_compressions_30s`
- `shock_non_shockable`
- `no_clear_call`
- `premature_termination`

Descriptions require clinical approval.

## Remaining work

- Patient and teammate assets and Animator Controllers
- Compression tracking and clinical calibration
- Rhythm monitor
- Medication tray
- Reversible-causes checklist
- Team delegation
- Final HUD and debrief prefab wiring
- Secure Gemini backend and final proxy URL
- Build Settings registration
- Full XR test

## Collaboration rules

- The Simulation team exclusively owns `Simulation_CardiacArrest01.unity`.
- Do not modify MainMenu for scenario wiring.
- Do not duplicate TaskCatalog entries.
- Merge shared contracts through `main`; the Simulation team merges `origin/main` into `simulation`.
- Commit scene assembly separately from shared scripts.
- Do not commit MCP packages, captures, API keys, or credentials.

## End-to-end validation

`Bootstrap -> authentication -> Scenarios -> cardiac arrest -> TaskId match -> safe/unsafe defibrillation -> finish once -> optional AI/local fallback -> Cloud Save result_cardiac_arrest_01 -> ReturnToMenu -> completed badge`


## Temporary smoke-test scene policy

The intended final user flow is `Bootstrap -> Login/Registration -> MainMenu -> Scenarios -> In-Hospital Cardiac Arrest`. That flow becomes available only after the Simulation team creates `Simulation_CardiacArrest01.unity`, wires the integration components, and adds the scene to Build Settings.

`CardiacArrest_SmokeTest.unity` is optional and exists only for isolated developer testing before the final scene is ready. If anyone creates it:

- Do not add it to Build Settings.
- Do not push or merge it.
- Do not rename it to the final scene name.
- Remove it before staging the integration commit, or keep it as an ignored local file.
- Do not delete the shared scripts or handoff documentation after testing.

The Simulation team should create and commit only the final `Assets/_Simulation/Scenes/Simulation_CardiacArrest01.unity` scene after wiring and reviewing it.
