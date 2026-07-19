---
params:
    - name: args
      description: Example arguments appended after the command name when metadata is generated.
      type: object[]
      default: '[]'
---

# ExampleAttribute

Adds example input for a command class. [`ConsoleUtils.GetAllCommands`](../types/console-utils#getallcommands) reads this attribute and combines the command name with the provided values to produce the `Example` value on [`ConsoleCommand`](../types/console-command).

Use this attribute when command listing or help output should show a realistic invocation instead of only the generated parameter usage string.

## Usage

```csharp
using Microsoft.Extensions.Logging;
using AlmightyShogun.ConsoleCommands;

[Example("42")]
[ConsoleCommand("user", "Prints user information.")]
public sealed class UserCommand(ILogger<ConsoleCommandBase> logger) : ConsoleCommandBase(logger)
{
    public Task ExecuteAsync(int userId)
    {
        Console.WriteLine($"Loading user {userId}");

        return Task.CompletedTask;
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public ExampleAttribute(params object[] args);
```
