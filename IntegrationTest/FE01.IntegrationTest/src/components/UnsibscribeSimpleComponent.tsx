import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const UnsubscribeSimple = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    let stackCounter = 0;
    var subscription =
      uiRtcSubscription.UnsubscribeSimpleHub.UnsubscribeSimpleAnswer(() => {
        setStatus(false);
        subscription.unsubscribe();
        stackCounter++;
        if (stackCounter < 10) {
          setStatus(true);
          actFun();
        }
      });

    const actFun = async () => {
      await uiRtcCommunication.UnsubscribeSimpleHub.UnsubscribeSimpleHandler();
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Unsubscribe Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default UnsubscribeSimple;
