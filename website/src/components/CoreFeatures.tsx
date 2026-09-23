import React from 'react';
import { Heart, Zap, UserCheck, Users, BarChart3, Cloud, CheckCircle, Code } from 'lucide-react';

export const CoreFeatures: React.FC = () => {
  const features = [
    {
      icon: Heart,
      color: 'text-rose-400',
      bg: 'bg-rose-500/10 border-rose-500/20',
      title: 'Biomechanical CPR Engine',
      script: 'CompressionDetector.cs',
      description: 'Continuous spatial depth and rate calculation. Scores compressions against the clinical 5 cm (0.05m ±1cm) depth standard and 100–120 cpm rate.',
      details: [
        'Hands-on-chest engagement detection via vector projection',
        'Real-time stroke quality calculation combining depth + rate',
        'Automatic warning for hands-off interruptions >= 10s',
        'Critical safety violation triggered if pause exceeds 30s'
      ]
    },
    {
      icon: Zap,
      color: 'text-amber-400',
      bg: 'bg-amber-500/10 border-amber-500/20',
      title: 'Closed-Loop Defibrillation',
      script: 'DefibrillatorController.cs',
      description: 'Realistic defibrillator device with authentic audio whining capacitor charge, sternum/apex pad placement checks, and safety interlocks.',
      details: [
        'Hardware shock button rejects shock if "Clear" is not called',
        'Forced rhythm classification check (Shockable vs Non-Shockable)',
        'Immediate critical error flag if non-shockable rhythm is shocked',
        'Multi-pad contact zone and socket arming validation'
      ]
    },
    {
      icon: UserCheck,
      color: 'text-medTeal-400',
      bg: 'bg-medTeal-500/10 border-medTeal-500/20',
      title: 'Dynamic Patient Physiology',
      script: 'PatientPresence.cs',
      description: 'State machine governing patient consciousness and vitals deterioration through 5 distinct clinical stages.',
      details: [
        'Interactive head gaze tracking (42° max turn) toward learner',
        '5 states: Awake, Monitored, Deteriorating, Arrested, ROSC',
        'Interpolated vitals: Heart Rate (82 -> 150 bpm), SpO2 (97 -> 78%)',
        'Spontaneous vs non-breathing chest motion kinematics'
      ]
    },
    {
      icon: Users,
      color: 'text-medCyan-400',
      bg: 'bg-medCyan-500/10 border-medCyan-500/20',
      title: 'AI Teammate Task Delegation',
      script: 'AITeammate.cs',
      description: 'Deterministic 4-state state machine modeling nursing and resuscitation team communication and delegation.',
      details: [
        '4 clinical tasks: AttachMonitor, GiveMedication, GetIvAccess, DocumentTime',
        'States: Idle -> Called -> Performing -> Done with voice cues',
        'Radial UI assignment for rapid hands-busy command execution',
        'Logged directly to TaskManager for team communication scoring'
      ]
    },
    {
      icon: BarChart3,
      color: 'text-emerald-400',
      bg: 'bg-emerald-500/10 border-emerald-500/20',
      title: 'Deterministic Scoring Engine',
      script: 'ScoringCalculator.cs',
      description: 'Mathematical, evidence-based performance scoring derived strictly from telemetry event logs rather than LLM grading.',
      details: [
        '5 weighted categories: Protocol (30%), Efficiency (25%), Technical (20%), Team (15%), Decision (10%)',
        'Dynamic weight normalization across actually assessed categories',
        'Zero artificial default 100s for omitted procedures',
        'Critical error gates with immediate safety flags'
      ]
    },
    {
      icon: Cloud,
      color: 'text-blue-400',
      bg: 'bg-blue-500/10 border-blue-500/20',
      title: 'Cloud Learner Profiles',
      script: 'CloudSaveManager.cs',
      description: 'Secure cloud session management storing user authentication, completed task histories, and debrief logs.',
      details: [
        'Unity Services Cloud Save integration (com.unity.services.cloudsave)',
        'Unique learner credentials & progress persistence',
        'Scenario completion badges and historic score comparisons',
        'Seamless transition between Bootstrap and Simulation scenes'
      ]
    },
  ];

  return (
    <section id="features" className="py-20 bg-navy-950 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medCyan-400 bg-medCyan-950/80 px-3 py-1 rounded-full border border-medCyan-500/30">
            Clinical Systems
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Verified Architectural Systems
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            Every feature in MedTriage is backed by verifiable C# source code and Unity engine implementations.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {features.map((feat, idx) => {
            const Icon = feat.icon;
            return (
              <div
                key={idx}
                className="p-6 rounded-2xl bg-navy-900/50 border border-slate-800 hover:border-slate-700 transition-all hover:shadow-xl hover:shadow-black/50 flex flex-col justify-between group"
              >
                <div>
                  <div className="flex items-center justify-between mb-4">
                    <div className={`w-12 h-12 rounded-xl flex items-center justify-center border ${feat.bg}`}>
                      <Icon className={`w-6 h-6 ${feat.color}`} />
                    </div>
                    <span className="text-[11px] font-mono text-slate-400 bg-slate-900 px-2.5 py-1 rounded border border-slate-800 flex items-center gap-1">
                      <Code className="w-3 h-3 text-slate-500" />
                      {feat.script}
                    </span>
                  </div>

                  <h3 className="text-lg font-bold text-white mb-2 group-hover:text-medTeal-300 transition-colors">
                    {feat.title}
                  </h3>
                  <p className="text-sm text-slate-300 mb-5 leading-relaxed">
                    {feat.description}
                  </p>

                  <div className="space-y-2 border-t border-slate-800/80 pt-4">
                    {feat.details.map((detail, dIdx) => (
                      <div key={dIdx} className="flex items-start gap-2 text-xs text-slate-400">
                        <CheckCircle className="w-3.5 h-3.5 text-medTeal-400 shrink-0 mt-0.5" />
                        <span>{detail}</span>
                      </div>
                    ))}
                  </div>
                </div>

                <div className="mt-6 pt-3 border-t border-slate-800/50 flex items-center justify-between text-[11px] text-slate-400">
                  <span className="text-emerald-400 font-semibold flex items-center gap-1">
                    <span className="w-1.5 h-1.5 rounded-full bg-emerald-400"></span> Verified in Repo
                  </span>
                  <span>ACLS Aligned</span>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
};
