# ConsoleUtils

Provides small console helpers used by the command runtime and by applications that want command metadata or simple console prompts. The class includes methods for clearing the last console line, asking a non-empty question, and reading all discovered command metadata from loaded assemblies.

Use `ConsoleUtils` when building command help output or when a command needs a simple prompt without taking a dependency on another prompt library.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

List<ConsoleCommand> commands = ConsoleUtils.GetAllCommands();

foreach (ConsoleCommand command in commands)
{
    Console.WriteLine(command.Usage);
}
```

## Methods

- [AskQuestionAsync](./ask-question-async) &mdash; prompts until a value or default value is available.
- [GetAllCommands](./get-all-commands) &mdash; discovers command metadata from loaded assemblies.
- [RemoveLastConsoleLine](./remove-last-console-line) &mdash; clears the previous line from the console.
