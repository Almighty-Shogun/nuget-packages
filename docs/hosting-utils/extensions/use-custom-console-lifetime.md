---
outline: deep

returns: The `IServiceCollection` instance with the custom console lifetime registered.
---

# UseCustomConsoleLifetime

Replaces the default .NET `ConsoleLifetime` with the package's custom console lifetime. The custom lifetime prevents `Ctrl+C` from stopping the host unless the `DOTNET_RUNNING_IN_IDE` environment variable is set. On non-Windows platforms, it also registers a `SIGTERM` handler that cancels the signal.

Use this method when a console-hosted application should not be stopped accidentally by `Ctrl+C` in normal terminal usage, while still allowing IDE run/debug sessions to stop cleanly when `DOTNET_RUNNING_IN_IDE=true` is configured.

## Usage

```csharp
using AlmightyShogun.Hosting.Utils;

builder.Services.UseCustomConsoleLifetime();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection UseCustomConsoleLifetime();
```
