---
fields:
    - name: Name
      description: The name the command is invoked by, which is also what `Usage` and `Example` are prefixed with.
      type: string

    - name: Description
      description: The explanation for a listing, or null when the command was declared without one.
      type: string?

    - name: Aliases
      description: The extra names the command answers to, from [`AliasAttribute`](../attributes/alias-attribute). Empty when it declares none.
      type: 'IReadOnlyList<string>'

    - name: Usage
      description: The full line to type, as the name followed by one `<name:Type>` placeholder per handler parameter. A command taking no arguments yields the bare name.
      type: string

    - name: Example
      description: A complete sample invocation from [`ExampleAttribute`](../attributes/example-attribute), including the command name, or null when the command declares none.
      type: string?
---

# ConsoleCommand

The metadata describing one discovered command, built by [`ConsoleCommandDiscovery.GetAllCommands`](../utilities/console-command-discovery#getallcommands) from a command class's attributes and `ExecuteAsync` parameters. It is a read model, so holding one never constructs the class it came from. Instances come only from that method.

## Usage

```csharp
using AlmightyShogun.ConsoleCommands;

foreach (ConsoleCommand command in ConsoleCommandDiscovery.GetAllCommands())
{
    string description = command.Description ?? string.Empty;

    Console.WriteLine($"{command.Usage} - {description}");
}
```

<FrontmatterDocs/>
