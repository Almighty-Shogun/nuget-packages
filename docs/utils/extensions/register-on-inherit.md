---
params:
    - name: serviceLifetime
      description: Lifetime used for each discovered service registration.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton
    - name: assemblies
      description: Assemblies scanned for concrete implementations. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'
    - name: addType
      description: Registers each discovered implementation under service type `T` when `true`; registers it as its concrete type when `false`.
      type: bool

returns: The same `IServiceCollection` instance with matching discovered types registered.
---

# RegisterOnInherit

Scans assemblies for non-abstract, non-interface types assignable to `T` and registers each discovered type in the service collection. This is useful for command handlers, recurring jobs, plugin-style modules, and other patterns where implementations should be discovered from assemblies.

The simpler overload registers each discovered implementation as service type `T`, which is useful when consumers resolve `IEnumerable<T>` or a known base contract. The overload with `addType` lets callers choose whether registrations use service type `T` or the concrete implementation type itself.

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
