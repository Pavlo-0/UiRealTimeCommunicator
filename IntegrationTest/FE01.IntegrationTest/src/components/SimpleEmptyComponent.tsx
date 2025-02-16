import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const SimpleEmptyComponent = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    uiRtcSubscription.SimpleEmptyHub.SimpleEmptyAnswer(() => {
      setStatus(true);
    });

    const actFun = async () => {
      uiRtcCommunication.SimpleEmptyHub.SimpleEmptyHandler();
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>Simple Empty Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default SimpleEmptyComponent;
