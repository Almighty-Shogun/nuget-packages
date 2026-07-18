# ApplicationUtils

Static utility type for small application-level helpers. It includes console window behavior and reflection-based type discovery used by packages that register classes from assemblies.

Use these helpers when the application needs to set console state, suppress Ctrl+C shutdown in an interactive process, or discover all loaded types that inherit from a base class or implement an interface.

## Title

Sets the current console window title through `Console.Title`. This is useful for console applications, background worker hosts running in a terminal, or local tooling where the window title should identify the current process.

The method only affects environments that expose a console title. In hosts without an interactive console, the runtime may ignore the title change or apply platform-specific behavior.

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.Title("Import Worker");
```

### Type signature

```csharp
public static void Title(string title);
```

## GetOnInherit

Finds types that inherit from a base class or implement an interface. The method scans the provided assemblies and returns every type where `typeof(T).IsAssignableFrom(type)` is `true`.

Use this method when building discovery-based registration, diagnostics, or metadata tooling. The returned sequence can include interfaces, abstract classes, and the base type itself, so filter the result when only concrete implementations should be used.

::: code-group

```csharp [Program.cs]
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

```csharp [ICommand.cs]
public interface ICommand
{
    Task ExecuteAsync();
}
```

:::

### Type signature

```csharp
public static IEnumerable<Type> GetOnInherit<T>(
    params Assembly[] assemblies
);
```

## PreventCancellation

Prevents Ctrl+C from ending the current console process by subscribing to `Console.CancelKeyPress` and setting `Cancel` to `true`. This is useful for interactive command loops or local development tools that should decide for themselves when to stop.

Call this once during startup before entering a long-running console loop. The method does not stop the event from being raised; it only marks the default cancellation behavior as handled.

```csharp
using AlmightyShogun.Utils;

ApplicationUtils.PreventCancellation();

while (true)
{
    string? input = Console.ReadLine();

    if (input == "exit")
        break;
}
```

### Type signature

```csharp
public static void PreventCancellation();
```
