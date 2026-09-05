---
params:
    - name: options
      description: Serializer options to apply. Left unset, the package defaults are used, which bind an ASP.NET Core payload on the same rules the API serialized it with, meaning camel-case naming, case-insensitive property matching, and numbers accepted from JSON strings.
      type: JsonSerializerOptions?
      default: 'null'
    - name: cancellationToken
      description: Stops the read part way through. The stream is left at wherever reading reached, so a cancelled call leaves it unusable for a second attempt unless the caller can rewind it.
      type: CancellationToken
      default: default

returns: A task containing the deserialized value, or `null` when the JSON payload resolves to null.
---

# DeserializeAsync

Deserializes a readable stream into `T` without buffering it into a string first, for file and network streams. Use [`TryDeserialize`](./try-deserialize) when the JSON is already in memory.

The stream is read from its current position and is not disposed, and a malformed payload throws `JsonException`, which must be caught at the call site because `out` parameters are not allowed on async methods.

## Usage

::: code-group

```csharp [FileStream.cs]
using AlmightyShogun.Utils;

await using FileStream stream = File.OpenRead("orders.json");

Order[]? orders = await stream.DeserializeAsync<Order[]>(
    cancellationToken: cancellationToken
);
```

```csharp [ExplicitOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
};

Order[]? orders = await stream.DeserializeAsync<Order[]>(options);
```

```csharp [Order.cs]
public sealed record Order(int OrderId, string CustomerName);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public Task<T?> DeserializeAsync<T>(
    JsonSerializerOptions? options = null,
    CancellationToken cancellationToken = default
);
```
