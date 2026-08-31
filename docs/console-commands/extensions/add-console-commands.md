---
returns: The `IServiceCollection` instance with the console command handler registered.
---

# AddConsoleCommands

Registers the console command runtime services. The method adds [`ConsoleCommandHandler`](../services/console-command-handler) as the implementation for `IConsoleCommandHandler`, which is the service application code should resolve when it wants to start the command loop.

Pair it with [`RegisterConsoleCommands`](./register-console-commands), which registers the command classes themselves. Neither call reads what the other registered, so they may be written in either order.

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
