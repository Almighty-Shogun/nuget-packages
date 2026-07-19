# ConsoleCommandBase

Base class for application-defined console commands. A command class should inherit from this type, add [`ConsoleCommandAttribute`](../attributes/console-command-attribute) to the class, and define exactly one public instance method named `ExecuteAsync` that returns `Task`.

The base class reads command metadata and aliases from class attributes, validates argument counts, converts string input to `ExecuteAsync` parameter types, and invokes the command through the package's internal command execution contract. Application code should implement commands by inheriting from this class; it should not call the internal execution path directly.

## Usage

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[Alias("p")]
[Example("production")]
[ConsoleCommand("ping", "Writes a pong response.")]
public sealed class PingCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync(string environment)
    {
        Console.WriteLine($"pong from {environment}");

        return Task.CompletedTask;
    }
}
```

Command metadata such as the command name, description, and aliases is read from attributes and used by the package runtime internally. Application code should expose user-facing metadata through [`ConsoleCommandAttribute`](../attributes/console-command-attribute), [`AliasAttribute`](../attributes/alias-attribute), and [`ExampleAttribute`](../attributes/example-attribute) instead of setting properties on the base class.

Generated usage text belongs to the [`ConsoleCommand`](./console-command) metadata returned by [`ConsoleUtils.GetAllCommands`](./console-utils#getallcommands). `ConsoleCommandBase` owns runtime command execution and argument conversion; it does not generate user-facing help output by itself.

## ExecuteAsync

Derived command classes must define exactly one public instance method named `ExecuteAsync` that returns `Task`. Parameters on that method become positional command arguments. Non-optional parameters are required, optional parameters use their C# default value when the user omits them, and invalid conversions are logged instead of invoking the command.

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("promote", "Promotes a release to an environment.")]
public sealed class PromoteCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync(string version, string environment = "staging")
    {
        Console.WriteLine($"Promoting {version} to {environment}");

        return Task.CompletedTask;
    }
}
```
