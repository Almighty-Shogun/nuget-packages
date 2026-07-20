# ASP.NET Validation

Adds request validation to ASP.NET Core APIs with attribute-based rules, fluent request rules, localized validation messages, and standardized JSON validation error responses.

Use this package when request DTOs should declare their own validation rules without mixing validation logic into controllers or endpoint handlers. Controller actions, model-state errors, minimal API endpoint filters, request-body parsing failures, and manually thrown validation exceptions are converted into the same response shape when the package is registered with ASP.NET Utils error responses.

The package targets `net10.0`, uses the ASP.NET Core shared framework, and depends on [ASP.NET Utils](/asp-net-utils/) for localized message resolution and standardized HTTP error response records.

## Categories

- [Localization](./localization) &mdash; message files required for validation errors and invalid request bodies.
- [Fluent Validation](./fluent-validation) &mdash; request-owned rule builders for validation that needs expressions or fluent chaining.
- [Custom Rules](./custom-rules) &mdash; dependency-injected validation rules for checks that need application services.
- [Extensions](./extensions/add-asp-net-validation) &mdash; service, middleware, controller, and minimal API registration methods.
- [Validation Rules](./validation-rules/presence) &mdash; grouped validation rules with side-by-side attribute and fluent examples.
- [Types](./types/validation-exception) &mdash; public helper types for manual validation failures and object results.
- [Records](./records/validation-error-response) &mdash; standardized validation response records returned to API clients.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddAspNetValidation();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

app.MapControllers();
```

```csharp [CreateProjectRequest.cs]
using AlmightyShogun.AspNet.Validation;

public sealed class CreateProjectRequest
{
    [Required]
    [String]
    [Max(80)]
    public string Name { get; init; } = string.Empty;

    [Array]
    [Distinct]
    public string[] Tags { get; init; } = [];
}
```

:::
