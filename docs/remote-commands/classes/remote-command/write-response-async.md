---
outline: deep

params:
    - name: stream
      description: Network stream connected to the remote client.
      type: NetworkStream

    - name: data
      description: Response object that should be serialized to JSON and sent to the client.
      type: object

returns: A task representing the asynchronous write and flush operation.
---

# WriteResponseAsync

Writes a JSON response object to the connected remote client. The method serializes the supplied object with `System.Text.Json`, writes the UTF-8 bytes to the provided `NetworkStream`, and flushes the stream so the client can read the response immediately.

Use this helper inside `HandleCommandAsync` when a command should return a small structured response after it has processed the incoming payload. The method does not add a length prefix to the response, so the client side should read the JSON response according to the protocol used by the application.

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
        await WriteResponseAsync(stream, new PingCommandResponse(
            "ok",
            message.RequestId,
            DateTimeOffset.UtcNow
        ));
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

```csharp [PingCommandResponse.cs]
public sealed record PingCommandResponse(
    string Status,
    string RequestId,
    DateTimeOffset ReceivedAt
);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
protected Task WriteResponseAsync(
    NetworkStream stream,
    object data
);
```
