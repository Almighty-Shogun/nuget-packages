# Configuration

Resend Utils reads the `Email` section from `appsettings.json` when [`AddResendEmail`](./extensions/add-resend-email) receives an `IConfiguration` instance. The section is bound to [`EmailSettings`](./configuration/email-settings) and is used to configure the Resend API token, sender identity, brand values, footer text, and reusable template placeholders.

The `Email` section is required during startup registration. If the section is missing, [`AddResendEmail`](./extensions/add-resend-email) throws an `InvalidOperationException` before any email services are registered, so configuration mistakes fail early instead of surfacing later when the application tries to send an email.

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

`CopyrightTextTemplate` and `FooterLinkText` support `{app_name}` and `{app_url}` placeholders in HTML and plain-text output. `IgnoreText` is resolved in HTML output and rendered as configured in plain-text output.
