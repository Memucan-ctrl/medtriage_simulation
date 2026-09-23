import React from 'react';
import { ShieldAlert, Sparkles, Scale } from 'lucide-react';

export const ScoringMatrix: React.FC = () => {
  const categories = [
    {
      name: 'Protocol Adherence',
      weight: '30%',
      color: 'bg-emerald-500',
      textColor: 'text-emerald-400',
      desc: 'Execution of standard ACLS resuscitation algorithms, shockable rhythm recognition, and CPR cycle timing.',
    },
    {
      name: 'Efficiency',
      weight: '25%',
      color: 'bg-medTeal-500',
      textColor: 'text-medTeal-300',
      desc: 'Minimization of pre-shock pauses and hands-off chest compression interruptions under 10 seconds.',
    },
    {
      name: 'Technical Execution',
      weight: '20%',
      color: 'bg-medCyan-500',
      textColor: 'text-medCyan-300',
      desc: 'Physical accuracy: 5cm compression depth, 100-120 cpm cadence, and correct pad placement.',
    },
    {
      name: 'Team Communication',
      weight: '15%',
      color: 'bg-blue-500',
      textColor: 'text-blue-300',
      desc: 'Clear delegation to AI nursing teammates (AttachMonitor, GiveMedication, GetIvAccess, DocumentTime).',
    },
    {
      name: 'Decision-Making',
      weight: '10%',
      color: 'bg-purple-500',
      textColor: 'text-purple-300',
      desc: 'Accurate clinical classification of ECG waveforms and identification of reversible causes (Hs & Ts).',
    },
  ];

  const criticalErrors = [
    { id: 'no_compressions_30s', name: 'No Compressions for >= 30s', penalty: 'Safety Flag' },
    { id: 'shock_non_shockable', name: 'Shock Delivered on Asystole / PEA', penalty: 'Immediate Fail' },
    { id: 'no_clear_call', name: 'Defibrillator Discharged without "All Clear"', penalty: 'Critical Flag' },
    { id: 'premature_termination', name: 'Session Terminated Prior to Resuscitation', penalty: 'Incomplete' },
  ];

  return (
    <section id="scoring" className="py-20 bg-navy-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-emerald-400 bg-emerald-950/80 px-3 py-1 rounded-full border border-emerald-500/30">
            Assessment & Debriefing Matrix
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Deterministic Competency Telemetry
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            MedTriage utilizes an evidence-backed deterministic mathematical scoring model in <code className="text-medTeal-300 font-mono text-xs">ScoringCalculator.cs</code>. Scores are calculated strictly from event logs—never hallucinated by AI.
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <div className="lg:col-span-7 p-6 rounded-2xl bg-navy-900/60 border border-slate-800 space-y-5">
            <div className="flex items-center justify-between pb-3 border-b border-slate-800">
              <h3 className="font-bold text-white text-base flex items-center gap-2">
                <Scale className="w-5 h-5 text-medTeal-400" />
                <span>Standard Category Weighting Matrix</span>
              </h3>
              <span className="text-xs font-mono text-slate-400">Total: 100% Normalized</span>
            </div>

            <div className="space-y-4">
              {categories.map((cat, idx) => (
                <div key={idx} className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800/80">
                  <div className="flex items-center justify-between mb-1.5">
                    <span className="font-semibold text-sm text-white">{cat.name}</span>
                    <span className={`font-mono font-bold text-sm ${cat.textColor}`}>{cat.weight}</span>
                  </div>
                  <div className="w-full h-2 rounded-full bg-slate-950 overflow-hidden mb-2">
                    <div className={`h-full ${cat.color} rounded-full`} style={{ width: cat.weight }}></div>
                  </div>
                  <p className="text-xs text-slate-400">{cat.desc}</p>
                </div>
              ))}
            </div>

            <div className="p-3 rounded-lg bg-slate-950/80 border border-slate-800 text-[11px] text-slate-400 leading-relaxed">
              <strong className="text-slate-200">Dynamic Weight Normalization:</strong> If a category is not triggered during a session (e.g., zero team delegation commands), it is omitted from calculation rather than falsely awarding 100% or 0%.
            </div>
          </div>

          <div className="lg:col-span-5 space-y-6">
            <div className="p-6 rounded-2xl bg-gradient-to-br from-navy-900 to-slate-900 border border-rose-500/30 space-y-4">
              <h3 className="font-bold text-white text-base flex items-center gap-2">
                <ShieldAlert className="w-5 h-5 text-rose-400" />
                <span>Critical Safety Error Catalog</span>
              </h3>
              <p className="text-xs text-slate-300 leading-relaxed">
                Critical errors override percentage scores with mandatory safety review flags to guarantee patient safety vigilance.
              </p>

              <div className="space-y-2.5">
                {criticalErrors.map((err, idx) => (
                  <div key={idx} className="p-3 rounded-xl bg-slate-950/90 border border-slate-800 flex items-center justify-between">
                    <div>
                      <div className="text-xs font-semibold text-slate-200">{err.name}</div>
                      <div className="text-[10px] font-mono text-slate-500">{err.id}</div>
                    </div>
                    <span className="text-[11px] font-mono font-bold text-rose-400 bg-rose-950/80 px-2 py-0.5 rounded border border-rose-500/30">
                      {err.penalty}
                    </span>
                  </div>
                ))}
              </div>
            </div>

            <div className="p-6 rounded-2xl bg-navy-900/60 border border-medTeal-500/30 space-y-3">
              <h4 className="font-bold text-white text-sm flex items-center gap-2">
                <Sparkles className="w-4 h-4 text-medTeal-400" />
                <span>Secure AI Debrief Coaching (LLM Proxy)</span>
              </h4>
              <p className="text-xs text-slate-300 leading-relaxed">
                In <code className="text-medTeal-300 font-mono text-[11px]">DebriefCoachingService.cs</code>, session telemetry is dispatched to a secure external backend proxy for narrative strengths & improvements summaries.
              </p>
              <div className="p-2.5 rounded bg-slate-950 border border-slate-800 text-[11px] text-emerald-400 font-mono">
                ✓ Zero hardcoded API keys in client builds<br/>
                ✓ Instant local rule-based fallback if proxy offline
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};
