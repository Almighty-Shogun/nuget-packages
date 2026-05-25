---
outline: deep

returns: Command metadata containing the name, optional description, aliases, generated usage, and optional example.
---

# ConsoleCommand

Represents metadata for a discovered console command. `ConsoleUtils.GetAllCommands` creates this value from command class attributes and `ExecuteAsync` parameters so application code can build help output, command listings, or diagnostics. The description is `null` when the command attribute does not provide one.

Use this class as a read model. It does not execute commands; command execution is handled by `ConsoleCommandHandler` and `ConsoleCommandBase`.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

List<ConsoleCommand> commands = ConsoleUtils.GetAllCommands();

foreach (ConsoleCommand command in commands)
{
    Console.WriteLine($"{command.Name}: {command.Description ?? "No description"}");
}
```

<FrontmatterDocs/>
