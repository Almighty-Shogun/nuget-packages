---
params:
    - name: name
      description: Command name typed by the user in the console input, matched case-insensitively. Must not be blank or contain whitespace, since input is split on spaces before the first token is matched.
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

Marks a command class with its name, optional description, and argument-count behavior. Required on every class inheriting [`ConsoleCommandBase`](../types/console-command-base).

Parameters on the class's single public `ExecuteAsync` become command arguments; optional ones may be omitted, and `ignoreExtraArgs` decides whether trailing arguments are accepted.

## Usage

::: code-group

```csharp [StatusCommand.cs]
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("status", "Writes the current worker status.")]
public sealed class StatusCommand : ConsoleCommandBase
{
    public Task ExecuteAsync()
    {
        Console.WriteLine("Worker is running.");

        return Task.CompletedTask;
    }
}
```

```csharp [DeployCommand.cs]
using AlmightyShogun.ConsoleCommands;

[Example("production true")]
[ConsoleCommand("deploy", "Deploys a named environment.")]
public sealed class DeployCommand : ConsoleCommandBase
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
