import React from 'react';
import { Activity, ShieldCheck, HeartPulse, Zap, Award, Layers, ArrowRight, Play, Eye } from 'lucide-react';

export const Hero: React.FC = () => {
  return (
    <section className="relative pt-32 pb-20 md:pt-40 md:pb-28 overflow-hidden medical-grid">
      {/* Ambient background glows */}
      <div className="absolute top-1/4 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[350px] bg-medTeal-500/10 rounded-full blur-3xl pointer-events-none -z-10" />
      <div className="absolute top-1/3 right-10 w-[400px] h-[300px] bg-medCyan-500/10 rounded-full blur-3xl pointer-events-none -z-10" />

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative">
        
        {/* Top Badges */}
        <div className="flex flex-wrap items-center justify-center gap-3 mb-6">
          <div className="inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full bg-slate-900/90 border border-slate-700/80 text-xs font-medium text-slate-300 shadow-sm">
            <span className="flex h-2 w-2 relative">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-medTeal-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2 w-2 bg-medTeal-500"></span>
            </span>
            <span>Task ID: <code className="text-medTeal-300 font-mono">cardiac_arrest_01</code></span>
          </div>

          <div className="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-navy-900 border border-medTeal-500/30 text-xs font-medium text-medTeal-300">
            <ShieldCheck className="w-3.5 h-3.5 text-medTeal-400" />
            <span>ACLS Resuscitation Protocol</span>
          </div>
        </div>

        {/* Hero Title & Subheading */}
        <div className="text-center max-w-4xl mx-auto">
          <h1 className="text-4xl sm:text-5xl md:text-6xl font-extrabold tracking-tight text-white leading-[1.15]">
            High-Fidelity Virtual Reality for{' '}
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-medTeal-300 via-medCyan-400 to-teal-200">
              Acute Cardiac Triage
            </span>
          </h1>
          
          <p className="mt-6 text-lg sm:text-xl text-slate-300 leading-relaxed max-w-3xl mx-auto font-normal">
            An evidence-based XR healthcare simulation built in <strong>Unity 6</strong> and <strong>OpenXR</strong>.
            Train deliberate decision-making for In-Hospital Cardiac Arrest with biomechanical CPR tracking, closed-loop defibrillator interlocks, and deterministic objective scoring.
          </p>

          {/* Action CTAs */}
          <div className="mt-8 flex flex-wrap items-center justify-center gap-4">
            <a
              href="#vitals-sim"
              className="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-gradient-to-r from-medTeal-500 to-medCyan-500 hover:from-medTeal-400 hover:to-medCyan-400 text-navy-950 font-bold text-base shadow-lg shadow-medTeal-500/25 transition-all hover:scale-[1.02] focus:outline-none focus:ring-2 focus:ring-medTeal-400"
            >
              <HeartPulse className="w-5 h-5 text-navy-950" />
              <span>Explore Monitor Simulator</span>
              <ArrowRight className="w-4 h-4" />
            </a>

            <a
              href="#oximeter"
              className="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-slate-900/90 hover:bg-slate-800 text-slate-200 font-semibold text-base border border-slate-700 hover:border-slate-600 transition-all focus:outline-none focus:ring-2 focus:ring-medTeal-400"
            >
              <Eye className="w-5 h-5 text-medTeal-400" />
              <span>Pulse Oximeter Spotlight</span>
            </a>
          </div>
        </div>

        {/* Quick Spec Metrics Grid */}
        <div className="mt-14 grid grid-cols-2 sm:grid-cols-4 gap-3 sm:gap-4 max-w-5xl mx-auto">
          <div className="p-4 rounded-xl bg-navy-900/70 border border-slate-800/80 backdrop-blur-sm text-center">
            <div className="text-2xl sm:text-3xl font-black text-medTeal-300 font-mono">5 cm ±1</div>
            <div className="text-xs text-slate-400 mt-1 font-medium">CPR Depth Tracking</div>
            <div className="text-[10px] text-slate-500 font-mono mt-0.5">CompressionDetector.cs</div>
          </div>

          <div className="p-4 rounded-xl bg-navy-900/70 border border-slate-800/80 backdrop-blur-sm text-center">
            <div className="text-2xl sm:text-3xl font-black text-medCyan-300 font-mono">100-120</div>
            <div className="text-xs text-slate-400 mt-1 font-medium">Target Compressions / Min</div>
            <div className="text-[10px] text-slate-500 font-mono mt-0.5">AHA / ERC Guidelines</div>
          </div>

          <div className="p-4 rounded-xl bg-navy-900/70 border border-slate-800/80 backdrop-blur-sm text-center">
            <div className="text-2xl sm:text-3xl font-black text-emerald-400 font-mono">5 Tiers</div>
            <div className="text-xs text-slate-400 mt-1 font-medium">Objective Telemetry Scoring</div>
            <div className="text-[10px] text-slate-500 font-mono mt-0.5">ScoringCalculator.cs</div>
          </div>

          <div className="p-4 rounded-xl bg-navy-900/70 border border-slate-800/80 backdrop-blur-sm text-center">
            <div className="text-2xl sm:text-3xl font-black text-amber-300 font-mono">Closed-Loop</div>
            <div className="text-xs text-slate-400 mt-1 font-medium">Defib Shock Interlocks</div>
            <div className="text-[10px] text-slate-500 font-mono mt-0.5">Safety Call Clear Gate</div>
          </div>
        </div>

      </div>
    </section>
  );
};
