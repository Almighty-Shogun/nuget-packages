# ConsoleCommandBase

Base class for application-defined console commands. A command class should inherit from this type, add `ConsoleCommandAttribute` to the class, and define exactly one public instance method named `ExecuteAsync` that returns `Task`.

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
