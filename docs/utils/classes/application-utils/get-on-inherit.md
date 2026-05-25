---
outline: deep

params:
    - name: assemblies
      description: Assemblies to scan. When omitted, the calling assembly is scanned.
      type: Assembly[]
      default: '[]'

returns: A sequence of types assignable to `T` from the selected assemblies.
---

# GetOnInherit

Finds types that inherit from a base class or implement an interface. The method scans the provided assemblies and returns every type where `typeof(T).IsAssignableFrom(type)` is `true`.

Use this method when building discovery-based registration, diagnostics, or metadata tooling. The returned sequence can include interfaces, abstract classes, and the base type itself, so filter the result when only concrete implementations should be used.

## Usage

```csharp
using System.Reflection;
using AlmightyShogun.Utils;

IEnumerable<Type> commandTypes = ApplicationUtils.GetOnInherit<ICommand>(
    Assembly.GetExecutingAssembly()
);

foreach (Type commandType in commandTypes.Where(type => type is { IsInterface: false, IsAbstract: false }))
{
    Console.WriteLine(commandType.FullName);
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public static IEnumerable<Type> GetOnInherit<T>(
    params Assembly[] assemblies
);
```
