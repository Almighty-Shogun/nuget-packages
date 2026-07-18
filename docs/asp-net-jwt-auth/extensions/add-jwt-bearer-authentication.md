---
params:
    - name: configuration
      description: Application configuration containing the `Auth` section.
      type: IConfiguration

returns: The `AuthenticationBuilder` instance with JWT bearer authentication configured.
---

# AddJwtBearerAuthentication

Adds JWT bearer authentication to an existing `AuthenticationBuilder` by reading the package's `Auth` configuration section. The method configures issuer validation, token lifetime validation, signing-key validation, and the symmetric signing key from `AuthSettings.Secret`.

After the JWT has passed the normal bearer-token validation, the package resolves the current request host to an application name and verifies that the token audience contains that application. This is useful when the same authentication setup is shared by multiple apps or hosts, but tokens must still be scoped to the app they are used against.

## Usage

::: warning
Requires an `Auth` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearerAuthentication(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public AuthenticationBuilder AddJwtBearerAuthentication(
    IConfiguration configuration
);
```
