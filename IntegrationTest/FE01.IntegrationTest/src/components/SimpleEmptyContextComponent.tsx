import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const SimpleEmptyContextComponent = () => {
  const [status, setStatus] = useState(false); // Change to true/false to test

  useEffect(() => {
    var correlationId = "SimpleContextId";
    uiRtcSubscription.SimpleEmptyContextHub.SimpleEmptyContextAnswer(() => {
      setStatus(true);
    });

    uiRtcCommunication.SimpleEmptyContextHub.SimpleEmptyContextHandler();
  }, []);

  return (
    <tr>
      <td>Simple Empty Context Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default SimpleEmptyContextComponent;
