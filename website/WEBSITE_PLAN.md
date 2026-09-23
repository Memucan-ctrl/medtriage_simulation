# MedTriage Website Implementation Plan

**Repository:** [https://github.com/Memucan-ctrl/medtriage_simulation](https://github.com/Memucan-ctrl/medtriage_simulation)  
**Document:** /website/WEBSITE_PLAN.md  
**Stack:** React 19 / Vite / TypeScript / Tailwind CSS / Lucide Icons

---

## 1. Target Audience
- **Medical & Nursing Educators:** Seeking evidence-based, objective XR simulation curricula.
- **Healthcare Students & Residents:** Needing deliberate practice for acute cardiac triage and ACLS resuscitation.
- **Simulation Center Directors & Evaluators:** Evaluating automated scoring and debriefing telemetry.
- **Technical Evaluators & XR Developers:** Reviewing Unity, OpenXR, and biomechanical interaction architecture.

---

## 2. Main Website Objective
To provide a clinical, trustworthy, and interactive presentation of the **MedTriage Virtual Reality Healthcare Simulation System**, clearly detailing its clinical problem space, verified interaction mechanics (pulse oximetry, biomechanical CPR, safe defibrillation), objective 5-tier scoring engine, and architectural foundation without hyperbole or unsupported medical claims.

---

## 3. Recommended Sitemap & Structure (Single Page App with Section Anchors)
- **Header / Navigation:** Brand identity, Navigation links (Overview, Mechanics, Spotlight, Flow, Scoring, Tech, Roadmap, Team), GitHub Repo CTA, Theme/Audio toggle.
- **Hero Section:** High-impact clinical headline, core value proposition, interactive Quick Metric Pill tags, Live Simulation Status Badge, Call-to-Action buttons (Explore Architecture, View GitHub).
- **Project Overview:** Clinical challenge vs MedTriage solution, supported hardware platforms, immersive training paradigm.
- **Core Clinical Systems (Feature Cards):**
  - Biomechanical CPR Depth & Rate Engine
  - Defibrillator & Closed-Loop Safety Interlocks
  - Interactive Virtual Patient Physiology
  - AI Teammate Clinical Delegation
  - Deterministic 5-Category Assessment Engine
  - Cloud-Synchronized Learner Profile
- **Interactive Spotlight: Pulse Oximeter Procedure:** Deep dive into PulseOximeter_Probe, Oximeter_FingerSocket, XR Socket Interactor, and physiological state triggering.
- **Simulation Workflow Timeline:** Interactive step-by-step walkthrough from Authentication to Scenario Selection, Triage, Resuscitation, and Automated Debrief.
- **Live Telemetry & Vitals Monitor Simulator:** Interactive web simulation of the bedside monitor showing real-time waveform rhythms (Sinus, VFib, ROSC) and CPR gauge metrics.
- **Objective Scoring & Debriefing Matrix:** Interactive breakdown of the 5 weighted competency categories (Protocol Adherence, Efficiency, Technical Execution, Team Communication, Decision-Making) and critical error safety gates.
- **Technology & Architecture:** Runtime stack (Unity, OpenXR, URP, XR Interaction Toolkit) vs Development tooling (Unity MCP, Python asset pipeline).
- **Development Roadmap:** Available Now vs In Development vs Planned milestones.
- **Visual Gallery & Screenshot Checklist:** Clean visual presentation with guided screenshot capture parameters for Unity evaluators.
- **Team & Open Collaboration:** Verified repository contributors with GitHub profile links.
- **Footer & Clinical Disclaimer:** Explicit educational-use disclaimer, version info, repository links.

---

## 4. Visual Direction & Design Tokens
- **Aesthetic:** Modern, clinical, premium, data-dense yet uncluttered.
- **Color Palette:**
  - navy-950: #030712 (Deep Obsidian background)
  - navy-900: #0a1128 (Surface cards)
  - teal-400: #2dd4bf (Medical telemetry accent)
  - cyan-400: #22d3ee (XR interaction highlight)
  - emerald-400: #34d399 (Sinus rhythm / normal vitals)
  - amber-400: #fbbf24 (Warning / Defib charging)
  - rose-400: #f87171 (Critical error / VFib rhythm)
- **Typography:** Inter / Outfit for ultra-clean readability; JetBrains Mono / Space Mono for clinical telemetry values.
- **Micro-Interactions:** Subtle glows, pulse animations for ECG waveforms, interactive sliders for compression depth/rate demonstration.

---

## 5. Required Screenshots and Media
- Official Brand Logos: Integrated from Medtriage/Assets/_Frontend/UI/Brand/PNG/.
- Interactive SVG ECG waveforms and cardiac monitor visuals.
- Captured Scene View / Game View placeholders detailed in SCREENSHOT_CHECKLIST.md.

---

## 6. Accessibility & Performance
- Full keyboard navigation with visible focus rings.
- Contrast ratio >= 4.5:1 (WCAG AA).
- prefers-reduced-motion compliance on dynamic waveforms.
- Zero layout shifts, asynchronous asset loading.
