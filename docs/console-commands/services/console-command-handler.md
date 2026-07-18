# ConsoleCommandHandler

Dependency-injection service for starting the console command input loop. `AddConsoleCommands` registers the package handler for `IConsoleCommandHandler`, and application code should resolve the interface when it is ready to read and dispatch console commands.

Use this service from hosted services, startup code, or console application entry points that control when command input should begin. Command discovery and registration still happen through `RegisterConsoleCommands`; this service starts and stops the runtime loop that consumes the registered command instances.

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(IConsoleCommandHandler commandHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return commandHandler.StartAsync(stoppingToken);
    }
}
```

Register the command handler and command classes before resolving the service.

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands(typeof(Program).Assembly)
    .AddHostedService<ConsoleCommandWorker>();
```

## StartAsync

Starts the console command input loop through the registered command handler. The provided implementation reads lines from `Console.In`, treats the first token as the command name, and forwards the remaining tokens to the matching command class.

Use this method after the service provider has been built and command classes have been registered. Resolve the handler through `IConsoleCommandHandler` so application code depends on the public DI contract instead of the concrete handler type.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(IConsoleCommandHandler commandHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return commandHandler.StartAsync(stoppingToken);
    }
}
```

### Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Stop

Stops the active console command input loop started by `StartAsync`. Use this when application lifetime code needs to shut the command loop down explicitly instead of only relying on the original cancellation token.

Calling `Stop` when the handler is not currently running logs an error and returns. It does not dispose the service provider or unregister commands; it only cancels the active read loop owned by the handler.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(IConsoleCommandHandler commandHandler) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return commandHandler.StartAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        commandHandler.Stop();

        await base.StopAsync(cancellationToken);
    }
}
```

### Type signature

```csharp
public void Stop();
```
