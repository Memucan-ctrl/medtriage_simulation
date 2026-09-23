import React from 'react';
import { Activity, ShieldAlert, Github, Heart } from 'lucide-react';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-slate-950 border-t border-slate-800 text-slate-400 text-xs">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-14">
        
        {/* Main Footer Row */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8 mb-12">
          
          {/* Col 1: Brand & Identity */}
          <div className="md:col-span-2 space-y-4">
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 rounded-lg bg-medTeal-500/20 border border-medTeal-500/40 flex items-center justify-center text-medTeal-400">
                <Activity className="w-4 h-4" />
              </div>
              <span className="font-bold text-lg text-white tracking-tight">MedTriage Simulator</span>
            </div>
            <p className="text-slate-400 text-xs leading-relaxed max-w-sm">
              An evidence-based Virtual Reality resuscitation and triage training laboratory engineered in Unity 6, OpenXR, and Universal Render Pipeline.
            </p>
            <div className="flex items-center gap-3 pt-2">
              <a
                href="https://github.com/Memucan-ctrl/medtriage_simulation"
                target="_blank"
                rel="noopener noreferrer"
                className="p-2 rounded-lg bg-slate-900 hover:bg-slate-800 text-slate-300 hover:text-white border border-slate-800 transition-colors"
                aria-label="GitHub Repository"
              >
                <Github className="w-4 h-4" />
              </a>
            </div>
          </div>

          {/* Col 2: Navigation */}
          <div>
            <h4 className="font-bold text-white uppercase tracking-wider text-[11px] mb-3">Simulation Systems</h4>
            <ul className="space-y-2">
              <li><a href="#overview" className="hover:text-medTeal-300 transition-colors">Project Overview</a></li>
              <li><a href="#features" className="hover:text-medTeal-300 transition-colors">Biomechanical CPR</a></li>
              <li><a href="#oximeter" className="hover:text-medTeal-300 transition-colors">Pulse Oximeter Spotlight</a></li>
              <li><a href="#vitals-sim" className="hover:text-medTeal-300 transition-colors">Telemetry Simulator</a></li>
              <li><a href="#scoring" className="hover:text-medTeal-300 transition-colors">Scoring Engine</a></li>
            </ul>
          </div>

          {/* Col 3: Specifications */}
          <div>
            <h4 className="font-bold text-white uppercase tracking-wider text-[11px] mb-3">Technical Specifications</h4>
            <ul className="space-y-2 font-mono text-[11px]">
              <li>Engine: <span className="text-slate-300">Unity 6 (6000.0)</span></li>
              <li>Pipeline: <span className="text-slate-300">URP v17.5.0</span></li>
              <li>Standard: <span className="text-slate-300">OpenXR 1.17.1</span></li>
              <li>Toolkit: <span className="text-slate-300">XRI v3.5.1</span></li>
              <li>Task ID: <span className="text-medTeal-300">cardiac_arrest_01</span></li>
            </ul>
          </div>

        </div>

        {/* Clinical & Educational Disclaimer */}
        <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800/80 mb-8 flex items-start gap-3">
          <ShieldAlert className="w-5 h-5 text-amber-400 shrink-0 mt-0.5" />
          <div className="text-[11px] leading-relaxed text-slate-300">
            <strong className="text-amber-300 font-semibold">Clinical & Educational Disclaimer:</strong> MedTriage is designed solely as an educational and procedural training simulation platform for healthcare learners. It does not provide medical advice, diagnosis, or treatment recommendations, and must not replace formal clinical education or professional medical judgment. Not evaluated or certified as a medical device by regulatory agencies.
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="pt-6 border-t border-slate-800/80 flex flex-col sm:flex-row items-center justify-between gap-4 text-[11px] text-slate-400">
          <div>
            © {new Date().getFullYear()} MedTriage Simulation Project. All verified rights reserved.
          </div>
          <div className="flex items-center gap-4">
            <a href="https://github.com/Memucan-ctrl/medtriage_simulation" target="_blank" rel="noopener noreferrer" className="hover:text-white transition-colors">
              GitHub Repository
            </a>
            <span>•</span>
            <a href="#overview" className="hover:text-white transition-colors">
              Back to Top
            </a>
          </div>
        </div>

      </div>
    </footer>
  );
};
