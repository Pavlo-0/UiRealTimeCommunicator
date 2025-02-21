﻿/* 
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
const allHubs: uiRtcHubs[] = [
  {{ALL_HUBS}}
];

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
