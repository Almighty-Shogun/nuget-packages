# RemoteCommand&lt;T&gt;

Base type for typed remote commands. A command class should inherit from this type, specify the message type as `T`, and implement `HandleCommandAsync`.

The base type reads [`RemoteCommandAttribute`](../attributes/remote-command-attribute) to expose the command name and uses the package's internal execution contract to deserialize incoming JSON into the typed message. Application code should inherit from this type; it should not call the internal raw execution path directly.

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
    DateTimeOffset SentAt
);
```

:::

## HandleCommandAsync

Handles a remote command after the package has deserialized the incoming JSON payload into the command's message type. Implement this method in each command class to perform the command behavior and optionally write a response to the connected network stream.

Use `WriteResponseAsync` from the base type when the command should send a JSON response object back to the client.

### Type signature

```csharp
public abstract Task HandleCommandAsync(T message, NetworkStream stream);
```

## WriteResponseAsync

Writes a JSON response object to the connected remote client. The method serializes the supplied object with `System.Text.Json`, writes the UTF-8 bytes to the provided `NetworkStream`, and flushes the stream so the client can read the response immediately.

Use this helper inside `HandleCommandAsync` when a command should return a small structured response after it has processed the incoming payload. The method does not add a length prefix to the response, so the client side should read the JSON response according to the protocol used by the application.

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

### Type signature

```csharp
protected Task WriteResponseAsync(
    NetworkStream stream,
    object data
);
```
