using Cocona;
using Microsoft.Build.Locator;

MSBuildLocator.RegisterDefaults();

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<App>();

await app.RunAsync();