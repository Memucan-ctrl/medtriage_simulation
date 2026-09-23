# MedTriage Website

This directory contains the React, TypeScript, and Vite website for the MedTriage cardiac-arrest clinical simulation project. The current site presents verified project information, implementation details, development status, and in-engine imagery.

## Requirements

- Git
- Git LFS
- Node.js 20 or newer
- npm

Unity is not required to run or build the website. The Unity screenshots are provided as static assets tracked with Git LFS.

## Local setup

```bash
git lfs install
git clone https://github.com/Memucan-ctrl/medtriage_simulation.git
cd medtriage_simulation/website
git lfs pull
npm ci
npm run dev
```

Before this pull request is merged, clone its feature branch instead:

```bash
git clone --branch website/share-local-setup https://github.com/Memucan-ctrl/medtriage_simulation.git
cd medtriage_simulation/website
git lfs pull
npm ci
npm run dev
```

The development server binds to `127.0.0.1` by default. Open the local URL printed by Vite, normally `http://localhost:5173/`.

To expose the development server to another device on a trusted network, opt in explicitly:

```bash
npm run dev -- --host
```

## Production build

```bash
npm run build
npm run preview
```

The production files are generated in `website/dist/`.

## Active source structure

```text
website/
├── public/
│   ├── _Master*.png
│   └── screenshots/
├── src/
│   ├── components/
│   │   ├── EditorialSite.tsx
│   │   └── editorial.css
│   ├── App.tsx
│   ├── index.css
│   └── main.tsx
├── index.html
├── package.json
├── package-lock.json
├── tsconfig.json
└── vite.config.ts
```

The active site uses these screenshots:

- `current_cardiac_room.png`
- `current_patient_bed.png`
- `current_clinical_equipment.png`
- `current_room_detail.png`

If images are missing, run `git lfs pull`. If dependencies are missing, run `npm ci` again.

## Medical disclaimer

MedTriage is an educational clinical simulation project. It is not a certified medical device and is not a substitute for professional medical education, clinical judgement, institutional protocols, or supervised training.
