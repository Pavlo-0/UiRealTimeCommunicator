import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import { Col, Container, Row, Tab, Tabs } from "react-bootstrap";
import RandomNumberComponent from "./components/RandomNumber/RandomNumberComponent";
import { WeatherComponent } from "./components/WeatherComponent/WeatherComponent";

function App() {
  return (
    <>
      <Container>
        <Row>
          <Col md={{ span: 6, offset: 3 }}>
            <Tabs>
              <Tab eventKey="randomNumberTab" title="Random Number">
                <RandomNumberComponent></RandomNumberComponent>
              </Tab>
              <Tab eventKey="WeatherTab" title="Weather">
                <WeatherComponent></WeatherComponent>
              </Tab>
            </Tabs>
          </Col>
        </Row>
      </Container>
    </>
  );
}

export default App;
