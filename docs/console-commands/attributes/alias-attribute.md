---
params:
    - name: aliases
      description: Alternative command names that should resolve to the same command handler.
      type: string[]
      default: '[]'
---

# AliasAttribute

Adds one or more aliases to a command class, so a short form or a retired name keeps working without a second class. [`ConsoleCommandHandler`](../services/console-command-handler) maps each alias to the same command type as the name on [`ConsoleCommandAttribute`](./console-command-attribute), matching case-insensitively.

An alias already claimed by another command is dropped with a warning, so the first registration keeps the name.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

[Alias("stop", "quit")]
[ConsoleCommand("shutdown", "Stops the application.")]
public sealed class ShutdownCommand : ConsoleCommandBase
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("Stopping...");

        return Task.CompletedTask;
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public AliasAttribute(params string[] aliases);
```
