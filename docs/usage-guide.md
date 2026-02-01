# UiRealTimeCommunicator Usage Guide

This guide provides detailed, end-to-end examples for using UiRealTimeCommunicator in real applications.

## Table of Contents

- [Server Setup](#server-setup)
- [Define Hubs](#define-hubs)
- [Server-to-Client Messaging (Sender Contracts)](#server-to-client-messaging-sender-contracts)
- [Client-to-Server Messaging (Handlers)](#client-to-server-messaging-handlers)
- [Handlers with Context](#handlers-with-context)
- [Connection Lifecycle Events](#connection-lifecycle-events)
- [Sending to Specific Connections](#sending-to-specific-connections)
- [Model Generation](#model-generation)
- [TypeScript Client Usage](#typescript-client-usage)
- [Multiple Hubs](#multiple-hubs)
- [Custom Hub and Method Names](#custom-hub-and-method-names)
- [Troubleshooting](#troubleshooting)

## Server Setup

Install the package:

```sh
dotnet add package UiRealTimeCommunicator
```

Register the services and middleware:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUiRealTimeCommunicator();

var app = builder.Build();

app.UseUiRealTimeCommunicator();

app.Run();
```

## Define Hubs

Hubs are marker classes that identify a communication channel.

```csharp
public class WeatherHub : IUiRtcHub
{
}
```

By default, the hub name is the class name (`WeatherHub`). You can override it with `[UiRtcHub]` (see [Custom Hub and Method Names](#custom-hub-and-method-names)).

## Server-to-Client Messaging (Sender Contracts)

Define a sender contract interface. Each method becomes a TypeScript subscription.

```csharp
public interface WeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task WeatherForecast(WeatherForecastModel forecast);
    Task WeatherAlert(WeatherAlertModel alert);
}
```

Use the sender contract in your services:

```csharp
public class WeatherService(IUiRtcSenderService senderService)
{
    public async Task PublishForecastAsync(WeatherForecastModel model)
    {
        await senderService.Send<WeatherSender>().WeatherForecast(model);
    }

    public async Task PublishAlertAsync(WeatherAlertModel model)
    {
        await senderService.Send<WeatherSender>().WeatherAlert(model);
    }
}
```

## Client-to-Server Messaging (Handlers)

Handlers receive messages from the client. Each handler class becomes a TypeScript communication method.

```csharp
[UiRtcMethod("GetWeatherForecast")]
public class GetWeatherForecastHandler : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
{
    public async Task ConsumeAsync(WeatherForecastRequestModel model)
    {
        // Handle request from client
    }
}
```

## Handlers with Context

If you need connection IDs or other SignalR context, use `IUiRtcContextHandler`:

```csharp
public class GetWeatherWithContextHandler : IUiRtcContextHandler<WeatherHub, WeatherForecastRequestModel>
{
    public async Task ConsumeAsync(WeatherForecastRequestModel model, IUiRtcProxyContext context)
    {
        var connectionId = context.ConnectionId;
        // Use connectionId if needed
    }
}
```

## Connection Lifecycle Events

Implement `IUiRtcConnection<THub>` to receive connection events:

```csharp
public class WeatherConnectionHandler : IUiRtcConnection<WeatherHub>
{
    public Task OnConnectedAsync(string connectionId)
    {
        // Handle client connection
        return Task.CompletedTask;
    }

    public Task OnDisconnectedAsync(string connectionId, Exception? exception)
    {
        // Handle client disconnection
        return Task.CompletedTask;
    }
}
```

## Sending to Specific Connections

You can target specific connections when sending messages:

```csharp
public class WeatherService(IUiRtcSenderService senderService)
{
    public async Task SendToSpecificClientAsync(string connectionId, WeatherForecastModel model)
    {
        await senderService.Send<WeatherSender>(connectionId).WeatherForecast(model);
    }

    public async Task SendToMultipleClientsAsync(string[] connectionIds, WeatherAlertModel model)
    {
        await senderService.Send<WeatherSender>(connectionIds).WeatherAlert(model);
    }
}
```

## Model Generation

Mark models with `[TranspilationSource]` so they are generated into TypeScript.

```csharp
[TranspilationSource]
public class WeatherForecastModel
{
    public required string City { get; set; }
    public required double Temperature { get; set; }
    public string? Summary { get; set; }
}

[TranspilationSource]
public class WeatherAlertModel
{
    public required string Level { get; set; }
    public required string Message { get; set; }
}

[TranspilationSource]
public class WeatherForecastRequestModel
{
    public required string City { get; set; }
}
```

## TypeScript Client Usage

Install SignalR:

```sh
npm install @microsoft/signalr
```

Initialize connections:

```typescript
import { uiRtc } from "./communication/contract";

await uiRtc.initAsync({
  serverUrl: "http://localhost:5064/",
  activeHubs: "All",
});
```

Send messages to the server:

```typescript
import { uiRtcCommunication, WeatherForecastRequestModel } from "./communication/contract";

await uiRtcCommunication.Weather.GetWeatherForecast({
  city: "Kharkiv",
} as WeatherForecastRequestModel);
```

Subscribe to messages from the server:

```typescript
import { uiRtcSubscription, WeatherForecastModel } from "./communication/contract";

const subscription = uiRtcSubscription.Weather.WeatherForecast(
  (data: WeatherForecastModel) => {
    console.log("Forecast:", data);
  }
);

// Unsubscribe when needed
subscription.unsubscribe();
```

## Multiple Hubs

You can define multiple hubs and use them independently.

```csharp
public class ChatHub : IUiRtcHub { }
public class WeatherHub : IUiRtcHub { }

public interface ChatSender : IUiRtcSenderContract<ChatHub>
{
    Task Message(ChatMessageModel message);
}

public interface WeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task Forecast(WeatherForecastModel forecast);
}
```

TypeScript initialization can target all hubs or specific ones:

```typescript
await uiRtc.initAsync({
  serverUrl: "http://localhost:5064/",
  activeHubs: ["Chat", "Weather"],
});
```

## Custom Hub and Method Names

Use attributes to customize hub and method names.

```csharp
[UiRtcHub("Weather")]
public class WeatherHub : IUiRtcHub { }

[UiRtcMethod("GetForecast")]
public class GetWeatherHandler : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
{
    public Task ConsumeAsync(WeatherForecastRequestModel model)
    {
        return Task.CompletedTask;
    }
}

public interface WeatherSender : IUiRtcSenderContract<WeatherHub>
{
    [UiRtcMethod("Forecast")]
    Task SendForecast(WeatherForecastModel forecast);
}
```

## Troubleshooting

### Hub or method not found in TypeScript output
- Ensure hub classes implement `IUiRtcHub`
- Ensure handler classes implement `IUiRtcHandler<THub>` or `IUiRtcHandler<THub, TModel>`
- Ensure sender contracts implement `IUiRtcSenderContract<THub>`

### Model types show as `any`
- Ensure models are marked with `[TranspilationSource]`
- Ensure model types are in the project being transpiled

### Connection errors on the client
- Check `serverUrl` includes the correct base URL
- Ensure you call `uiRtc.initAsync()` before sending or subscribing

## Generate TypeScript Code

Install the generator tool:

```sh
dotnet tool install --global UiRealTimeCommunicator.TypeScriptGenerator
```

Generate code:

```sh
dotnet-uirtc -p "./YourProject/YourProject.csproj" -o "./frontend/src/communication/contract.ts"
```

If the output path is a directory, the file `contract.ts` is created automatically.
