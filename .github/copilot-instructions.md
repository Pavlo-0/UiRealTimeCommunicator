# UiRealTimeCommunicator - Copilot Instructions

> **IMPORTANT**: This documentation helps Copilot understand the project quickly. When modifying code that affects the patterns documented here, update the relevant documentation file.

## Quick Project Overview

**UiRealTimeCommunicator** is a NuGet library for **strongly-typed SignalR communication** between C# .NET 8 servers and TypeScript clients. It provides:

- Type-safe message contracts between server and client
- Automatic TypeScript code generation from C# contracts
- Simplified SignalR hub management with dependency injection

## Documentation Index

| Document | Purpose |
|----------|---------|
| [Architecture](docs/architecture.md) | Core concepts, component relationships, data flow |
| [Project Structure](docs/project-structure.md) | Solution layout, project responsibilities, file locations |
| [Code Patterns](docs/code-patterns.md) | Common patterns, naming conventions, implementation examples |
| [TypeScript Generator](docs/typescript-generator.md) | CLI tool usage, template system, code generation |
| [Testing Guide](docs/testing-guide.md) | Test structure, integration tests, scenarios |

## Key Concepts (Quick Reference)

### Core Types

| Type | Purpose | Location |
|------|---------|----------|
| `IUiRtcHub` | Marker interface for hub classes | `UiRtc.Typing` |
| `IUiRtcSenderContract<THub>` | Interface for server-to-client message contracts | `UiRtc.Typing` |
| `IUiRtcHandler<THub, TModel>` | Interface for client-to-server message handlers | `UiRtc.Typing` |
| `[UiRtcHub("Name")]` | Attribute to define hub name (optional) | `UiRtc.Typing` |
| `[UiRtcMethod("Name")]` | Attribute to define method name (optional) | `UiRtc.Typing` |
| `[TranspilationSource]` | Marks models for TypeScript generation (from Tapper) | External |

### Entry Points

- **DI Registration**: `services.AddUiRealTimeCommunicator()` in `UiRtcServiceExtensions.cs`
- **Middleware**: `app.UseUiRealTimeCommunicator()` in `UiRtcServiceExtensions.cs`
- **CLI Tool**: `dotnet-uirtc` command in `Program.cs` (TypeScriptGenerator project)

## How to Navigate This Codebase

### When modifying server-side behavior:
1. Start with `UiRtc\Public\UiRtcServiceExtensions.cs` for DI/middleware
2. Check `UiRtc\Domain\` for implementation details
3. Look at `IntegrationTest\BE01.IntegrationTest\Scenarios\` for usage examples

### When modifying TypeScript generation:
1. Start with `UiRtc.TypeScriptGenerator\App.cs` for CLI flow
2. Check `DataCollectorService.cs` for C# code analysis
3. Check `TsGeneratorService.cs` for TypeScript output
4. Modify `Templates\TsTemplate.v1.0.ts` for generated code structure

### When adding new features:
1. Check existing scenarios in `IntegrationTest\BE01.IntegrationTest\Scenarios\`
2. Follow the pattern of existing implementations
3. Add new integration test scenario if needed

## Critical Implementation Notes

### Hub Name Resolution
Hub names are resolved by `NameHelper.GetHubName()`:
1. First checks for `[UiRtcHub("Name")]` attribute
2. Falls back to class name if no attribute

### Method Name Resolution
Method names are resolved by `NameHelper.GetMethodName()`:
1. First checks for `[UiRtcMethod("Name")]` attribute
2. Falls back to class name if no attribute

### Dynamic Hub Generation
`SignalRHubBuilder.cs` dynamically generates SignalR Hub types at runtime using `System.Reflection.Emit`. This is why hubs don't need explicit method definitions.

## Documentation Maintenance Instructions

> **When to Update Documentation:**
> - Adding new public interfaces or attributes ? Update `architecture.md`
> - Adding new projects or major folders ? Update `project-structure.md`
> - Changing naming conventions or patterns ? Update `code-patterns.md`
> - Modifying CLI tool or templates ? Update `typescript-generator.md`
> - Adding new test scenarios ? Update `testing-guide.md`

> **How to Update:**
> 1. Make code changes first
> 2. Identify which documentation files are affected
> 3. Update the relevant sections
> 4. Keep examples in sync with actual code
