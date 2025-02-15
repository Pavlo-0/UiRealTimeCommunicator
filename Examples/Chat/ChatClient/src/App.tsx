import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import { Container, Row } from "react-bootstrap";
import ChatLogin from "./components/ChatLogin";
import { useState } from "react";
import ChatComponent from "./components/ChatComponent";

function App() {
  const [isJoin, setIsJoin] = useState(false);

  return (
    <>
      <Container>
        <Row>
          {!isJoin && (
            <ChatLogin
              onJoin={() => {
                setIsJoin(true);
              }}
            ></ChatLogin>
          )}
          {isJoin && <ChatComponent></ChatComponent>}
        </Row>
      </Container>
    </>
  );
}

export default App;
