# TypeDiscovery

Finds concrete implementations across assemblies by reflection, which is what the registration helpers in this package are built on. It is a raw primitive with no dependency-injection semantics: nothing here reads an attribute or decides a lifetime.

## Usage

```csharp
using AlmightyShogun.Core;

IEnumerable<Type> steps = TypeDiscovery.FindAssignableTypes<IImportStep>();
```

## FindAssignableTypes

Returns the concrete types assignable to `T`. Interfaces and abstract classes are filtered out, so every returned type can be instantiated.

Three overloads cover the usual cases: no argument scans the calling assembly, a single assembly scans that one, and an array scans each in the order given. An assembly whose types cannot all be loaded contributes the types that did load instead of failing the scan, so one broken dependency does not stop discovery. The sequence is lazy, so the reflection work happens as it is enumerated rather than when the method returns.

```csharp
using System.Reflection;
using AlmightyShogun.Core;

IEnumerable<Type> local = TypeDiscovery.FindAssignableTypes<IImportStep>();

IEnumerable<Type> contracts = TypeDiscovery
    .FindAssignableTypes<IImportStep>(typeof(IImportStep).Assembly);

IEnumerable<Type> all = TypeDiscovery.FindAssignableTypes<IImportStep>(
    [Assembly.GetExecutingAssembly(), typeof(IImportStep).Assembly]
);
```

::: tip
This is a raw reflection primitive with no dependency-injection semantics. It does **not** honor [`SkipAutoRegistration`](../attributes/skip-auto-registration); only [`RegisterOnInherit`](../extensions/register-on-inherit) applies that attribute.
:::

### Type signature

```csharp
public static IEnumerable<Type> FindAssignableTypes<T>();

public static IEnumerable<Type> FindAssignableTypes<T>(
    Assembly assembly
);

public static IEnumerable<Type> FindAssignableTypes<T>(
    Assembly[] assemblies
);
```
