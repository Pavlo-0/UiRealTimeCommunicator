import React, { useState, useEffect } from "react";
import {
  SimpleResponseMessage,
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const TwoContractMethodsComponent = () => {
  const [status1, setStatus1] = useState(false); // Change to true/false to test
  const [status2, setStatus2] = useState(false);

  useEffect(() => {
    var correlationId = "SimpleId";
    uiRtcSubscription.TwoContractMethodsHub.TwoContractMethodsAnswer1(
      (model: SimpleResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus1(true);
        }
      }
    );

    uiRtcSubscription.TwoContractMethodsHub.TwoContractMethodsAnswer2(
      (model: SimpleResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus2(true);
        }
      }
    );

    uiRtcCommunication.TwoContractMethodsHub.TwoContractMethods({
      correlationId: correlationId,
    });
  }, []);

  return (
    <tr>
      <td>Two contract method hub Test</td>
      <td className="text-center">
        <Badge bg={status1 && status2 ? "success" : "danger"}>
          {status1 && status2 ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default TwoContractMethodsComponent;
