---
outline: deep

params:
    - name: json
      description: JSON string to deserialize.
      type: string

    - name: options
      description: Optional `System.Text.Json` serializer options. When omitted, the runtime serializer defaults are used.
      type: JsonSerializerOptions?
      default: 'null'

    - name: useDefaultOptions
      description: Whether to use the package default serializer options with camel-case naming.
      type: bool
      default: 'true'

returns: A deserialized `T` instance, or `null` when the JSON contains `null` for a nullable target.
---

# Deserialize

Deserializes a JSON string into a nullable value of type `T`. Use this extension when the complete JSON payload is already available as a string and the caller wants a compact wrapper around `JsonSerializer.Deserialize<T>`.

There are two overloads. Pass `JsonSerializerOptions` when the caller needs exact control over naming, converters, or other serializer behavior. Use the `bool` overload when the caller wants the package default options, which currently use camel-case naming.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using AlmightyShogun.Utils;

string json = """
{
    "name": "Akari",
    "enabled": true
}
""";

ExampleSettings? settings = json.Deserialize<ExampleSettings>();
```

```csharp [CustomOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    PropertyNameCaseInsensitive = true
};

ExampleSettings? settings = json.Deserialize<ExampleSettings>(options);
```

```csharp [ExampleSettings.cs]
public sealed record ExampleSettings(string Name, bool Enabled);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public static T? Deserialize<T>(
    this string json,
    JsonSerializerOptions? options = null
);

public static T? Deserialize<T>(
    this string json,
    bool useDefaultOptions = true
);
```
