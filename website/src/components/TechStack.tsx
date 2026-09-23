import React from 'react';
import { Cpu, Wrench, Layers, Terminal, Box, ShieldCheck } from 'lucide-react';

export const TechStack: React.FC = () => {
  const runtimeTech = [
    { name: 'Unity 6 Engine', desc: 'Core game engine with high-performance 64-bit multi-threading & XR runtime.' },
    { name: 'Universal Render Pipeline (URP 17.5)', desc: 'Optimized forward rendering pipeline for mobile VR and standalone PC.' },
    { name: 'XR Interaction Toolkit (XRI 3.5.1)', desc: 'Direct, ray, and socket interactors with event-driven interaction filtering.' },
    { name: 'Unity OpenXR Plugin (1.17.1)', desc: 'Cross-platform standard interface for Meta Quest 3, Quest Pro, and PCVR.' },
    { name: 'Unity XR Hands (1.8.0)', desc: 'Articulated hand tracking and gesture detection for natural CPR compressions.' },
    { name: 'Unity Cloud Save (3.4.1)', desc: 'Encrypted cloud persistence for learner profiles, histories, and telemetry.' },
    { name: 'C# .NET Standard Runtime', desc: 'Type-safe simulation state machines and deterministic scoring engines.' },
    { name: 'TextMesh Pro', desc: 'Subpixel anti-aliased dynamic world-space and screen-space clinical telemetry HUDs.' },
  ];

  const devTooling = [
    { name: 'Model Context Protocol (Unity MCP)', desc: 'AI Game Developer integration for autonomous editor tool execution during development.' },
    { name: 'Python Blender Pipeline', desc: 'Custom procedural asset generators (build_B1_defib.py through build_B9_ivpole_bvm.py).' },
    { name: 'XR Device Simulator', desc: 'Desktop testing rig enabling full interaction validation without physical headset tethering.' },
    { name: 'Git LFS Asset Tracking', desc: 'Large binary tracking for 3D FBX models, textures, animations, and soundscapes.' },
  ];

  return (
    <section id="technology" className="py-20 bg-slate-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Section Header */}
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medTeal-400 bg-medTeal-950/80 px-3 py-1 rounded-full border border-medTeal-500/30">
            Technical Architecture
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Built on Industry-Standard XR & Engine Tech
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            Distinguishing runtime simulation systems from specialized developer tooling.
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          
          {/* Runtime Technologies */}
          <div className="p-7 rounded-2xl bg-navy-900/60 border border-slate-800 space-y-6">
            <div className="flex items-center gap-3 pb-3 border-b border-slate-800">
              <div className="w-10 h-10 rounded-xl bg-medTeal-500/10 border border-medTeal-500/30 flex items-center justify-center text-medTeal-400">
                <Cpu className="w-5 h-5" />
              </div>
              <div>
                <h3 className="font-bold text-white text-lg">Runtime Simulation Stack</h3>
                <p className="text-xs text-slate-400">Targeting Meta Quest 3 & Standalone XR</p>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              {runtimeTech.map((tech, idx) => (
                <div key={idx} className="p-3.5 rounded-xl bg-slate-900/80 border border-slate-800/80">
                  <div className="font-semibold text-xs text-white">{tech.name}</div>
                  <p className="text-[11px] text-slate-400 mt-1 leading-relaxed">{tech.desc}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Development & Tooling */}
          <div className="p-7 rounded-2xl bg-navy-900/60 border border-slate-800 space-y-6">
            <div className="flex items-center gap-3 pb-3 border-b border-slate-800">
              <div className="w-10 h-10 rounded-xl bg-medCyan-500/10 border border-medCyan-500/30 flex items-center justify-center text-medCyan-400">
                <Wrench className="w-5 h-5" />
              </div>
              <div>
                <h3 className="font-bold text-white text-lg">Development & Asset Tooling</h3>
                <p className="text-xs text-slate-400">Internal Pipelines (Not Runtime Dependencies)</p>
              </div>
            </div>

            <div className="space-y-3">
              {devTooling.map((tool, idx) => (
                <div key={idx} className="p-4 rounded-xl bg-slate-900/80 border border-slate-800/80">
                  <div className="font-semibold text-xs text-medCyan-300 font-mono">{tool.name}</div>
                  <p className="text-xs text-slate-400 mt-1 leading-relaxed">{tool.desc}</p>
                </div>
              ))}
            </div>

            <div className="p-3.5 rounded-xl bg-slate-950/80 border border-slate-800 text-[11px] text-slate-400 leading-relaxed flex items-start gap-2">
              <ShieldCheck className="w-4 h-4 text-emerald-400 shrink-0 mt-0.5" />
              <span>
                <strong>Zero Client Key Storage:</strong> All LLM integrations connect strictly via backend proxies without hardcoded credentials in Unity client binaries.
              </span>
            </div>
          </div>

        </div>

      </div>
    </section>
  );
};
