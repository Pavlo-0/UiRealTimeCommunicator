import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const SimpleEmptyComponent = () => {
  const [status, setStatus] = useState(false); // Change to true/false to test

  useEffect(() => {
    uiRtcSubscription.SimpleEmptyHub.SimpleEmptyAnswer(() => {
      setStatus(true);
    });

    uiRtcCommunication.SimpleEmptyHub.SimpleEmptyHandler();
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
