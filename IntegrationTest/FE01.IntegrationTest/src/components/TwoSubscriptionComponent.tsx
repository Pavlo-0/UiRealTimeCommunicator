import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";
import { TwoSubscriptionResponseMessage } from "../communication/BE01.IntegrationTest.Scenarios.TwoSubscription";

const TwoSubscriptionComponent = () => {
  const [status1, setStatus1] = useState(false);
  const [status2, setStatus2] = useState(false);

  useEffect(() => {
    var correlationId = "SimpleId";
    uiRtcSubscription.TwoSubscriptionHub.TwoSubscriptionAnswer(
      (model: TwoSubscriptionResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus1(true);
        }
      }
    );

    uiRtcSubscription.TwoSubscriptionHub.TwoSubscriptionAnswer(
      (model: TwoSubscriptionResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus2(true);
        }
      }
    );

    const actFun = async () => {
      await uiRtcCommunication.TwoSubscriptionHub.TwoSubscriptionHandler({
        correlationId: correlationId,
      });
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Two subscription Test</td>
      <td className="text-center">
        <Badge bg={status1 && status2 ? "success" : "danger"}>
          {status1 && status2 ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default TwoSubscriptionComponent;
