/* 
 * Auto-generated TypeScript File by UiRtc
 * Version: {{VERSION}}
 * Generated on: {{TIMESTAMP}} 
 * Do not modify this file manually.
 */
/* eslint-disable */
/* tslint:disable */

import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState
} from "@microsoft/signalr";

type uiRtcHubs = {{HUBS}};
const allHubs: uiRtcHubs[] = [{{ALL_HUBS}}];

type hubMethods = {{HUB_METHODS}};

{{HUB_METHOD_DEFINITIONS}}

type hubSubscriptions = {{HUB_SUBSCRIPTIONS}};

{{HUB_SUBSCRIPTION_DEFINITIONS}}

const connections: RConnections = {
  {{CONNECTIONS}}
};

export const uiRtcSubscription = {
  {{UI_RTC_SUBSCRIPTION}}
};

export const uiRtcCommunication = {
  {{UI_RTC_COMMUNICATION}}
};

{{MODELS}}

/* Hard code */
export interface IUiRtcConfiguration {
  serverUrl: string;
  activeHubs: uiRtcHubs[] | "All";
}

interface IHub {
  connection?: HubConnection;
}

type RConnections = Record<uiRtcHubs, IHub>;

export const uiRtc = {
  init: async (config: IUiRtcConfiguration) => {
    const hubsToInitialize =
      config.activeHubs === "All" ? allHubs : config.activeHubs;
    hubsToInitialize.forEach((hub) => initHub(config.serverUrl, hub));
  },
  dispose: (hubs: uiRtcHubs[] | "All" | undefined) => {
    const hubsToInitialize =
      hubs === "All" || hubs === undefined ? allHubs : hubs;
    hubsToInitialize.forEach((hub) => disposeHub(hub));
  },
};

const initHub = async (serverUrl: string, hubName: uiRtcHubs) => {
  if (!!connections[hubName].connection) {
    console.warn(hubName + " hub has been initialized already");
    return;
  }

  try {
    connections[hubName].connection = buildConnection(serverUrl + hubName);
    await connections[hubName].connection.start();
  } catch (err) {
    console.error(
      "Error while establishing connection '" + hubName + "': ",
      err
    );
  }
};

const buildConnection = (url: string) => {
  let builder = new HubConnectionBuilder();
  builder.withUrl(url);
  builder.withAutomaticReconnect();

  return builder.build();
};

const disposeHub = (hubName: uiRtcHubs) => {
  if (
    !!connections[hubName].connection &&
    (connections[hubName].connection.state === HubConnectionState.Connected ||
      connections[hubName].connection.state === HubConnectionState.Connecting ||
      connections[hubName].connection.state === HubConnectionState.Reconnecting)
  ) {
    try {
      connections[hubName].connection.stop();
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

const subscribe = (
  hub: uiRtcHubs,
  sub: hubSubscriptions,
  callBack: (data: any) => void
) => {
  connections[hub].connection?.on(sub, callBack);
};

const send = (hub: uiRtcHubs, method: hubMethods, request?: any) => {
  if (!!request) {
    connections[hub].connection?.send(method, request);
  } else {
    connections[hub].connection?.send(method);
  }
};