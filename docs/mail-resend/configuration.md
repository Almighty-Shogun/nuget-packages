---
fields:
    - name: EmailSettings
      description: The `Email` section, bound by [`AddResendEmail`](./extensions/add-resend-email) and validated while the host starts, so a missing token or sender address fails startup instead of the first send. Inject `IOptions<EmailSettings>` to read it. Its computed `From` property pairs `FromName` with `FromEmail` as Resend expects, falling back to the bare address when no display name is configured.
      fields:
          - name: ApiToken
            description: Token every send is authenticated with. Required, and checked at startup.
            type: string

          - name: FromEmail
            description: Address messages are sent from. It must sit on a domain verified with Resend, which startup validation cannot check, so an unverified domain surfaces as a failed send.
            type: string

          - name: FromName
            description: Display name paired with `FromEmail`. Left unset, messages send from the bare address.
            type: string?
            default: 'null'

          - name: BrandName
            description: Product name substituted for the `{app_name}` placeholder and rendered in the brand slot of the base template. Left empty it renders as nothing rather than falling back to `FromName`.
            type: string
            default: "''"

          - name: LogoUrl
            description: Logo shown in the base template header. Dropped from the rendered HTML unless it is an absolute http, https, or mailto URL.
            type: string?
            default: 'null'

          - name: AppUrl
            description: URL behind the footer link and the `{app_url}` placeholder. Both are dropped under the same scheme rule as `LogoUrl`, leaving the footer link inert and the placeholder empty in the HTML and plain-text bodies alike.
            type: string?
            default: 'null'

          - name: Links
            description: Named shared links as label-to-URL pairs. The package never reads them; they exist so an application can keep its link set alongside the rest of its mail configuration.
            type: 'IReadOnlyDictionary<string, string>'
            default: '{}'

          - name: Template
            description: The nested `Template` section. When it is absent every value below keeps its own default rather than binding to an empty string.
            type: EmailTemplateSettings
            default: '{}'

    - name: EmailTemplateSettings
      description: The nested `Email:Template` section, holding the footer and fallback wording every [`BaseMailTemplate`](./types/base-mail-template) shares, so copy appearing in each message is configured once instead of restated by every template class. `{app_name}` and `{app_url}` are substituted before the value is HTML encoded, so a brand name containing markup cannot escape into the document.
      fields:
          - name: CopyrightTextTemplate
            description: Copyright line closing the footer, resolved in both the HTML and the plain-text body.
            type: string
            default: © {app_name}

          - name: FooterLinkText
            description: Label of the footer link pointing at `AppUrl`. It still renders when that URL is unset or was rejected.
            type: string
            default: '{app_name}'

          - name: IgnoreText
            description: Line telling a recipient to disregard a message they did not expect. Empty by default, which drops the line from the plain-text rendering and substitutes nothing for its HTML placeholder; whether that leaves a blank paragraph depends on the application's own base template.
            type: string
            default: "''"
---

# Configuration

Mail Resend reads the `Email` section when [`AddResendEmail`](./extensions/add-resend-email) receives an `IConfiguration` instance. An absent section binds successfully and fails validation instead, which is what reports the missing token rather than a binding error.

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
            "IgnoreText": "Ignore this mail when you did not request it."
        }
    }
}
```

::: tip
`CopyrightTextTemplate`, `FooterLinkText` and `IgnoreText` each resolve `{app_name}` from `BrandName` and `{app_url}` from `AppUrl`, in the HTML body and the plain-text body alike.
:::

<FrontmatterDocs/>
