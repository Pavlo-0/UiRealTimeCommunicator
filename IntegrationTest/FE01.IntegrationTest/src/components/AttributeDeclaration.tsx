import React, { useState, useEffect } from "react";
import {
  SimpleResponseMessage,
  uiRtcCommunication,
  uiRtcSubscription,
} from "../communication/contract";
import { Badge } from "react-bootstrap";

const AttributeDeclarationComponent = () => {
  const [status, setStatus] = useState(false);

  useEffect(() => {
    var correlationId = "AttributeDeclarationId";
    uiRtcSubscription.AttributeDeclaration.AttributeDeclarationAttributeAnswer(
      (model: SimpleResponseMessage) => {
        if (model.correlationId == correlationId) {
          setStatus(true);
        }
      }
    );

    const actFun = async () => {
      await uiRtcCommunication.AttributeDeclaration.AttributeDeclarationAttributeHandler(
        { correlationId: correlationId }
      );
    };
    actFun();
  }, []);

  return (
    <tr>
      <td>AttributeDeclaration Hub Test</td>
      <td className="text-center">
        <Badge bg={status ? "success" : "danger"}>
          {status ? "Success" : "Fail"}
        </Badge>
      </td>
    </tr>
  );
};

export default AttributeDeclarationComponent;
