# Installation

Install `AlmightyShogun.Hosting.ConsoleLifetime` in the .NET hosted application that needs custom host options or console lifetime behavior. The package targets `net10.0`.

```sh
dotnet add package AlmightyShogun.Hosting.ConsoleLifetime
```

## Dependencies

### Package references

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; provides the service collection APIs used by the extension methods.
- `Microsoft.Extensions.Hosting` `10.0.11` &mdash; provides host options, host lifetime contracts, and background-service exception behavior.

### Project references

- `AlmightyShogun.Core` &mdash; supplies `ReplaceService`, which swaps the framework's own console lifetime out rather than adding a second registration alongside it.

## Startup Registration

Both methods have overloads for `IHostApplicationBuilder`, `IHostBuilder`, and `IServiceCollection`. Use whichever matches how the host is built; the builder overloads forward to the service collection ones.

::: code-group

```csharp [IHostApplicationBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.UseCustomConsoleLifetime();
builder.ConfigureHostOptions(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);
```

```csharp [IHostBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

IHost host = Host.CreateDefaultBuilder(args)
    .UseCustomConsoleLifetime()
    .ConfigureHostOptions(
        TimeSpan.FromSeconds(30),
        BackgroundServiceExceptionBehavior.StopHost
    )
    .Build();
```

```csharp [IServiceCollection.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .UseCustomConsoleLifetime()
    .ConfigureHostOptions(
        TimeSpan.FromSeconds(30),
        BackgroundServiceExceptionBehavior.StopHost
    );
```

:::
