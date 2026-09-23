# MedTriage Project Audit Report

**Audit Date:** September 2026  
**Repository:** [https://github.com/Memucan-ctrl/medtriage_simulation](https://github.com/Memucan-ctrl/medtriage_simulation)  
**Target Directory:** /website/PROJECT_AUDIT.md

---

## 1. Project Name and Purpose
- **Project Name:** MedTriage (Sub-projects: medtriage_simulation, MedTriage-CardiacMVP)
- **Primary Purpose:** A clinical virtual reality (VR/XR) and desktop simulation platform designed for immersive emergency response and triage training, specifically focusing on In-Hospital Cardiac Arrest (IHCA) protocols, rhythm analysis, CPR execution, team delegation, and closed-loop defibrillation.
- **Supporting Evidence:**
  - Medtriage/Assets/_Simulation/Documentation/CardiacArrest_Integration_Handoff.md: States "MedTriage Cardiac Arrest Integration Handoff" and canonical task cardiac_arrest_01 ("In-Hospital Cardiac Arrest").
  - Medtriage/Assets/_Shared/Data/CardiacArrest01Scenario.asset: Configures the canonical scenario definition.
  - MedTriage-CardiacMVP/Assets/MedTriage/Scenes/Cardiac_MVP.unity: Main clinical simulation environment.

---

## 2. Healthcare and Educational Problem Addressed
- **Clinical Training Deficit:** In-hospital cardiac arrest requires rapid, high-stakes decision-making under stress. Traditional manikin-based or lecture training often lacks real-time physiological reactivity, dynamic patient deterioration, and structured team coordination practice.
- **High-Risk Protocol Failures Addressed:**
  - Delay in chest compressions or interruptions longer than 10 seconds (
o_compressions_30s).
  - Failure to confirm safety clearance ("All Clear") before shock delivery (
o_clear_call).
  - Accidental shock delivery on non-shockable rhythms like Asystole/PEA (shock_non_shockable).
  - Misplacement of diagnostic equipment and delayed rhythm diagnosis.
- **Supporting Evidence:**
  - MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/DefibrillatorController.cs: Lines implementing shock safety interlocks and critical error logging for ShockDenyReason.ClearNotCalled and CriticalErrorId.ShockNonShockable.
  - MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/CompressionDetector.cs: Lines 67-85 tracking interruption timers (HandsOffWarningSeconds = 10f, CriticalErrorId.NoCompressions30s).
  - Medtriage/Assets/_Shared/Scripts/Data/CriticalErrorRule.cs: Structured safety violation definitions.

---

## 3. Intended Users
- **Medical & Nursing Students:** Developing foundational ACLS/BLS emergency protocol familiarity.
- **Emergency Care Practitioners & Residents:** Practicing high-fidelity resuscitation leadership and rhythm diagnosis.
- **Clinical Educators & Simulation Directors:** Conducting objective, data-driven competency evaluations with standardized debriefing metrics.
- **Supporting Evidence:**
  - Medtriage/Assets/_Frontend/Scenes/Login.unity & Registration.unity: Learner account and profile authentication.
  - Medtriage/Assets/_Shared/Scripts/Data/ScoreCategories.cs: 5 competency categories structured for academic assessment.

---

## 4. Main Simulation Workflow
The confirmed end-to-end user workflow is:
1. **Bootstrap & Authentication:** Initial system initialization (Bootstrap.unity) -> Learner Login / Registration (Login.unity, Registration.unity).
2. **Main Menu & Scenario Selection:** Browse active training catalog (MainMenu.unity), select scenario (cardiac_arrest_01 / In-Hospital Cardiac Arrest).
3. **Session Initialization:** SessionManager.LoadTask() initializes TaskManager and sets active ScenarioDefinition.
4. **Initial Assessment & Monitoring:** Learner enters cardiac room; patient is awake and breathing. Learner grabs PulseOximeter_Probe and docks it onto Oximeter_FingerSocket.
5. **Dynamic Patient Deterioration:** PatientPresence.OnOximeterAttached() shifts state from Awake to Monitored (HR 82, SpO2 97%). After 18s, patient deteriorates over 40s (tachycardia HR 150, SpO2 drops to 78%, shallow breathing at 30 bpm) before collapsing into full Ventricular Fibrillation cardiac arrest.
6. **Resuscitation Protocol:**
   - **Chest Compressions:** Continuous real-time depth (target 5 cm ± 1 cm) and rate (100–120 cpm) tracking via CompressionDetector.cs.
   - **Defibrillation:** Apply pads (DefibPadPlacement.cs), charge capacitor (BeginCharge()), declare safety clear (CallClear()), and deliver shock (TryDeliverShock()).
   - **Team Delegation:** Radial UI commands to AI Teammates (AITeammate.cs) for AttachMonitor, GiveMedication, GetIvAccess, DocumentTime.
   - **Medication:** Syringe draw and IV port injection (SyringeInjector.cs).
7. **Session Conclusion & Debrief:** TaskManager.Complete() computes weighted score across assessed categories, checks critical error violations, requests AI coaching summary via secure proxy (or local fallback in DebriefCoachingService.cs), saves progress to cloud (CloudSaveManager.cs), and renders interactive debrief screen (DebriefScreenUI.cs).
8. **Return to Menu:** Returns to MainMenu.unity with persistent completion badge.
- **Supporting Evidence:**
  - Medtriage/Assets/_Simulation/Documentation/CardiacArrest_Integration_Handoff.md
  - MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Patient/PatientPresence.cs
  - Medtriage/Assets/_Shared/Scripts/Managers/TaskManager.cs

---

## 5. Confirmed Features (Evidence-Backed)
1. **Pulse Oximeter Probe XR Interaction:**
   - Script: MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/PulseOximeterProbe.cs
   - Prefab/Socket: XRSocketInteractor on finger, strict name filtering for PulseOximeter_Probe, spatial audio feedback, vitals trigger.
2. **Dynamic Patient Presence & Physiological Model:**
   - Script: MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Patient/PatientPresence.cs
   - Interactive head tracking (MaxHeadTurnDegrees = 42f) looking toward learner; 5 clinical states (Awake, Monitored, Deteriorating, Arrested, Rosc).
3. **Biomechanical CPR Compression Tracking:**
   - Script: MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/CompressionDetector.cs
   - Calculates depth error against 0.05m target, rate quality (100-120 cpm), and interruption penalties.
4. **Defibrillator Charging & Shock Interlock System:**
   - Script: MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/DefibrillatorController.cs
   - Authentic capacitor charge audio whine, ready tone, rhythm classification requirement, and "Call Clear" enforcement.
5. **AI Teammate Task Delegation:**
   - Script: Medtriage/Assets/_Simulation/Scripts/Characters/AITeammate.cs
   - 4-state state machine (Idle, Called, Performing, Done) handling 4 core clinical delegation tasks.
6. **Deterministic 5-Category Objective Scoring:**
   - Script: Medtriage/Assets/_Shared/Scripts/Managers/ScoringCalculator.cs
   - Weightings: Protocol Adherence (30%), Efficiency (25%), Technical Execution (20%), Team Communication (15%), Decision-Making (10%). Unassessed categories are omitted without artificial penalization.
7. **Secure AI Debrief Coaching Architecture:**
   - Script: Medtriage/Assets/_Simulation/Scripts/Networking/DebriefCoachingService.cs
   - Secure proxy pattern preventing client-side API key exposure, with graceful local coaching fallback.
8. **Cloud Persistence:**
   - Script: Medtriage/Assets/_Shared/Scripts/Managers/CloudSaveManager.cs
   - Integration with Unity Services Cloud Save (com.unity.services.cloudsave v3.4.1).

---

## 6. Work-in-Progress & Planned Features
- **In Development:**
  - Final scene assembly into Simulation_CardiacArrest01.unity (standalone integration tested via Cardiac_MVP.unity).
  - Backend Gemini coaching proxy server deployment (currently uses local rule-based fallback).
  - Multi-scenario library extension (catalog initialized with cardiac_arrest_01).
- **Planned:**
  - Full multiplayer collaborative VR support (com.unity.multiplayer.center manifest dependency present).
  - Dynamic procedural reversible-causes checklist (Hs & Ts).
- **Supporting Evidence:**
  - Medtriage/Assets/_Simulation/Documentation/CardiacArrest_Integration_Handoff.md ("Remaining work" section).

---

## 7. Technologies and Packages Used
### Core Runtime Engine
- **Engine:** Unity 6000.0.x / URP (Universal Render Pipeline v17.5.0)
- **Language:** C# (.NET Standard / Unity runtime)
- **Render Pipeline:** Universal Render Pipeline (URP) with custom post-processing & volume profiles (CardiacMVP_URP.asset).

### Unity Packages (from Packages/manifest.json)
- com.unity.xr.interaction.toolkit (v3.5.1): XR socket interactors, grab interactables, direct interactors.
- com.unity.xr.openxr (v1.17.1): OpenXR standard VR device interface.
- com.unity.xr.hands (v1.8.0): Biomechanical hand tracking and gesture detection.
- com.unity.inputsystem (v1.19.0): Modern unified input system.
- com.unity.services.cloudsave (v3.4.1): Cloud learner persistence.
- com.unity.render-pipelines.universal (v17.5.0): High-performance mobile & desktop VR rendering.
- com.unity.ugui & TextMesh Pro: High-resolution world-space and screen-space HUDs.

### Development-Only Tooling (Explicitly Distinguished)
- com.ivanmurzak.unity.mcp / com.coplaydev.unity-mcp: Model Context Protocol editor connection for automated agentic development.
- Python Blender asset generator pipeline (uild_B1_defib.py through uild_B9_ivpole_bvm.py).

---

## 8. Supported Hardware & Target Platforms
- **Primary VR Target:** Meta Quest 3 / Quest Pro / Quest 2 (via OpenXR + Android build target with URP Mobile optimization).
- **Secondary / Testing Platform:** PC Standalone / XR Simulator (XRDeviceSimulatorSettings.asset, Cardiac_MVP_DesktopTest.unity).
- **Supporting Evidence:**
  - MedTriage-CardiacMVP/Assets/XR/Settings/OpenXR Package Settings.asset
  - MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Testing/DesktopCardiacTestingRig.cs
  - Medtriage/Assets/_Frontend/UI/Brand/PNG/ (Mobile/desktop assets)

---

## 9. Current Development Status
- **Status:** **Functional MVP / Pre-Release Integration**
- **Working Elements:** Full Cardiac MVP simulation loop, physical interactions (probe, defib, compressions, syringe), state machines, vitals monitors, deterministic scoring algorithms, authentication frontend.
- **Pending Elements:** Final scene build packaging and deployment of the external AI coaching proxy server.

---

## 10. Distinctive Differentiators
1. **Interactive Biomechanical Interlocks:** Does not simply play pre-rendered animations; physics and socket tracking require correct technique (depth, rate, pad orientation, clear calling).
2. **Reactive Virtual Patient Physiology:** Patient exhibits interactive head gaze tracking and continuous mathematical vitals degradation (interpolated SpO2, heart rate, respiration).
3. **Deterministic Objective Assessment:** Avoids non-deterministic AI grading. All scores derive from deterministic telemetry logs, with AI utilized purely for constructive conversational coaching summaries.
4. **Security-First Architecture:** Zero hardcoded API keys in client binaries; strictly proxies LLM coaching calls.

---

## 11. Safe & Suitable Media Assets for Website
- **Brand Assets:** Medtriage/Assets/_Frontend/UI/Brand/PNG/ contains high-resolution official vector/raster logos (_Master2048X512.png, _MasterHorizontal_Logo.png, _MasterIcon.png, _MasterLight_Horizontal_Logo.png).
- **Color Palette:**
  - Primary Dark Navy: #0a1128 / #001f3f
  - Medical Teal / Cyan: #00a896 / #028090 / #00e5ff
  - Warning / Defib Amber: #f4a261 / #e76f51
  - Neutral Background: #f8f9fa / #0d1b2a

---

## 12. Missing Information (To Be Inquired / Clarified)
- Official institutional / organizational affiliations (if any).
- Preferred public deployment domain or demo release timeline.
- Production URL for the backend LLM coaching proxy server.
