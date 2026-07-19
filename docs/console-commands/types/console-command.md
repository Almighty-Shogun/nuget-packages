---
params:
    - name: name
      description: Command name read from the command class metadata.
      type: string

    - name: description
      description: Optional command description read from the command class metadata.
      type: string?

    - name: aliases
      description: Aliases read from the command class metadata.
      type: string[]

    - name: usage
      description: Generated usage text built from the command `ExecuteAsync` parameters.
      type: string

    - name: example
      description: Optional example arguments read from the command class metadata.
      type: string?

returns: Command metadata containing the name, optional description, aliases, generated usage, and optional example.
---

# ConsoleCommand

Represents metadata for a discovered console command. [`ConsoleUtils.GetAllCommands`](./console-utils#getallcommands) creates this value from command class attributes and `ExecuteAsync` parameters so application code can build help output, command listings, or diagnostics.

Use this class as a read model. It does not execute commands; command execution is handled by the registered command handler and by [`ConsoleCommandBase`](./console-command-base).

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

## Type signature

```csharp
public sealed class ConsoleCommand(
    string name,
    string? description,
    string[] aliases,
    string usage,
    string? example
);
```
