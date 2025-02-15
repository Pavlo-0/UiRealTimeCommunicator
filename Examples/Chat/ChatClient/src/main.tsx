import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { uiRtc } from "./chatHub/chat.ts";

await uiRtc.initAsync({
  serverUrl: "http://localhost:5115/",
  activeHubs: "All",
});

createRoot(document.getElementById("root")!).render(<App />);
