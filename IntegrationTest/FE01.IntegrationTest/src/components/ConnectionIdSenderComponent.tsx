import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const ConnectionIdSenderComponent = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    uiRtcSubscription.ConnectionIdSenderHub.SendToSpecificUser((model) => {
      if (model.connectionId) {
        setStatus(true);
      }
    });

    uiRtcCommunication.ConnectionIdSenderHub.ConnectionIdRequest();
  }, []);

  return (
    <tr>
      <td>Send to user by connection</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default ConnectionIdSenderComponent;
