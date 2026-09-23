import React, { useState } from 'react';
import { Activity, Heart, Zap } from 'lucide-react';

type RhythmState = 'sinus' | 'tachy' | 'vfib' | 'rosc';

export const VitalsSimulator: React.FC = () => {
  const [rhythm, setRhythm] = useState<RhythmState>('vfib');
  const [compressionDepth, setCompressionDepth] = useState<number>(5.2);
  const [compressionRate, setCompressionRate] = useState<number>(110);
  const [defibCharged, setDefibCharged] = useState<boolean>(false);
  const [clearCalled, setClearCalled] = useState<boolean>(false);
  const [shockMessage, setShockMessage] = useState<string>('');

  const rhythmData = {
    sinus: {
      name: 'Normal Sinus Rhythm',
      type: 'Normal / Stable',
      hr: 82,
      spo2: 97,
      etco2: 38,
      bp: '120/80',
      color: 'text-emerald-400',
      path: 'M 0 50 L 50 50 L 60 40 L 70 50 L 80 50 L 85 10 L 95 90 L 105 50 L 120 50 L 135 35 L 150 50 L 250 50 L 260 40 L 270 50 L 280 50 L 285 10 L 295 90 L 305 50 L 320 50 L 335 35 L 350 50 L 450 50',
    },
    tachy: {
      name: 'Sinus Tachycardia (Deteriorating)',
      type: 'Acute Deterioration',
      hr: 150,
      spo2: 78,
      etco2: 24,
      bp: '85/50',
      color: 'text-amber-400',
      path: 'M 0 50 L 30 50 L 38 35 L 45 50 L 52 50 L 56 12 L 64 88 L 72 50 L 80 50 L 90 38 L 100 50 L 160 50 L 168 35 L 175 50 L 182 50 L 186 12 L 194 88 L 202 50 L 210 50 L 220 38 L 230 50 L 290 50 L 298 35 L 305 50 L 312 50 L 316 12 L 324 88 L 332 50 L 340 50 L 350 38 L 360 50 L 450 50',
    },
    vfib: {
      name: 'Ventricular Fibrillation (VFib)',
      type: 'Cardiac Arrest [Shockable]',
      hr: 0,
      spo2: 0,
      etco2: 12,
      bp: '--/--',
      color: 'text-rose-400',
      path: 'M 0 50 Q 20 15 40 50 T 80 80 T 120 20 T 160 75 T 200 25 T 240 85 T 280 15 T 320 70 T 360 30 T 400 65 T 450 50',
    },
    rosc: {
      name: 'ROSC (Post-Defibrillation Recovery)',
      type: 'Return of Circulation',
      hr: 96,
      spo2: 92,
      etco2: 35,
      bp: '110/70',
      color: 'text-medTeal-400',
      path: 'M 0 50 L 40 50 L 50 42 L 60 50 L 70 50 L 75 15 L 85 85 L 95 50 L 110 50 L 125 38 L 140 50 L 230 50 L 240 42 L 250 50 L 260 50 L 265 15 L 275 85 L 285 50 L 300 50 L 315 38 L 330 50 L 450 50',
    },
  };

  const current = rhythmData[rhythm];

  const depthError = Math.abs(compressionDepth - 5.0);
  const depthQuality = Math.max(0, 1 - Math.max(0, depthError - 1.0) / 5.0);
  const rateQuality = compressionRate >= 100 && compressionRate <= 120 
    ? 1.0 
    : compressionRate < 100 
      ? compressionRate / 100 
      : Math.max(0, 1 - (compressionRate - 120) / 40);
  const strokeQuality = Math.round(((depthQuality + rateQuality) / 2) * 100);

  const handleShock = () => {
    if (!defibCharged) {
      setShockMessage('Defibrillator is not charged. Charge before attempting shock.');
      return;
    }
    if (!clearCalled) {
      setShockMessage('SAFETY INTERLOCK TRIGGERED: "All Clear" was not called. Shock rejected (Critical Error: no_clear_call).');
      return;
    }
    if (rhythm !== 'vfib') {
      setShockMessage('CRITICAL ERROR: Shock delivered on non-shockable rhythm (shock_non_shockable)!');
      return;
    }
    setRhythm('rosc');
    setDefibCharged(false);
    setClearCalled(false);
    setShockMessage('SUCCESS: Shock delivered safely. Patient achieved Return of Spontaneous Circulation (ROSC)!');
  };

  return (
    <section id="vitals-sim" className="py-20 bg-navy-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medCyan-400 bg-medCyan-950/80 px-3 py-1 rounded-full border border-medCyan-500/30">
            Interactive Telemetry Simulator
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Bedside Vitals & Defibrillator Safety Lab
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            Test the telemetry and interaction logic implemented in <code className="text-medTeal-300 font-mono text-xs">PatientPresence.cs</code>, <code className="text-medTeal-300 font-mono text-xs">CompressionDetector.cs</code>, and <code className="text-medTeal-300 font-mono text-xs">DefibrillatorController.cs</code>.
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <div className="lg:col-span-7 bg-slate-950 rounded-2xl border-2 border-slate-800 p-6 shadow-2xl relative overflow-hidden flex flex-col justify-between">
            <div>
              <div className="flex items-center justify-between pb-4 border-b border-slate-800/80">
                <div className="flex items-center gap-3">
                  <div className="w-3 h-3 rounded-full bg-emerald-400 animate-ping"></div>
                  <div>
                    <span className="text-xs text-slate-400 uppercase font-mono">Bedside Monitor // Lead II</span>
                    <h3 className={`text-base font-bold font-mono ${current.color}`}>{current.name}</h3>
                  </div>
                </div>
                <span className="text-xs font-mono px-2.5 py-1 rounded bg-slate-900 border border-slate-800 text-slate-300">
                  {current.type}
                </span>
              </div>

              <div className="my-6 h-36 bg-slate-900/50 rounded-xl border border-slate-800/80 relative overflow-hidden flex items-center">
                <div className="absolute inset-0 medical-grid opacity-30 pointer-events-none"></div>
                <svg className="w-full h-28" viewBox="0 0 450 100" preserveAspectRatio="none">
                  <path
                    d={current.path}
                    fill="none"
                    stroke={rhythm === 'vfib' ? '#f87171' : rhythm === 'tachy' ? '#fbbf24' : '#34d399'}
                    strokeWidth="2.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    className="animate-ecg-path"
                  />
                </svg>
                <div className="absolute top-2 right-3 text-[11px] font-mono text-slate-500">25mm/s 10mm/mV</div>
              </div>

              <div className="grid grid-cols-4 gap-3 text-center">
                <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-[10px] text-slate-400 uppercase font-mono">HR (bpm)</div>
                  <div className={`text-2xl font-black font-mono mt-1 ${current.color}`}>
                    {current.hr > 0 ? current.hr : '0'}
                  </div>
                </div>

                <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-[10px] text-slate-400 uppercase font-mono">SpO2 (%)</div>
                  <div className="text-2xl font-black font-mono text-medCyan-400 mt-1">
                    {current.spo2 > 0 ? `${current.spo2}%` : '--%'}
                  </div>
                </div>

                <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-[10px] text-slate-400 uppercase font-mono">EtCO2 (mmHg)</div>
                  <div className="text-2xl font-black font-mono text-amber-400 mt-1">
                    {current.etco2}
                  </div>
                </div>

                <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-[10px] text-slate-400 uppercase font-mono">NIBP</div>
                  <div className="text-sm font-black font-mono text-slate-200 mt-2">
                    {current.bp}
                  </div>
                </div>
              </div>
            </div>

            <div className="mt-6 pt-4 border-t border-slate-800/80 flex flex-wrap gap-2 items-center justify-between">
              <span className="text-xs text-slate-400">Select Clinical Rhythm:</span>
              <div className="flex flex-wrap gap-1.5">
                <button
                  onClick={() => setRhythm('sinus')}
                  className={`px-2.5 py-1 text-xs font-mono rounded cursor-pointer ${
                    rhythm === 'sinus' ? 'bg-emerald-500 text-navy-950 font-bold' : 'bg-slate-900 text-slate-300 hover:bg-slate-800'
                  }`}
                >
                  Normal Sinus
                </button>
                <button
                  onClick={() => setRhythm('tachy')}
                  className={`px-2.5 py-1 text-xs font-mono rounded cursor-pointer ${
                    rhythm === 'tachy' ? 'bg-amber-500 text-navy-950 font-bold' : 'bg-slate-900 text-slate-300 hover:bg-slate-800'
                  }`}
                >
                  Tachycardia
                </button>
                <button
                  onClick={() => setRhythm('vfib')}
                  className={`px-2.5 py-1 text-xs font-mono rounded cursor-pointer ${
                    rhythm === 'vfib' ? 'bg-rose-500 text-white font-bold' : 'bg-slate-900 text-slate-300 hover:bg-slate-800'
                  }`}
                >
                  VFib Arrest
                </button>
                <button
                  onClick={() => setRhythm('rosc')}
                  className={`px-2.5 py-1 text-xs font-mono rounded cursor-pointer ${
                    rhythm === 'rosc' ? 'bg-medTeal-500 text-navy-950 font-bold' : 'bg-slate-900 text-slate-300 hover:bg-slate-800'
                  }`}
                >
                  Post-Shock ROSC
                </button>
              </div>
            </div>
          </div>

          <div className="lg:col-span-5 space-y-6 flex flex-col justify-between">
            <div className="p-6 rounded-2xl bg-navy-900/60 border border-slate-800 space-y-4">
              <div className="flex items-center justify-between">
                <h4 className="font-bold text-white text-sm flex items-center gap-2">
                  <Heart className="w-4 h-4 text-rose-400" />
                  <span>CPR Biomechanics Telemetry</span>
                </h4>
                <span className="text-xs font-mono font-bold text-emerald-400 bg-emerald-950/80 px-2 py-0.5 rounded border border-emerald-500/30">
                  Quality: {strokeQuality}%
                </span>
              </div>

              <div className="space-y-1.5">
                <div className="flex justify-between text-xs text-slate-300">
                  <span>Compression Depth:</span>
                  <span className="font-mono font-bold text-medTeal-300">{compressionDepth.toFixed(1)} cm (Target: 5.0 cm)</span>
                </div>
                <input
                  type="range"
                  min="2.0"
                  max="7.0"
                  step="0.1"
                  value={compressionDepth}
                  onChange={(e) => setCompressionDepth(parseFloat(e.target.value))}
                  className="w-full accent-medTeal-400 cursor-pointer"
                />
              </div>

              <div className="space-y-1.5">
                <div className="flex justify-between text-xs text-slate-300">
                  <span>Compression Cadence:</span>
                  <span className="font-mono font-bold text-medCyan-300">{compressionRate} cpm (Target: 100-120)</span>
                </div>
                <input
                  type="range"
                  min="70"
                  max="160"
                  step="1"
                  value={compressionRate}
                  onChange={(e) => setCompressionRate(parseInt(e.target.value))}
                  className="w-full accent-medCyan-400 cursor-pointer"
                />
              </div>
            </div>

            <div className="p-6 rounded-2xl bg-gradient-to-br from-navy-900 to-slate-900 border border-amber-500/30 space-y-4">
              <div className="flex items-center justify-between">
                <h4 className="font-bold text-white text-sm flex items-center gap-2">
                  <Zap className="w-4 h-4 text-amber-400" />
                  <span>Defibrillator Safety Interlocks</span>
                </h4>
                <span className={`text-[11px] font-mono px-2 py-0.5 rounded ${
                  defibCharged ? 'bg-amber-500/20 text-amber-300 border border-amber-500/40' : 'bg-slate-800 text-slate-400'
                }`}>
                  {defibCharged ? '200J CHARGED' : 'DISARMED'}
                </span>
              </div>

              <div className="grid grid-cols-2 gap-2">
                <button
                  onClick={() => setDefibCharged(!defibCharged)}
                  className={`px-3 py-2 rounded-lg text-xs font-semibold border transition-all cursor-pointer ${
                    defibCharged ? 'bg-amber-500 text-navy-950 border-amber-400 font-bold' : 'bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700'
                  }`}
                >
                  {defibCharged ? '1. Capacitor Charged' : '1. Charge Defibrillator'}
                </button>

                <button
                  onClick={() => setClearCalled(!clearCalled)}
                  className={`px-3 py-2 rounded-lg text-xs font-semibold border transition-all cursor-pointer ${
                    clearCalled ? 'bg-emerald-500 text-navy-950 border-emerald-400 font-bold' : 'bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700'
                  }`}
                >
                  {clearCalled ? '2. "All Clear" Confirmed' : '2. Call "All Clear"'}
                </button>
              </div>

              <button
                onClick={handleShock}
                className="w-full py-3 rounded-xl bg-gradient-to-r from-rose-600 to-rose-500 hover:from-rose-500 hover:to-rose-400 text-white font-extrabold text-sm shadow-lg shadow-rose-600/30 transition-all flex items-center justify-center gap-2 cursor-pointer"
              >
                <Zap className="w-4 h-4 fill-white" />
                <span>Deliver Defibrillation Shock (200J)</span>
              </button>

              {shockMessage && (
                <div className="p-3 rounded-lg bg-slate-950 border border-slate-800 text-xs leading-relaxed text-slate-300">
                  {shockMessage}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};
