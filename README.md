## UiRealTimeCommunicator

**UiRealTimeCommunicator** is a NuGet library that enables seamless **strongly-typed message exchange** between a **C# .NET 8 server-side application** and a **TypeScript client-side application** using **SignalR**. It simplifies WebSocket-based communication by providing strict type safety and an intuitive API.

### 🚀 Features:

- 🏗 **Strongly-typed messages** ensuring compile-time safety.
- 📡 **WebSocket communication** via SignalR for real-time interactions.
- 🔄 **Automatic serialization/deserialization** between client and server.
- 📦 **Flexible and extensible** for various real-time scenarios.

### 🛠 Installation:

#### Server-Side (C# .NET 8)

Install via NuGet:

```sh
dotnet add package UiRealTimeCommunicator
```

Install CLI package for generate TypeScript code:

```sh
dotnet tool install --global UiRealTimeCommunicator.TypeScriptGenerator
#or update
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator
```

### 💻 Usage Example:

#### Server-Side (C#)

Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddUiRealTimeCommunicator();
...
var app = builder.Build();
app.UseUiRealTimeCommunicator();
```

Hub (exchange point)

```csharp
 [UiRtcHub("Weather")] //Declare hub (SignalR hub)
 public class WeatherHub : IUiRtcHub { }
```

Send - Contract

```csharp
 public interface WeatherChannelSenderContract: IUiRtcSenderContract<WeatherHub>
 {
    //Declare method and data for sending message to frontend here
    [UiRtcMethod("WeatherForecast")]
    Task SendWeatherForecast(WeatherForecastModel forecast);
 }
```

Send message

```csharp
public class WeatherService(ISenderService senderService)
{
    public async Task WetherServiceMethod(WeatherForecastModel model)
    {
        //Send message to frontEnd
        await senderService.Send<WeatherChannelSenderContract>().SendWeatherForecast(model); //Strong typed by contract !!
    }
}
```

Receive message

```csharp
[UiRtcMethod("GetWeatherForecast")]
public class GetWeatherForecastHandler() : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
{
    public async Task ConsumeAsync(WeatherForecastRequestModel model) {
        //Handle message from frontEnd
     }
}
```

Describe send and subscription model

```csharp
[TranspilationSource]
public class WeatherForecastModel
{
 //....
}

 [TranspilationSource]
 public class WeatherForecastRequestModel
 {
     //...
 }
```

#### Generate TypeScript code for client-side

```sh
# -p Path to the project file (Xxx.csproj)
# -o Output directory and file
dotnet-uirtc -p ".\App-backend\App-backend.csproj" -o ".\app-frontend\src\communication\contract.ts"
```

#### Client-Side (TypeScript)

```typescript
import { uiRtc } from "./communication/contract.ts";

await uiRtc.init({
  serverUrl: "http://localhost:5064/",
  activeHubs: "All",
});
```

Send message:

```typescript
import {
  uiRtcCommunication,
  WeatherForecastRequestModel,
} from "../../communication/contract";

uiRtcCommunication.Weather.GetWeatherForecast({
  city: "Kharkiv",
} as WeatherForecastRequestModel); //Strong typed!!!
```

Receive message

```typescript
import {
  uiRtcSubscription,
  WeatherForecastResponseModel,
} from "../../communication/contract";

uiRtcSubscription.Weather.WeatherForecast(
  (data: WeatherForecastResponseModel) => {
    //Handler from backend
  }
);
```

### 🎯 Why UiRealTimeCommunicator?

✅ Easy setup and integration  
✅ Reliable data exchange between client and server  
✅ Fully typed communication to reduce errors

⚡ **Start building real-time applications today!**
