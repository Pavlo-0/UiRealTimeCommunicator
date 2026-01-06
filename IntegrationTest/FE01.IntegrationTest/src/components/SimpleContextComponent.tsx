import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";
import { SimpleContextResponseMessage } from "../communication/BE01.IntegrationTest.Scenarios.SimpleContext";

const SimpleContextComponent = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    var correlationId = "SimpleContextId";
    uiRtcSubscription.SimpleContextHub.SimpleContextAnswer(
      (model: SimpleContextResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus(true);
        }
      }
    );

    const actFun = async () => {
      await uiRtcCommunication.SimpleContextHub.SimpleContextHandler({
        correlationId: correlationId,
      });
    };
    actFun();
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
