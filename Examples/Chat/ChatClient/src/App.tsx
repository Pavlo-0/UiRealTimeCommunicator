import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import { Container, Row } from "react-bootstrap";
import ChatComponent from "./components/chatComponent";
import ChatLogin from "./components/ChatLogin";
import { useState } from "react";

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
