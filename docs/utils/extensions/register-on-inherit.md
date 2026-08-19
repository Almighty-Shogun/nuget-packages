---
params:
    - name: assembly / assemblies
      description: The assembly to scan, or several as an array in the order they should be searched. An empty array registers nothing, and the overload taking neither scans the calling assembly.
      type: 'Assembly / Assembly[]'
    - name: serviceLifetime
      description: Lifetime used for each discovered service registration.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton
    - name: addType
      description: Registers each discovered implementation under service type `T` when `true`, which is what a consumer resolving `IEnumerable<T>` needs. Registers it under its own concrete type when `false`. Only on the overload taking an array.
      type: bool
      default: 'true'
    - name: filter
      description: Predicate applied to each discovered type. Only types it accepts are registered. Only on the overload taking an array.
      type: 'Func<Type, bool>?'
      default: 'null'

returns: The same `IServiceCollection` instance with matching discovered types registered.
---

# RegisterOnInherit

Scans assemblies for concrete types assignable to `T` and registers each one, for command handlers, recurring jobs, validation rules, and anything else better discovered than listed by hand. Interfaces and abstract classes are never registered, and neither is a type carrying [`SkipAutoRegistration`](../attributes/skip-auto-registration).

Three overloads trade brevity for control: no assembly scans the calling one, a single assembly scans that, and an array scans each in order and adds `addType` and `filter`. Every overload defaults the lifetime to `ServiceLifetime.Singleton`. Registrations are added rather than replaced, so scanning the same assembly twice registers everything twice.

## Usage

::: code-group

```csharp [CallingAssembly.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .RegisterOnInherit<ICommandHandler>(ServiceLifetime.Transient);
```

```csharp [Filtered.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.RegisterOnInherit<ICommandHandler>(
    [typeof(ImportCommandHandler).Assembly],
    ServiceLifetime.Transient,
    addType: false,
    filter: type => type.Name.EndsWith("CommandHandler")
);
```

:::

::: tip
Several assemblies go in as an array; no overload takes `params`. `addType` and `filter` live only on that overload, so reaching for either means passing the assemblies explicitly.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterOnInherit<T>(
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
) where T : class;

public IServiceCollection RegisterOnInherit<T>(
    Assembly assembly,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
) where T : class;

public IServiceCollection RegisterOnInherit<T>(
    Assembly[] assemblies,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
    bool addType = true,
    Func<Type, bool>? filter = null
) where T : class;
```
