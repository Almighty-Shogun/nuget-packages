# ConsoleUtils

Provides small console helpers used by the command runtime and by applications that want command metadata or simple console prompts. The type includes methods for clearing the last console line, asking a non-empty question, and reading all discovered command metadata from loaded assemblies.

Use `ConsoleUtils` when building command help output or when a command needs a small prompt without taking a dependency on another prompt library.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

List<ConsoleCommand> commands = ConsoleUtils.GetAllCommands();

foreach (ConsoleCommand command in commands)
{
    Console.WriteLine(command.Usage);
}
```

## AskQuestionAsync

Writes a question prompt to the console and waits until a non-null answer is available. When `defaultValue` is provided, pressing enter without typing a value returns that default.

Use this helper inside command handlers that need a small interactive prompt. After reading input, the method clears the prompt line with [`RemoveLastLine`](#removelastline).

```csharp
using AlmightyShogun.ConsoleCommands;

string environment = await ConsoleUtils.AskQuestionAsync(
    "Which environment should be deployed?",
    defaultValue: "development"
);
```

### Type signature

```csharp
public static Task<string> AskQuestionAsync(
    string question,
    string? defaultValue = null
);
```

## GetAllCommands

Discovers command metadata from currently loaded assemblies. The method finds concrete command implementations marked with [`ConsoleCommandAttribute`](../attributes/console-command-attribute), reads class-level aliases and examples, inspects the public `ExecuteAsync` method, and returns [`ConsoleCommand`](./console-command) values containing names, descriptions, aliases, generated usage, and examples.

Use this method to build help output or command listings. It reads loaded assemblies from the current application domain, so command assemblies must already be loaded for their metadata to appear.

```csharp
using AlmightyShogun.ConsoleCommands;

List<ConsoleCommand> commands = ConsoleUtils.GetAllCommands();

foreach (ConsoleCommand command in commands)
{
    Console.WriteLine($"{command.Name} - {command.Description}");
}
```

### Type signature

```csharp
public static List<ConsoleCommand> GetAllCommands();
```

## RemoveLastLine

Clears the previous line in the console and moves the cursor back to the start of that cleared line. The command runtime uses this to remove entered command text or prompt text after it has been processed.

Use this helper when a command wants to keep the console output tidy after reading temporary input. It assumes the console cursor can be moved, so it is best suited for interactive terminals rather than redirected output.

```csharp
using AlmightyShogun.ConsoleCommands;

Console.WriteLine("Temporary status");
ConsoleUtils.RemoveLastLine();
```

### Type signature

```csharp
public static void RemoveLastLine();
```
