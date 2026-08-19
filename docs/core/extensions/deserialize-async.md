---
params:
    - name: options
      description: Serializer options to apply. Left unset, the package defaults are used, which bind camel-case property names.
      type: JsonSerializerOptions?
      default: 'null'

returns: A task containing the deserialized value, or `null` when the JSON payload resolves to null.
---

# DeserializeAsync

Deserializes a readable stream into `T` without buffering it into a string first, for file and network streams. Use [`TryDeserialize`](./try-deserialize) when the JSON is already in memory.

The stream is read from its current position and is not disposed, and a malformed payload throws `JsonException`, which must be caught at the call site because `out` parameters are not allowed on async methods.

## Usage

::: code-group

```csharp [FileStream.cs]
using AlmightyShogun.Core;

await using FileStream stream = File.OpenRead("orders.json");

Order[]? orders = await stream.DeserializeAsync<Order[]>();
```

```csharp [ExplicitOptions.cs]
using System.Text.Json;
using AlmightyShogun.Core;

JsonSerializerOptions options = new() 
{
    PropertyNameCaseInsensitive = true
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
    JsonSerializerOptions? options = null
);
```
