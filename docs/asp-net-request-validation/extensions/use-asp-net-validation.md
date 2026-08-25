---
returns: The same builder instance, with validation applied.
---

# UseAspNetValidation

Adds validation to the request pipeline. Three overloads cover the three places it can be applied.

## Usage

::: code-group

```csharp [IApplicationBuilder.cs]
using AlmightyShogun.AspNet.RequestValidation;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
```

```csharp [RouteHandlerBuilder.cs]
using AlmightyShogun.AspNet.RequestValidation;

app.MapPost("/signup", (SignupRequest request) => Results.Ok())
    .UseAspNetValidation();
```

```csharp [RouteGroupBuilder.cs]
using AlmightyShogun.AspNet.RequestValidation;

RouteGroupBuilder accounts = app.MapGroup("/accounts")
    .UseAspNetValidation();

accounts.MapPost("/signup", (SignupRequest request) => Results.Ok());
accounts.MapPost("/reset", (ResetRequest request) => Results.Ok());
```

:::

## Where each overload applies

The `IApplicationBuilder` overload adds the middleware that turns an unreadable request body into the standard error shape, whether it surfaced as a malformed body or as an unsupported content type. Rule failures never reach it, because the filters answer those before an action runs. Call it once, early, before routing.

The `RouteHandlerBuilder` overload adds an endpoint filter that validates the bound request model of a single minimal API endpoint. The `RouteGroupBuilder` overload does the same for every endpoint in a group, which is usually what you want rather than repeating the call per endpoint.

MVC controllers need neither endpoint overload, because [`AddAspNetValidation`](./add-asp-net-validation) registers the controller filters globally.

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseAspNetValidation();

public RouteHandlerBuilder UseAspNetValidation();

public RouteGroupBuilder UseAspNetValidation();
```
