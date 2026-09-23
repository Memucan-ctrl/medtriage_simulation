import React from 'react';
import { Github, Users, Code2 } from 'lucide-react';

export const TeamSection: React.FC = () => {
  const contributors = [
    {
      name: 'Memucan-ctrl',
      role: 'Project Lead & Repository Architect',
      github: 'https://github.com/Memucan-ctrl',
      contributions: 'Simulation scene architecture, Cardiac MVP mechanics, Git LFS integration, and core XR telemetry.',
    },
    {
      name: 'Josephat Onkoba',
      role: 'Simulation & Interaction Contributor',
      github: 'https://github.com/Josephat-github',
      contributions: 'Pulse oximeter interactions, patient presence state machine, and defibrillator safety interlocks.',
    },
    {
      name: 'Samuel Kangethe',
      role: 'Integration Contributor',
      github: 'https://github.com/Vexx-bit',
      contributions: 'Frontend authentication, scenario definitions, Cloud Save, and scoring engine integration.',
    },
    {
      name: 'tmlnaliaka',
      role: 'Project Contributor',
      github: 'https://github.com/tmlnaliaka',
      contributions: 'Contributor to the MedTriage XR healthcare simulation project.',
    },
    {
      name: 'gracekamure754-spec',
      role: 'Project Contributor',
      github: 'https://github.com/gracekamure754-spec',
      contributions: 'Contributor to the MedTriage XR healthcare simulation project.',
    },
  ];

  return (
    <section id="team" className="py-20 bg-navy-950 border-t border-slate-800 relative">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Section Header */}
        <div className="max-w-3xl mx-auto text-center mb-16">
          <span className="text-xs uppercase tracking-widest font-semibold text-medCyan-400 bg-medCyan-950/80 px-3 py-1 rounded-full border border-medCyan-500/30">
            Contributors & Open Collaboration
          </span>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-white mt-4 tracking-tight">
            Repository Contributors & Development Team
          </h2>
          <p className="mt-4 text-base sm:text-lg text-slate-300 leading-relaxed">
            MedTriage is an open healthcare simulation initiative developed with clean Git history and modular Unity architecture.
          </p>
        </div>

        {/* Contributors Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 max-w-5xl mx-auto">
          {contributors.map((member) => (
            <div
              key={member.github}
              className="p-6 rounded-2xl bg-navy-900/60 border border-slate-800 hover:border-slate-700 transition-all flex flex-col justify-between group shadow-lg"
            >
              <div>
                <div className="w-12 h-12 rounded-xl bg-slate-900 border border-slate-700 flex items-center justify-center text-medTeal-400 mb-4 group-hover:scale-105 transition-transform">
                  <Users className="w-6 h-6" />
                </div>

                <h3 className="text-lg font-bold text-white mb-1 group-hover:text-medTeal-300 transition-colors">
                  {member.name}
                </h3>
                <div className="text-xs text-medCyan-400 font-semibold mb-3">{member.role}</div>
                <p className="text-xs text-slate-300 leading-relaxed">{member.contributions}</p>
              </div>

              <div className="mt-6 pt-4 border-t border-slate-800">
                <a
                  href={member.github}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="inline-flex items-center gap-2 text-xs font-semibold text-slate-300 hover:text-medTeal-300 transition-colors"
                >
                  <Github className="w-4 h-4 text-slate-400" />
                  <span>GitHub Profile</span>
                </a>
              </div>
            </div>
          ))}
        </div>

        {/* GitHub Call to Action Banner */}
        <div className="mt-16 p-8 rounded-2xl bg-gradient-to-r from-navy-900 via-slate-900 to-navy-900 border border-slate-700 text-center max-w-4xl mx-auto shadow-2xl">
          <Code2 className="w-10 h-10 text-medTeal-400 mx-auto mb-3" />
          <h3 className="text-2xl font-bold text-white">Explore the Source Code on GitHub</h3>
          <p className="text-sm text-slate-300 mt-2 max-w-2xl mx-auto">
            Inspect the complete Unity project, C# telemetry systems, scene configurations, and documentation.
          </p>
          <div className="mt-6 flex flex-wrap justify-center gap-4">
            <a
              href="https://github.com/Memucan-ctrl/medtriage_simulation"
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 px-6 py-3 rounded-xl bg-medTeal-500 hover:bg-medTeal-400 text-navy-950 font-bold text-sm shadow-lg shadow-medTeal-500/20 transition-all hover:scale-105"
            >
              <Github className="w-4 h-4" />
              <span>Memucan-ctrl / medtriage_simulation</span>
            </a>
          </div>
        </div>

      </div>
    </section>
  );
};
