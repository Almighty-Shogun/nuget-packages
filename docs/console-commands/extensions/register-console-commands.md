---
params:
    - name: assemblies
      description: The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking no assembly is the one that falls back to the calling assembly.
      type: Assembly[]

returns: The `IServiceCollection` instance with the discovered command classes registered.
---

# RegisterConsoleCommands

Registers each discovered command class as a transient service under its own concrete type, so [`ConsoleCommandHandler`](../services/console-command-handler) builds a fresh one from a new scope per invocation and a command may depend on scoped application services. With no argument it scans the calling assembly; pass assemblies when the commands live in another project.

Pair it with [`AddConsoleCommands`](./add-console-commands), which registers the handler itself.

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

A class that carries no [`ConsoleCommandAttribute`](../attributes/console-command-attribute), declares a name that is blank or contains whitespace, declares anything other than exactly one public `ExecuteAsync`, or declares one returning something other than `Task` or `ValueTask`, throws `InvalidOperationException` naming the class during registration.

::: warning
Discovery only finds classes inheriting [`ConsoleCommandBase`](../types/console-command-base). A class carrying the attribute without it is never registered and never reachable at the prompt.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterConsoleCommands();

public IServiceCollection RegisterConsoleCommands(
    Assembly[] assemblies
);
```
