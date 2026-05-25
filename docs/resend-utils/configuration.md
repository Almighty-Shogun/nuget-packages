---
outline: deep

fields:
    - name: ApiToken
      description: Resend API token used by the underlying `ResendClient`.
      type: string

    - name: BrandName
      description: Application or product name inserted into shared email template placeholders.
      type: string

    - name: FromEmail
      description: Sender email address used in the Resend message.
      type: string

    - name: FromName
      description: Display name combined with `FromEmail` for the message sender.
      type: string

    - name: LogoUrl
      description: Public URL for the logo rendered by the base HTML template.
      type: string

    - name: AppUrl
      description: Application URL used by footer template placeholders.
      type: string

    - name: Links
      description: Named links available to application code when it needs shared destinations.
      type: 'Dictionary<string, string>'

    - name: Template
      description: Shared footer and fallback text used while rendering HTML and plain-text emails.
      type: EmailTemplateSettings
---

# Configuration

Resend Utils reads the `Email` section from `appsettings.json` when `AddResendEmail` receives an `IConfiguration` instance. The section is bound to `EmailSettings` and is used to configure the Resend API token, sender identity, brand values, footer text, and reusable template placeholders.

The `Email` section is required during startup registration. If the section is missing, `AddResendEmail` throws an `InvalidOperationException` before any email services are registered, so configuration mistakes fail early instead of surfacing later when the application tries to send an email.

```json
{
    "Email": {
        "ApiToken": "re_123456789",
        "BrandName": "Shogun",
        "FromEmail": "noreply@example.com",
        "FromName": "Shogun",
        "LogoUrl": "https://example.com/logo.png",
        "AppUrl": "https://example.com",
        "Links": {
            "Support": "https://example.com/support",
            "Dashboard": "https://example.com/dashboard"
        },
        "Template": {
            "CopyrightTextTemplate": "Copyright {app_name}.",
            "FooterLinkText": "Open {app_name}",
            "IgnoreText": "If you did not request this email, you can ignore it."
        }
    }
}
```

<FrontmatterDocs/>

The template text supports `{app_name}` and `{app_url}` placeholders. These placeholders are replaced with `BrandName` and `AppUrl` when a `BaseMailTemplate` is rendered.
