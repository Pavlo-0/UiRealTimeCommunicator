import React, { useState, useEffect } from "react";
import {
  SimpleResponseMessage,
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const TwoContractsComponent = () => {
  const [status1, setStatus1] = useState(false); // Change to true/false to test
  const [status2, setStatus2] = useState(false);

  useEffect(() => {
    var correlationId = "SimpleId";
    uiRtcSubscription.TwoContractsHub.TwoContractsAnswer1(
      (model: SimpleResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus1(true);
        }
      }
    );

    uiRtcSubscription.TwoContractsHub.TwoContractsAnswer2(
      (model: SimpleResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus2(true);
        }
      }
    );

    uiRtcCommunication.TwoContractsHub.TwoContracts({
      correlationId: correlationId,
    });
  }, []);

  return (
    <tr>
      <td>Two contract Hub Test</td>
      <td className="text-center">
        <Badge bg={status1 && status2 ? "success" : "danger"}>
          {status1 && status2 ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default TwoContractsComponent;
