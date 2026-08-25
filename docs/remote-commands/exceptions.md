# Exceptions

[`RemoteCommandClient`](./services/remote-command-client) throws four exceptions, all deriving from `RemoteCommandException` so a single
`catch` covers a command that did not run. The listener itself throws none: it answers a bad request with a [
`RemoteCommandResponse`](./records/remote-command-response) instead.

Which type is thrown says whether the problem was the connection, the wire format, or the server's own decision.

| Exception                            | Cause                                           | Retry      |
|--------------------------------------|-------------------------------------------------|------------|
| `RemoteCommandUnreachableException`  | The connection could not be opened              | Reasonable |
| `RemoteCommandDisconnectedException` | Closed before a response arrived                | Reasonable |
| `RemoteCommandProtocolException`     | A frame arrived that is not a response envelope | Pointless  |
| `RemoteCommandRefusedException`      | The server answered and declined                | Pointless  |

## Usage

```csharp
using AlmightyShogun.RemoteCommands;

try
{
    await client.SendAsync(
        "restart",
        new RestartMessage(),
        cancellationToken
    );
}
catch (RemoteCommandUnreachableException exception)
{
    logger.LogWarning(
        "No listener at {Host}:{Port}",
        exception.Host,
        exception.Port
    );
}
catch (RemoteCommandRefusedException exception)
{
    logger.LogError(
        "The server refused the command: {Reason}",
        exception.Reason
    );
}
```

## RemoteCommandUnreachableException

The connection could never be opened, so nothing was sent. The listener is down, the port is wrong, or a network rule dropped the attempt.
`Host` and `Port` carry what was dialled, and the socket failure is the inner exception.

### Type signature

```csharp
public sealed class RemoteCommandUnreachableException(
    string host,
    int port,
    Exception innerException
) : RemoteCommandException
{
    public string Host { get; }
    public int Port { get; }
}
```

## RemoteCommandDisconnectedException

The connection opened but closed before a response arrived. The usual cause is the address not being in [`Whitelisted`](./configuration),
because the listener drops such a client without answering rather than explaining itself.

### Type signature

```csharp
public sealed class RemoteCommandDisconnectedException(
    Exception? innerException = null
) : RemoteCommandException;
```

## RemoteCommandProtocolException

A frame arrived that is not a [`RemoteCommandResponse`](./records/remote-command-response) envelope, which means the two ends disagree about
the wire format rather than that the command failed. Against a server running this package this is a bug; against another implementation it
means the envelope is not being produced.

### Type signature

```csharp
public sealed class RemoteCommandProtocolException(
    string message
) : RemoteCommandException;
```

## RemoteCommandRefusedException

The server answered and declined. `Reason` says what it objected to, and the message is derived from it, so a caller switches on a value and
never parses text.

### Type signature

```csharp
public sealed class RemoteCommandRefusedException(
    RemoteCommandRefusal reason
) : RemoteCommandException
{
    public RemoteCommandRefusal Reason { get; }
}
```

## RemoteCommandRefusal

The reason on a refusal, and the only place the package names one. It travels on the wire as its underlying number, so neither side spells a
code, and a value introduced by a newer server is read as `Other` rather than rejected.

::: tip
`InvalidMessage` covers a genuine conflict only. A property the payload omits binds to the default, so a message missing a field reaches the
command with that field null rather than being refused.
:::

| Member               | Value | Meaning                                         |
|----------------------|-------|-------------------------------------------------|
| `Other`              | `0`   | A reason this client has no name for            |
| `MalformedPayload`   | `1`   | The request was not readable as JSON            |
| `MissingCommandName` | `2`   | The request parsed but named no command         |
| `Unauthorized`       | `3`   | The pre-shared key was missing or wrong         |
| `CommandNotFound`    | `4`   | No command is registered under that name        |
| `InvalidMessage`     | `5`   | The data did not fit the command's message type |

### Type signature

```csharp
public enum RemoteCommandRefusal
{
    Other = 0,
    MalformedPayload,
    MissingCommandName,
    Unauthorized,
    CommandNotFound,
    InvalidMessage
}
```
