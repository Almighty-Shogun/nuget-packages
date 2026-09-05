---
fields:
    - name: StatusCode
      description: Status the response is sent with, and the value the handler passes to the response writer so the error body's `Code` repeats it.
      type: int

    - name: Code
      description: Machine-readable identifier the client branches on, such as `invalid_credentials`. Treat it as public API, because renaming it breaks consumers without breaking a build.
      type: string

    - name: MessageKey
      description: Key resolved through the message resolver for the description. A key no message file defines reaches the client verbatim, so it should read as a key rather than as prose.
      type: string

    - name: MessageParameters
      description: Values substituted into the resolved template by position, as `{0}` onwards. Pass an empty list when the message takes none.
      type: 'IReadOnlyList<object?>'
---

# ErrorMapping

What one exception becomes on the wire, returned by [`IExceptionMapper`](../exceptions) and consumed by the handler that owns it. It carries the whole presentation decision for a failure, which is what keeps the exception itself free of HTTP and localization detail.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ErrorMapping
{
    public required int StatusCode { get; init; }
    public required string Code { get; init; }
    public required string MessageKey { get; init; }
    public required IReadOnlyList<object?> MessageParameters { get; init; }
}
```
