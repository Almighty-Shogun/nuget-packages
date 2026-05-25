---
outline: deep

params:
    - name: stream
      description: Stream containing JSON content to deserialize.
      type: Stream

    - name: options
      description: Optional `System.Text.Json` serializer options. When omitted, the runtime serializer defaults are used.
      type: JsonSerializerOptions?
      default: 'null'

    - name: useDefaultOptions
      description: Whether to use the package default serializer options with camel-case naming.
      type: bool
      default: 'true'

returns: A task that resolves to a deserialized `T` instance, or `null` when the JSON contains `null` for a nullable target.
---

# DeserializeAsync

Deserializes JSON from a stream into a nullable value of type `T`. Use this extension when reading JSON from files, request bodies, network streams, or any source where keeping the payload as a stream is more practical than first loading it into a string.

There are two overloads. Pass `JsonSerializerOptions` when the caller needs exact control over naming, converters, or other serializer behavior. Use the `bool` overload when the caller wants the package default options, which currently use camel-case naming.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using AlmightyShogun.Utils;

await using FileStream stream = File.OpenRead("settings.json");

ExampleSettings? settings = await stream.DeserializeAsync<ExampleSettings>();
```

```csharp [CustomOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    PropertyNameCaseInsensitive = true
};

await using FileStream stream = File.OpenRead("settings.json");

ExampleSettings? settings = await stream.DeserializeAsync<ExampleSettings>(options);
```

```csharp [ExampleSettings.cs]
public sealed record ExampleSettings(string Name, bool Enabled);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public static Task<T?> DeserializeAsync<T>(
    this Stream stream,
    JsonSerializerOptions? options = null
);

public static Task<T?> DeserializeAsync<T>(
    this Stream stream,
    bool useDefaultOptions = true
);
```
