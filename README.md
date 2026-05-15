## UiRealTimeCommunicator

**UiRealTimeCommunicator** is a NuGet library designed to enable seamless **strongly-typed message exchange** between a **C# .NET 8 server-side application** and a **TypeScript client-side application** using **SignalR**. This library simplifies WebSocket-based communication by providing strict type safety and an intuitive API, making it easy to implement real-time features like live updates, notifications, and interactive communication.

### 🚀 Features:

- 🏗 **Strongly-typed messages**: Enforces compile-time safety and ensures that communication between client and server is consistent.
- 🧑‍💻 **Code generation**: Automatically generates TypeScript models and contracts from the server-side code, reducing boilerplate and ensuring synchronization between client and server code.
- 🔐 **Type safety on both sides**: Guarantees that the TypeScript client-side code mirrors the server-side contracts, eliminating runtime errors due to mismatched data structures.
- 📡 **WebSocket communication**: Uses SignalR to enable fast and reliable real-time communication over WebSockets.
- 🔄 **Automatic serialization/deserialization**: Automatically serializes and deserializes messages, allowing developers to focus on business logic rather than manually handling data transformations.
- ⚙️ **Flexible and extensible**: Supports various real-time messaging scenarios, including notifications, data streaming, and event-driven architectures.

### 🛠 Installation:

#### Server-Side (C# .NET 8)

Install the **UiRealTimeCommunicator** library via NuGet:   
[![NuGet](https://img.shields.io/nuget/v/UiRealTimeCommunicator.svg)](https://www.nuget.org/packages/UiRealTimeCommunicator/)

```sh
dotnet add package UiRealTimeCommunicator
```

Additionally, install the **CLI tool** for generating TypeScript code:   
[![NuGet](https://img.shields.io/nuget/v/UiRealTimeCommunicator.TypeScriptGenerator.svg)](https://www.nuget.org/packages/UiRealTimeCommunicator.TypeScriptGenerator/)  

```sh
dotnet tool install --global UiRealTimeCommunicator.TypeScriptGenerator
# or update to the latest version
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator
```

#### Client-Side (TypeScript)

Once you have generated the TypeScript code, you will be able to use the contract and subscription models in your TypeScript frontend.

### 💻 Usage Example:

For a full walkthrough and more scenarios, see the [Full Usage Guide](docs/usage-guide.md).

#### Server-Side (C#)

In your `Program.cs`, register the services and middleware:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddUiRealTimeCommunicator();

var app = builder.Build();
app.UseUiRealTimeCommunicator();
```

Define a SignalR **Hub** for communication:

```csharp
[UiRtcHub("Weather")] // Optional custom hub name
public class WeatherHub : IUiRtcHub { }
```

Define the **sender contract** for sending messages to the frontend:

```csharp
public interface WeatherChannelSenderContract : IUiRtcSenderContract<WeatherHub>
{
    [UiRtcMethod("WeatherForecast")] // Optional custom method name
    Task SendWeatherForecastAsync(WeatherForecastModel forecast);
}
```

In your service, use the **sender contract** to send messages:

```csharp
public class WeatherService(IUiRtcSenderService senderService)
{
    public async Task WeatherServiceMethod(WeatherForecastModel model)
    {
        await senderService.Send<WeatherChannelSenderContract>().SendWeatherForecastAsync(model);
    }
}
```

Define the **handler contract** to receive messages from the frontend:

```csharp
public class GetWeatherForecastHandler() : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
{
    public async Task ConsumeAsync(WeatherForecastRequestModel model)
    {
        // Handle message from frontend
    }
}
```

Define your data models and mark them for TypeScript code generation:

```csharp
[TranspilationSource]
public class WeatherForecastModel
{
    public string City { get; set; }
    public double Temperature { get; set; }
    // Add additional fields
}

[TranspilationSource]
public class WeatherForecastRequestModel
{
    public string City { get; set; }
}
```

We utilize **Tapper** for model generation. For more details, visit the [Tapper GitHub repository](https://github.com/nenoNaninu/Tapper) (external link).

#### Generate TypeScript Code for Client-Side

Use the CLI tool to generate TypeScript models and contracts from your C# project:

```sh
# -p Path to the project file (Xxx.csproj)
# -o Output directory and file
dotnet-uirtc -p ".\Examples\Simple\App-backend\App-backend.csproj" -o ".\Examples\Simple\app-frontend\src\communication\contract.ts"
```

#### Client-Side (TypeScript)

### Install SignalR

Install **SignalR** in your TypeScript project. You can find more details on the [SignalR npm page](https://www.npmjs.com/package/@microsoft/signalr) (external link).

```bash
npm install @microsoft/signalr
# or
yarn add @microsoft/signalr
```

In the TypeScript client, initialize the **SignalR connection**:

```typescript
import { uiRtc } from "./communication/contract.ts";

await uiRtc.initAsync({
  serverUrl: "http://localhost:5064/", // Base URL of the server
  activeHubs: "All", // "All" or a list of hub names
});
```

#### JWT / Bearer Authentication

Generated clients support SignalR's `accessTokenFactory` for authenticated hub connections:

```typescript
await uiRtc.initAsync({
  serverUrl: "https://localhost:5001/",
  activeHubs: "All",
  accessTokenFactory: () => authService.getAccessToken(),
});
```

The token factory can be async, which is useful when the client refreshes tokens before connecting:

```typescript
await uiRtc.initAsync({
  serverUrl: "https://localhost:5001/",
  activeHubs: "All",
  accessTokenFactory: async () => {
    return await authService.getValidAccessToken();
  },
});
```

Advanced SignalR options can be passed through `connectionOptions`:

```typescript
await uiRtc.initAsync({
  serverUrl: "https://localhost:5001/",
  activeHubs: "All",
  connectionOptions: {
    accessTokenFactory: () => authService.getAccessToken(),
    withCredentials: false,
  },
});
```

For multiple hubs, per-hub options override global options:

```typescript
await uiRtc.initAsync({
  serverUrl: "https://localhost:5001/",
  activeHubs: ["Chat", "Weather"],
  hubConnectionOptions: {
    Chat: {
      accessTokenFactory: () => chatTokenService.getToken(),
    },
    Weather: {
      accessTokenFactory: () => weatherTokenService.getToken(),
    },
  },
});
```

Server-side JWT Bearer authentication still has to be configured in ASP.NET Core. For WebSockets and Server-Sent Events, SignalR may transmit the token via the `access_token` query string, so configure `JwtBearerEvents.OnMessageReceived` for UiRtc hub paths.

Send a message from the frontend:

```typescript
import {
  uiRtcCommunication,
  WeatherForecastRequestModel,
} from "../../communication/contract";

// Call a backend method with a strongly-typed model
await uiRtcCommunication.Weather.GetWeatherForecast({
  city: "Kharkiv",
} as WeatherForecastRequestModel); // Strongly typed
```

Subscribe to a message and handle the response:

```typescript
import {
  uiRtcSubscription,
  WeatherForecastModel,
} from "../../communication/contract";

// Listen for messages from the backend
uiRtcSubscription.Weather.WeatherForecast(
  (data: WeatherForecastModel) => {
    // Handle received data
    console.log("Weather data received: ", data);
  }
);
```

### 🎯 Why UiRealTimeCommunicator?

- **✅ Easy setup and integration**: Simple installation and configuration for both server and client sides. No complicated setup or dependencies.
- **✅ Reliable data exchange**: Ensures high-quality, real-time data transfer using WebSockets and SignalR with strong type safety.
- **✅ Fully typed communication**: Both client and server-side code are strongly typed, reducing runtime errors and improving developer productivity.
- **✅ Automatic code generation**: The CLI tool automatically generates TypeScript models, ensuring consistency between server and client code and reducing boilerplate.
- **✅ Scalable architecture**: Ideal for building scalable real-time applications such as chat systems, live updates, and notifications.
- **✅ Reduces boilerplate code**: The framework abstracts away many manual tasks like serialization, deserialization, and message routing, allowing developers to focus on core functionality.

⚡ **Start building real-time applications today!** The library ensures a seamless and type-safe communication layer for your .NET and TypeScript applications.
