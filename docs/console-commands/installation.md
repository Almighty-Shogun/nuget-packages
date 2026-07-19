# Installation

Install `AlmightyShogun.ConsoleCommands` in the .NET console or hosted application that should discover and run command classes from dependency injection. The package targets `net10.0` and expects command implementations to be registered from one or more assemblies.

```sh
dotnet add package AlmightyShogun.ConsoleCommands
```

## Dependencies

### Project references

- `AlmightyShogun.Utils` &mdash; provides assembly scanning and inherited-type registration helpers.
- `AlmightyShogun.Logging` &mdash; provides logging support used by the command runtime.

## Startup Registration

Register the command handler once, then scan the assemblies that contain command classes. If command classes live outside the startup project, pass a marker type from the project that contains them.

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands(typeof(Program).Assembly);
```

When the application is ready to run the input loop, resolve the handler through [`IConsoleCommandHandler`](./services/console-command-handler) and call `StartAsync`.

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.DependencyInjection;

IConsoleCommandHandler handler = serviceProvider.GetRequiredService<IConsoleCommandHandler>();

await handler.StartAsync();
```
