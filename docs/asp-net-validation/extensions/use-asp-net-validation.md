# UseAspNetValidation

Adds ASP.NET Validation to an application pipeline, minimal API endpoint, or minimal API route group. The application-builder overload adds middleware that converts [`ValidationException`](../types/validation-exception), invalid request bodies, and unsupported body content into standardized JSON responses. The endpoint and route-group overloads add the endpoint filter that validates request objects before the handler runs.

Use the application-builder overload when controller actions or code deeper in the pipeline can throw [`ValidationException`](../types/validation-exception). Use the endpoint or route-group overload when minimal API handlers should validate their bound request objects.

Minimal API applications that want standardized invalid-body responses should use the application-builder overload once in the pipeline and then add the endpoint or route-group overload to the routes that should validate request objects.

## ApplicationBuilder

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

app.MapControllers();
```

### Type signature

```csharp
public IApplicationBuilder UseAspNetValidation();
```

## RouteHandlerBuilder

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

app.MapPost("/projects", (CreateProjectRequest request) => Results.Ok(request))
    .UseAspNetValidation();
```

### Type signature

```csharp
public RouteHandlerBuilder UseAspNetValidation();
```

## RouteGroupBuilder

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

RouteGroupBuilder projects = app.MapGroup("/projects")
    .UseAspNetValidation();

projects.MapPost("/", (CreateProjectRequest request) => Results.Ok(request));
```

### Type signature

```csharp
public RouteGroupBuilder UseAspNetValidation();
```
