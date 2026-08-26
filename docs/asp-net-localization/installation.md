# Installation

Install `AlmightyShogun.AspNet.Localization` in the ASP.NET Core application that serves localized text. The package targets `net10.0` and references the ASP.NET Core shared framework. `AlmightyShogun.AspNet.Core` depends on it, so an application already using standardized error responses has it transitively.

```sh
dotnet add package AlmightyShogun.AspNet.Localization
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; the ASP.NET Core shared framework.

### Project references

- `AlmightyShogun.Core` &mdash; supplies the configuration binding helper the `Localization` section is bound through, so a malformed value fails at startup.

## Startup Registration

[`AddMessageLocalization`](./extensions/add-message-localization) registers the language provider, the message store, and the resolver together, along with the HTTP context accessor the provider reads the request through. [`UseMessageLocalization`](./extensions/use-message-localization) is optional and only adds the middleware that reports the negotiated language on the response.

::: warning
Message files are read from disk at runtime and the package ships none. Lay them out as `messages/{language}/{group}.json`, where the file name becomes the first segment of every key it defines. The content root, the output folder, and the working directory are all searched in that order, and the first to define a key keeps it, so the application's own files are never displaced by whatever directory the process was started from.
:::

```csharp
using AlmightyShogun.AspNet.Localization;

builder.Services.AddMessageLocalization(builder.Configuration);

WebApplication app = builder.Build();

app.UseMessageLocalization();
```
