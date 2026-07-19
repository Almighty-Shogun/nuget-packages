---
params:
    - name: stream
      description: Writable stream that receives the serialized JSON payload.
      type: Stream
    - name: options
      description: Serializer options passed directly to `System.Text.Json`.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Uses the package default serializer options when `true`; passes `null` options when `false`.
      type: bool
      default: 'true'

returns: A task that completes when the value has been serialized to the stream.
---

# SerializeAsync

Serializes a value of type `T` into a writable stream. Use this method when writing JSON to a file, response body, network stream, or another destination where creating an intermediate string is unnecessary.

Pass `JsonSerializerOptions` when the caller needs exact control over serializer behavior. Use the `bool` overload when the caller wants the package default camel-case options, or pass `false` to use the runtime serializer defaults.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using System.IO;
using AlmightyShogun.Utils;

WorkerSettings settings = new("Importer", true);

await using FileStream stream = File.Create("settings.json");

await settings.SerializeAsync(stream);
```

```csharp [CustomOptions.cs]
using System.IO;
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    WriteIndented = true
};

WorkerSettings settings = new("Importer", true);

await using FileStream stream = File.Create("settings.json");

await settings.SerializeAsync(stream, options);
```

```json [settings.json]
{
    "name": "Importer",
    "enabled": true
}
```

```csharp [WorkerSettings.cs]
public sealed record WorkerSettings(string Name, bool Enabled);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public Task SerializeAsync(Stream stream, JsonSerializerOptions options);

public Task SerializeAsync(Stream stream, bool useDefaultOptions = true);
```
