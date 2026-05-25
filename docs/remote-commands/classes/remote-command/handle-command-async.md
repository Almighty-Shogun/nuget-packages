---
outline: deep

params:
    - name: message
      description: Deserialized command message from the payload `Data` value.
      type: T

    - name: stream
      description: Network stream connected to the remote client.
      type: NetworkStream

returns: A task representing the asynchronous command handling operation.
---

# HandleCommandAsync

Handles a remote command after the package has deserialized the incoming JSON payload into the command's message type. Implement this method in each command class to perform the command behavior and optionally write a response to the connected network stream.

Use `WriteResponseAsync` from the base class when the command should send a JSON response object back to the client.

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

## Type signature

```csharp
public abstract Task HandleCommandAsync(T message, NetworkStream stream);
```
