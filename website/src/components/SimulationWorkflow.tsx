import React from 'react';
import { LogIn, Compass, Fingerprint, AlertOctagon, Heart, Zap, Award, RotateCcw } from 'lucide-react';

export const SimulationWorkflow: React.FC = () => {
  const steps = [
    {
      step: '01',
      icon: LogIn,
      title: 'Bootstrap & Authentication',
      scene: 'Bootstrap.unity -> Login.unity',
      desc: 'System initialization and secure learner login via Unity Cloud Save credentials.',
    },
    {
      step: '02',
      icon: Compass,
      title: 'Scenario Selection',
      scene: 'MainMenu.unity',
      desc: 'Learner selects canonical task cardiac_arrest_01 (In-Hospital Cardiac Arrest).',
    },
    {
      step: '03',
      icon: Fingerprint,
      title: 'Diagnostic Triage',
      scene: 'Cardiac_MVP.unity',
      desc: 'Learner docks PulseOximeter_Probe onto finger socket, activating monitor telemetry.',
    },
    {
      step: '04',
      icon: AlertOctagon,
      title: 'Clinical Deterioration',
      scene: 'PatientPresence.cs',
      desc: 'Vitals decline over 40s (HR 82 -> 150, SpO2 97 -> 78%) before Ventricular Fibrillation collapse.',
    },
    {
      step: '05',
      icon: Heart,
      title: 'Biomechanical Resuscitation',
      scene: 'CompressionDetector.cs',
      desc: 'Continuous CPR depth (5cm) and frequency (100-120 cpm) tracking with hands-off warning flags.',
    },
    {
      step: '06',
      icon: Zap,
      title: 'Closed-Loop Defibrillation',
      scene: 'DefibrillatorController.cs',
      desc: 'Pad placement, capacitor charging, and mandatory "All Clear" confirmation before shock.',
    },
    {
      step: '07',
      icon: Award,
      title: 'Automated Scoring & Debrief',
      scene: 'ScoringCalculator.cs',
      desc: 'Telemetry logs compute scores across 5 categories; optional AI coaching summary generated.',
    },
    {
      step: '08',
      icon: RotateCcw,
      title: 'Cloud Save & Progression',
      scene: 'CloudSaveManager.cs',
      desc: 'Learner results persist to cloud and player returns to MainMenu with updated scenario badge.',
    },
  ];

  return (
    <section id="workflow" className="py-20 bg-slate-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Section Header */}
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medTeal-400 bg-medTeal-950/80 px-3 py-1 rounded-full border border-medTeal-500/30">
            End-to-End Simulation Flow
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            The Learner Journey Through MedTriage
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            From initial authentication to clinical triage, emergency resuscitation, and automated debriefing.
          </p>
        </div>

        {/* Step Flow Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {steps.map((item, idx) => {
            const Icon = item.icon;
            return (
              <div
                key={idx}
                className="p-6 rounded-2xl bg-navy-900/50 border border-slate-800 hover:border-slate-700 transition-all hover:shadow-lg hover:shadow-black/50 relative flex flex-col justify-between"
              >
                <div>
                  <div className="flex items-center justify-between mb-4">
                    <span className="text-2xl font-black font-mono text-slate-600 group-hover:text-medTeal-400 transition-colors">
                      {item.step}
                    </span>
                    <div className="w-10 h-10 rounded-xl bg-slate-900 border border-slate-700 flex items-center justify-center text-medTeal-400">
                      <Icon className="w-5 h-5" />
                    </div>
                  </div>

                  <h3 className="text-base font-bold text-white mb-1.5">{item.title}</h3>
                  <div className="text-[11px] font-mono text-medCyan-300 bg-navy-950 px-2 py-0.5 rounded border border-slate-800 mb-3 inline-block">
                    {item.scene}
                  </div>
                  <p className="text-xs text-slate-400 leading-relaxed">{item.desc}</p>
                </div>

                <div className="mt-5 pt-3 border-t border-slate-800/60 flex items-center justify-between text-[10px] text-slate-400">
                  <span>Step {idx + 1} of 8</span>
                  <span className="text-emerald-400 font-medium">Validated</span>
                </div>
              </div>
            );
          })}
        </div>

      </div>
    </section>
  );
};
