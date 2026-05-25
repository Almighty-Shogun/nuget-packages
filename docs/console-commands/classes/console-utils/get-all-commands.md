---
outline: deep

returns: A list of discovered `ConsoleCommand` metadata values.
---

# GetAllCommands

Discovers command metadata from currently loaded assemblies. The method finds concrete `IConsoleCommand` implementations marked with `ConsoleCommandAttribute`, reads class-level aliases and examples, inspects the public `ExecuteAsync` method, and returns `ConsoleCommand` values containing names, descriptions, aliases, generated usage, and examples.

Use this method to build help output or command listings. It reads loaded assemblies from the current application domain, so command assemblies must already be loaded for their metadata to appear.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

List<ConsoleCommand> commands = ConsoleUtils.GetAllCommands();

foreach (ConsoleCommand command in commands)
{
    Console.WriteLine($"{command.Name} - {command.Description}");
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public static List<ConsoleCommand> GetAllCommands();
```

## Uses

- [ConsoleCommand](../console-command)
- [ConsoleCommandAttribute](../../attributes/console-command-attribute)
- [AliasAttribute](../../attributes/alias-attribute)
- [ExampleAttribute](../../attributes/example-attribute)
