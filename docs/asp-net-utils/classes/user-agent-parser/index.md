# UserAgentParser

Parses raw `User-Agent` header values into the package's simpler `UserAgent` record. The class wraps `UAParser` so application code can work with browser and device strings instead of the full parser result model.

Use `UserAgentParser` when storing request metadata, writing audit logs, or displaying basic client information. Empty or missing header values are treated as unknown values rather than causing parsing failures.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgentParser.Parse(
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0 Safari/537.36"
);
```

## Methods

- [Parse](./parse) &mdash; parses a raw User-Agent header into browser and device values.
