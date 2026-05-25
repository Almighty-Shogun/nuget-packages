# RemoteCommand&lt;T&gt;

Base class for typed remote commands. A command class should inherit from this type, specify the message type as `T`, and implement `HandleCommandAsync`.

The base class reads `RemoteCommandAttribute` to expose the command name and uses the package's internal execution contract to deserialize incoming JSON into the typed message. Application code should inherit from this class; it should not call the internal raw execution path directly.

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

## Methods

- [HandleCommandAsync](./handle-command-async) &mdash; handles a deserialized command message.
- [WriteResponseAsync](./write-response-async) &mdash; writes a JSON response object to the connected client.
