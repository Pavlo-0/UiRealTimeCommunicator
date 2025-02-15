import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import { uiRtcCommunication } from "../chatHub/chat";

interface ChatLoginProps {
  onJoin: () => void; // Explicit type for 'onJoin'
}

const ChatLogin: React.FC<ChatLoginProps> = ({ onJoin }) => {
  const [username, setUsername] = useState("");

  const handleJoin = async () => {
    if (username.trim()) {
      await uiRtcCommunication.Chat.Join({ name: username.trim() });
      onJoin();
    }
  };

  return (
    <div className="container d-flex justify-content-center align-items-center vh-100">
      <div className="card p-4 shadow" style={{ width: "300px" }}>
        <h5 className="text-center mb-3">Join the Chat</h5>

        <div className="mb-3">
          <input
            type="text"
            className="form-control"
            placeholder="Enter your name"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            onKeyUp={(e) => e.key === "Enter" && handleJoin()}
          />
        </div>

        <button className="btn btn-primary w-100" onClick={handleJoin}>
          Join
        </button>
      </div>
    </div>
  );
};

export default ChatLogin;
