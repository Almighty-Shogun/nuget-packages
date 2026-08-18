---
params:
    - name: serviceLifetime
      description: Lifetime used for each discovered service registration.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton
    - name: addType
      description: Registers each discovered implementation under service type `T` when `true`, which is what a consumer resolving `IEnumerable<T>` needs. Registers it under its own concrete type when `false`.
      type: bool
      default: 'true'
    - name: filter
      description: Predicate applied to each discovered type. Only types it accepts are registered.
      type: 'Func<Type, bool>?'
      default: 'null'
    - name: assemblies
      description: Assemblies scanned for implementations. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'

returns: The same `IServiceCollection` instance with matching discovered types registered.
---

# RegisterOnInherit

Scans assemblies for concrete types assignable to `T` and registers each one, for command handlers, recurring jobs, validation rules, and anything else better discovered than listed by hand. Interfaces and abstract classes are never registered, and neither is a type carrying [`SkipAutoRegistration`](../attributes/skip-auto-registration).

## Usage

::: code-group

```csharp [Contract.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .RegisterOnInherit<ICommandHandler>(ServiceLifetime.Transient);
```

```csharp [Assemblies.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.RegisterOnInherit<ICommandHandler>(
    ServiceLifetime.Transient,
    typeof(ImportCommandHandler).Assembly
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

::: tip
The second overload takes a lifetime followed directly by assemblies. Use the first when you need `addType` or `filter`, and pass `filter:` by name there, because it sits behind another optional parameter.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterOnInherit<T>(
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
    bool addType = true,
    Func<Type, bool>? filter = null,
    params Assembly[] assemblies
) where T : class;

public IServiceCollection RegisterOnInherit<T>(
    ServiceLifetime serviceLifetime,
    params Assembly[] assemblies
) where T : class;
```
