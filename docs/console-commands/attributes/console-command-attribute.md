---
params:
    - name: name
      description: Command name typed by the user in the console input.
      type: string

    - name: description
      description: Optional command description used when command metadata is listed.
      type: string?
      default: 'null'

    - name: ignoreExtraArgs
      description: Whether extra input arguments should be ignored instead of treated as an invalid argument count.
      type: bool
      default: 'false'
---

# ConsoleCommandAttribute

Marks a command class with the command name, optional description, and argument-count behavior used by the console command runtime. [`ConsoleCommandBase`](../types/console-command-base) reads this attribute from the class when the command is constructed. When the description is omitted, command metadata keeps the description as `null`.

Use this attribute on every class that inherits from [`ConsoleCommandBase`](../types/console-command-base). Each command class must define exactly one public instance method named `ExecuteAsync`, and that method must return `Task`. Parameters on `ExecuteAsync` become command arguments; optional method parameters can be omitted by the user, and `ignoreExtraArgs` controls whether unexpected trailing arguments are accepted.

## Usage

::: code-group

```csharp [StatusCommand.cs]
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("status", "Writes the current worker status.")]
public sealed class StatusCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("Worker is running.");

        return Task.CompletedTask;
    }
}
```

```csharp [DeployCommand.cs]
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[Example("production true")]
[ConsoleCommand("deploy", "Deploys a named environment.")]
public sealed class DeployCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync(string environment, bool force = false)
    {
        Console.WriteLine($"Deploying {environment}. Force: {force}");

        return Task.CompletedTask;
    }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public ConsoleCommandAttribute(
    string name,
    string? description = null,
    bool ignoreExtraArgs = false
);
```
