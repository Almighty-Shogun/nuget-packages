# Console Commands

Attribute-discovered console commands for dependency-injected .NET applications. The package scans assemblies for command classes, maps names and aliases, converts string input into `ExecuteAsync` parameters, and dispatches from an input loop.

Commands are ordinary DI-created classes: one that needs application services declares its own constructor, and one that needs none declares no constructor at all.

## Categories

- [Extensions](./extensions/add-console-commands) &mdash; startup methods that register the input loop and discover command classes.
- [Attributes](./attributes/console-command-attribute) &mdash; class metadata for command names, aliases, descriptions, and examples.
- [Services](./services/console-command-handler) &mdash; the input loop application code starts and stops.
- [Utilities](./utilities/console-command-discovery) &mdash; command metadata read by reflection, for building a help listing.
- [Types](./types/console-command-base) &mdash; the base class commands inherit and the metadata it is described by.

## Quick Example

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands();
```
