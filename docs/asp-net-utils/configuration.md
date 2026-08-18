---
fields:
    - name: HttpErrors
      description: Bound by `AddHttpErrorResponseWriter` into `HttpErrorSettings`. Decides the shape of every error body the application returns and how much of it reaches the log.
      fields:
          - name: UseProblemDetails
            description: Writes error bodies as `application/problem+json` following RFC 9457 instead of the package shape.
            type: bool
            default: 'false'

          - name: LogErrors
            description: Logs every handled error. A `5xx` is logged with the exception, a `4xx` without it.
            type: bool
            default: 'true'

          - name: MinimumLogStatusCode
            description: Lowest status code that is logged. Raise it to `500` to keep client faults out of the log.
            type: int
            default: '400'

    - name: Localization
      description: Bound by `AddMessageLocalization` into `LocalizationSettings`. Decides which language a message resolves in and whether message files are watched for changes.
      fields:
          - name: DefaultLanguage
            description: Language used when the request does not ask for one, and as the final fallback when a key is missing from the requested language.
            type: string
            default: en

          - name: AutomaticReload
            description: Watches the message directories and clears the cache when a file changes, so edits apply without a restart.
            type: bool
            default: 'false'

    - name: AllowedOrigins
      description: Read directly by `AddCorsPolicy` rather than bound to a record.
      fields:
          - name: AllowedOrigins
            description: Origins permitted by the CORS policy. The `*` wildcard is rejected, because browsers refuse it when credentials are allowed.
            type: 'string[]'
            default: '[]'
---

# Configuration

Three independent sections, each read by a different registration method. All three are optional and every value has a default, so an application can register the services without adding any of them. `AllowedOrigins` is the exception in shape: it is a bare array read directly rather than bound to a settings record.

```json
{
    "HttpErrors": {
        "UseProblemDetails": false,
        "LogErrors": true,
        "MinimumLogStatusCode": 400
    },
    "Localization": {
        "DefaultLanguage": "en",
        "AutomaticReload": false
    },
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ]
}
```

::: tip
Add a section only to change a default: to emit problem details, to keep client faults out of the log, to change the fallback language, or to allow cross-origin callers.
:::

<FrontmatterDocs/>
