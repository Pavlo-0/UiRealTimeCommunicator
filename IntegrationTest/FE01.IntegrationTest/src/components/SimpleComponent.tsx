import React, { useState, useEffect } from "react";
import {
  SimpleResponseMessage,
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const SimpleComponent = () => {
  const [status, setStatus] = useState(false); // Change to true/false to test

  useEffect(() => {
    var correlationId = "SimpleId";
    uiRtcSubscription.SimpleHub.SimpleAnswer((model: SimpleResponseMessage) => {
      if (model.correlationId == correlationId) {
        setStatus(true);
      }
    });

    const actFun = async () => {
      await uiRtcCommunication.SimpleHub.SimpleHandler({
        correlationId: correlationId,
      });
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Simple Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default SimpleComponent;
