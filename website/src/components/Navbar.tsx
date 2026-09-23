import React, { useState, useEffect } from 'react';
import { Activity, Github, Menu, X } from 'lucide-react';

export const Navbar: React.FC = () => {
  const [scrolled, setScrolled] = useState(false);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => setScrolled(window.scrollY > 20);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const navLinks = [
    { name: 'Overview', href: '#overview' },
    { name: 'Core Systems', href: '#features' },
    { name: 'Pulse Oximeter', href: '#oximeter' },
    { name: 'Workflow', href: '#workflow' },
    { name: 'Monitor Sim', href: '#vitals-sim' },
    { name: 'Scoring Engine', href: '#scoring' },
    { name: 'Technology', href: '#technology' },
    { name: 'Roadmap', href: '#roadmap' },
    { name: 'Team', href: '#team' },
  ];

  const headerClass = scrolled 
    ? 'bg-navy-950/90 backdrop-blur-md border-b border-slate-800/80 shadow-lg shadow-black/40' 
    : 'bg-transparent';

  return (
    <header className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${headerClass}`}>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-20">
          
          <a href="#" className="flex items-center gap-3 group focus:outline-none focus:ring-2 focus:ring-medTeal-400 rounded-lg p-1">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-medTeal-400 to-medCyan-500 p-0.5 shadow-md shadow-medTeal-500/20 group-hover:shadow-medTeal-500/40 transition-all">
              <div className="w-full h-full bg-navy-950 rounded-[10px] flex items-center justify-center">
                <Activity className="w-5 h-5 text-medTeal-400 group-hover:scale-110 transition-transform" />
              </div>
            </div>
            <div className="flex flex-col">
              <div className="flex items-center gap-1.5">
                <span className="font-bold text-xl tracking-tight text-white">MedTriage</span>
                <span className="text-[10px] uppercase tracking-wider font-semibold px-1.5 py-0.5 rounded bg-medTeal-500/20 text-medTeal-300 border border-medTeal-500/30">XR Sim</span>
              </div>
              <span className="text-[11px] text-slate-400 -mt-0.5">Cardiac Resuscitation Lab</span>
            </div>
          </a>

          <nav className="hidden lg:flex items-center gap-1 xl:gap-2">
            {navLinks.map((link) => (
              <a
                key={link.name}
                href={link.href}
                className="px-3 py-1.5 text-sm font-medium text-slate-300 hover:text-medTeal-300 hover:bg-slate-800/50 rounded-md transition-colors focus:outline-none focus:ring-2 focus:ring-medTeal-400"
              >
                {link.name}
              </a>
            ))}
          </nav>

          <div className="hidden sm:flex items-center gap-3">
            <div className="hidden xl:flex items-center gap-2 px-2.5 py-1 rounded-full bg-slate-900 border border-slate-800 text-xs text-slate-400">
              <span className="w-2 h-2 rounded-full bg-emerald-400 animate-pulse"></span>
              <span>Unity 6 + OpenXR</span>
            </div>
            
            <a
              href="https://github.com/Memucan-ctrl/medtriage_simulation"
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-100 font-medium text-sm border border-slate-700 transition-all hover:border-medTeal-500/50 shadow-sm focus:outline-none focus:ring-2 focus:ring-medTeal-400"
            >
              <Github className="w-4 h-4 text-medTeal-400" />
              <span>GitHub</span>
            </a>
          </div>

          <div className="lg:hidden flex items-center">
            <button
              onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
              className="p-2 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-medTeal-400"
              aria-label="Toggle Navigation Menu"
            >
              {mobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </div>

      {mobileMenuOpen && (
        <div className="lg:hidden bg-navy-950/98 border-b border-slate-800 px-4 pt-2 pb-6 space-y-1 shadow-2xl backdrop-blur-xl">
          {navLinks.map((link) => (
            <a
              key={link.name}
              href={link.href}
              onClick={() => setMobileMenuOpen(false)}
              className="block px-3 py-2.5 rounded-md text-base font-medium text-slate-200 hover:text-medTeal-300 hover:bg-slate-800/80 transition-colors"
            >
              {link.name}
            </a>
          ))}
          <div className="pt-4 border-t border-slate-800 flex flex-col gap-3">
            <a
              href="https://github.com/Memucan-ctrl/medtriage_simulation"
              target="_blank"
              rel="noopener noreferrer"
              className="flex items-center justify-center gap-2 px-4 py-3 rounded-lg bg-medTeal-600 hover:bg-medTeal-500 text-navy-950 font-semibold text-sm transition-all"
            >
              <Github className="w-4 h-4" />
              <span>View on GitHub</span>
            </a>
          </div>
        </div>
      )}
    </header>
  );
};
