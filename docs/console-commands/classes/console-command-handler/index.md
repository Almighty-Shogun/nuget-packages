# ConsoleCommandHandler

Runtime command dispatcher for console input. The handler receives registered commands from dependency injection, verifies that they inherit from `ConsoleCommandBase`, maps command names and aliases to command instances, reads lines from `Console.In`, and executes the matching command through the package's internal command execution contract.

`AddConsoleCommands` registers this class for the `IConsoleCommandHandler` interface. Application code should resolve `IConsoleCommandHandler` from dependency injection when it wants to start the input loop.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.DependencyInjection;

IConsoleCommandHandler handler = serviceProvider.GetRequiredService<IConsoleCommandHandler>();

await handler.StartAsync();
```

## Methods

- [StartAsync](./start-async) &mdash; starts reading console input and dispatching commands.
