---
params:
    - name: options
      description: Serializer options passed directly to `System.Text.Json`.
      type: JsonSerializerOptions
    - name: useDefaultOptions
      description: Uses the package default serializer options when `true`; passes `null` options when `false`.
      type: bool
      default: 'true'

returns: A task that resolves to the deserialized value, or `null` when `System.Text.Json` cannot create a value.
---

# DeserializeAsync

Deserializes JSON from a stream into a nullable value of type `T`. Use this method when reading JSON from files, request bodies, network streams, or any other source where loading the entire payload into a string first would be unnecessary.

Pass `JsonSerializerOptions` when the caller needs exact control over serializer behavior. Use the `bool` overload when the caller wants the package default camel-case options, or pass `false` to use the runtime serializer defaults.

## Usage

::: code-group

```csharp [DefaultOptions.cs]
using System.IO;
using AlmightyShogun.Utils;

await using FileStream stream = File.OpenRead("settings.json");

WorkerSettings? settings = await stream.DeserializeAsync<WorkerSettings>();
```

```csharp [CustomOptions.cs]
using System.IO;
using System.Text.Json;
using AlmightyShogun.Utils;

JsonSerializerOptions options = new()
{
    PropertyNameCaseInsensitive = true
};

await using FileStream stream = File.OpenRead("settings.json");

WorkerSettings? settings = await stream.DeserializeAsync<WorkerSettings>(options);
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
extension(Stream stream)
{
    public Task<T?> DeserializeAsync<T>(JsonSerializerOptions options);
    public Task<T?> DeserializeAsync<T>(bool useDefaultOptions = true);
}
```
