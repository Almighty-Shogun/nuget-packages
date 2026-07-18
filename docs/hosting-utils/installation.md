# Installation

Install `AlmightyShogun.Hosting.Utils` in the .NET hosted application that needs custom host options or console lifetime behavior. The package targets `net10.0` and extends `IServiceCollection`.

```sh
dotnet add package AlmightyShogun.Hosting.Utils
```

## Dependencies

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.10` &mdash; provides the service collection APIs used by the extension methods.
- `Microsoft.Extensions.Hosting` `10.0.10` &mdash; provides host options, host lifetime contracts, and background-service exception behavior.

## Startup Registration

Call the extension methods while configuring services. `ConfigureHost` updates `HostOptions`; `UseCustomConsoleLifetime` replaces the default `ConsoleLifetime` service when it is present.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.Utils;

builder.Services
    .ConfigureHost(
        TimeSpan.FromSeconds(30),
        BackgroundServiceExceptionBehavior.StopHost
    )
    .UseCustomConsoleLifetime();
```
