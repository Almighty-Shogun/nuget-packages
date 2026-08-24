# AspNet Localization

Resolves human-readable messages in the language a request asks for. Message text lives in JSON files on disk rather than in resource assemblies, so a wording change is a file edit and a translator never needs the solution.

Language negotiation reads `Accept-Language` and falls back to a configured default, so a caller that sends no preference still gets a complete message. The negotiated language is reported back on the response through `Content-Language`.

Adopting it is one registration and, optionally, one middleware call. `AlmightyShogun.AspNet.Utils` depends on this package to localize the description on every error body, so an application using standardized error responses already has it.

## Categories

- [Configuration](./configuration) &mdash; the optional `Localization` section.
- [Localization](./localization) &mdash; how message files are laid out, resolved, and negotiated per request.
- [Extensions](./extensions/add-message-localization) &mdash; registration, middleware, and the language negotiation helpers.
- [Services](./services/message-resolver) &mdash; message resolution and language negotiation.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Localization;

builder.Services.AddMessageLocalization(builder.Configuration);

WebApplication app = builder.Build();

app.UseMessageLocalization();
```

```csharp [SessionsController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Localization;

[ApiController]
[Route("sessions")]
public sealed class SessionsController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
        => Ok(messageResolver.Resolve("auth.failed"));
}
```

```json [messages/en/auth.json]
{
    "failed": "Authentication failed",
    "expired": "Your session expired at {0}"
}
```

:::
