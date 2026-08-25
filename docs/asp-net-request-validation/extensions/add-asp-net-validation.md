---
returns: The same `IServiceCollection` instance with validation services configured.
---

# AddAspNetValidation

Registers the rule cache, the request filters, the response factory, and the rule describer.

Pair it with [`UseAspNetValidation`](./use-asp-net-validation), which adds the middleware. Registration alone configures services but puts nothing in the request pipeline.

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

Controller integration is configured through `MvcOptions`, adding the body and request validation filters globally, and `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes` is turned on so a non-nullable property is not implicitly required by the framework. Requiredness stays something you declare with [`[Required]`](../validation-rules/presence#required) rather than something the type system decides.

`ApiBehaviorOptions.InvalidModelStateResponseFactory` is replaced so model-binding failures return the same shape as rule failures, and `RouteHandlerOptions.ThrowOnBadRequest` is enabled so a minimal API body that cannot be parsed reaches the middleware instead of returning a bare `400`.

The validation exception handler is separate from the mapped exception handler in `AlmightyShogun.AspNet.Core`. Both can be registered at once: ASP.NET tries handlers in order and each returns `false` for exceptions it does not recognize, so they compose without interfering.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddAspNetValidation();
```
