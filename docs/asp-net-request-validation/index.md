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
- [Utilities](./utilities/validation-error-result) &mdash; returning a validation failure from a controller action.
- [Types](./types/comparison-target) &mdash; how a comparison rule reads its target.
- [Records](./records/validation-error-response) &mdash; the response shapes.

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
    .AddAspNetValidation();

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
