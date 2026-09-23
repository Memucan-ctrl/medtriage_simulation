# MedTriage Website

This directory contains the React, TypeScript, and Vite website for the MedTriage cardiac-arrest clinical simulation project. It showcases the architectural design, clinical state machines, interactive vital sign simulators, and in-engine hospital environments.

---

## Requirements

- **Git**
- **Node.js 20 or newer** (verify with `node --version`)
- **npm** (verify with `npm --version`)

> **Note:** Unity is **not** required to run or build the website. All 3D hospital imagery and clinical assets are pre-rendered into static visual assets.

---

## Clone the repository

To clone the repository and navigate to the website directory:

```bash
git clone https://github.com/Memucan-ctrl/medtriage_simulation.git
cd medtriage_simulation/website
```

### Working from the feature branch

If you are reviewing or developing on the feature branch before the pull request is merged into the default branch (`main`), clone using the branch-specific command:

```bash
git clone --branch website/share-local-setup https://github.com/Memucan-ctrl/medtriage_simulation.git
cd medtriage_simulation/website
```

*(Note: The branch-specific command is only necessary until the pull request is merged into the default branch).*

---

## Install dependencies

Install the project dependencies using npm:

```bash
npm install
```

For a clean, reproducible installation matching the lockfile (`package-lock.json`), you can use:

```bash
npm ci
```

---

## Run the development server

Start the local Vite development server with Hot Module Replacement (HMR):

```bash
npm run dev
```

Once started, open the local URL displayed in the terminal:

```text
http://localhost:5173/
```

*(If port 5173 is already in use by another process, Vite will automatically select the next available port, such as 5174).*

---

## Create a production build

To type-check with TypeScript and build an optimized production bundle:

```bash
npm run build
```

The generated static production files will be placed in:

```text
website/dist/
```

---

## Preview the production build

To locally serve and test the generated production build:

```bash
npm run preview
```

Open the local preview URL printed by Vite in your browser to verify the production bundle.

---

## Project structure

```text
website/
├── public/
│   ├── brand/                 # Official brand icons and logos
│   └── screenshots/           # Static in-engine scene captures used throughout the site
├── src/
│   ├── components/
│   │   ├── EditorialSite.tsx  # Primary website content and React structure
│   │   └── editorial.css      # Main website layout and visual styling
│   ├── App.tsx                # Application entry component rendering the main site
│   ├── index.css              # Global styling tokens and Tailwind directives
│   └── main.tsx               # React application entry point
├── dist/                      # Generated production build output (do not edit manually)
├── index.html                   # HTML shell and metadata
├── package.json               # Scripts and dependency declarations
├── package-lock.json          # Dependency lockfile
├── tsconfig.json              # TypeScript compiler configuration
└── vite.config.ts             # Vite build configuration
```

- **`src/App.tsx`**: The application entry component that renders the main website.
- **`src/components/EditorialSite.tsx`**: The primary website content, narrative layout, and React component structure.
- **`src/components/editorial.css`**: The main website layout, typography, animations, and visual styling.
- **`public/screenshots/`**: Static screenshots and scene renders used throughout the website.
- **`dist/`**: Generated production build output. It should not normally be edited manually.

---

## Updating website screenshots

Production screenshots should be placed inside:

```text
website/public/screenshots/
```

The active in-engine screenshots used by the website:
- `current_cardiac_room.png` — Resuscitation bay and clinical zone layout
- `current_patient_bed.png` — Virtual patient bedside and monitoring clearance
- `current_clinical_equipment.png` — Bedside vitals monitor, crash cart, and defibrillator staging
- `current_room_detail.png` — Architectural detail, medical wall fixtures, and circulation space

Image references, descriptive alt text, and captions are managed inside:

```text
website/src/components/EditorialSite.tsx
```

When adding or replacing screenshots, ensure high-resolution PNG captures are used with clear filenames and meaningful alt text.

---

## Available commands

The following scripts are defined in `package.json`:

| Command | Description |
| :--- | :--- |
| `npm run dev` | Starts the local Vite development server at `http://localhost:5173/` |
| `npm run build` | Runs TypeScript validation (`tsc`) and compiles the production bundle into `dist/` |
| `npm run preview` | Serves the compiled production bundle locally for preview and QA |

---

## Troubleshooting

### Dependencies are missing
If packages or modules are not found:
```bash
npm install
```

### Port 5173 is already occupied
To explicitly specify an alternative port:
```bash
npm run dev -- --port 5174
```

### Clean installation
If you encounter dependency issues, perform a clean reinstall using the existing lockfile:
```bash
# On Windows PowerShell:
Remove-Item -Recurse -Force node_modules
npm ci
```
*(Do not delete `package-lock.json` as it guarantees consistent package versions).*

### Build fails
Verify your Node.js and npm versions:
```bash
node --version
npm --version
```
Ensure you are running **Node.js 20 or newer**.

---

## Medical disclaimer

MedTriage is an educational clinical simulation project. It is not a certified medical device and is not a substitute for professional medical education, clinical judgement, institutional protocols, or supervised training.
