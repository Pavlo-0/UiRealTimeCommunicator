# Code Patterns and Conventions

> **Update this file when:** Changing naming conventions, adding new patterns, or modifying implementation approaches.

## Naming Conventions

### Hub Naming
```csharp
// Default: Class name is used as hub name
public class WeatherHub : IUiRtcHub { }
// Hub name: "WeatherHub"

// Custom: Use attribute to specify name
[UiRtcHub("Weather")]
public class WeatherHub : IUiRtcHub { }
// Hub name: "Weather"
```

### Method Naming
```csharp
// For handlers - Default: Class name is used
public class GetWeatherHandler : IUiRtcHandler<WeatherHub, RequestModel> { }
// Method name: "GetWeatherHandler"

// For handlers - Custom: Use attribute
[UiRtcMethod("GetWeather")]
public class GetWeatherHandler : IUiRtcHandler<WeatherHub, RequestModel> { }
// Method name: "GetWeather"

// For sender contracts - Default: Method name is used
public interface IWeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task SendForecast(ForecastModel model);
}
// Method name: "SendForecast"

// For sender contracts - Custom: Use attribute
public interface IWeatherSender : IUiRtcSenderContract<WeatherHub>
{
    [UiRtcMethod("Forecast")]
    Task SendForecast(ForecastModel model);
}
// Method name: "Forecast"
```

## Common Patterns

### Pattern 1: Basic Hub + Handler + Sender

This is the most common pattern. One hub, one handler for receiving, one sender for sending.

```csharp
// 1. Define the hub (marker interface)
public class WeatherHub : IUiRtcHub { }

// 2. Define sender contract (server ? client)
public interface IWeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task SendForecast(ForecastModel model);
}

// 3. Define handler (client ? server)
public class GetForecastHandler(IUiRtcSenderService senderService) 
    : IUiRtcHandler<WeatherHub, ForecastRequest>
{
    public async Task ConsumeAsync(ForecastRequest model)
    {
        // Process request
        var forecast = await GetForecastAsync(model.City);
        
        // Send response via sender contract
        await senderService.Send<IWeatherSender>().SendForecast(forecast);
    }
}

// 4. Define models (marked for TypeScript generation)
[TranspilationSource]
public class ForecastRequest
{
    public required string City { get; set; }
}

[TranspilationSource]
public class ForecastModel
{
    public required string City { get; set; }
    public required double Temperature { get; set; }
}
```

### Pattern 2: Handler Without Parameters

For simple triggers that don't need data.

```csharp
public class RefreshHandler : IUiRtcHandler<WeatherHub>
{
    public async Task ConsumeAsync()
    {
        // No parameters, just a trigger
        await DoRefreshAsync();
    }
}
```

### Pattern 3: Handler With Context Access

When you need access to SignalR context (connection ID, etc.).

```csharp
public class ContextAwareHandler(IUiRtcSenderService senderService) 
    : IUiRtcContextHandler<WeatherHub, RequestModel>
{
    public async Task ConsumeAsync(RequestModel model, IUiRtcProxyContext context)
    {
        // Access connection ID
        var connectionId = context.ConnectionId;
        
        // Send only to the requesting client
        await senderService.Send<IWeatherSender>(connectionId)
            .SendForecast(forecast);
    }
}
```

### Pattern 4: Sending to Specific Connections

```csharp
// Send to all clients (broadcast)
await senderService.Send<IWeatherSender>().SendForecast(forecast);

// Send to specific connection(s)
await senderService.Send<IWeatherSender>(connectionId).SendForecast(forecast);
await senderService.Send<IWeatherSender>(connectionId1, connectionId2).SendForecast(forecast);
```

### Pattern 5: Connection Lifecycle Events

Handle client connect/disconnect events.

```csharp
public class WeatherConnectionHandler : IUiRtcConnection<WeatherHub>
{
    public Task OnConnectedAsync(string connectionId)
    {
        // Client connected
        return Task.CompletedTask;
    }

    public Task OnDisconnectedAsync(string connectionId, Exception? exception)
    {
        // Client disconnected
        return Task.CompletedTask;
    }
}
```

### Pattern 6: Multiple Subscriptions Per Hub

```csharp
public interface IWeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task SendForecast(ForecastModel model);
    Task SendAlert(AlertModel model);
    Task SendUpdate(UpdateModel model);
}

// TypeScript client will have:
// uiRtcSubscription.Weather.SendForecast(callback)
// uiRtcSubscription.Weather.SendAlert(callback)
// uiRtcSubscription.Weather.SendUpdate(callback)
```

## Model Conventions

### Required Properties
Use `required` keyword for mandatory properties:
```csharp
[TranspilationSource]
public class RequestModel
{
    public required string Id { get; set; }
    public string? OptionalField { get; set; }
}
```

### Collections
```csharp
[TranspilationSource]
public class ListModel
{
    public required List<string> Items { get; set; }
}
```

### DateTime Handling
DateTime is serialized as ISO 8601 string:
```csharp
[TranspilationSource]
public class TimedModel
{
    public required DateTime Timestamp { get; set; }
}
```

## Interface Conventions

### Sender Contracts
- Must inherit from `IUiRtcSenderContract<THub>`
- All methods must return `Task`
- Methods should have at most one parameter (the model)
- Use descriptive names (will become TypeScript method names)

### Handlers
- Must inherit from `IUiRtcHandler<THub>` or `IUiRtcHandler<THub, TModel>`
- For context access, use `IUiRtcContextHandler<THub>` or `IUiRtcContextHandler<THub, TModel>`
- Handler class names or `[UiRtcMethod]` attribute determine the TypeScript method name

## Internal Code Patterns

### Dynamic Type Generation
`SignalRHubBuilder` uses IL emit. When adding new features:
1. Understand the existing IL generation in `MethodBuild()`
2. Test with simple scenarios first
3. Use `ILGenerator` opcodes carefully

### Name Resolution Flow
Always use `NameHelper` for name resolution:
```csharp
// Get hub name from hub type
var hubName = NameHelper.GetHubName(hubType);

// Get hub name from contract/handler type
var hubName = NameHelper.GetHubNameByContract(contractType);

// Get method name from handler type
var methodName = NameHelper.GetMethodName(handlerType);
```

### Repository Pattern
Repositories are singletons holding registration data:
- `HubRepository` - Hub metadata
- `HandlerRepository` - Handler registrations  
- `ConnectionRepository` - Connection event handlers

## Anti-Patterns to Avoid

### Don't
```csharp
// ? Don't inherit hub from SignalR Hub directly
public class MyHub : Hub, IUiRtcHub { }

// ? Don't add methods to hub classes
public class MyHub : IUiRtcHub
{
    public Task SomeMethod() { } // Won't work
}

// ? Don't use void return type in sender contracts
public interface ISender : IUiRtcSenderContract<MyHub>
{
    void Send(Model m); // Must return Task
}

// ? Don't forget [TranspilationSource] on models
public class MyModel { } // Won't be generated to TypeScript
```

### Do
```csharp
// ? Hub is just a marker
public class MyHub : IUiRtcHub { }

// ? Use sender contracts for server?client
public interface ISender : IUiRtcSenderContract<MyHub>
{
    Task Send(Model m);
}

// ? Use handlers for client?server
public class MyHandler : IUiRtcHandler<MyHub, RequestModel>
{
    public Task ConsumeAsync(RequestModel model) { ... }
}

// ? Mark models for generation
[TranspilationSource]
public class MyModel { ... }
```
