import React from 'react';
import { CheckCircle2, Clock, Calendar, Compass, Shield } from 'lucide-react';

export const Roadmap: React.FC = () => {
  return (
    <section id="roadmap" className="py-20 bg-navy-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Section Header */}
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medCyan-400 bg-medCyan-950/80 px-3 py-1 rounded-full border border-medCyan-500/30">
            Development Status
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Current Status & Architectural Roadmap
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            Honest tracking of verified features versus in-development and planned milestones.
          </p>
        </div>

        {/* 3 Columns: Available Now, In Development, Planned */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          
          {/* Column 1: Available Now */}
          <div className="p-6 rounded-2xl bg-navy-900/60 border border-emerald-500/30 shadow-xl flex flex-col justify-between">
            <div>
              <div className="flex items-center justify-between pb-3 border-b border-slate-800 mb-4">
                <div className="flex items-center gap-2">
                  <CheckCircle2 className="w-5 h-5 text-emerald-400" />
                  <h3 className="font-bold text-white text-base">Available Now</h3>
                </div>
                <span className="text-[10px] font-mono uppercase px-2 py-0.5 rounded bg-emerald-950/80 text-emerald-300 border border-emerald-500/30">
                  Verified
                </span>
              </div>

              <ul className="space-y-3 text-xs text-slate-300">
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Pulse Oximeter Probe:</strong> XR socket docking, probe filtering, spatial audio, and vitals triggering.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Patient Physiology Model:</strong> 5-stage deterioration state machine with interactive head gaze.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Biomechanical CPR Engine:</strong> 5cm depth & 100-120 cpm tracking with hands-off interruption timers.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Defibrillator Safety Interlocks:</strong> Closed-loop "Call Clear" confirmation & non-shockable rhythm gating.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Deterministic Scoring Engine:</strong> 5 weighted competency categories with critical safety error flags.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-emerald-400 font-bold">✓</span>
                  <span><strong>Authentication & Main Menu:</strong> Bootstrap flow, user profile loading, and Cloud Save integration.</span>
                </li>
              </ul>
            </div>

            <div className="mt-6 pt-3 border-t border-slate-800/80 text-[11px] text-emerald-400/80 font-mono">
              ● Tested in Cardiac_MVP.unity
            </div>
          </div>

          {/* Column 2: In Development */}
          <div className="p-6 rounded-2xl bg-navy-900/60 border border-amber-500/30 shadow-xl flex flex-col justify-between">
            <div>
              <div className="flex items-center justify-between pb-3 border-b border-slate-800 mb-4">
                <div className="flex items-center gap-2">
                  <Clock className="w-5 h-5 text-amber-400" />
                  <h3 className="font-bold text-white text-base">In Development</h3>
                </div>
                <span className="text-[10px] font-mono uppercase px-2 py-0.5 rounded bg-amber-950/80 text-amber-300 border border-amber-500/30">
                  Active
                </span>
              </div>

              <ul className="space-y-3 text-xs text-slate-300">
                <li className="flex items-start gap-2">
                  <span className="text-amber-400 font-bold">⚙</span>
                  <span><strong>Final Scene Integration:</strong> Unifying all components into <code className="font-mono text-amber-300">Simulation_CardiacArrest01.unity</code>.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-amber-400 font-bold">⚙</span>
                  <span><strong>Secure LLM Proxy Backend:</strong> Production deployment of the external coaching summary proxy server.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-amber-400 font-bold">⚙</span>
                  <span><strong>Advanced Medication Tray:</strong> Multi-dose epinephrine syringe delivery with IV port cannula visual feedback.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-amber-400 font-bold">⚙</span>
                  <span><strong>AI Teammate Voice Lines:</strong> Full voiceover audio integration for team communication acknowledgement.</span>
                </li>
              </ul>
            </div>

            <div className="mt-6 pt-3 border-t border-slate-800/80 text-[11px] text-amber-400/80 font-mono">
              ● Part B/C Integration Handoff
            </div>
          </div>

          {/* Column 3: Planned */}
          <div className="p-6 rounded-2xl bg-navy-900/60 border border-blue-500/30 shadow-xl flex flex-col justify-between">
            <div>
              <div className="flex items-center justify-between pb-3 border-b border-slate-800 mb-4">
                <div className="flex items-center gap-2">
                  <Calendar className="w-5 h-5 text-blue-400" />
                  <h3 className="font-bold text-white text-base">Planned</h3>
                </div>
                <span className="text-[10px] font-mono uppercase px-2 py-0.5 rounded bg-blue-950/80 text-blue-300 border border-blue-500/30">
                  Roadmap
                </span>
              </div>

              <ul className="space-y-3 text-xs text-slate-300">
                <li className="flex items-start gap-2">
                  <span className="text-blue-400 font-bold">→</span>
                  <span><strong>Multiplayer Collaborative Resuscitation:</strong> Multi-user VR team triage sessions (multiplayer center manifest ready).</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-blue-400 font-bold">→</span>
                  <span><strong>Reversible Causes Diagnostic Checklist:</strong> Interactive ultrasound & blood gas assessment (Hs & Ts).</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-blue-400 font-bold">→</span>
                  <span><strong>Multi-Scenario Library:</strong> Expanding catalog beyond Cardiac Arrest to Respiratory Failure & Trauma Triage.</span>
                </li>
                <li className="flex items-start gap-2">
                  <span className="text-blue-400 font-bold">→</span>
                  <span><strong>Institutional LMS Integration:</strong> LTI / SCORM standard exports for university curriculum reporting.</span>
                </li>
              </ul>
            </div>

            <div className="mt-6 pt-3 border-t border-slate-800/80 text-[11px] text-blue-400/80 font-mono">
              ● Future Release Targets
            </div>
          </div>

        </div>

      </div>
    </section>
  );
};
