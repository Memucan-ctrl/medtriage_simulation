import React from 'react';
import { AlertCircle, Target, Users, Monitor, Sparkles, CheckCircle2 } from 'lucide-react';

export const ProjectOverview: React.FC = () => {
  return (
    <section id="overview" className="py-20 bg-slate-950/60 border-y border-slate-800/80 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Section Header */}
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medTeal-400 bg-medTeal-950/80 px-3 py-1 rounded-full border border-medTeal-500/30">
            Project Overview
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Bridging Clinical Protocol and Reflexive Deliberate Practice
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            In-Hospital Cardiac Arrest (IHCA) requires seamless team synchronization and immediate execution. MedTriage provides a safe, highly reproducible VR environment where clinicians practice critical interventional algorithms without patient risk.
          </p>
        </div>

        {/* 2-Column Challenge vs Solution */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          
          {/* Clinical Challenge Card */}
          <div className="p-7 rounded-2xl bg-navy-900/60 border border-slate-800/90 shadow-xl relative overflow-hidden">
            <div className="w-12 h-12 rounded-xl bg-rose-500/10 border border-rose-500/30 flex items-center justify-center mb-5 text-rose-400">
              <AlertCircle className="w-6 h-6" />
            </div>
            
            <h3 className="text-xl font-bold text-white mb-3">The Healthcare Education Challenge</h3>
            <p className="text-slate-300 text-sm leading-relaxed mb-6">
              Conventional ACLS manikin training often fails to replicate the physiological urgency and nuanced communication of an actual code event.
            </p>

            <ul className="space-y-3 text-sm text-slate-300">
              <li className="flex items-start gap-2.5">
                <span className="text-rose-400 font-bold">•</span>
                <span><strong>Compression Interruptions:</strong> Hands-off time exceeding 10 seconds drastically reduces coronary perfusion pressure.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <span className="text-rose-400 font-bold">•</span>
                <span><strong>Defibrillation Safety Breaches:</strong> Failure to confirm "All Clear" before shock delivery creates severe bystander hazard.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <span className="text-rose-400 font-bold">•</span>
                <span><strong>Non-Shockable Rhythm Errors:</strong> Inappropriate shock delivery during Asystole or PEA delays life-saving epinephrine.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <span className="text-rose-400 font-bold">•</span>
                <span><strong>Subjective Debriefing:</strong> Qualitative feedback without telemetry logs makes progress tracking inconsistent.</span>
              </li>
            </ul>
          </div>

          {/* MedTriage Solution Card */}
          <div className="p-7 rounded-2xl bg-gradient-to-br from-navy-900/80 to-slate-900/90 border border-medTeal-500/40 shadow-xl relative overflow-hidden">
            <div className="w-12 h-12 rounded-xl bg-medTeal-500/10 border border-medTeal-500/30 flex items-center justify-center mb-5 text-medTeal-400">
              <Target className="w-6 h-6" />
            </div>

            <h3 className="text-xl font-bold text-white mb-3">The MedTriage Architectural Solution</h3>
            <p className="text-slate-300 text-sm leading-relaxed mb-6">
              A reactive physiological state machine synchronized with physical XR socket interlocks and continuous telemetry logging.
            </p>

            <ul className="space-y-3 text-sm text-slate-300">
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-medTeal-400 shrink-0 mt-0.5" />
                <span><strong>Biomechanical CPR Engine:</strong> Real-time 5 cm depth and 100–120 cpm frequency scoring with instant visual HUD feedback.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-medTeal-400 shrink-0 mt-0.5" />
                <span><strong>Closed-Loop Safety Interlocks:</strong> Hardware shock buttons actively reject shock attempts until rhythm is analyzed and "Clear" is verbalized.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-medTeal-400 shrink-0 mt-0.5" />
                <span><strong>Reactive Virtual Patient:</strong> Dynamic head gaze tracking toward the learner, deteriorating from stable vitals to Ventricular Fibrillation.</span>
              </li>
              <li className="flex items-start gap-2.5">
                <CheckCircle2 className="w-4 h-4 text-medTeal-400 shrink-0 mt-0.5" />
                <span><strong>Deterministic 5-Tier Telemetry:</strong> Mathematical scoring across Protocol, Efficiency, Technical, Team, and Decision-Making metrics.</span>
              </li>
            </ul>
          </div>

        </div>

        {/* Hardware & Compatibility Grid */}
        <div className="mt-12 p-6 rounded-xl bg-navy-900/40 border border-slate-800 flex flex-col md:flex-row items-center justify-between gap-6">
          <div className="flex items-center gap-4">
            <div className="w-10 h-10 rounded-lg bg-medCyan-500/10 border border-medCyan-500/30 flex items-center justify-center text-medCyan-400">
              <Monitor className="w-5 h-5" />
            </div>
            <div>
              <h4 className="text-base font-bold text-white">Target Platforms & Hardware Support</h4>
              <p className="text-xs text-slate-400">Meta Quest 3 / Quest Pro / Quest 2 (via OpenXR) & Desktop XR Simulator Rig</p>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-2 text-xs font-mono">
            <span className="px-3 py-1 rounded-md bg-slate-800 text-slate-200 border border-slate-700">OpenXR 1.17</span>
            <span className="px-3 py-1 rounded-md bg-slate-800 text-slate-200 border border-slate-700">XR Interaction Toolkit 3.5.1</span>
            <span className="px-3 py-1 rounded-md bg-slate-800 text-slate-200 border border-slate-700">Universal Render Pipeline</span>
          </div>
        </div>

      </div>
    </section>
  );
};
