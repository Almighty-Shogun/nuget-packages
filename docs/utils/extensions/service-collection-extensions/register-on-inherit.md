---
outline: deep

params:
    - name: serviceLifetime
      description: Lifetime used for every discovered registration.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton

    - name: assemblies
      description: Assemblies to scan. When omitted, the calling assembly is scanned.
      type: Assembly[]
      default: '[]'

    - name: addType
      description: Whether discovered implementations should be registered as service type `T` instead of their concrete type.
      type: bool

returns: The `IServiceCollection` instance with discovered concrete types registered.
---

# RegisterOnInherit

Scans assemblies for non-abstract, non-interface types assignable to `T` and registers each discovered type in the service collection. This is useful for command handlers, job classes, plugin-style modules, and other patterns where implementations should be discovered from assemblies.

There are two overloads. The simpler overload registers each discovered implementation as `T`, which is useful when consumers resolve `IEnumerable<T>` or a known base contract. The overload with `addType` lets callers choose whether registrations use service type `T` or the concrete type itself.

## Usage

::: code-group

```csharp [Contract.cs]
using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.RegisterOnInherit<ICommandHandler>(
    ServiceLifetime.Transient,
    Assembly.GetExecutingAssembly()
);
```

```csharp [Concrete.cs]
using System.Reflection;
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.RegisterOnInherit<ICommandHandler>(
    false,
    ServiceLifetime.Transient,
    Assembly.GetExecutingAssembly()
);
```

```csharp [CommandHandler.cs]
public interface ICommandHandler
{
    Task HandleAsync();
}

public sealed class ImportCommandHandler : ICommandHandler
{
    public Task HandleAsync()
    {
        Console.WriteLine("Import started.");

        return Task.CompletedTask;
    }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterOnInherit<T>(
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
    params Assembly[] assemblies
) where T : class;

public IServiceCollection RegisterOnInherit<T>(
    bool addType,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
    params Assembly[] assemblies
) where T : class;
```

## Uses

- [GetOnInherit](../../classes/application-utils/get-on-inherit)
