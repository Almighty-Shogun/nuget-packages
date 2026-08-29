# Installation

Install `AlmightyShogun.ConsoleCommands` in the .NET console or hosted application that should discover and run command classes from dependency injection. The package targets `net10.0` and expects command implementations to be registered from one or more assemblies.

```sh
dotnet add package AlmightyShogun.ConsoleCommands
```

## Dependencies

### Package references

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; the service collection the commands register into.
- `Microsoft.Extensions.Logging.Abstractions` `10.0.11` &mdash; `ILogger<T>`, which the input loop reports unusable input through. Output goes to whichever logging provider the application registered.

### Project references

- `AlmightyShogun.Utils` &mdash; provides assembly scanning and inherited-type registration helpers.

## Startup Registration

[`AddConsoleCommands`](./extensions/add-console-commands) registers the input loop, and [`RegisterConsoleCommands`](./extensions/register-console-commands) discovers the command classes it dispatches to. Nothing reads the console until [`IConsoleCommandHandler`](./services/console-command-handler) is resolved and started.

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands();
```
