# ASP.NET JWT Auth

Adds the authentication pieces commonly needed by ASP.NET Core APIs in this package family. It wires JWT bearer authentication, binds the `Auth` configuration section, validates token issuer, signing key, lifetime, and configured audiences, and adds authorization rules that match protected requests to their configured host app.

Use this package when an API should accept JWT access tokens, store refresh tokens in a consistent cookie, and protect controllers or actions with permission attributes. The package is intentionally focused: token issuing remains application code, while validation, cookie names, cookie helpers, host-to-app resolution, refresh-token guards, and permission policy registration are handled here.

## Categories

- [Configuration](./configuration/auth-settings) &mdash; public configuration shapes bound from application configuration.
- [Extensions](./extensions/add-jwt-auth) &mdash; startup, request, response, and claims-principal extension methods.
- [Attributes](./attributes/auth-permission-attribute) &mdash; endpoint metadata for permission and refresh-token authorization.
- [Services](./services/app-host-resolver) &mdash; dependency-injection services and contracts used by authentication.
- [Constants](./constants/cookie-names) &mdash; shared names used by the cookie helpers.

## Quick Example

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.JwtAuth;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddJwtAuth(builder.Configuration);

app.UseHttpErrorResponses();
```
