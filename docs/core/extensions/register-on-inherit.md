---
params:
    - name: assemblies
      description: The assemblies to scan, in the order they should be searched. An empty array registers nothing, and the overload taking none scans the calling assembly.
      type: 'Assembly[]'
    - name: serviceLifetime
      description: Lifetime used for each discovered service registration.
      type: ServiceLifetime
      default: ServiceLifetime.Singleton
    - name: addType
      description: Registers each discovered implementation under service type `T` when `true`, which is what a consumer resolving `IEnumerable<T>` needs. Registers it under its own concrete type when `false`. Only on the overload taking assemblies.
      type: bool
      default: 'true'
    - name: filter
      description: Predicate applied to each discovered type. Only types it accepts are registered. Only on the overload taking assemblies.
      type: 'Func<Type, bool>?'
      default: 'null'

returns: The same `IServiceCollection` instance with matching discovered types registered.
---

# RegisterOnInherit

Scans assemblies for concrete types assignable to `T` and registers each one, for command handlers, recurring jobs, validation rules, and anything else better discovered than listed by hand. Interfaces and abstract classes are never registered, and neither is a type carrying [`SkipAutoRegistration`](../attributes/skip-auto-registration).

Two overloads trade brevity for control: passing no assembly scans the calling one, while passing an array scans each in order and opens up `addType` and `filter`. Both default the lifetime to `ServiceLifetime.Singleton`. Registrations are added rather than replaced, so scanning the same assembly twice registers everything twice.

## Usage

::: code-group

```csharp [CallingAssembly.cs]
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .RegisterOnInherit<ICommandHandler>(ServiceLifetime.Transient);
```

```csharp [Filtered.cs]
using AlmightyShogun.Core;
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
Assemblies always go in as an array; no overload takes a single `Assembly` or `params`. `addType` and `filter` live only there, so reaching for either means passing the assemblies explicitly.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterOnInherit<T>(
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton
) where T : class;

public IServiceCollection RegisterOnInherit<T>(
    Assembly[] assemblies,
    ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
    bool addType = true,
    Func<Type, bool>? filter = null
) where T : class;
```
