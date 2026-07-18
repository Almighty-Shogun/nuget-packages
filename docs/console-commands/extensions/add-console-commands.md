---
returns: The `IServiceCollection` instance with the console command handler registered.
---

# AddConsoleCommands

Registers the console command runtime services. The method adds `ConsoleCommandHandler` as the implementation for `IConsoleCommandHandler`, which is the service application code should resolve when it wants to start the command loop.

Use this method before registering command classes with `RegisterConsoleCommands`.

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
