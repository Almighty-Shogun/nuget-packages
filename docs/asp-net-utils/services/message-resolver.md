# MessageResolver

Resolves localized HTTP messages from message keys. `AddHttpErrorResponses` registers `IMessageResolver` with the package's JSON-backed implementation, and application code can depend on the interface when it needs to resolve the same messages used by standardized HTTP error responses.

The registered implementation reads JSON files from `messages/{language}` directories, flattens nested objects into dot-separated keys, and falls back from the requested language to the neutral language and then to the configured `DefaultLanguage`. When a message is resolved, the response `Content-Language` header is updated through the package language helpers.

## Usage

::: code-group

```csharp [LocalizedErrors.cs]
using AlmightyShogun.AspNet.Utils;

public sealed class LocalizedErrors(IMessageResolver messageResolver)
{
    public string GetUserNotFoundMessage(int userId)
        => messageResolver.Resolve("users.not-found", [userId]);
}
```

```json [messages/en/users.json]
{
    "not-found": "User {0} was not found."
}
```

:::

## Resolve

Resolves a message by key. The key must match the flattened message-file key, where the file name becomes the root group. For example, `messages/en/users.json` with a `not-found` property resolves as `users.not-found`.

When parameters are supplied, the resolved message is formatted with `string.Format`. Formatting errors return the unformatted template instead of throwing. Missing languages, missing files, and missing keys return the original message key.

```csharp
using AlmightyShogun.AspNet.Utils;

public sealed class ErrorMessageService(IMessageResolver messageResolver)
{
    public string GetLockedMessage(string username)
        => messageResolver.Resolve("users.locked", [username]);
}
```

### Type signature

```csharp
public string Resolve(string key);

public string Resolve(
    string key,
    IReadOnlyList<object?> parameters
);
```
