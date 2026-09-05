# ConsoleCommandDiscovery

Reads the metadata of every command declared in an assembly, without constructing any of them. Use it to build a `help` listing or a command reference. Reflection runs on every call, so a listing shown often should be built once and held rather than rebuilt each time.

## GetAllCommands

Builds the metadata for each command class, reading the class attributes and the parameters of its `ExecuteAsync` method. A class that breaks the command rules throws `InvalidOperationException` naming it, the same failure [`RegisterConsoleCommands`](../extensions/register-console-commands) raises.

The overload taking no argument scans the calling assembly. Pass assemblies explicitly when the commands live in another project; an empty array yields nothing.

```csharp
using AlmightyShogun.ConsoleCommands;

[Alias("?")]
[ConsoleCommand("help", "Lists the available commands.")]
public sealed class HelpCommand : ConsoleCommandBase
{
    private static readonly IReadOnlyList<ConsoleCommand> _commands =
        ConsoleCommandDiscovery.GetAllCommands();

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

::: warning
This does not apply [`SkipAutoRegistrationAttribute`](/utils/attributes/skip-auto-registration), which registration does. A command carrying that attribute appears in a listing built from here while being unreachable at the prompt.
:::

### Type signature

```csharp
public static IReadOnlyList<ConsoleCommand> GetAllCommands();

public static IReadOnlyList<ConsoleCommand> GetAllCommands(
    Assembly[] assemblies
);
```
