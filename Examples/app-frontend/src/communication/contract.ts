/* 
 * Auto-generated TypeScript File by UiRtc
 * Version: 1.0.
 * Generated on: 2025-02-08 03:06:18 UTC 
 * Do not modify this file manually.
 */
/* eslint-disable */
/* tslint:disable */

import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState
} from "@microsoft/signalr";

type uiRtcHubs = 'RandomNumberHub' | 'Weather';
const allHubs: uiRtcHubs[] = ['RandomNumberHub', 'Weather'];

type hubMethods = RandomNumberHubMethod | WeatherMethod;

type RandomNumberHubMethod = 'RequestNewNumber' | 'RequestNewRangeNumber';
type WeatherMethod = 'GetWeatherForecast';

type hubSubscriptions = RandomNumberHubSubscription | WeatherSubscription;

type RandomNumberHubSubscription = 'RandomNumber';
type WeatherSubscription = 'WeatherForecast';

const connections: RConnections = {
    RandomNumberHub: {},
  Weather: {},
};

export const uiRtcSubscription = {
    RandomNumberHub: {
    RandomNumber: (callBack: (data: RandomValueResponseModel) => void) => subscribe("RandomNumberHub", "RandomNumber", callBack),
  },
  Weather: {
    WeatherForecast: (callBack: (data: WeatherForecastResponseModel) => void) => subscribe("Weather", "WeatherForecast", callBack),
  },

};

export const uiRtcCommunication = {
    RandomNumberHub: {
    RequestNewNumber: () => send("RandomNumberHub", "RequestNewNumber"),
    RequestNewRangeNumber: (request: RandomRangeRequestModel) => send("RandomNumberHub", "RequestNewRangeNumber", request),
  },
  Weather: {
    GetWeatherForecast: (request: WeatherForecastRequestModel) => send("Weather", "GetWeatherForecast", request),
  },

};

/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from App_backend.Communication.RandomNumber.Models.RandomRangeRequestModel */
export type RandomRangeRequestModel = {
    /** Transpiled from int */
    minValue: number;
    /** Transpiled from int */
    maxValue: number;
}

/** Transpiled from App_backend.Communication.RandomNumber.Models.RandomValueResponseModel */
export type RandomValueResponseModel = {
    /** Transpiled from int */
    value: number;
}



/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from App_backend.Communication.WeatherChannel.Models.WeatherForecastDetail */
export type WeatherForecastDetail = {
    /** Transpiled from System.DateTime */
    date: (Date | string);
    /** Transpiled from int */
    temperatureC: number;
    /** Transpiled from int */
    temperatureF: number;
    /** Transpiled from string? */
    summary?: string;
}

/** Transpiled from App_backend.Communication.WeatherChannel.Models.WeatherForecastRequestModel */
export type WeatherForecastRequestModel = {
    /** Transpiled from string */
    city: string;
}

/** Transpiled from App_backend.Communication.WeatherChannel.Models.WeatherForecastResponseModel */
export type WeatherForecastResponseModel = {
    /** Transpiled from System.Collections.Generic.IEnumerable<App_backend.Communication.WeatherChannel.Models.WeatherForecastDetail> */
    weatherForecast: WeatherForecastDetail[];
    /** Transpiled from string */
    city: string;
}



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