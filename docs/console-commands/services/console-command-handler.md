# ConsoleCommandHandler

Starts and stops the console command input loop. Application code depends on `IConsoleCommandHandler` and resolves it from a hosted service or entry point when it is ready to read commands.

Command classes are discovered separately, by [`RegisterConsoleCommands`](../extensions/register-console-commands).

## Usage

::: code-group

```csharp [ConsoleCommandWorker.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(
    IConsoleCommandHandler commandHandler
) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken cancellationToken
    ) => commandHandler.StartAsync(cancellationToken);
}
```

```csharp [Program.cs]
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands()
    .AddHostedService<ConsoleCommandWorker>();
```

:::

## StartAsync

Reads lines from `Console.In`, treats the first token as the command name, and forwards the rest to the matching command class. The loop ends when the token is cancelled or [`Stop`](#stop) is called.

An exception escaping a command is logged and the prompt keeps reading, so one failing command does not take the console down. Subscribe to `CommandFailed` to report it elsewhere.

Only one loop may run at a time. Calling this while one is already running logs an error and returns.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(
    IConsoleCommandHandler commandHandler
) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken cancellationToken
    ) => commandHandler.StartAsync(cancellationToken);
}
```

### Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Stop

Cancels the running loop. A command such as `exit` calls it to shut the prompt down from inside itself.

Calling it when no loop is running logs an error and returns. A command already executing is not interrupted unless it accepts a `CancellationToken` of its own.

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.ConsoleCommands;

public sealed class ConsoleCommandWorker(
    IConsoleCommandHandler commandHandler
) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken cancellationToken
    ) => commandHandler.StartAsync(cancellationToken);

    public override async Task StopAsync(
        CancellationToken cancellationToken
    )
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

## CommandFailed

Raised after a command threw and the failure was logged, for reporting it somewhere the dispatcher knows nothing about, such as telemetry or a non-zero exit code. `CommandName` is the name as typed, so an alias comes through as the alias.

Handlers run on the loop's thread before the next line is read, so a slow one delays the prompt, and an exception from a handler is not caught.

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.Logging;

public sealed class CommandFailureReporter(
    IConsoleCommandHandler commandHandler,
    ILogger<CommandFailureReporter> logger
)
{
    public void Subscribe() => commandHandler.CommandFailed += (_, args) => logger.LogCritical(
        args.Exception,
        "Operator command {CommandName} failed",
        args.CommandName
    );
}
```

### Type signature

```csharp
public event EventHandler<ConsoleCommandErrorEvent>? CommandFailed;
```
