---
outline: deep

params:
    - name: userAgent
      description: Raw User-Agent header value to parse. `null` or empty values return `Unknown` browser and device values.
      type: string?

returns: A `UserAgent` record containing the parsed browser and device strings.
---

# Parse

Parses a raw User-Agent header value into a compact `UserAgent` record. The method extracts the browser family and major version into `Browser`, and the operating-system family and major version into `Device`.

Use this method when application code only needs basic client information and does not need the full `UAParser` object model. When the input is `null` or empty, the method returns `Unknown` for both values so callers can safely store or display the result without special error handling.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgentParser.Parse(
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0 Safari/537.36"
);

Console.WriteLine(userAgent.Browser);
Console.WriteLine(userAgent.Device);
```

<FrontmatterDocs/>

## Type signature

```csharp
public static UserAgent Parse(string? userAgent);
```

## Uses

- [UserAgent](../../records/user-agent)
