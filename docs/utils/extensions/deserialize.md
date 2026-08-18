---
params:
    - name: options
      description: Serializer options to apply, in place of the package defaults.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Applies the package default options, which use camel-case property naming, when `true`. Uses the `System.Text.Json` defaults when `false`.
      type: bool
      default: 'true'

returns: The deserialized value, or `null` when the JSON payload resolves to null.
---

# Deserialize

Deserializes a JSON string into `T`. The parameterless form applies camel-case property naming, so a payload produced by an ASP.NET Core API binds without extra configuration. A malformed payload throws `JsonException`; use [`TryDeserialize`](./try-deserialize) when invalid input is expected.

## Usage

::: code-group

```csharp [Default.cs]
using AlmightyShogun.Utils;

string json = """{"orderId":42,"customerName":"Ada"}""";

Order? order = json.Deserialize<Order>();
```

```csharp [ExplicitOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new() 
{
    PropertyNameCaseInsensitive = true
};

Order? order = json.Deserialize<Order>(options);
```

```csharp [Order.cs]
public sealed record Order(int OrderId, string CustomerName);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public T? Deserialize<T>(JsonSerializerOptions options);

public T? Deserialize<T>(bool useDefaultOptions = true);
```
