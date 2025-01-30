import { Button, Card, Col, Container, Row } from "react-bootstrap";
import "./WeatherComponent.css"; // Create a custom CSS file for styles
import { useEffect, useState } from "react";
import {
  uiRtcCommunication,
  uiRtcSubscription,
  WeatherForecast,
  WeatherForecastResponseModel,
} from "../../communication/contract";

export const WeatherComponent = () => {
  const [weather, setWeather] = useState<WeatherForecast[]>([]);

  useEffect(() => {
    uiRtcSubscription.Weather.SendWeatherForecast(
      (data: WeatherForecastResponseModel) => {
        setWeather(data.weatherForecasts);
      }
    );
  }, []);

  const getForecast = () => {
    uiRtcCommunication.Weather.GetWeatherForecast({ city: "Kharkiv" });
  };

  return (
    <div className="weather-container">
      <Card className="weather-card">
        <Card.Title className="text-center">Weather</Card.Title>
        <Card.Text className="text-center">
          Weather:
          {weather.map((w: any) => {
            return (
              <Container>
                <Row>
                  <Col md={{ span: 5 }}>
                    <small>
                      {new Date(w.date).toLocaleDateString("en-US", {
                        month: "long",
                        day: "numeric",
                      })}
                    </small>
                  </Col>
                  <Col md={{ span: 3 }}>{w.temperatureC} C</Col>
                  <Col md={{ span: 4 }}>{w.summary}</Col>
                </Row>
              </Container>
            );
          })}
        </Card.Text>
        <Button variant="primary" size="lg" onClick={getForecast}>
          Forecast
        </Button>
      </Card>
    </div>
  );
};
