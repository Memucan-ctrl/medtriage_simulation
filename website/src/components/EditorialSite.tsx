import React, { useEffect, useState } from 'react';
import {
  Activity,
  ArrowUpRight,
  Github,
  Menu,
  X,
  Check,
  AlertTriangle,
} from 'lucide-react';
import './editorial.css';

const repositoryUrl = 'https://github.com/Memucan-ctrl/medtriage_simulation';
const assetUrl = (path: string) => `${import.meta.env.BASE_URL}${path.replace(/^\/+/, '')}`;

const navigation = [
  { label: 'Overview', href: '#overview' },
  { label: 'Simulation', href: '#simulation' },
  { label: 'Assessment', href: '#assessment' },
  { label: 'Technology', href: '#technology' },
  { label: 'Roadmap', href: '#roadmap' },
  { label: 'Team', href: '#team' },
];

const contributors = [
  { name: 'Memucan-ctrl', role: 'Project lead & repository architect', href: 'https://github.com/Memucan-ctrl' },
  { name: 'Josephat Onkoba', role: 'Simulation & interaction contributor', href: 'https://github.com/Josephat-github' },
  { name: 'Samuel Kangethe', role: 'Integration contributor', href: 'https://github.com/Vexx-bit' },
  { name: 'tmlnaliaka', role: 'Project contributor', href: 'https://github.com/tmlnaliaka' },
  { name: 'gracekamure754-spec', role: 'Project contributor', href: 'https://github.com/gracekamure754-spec' },
];

const journey = [
  { number: '01', title: 'Assess', copy: 'Enter the scenario, review the patient and establish monitoring with the pulse oximeter.' },
  { number: '02', title: 'Recognise', copy: 'Respond to changing physiology and identify deterioration before cardiac arrest.' },
  { number: '03', title: 'Intervene', copy: 'Perform CPR, place pads and follow defibrillation safety gates in immersive XR.' },
  { number: '04', title: 'Debrief', copy: 'Review event-based scores, critical safety flags and opportunities to improve.' },
];

const assessment = [
  ['Protocol adherence', 'Weighted'],
  ['Efficiency', 'Weighted'],
  ['Technical execution', 'Weighted'],
  ['Team communication', 'Weighted'],
  ['Decision-making', 'Weighted'],
];

export const EditorialSite: React.FC = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 12);
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  return (
    <div className="editorial-site">
      <header className={`site-header ${scrolled ? 'site-header--scrolled' : ''}`}>
        <div className="site-shell header-inner">
          <a className="brand" href="#overview" aria-label="MedTriage home">
            <span className="brand-mark"><Activity aria-hidden="true" /></span>
            <span><strong>MedTriage</strong><small>Cardiac resuscitation simulation</small></span>
          </a>
          <nav className="desktop-nav" aria-label="Primary navigation">
            {navigation.map((item) => <a key={item.href} href={item.href}>{item.label}</a>)}
          </nav>
          <a className="header-action" href={repositoryUrl} target="_blank" rel="noreferrer">
            <Github aria-hidden="true" /><span>View source</span>
          </a>
          <button className="menu-button" type="button" aria-label="Toggle navigation" aria-expanded={menuOpen} onClick={() => setMenuOpen((value) => !value)}>
            {menuOpen ? <X /> : <Menu />}
          </button>
        </div>
        {menuOpen && (
          <nav className="mobile-nav" aria-label="Mobile navigation">
            {navigation.map((item) => <a key={item.href} href={item.href} onClick={() => setMenuOpen(false)}>{item.label}</a>)}
            <a href={repositoryUrl} target="_blank" rel="noreferrer">View source</a>
          </nav>
        )}
      </header>

      <main>
        <section className="hero" id="overview">
          <div className="site-shell hero-grid">
            <div className="hero-copy">
              <p className="eyebrow">Unity 6 · OpenXR · Cardiac arrest scenario</p>
              <h1>Practice the decisions that matter before they become real.</h1>
              <p className="hero-lede">MedTriage is an immersive training simulation for in-hospital cardiac arrest. It connects physical XR interactions with patient physiology, safety checks and objective debriefing.</p>
              <div className="hero-actions">
                <a className="button button--primary" href="#simulation">Explore the simulation <ArrowUpRight aria-hidden="true" /></a>
                <a className="button button--quiet" href={repositoryUrl} target="_blank" rel="noreferrer"><Github aria-hidden="true" /> Source on GitHub</a>
              </div>
              <dl className="hero-facts" aria-label="Project facts">
                <div><dt>Scenario</dt><dd>In-hospital cardiac arrest</dd></div>
                <div><dt>Platform</dt><dd>Meta Quest & desktop XR</dd></div>
                <div><dt>Assessment</dt><dd>Event-based scoring</dd></div>
              </dl>
            </div>
            <figure className="hero-visual">
              <img src={assetUrl('screenshots/current_cardiac_room.png')} alt="Current furnished cardiac resuscitation room in the MedTriage Unity scene" />
              <figcaption><span>Unity environment</span><span>Cardiac_MVP.unity</span></figcaption>
            </figure>
          </div>
        </section>

        <section className="section section--light">
          <div className="site-shell editorial-split">
            <div><p className="section-kicker">Why MedTriage</p><h2>Clinical rehearsal with measurable consequences.</h2></div>
            <div className="prose-column">
              <p>Emergency training is most useful when learners can connect protocol, timing and team communication. MedTriage turns those decisions into observable events rather than relying on a purely subjective recap.</p>
              <p>The current cardiac-arrest scenario combines tracked CPR mechanics, rhythm-aware defibrillation, patient deterioration and structured performance review in one repeatable environment.</p>
            </div>
          </div>
          <div className="site-shell capability-list" aria-label="Core capabilities">
            <article><span>01</span><div><h3>Biomechanical CPR</h3><p>Tracks compression depth, cadence and hands-off interruptions.</p></div></article>
            <article><span>02</span><div><h3>Defibrillation safety</h3><p>Validates pad placement, rhythm classification and the “all clear” sequence.</p></div></article>
            <article><span>03</span><div><h3>Reactive physiology</h3><p>Moves the virtual patient through monitored, deteriorating, arrest and recovery states.</p></div></article>
          </div>
        </section>

        <section className="section" id="simulation">
          <div className="site-shell">
            <div className="section-heading">
              <div><p className="section-kicker">Simulation</p><h2>A deliberate learning loop.</h2></div>
              <p>One scenario moves from assessment to intervention and finishes with evidence the learner can review.</p>
            </div>
            <ol className="journey-list">
              {journey.map((step) => <li key={step.number}><span>{step.number}</span><h3>{step.title}</h3><p>{step.copy}</p></li>)}
            </ol>
            <div className="feature-story">
              <div className="feature-image"><img src={assetUrl('screenshots/current_patient_bed.png')} alt="Current cardiac-arrest patient and treatment bed in MedTriage" /></div>
              <div className="feature-copy">
                <p className="section-kicker">Interaction detail</p>
                <h2>The pulse oximeter starts the clinical story.</h2>
                <p>The learner docks a dedicated probe onto the patient’s finger socket. A successful placement activates monitoring, advances the scenario and begins the deterioration sequence.</p>
                <ul className="plain-checklist">
                  <li><Check aria-hidden="true" />Probe-specific socket filtering</li>
                  <li><Check aria-hidden="true" />Spatial confirmation audio</li>
                  <li><Check aria-hidden="true" />Vitals and scenario-state trigger</li>
                </ul>
                <a className="text-link" href={repositoryUrl} target="_blank" rel="noreferrer">Inspect the implementation <ArrowUpRight aria-hidden="true" /></a>
              </div>
            </div>
          </div>
        </section>

        <section className="section section--ink" id="assessment">
          <div className="site-shell assessment-grid">
            <div className="assessment-intro">
              <p className="section-kicker">Assessment</p><h2>Scored from events, not impressions.</h2>
              <p>Session telemetry is evaluated across five competency areas. Categories that are not assessed are excluded from normalization instead of receiving an artificial score.</p>
            </div>
            <div className="score-table" role="table" aria-label="Assessment categories">
              {assessment.map(([label, value]) => <div className="score-row" role="row" key={label}><span role="cell">{label}</span><strong role="cell">{value}</strong></div>)}
            </div>
            <aside className="safety-note"><AlertTriangle aria-hidden="true" /><div><h3>Safety events remain visible.</h3><p>Extended pauses, unsafe shocks and missing clear calls create explicit review flags alongside the score.</p></div></aside>
          </div>
        </section>

        <section className="section" id="technology">
          <div className="site-shell">
            <div className="section-heading">
              <div><p className="section-kicker">Technology</p><h2>Built for portable XR.</h2></div>
              <p>The runtime remains separate from development tooling, keeping the learner experience focused.</p>
            </div>
            <dl className="technology-list">
              <div><dt>Engine</dt><dd>Unity 6</dd></div><div><dt>XR runtime</dt><dd>OpenXR 1.17</dd></div><div><dt>Interaction</dt><dd>XR Interaction Toolkit 3.5</dd></div><div><dt>Rendering</dt><dd>Universal Render Pipeline 17.5</dd></div><div><dt>Profiles</dt><dd>Unity Cloud Save</dd></div><div><dt>Targets</dt><dd>Meta Quest & PCVR</dd></div>
            </dl>
            <div className="image-pair">
              <figure><img src={assetUrl('screenshots/current_clinical_equipment.png')} alt="Current clinical equipment in the cardiac room, including monitoring and resuscitation tools" /><figcaption>Live-scene clinical equipment</figcaption></figure>
              <figure><img src={assetUrl('screenshots/current_room_detail.png')} alt="Current furnished cardiac room from an alternate clinical angle" /><figcaption>Current cardiac room detail</figcaption></figure>
            </div>
          </div>
        </section>

        <section className="section section--light roadmap-section" id="roadmap">
          <div className="site-shell">
            <div className="section-heading"><div><p className="section-kicker">Roadmap</p><h2>Clear about what exists—and what comes next.</h2></div></div>
            <div className="roadmap-columns">
              <div><span className="roadmap-label roadmap-label--ready">Available now</span><ul><li>Pulse oximeter interaction</li><li>Patient deterioration model</li><li>CPR tracking</li><li>Defibrillator safety gates</li><li>Objective scoring</li></ul></div>
              <div><span className="roadmap-label roadmap-label--active">In development</span><ul><li>Final integrated scenario</li><li>Production debrief proxy</li><li>Medication interaction</li><li>Team voice integration</li></ul></div>
              <div><span className="roadmap-label">Exploring</span><ul><li>Collaborative multiplayer</li><li>Additional emergency scenarios</li><li>Reversible-cause diagnostics</li><li>LMS reporting</li></ul></div>
            </div>
          </div>
        </section>

        <section className="section" id="team">
          <div className="site-shell team-layout">
            <div><p className="section-kicker">Team</p><h2>Built in the open.</h2><p className="team-intro">MedTriage is developed as a modular Unity project with source, implementation notes and project history available on GitHub.</p></div>
            <div className="team-list">
              {contributors.map((person) => <a href={person.href} target="_blank" rel="noreferrer" key={person.href}><span><strong>{person.name}</strong><small>{person.role}</small></span><ArrowUpRight aria-hidden="true" /></a>)}
            </div>
          </div>
        </section>
      </main>

      <footer className="site-footer">
        <div className="site-shell footer-main">
          <div className="brand brand--footer"><span className="brand-mark"><Activity aria-hidden="true" /></span><span><strong>MedTriage</strong><small>Educational XR simulation</small></span></div>
          <p>MedTriage is an educational and procedural training simulation. It does not provide medical advice and is not a certified medical device.</p>
          <a href={repositoryUrl} target="_blank" rel="noreferrer"><Github aria-hidden="true" /> GitHub</a>
        </div>
        <div className="site-shell footer-meta"><span>© 2026 MedTriage Simulation Project</span><span>Unity 6 · OpenXR · URP</span></div>
      </footer>
    </div>
  );
};
