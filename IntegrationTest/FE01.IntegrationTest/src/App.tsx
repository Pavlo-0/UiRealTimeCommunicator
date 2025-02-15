import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import { Col, Container, Row, Tab, Table, Tabs } from "react-bootstrap";
import SimpleComponent from "./components/SimpleComponent";
import AttributeDeclarationComponent from "./components/AttributeDeclaration";
import TwoContractsComponent from "./components/TwoContractMethodsComponent";
import SimpleContextComponent from "./components/SimpleContextComponent";
import SimpleEmptyComponent from "./components/SimpleEmptyComponent";
import OnConnectionComponent from "./components/OnConnectionComponent";
import TwoHandlersComponent from "./components/TwoHandlersComponent";
import ConnectionIdSenderComponent from "./components/ConnectionIdSenderComponent";
import SimpleEmptyContextComponent from "./components/SimpleEmptyContextComponent";
import TwoSubscriptionComponent from "./components/TwoSubscriptionComponent";

function App() {
  return (
    <>
      <Container className="mt-3">
        <Table striped bordered hover>
          <thead>
            <tr>
              <th>Tests</th>
              <th className="text-center">Status</th>
            </tr>
          </thead>
          <tbody>
            <SimpleEmptyComponent></SimpleEmptyComponent>
            <SimpleComponent></SimpleComponent>
            <SimpleContextComponent></SimpleContextComponent>
            <SimpleEmptyContextComponent></SimpleEmptyContextComponent>
            <AttributeDeclarationComponent></AttributeDeclarationComponent>
            <TwoContractsComponent></TwoContractsComponent>
            <OnConnectionComponent></OnConnectionComponent>
            <TwoHandlersComponent></TwoHandlersComponent>
            <TwoSubscriptionComponent></TwoSubscriptionComponent>
            <ConnectionIdSenderComponent></ConnectionIdSenderComponent>
          </tbody>
        </Table>
      </Container>
    </>
  );
}

export default App;
