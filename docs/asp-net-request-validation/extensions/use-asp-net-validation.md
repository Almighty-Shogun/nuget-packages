---
returns: The same builder instance, with validation applied.
---

# UseAspNetValidation

Adds validation to the request pipeline, with the receiver deciding what is added: the application builder gets the middleware for a body the framework could not read, and a route handler or route group gets the endpoint filter that validates a minimal API endpoint's bound arguments. MVC controllers need neither endpoint overload, because [`AddAspNetValidation`](./add-asp-net-validation) registers the controller filters globally.

## IApplicationBuilder

Adds the middleware that turns an unreadable request body into the standard error shape, whether it surfaced as a malformed body or as an unsupported content type. Rule failures never reach it, because the filters answer those before an action runs. Call it once, early, before routing.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
```

### Type signature

```csharp
public IApplicationBuilder UseAspNetValidation();
```

## RouteHandlerBuilder

Adds an endpoint filter that validates the bound arguments of a single minimal API endpoint before its handler runs.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

app.MapPost("/signup", (SignupRequest request) => Results.Ok())
    .UseAspNetValidation();
```

### Type signature

```csharp
public RouteHandlerBuilder UseAspNetValidation();
```

## RouteGroupBuilder

Adds the same endpoint filter to every endpoint in a group, which is usually what you want rather than repeating the call per endpoint.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

RouteGroupBuilder accounts = app.MapGroup("/accounts")
    .UseAspNetValidation();

accounts.MapPost("/signup", (SignupRequest request) => Results.Ok());
accounts.MapPost("/reset", (ResetRequest request) => Results.Ok());
```

### Type signature

```csharp
public RouteGroupBuilder UseAspNetValidation();
```

<FrontmatterDocs/>
