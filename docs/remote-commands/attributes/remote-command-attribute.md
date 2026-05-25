---
outline: deep

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

Marks a class as a remote command and defines the command name used by incoming payloads. `RemoteCommand<T>` reads this attribute to expose the command name to the listener.

Use this attribute on classes that inherit from `RemoteCommand<T>`. The `name` must match the `Command` value sent by remote clients.

## Usage

::: code-group

```csharp [PingCommand.cs]
using System.Net.Sockets;
using AlmightyShogun.RemoteCommands;

[RemoteCommand("ping", "Replies to a health-check command.")]
public sealed class PingCommand : RemoteCommand<PingCommandData>
{
    public override async Task HandleCommandAsync(PingCommandData message, NetworkStream stream)
    {
        await WriteResponseAsync(stream, new
        {
            status = "ok",
            message.RequestId,
            message.SentAt
        });
    }
}
```

```csharp [PingCommandData.cs]
public sealed record PingCommandData(
    string RequestId,
    string Source,
    DateTimeOffset SentAt
);
```

:::

<FrontmatterDocs/>
