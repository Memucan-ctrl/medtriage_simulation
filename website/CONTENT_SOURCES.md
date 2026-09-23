# MedTriage Content Sources & Repository Mapping

**Document:** /website/CONTENT_SOURCES.md  
**Purpose:** Precise mapping of every technical and clinical claim on the website to its verified source file in the repository.

---

## 1. Project Overview & Clinical Scenario
| Website Statement | Repository Source File | Notes / Code Reference |
| :--- | :--- | :--- |
| **Project Identity & Task ID** | Medtriage/Assets/_Simulation/Documentation/CardiacArrest_Integration_Handoff.md | Task ID: cardiac_arrest_01, Display: "In-Hospital Cardiac Arrest" |
| **Scenario Definition & Parameters** | Medtriage/Assets/_Shared/Data/CardiacArrest01Scenario.asset | Max Cycles: 4, Epinephrine Interval: 180s, Cycle: 120s, Cause: Hypoxia |
| **Authentication Flow** | Medtriage/Assets/_Frontend/Scenes/Login.unity | CloudSave authentication and profile loading |

---

## 2. Interactive Mechanics
| Website Statement | Repository Source File | Notes / Code Reference |
| :--- | :--- | :--- |
| **Pulse Oximeter Probe Socket** | MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/PulseOximeterProbe.cs | XRSocketInteractor, requires PulseOximeter_Probe, plays audio, calls Presence.OnOximeterAttached() |
| **Patient Physiology & Gaze** | MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Patient/PatientPresence.cs | 5 States (Awake, Monitored, Deteriorating, Arrested, Rosc), head rotation tracking (42° max), vitals interpolation |
| **CPR Compression Depth & Rate** | MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/CompressionDetector.cs | Target depth 5cm (0.05m ± 1cm), rate 100-120 cpm, 10s interruption warning, 30s critical stop |
| **Defibrillator Interlocks & Charge** | MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/DefibrillatorController.cs | Audio whine, capacitor charging, CallClear() requirement, shockable rhythm check |
| **AI Teammate Delegation** | Medtriage/Assets/_Simulation/Scripts/Characters/AITeammate.cs | 4-state state machine (Idle, Called, Performing, Done), 4 tasks in TeammateTaskType.cs |
| **Medication & IV Injection** | MedTriage-CardiacMVP/Assets/MedTriage/_Simulation/Scripts/Interactions/SyringeInjector.cs | Syringe plunger displacement and IV port injection mechanics |

---

## 3. Telemetry, Assessment & Scoring
| Website Statement | Repository Source File | Notes / Code Reference |
| :--- | :--- | :--- |
| **5 Score Categories** | Medtriage/Assets/_Shared/Scripts/Data/ScoreCategories.cs | Efficiency, Protocol Adherence, Technical Execution, Team Communication, Decision-Making |
| **Weighting Formulation** | Medtriage/Assets/_Shared/Data/CardiacArrest01Scenario.asset | Protocol: 30%, Efficiency: 25%, Technical: 20%, Communication: 15%, Decision: 10% |
| **Critical Safety Errors** | Medtriage/Assets/_Shared/Scripts/Data/CriticalErrorRule.cs | 
o_compressions_30s, shock_non_shockable, 
o_clear_call, premature_termination |
| **Secure AI Coaching Proxy** | Medtriage/Assets/_Simulation/Scripts/Networking/DebriefCoachingService.cs | Calls backend proxy only; zero keys in Unity client; local fallback included |
| **Cloud Progress Persistence** | Medtriage/Assets/_Shared/Scripts/Managers/CloudSaveManager.cs | Unity Cloud Save integration |

---

## 4. Technology & Infrastructure
| Website Statement | Repository Source File | Notes / Code Reference |
| :--- | :--- | :--- |
| **XR Interaction Toolkit (v3.5.1)** | MedTriage-CardiacMVP/Packages/manifest.json | Core VR grab, poke, and socket infrastructure |
| **OpenXR VR Standards (v1.17.1)** | MedTriage-CardiacMVP/Packages/manifest.json | Quest 3, Quest Pro, and PCVR compatibility |
| **XR Hands Tracking (v1.8.0)** | MedTriage-CardiacMVP/Packages/manifest.json | Articulated hand mesh & gestures |
| **Universal Render Pipeline (v17.5.0)** | MedTriage-CardiacMVP/Packages/manifest.json | High performance mobile VR lighting |
| **Development Tooling vs Runtime** | Packages/manifest.json & CardiacArrest_Integration_Handoff.md | Unity MCP editor tooling used during development, not runtime dependency |
