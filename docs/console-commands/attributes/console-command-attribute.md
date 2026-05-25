---
outline: deep

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

Marks a command class with the command name, optional description, and argument-count behavior used by the console command runtime. `ConsoleCommandBase` reads this attribute from the class when the command is constructed. When the description is omitted, command metadata keeps the description as `null`.

Use this attribute on every class that inherits from `ConsoleCommandBase`. Each command class must define exactly one public instance method named `ExecuteAsync`, and that method must return `Task`. Parameters on `ExecuteAsync` become command arguments; optional method parameters can be omitted by the user, and `ignoreExtraArgs` controls whether unexpected trailing arguments are accepted.

## Usage

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

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

<FrontmatterDocs/>
