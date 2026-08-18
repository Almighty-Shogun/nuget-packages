# ApplicationUtils

Small application-level helpers for console configuration and assembly type discovery. All members are static; the type is not registered in the container and is used directly from startup code.

## Usage

```csharp
using System.Reflection;
using AlmightyShogun.Utils;

ApplicationUtils.Title("Importer");
ApplicationUtils.PreventCancellation();

IEnumerable<Type> steps = ApplicationUtils
    .GetOnInherit<IImportStep>(Assembly.GetExecutingAssembly());
```

## Title

Sets the console window title. A thin wrapper over `Console.Title`, provided so startup code reads consistently alongside the other helpers.

Setting the title is not supported on every platform and terminal. Where it is unsupported the value is ignored rather than throwing.

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.Title("Importer");
```

### Type signature

```csharp
public static void Title(string title);
```

## GetOnInherit

Returns the concrete types in the given assemblies that are assignable to `T`. Interfaces and abstract classes are filtered out, so every returned type can be instantiated.

When no assemblies are passed, the calling assembly is scanned. An assembly whose types cannot all be loaded contributes the types that did load instead of failing the scan, so one broken dependency does not stop discovery.

```csharp
using System.Reflection;
using AlmightyShogun.Utils;

IEnumerable<Type> steps = ApplicationUtils
    .GetOnInherit<IImportStep>(Assembly.GetExecutingAssembly());
```

::: tip
This is a raw reflection primitive with no dependency-injection semantics. It does **not** honor [`SkipAutoRegistration`](../attributes/skip-auto-registration); only [`RegisterOnInherit`](../extensions/register-on-inherit) applies that attribute.
:::

### Type signature

```csharp
public static IEnumerable<Type> GetOnInherit<T>(
    params Assembly[] assemblies
);
```

## PreventCancellation

Stops `Ctrl+C` from terminating the process, so a console application can handle shutdown itself.

Repeated calls have no additional effect. The handler is attached once for the lifetime of the process, so calling this from several entry points does not stack handlers.

There is no matching method to restore the default behavior. Once cancellation is prevented it stays prevented until the process exits.

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.PreventCancellation();
```

### Type signature

```csharp
public static void PreventCancellation();
```
