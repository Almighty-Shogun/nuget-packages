# DeserializeExtensions

Extension methods for deserializing JSON strings and streams with `System.Text.Json`. The string overloads are useful for payloads that are already in memory, while the stream overloads support request bodies, files, or network data.

The overloads that accept `useDefaultOptions` can use the package defaults, which currently apply `JsonNamingPolicy.CamelCase`. The overloads that accept `JsonSerializerOptions` let callers fully control deserialization behavior.

## Methods

- [Deserialize](./deserialize) &mdash; deserializes a JSON string to a nullable `T`.
- [DeserializeAsync](./deserialize-async) &mdash; deserializes a JSON stream to a nullable `T`.
