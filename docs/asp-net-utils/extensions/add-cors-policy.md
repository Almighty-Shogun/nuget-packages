---
params:
    - name: name
      description: Name of the CORS policy to register.
      type: string
    - name: configuration
      description: Application configuration that may contain the `AllowedOrigins` string array.
      type: IConfiguration

returns: The same `IServiceCollection` instance with the CORS policy configured.
---

# AddCorsPolicy

Registers a named CORS policy from the `AllowedOrigins` configuration array. The policy allows any header and any method, and allows credentials, which is what a browser needs to send cookies to an API on another origin.

An absent or empty `AllowedOrigins` produces a policy with no origins, which blocks every cross-origin request rather than allowing them.

::: warning
Because the policy allows credentials, the `*` wildcard cannot be used &mdash; browsers reject that combination. A `*` entry throws at startup with a message saying so, rather than producing a policy that fails only in the browser.
:::

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services.AddCorsPolicy("frontend", builder.Configuration);

WebApplication app = builder.Build();

app.UseCors("frontend");
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
public IServiceCollection AddCorsPolicy(
    string name,
    IConfiguration configuration
);
```
