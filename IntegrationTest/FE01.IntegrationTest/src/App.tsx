import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";
import { Col, Container, Row, Tab, Table, Tabs } from "react-bootstrap";
import SimpleComponent from "./components/SimpleComponent";
import AttributeDeclarationComponent from "./components/AttributeDeclaration";
import TwoContractsComponent from "./components/TwoContractsComponent";
import SimpleContextComponent from "./components/SimpleContextComponent";
import SimpleEmptyComponent from "./components/SimpleEmptyComponent";

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
            <AttributeDeclarationComponent></AttributeDeclarationComponent>
            <TwoContractsComponent></TwoContractsComponent>
          </tbody>
        </Table>
      </Container>
    </>
  );
}

export default App;
