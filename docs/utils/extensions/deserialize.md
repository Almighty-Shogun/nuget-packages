---
params:
    - name: options
      description: Serializer options passed directly to `System.Text.Json`.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Uses the package default serializer options when `true`; passes `null` options when `false`.
      type: bool
      default: 'true'

returns: The deserialized value, or `null` when `System.Text.Json` cannot create a value.
---

# Deserialize

Deserializes a JSON string into a nullable value of type `T`. Use this method when the full JSON payload is already available in memory and the caller wants a compact wrapper around `JsonSerializer.Deserialize<T>`.

Pass `JsonSerializerOptions` when the caller needs exact control over property naming, converters, casing, or other serializer behavior. Use the `bool` overload when the caller wants the package defaults; the default options currently use camel-case property naming.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using AlmightyShogun.Utils;

string json = """
{
    "name": "Importer",
    "enabled": true
}
""";

WorkerSettings? settings = json.Deserialize<WorkerSettings>();
```

```csharp [CustomOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    PropertyNameCaseInsensitive = true
};

string json = """
{
    "Name": "Importer",
    "Enabled": true
}
""";

WorkerSettings? settings = json.Deserialize<WorkerSettings>(options);
```

```csharp [WorkerSettings.cs]
public sealed record WorkerSettings(string Name, bool Enabled);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public T? Deserialize<T>(JsonSerializerOptions options);

public T? Deserialize<T>(bool useDefaultOptions = true);
```
