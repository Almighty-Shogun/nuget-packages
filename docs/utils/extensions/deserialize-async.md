---
params:
    - name: options
      description: Serializer options to apply, in place of the package defaults.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Applies the package default options, which use camel-case property naming, when `true`. Uses the `System.Text.Json` defaults when `false`.
      type: bool
      default: 'true'

returns: A task containing the deserialized value, or `null` when the JSON payload resolves to null.
---

# DeserializeAsync

Deserializes a readable stream into `T` without buffering it into a string first, for file and network streams. Use [`Deserialize`](./deserialize) when the JSON is already in memory.

The stream is read from its current position and is not disposed, and a malformed payload throws `JsonException`, which must be caught at the call site because `out` parameters are not allowed on async methods.

## Usage

::: code-group

```csharp [FileStream.cs]
using AlmightyShogun.Utils;

await using FileStream stream = File.OpenRead("orders.json");

Order[]? orders = await stream.DeserializeAsync<Order[]>();
```

```csharp [ExplicitOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

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
public Task<T?> DeserializeAsync<T>(JsonSerializerOptions options);

public Task<T?> DeserializeAsync<T>(bool useDefaultOptions = true);
```
