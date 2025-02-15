import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import {
  MessageUiModel,
  uiRtcSubscription,
  UserUiModel,
} from "../chatHub/chat";

const MessagesComponent = () => {
  const [messages, setMessages] = useState([] as MessageUiModel[]);
  const [currentUser, setCurrentUser] = useState({
    id: "",
    name: "",
  } as UserUiModel);

  useEffect(() => {
    uiRtcSubscription.Chat.OnMessage((newMessage) => {
      setMessages((messagesList) => [newMessage, ...messagesList]);
    });

    uiRtcSubscription.Chat.OnUpdate((update) => {
      setCurrentUser(update.currentUser);
      setMessages(update.messagesList.reverse());
    });
  }, []);

  const getBackBackGroundColor = (msg: MessageUiModel) => {
    if (msg.isPrivate) return "#ccfeff";
    if (msg.authorName === currentUser.name) return "#d4edda";
    return "";
  };

  return (
    <div
      className="border flex-grow-1 p-3 overflow-auto"
      style={{ background: "#f8f9fa" }}
    >
      {messages.map((msg, index) => (
        <div
          key={index}
          className={`mb-2 p-2 rounded ${
            msg.authorName === currentUser.name ? "bg-success-subtle" : ""
          }`}
          style={{ backgroundColor: getBackBackGroundColor(msg) }}
        >
          {msg.isPrivate ? (
            <>
              <strong>
                {msg.authorName} to {msg.recipientName}:{" "}
              </strong>
            </>
          ) : (
            <strong>{msg.authorName}: </strong>
          )}

          {msg.message}
          <p className="small text-secondary"> {msg.createDate.toString()} </p>
        </div>
      ))}
    </div>
  );
};

export default MessagesComponent;
