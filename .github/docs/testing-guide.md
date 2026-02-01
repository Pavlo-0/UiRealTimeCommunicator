# Testing Guide

> **Update this file when:** Adding new test scenarios, changing test patterns, or modifying the integration test setup.

## Test Projects Overview

| Project | Type | Framework | Purpose |
|---------|------|-----------|---------|
| `UiRtc.UnitTests` | Unit | MSTest | Unit tests for UiRtc library |
| `UiRtc.TypeScriptGenerator.UnitTests` | Unit | MSTest | Unit tests for TypeScript generator |
| `BE01.IntegrationTest` | Integration | - | Live server demonstrating all features |

## Integration Test Server

### Location
`IntegrationTest/BE01.IntegrationTest/`

### Purpose
The integration test server is a fully functional ASP.NET application that demonstrates and tests all library features. Each feature has its own "scenario" folder.

### Running the Server
```bash
cd IntegrationTest/BE01.IntegrationTest
dotnet run
```

### Scenario Structure
Each scenario is self-contained in its own folder:

```
IntegrationTest/BE01.IntegrationTest/Scenarios/
??? Simple/                     # Basic functionality
?   ??? SimpleHub.cs           # Hub, sender, handler, models in one file
??? SimpleContext/             # Context access
??? SimpleEmpty/               # Parameterless handlers
??? SimpleDateTimeField/       # DateTime serialization
??? AttributeDeclaration/      # Custom naming via attributes
??? TwoSubscription/           # Multiple subscriptions
??? OnConnection/              # Connection lifecycle
??? ConnectionId/              # Targeted messaging
??? UnsubscribeSimple/         # Unsubscribe functionality
```

## Scenario Examples

### Scenario: Simple (Basic Pattern)
**File**: `Scenarios/Simple/SimpleHub.cs`

Tests the most basic flow:
```csharp
// Hub marker
public class SimpleHub : IUiRtcHub { }

// Sender contract
public interface SimpleSender : IUiRtcSenderContract<SimpleHub>
{
    Task SimpleAnswer(SimpleResponseMessage message);
}

// Handler receives request, sends response
public class SimpleHandler(IUiRtcSenderService senderService) 
    : IUiRtcHandler<SimpleHub, SimpleRequestMessage>
{
    public async Task ConsumeAsync(SimpleRequestMessage Model)
    {
        await senderService.Send<SimpleSender>()
            .SimpleAnswer(new SimpleResponseMessage { CorrelationId = Model.CorrelationId });
    }
}

// Models with TranspilationSource for TypeScript generation
[TranspilationSource]
public class SimpleRequestMessage
{
    public required string CorrelationId { get; set; }
}

[TranspilationSource]
public class SimpleResponseMessage
{
    public required string CorrelationId { get; set; }
}
```

### Scenario: SimpleContext (Context Access)
**File**: `Scenarios/SimpleContext/SimpleContextHub.cs`

Tests access to SignalR context in handlers:
```csharp
public class ContextHandler : IUiRtcContextHandler<ContextHub, RequestModel>
{
    public async Task ConsumeAsync(RequestModel model, IUiRtcProxyContext context)
    {
        // Access connection ID and other context properties
        var connectionId = context.ConnectionId;
    }
}
```

### Scenario: SimpleEmpty (No Parameters)
**File**: `Scenarios/SimpleEmpty/SimpleEmptyHub.cs`

Tests handlers that don't receive any parameters:
```csharp
public class EmptyHandler : IUiRtcHandler<EmptyHub>
{
    public async Task ConsumeAsync()
    {
        // No parameters
    }
}
```

### Scenario: AttributeDeclaration (Custom Names)
**File**: `Scenarios/AttributeDeclaration/AttributeDeclarationHub.cs`

Tests custom hub and method naming via attributes:
```csharp
[UiRtcHub("CustomHubName")]
public class AttributeHub : IUiRtcHub { }

[UiRtcMethod("CustomMethodName")]
public class AttributeHandler : IUiRtcHandler<AttributeHub, Model>
{
    public Task ConsumeAsync(Model model) { ... }
}
```

### Scenario: OnConnection (Lifecycle Events)
**Files**: `Scenarios/OnConnection/OnConnectionHub.cs`, `OnConnectionManager.cs`

Tests connection lifecycle handling:
```csharp
public class ConnectionHandler : IUiRtcConnection<ConnectionHub>
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

### Scenario: ConnectionId (Targeted Messaging)
**File**: `Scenarios/ConnectionId/ConnectionIdSenderHub.cs`

Tests sending messages to specific connections:
```csharp
// In handler - send to specific connection
await senderService.Send<ISender>(specificConnectionId)
    .SendMessage(response);
```

### Scenario: TwoSubscription (Multiple Subscriptions)
**File**: `Scenarios/TwoSubscription/TwoSubscriptionHub.cs`

Tests multiple sender methods on one hub:
```csharp
public interface MultiSender : IUiRtcSenderContract<MultiHub>
{
    Task SendFirst(FirstModel model);
    Task SendSecond(SecondModel model);
}
```

## Unit Test Guidelines

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific project
dotnet test UiRtc.UnitTests/UiRtc.UnitTests.csproj

# Run with code coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

### Test Naming Convention
```csharp
// Pattern: WhenConditionThenExpectedResult
public class NameHelperTests
{
    [TestMethod]
    public void WhenTypeHasUiRtcHubAttributeThenReturnsAttributeValue()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### What to Test

**UiRtc Library**:
- `NameHelper` - Hub and method name resolution
- Repository classes - Registration and retrieval
- `SendMethodBuilder` - Dynamic proxy generation

**TypeScriptGenerator**:
- `DataCollectorService` - Code analysis accuracy
- `TsGeneratorService` - Template substitution
- Model extraction with various attributes

## Adding New Scenarios

### When to Add
Add a new scenario when:
1. Testing a new feature
2. Demonstrating a new use case
3. Reproducing a bug

### Steps
1. Create new folder under `Scenarios/`
2. Create hub file with all related code
3. Follow existing pattern (hub, sender, handler, models in one file)
4. Run the integration test server to verify

### Template for New Scenario
```csharp
// Scenarios/NewFeature/NewFeatureHub.cs
using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.NewFeature
{
    public class NewFeatureHub : IUiRtcHub { }

    public interface NewFeatureSender : IUiRtcSenderContract<NewFeatureHub>
    {
        Task SendResponse(ResponseModel response);
    }

    public class NewFeatureHandler(IUiRtcSenderService senderService) 
        : IUiRtcHandler<NewFeatureHub, RequestModel>
    {
        public async Task ConsumeAsync(RequestModel model)
        {
            // Implementation
            await senderService.Send<NewFeatureSender>()
                .SendResponse(new ResponseModel { /* ... */ });
        }
    }

    [TranspilationSource]
    public class RequestModel
    {
        public required string Id { get; set; }
    }

    [TranspilationSource]
    public class ResponseModel
    {
        public required string Result { get; set; }
    }
}
```

## TypeScript Generation Testing

### Generate from Integration Test
```bash
dotnet-uirtc -p "IntegrationTest/BE01.IntegrationTest/BE01.IntegrationTest.csproj" -o "./test-output/"
```

### Verify Generated Output
Check that generated TypeScript includes:
1. All hub names in `uiRtcHubs` type
2. All handler methods in `uiRtcCommunication`
3. All sender methods in `uiRtcSubscription`
4. All models with correct TypeScript types
