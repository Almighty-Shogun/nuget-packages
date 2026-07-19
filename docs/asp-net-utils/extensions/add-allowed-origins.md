---
params:
    - name: name
      description: Name of the CORS policy to register.
      type: string

    - name: configuration
      description: Application configuration containing the `AllowedOrigins` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with the named CORS policy configured.
---

# AddAllowedOrigins

Registers a named CORS policy from the `AllowedOrigins` configuration section. The policy allows the configured origins, any request header, any HTTP method, and credentials.

Use this method when allowed browser origins should be controlled from configuration instead of hard-coded in startup code. Because the policy allows credentials, configure explicit origins and avoid wildcard-style values.

## Usage

::: warning
When `AllowedOrigins` is missing or empty, the policy is registered with no configured origins. Configure explicit origins before using the policy in production.
:::

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services.AddAllowedOrigins("DefaultCors", builder.Configuration);
```

```json [appsettings.json]
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ]
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddAllowedOrigins(
    string name,
    IConfiguration configuration
);
```
