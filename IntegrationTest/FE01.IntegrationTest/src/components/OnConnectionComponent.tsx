import React, { useState, useEffect } from "react";
import { uiRtc, uiRtcSubscription } from "../communication/contract";
import { Badge } from "react-bootstrap";
import { SERVER_URL } from "../consts";

const OnConnectionComponent = () => {
  const [statusConnected, setStatusConnected] = useState(false);
  const [statusDisconnected, setStatusDisconnected] = useState(false);

  useEffect(() => {
    uiRtcSubscription.OnConnectionManager.UpdateStatus((statusModel) => {
      if (statusModel.isConnected) {
        setStatusConnected(true);
      } else {
        setStatusDisconnected(true);
      }
    });

    setTimeout(async () => {
      await uiRtc.disposeAsync(["OnConnectionHub"]);
      await uiRtc.initAsync({
        serverUrl: SERVER_URL,
        activeHubs: ["OnConnectionHub"],
      });
    }, 500);
  }, []);

  return (
    <>
      <tr>
        <td>On connection Test</td>
        <td className="text-center">
          <Badge bg={statusConnected ? "success" : "danger"}>
            {statusConnected ? "Success" : "Fail"}
          </Badge>
        </td>
      </tr>
      <tr>
        <td>On disconnection Test</td>
        <td className="text-center">
          <Badge bg={statusDisconnected ? "success" : "danger"}>
            {statusDisconnected ? "Success" : "Fail"}
          </Badge>
        </td>
      </tr>
    </>
  );
};

export default OnConnectionComponent;
