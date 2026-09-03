# ASP.NET Request Validation

Validates request models through attributes, fluent rules, or both. Failures are collected per field and returned as a single `422` response with localized messages, so a client sees every problem at once rather than one at a time.

Works with MVC controllers and minimal API endpoints, discovering rules once at startup and caching them so validation costs a dictionary lookup rather than a reflection pass per request.

## Categories

- [Localization](./localization) &mdash; where validation messages come from and how keys are named.
- [Fluent Validation](./fluent-validation) &mdash; declaring rules in code instead of attributes.
- [Custom Rules](./custom-rules) &mdash; writing a rule with its own dependencies.
- [Extensions](./extensions/add-asp-net-validation) &mdash; registration and pipeline setup.
- [Validation Rules](./validation-rules/presence) &mdash; the full rule catalogue, grouped by family.
- [Services](./services/validation-rule-describer) &mdash; reading the declared rules at runtime.
- [Types](./types/validation-error-result) &mdash; the result helper and comparison target.
- [Records](./records/validation-error-response) &mdash; the response shapes.

## The failure response

Every validation failure returns `422` with the same shape:

```json
{
    "code": 422,
    "error": "validation_error",
    "errorDescription": "The request did not pass validation",
    "errors": {
        "username": {
            "code": 1044866933,
            "error": "validation_regex",
            "errorDescription": "Must match the expected shape: lowercase letters and digits only"
        }
    }
}
```

The outer three fields are the standard error body shared with the other packages. `errors` is added by this package, keyed by the field name a client sees, with one entry per field carrying its first failure. A field renamed with `[JsonPropertyName]` is reported under that name, and a failure inside a nested object keeps its full path, such as `billingAddress.street` or `items[0].name`.

The per-field `code` is a stable number derived from the message key, so a client can branch on it without matching English text or parsing the key.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter();
builder.Services.AddAspNetValidation();
builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
app.MapControllers();
```

```csharp [SignupController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.RequestValidation;

[ApiController]
[Route("signup")]
public sealed class SignupController : ControllerBase
{
    [HttpPost]
    public IActionResult Post(SignupRequest request) => Ok();
}
```

```csharp [SignupRequest.cs]
using AlmightyShogun.AspNet.RequestValidation;

public sealed record SignupRequest
{
    [Required]
    [Email]
    public string Email { get; init; } = "";

    [Required]
    [Min(12)]
    public string Password { get; init; } = "";

    [RequiredWith(nameof(Password))]
    public string PasswordConfirmation { get; init; } = "";
}
```

:::
