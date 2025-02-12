import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const OnConnectionComponent = () => {
  const [status, setStatus] = useState(false); // Change to true/false to test

  useEffect(() => {
    uiRtcSubscription.OnConnectionHub.OnConnectionAnswer(() => {
      setStatus(true);
    });

    setTimeout(() => {
      uiRtcCommunication.OnConnectionHub.OnConnectionHandler();
    }, 1000);
  }, []);

  return (
    <tr>
      <td>On connection Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default OnConnectionComponent;
