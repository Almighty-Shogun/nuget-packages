# ConsoleCommandBase

Base class for application-defined console commands. A command class should inherit from this type, add [`ConsoleCommandAttribute`](../attributes/console-command-attribute) to the class, and define exactly one public instance method named `ExecuteAsync` that returns `Task` or `ValueTask`.

It reads metadata and aliases from the class attributes, validates argument counts, and converts string input to the `ExecuteAsync` parameter types before invoking it.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

[Alias("p")]
[Example("production")]
[ConsoleCommand("ping", "Writes a pong response.")]
public sealed class PingCommand : ConsoleCommandBase
{
    public Task ExecuteAsync(string environment)
    {
        Console.WriteLine($"pong from {environment}");

        return Task.CompletedTask;
    }
}
```

::: tip
The base takes no constructor arguments, so a command needing nothing declares no constructor at all. One that needs application services declares its own, and the dispatcher resolves them from a fresh scope per invocation.
:::

## ExecuteAsync

Derived command classes must define exactly one public instance method named `ExecuteAsync` that returns `Task` or `ValueTask`. Parameters on that method become positional command arguments. Non-optional parameters are required, optional parameters use their C# default value when the user omits them, and invalid conversions are logged instead of invoking the command.

A trailing `CancellationToken` parameter is supplied by the dispatcher rather than typed by the user, and is signalled when the command loop is stopping.

::: code-group

```csharp [PromoteCommand.cs]
using AlmightyShogun.ConsoleCommands;

[ConsoleCommand("promote", "Promotes a release to an environment.")]
public sealed class PromoteCommand(
    IReleaseService releaseService
) : ConsoleCommandBase
{
    public async Task ExecuteAsync(
        string version,
        string environment = "staging",
        CancellationToken cancellationToken = default
    )
    {
        await releaseService.PromoteAsync(
            version,
            environment,
            cancellationToken
        );

        Console.WriteLine($"Promoted {version} to {environment}");
    }
}
```

```csharp [IReleaseService.cs]
public interface IReleaseService
{
    Task PromoteAsync(
        string version,
        string environment,
        CancellationToken cancellationToken = default
    );
}
```

:::
