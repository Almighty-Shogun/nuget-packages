---
returns: The `IServiceCollection` instance with the console command handler registered.
---

# AddConsoleCommands

Registers [`ConsoleCommandHandler`](../services/console-command-handler) as the singleton implementation of `IConsoleCommandHandler`, which application code resolves to start and stop the input loop. It registers no command classes, so pair it with [`RegisterConsoleCommands`](./register-console-commands).

Neither call reads what the other registered, so they may be written in either order.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services.AddConsoleCommands();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddConsoleCommands();
```
