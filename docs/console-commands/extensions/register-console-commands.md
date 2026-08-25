---
params:
    - name: assemblies
      description: The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking no assembly is the one that falls back to the calling assembly.
      type: Assembly[]

returns: The `IServiceCollection` instance with the discovered command classes registered.
---

# RegisterConsoleCommands

Registers the command classes declared in the given assemblies as transient services, so [`ConsoleCommandHandler`](../services/console-command-handler) receives them from dependency injection. Call it after [`AddConsoleCommands`](./add-console-commands), which registers the handler itself.

A fresh instance is built per invocation, so a command may depend on scoped application services.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands();
```

```csharp [DeployCommand.cs]
using AlmightyShogun.ConsoleCommands;

[Alias("ship")]
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

## Malformed commands

Every discovered command class is registered, including one that is malformed. A class that carries no [`ConsoleCommandAttribute`](../attributes/console-command-attribute), declares anything other than exactly one public `ExecuteAsync`, or declares one that does not return `Task`, throws `InvalidOperationException` naming the class when the handler is resolved.

::: warning
A command that does not inherit [`ConsoleCommandBase`](../types/console-command-base) is rejected the same way.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterConsoleCommands();

public IServiceCollection RegisterConsoleCommands(
    Assembly[] assemblies
);
```
