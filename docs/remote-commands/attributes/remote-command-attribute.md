---
params:
    - name: name
      description: Command name expected in incoming remote command payloads.
      type: string

    - name: description
      description: Optional description of what the command does.
      type: string?
      default: 'null'
---

# RemoteCommandAttribute

Marks a class as a remote command and names it. Required on every class inheriting [`RemoteCommand<T>`](../types/remote-command), and the `name` must match the `Command` value sent by clients.

## Usage

::: code-group

```csharp [PingCommand.cs]
using AlmightyShogun.RemoteCommands;

[RemoteCommand("ping", "Replies to a health-check command.")]
public sealed class PingCommand : RemoteCommand<PingCommandData>
{
    public override Task HandleCommandAsync(
        PingCommandData message,
        ICommandResponse response,
        CancellationToken cancellationToken
    ) => response.WriteAsync(new 
    {
        status = "ok",
        message.RequestId
    }, cancellationToken);
}
```

```csharp [PingCommandData.cs]
public sealed record PingCommandData(
    string RequestId,
    DateTimeOffset SentAt
);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public RemoteCommandAttribute(
    string name,
    string? description = null
);
```
