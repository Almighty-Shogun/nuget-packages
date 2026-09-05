# Installation

Install `AlmightyShogun.AspNet.Auth` in the ASP.NET Core API that needs to validate JWT access tokens and register the package's permission and app-audience authorization services. The package targets `net10.0`, uses ASP.NET Core framework APIs, and expects an `Auth` configuration section to be available when authentication is registered.

```sh
dotnet add package AlmightyShogun.AspNet.Auth
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides the ASP.NET Core authentication, authorization, HTTP, controller, and middleware infrastructure used by the package.

### Package references

- `Microsoft.AspNetCore.Authentication.JwtBearer` `10.0.11` &mdash; provides JWT bearer authentication middleware and token validation options.
- `Microsoft.IdentityModel.Tokens` `8.22.0` &mdash; provides token validation parameters and symmetric signing key types used directly by JWT setup.
- `System.IdentityModel.Tokens.Jwt` `8.22.0` &mdash; provides JWT claim name constants used by app-audience authorization.

### Project references

- `AlmightyShogun.Utils` &mdash; provides the validated configuration binding helper the `Auth` section is bound with.
- `AlmightyShogun.AspNet.Core` &mdash; provides the [`IExceptionMapper`](/asp-net-core/exceptions) contract and the standardized HTTP error response pipeline this package's failures are answered through.
- `AlmightyShogun.AspNet.Localization` &mdash; resolves the message shown for each mapped failure, so a rejected request is explained in the caller's language.

## Startup Registration

Register the package once while configuring application services. [`AddAuth`](./extensions/add-auth) binds and validates the `Auth` section, turns on JWT bearer authentication, and registers the host resolver, the token generator, and the permission policy provider. The package's [exceptions](./exceptions) reach the client as JSON only once [`UseHttpErrorResponses`](/asp-net-core/extensions/use-http-error-responses) is in the pipeline.

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Localization;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddAuth(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```
