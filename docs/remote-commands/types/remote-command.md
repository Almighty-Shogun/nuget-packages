# RemoteCommand

Base type for typed remote commands. A command class inherits from it, specifies its message type as `T`, and implements `HandleCommandAsync`. The base reads [`RemoteCommandAttribute`](../attributes/remote-command-attribute) for the command name and deserializes the incoming JSON into `T` before dispatching.

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
    ) => response.WriteAsync(
        new PingCommandResponse(
            "ok",
            message.RequestId,
            DateTimeOffset.UtcNow
        ),
        cancellationToken
    );
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

::: warning
A class must declare [`RemoteCommandAttribute`](../attributes/remote-command-attribute), because the name on it is what [`RegisterRemoteCommands`](../extensions/register-remote-commands) records. A class without one throws `InvalidOperationException` during registration, so a missing attribute stops startup rather than reaching the first request.
:::

## HandleCommandAsync

Handles the command after the incoming JSON payload has been deserialized into `T`. Write a reply through [`ICommandResponse`](../services/command-response) when the client expects one; a command that returns nothing simply completes.

Deserialization failure never reaches this method. A payload whose `Data` does not produce a `T` raises a `JsonException` before dispatch, which the handler turns into an `invalid_message` [`RemoteCommandResponse`](../records/remote-command-response).

### Type signature

```csharp
public abstract Task HandleCommandAsync(
    T message,
    ICommandResponse response,
    CancellationToken cancellationToken = default
);
```

## CommandName

The command name declared on the class, read once in the constructor. Available to derived classes for logging and error messages, so a command does not have to reflect on its own attribute again.

::: code-group

```csharp [ReindexCommand.cs]
using Microsoft.Extensions.Logging;
using AlmightyShogun.RemoteCommands;

[RemoteCommand("reindex")]
public sealed class ReindexCommand(
    ILogger<ReindexCommand> logger
) : RemoteCommand<ReindexCommandData>
{
    public override Task HandleCommandAsync(
        ReindexCommandData message,
        ICommandResponse response,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Running {Command} for {Index}",
            CommandName,
            message.Index
        );

        return response.WriteAsync(
            new { status = "queued" },
            cancellationToken
        );
    }
}
```

```csharp [ReindexCommandData.cs]
public sealed record ReindexCommandData(string Index);
```

:::

### Type signature

```csharp
protected string CommandName { get; }
```
