# ConsoleUtils

Reads the metadata of every command declared in an assembly, without constructing any of them. Use it to build a `help` listing or a command reference.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

foreach (ConsoleCommand command in ConsoleUtils.GetAllCommands())
{
    Console.WriteLine(command.Usage);
}
```

## GetAllCommands

Builds the metadata for each valid command class, reading the class attributes and the parameters of its `ExecuteAsync` method. A class that carries [`ConsoleCommandAttribute`](../attributes/console-command-attribute) but breaks the handler-method rules is skipped, so this never throws on a malformed command.

The overload taking no argument scans the calling assembly. Pass assemblies explicitly when the commands live in another project; an empty array yields nothing.

::: tip
Reflection runs on every call. A `help` command that runs often should build the listing once and hold it rather than calling this each time.
:::

```csharp
using AlmightyShogun.ConsoleCommands;

[Alias("?")]
[ConsoleCommand("help", "Lists the available commands.")]
public sealed class HelpCommand : ConsoleCommandBase
{
    private static readonly IReadOnlyList<ConsoleCommand> _commands =
        ConsoleUtils.GetAllCommands();

    public Task ExecuteAsync()
    {
        foreach (ConsoleCommand command in _commands)
        {
            Console.WriteLine(command.Usage);

            if (command.Description is not null)
                Console.WriteLine($"    {command.Description}");
        }

        return Task.CompletedTask;
    }
}
```

### Type signature

```csharp
public static IReadOnlyList<ConsoleCommand> GetAllCommands();

public static IReadOnlyList<ConsoleCommand> GetAllCommands(
    Assembly[] assemblies
);
```
