import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { uiRtc } from "./communication/contract.ts";
import { SERVER_URL } from "./consts.tsx";

await uiRtc.initAsync({
  serverUrl: SERVER_URL,
  activeHubs: "All",
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>
);
