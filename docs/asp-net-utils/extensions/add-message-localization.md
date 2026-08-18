---
params:
    - name: configuration
      description: Application configuration, read for the optional `Localization` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with message localization registered.
---

# AddMessageLocalization

Registers message resolution: the language provider that negotiates a language from the request, the store that reads the `messages/` directory, and the resolver that turns a message key into localized text.

Every other part of the package that produces text depends on it: the exception handlers, the MVC error filter, and the validation and maintenance packages. Register it once, before or after the others, since none of them register it for you.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services.AddMessageLocalization(builder.Configuration);

WebApplication app = builder.Build();

app.UseMessageLocalization();
```

```csharp [WelcomeService.cs]
using AlmightyShogun.AspNet.Utils;

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
