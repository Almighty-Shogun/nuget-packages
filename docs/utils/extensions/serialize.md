---
params:
    - name: options
      description: Serializer options passed directly to `System.Text.Json`.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Uses the package default serializer options when `true`; passes `null` options when `false`.
      type: bool
      default: 'true'

returns: The JSON string produced by `System.Text.Json`.
---

# Serialize

Serializes a value of type `T` into a JSON string. Use this method when the caller needs a compact wrapper around `JsonSerializer.Serialize` and wants the same default camel-case behavior as the package deserialization helpers.

Pass `JsonSerializerOptions` when the caller needs exact control over property naming, converters, casing, indentation, or other serializer behavior. Use the `bool` overload when the caller wants the package defaults, or pass `false` to use the runtime serializer defaults.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using AlmightyShogun.Utils;

WorkerSettings settings = new("Importer", true);

string json = settings.Serialize();
```

```csharp [CustomOptions.cs]
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    WriteIndented = true
};

WorkerSettings settings = new("Importer", true);

string json = settings.Serialize(options);
```

```csharp [WorkerSettings.cs]
public sealed record WorkerSettings(string Name, bool Enabled);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public string Serialize(JsonSerializerOptions options);

public string Serialize(bool useDefaultOptions = true);
```
