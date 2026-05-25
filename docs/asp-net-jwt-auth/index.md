# ASP.NET JWT Auth

Adds the authentication pieces commonly needed by ASP.NET Core APIs in this package family. It wires JWT bearer authentication, binds the `Auth` configuration section, validates token issuer and signing key, checks token audiences against configured request hosts, and registers permission-based authorization support.

Use this package when an API should accept JWT access tokens, store refresh tokens in a consistent cookie, and protect controllers or actions with permission attributes. The package is intentionally focused: token issuing remains application code, while validation, cookie names, cookie helpers, host-to-app resolution, and permission policy registration are handled here.

## Categories

- [Attributes](./attributes/auth-permission-attribute) &mdash; endpoint metadata for permission-based authorization.
- [Classes](./classes/app-host-resolver/) &mdash; concrete package services, with guidance on the DI interfaces consumers should request.
- [Configuration](./configuration/auth-settings) &mdash; public configuration shapes bound from application configuration.
- [Constants](./constants/cookie-names) &mdash; shared names used by the cookie helpers.
- [Extensions](./extensions/add-api-auth) &mdash; startup, request, response, and claims-principal extension methods.

## Quick Example

```csharp
using AlmightyShogun.AspNet.JwtAuth;

builder.Services.AddApiAuth(builder.Configuration);
```
