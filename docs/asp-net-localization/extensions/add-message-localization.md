---
params:
    - name: configuration
      description: Application configuration, read for the optional `Localization` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with message localization registered.
---

# AddMessageLocalization

Registers message resolution: the language provider that negotiates a language from the request, the store that reads the `messages/` directory, and the resolver that turns a message key into localized text.

Anything that produces localized text depends on it, including the error responses, validation messages, and maintenance notices of the other ASP.NET packages. None of them register it, so an application that uses any of those calls this once itself.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Localization;

builder.Services.AddMessageLocalization(builder.Configuration);

WebApplication app = builder.Build();

app.UseMessageLocalization();
```

```csharp [WelcomeService.cs]
using AlmightyShogun.AspNet.Localization;

public sealed class WelcomeService(IMessageResolver messageResolver)
{
    public string Greeting() => messageResolver.Resolve("welcome.greeting");
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddMessageLocalization(
    IConfiguration configuration
);
```
