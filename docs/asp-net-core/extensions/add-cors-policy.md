---
params:
    - name: name
      description: Name of the CORS policy to register.
      type: string
    - name: configuration
      description: Application configuration that may contain the `AllowedOrigins`, `AllowedHeaders` and `AllowedMethods` string arrays.
      type: IConfiguration

returns: The same `IServiceCollection` instance with the CORS policy configured.
---

# AddCorsPolicy

Registers a named CORS policy from the `AllowedOrigins` configuration array, with headers and methods from the optional `AllowedHeaders` and `AllowedMethods` arrays. Leaving either out allows any header or any method, and the policy always allows credentials, which is what a browser needs to send cookies to an API on another origin.

An absent or empty `AllowedOrigins` produces a policy with no origins, which blocks every cross-origin request rather than allowing them.

::: warning
Because the policy allows credentials, the `*` wildcard cannot be used &mdash; browsers reject that combination. A `*` entry throws at startup with a message saying so, rather than producing a policy that fails only in the browser.
:::

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;

builder.Services.AddCorsPolicy("frontend", builder.Configuration);

WebApplication app = builder.Build();

app.UseCors("frontend");
```

```json [appsettings.json]
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ],
    "AllowedMethods": ["GET", "POST"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCorsPolicy(
    string name,
    IConfiguration configuration
);
```
