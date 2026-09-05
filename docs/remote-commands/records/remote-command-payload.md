---
fields:
    - name: Command
      description: Name declared by [`RemoteCommandAttribute`](../attributes/remote-command-attribute) on the handling class.
      type: string
    - name: Data
      description: The command's message, deserialized into `T` by [`RemoteCommand<T>`](../types/remote-command).
      type: JsonElement
    - name: Secret
      description: Pre-shared key, when the server configures one.
      type: string?
      default: 'null'
---

# RemoteCommandPayload

The request frame a client sends, as a big-endian four-byte length prefix followed by that many bytes of UTF-8 JSON in this shape.

[`RemoteCommandClient`](../services/remote-command-client) builds it, so this matters only to a client written in another language.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record RemoteCommandPayload
{
    public string Command { get; init; } = string.Empty;
    public JsonElement Data { get; init; }
    public string? Secret { get; init; }
}
```
