---
fields:
    - name: CommandName
      description: The name as the user typed it, so a command reached through an alias reports the alias rather than its declared name.
      type: string

    - name: Exception
      description: The exception the command let escape, with its original stack trace rather than a reflection wrapper.
      type: Exception
---

# ConsoleCommandErrorEvent

The failure passed to [`CommandFailed`](../services/console-command-handler#commandfailed) when a command throws. The dispatcher has already logged the exception by the time this is raised, so a subscriber adds to the report rather than replacing it.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.Hosting;

public sealed class CommandFailureReporter(
    IConsoleCommandHandler commandHandler,
    IHostApplicationLifetime lifetime
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        commandHandler.CommandFailed += OnCommandFailed;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        commandHandler.CommandFailed -= OnCommandFailed;

        return Task.CompletedTask;
    }

    private void OnCommandFailed(object? sender, ConsoleCommandErrorEvent args)
    {
        Console.Error.WriteLine($"{args.CommandName} failed: {args.Exception.Message}");

        lifetime.StopApplication();
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed class ConsoleCommandErrorEvent(
    string commandName,
    Exception exception
) : EventArgs
```
