import React, { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import {
  uiRtcCommunication,
  uiRtcSubscription,
  UserUiModel,
} from "../chatHub/chat";
import { Button, Form, Modal } from "react-bootstrap";

const UsersComponent = () => {
  const [users, setUsers] = useState([] as UserUiModel[]);
  const [currentUser, setCurrentUser] = useState({
    id: "",
    name: "",
  } as UserUiModel);

  const [showPopup, setShowPopup] = useState(false);
  const [selectedUser, setSelectedUser] = useState(
    null as unknown as UserUiModel
  );
  const [message, setMessage] = useState("");

  useEffect(() => {
    uiRtcSubscription.Chat.UserListUpdate((users) => {
      setUsers(users.list);
    });

    uiRtcSubscription.Chat.OnUpdate((update) => {
      setCurrentUser(update.currentUser);
      setUsers(update.usersList);
    });
  }, []);

  const handleShowPopup = (user: UserUiModel) => {
    setSelectedUser(user);
    setShowPopup(true);
  };

  const handleClosePopup = () => {
    setShowPopup(false);
    setMessage("");
  };

  const handleSendMessage = async () => {
    if (selectedUser && message.trim()) {
      await uiRtcCommunication.Chat.Message({
        message: message.trim(),
        recipientId: selectedUser.id,
      });
      handleClosePopup();
    }
  };

  return (
    <>
      <h5 className="text-center">Users</h5>
      <ul className="list-group">
        {users.map((user, index) => (
          <li
            key={index}
            className="list-group-item d-flex justify-content-between align-items-center"
          >
            {user.id === currentUser.id ? <b>{user.name}</b> : user.name}
            {user.id !== currentUser.id && (
              <Button
                variant="outline-primary"
                size="sm"
                onClick={() => handleShowPopup(user)}
              >
                PM
              </Button>
            )}
          </li>
        ))}
      </ul>

      <Modal show={showPopup} onHide={handleClosePopup}>
        <Modal.Header closeButton>
          <Modal.Title>Private Message to {selectedUser?.name}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group>
              <Form.Label>Message</Form.Label>
              <Form.Control
                type="text"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Type your message..."
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClosePopup}>
            Close
          </Button>
          <Button variant="primary" onClick={handleSendMessage}>
            Send
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default UsersComponent;
