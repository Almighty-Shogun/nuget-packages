---
params:
    - name: result
      description: When the method returns `true`, contains the deserialized value, annotated so the compiler treats it as non-null from that point. Left at the default for `T` otherwise.
      type: 'out T?'
    - name: options
      description: Serializer options to apply. Left unset, the package defaults are used, which bind camel-case property names.
      type: JsonSerializerOptions?
      default: 'null'

returns: '`true` when a non-null value was read; otherwise `false`.'
---

# TryDeserialize

Deserializes a JSON string into `T` without throwing on invalid input, for a request body, queue message, or user-supplied file where malformed JSON is an expected outcome.

Returns `false` for a malformed payload and for the JSON literal `null`, so a `true` result always yields something usable; any other failure still throws, so a genuine programming error is not swallowed. `result` comes first because an `out` parameter cannot follow an optional one, and there is no stream equivalent because `out` is not allowed on async methods, so a stream uses [`DeserializeAsync`](./deserialize-async).

## Usage

::: code-group

```csharp [TryDeserialize.cs]
using AlmightyShogun.Utils;

if (!payload.TryDeserialize(out Order? order))
{
    logger.LogWarning("Discarded a message that was not valid JSON.");

    return;
}

await ProcessAsync(order);
```

```csharp [ExplicitOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new() 
{
    PropertyNameCaseInsensitive = true
};

bool parsed = payload.TryDeserialize(out Order? order, options);
```

```csharp [Order.cs]
public sealed record Order(int OrderId, string CustomerName);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public bool TryDeserialize<T>(
    [NotNullWhen(true)] out T? result,
    JsonSerializerOptions? options = null
);
```
