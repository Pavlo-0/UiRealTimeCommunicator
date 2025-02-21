/* 
 * Auto-generated TypeScript File by UiRtc
 * Version: 1.0.
 * Generated on: 2025-02-16 01:52:35 UTC 
 * Do not modify this file manually.
 */
/* eslint-disable */
/* tslint:disable */

import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState
} from "@microsoft/signalr";

type uiRtcHubs = "AttributeDeclaration"
  | "ConnectionIdSenderHub"
  | "OnConnectionHub"
  | "OnConnectionManager"
  | "SimpleContextHub"
  | "SimpleEmptyContextHub"
  | "SimpleEmptyHub"
  | "SimpleHub"
  | "TwoContractMethodsHub"
  | "TwoHandlers"
  | "TwoSubscriptionHub"
  | "UnsubscribeSimpleHub";
const allHubs: uiRtcHubs[] = [
  "AttributeDeclaration",
  "ConnectionIdSenderHub",
  "OnConnectionHub",
  "OnConnectionManager",
  "SimpleContextHub",
  "SimpleEmptyContextHub",
  "SimpleEmptyHub",
  "SimpleHub",
  "TwoContractMethodsHub",
  "TwoHandlers",
  "TwoSubscriptionHub",
  "UnsubscribeSimpleHub"
];

type hubMethods = AttributeDeclarationMethod
  | ConnectionIdSenderHubMethod
  | SimpleContextHubMethod
  | SimpleEmptyContextHubMethod
  | SimpleEmptyHubMethod
  | SimpleHubMethod
  | TwoContractMethodsHubMethod
  | TwoHandlersMethod
  | TwoSubscriptionHubMethod
  | UnsubscribeSimpleHubMethod;

type AttributeDeclarationMethod = "AttributeDeclarationAttributeHandler";
type ConnectionIdSenderHubMethod = "ConnectionIdRequest";
type SimpleContextHubMethod = "SimpleContextHandler";
type SimpleEmptyContextHubMethod = "SimpleEmptyContextHandler";
type SimpleEmptyHubMethod = "SimpleEmptyHandler";
type SimpleHubMethod = "SimpleHandler";
type TwoContractMethodsHubMethod = "TwoContractMethods";
type TwoHandlersMethod = "TwoHandler";
type TwoSubscriptionHubMethod = "TwoSubscriptionHandler";
type UnsubscribeSimpleHubMethod = "UnsubscribeSimpleHandler";

type hubSubscriptions = AttributeDeclarationSubscription
  | ConnectionIdSenderHubSubscription
  | OnConnectionHubSubscription
  | OnConnectionManagerSubscription
  | SimpleContextHubSubscription
  | SimpleEmptyContextHubSubscription
  | SimpleEmptyHubSubscription
  | SimpleHubSubscription
  | TwoContractMethodsHubSubscription
  | TwoHandlersSubscription
  | TwoSubscriptionHubSubscription
  | UnsubscribeSimpleHubSubscription;

type AttributeDeclarationSubscription = "AttributeDeclarationAttributeAnswer";
type ConnectionIdSenderHubSubscription = "SendToSpecificUser";
type OnConnectionHubSubscription = "DummyMethod";
type OnConnectionManagerSubscription = "UpdateStatus";
type SimpleContextHubSubscription = "SimpleContextAnswer";
type SimpleEmptyContextHubSubscription = "SimpleEmptyContextAnswer";
type SimpleEmptyHubSubscription = "SimpleEmptyAnswer";
type SimpleHubSubscription = "SimpleAnswer";
type TwoContractMethodsHubSubscription = "TwoContractMethodsAnswer1" | "TwoContractMethodsAnswer2";
type TwoHandlersSubscription = "HandlerAnswer";
type TwoSubscriptionHubSubscription = "TwoSubscriptionAnswer";
type UnsubscribeSimpleHubSubscription = "UnsubscribeSimpleAnswer";

const connections: RConnections = {
    AttributeDeclaration: {},
  ConnectionIdSenderHub: {},
  OnConnectionHub: {},
  OnConnectionManager: {},
  SimpleContextHub: {},
  SimpleEmptyContextHub: {},
  SimpleEmptyHub: {},
  SimpleHub: {},
  TwoContractMethodsHub: {},
  TwoHandlers: {},
  TwoSubscriptionHub: {},
  UnsubscribeSimpleHub: {},
};

export const uiRtcSubscription = {
    AttributeDeclaration: {
    AttributeDeclarationAttributeAnswer: (callBack: (data: AttributeDeclarationResponseMessage) => void) =>
      subscribe("AttributeDeclaration", "AttributeDeclarationAttributeAnswer", callBack),
  },
  ConnectionIdSenderHub: {
    SendToSpecificUser: (callBack: (data: InfoModel) => void) =>
      subscribe("ConnectionIdSenderHub", "SendToSpecificUser", callBack),
  },
  OnConnectionHub: {
    DummyMethod: (callBack: () => void) =>
      subscribe("OnConnectionHub", "DummyMethod", callBack),
  },
  OnConnectionManager: {
    UpdateStatus: (callBack: (data: OnConnectionStatusModel) => void) =>
      subscribe("OnConnectionManager", "UpdateStatus", callBack),
  },
  SimpleContextHub: {
    SimpleContextAnswer: (callBack: (data: SimpleContexResponseMessage) => void) =>
      subscribe("SimpleContextHub", "SimpleContextAnswer", callBack),
  },
  SimpleEmptyContextHub: {
    SimpleEmptyContextAnswer: (callBack: () => void) =>
      subscribe("SimpleEmptyContextHub", "SimpleEmptyContextAnswer", callBack),
  },
  SimpleEmptyHub: {
    SimpleEmptyAnswer: (callBack: () => void) =>
      subscribe("SimpleEmptyHub", "SimpleEmptyAnswer", callBack),
  },
  SimpleHub: {
    SimpleAnswer: (callBack: (data: SimpleResponseMessage) => void) =>
      subscribe("SimpleHub", "SimpleAnswer", callBack),
  },
  TwoContractMethodsHub: {
    TwoContractMethodsAnswer1: (callBack: (data: TwoContractMethodsResponseMessage) => void) =>
      subscribe("TwoContractMethodsHub", "TwoContractMethodsAnswer1", callBack),
    TwoContractMethodsAnswer2: (callBack: (data: TwoContractMethodsResponseMessage) => void) =>
      subscribe("TwoContractMethodsHub", "TwoContractMethodsAnswer2", callBack),
  },
  TwoHandlers: {
    HandlerAnswer: (callBack: (data: TwoHandlersResponse) => void) =>
      subscribe("TwoHandlers", "HandlerAnswer", callBack),
  },
  TwoSubscriptionHub: {
    TwoSubscriptionAnswer: (callBack: (data: TwoSubscriptionResponseMessage) => void) =>
      subscribe("TwoSubscriptionHub", "TwoSubscriptionAnswer", callBack),
  },
  UnsubscribeSimpleHub: {
    UnsubscribeSimpleAnswer: (callBack: () => void) =>
      subscribe("UnsubscribeSimpleHub", "UnsubscribeSimpleAnswer", callBack),
  },

};

export const uiRtcCommunication = {
    AttributeDeclaration: {
    AttributeDeclarationAttributeHandler: (request: AttributeDeclarationRequestMessage) =>
      send("AttributeDeclaration", "AttributeDeclarationAttributeHandler", request),
  },
  ConnectionIdSenderHub: {
    ConnectionIdRequest: () =>
      send("ConnectionIdSenderHub", "ConnectionIdRequest"),
  },
  SimpleContextHub: {
    SimpleContextHandler: (request: SimpleContexRequestMessage) =>
      send("SimpleContextHub", "SimpleContextHandler", request),
  },
  SimpleEmptyContextHub: {
    SimpleEmptyContextHandler: () =>
      send("SimpleEmptyContextHub", "SimpleEmptyContextHandler"),
  },
  SimpleEmptyHub: {
    SimpleEmptyHandler: () =>
      send("SimpleEmptyHub", "SimpleEmptyHandler"),
  },
  SimpleHub: {
    SimpleHandler: (request: SimpleRequestMessage) =>
      send("SimpleHub", "SimpleHandler", request),
  },
  TwoContractMethodsHub: {
    TwoContractMethods: (request: TwoContractMethodsRequestMessage) =>
      send("TwoContractMethodsHub", "TwoContractMethods", request),
  },
  TwoHandlers: {
    TwoHandler: () =>
      send("TwoHandlers", "TwoHandler"),
  },
  TwoSubscriptionHub: {
    TwoSubscriptionHandler: (request: TwoSubscriptionRequestMessage) =>
      send("TwoSubscriptionHub", "TwoSubscriptionHandler", request),
  },
  UnsubscribeSimpleHub: {
    UnsubscribeSimpleHandler: () =>
      send("UnsubscribeSimpleHub", "UnsubscribeSimpleHandler"),
  },

};

/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.AttributeDeclaration.AttributeDeclarationRequestMessage */
export type AttributeDeclarationRequestMessage = {
    /** Transpiled from string */
    correlationId: string;
}

/** Transpiled from BE01.IntegrationTest.Scenarios.AttributeDeclaration.AttributeDeclarationResponseMessage */
export type AttributeDeclarationResponseMessage = {
    /** Transpiled from string */
    correlationId: string;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.ConnectionIdSender.InfoModel */
export type InfoModel = {
    /** Transpiled from string? */
    connectionId?: string;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.OnConnection.OnConnectionStatusModel */
export type OnConnectionStatusModel = {
    /** Transpiled from bool */
    isConnected: boolean;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.SimpleContext.SimpleContexRequestMessage */
export type SimpleContexRequestMessage = {
    /** Transpiled from string */
    correlationId: string;
}

/** Transpiled from BE01.IntegrationTest.Scenarios.SimpleContext.SimpleContexResponseMessage */
export type SimpleContexResponseMessage = {
    /** Transpiled from string */
    correlationId: string;
    /** Transpiled from string */
    connectionId: string;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.Simple.SimpleRequestMessage */
export type SimpleRequestMessage = {
    /** Transpiled from string */
    correlationId: string;
}

/** Transpiled from BE01.IntegrationTest.Scenarios.Simple.SimpleResponseMessage */
export type SimpleResponseMessage = {
    /** Transpiled from string */
    correlationId: string;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.TwoContractMethods.TwoContractMethodsRequestMessage */
export type TwoContractMethodsRequestMessage = {
    /** Transpiled from string */
    correlationId: string;
}

/** Transpiled from BE01.IntegrationTest.Scenarios.TwoContractMethods.TwoContractMethodsResponseMessage */
export type TwoContractMethodsResponseMessage = {
    /** Transpiled from string */
    correlationId: string;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.TwoHandlers.TwoHandlersResponse */
export type TwoHandlersResponse = {
    /** Transpiled from int */
    handlerNumber: number;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from BE01.IntegrationTest.Scenarios.TwoSubscription.TwoSubscriptionRequestMessage */
export type TwoSubscriptionRequestMessage = {
    /** Transpiled from string */
    correlationId: string;
}

/** Transpiled from BE01.IntegrationTest.Scenarios.TwoSubscription.TwoSubscriptionResponseMessage */
export type TwoSubscriptionResponseMessage = {
    /** Transpiled from string */
    correlationId: string;
}



/* Hard code */
export interface IUiRtcConfiguration {
  serverUrl: string;
  activeHubs: uiRtcHubs[] | "All";
}

interface IHub {
  connection?: HubConnection;
  config?: IUiRtcConfiguration;
}

type RConnections = Record<uiRtcHubs, IHub>;

export const uiRtc = {
  initAsync: async (config: IUiRtcConfiguration) => {
    const hubsToInitialize =
      config.activeHubs === "All" ? allHubs : config.activeHubs;
    await Promise.all(hubsToInitialize.map((hub) => initHubAsync(config, hub)));
  },
  disposeAsync: async (hubs: uiRtcHubs[] | "All" | undefined) => {
    const hubsToInitialize =
      hubs === "All" || hubs === undefined ? allHubs : hubs;
    await Promise.all(hubsToInitialize.map((hub) => disposeHubAsync(hub)));
  },
  getConnection: (hub: uiRtcHubs) => {
    return connections[hub].connection;
  },
};

const initHubAsync = async (
  config: IUiRtcConfiguration,
  hubName: uiRtcHubs
) => {
  if (!!connections[hubName].connection) {
    console.warn(hubName + " hub has been initialized already");
    return;
  }

  connections[hubName].config = config;

  try {
    connections[hubName].connection = buildConnection(
      config.serverUrl + hubName
    );
    await connections[hubName].connection.start();
  } catch (err) {
    console.error(
      "Error while establishing connection '" + hubName + "': ",
      err
    );
  }
};

const buildConnection = (url: string): HubConnection => {
  let builder = new HubConnectionBuilder();
  builder.withUrl(url);
  builder.withAutomaticReconnect();

  return builder.build();
};

const disposeHubAsync = async (hubName: uiRtcHubs) => {
  if (isConnected(hubName)) {
    try {
      await connections[hubName].connection!.stop();
      connections[hubName] = {};
    } catch (err) {
      console.error(
        "Error while establishing connection '" + hubName + "': ",
        err
      );
    }
  } else {
    console.warn(hubName + " hub has not been initialized");
  }
};

const isConnected = (hubName: uiRtcHubs) => {
  if (
    !!connections &&
    !!connections[hubName] &&
    !!connections[hubName].connection &&
    (connections[hubName].connection.state === HubConnectionState.Connected ||
      connections[hubName].connection.state === HubConnectionState.Connecting ||
      connections[hubName].connection.state === HubConnectionState.Reconnecting)
  )
    return true;
  return false;
};

const subscribe = (
  hub: uiRtcHubs,
  sub: hubSubscriptions,
  callBack: (data: any) => void
) => {
  checkConnection(hub);
  connections[hub].connection?.on(sub, callBack);
  return {
    hub,
    sub,
    handler: callBack,
    unsubscribe: () => {
      connections[hub].connection?.off(sub, callBack);
    },
  };
};

const send = async (hub: uiRtcHubs, method: hubMethods, request?: any) => {
  checkConnection(hub);
  if (!!request) {
    await connections[hub].connection?.send(method, request);
  } else {
    await connections[hub].connection?.send(method);
  }
};

const checkConnection = (hub: uiRtcHubs) => {
  if (!connections[hub]?.connection) {
    throw new Error(
      "Connection to " +
      hub +
      " hasn't been established. Ensure you call await uiRtc.initAsync({ serverUrl: SERVER_URL, activeHubs: 'All' }) before using the hub."
    );
  }
};
