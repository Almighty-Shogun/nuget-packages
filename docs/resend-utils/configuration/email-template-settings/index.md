---
outline: deep

fields:
    - name: CopyrightTextTemplate
      description: Footer copyright text template. Supports `{app_name}` and `{app_url}` placeholders.
      type: string

    - name: FooterLinkText
      description: Text rendered for the footer application link. Supports `{app_name}` and `{app_url}` placeholders.
      type: string

    - name: IgnoreText
      description: Fallback text shown when the recipient can ignore an unexpected email.
      type: string
---

# EmailTemplateSettings

Represents the nested `Email:Template` configuration values used while rendering shared email footer text. These values are resolved by `BaseMailTemplate` when it renders both HTML and plain-text bodies.

Use this record through `EmailSettings.Template` when application code needs to inspect the configured template text. The renderer replaces `{app_name}` with `EmailSettings.BrandName` and `{app_url}` with `EmailSettings.AppUrl`.

## Usage

The JSON shape is documented on the [configuration page](/resend-utils/configuration). The example below shows how to read the already-bound template settings from options.

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.Resend.Utils;

public sealed class EmailFooterPreview(IOptions<EmailSettings> options)
{
    private readonly EmailTemplateSettings _template = options.Value.Template;

    public string IgnoreText => _template.IgnoreText;
}
```

<FrontmatterDocs/>
