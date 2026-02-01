# TypeScript Generator Documentation

> **Update this file when:** Modifying CLI commands, changing template placeholders, or updating the code analysis logic.

## Overview

The TypeScript Generator (`dotnet-uirtc`) is a CLI tool that analyzes a C# project and generates TypeScript code for client-side communication.

## Installation

```bash
# Install globally
dotnet tool install --global UiRealTimeCommunicator.TypeScriptGenerator

# Update to latest
dotnet tool update --global UiRealTimeCommunicator.TypeScriptGenerator
```

## CLI Usage

```bash
dotnet-uirtc -p "path/to/project.csproj" -o "path/to/output/contract.ts"
```

### Options
| Option | Description |
|--------|-------------|
| `-p`, `--project` | Path to the C# project file (`.csproj`) |
| `-o`, `--output` | Output path for generated TypeScript file |

### Examples
```bash
# Generate to specific file
dotnet-uirtc -p "./Backend/Backend.csproj" -o "./frontend/src/api/contract.ts"

# Generate to directory (creates contract.ts)
dotnet-uirtc -p "./Backend/Backend.csproj" -o "./frontend/src/api/"
```

## How It Works

### Flow Diagram
```
???????????????????     ?????????????????????     ????????????????????
?  App.cs         ???????DataCollectorService???????TsGeneratorService?
?  (CLI Handler)  ?     ?  (Roslyn Analysis) ?     ?  (Code Output)   ?
???????????????????     ?????????????????????     ????????????????????
         ?                       ?                         ?
         ?                       ?                         ?
         ?              ???????????????????       ??????????????????
         ?              ? Tapper Library  ?       ? TsTemplate     ?
         ?              ? (Model Gen)     ?       ? v1.0.ts        ?
         ?              ???????????????????       ??????????????????
         ?                       ?                         ?
         ?                       ?                         ?
???????????????????????????????????????????????????????????????????
?                     Output Files                                 ?
?  - contract.ts (main contracts)                                  ?
?  - *.ts (individual model files from Tapper)                     ?
???????????????????????????????????????????????????????????????????
```

### Step 1: Model Generation (Tapper)
`DataCollectorService.TsModelGenerator()`:
1. Loads C# project using Roslyn
2. Uses Tapper library to find `[TranspilationSource]` classes
3. Generates TypeScript type definitions for each model
4. Saves as separate `.ts` files in output directory

### Step 2: Contract Extraction
`DataCollectorService.ExtractHubsContracts()`:
1. Scans for `IUiRtcSenderContract<>` implementations
2. Scans for `IUiRtcHandler<>` and `IUiRtcContextHandler<>` implementations
3. Extracts hub names, method names, and parameter types
4. Groups by hub name

### Step 3: TypeScript Generation
`TsGeneratorService.GenerateService()`:
1. Loads template from `Templates/TsTemplate.v1.0.ts`
2. Replaces placeholders with generated content
3. Writes main contract file

## Template System

### Template Location
`UiRtc.TypeScriptGenerator/Templates/TsTemplate.v1.0.ts`

### Placeholders

| Placeholder | Purpose | Example Output |
|-------------|---------|----------------|
| `{{VERSION}}` | Template version | `v1.0` |
| `{{TIMESTAMP}}` | Generation timestamp | `2024-01-15 10:30:00 UTC` |
| `{{MODEL_IMPORTS}}` | Import statements for models | `import { Model } from './Model';` |
| `{{HUBS}}` | Hub type union | `"Weather" \| "Chat"` |
| `{{ALL_HUBS}}` | Hub array | `"Weather", "Chat"` |
| `{{HUB_METHODS}}` | Method type union | `WeatherMethod \| ChatMethod` |
| `{{HUB_METHOD_DEFINITIONS}}` | Method type definitions | `type WeatherMethod = "GetForecast"` |
| `{{HUB_SUBSCRIPTIONS}}` | Subscription type union | `WeatherSubscription \| ChatSubscription` |
| `{{HUB_SUBSCRIPTION_DEFINITIONS}}` | Subscription definitions | `type WeatherSubscription = "Forecast"` |
| `{{CONNECTIONS}}` | Connection object initialization | `Weather: {}, Chat: {}` |
| `{{UI_RTC_SUBSCRIPTION}}` | Subscription methods | `Weather: { Forecast: (cb) => ... }` |
| `{{UI_RTC_COMMUNICATION}}` | Communication methods | `Weather: { GetForecast: (model) => ... }` |

### Template Structure
```typescript
// Auto-generated header with version and timestamp
import { HubConnection, ... } from "@microsoft/signalr";

{{MODEL_IMPORTS}}                    // Model imports

type uiRtcHubs = {{HUBS}};          // Hub type union
const allHubs = [{{ALL_HUBS}}];     // Hub array

type hubMethods = {{HUB_METHODS}};  // Method types
{{HUB_METHOD_DEFINITIONS}}           // Method type definitions

type hubSubscriptions = {{HUB_SUBSCRIPTIONS}};
{{HUB_SUBSCRIPTION_DEFINITIONS}}

const connections = { {{CONNECTIONS}} };

export const uiRtcSubscription = { {{UI_RTC_SUBSCRIPTION}} };
export const uiRtcCommunication = { {{UI_RTC_COMMUNICATION}} };

// Hard-coded utilities: uiRtc.initAsync, subscribe, send, etc.
```

## Code Analysis Details

### Finding Sender Contracts
`DataCollectorService` searches for interfaces implementing `IUiRtcSenderContract<THub>`:

```csharp
// This interface will be found
public interface IWeatherSender : IUiRtcSenderContract<WeatherHub>
{
    Task SendForecast(ForecastModel model);  // Becomes subscription
}
```

Extracted data:
- Hub name (from `THub` type argument)
- Method name (from interface method or `[UiRtcMethod]` attribute)
- Parameter type (for TypeScript type reference)

### Finding Handlers
`DataCollectorService` searches for classes implementing:
- `IUiRtcHandler<THub>`
- `IUiRtcHandler<THub, TModel>`
- `IUiRtcContextHandler<THub>`
- `IUiRtcContextHandler<THub, TModel>`

```csharp
// This class will be found
[UiRtcMethod("GetForecast")]
public class GetForecastHandler : IUiRtcHandler<WeatherHub, RequestModel>
{
    public Task ConsumeAsync(RequestModel model) { ... }
}
```

Extracted data:
- Hub name (from `THub` type argument)
- Method name (from class name or `[UiRtcMethod]` attribute)
- Parameter type (from `TModel` type argument)

### Name Resolution
Uses same logic as runtime `NameHelper`:

```csharp
// Hub name resolution
string GetHubNameFromAttributes(ITypeSymbol hubType):
  1. Check for [UiRtcHub("Name")] ? return attribute value
  2. Return type name

// Method name resolution
string GetMethodName(INamedTypeSymbol classSymbol):
  1. Check for [UiRtcMethod("Name")] ? return attribute value
  2. Return class name
```

## Generated TypeScript API

### Initialization
```typescript
import { uiRtc } from './contract';

// Initialize all hubs
await uiRtc.initAsync({
  serverUrl: 'http://localhost:5000/',
  activeHubs: 'All'
});

// Initialize specific hubs
await uiRtc.initAsync({
  serverUrl: 'http://localhost:5000/',
  activeHubs: ['Weather', 'Chat']
});
```

### Sending Messages (Client ? Server)
```typescript
import { uiRtcCommunication, RequestModel } from './contract';

// Send to handler
await uiRtcCommunication.Weather.GetForecast({ city: 'London' } as RequestModel);

// Handler without parameters
await uiRtcCommunication.Weather.Refresh();
```

### Subscribing to Messages (Server ? Client)
```typescript
import { uiRtcSubscription, ForecastModel } from './contract';

// Subscribe to sender method
const subscription = uiRtcSubscription.Weather.Forecast((data: ForecastModel) => {
  console.log('Received:', data);
});

// Unsubscribe
subscription.unsubscribe();
```

### Connection Management
```typescript
// Get raw SignalR connection
const connection = uiRtc.getConnection('Weather');

// Dispose connections
await uiRtc.disposeAsync('All');
await uiRtc.disposeAsync(['Weather']);
```

## Troubleshooting

### Common Issues

**Issue**: Models not appearing in generated TypeScript
**Solution**: Ensure models have `[TranspilationSource]` attribute

**Issue**: Hub not found in generated code
**Solution**: Ensure hub class implements `IUiRtcHub`

**Issue**: Method name incorrect
**Solution**: Check `[UiRtcMethod]` attribute or class/method naming

**Issue**: Parameter type shows as `any`
**Solution**: Ensure parameter type has `[TranspilationSource]` attribute

### Debugging
1. Run with verbose logging to see analysis progress
2. Check if project builds successfully before generation
3. Verify all referenced projects are accessible
