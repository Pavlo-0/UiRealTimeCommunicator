import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";
import { SimpleDateTimeResponseMessage } from "../communication/BE01.IntegrationTest.Scenarios.SimpleDateTimeField";

const SimpleDateTimeComponent = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    var correlationId = "SimpleDateTimeId";
    var dateTime = new Date("2023-01-01T12:00:00Z");
    uiRtcSubscription.SimpleDateTimeHub.SimpleAnswer(
      (model: SimpleDateTimeResponseMessage) => {
        console.log("Received model:", new Date(model.paramDateTime));
        console.log("Expected dateTime:", dateTime);
        if (
          model.correlationId === correlationId &&
          new Date(model.paramDateTime).getTime() === dateTime.getTime()
        ) {
          setStatus(true);
        }
      }
    );

    const actFun = async () => {
      await uiRtcCommunication.SimpleDateTimeHub.SimpleDateTimeHandler({
        correlationId: correlationId,
        paramDateTime: dateTime,
      });
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Simple Date Time Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default SimpleDateTimeComponent;
