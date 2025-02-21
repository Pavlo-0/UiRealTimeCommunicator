import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const TwoHandlersComponent = () => {
  const [status1, setStatus1] = useState(false);
  const [status2, setStatus2] = useState(false);

  useEffect(() => {
    uiRtcSubscription.TwoHandlers.HandlerAnswer((model) => {
      if (model.handlerNumber == 1) {
        setStatus1(true);
      }

      if (model.handlerNumber == 2) {
        setStatus2(true);
      }
    });

    const actFun = async () => {
      uiRtcCommunication.TwoHandlers.TwoHandler();
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Two server handler</td>
      <td className="text-center">
        <Badge bg={status1 && status2 ? "success" : "danger"}>
          {status1 && status2 ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default TwoHandlersComponent;
