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

```sh
dotnet add package UiRealTimeCommunicator
```

Additionally, install the **CLI tool** for generating TypeScript code:

```sh
dotnet tool install --global UiRealTimeCommunicator.TypeScriptGenerator
# or update to the latest version
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator
```

#### Client-Side (TypeScript)

Once you have generated the TypeScript code, you will be able to use the contract and subscription models in your TypeScript frontend.

### 💻 Usage Example:

#### Server-Side (C#)

In your `Program.cs`, configure and add the **UiRealTimeCommunicator** to your services:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddUiRealTimeCommunicator(); // Add the service to DI container
...
var app = builder.Build();
app.UseUiRealTimeCommunicator(); // Enable real-time communication in the app pipeline
```

Define a SignalR **Hub** for communication:

```csharp
[UiRtcHub("Weather")] // Declare a SignalR Hub with a specific name (Weather)
public class WeatherHub : IUiRtcHub { }
```

Define the **sender contract** for sending messages to the frontend:

```csharp
public interface WeatherChannelSenderContract: IUiRtcSenderContract<WeatherHub>
{
    // Declare method and data for sending message to frontend
    [UiRtcMethod("WeatherForecast")]
    Task SendWeatherForecast(WeatherForecastModel forecast);
}
```

In your service, use the **sender contract** to send messages:

```csharp
public class WeatherService(ISenderService senderService)
{
    public async Task WeatherServiceMethod(WeatherForecastModel model)
    {
        // Send message to frontend with strongly-typed contract
        await senderService.Send<WeatherChannelSenderContract>().SendWeatherForecast(model);
    }
}
```

Define the **handler contract** to receive messages from the frontend:

```csharp
[UiRtcMethod("GetWeatherForecast")]
public class GetWeatherForecastHandler() : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
{
    public async Task ConsumeAsync(WeatherForecastRequestModel model)
    {
        // Handle message from frontend
        // Process the incoming request
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
dotnet-uirtc -p ".\App-backend\App-backend.csproj" -o ".\app-frontend\src\communication\contract.ts"
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

await uiRtc.init({
  serverUrl: "http://localhost:5064/", // Your server URL
  activeHubs: "All", // Specify which hubs to subscribe to
});
```

Send a message from the frontend:

```typescript
import {
  uiRtcCommunication,
  WeatherForecastRequestModel,
} from "../../communication/contract";

// Call a backend method and send a strongly-typed model
uiRtcCommunication.Weather.GetWeatherForecast({
  city: "Kharkiv",
} as WeatherForecastRequestModel); // Strongly typed
```

Subscribe to a message and handle the response:

```typescript
import {
  uiRtcSubscription,
  WeatherForecastResponseModel,
} from "../../communication/contract";

// Listen for messages from the backend
uiRtcSubscription.Weather.WeatherForecast(
  (data: WeatherForecastResponseModel) => {
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
