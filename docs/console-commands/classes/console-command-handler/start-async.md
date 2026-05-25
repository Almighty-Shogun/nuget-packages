---
outline: deep

params:
    - name: cancellationToken
      description: Token used to stop the console input loop.
      type: CancellationToken
      default: default

returns: A task that completes when the command loop stops.
---

# StartAsync

Starts the console command input loop. The method reads each line from `Console.In`, treats the first token as the command name, and passes the remaining tokens to the resolved command through the internal execution contract.

Use this method after the service provider has been built and command classes have been registered. Resolve the handler through `IConsoleCommandHandler` so application code depends on the public DI contract instead of constructing `ConsoleCommandHandler` manually.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;
using Microsoft.Extensions.DependencyInjection;

IConsoleCommandHandler handler = serviceProvider.GetRequiredService<IConsoleCommandHandler>();

await handler.StartAsync(CancellationToken.None);
```

<FrontmatterDocs/>

## Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken = default);
```

## Uses

- [ConsoleUtils](../console-utils/)
