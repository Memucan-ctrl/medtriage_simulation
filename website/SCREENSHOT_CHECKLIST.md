# MedTriage Unity Screenshot Capture Checklist

**Document:** /website/SCREENSHOT_CHECKLIST.md  
**Purpose:** Precise instructions for capturing promotional and technical screenshots from the Unity project to populate the website gallery.

---

## Safety & Compliance Rules
- **Privacy:** Hide any local path headers, Windows taskbar elements, personal user names, or internal debug console messages containing local file URIs.
- **Aspect Ratio:** Capture in 16:9 ratio (recommended: 1920x1080 or 2560x1440).
- **Format:** Save as .webp or compressed .png (under 500 KB per image).

---

## Screenshot Matrix

### 1. Bedside Overview & Virtual Patient
- **Scene to Open:** MedTriage-CardiacMVP/Assets/MedTriage/Scenes/Cardiac_MVP.unity
- **View:** Game View (Maximized) or Scene View with Game View aspect
- **Target Object:** Full patient on hospital bed with bedside vitals monitor and crash cart in view
- **Suggested Camera Angle:** Eye-level clinician perspective (35° elevation, 1.8m back from bed)
- **Target Filename:** bedside_patient_overview.png
- **Key Details to Show:** Reactive patient model, hospital room architecture, ambient lighting.

### 2. Pulse Oximeter Finger Attachment (Spotlight)
- **Scene to Open:** MedTriage-CardiacMVP/Assets/MedTriage/Scenes/Cardiac_MVP.unity
- **View:** Scene View (Close-up) or XR Simulator View
- **Target Object:** PulseOximeter_Probe docked onto Oximeter_FingerSocket on patient index finger
- **Suggested Camera Angle:** Close macro view (45° angle, 0.4m from hand)
- **Target Filename:** pulse_oximeter_probe_docked.png
- **Key Details to Show:** Probe clip, wire alignment, socket interaction highlight.

### 3. Defibrillator Charging & Pad Placement
- **Scene to Open:** MedTriage-CardiacMVP/Assets/MedTriage/Scenes/Cardiac_MVP.unity
- **View:** Game View / Desktop Test Rig
- **Target Object:** DefibrillatorDevice with sternum/apex pads placed on chest and charge indicator active
- **Suggested Camera Angle:** 45° angle showing both defibrillator screen and patient torso
- **Target Filename:** defib_pad_placement_charge.png
- **Key Details to Show:** Energy level display, pad position indicators, safety buttons.

### 4. Biomechanical CPR Compression HUD
- **Scene to Open:** Medtriage/Assets/Scenes/SampleScene.unity or Cardiac_MVP.unity
- **View:** Game View during simulated compression stroke
- **Target Object:** CompressionGaugeHUD showing depth gauge (green target zone) and cycle cadence timer
- **Suggested Camera Angle:** Learner head-height looking down at patient sternum
- **Target Filename:** cpr_compression_gauge.png
- **Key Details to Show:** 5cm depth target marker, rate counter (100-120 cpm).

### 5. Automated Debrief & Scoring Dashboard
- **Scene to Open:** Medtriage/Assets/_Frontend/Scenes/MainMenu.unity or runtime Debrief UI
- **View:** Game View (1920x1080)
- **Target Object:** DebriefScreenUI
- **Suggested Camera Angle:** Front-facing UI panel
- **Target Filename:** debrief_scoring_screen.png
- **Key Details to Show:** Category bars (Protocol Adherence, Technical Execution, etc.), critical error safety summary, coaching narrative.

### 6. AI Teammate Radial Delegation Menu
- **Scene to Open:** Medtriage/Assets/MedTriage/Scenes/Cardiac_MVP.unity
- **View:** Game View with delegation UI active
- **Target Object:** Radial selection menu showing AttachMonitor, GiveMedication, GetIvAccess, DocumentTime
- **Suggested Camera Angle:** First-person view with teammate in mid-ground
- **Target Filename:** team_delegation_radial.png
- **Key Details to Show:** Available teammate status (Idle / Performing), task icons.
