import React, { useState, useEffect } from "react";
import {
  SimpleContexResponseMessage,
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const SimpleContextComponent = () => {
  const [status, setStatus] = useState(false); // Change to true/false to test

  useEffect(() => {
    var correlationId = "SimpleContextId";
    uiRtcSubscription.SimpleContextHub.SimpleContextAnswer(
      (model: SimpleContexResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus(true);
        }
      }
    );

    uiRtcCommunication.SimpleContextHub.SimpleContextHandler({
      correlationId: correlationId,
    });
  }, []);

  return (
    <tr>
      <td>Simple Context Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default SimpleContextComponent;
