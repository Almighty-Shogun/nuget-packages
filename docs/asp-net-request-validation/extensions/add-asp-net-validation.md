---
params:
    - name: assemblies
      description: Assemblies to scan for [`Validator<TRequest>`](../fluent-validation) subclasses, in search order. An empty array finds no validators; the overload taking no assembly falls back to the calling assembly.
      type: 'Assembly[]'

returns: The same `IServiceCollection` instance with validation services configured.
---

# AddAspNetValidation

Registers the validation services, then scans the assemblies for validators and builds every request type's rules. Anything that cannot be built is refused here rather than on the first request that reaches it: two validators for one request type, a validator without a public parameterless constructor, a rule given no values to compare against, a blank date format, a field name matching no property, or a custom rule naming a type that does not implement the rule interface. Pair it with [`UseAspNetValidation`](./use-asp-net-validation), which adds the middleware but is otherwise separate.

## Usage

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter()
    .AddAspNetValidation();
```

::: warning
[`AddMessageLocalization`](/asp-net-localization/extensions/add-message-localization) and [`AddHttpErrorResponseWriter`](/asp-net-core/extensions/add-http-error-response-writer) must also be called. Without them, messages resolve to their raw keys such as `validation.required`.
:::

## MVC and minimal API integration

Controller integration is configured through `MvcOptions` rather than added, so the body and request validation filters take effect whenever the application registers controllers, whichever order the two calls are made in, and a minimal API application does not acquire the controller stack by asking for validation. `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes` is turned on so a non-nullable property is not implicitly required by the framework. Requiredness stays something you declare with [`[Required]`](../validation-rules/presence#required) rather than something the type system decides.

`ApiBehaviorOptions.InvalidModelStateResponseFactory` is replaced so model-binding failures return the same shape as rule failures, and `RouteHandlerOptions.ThrowOnBadRequest` is enabled so a minimal API body that cannot be parsed reaches the middleware instead of returning a bare `400`.

The validation exception handler is separate from the mapped exception handler in `AlmightyShogun.AspNet.Core`. Both can be registered at once: ASP.NET tries handlers in order and each returns `false` for exceptions it does not recognize, so they compose without interfering.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddAspNetValidation();

public IServiceCollection AddAspNetValidation(Assembly[] assemblies);
```
