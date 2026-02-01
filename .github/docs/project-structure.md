# Project Structure

> **Update this file when:** Adding new projects, creating new major folders, or reorganizing file locations.

## Solution Overview

```
UiRealTimeCommunicator.sln
??? UiRtc/                              # Main runtime library (NuGet: UiRealTimeCommunicator)
??? UiRtc.Typing/                       # Type definitions only (interfaces, attributes)
??? UiRtc.TypeScriptGenerator/          # CLI tool (NuGet: UiRealTimeCommunicator.TypeScriptGenerator)
??? UiRtc.UnitTests/                    # Unit tests for UiRtc
??? UiRtc.TypeScriptGenerator.UnitTests/# Unit tests for TypeScript generator
??? IntegrationTest/
?   ??? BE01.IntegrationTest/           # Integration test server with scenarios
??? Examples/
    ??? Simple/                         # Simple example application
    ?   ??? app-backend/
    ??? Chat/                           # Chat example application
        ??? ChatServer/
```

## Project Details

### UiRtc.Typing (Type Definitions)
**Package**: Part of main NuGet package  
**Purpose**: Contains only interfaces and attributes - no implementation

```
UiRtc.Typing/
??? PublicInterface/
    ??? IUiRtcHub.cs                    # Marker interface for hub classes
    ??? IUiRtcSenderContract.cs         # Server?client contract base
    ??? IUiRtcHandler.cs                # Client?server handlers (2 overloads)
    ??? IUiRtcContextHandler.cs         # Handlers with SignalR context access
    ??? IUiRtcConnection.cs             # Connection lifecycle events
    ??? IUiRtcProxyContext.cs           # Context accessor interface
    ??? Attributes/
        ??? UiRtcHubAttribute.cs        # [UiRtcHub("Name")] for custom hub names
        ??? UiRtcMethodAttribute.cs     # [UiRtcMethod("Name")] for custom method names
```

### UiRtc (Main Library)
**NuGet Package**: `UiRealTimeCommunicator`  
**Purpose**: Runtime implementation

```
UiRtc/
??? Public/                             # Public API surface
?   ??? UiRtcServiceExtensions.cs       # AddUiRealTimeCommunicator(), UseUiRealTimeCommunicator()
?   ??? IUiRtcSenderService.cs          # Public interface for sending messages
?   ??? IUiRtcConfiguration.cs          # Configuration interface
??? Domain/                             # Internal implementation
?   ??? NameHelper.cs                   # Hub/method name resolution from types/attributes
?   ??? Handler/                        # Client?server message handling
?   ?   ??? AutoRegistrationHandlers.cs # Auto-discovers and registers handlers
?   ?   ??? Receiver.cs                 # Dispatches messages to handlers
?   ?   ??? Interface/
?   ?       ??? IReceiverService.cs
?   ??? Sender/                         # Server?client message sending
?   ?   ??? SenderService.cs            # IUiRtcSenderService implementation
?   ?   ??? SendMethodBuilder.cs        # Dynamic proxy builder for sender contracts
?   ?   ??? InvokeSenderService.cs      # Actual SignalR invocation
?   ?   ??? Interface/
?   ?       ??? IInvokeService.cs
?   ?       ??? IInvokeSenderService.cs
?   ??? HubMap/                         # Dynamic hub generation
?   ?   ??? AutoRegistrationHubs.cs     # Registers generated hubs with SignalR
?   ?   ??? SignalRHubBuilder.cs        # IL emit to create hub types at runtime
?   ?   ??? ProxyHub.cs                 # Base class for generated hubs
?   ?   ??? ProxyContext.cs             # SignalR context wrapper
?   ?   ??? ConnectionInvokeService.cs  # Connection event handling
?   ?   ??? Interface/
?   ?       ??? IAutoRegistrationHubs.cs
?   ?       ??? IConnectionInvokeService.cs
?   ??? Repository/                     # Metadata storage
?       ??? HubRepository.cs            # Stores hub info
?       ??? HandlerRepository.cs        # Stores handler registrations
?       ??? ConnectionRepository.cs     # Stores connection handlers
?       ??? Records/
?       ?   ??? HubRecord.cs
?       ?   ??? HandlerRecord.cs
?       ?   ??? HandlerBuilderModel.cs
?       ?   ??? ConnectionRecord.cs
?       ??? Interface/
?           ??? IHubRepository.cs
?           ??? IHandlerRepository.cs
?           ??? IConnectionRepository.cs
??? Configuration.cs                    # Configuration implementation
```

### UiRtc.TypeScriptGenerator (CLI Tool)
**NuGet Package**: `UiRealTimeCommunicator.TypeScriptGenerator`  
**CLI Command**: `dotnet-uirtc`  
**Purpose**: Generate TypeScript from C# contracts

```
UiRtc.TypeScriptGenerator/
??? Program.cs                          # Entry point (Cocona CLI framework)
??? App.cs                              # Main command handler
??? DataCollectorService.cs             # Roslyn-based C# code analysis
??? TsGeneratorService.cs               # TypeScript code generation
??? DataModels/
?   ??? SenderDataRecord.cs             # Data model for sender contracts
?   ??? HandlerDataRecord.cs            # Data model for handlers
??? CustomExceptions/
?   ??? UiRtcHubAttributeNotFound.cs
??? Templates/
?   ??? TsTemplate.v1.0.ts              # TypeScript template with placeholders
??? README.md                           # CLI usage documentation
```

### IntegrationTest/BE01.IntegrationTest
**Purpose**: Integration test server demonstrating all features

```
IntegrationTest/BE01.IntegrationTest/
??? Program.cs                          # Test server startup
??? Scenarios/                          # Each folder is a complete feature test
    ??? Simple/                         # Basic hub, sender, handler
    ?   ??? SimpleHub.cs
    ??? SimpleContext/                  # Handler with context access
    ?   ??? SimpleContextHub.cs
    ??? SimpleEmpty/                    # Handler without parameters
    ?   ??? SimpleEmptyHub.cs
    ??? SimpleDateTimeField/            # DateTime serialization
    ?   ??? SimpleDateTimeHub.cs
    ??? AttributeDeclaration/           # Custom hub/method names via attributes
    ?   ??? AttributeDeclarationHub.cs
    ??? TwoSubscription/                # Multiple subscriptions per hub
    ?   ??? TwoSubscriptionHub.cs
    ??? OnConnection/                   # Connection lifecycle events
    ?   ??? OnConnectionHub.cs
    ?   ??? OnConnectionManager.cs
    ??? ConnectionId/                   # Sending to specific connections
    ?   ??? ConnectionIdSenderHub.cs
    ??? UnsubscribeSimple/              # Unsubscribe functionality
        ??? UnsubscribeSimpleHub.cs
```

### Examples
**Purpose**: Real-world usage examples

```
Examples/
??? Simple/
?   ??? app-backend/                    # Simple weather example
?       ??? Program.cs
?       ??? readme.txt
??? Chat/
    ??? ChatServer/                     # Chat application example
        ??? Program.cs
        ??? readme.txt
```

## Key File Locations by Task

| Task | Primary Files |
|------|---------------|
| Add new public interface | `UiRtc.Typing\PublicInterface\` |
| Change DI registration | `UiRtc\Public\UiRtcServiceExtensions.cs` |
| Modify sender logic | `UiRtc\Domain\Sender\SenderService.cs`, `SendMethodBuilder.cs` |
| Modify handler logic | `UiRtc\Domain\Handler\Receiver.cs`, `AutoRegistrationHandlers.cs` |
| Change hub generation | `UiRtc\Domain\HubMap\SignalRHubBuilder.cs` |
| Modify name resolution | `UiRtc\Domain\NameHelper.cs` |
| Change TypeScript output | `UiRtc.TypeScriptGenerator\Templates\TsTemplate.v1.0.ts` |
| Change code analysis | `UiRtc.TypeScriptGenerator\DataCollectorService.cs` |
| Add test scenario | `IntegrationTest\BE01.IntegrationTest\Scenarios\` |

## Dependencies

### UiRtc
- `Microsoft.AspNetCore.SignalR` - SignalR runtime

### UiRtc.TypeScriptGenerator
- `Cocona` - CLI framework
- `Tapper` - TypeScript model generation from C# classes
- `Microsoft.CodeAnalysis.CSharp.Workspaces` - Roslyn for C# analysis
- `Microsoft.Build.Locator` - MSBuild project loading

### External Types Used
- `[TranspilationSource]` - From Tapper library, marks models for TypeScript generation
