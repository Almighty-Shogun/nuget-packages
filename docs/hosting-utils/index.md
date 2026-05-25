# Hosting Utils

Adds small .NET hosting helpers for configuring host shutdown behavior and replacing the default console lifetime. The package is intended for hosted applications that need consistent shutdown options or custom `Ctrl+C` handling.

Use this package when a host should have an explicit graceful shutdown timeout, a defined background-service exception policy, or a console lifetime that prevents accidental shutdown outside an IDE.

## Categories

- [Extensions](./extensions/configure-host) &mdash; service registration methods for host options and console lifetime behavior.

## Quick Example

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.Utils;

builder.Services.ConfigureHost(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);
```
