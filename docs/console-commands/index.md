# Console Commands

Adds attribute-discovered console commands for dependency-injected .NET applications. The package registers a command handler, scans assemblies for command classes, maps command names and aliases, converts string input into `ExecuteAsync` parameters, and invokes the command from a console input loop.

Use this package when a hosted console application needs command-style interactions without hand-writing a parser and dispatcher. Commands are normal DI-created classes, so they can use logging and application services through constructors.

## Categories

- [Extensions](./extensions/add-console-commands) &mdash; startup extension methods for registering the handler and command classes.
- [Attributes](./attributes/console-command-attribute) &mdash; class metadata used for command names, aliases, descriptions, and examples.
- [Services](./services/console-command-handler) &mdash; dependency-injection services used to start the command loop.
- [Types](./types/console-command-base) &mdash; command base types, metadata values, and console helper utilities.

## Quick Example

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands(typeof(Program).Assembly);
```
