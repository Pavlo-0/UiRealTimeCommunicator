import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import { uiRtcCommunication } from "../chatHub/chat";
import UsersComponent from "./UsersComponent";
import MessagesComponent from "./MessagesComponent";

const ChatComponent = () => {
  const [message, setMessage] = useState("");

  useEffect(() => {
    uiRtcCommunication.Chat.RefreshHandler();
  }, []);

  const sendMessage = () => {
    if (message.trim() !== "") {
      uiRtcCommunication.Chat.Message({
        message: message,
        recipientId: undefined,
      });
      setMessage("");
    }
  };

  return (
    <div className="container mt-4">
      <div className="row">
        <div className="col-md-3 border-end">
          <UsersComponent></UsersComponent>
        </div>
        {/* Chat Window */}
        <div
          className="col-md-9 d-flex flex-column"
          style={{ height: "600px" }}
        >
          <MessagesComponent></MessagesComponent>
          <div className="d-flex mt-2">
            <input
              type="text"
              className="form-control"
              placeholder="Type a message..."
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && sendMessage()}
            />
            <button className="btn btn-primary ms-2" onClick={sendMessage}>
              Send
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatComponent;
