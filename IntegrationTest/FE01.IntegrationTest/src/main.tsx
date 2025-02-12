import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { uiRtc } from "./communication/contract.ts";

await uiRtc.initAsync({
  serverUrl: "http://localhost:5065/",
  activeHubs: "All",
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>
);
