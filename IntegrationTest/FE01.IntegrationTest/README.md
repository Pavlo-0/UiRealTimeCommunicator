**Project**

- **Name**: `app-frontend` (Vite + React + TypeScript)

**Prerequisites**

- **Node**: Install Node.js (recommended `>=18`).
- **Package manager**: `npm` is used in examples (works with `pnpm`/`yarn` if you prefer).

**Setup**

- **Project folder**: `d:\Projects\UiRealTimeCommunicator\IntegrationTest\FE01.IntegrationTest`
- **Install dependencies**: run below from the project folder.

```cmd
cd /d "d:\Projects\UiRealTimeCommunicator\IntegrationTest\FE01.IntegrationTest"
npm install
```

**Run (development)**

- Start the Vite dev server:

```cmd
npm run dev
```

- Vite will print the local URL (usually `http://localhost:5173`). Open that in your browser.

**Build & Preview (production)**

- Build the app (this runs `tsc -b` then `vite build`):

```cmd
npm run build
```

- Serve a local preview of the production build:

```cmd
npm run preview
```

**What the scripts do**

- `dev`: runs `vite` (development server with HMR).
- `build`: runs `tsc -b` (TypeScript project build) then `vite build`.
- `preview`: runs `vite preview` to serve the production bundle locally.

**Key dependencies (from `package.json`)**

- `react`, `react-dom`, `@microsoft/signalr`, `bootstrap`.
- Dev tools: `vite`, `typescript`, `@vitejs/plugin-react`, `eslint`.

**Troubleshooting**

- If `npm install` fails: ensure Node/npm are installed and available in `PATH`.
- If port is in use: Vite will suggest another port or you can run `npm run dev -- --port 3000`.
- If `npm run build` fails: fix TypeScript errors printed by `tsc`. Running `npm run dev` first can help identify issues interactively.
- If you prefer a different package manager: `pnpm install` or `yarn install` followed by the same `npm` scripts (replace `npm run` with `pnpm`/`yarn` equivalents).

**Where to look next**

- Source: `src/` (components and communication logic). The app entry is `src/main.tsx` and the HTML template is `index.html`.
- TypeScript config: `tsconfig.json` / `tsconfig.app.json`.

If you want, I can run `npm install` and `npm run dev` here and report back the server output.
