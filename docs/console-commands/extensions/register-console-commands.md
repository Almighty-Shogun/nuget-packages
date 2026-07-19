---
params:
    - name: assemblies
      description: Assemblies scanned for concrete command classes. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'

returns: The `IServiceCollection` instance with discovered console command classes registered.
---

# RegisterConsoleCommands

Registers application command classes from one or more assemblies. The method scans for concrete implementations of the public command contract and registers them as transient services so [`ConsoleCommandHandler`](../services/console-command-handler) can receive them from dependency injection.

Use this method after [`AddConsoleCommands`](./add-console-commands). Pass explicit assemblies when command classes live in another project; relying on the calling assembly is only appropriate when commands are defined in the startup assembly. Command classes should inherit from [`ConsoleCommandBase`](../types/console-command-base); the handler rejects registered command services that do not use the base class.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

builder.Services
    .AddConsoleCommands()
    .RegisterConsoleCommands(typeof(Program).Assembly);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterConsoleCommands(
    params Assembly[] assemblies
);
```
