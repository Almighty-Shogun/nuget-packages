---
fields:
    - name: ApiToken
      description: Resend API token used to authenticate email send requests.
      type: string

    - name: BrandName
      description: Product or application name used in shared email template placeholders.
      type: string

    - name: FromEmail
      description: Sender email address used for outgoing messages.
      type: string

    - name: FromName
      description: Display name used with `FromEmail` in outgoing messages.
      type: string

    - name: LogoUrl
      description: Public logo URL rendered by the base HTML template.
      type: string

    - name: AppUrl
      description: Public application URL used by footer links and placeholders.
      type: string

    - name: Links
      description: Named shared links that application code can use when building templates.
      type: 'Dictionary<string, string>'

    - name: Template
      description: Shared footer and fallback text settings used while rendering templates.
      type: EmailTemplateSettings
---

# EmailSettings

Represents the `Email` configuration section used by Resend Utils. The package binds this record during [`AddResendEmail`](../extensions/add-resend-email) and consumes it while configuring the Resend client and rendering [`BaseMailTemplate`](../types/base-mail-template) instances.

Application code normally does not create this record manually. Inject `IOptions<EmailSettings>` only when a service needs to inspect the configured sender, brand, or shared links.

## Usage

::: tip
The JSON shape is documented on the [configuration page](/resend-utils/configuration). The example below shows how application services can consume the already-bound options.
:::

```csharp
using AlmightyShogun.Resend.Utils;
using Microsoft.Extensions.Options;

public sealed class EmailLinkResolver(IOptions<EmailSettings> options)
{
    private readonly EmailSettings _settings = options.Value;

    public string DashboardUrl => _settings.Links["Dashboard"];
}
```

<FrontmatterDocs/>
