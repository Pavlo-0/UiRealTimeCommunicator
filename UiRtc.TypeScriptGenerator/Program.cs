using Cocona;
using Microsoft.Build.Locator;

MSBuildLocator.RegisterDefaults();

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<App>();

await app.RunAsync();

#if DEBUG
await Task.Delay(5000);
#endif