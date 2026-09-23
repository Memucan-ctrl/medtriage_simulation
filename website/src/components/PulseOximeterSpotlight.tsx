import React, { useState } from 'react';
import { Activity, Fingerprint, Shield, Sparkles, Volume2, CheckCircle2, RotateCcw } from 'lucide-react';

export const PulseOximeterSpotlight: React.FC = () => {
  const [attached, setAttached] = useState(false);
  const [stage, setStage] = useState<'awake' | 'monitored' | 'deteriorating' | 'arrested'>('awake');
  const [heartRate, setHeartRate] = useState(82);
  const [spO2, setSpO2] = useState(97);

  const handleAttach = () => {
    setAttached(true);
    setStage('monitored');
    setHeartRate(82);
    setSpO2(97);

    setTimeout(() => {
      setStage('deteriorating');
      setHeartRate(142);
      setSpO2(82);
    }, 2500);

    setTimeout(() => {
      setStage('arrested');
      setHeartRate(0);
      setSpO2(0);
    }, 5500);
  };

  const handleReset = () => {
    setAttached(false);
    setStage('awake');
    setHeartRate(82);
    setSpO2(97);
  };

  return (
    <section id="oximeter" className="py-20 bg-slate-950 border-t border-slate-800 relative overflow-hidden">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medTeal-400 bg-medTeal-950/80 px-3 py-1 rounded-full border border-medTeal-500/30">
            Interaction Spotlight
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Pulse Oximeter Probe & Socket Mechanics
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            The diagnostic gateway in MedTriage. Attaching the probe is not merely an animation—it physically docks into an XR socket, initializes real-time vitals monitoring, and unlocks scenario progression.
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-center">
          <div className="lg:col-span-7 space-y-6">
            <div className="p-6 rounded-2xl bg-navy-900/60 border border-slate-800 space-y-4">
              <h3 className="text-xl font-bold text-white flex items-center gap-2">
                <Fingerprint className="w-5 h-5 text-medTeal-400" />
                <span>How the Pulse Oximeter Interaction Works</span>
              </h3>
              
              <p className="text-sm text-slate-300 leading-relaxed">
                In <code className="text-medTeal-300 font-mono text-xs bg-slate-900 px-1.5 py-0.5 rounded">PulseOximeterProbe.cs</code>, the patient's right index finger houses an <strong className="text-white">XRSocketInteractor</strong> labeled <code className="text-medCyan-300 font-mono text-xs bg-slate-900 px-1.5 py-0.5 rounded">Oximeter_FingerSocket</code>.
              </p>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 pt-2">
                <div className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-xs font-semibold text-white flex items-center gap-1.5 mb-1">
                    <Shield className="w-3.5 h-3.5 text-medTeal-400" />
                    <span>Strict Name Filtering</span>
                  </div>
                  <p className="text-[12px] text-slate-400">
                    Rejects all non-matching objects. Only <code className="text-medTeal-300 font-mono text-[11px]">PulseOximeter_Probe</code> triggers docking.
                  </p>
                </div>

                <div className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-xs font-semibold text-white flex items-center gap-1.5 mb-1">
                    <Volume2 className="w-3.5 h-3.5 text-medTeal-400" />
                    <span>3D Spatial Audio</span>
                  </div>
                  <p className="text-[12px] text-slate-400">
                    Plays <code className="text-slate-300 font-mono text-[11px]">AttachClip</code> at 0.6 volume upon successful finger snap.
                  </p>
                </div>

                <div className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-xs font-semibold text-white flex items-center gap-1.5 mb-1">
                    <Activity className="w-3.5 h-3.5 text-medTeal-400" />
                    <span>State Transition</span>
                  </div>
                  <p className="text-[12px] text-slate-400">
                    Invokes <code className="text-slate-300 font-mono text-[11px]">Presence.OnOximeterAttached()</code> to awaken the monitor display.
                  </p>
                </div>

                <div className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800">
                  <div className="text-xs font-semibold text-white flex items-center gap-1.5 mb-1">
                    <Sparkles className="w-3.5 h-3.5 text-medTeal-400" />
                    <span>Clinical Deterioration</span>
                  </div>
                  <p className="text-[12px] text-slate-400">
                    Begins a countdown before SpO2 drops and heart rate spikes, precipitating cardiac arrest.
                  </p>
                </div>
              </div>
            </div>

            <div className="p-4 rounded-xl bg-slate-950 border border-slate-800 font-mono text-xs overflow-x-auto">
              <div className="flex items-center justify-between pb-2 border-b border-slate-800 mb-2 text-slate-400 text-[11px]">
                <span>PulseOximeterProbe.cs (Verified Snippet)</span>
                <span className="text-emerald-400">XR Interaction Toolkit</span>
              </div>
              <pre className="text-slate-300 text-[11px] leading-relaxed">
{`void OnSelectEntered(SelectEnterEventArgs args) {
    if (!IsRequiredProbe(args.interactableObject)) return;
    if (m_Attached) return;
    m_Attached = true;
    m_Audio.PlayOneShot(AttachClip, AttachVolume);
    if (Presence != null) {
        Presence.OnOximeterAttached(); // Activates patient vitals!
    }
}`}
              </pre>
            </div>
          </div>

          <div className="lg:col-span-5">
            <div className="p-6 rounded-2xl bg-gradient-to-br from-navy-900 to-slate-900 border border-medTeal-500/30 shadow-2xl relative">
              <div className="flex items-center justify-between pb-4 border-b border-slate-800">
                <div className="flex items-center gap-2">
                  <span className="w-3 h-3 rounded-full bg-medTeal-400 animate-pulse"></span>
                  <h4 className="font-bold text-white text-sm">Interactive Mechanics Demo</h4>
                </div>
                <button
                  onClick={handleReset}
                  className="p-1.5 text-slate-400 hover:text-white rounded-md hover:bg-slate-800 transition-colors"
                  title="Reset Demo"
                >
                  <RotateCcw className="w-4 h-4" />
                </button>
              </div>

              <div className="my-6 p-6 rounded-xl bg-slate-950/80 border border-slate-800/80 text-center">
                <div className="w-16 h-16 mx-auto rounded-full bg-slate-900 border-2 border-dashed border-medTeal-400/50 flex items-center justify-center mb-3">
                  <Fingerprint className={`w-8 h-8 transition-colors ${attached ? 'text-medTeal-400' : 'text-slate-500'}`} />
                </div>
                <div className="text-sm font-semibold text-white">Patient Finger Socket</div>
                <div className="text-xs text-slate-400 mt-0.5 font-mono">
                  {attached ? 'Status: Probe Connected [LOCKED]' : 'Status: Socket Empty [AWAITING PROBE]'}
                </div>

                {!attached ? (
                  <button
                    onClick={handleAttach}
                    className="mt-4 px-5 py-2.5 rounded-lg bg-medTeal-500 hover:bg-medTeal-400 text-navy-950 font-bold text-xs shadow-md shadow-medTeal-500/20 transition-all cursor-pointer"
                  >
                    Dock Pulse Oximeter Probe
                  </button>
                ) : (
                  <div className="mt-4 inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-emerald-500/20 text-emerald-300 text-xs font-semibold border border-emerald-500/30">
                    <CheckCircle2 className="w-3.5 h-3.5" />
                    <span>Monitoring Signal Active</span>
                  </div>
                )}
              </div>

              <div className="grid grid-cols-2 gap-3 p-4 rounded-xl bg-navy-950 border border-slate-800/80">
                <div className="text-center p-3 rounded-lg bg-slate-900/60">
                  <div className="text-[10px] text-slate-400 uppercase font-semibold">Heart Rate (BPM)</div>
                  <div className={`text-2xl font-black font-mono mt-1 ${
                    stage === 'arrested' ? 'text-rose-500' : stage === 'deteriorating' ? 'text-amber-400 animate-pulse' : 'text-emerald-400'
                  }`}>
                    {attached ? (stage === 'arrested' ? '0 [VFIB]' : heartRate) : '--'}
                  </div>
                </div>

                <div className="text-center p-3 rounded-lg bg-slate-900/60">
                  <div className="text-[10px] text-slate-400 uppercase font-semibold">SpO2 Oxygen (%)</div>
                  <div className={`text-2xl font-black font-mono mt-1 ${
                    stage === 'arrested' ? 'text-rose-500' : stage === 'deteriorating' ? 'text-amber-400' : 'text-medCyan-400'
                  }`}>
                    {attached ? (stage === 'arrested' ? '0%' : `${spO2}%`) : '--%'}
                  </div>
                </div>
              </div>

              <div className="mt-4 p-3 rounded-lg bg-slate-900/90 border border-slate-800 text-xs flex items-center justify-between">
                <span className="text-slate-400">Patient State:</span>
                <span className={`font-mono font-bold uppercase ${
                  stage === 'awake' ? 'text-slate-400' :
                  stage === 'monitored' ? 'text-emerald-400' :
                  stage === 'deteriorating' ? 'text-amber-400' : 'text-rose-400'
                }`}>
                  {stage}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};
