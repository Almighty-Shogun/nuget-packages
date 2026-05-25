---
outline: deep

params:
    - name: aliases
      description: Alternative command names that should resolve to the same command handler.
      type: string[]
      default: '[]'
---

# AliasAttribute

Adds one or more aliases to a command class. `ConsoleCommandBase` reads the aliases from the same class that has `ConsoleCommandAttribute`, and `ConsoleCommandHandler` maps each alias to the same command instance.

Use this attribute when a command should support short names or older command names without duplicating command classes.

## Usage

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[Alias("stop", "quit")]
[ConsoleCommand("shutdown", "Stops the application.")]
public sealed class ShutdownCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("Stopping...");

        return Task.CompletedTask;
    }
}
```

<FrontmatterDocs/>
