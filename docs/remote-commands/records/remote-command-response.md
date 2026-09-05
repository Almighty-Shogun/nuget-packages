---
fields:
    - name: Refusal
      description: Why the request was declined, or null when it was served. A [`RemoteCommandRefusal`](../exceptions#remotecommandrefusal), sent as its underlying number.
      type: RemoteCommandRefusal?

    - name: Data
      description: Whatever the command wrote, or null when it was refused or ran without writing. Both fields null is the acknowledgement for a command that answered nothing.
      type: JsonElement?
---

# RemoteCommandResponse

The frame the listener sends back for every request, carrying either a refusal or the command's own response. Both travel in one envelope, so a client decides which it received by reading `Refusal` rather than by guessing from the shape.

[`RemoteCommandClient`](../services/remote-command-client) unwraps it and throws a [`RemoteCommandRefusedException`](../exceptions) on a refusal, so this shape matters only to a client written in another language.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record RemoteCommandResponse
{
    public RemoteCommandRefusal? Refusal { get; init; }
    public JsonElement? Data { get; init; }
}
```
