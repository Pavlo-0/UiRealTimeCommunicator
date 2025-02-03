import React, { useState, useEffect } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
} from "../../communication/contract";
import { Button, Card, Form, ListGroup } from "react-bootstrap";
import "./RandomNumberComponent.css"; // Create a custom CSS file for styles

const RandomNumberComponent = () => {
  const [customNumber, setCustomNumber] = useState(0);
  const [minValue, setMinValue] = useState(0);
  const [maxValue, setMaxValue] = useState(100);

  useEffect(() => {
    uiRtcSubscription.RandomHub.SendRandomNumberMessage((data) => {
      setCustomNumber(data.value);
    });
  }, []);

  const generateNewNumber = () => {
    uiRtcCommunication.RandomHub.GenerateNewNumberHandler();
    //serverConnection.current?.send("GenerateNewNumber");
  };

  const generateNewRangeNumber = () => {
    if (minValue < maxValue) {
      uiRtcCommunication.RandomHub.GenerateNewRangeNumberHandler({
        minValue,
        maxValue,
      });
    } else {
      alert("Min value must be less than Max value.");
    }
  };

  return (
    <div className="random-number-container">
      <Card className="random-number-card">
        <Card.Title className="text-center">Generate Random Number</Card.Title>
        <Card.Text className="text-center">
          Random Number: {customNumber}
        </Card.Text>

        <ListGroup className="list-group-flush">
          <ListGroup.Item className="text-center">
            <Button variant="primary" size="lg" onClick={generateNewNumber}>
              Generate Number
            </Button>
          </ListGroup.Item>
          <ListGroup.Item>
            <Form.Group className="d-flex justify-content-center align-items-center">
              <Form.Label className="me-2 mb-0">Min Value</Form.Label>
              <Form.Control
                style={{ width: "100px" }}
                type="number"
                value={minValue}
                onChange={(e) => setMinValue(Number(e.target.value))}
              />
              <Form.Label className="mx-2 mb-0">Max Value</Form.Label>
              <Form.Control
                style={{ width: "100px" }}
                type="number"
                value={maxValue}
                onChange={(e) => setMaxValue(Number(e.target.value))}
              />
            </Form.Group>
          </ListGroup.Item>
          <ListGroup.Item className="text-center">
            <Button
              variant="primary"
              size="lg"
              onClick={generateNewRangeNumber}
            >
              Generate Number (Within Range)
            </Button>
          </ListGroup.Item>
        </ListGroup>
      </Card>
    </div>
  );
};

export default RandomNumberComponent;
