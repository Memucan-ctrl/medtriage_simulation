import React, { useState } from 'react';
import { Camera, Eye, Layers, Monitor, ExternalLink, X } from 'lucide-react';

export const GallerySection: React.FC = () => {
  const [selectedShot, setSelectedShot] = useState<number | null>(null);

  const captureCards = [
    {
      id: 1,
      title: 'Dedicated Cardiac Resuscitation Room',
      scene: 'Hospital_Integration.unity',
      view: 'Clinical Environment View (1920x1080)',
      target: 'Dedicated resuscitation bay configured for acute cardiac scenarios',
      angle: 'Eye-level clinical perspective, unobstructed resuscitation zone',
      badge: 'Scene Overview',
      imageSrc: './screenshots/reserved_cardiac_room.png',
      details: 'Showcases patient positioning area, clinical clearance boundaries, crash cart staging, and architectural integration.',
    },
    {
      id: 2,
      title: 'Patient Treatment & Observation Bay',
      scene: 'Hospital_Integration.unity',
      view: 'In-Engine Scene Capture',
      target: 'Standard patient treatment room with bedside furnishings & monitoring clearance',
      angle: 'Elevated isometric room perspective',
      badge: 'Environment & Ward',
      imageSrc: './screenshots/patient_room_01.png',
      details: 'Demonstrates surgical/patient bed placement, medical wall fixtures, and clinical movement corridors.',
    },
    {
      id: 3,
      title: 'Clinical Triage & Reception Wing',
      scene: 'Hospital_Integration.unity',
      view: 'In-Engine Architectural View',
      target: 'Triage reception desk, waiting clearance, and intake corridor',
      angle: 'Wide entrance perspective',
      badge: 'Triage & Ingestion',
      imageSrc: './screenshots/reception_area.png',
      details: 'Initial emergency admission environment for patient triage classification and intake staging.',
    },
    {
      id: 4,
      title: 'Emergency Department Circulation Corridor',
      scene: 'Hospital_Integration.unity',
      view: 'Rapid Transit View',
      target: 'Main hospital corridor connecting reception to resuscitation suites',
      angle: 'Linear central corridor perspective',
      badge: 'Circulation & Flow',
      imageSrc: './screenshots/corridor_circulation.png',
      details: 'Validated obstacle-free transport corridor ensuring code teams and gurneys have direct rapid transit.',
    },
    {
      id: 5,
      title: 'Furnished Clinical Waiting Bay',
      scene: 'Hospital_Integration.unity',
      view: 'Interior Architectural View',
      target: 'Modular patient waiting area with natural lighting and spatial acoustic layout',
      angle: 'Corner wide shot',
      badge: 'Hospital Facility',
      imageSrc: './screenshots/furnished_waiting_bay.png',
      details: 'Secondary observation and family waiting area integrated with the main emergency hospital floorplan.',
    },
    {
      id: 6,
      title: 'Hospital Emergency Entrance & Facade',
      scene: 'Hospital_Integration.unity',
      view: 'Exterior Architectural View',
      target: 'Hospital exterior facade, ambulance bay approach, and entry glazing',
      angle: 'Exterior approach shot',
      badge: 'Site & Architecture',
      imageSrc: './screenshots/exterior_entrance.png',
      details: 'Ambulance arrival zone and main hospital exterior entrance modeled in the Unity URP environment.',
    },
  ];

  const activeCapture = captureCards.find((c) => c.id === selectedShot);

  return (
    <section id="gallery" className="py-24 bg-slate-900 border-t border-slate-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-end justify-between mb-16 gap-6">
          <div>
            <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-cyan-500/10 border border-cyan-500/30 text-cyan-400 text-xs font-semibold uppercase tracking-wider mb-4">
              <Camera className="w-3.5 h-3.5" />
              In-Engine Environment & Visuals
            </div>
            <h2 className="text-3xl sm:text-4xl font-bold text-white tracking-tight">
              Hospital Simulation Environment
            </h2>
            <p className="mt-4 text-lg text-slate-400 max-w-2xl">
              Engineered in Unity URP with standardized spatial zoning, collision clearance, medical furnishings, and high-fidelity lighting.
            </p>
          </div>

          <div className="flex items-center gap-3">
            <a
              href="./SCREENSHOT_CHECKLIST.md"
              target="_blank"
              rel="noreferrer"
              className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-slate-800 hover:bg-slate-700 border border-slate-700 text-slate-300 text-sm font-medium transition"
            >
              <ExternalLink className="w-4 h-4" />
              Screenshot Capture Guide
            </a>
          </div>
        </div>

        {/* Gallery Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {captureCards.map((card) => (
            <div
              key={card.id}
              onClick={() => setSelectedShot(card.id)}
              className="group bg-slate-950/80 rounded-2xl border border-slate-800 hover:border-cyan-500/50 transition-all duration-300 overflow-hidden cursor-pointer flex flex-col shadow-lg hover:shadow-cyan-500/10"
            >
              {/* Image Preview Container */}
              <div className="relative aspect-video bg-slate-900 overflow-hidden border-b border-slate-800/80 flex items-center justify-center">
                {card.imageSrc ? (
                  <img
                    src={card.imageSrc}
                    alt={card.title}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                    loading="lazy"
                  />
                ) : (
                  <div className="text-center p-6">
                    <Monitor className="w-10 h-10 text-slate-600 mx-auto mb-2" />
                    <span className="text-xs text-slate-500 font-mono">1920x1080 View</span>
                  </div>
                )}
                
                <div className="absolute top-3 left-3 px-2.5 py-1 rounded-md bg-slate-950/80 backdrop-blur-md border border-slate-700 text-[11px] font-semibold text-cyan-400">
                  {card.badge}
                </div>

                <div className="absolute inset-0 bg-slate-950/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
                  <span className="px-3 py-1.5 rounded-lg bg-cyan-500 text-slate-950 font-semibold text-xs flex items-center gap-1.5 shadow-lg">
                    <Eye className="w-3.5 h-3.5" />
                    View Fullscreen
                  </span>
                </div>
              </div>

              {/* Content */}
              <div className="p-6 flex-1 flex flex-col justify-between">
                <div>
                  <h3 className="text-lg font-bold text-white group-hover:text-cyan-400 transition-colors">
                    {card.title}
                  </h3>
                  <p className="mt-2 text-sm text-slate-400 line-clamp-2">
                    {card.details}
                  </p>
                </div>

                <div className="mt-5 pt-4 border-t border-slate-800/80 space-y-2 text-xs font-mono text-slate-400">
                  <div className="flex items-center justify-between">
                    <span className="text-slate-500 flex items-center gap-1">
                      <Layers className="w-3.5 h-3.5" /> Scene
                    </span>
                    <span className="text-slate-300 truncate max-w-[180px]">{card.scene}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-slate-500 flex items-center gap-1">
                      <Monitor className="w-3.5 h-3.5" /> Capture View
                    </span>
                    <span className="text-slate-300">{card.view}</span>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {/* Lightbox Modal */}
        {activeCapture && (
          <div
            className="fixed inset-0 z-50 bg-slate-950/90 backdrop-blur-md p-4 sm:p-8 flex items-center justify-center"
            onClick={() => setSelectedShot(null)}
          >
            <div
              className="relative max-w-5xl w-full bg-slate-900 border border-slate-700 rounded-2xl overflow-hidden shadow-2xl"
              onClick={(e) => e.stopPropagation()}
            >
              {/* Modal Header */}
              <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-950/60">
                <div className="flex items-center gap-3">
                  <span className="px-2.5 py-1 rounded-md bg-cyan-500/20 text-cyan-400 border border-cyan-500/30 text-xs font-semibold">
                    {activeCapture.badge}
                  </span>
                  <h4 className="text-lg font-bold text-white">{activeCapture.title}</h4>
                </div>
                <button
                  onClick={() => setSelectedShot(null)}
                  className="p-1.5 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800 transition"
                  aria-label="Close modal"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Modal Image */}
              <div className="relative aspect-video bg-black flex items-center justify-center">
                <img
                  src={activeCapture.imageSrc}
                  alt={activeCapture.title}
                  className="w-full h-full object-contain"
                />
              </div>

              {/* Modal Meta */}
              <div className="p-6 bg-slate-950/90 border-t border-slate-800 grid grid-cols-1 md:grid-cols-2 gap-6 text-sm">
                <div>
                  <h5 className="font-semibold text-slate-200 mb-1">Architecture & Purpose</h5>
                  <p className="text-slate-400 text-xs leading-relaxed">{activeCapture.details}</p>
                </div>
                <div className="space-y-1.5 text-xs font-mono">
                  <div className="flex justify-between">
                    <span className="text-slate-500">Source Scene:</span>
                    <span className="text-cyan-400">{activeCapture.scene}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-500">Camera Setup:</span>
                    <span className="text-slate-300">{activeCapture.angle}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-500">Target:</span>
                    <span className="text-slate-300 truncate max-w-[280px]">{activeCapture.target}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

      </div>
    </section>
  );
};
