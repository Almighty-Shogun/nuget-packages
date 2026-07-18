---
params:
    - name: Browser
      description: Browser family and major version returned by the parser.
      type: string

    - name: Device
      description: Operating-system family and major version returned by the parser.
      type: string

returns: A parsed User-Agent record containing browser and device strings.
---

# UserAgent

Represents simplified User-Agent information returned by `UserAgent.Parse` and `GetUserAgent`. The record keeps only the browser and device strings that are usually useful for request logging, audit trails, and simple client displays.

Use this record when application code does not need the full `UAParser` result. If parsing starts with a missing or empty header, both values are set to `Unknown`.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgent.Parse(
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/124.0 Safari/537.36"
);

string browser = userAgent.Browser;
string device = userAgent.Device;
```

<FrontmatterDocs/>

## Type signature

```csharp
public record UserAgent(
    string Browser,
    string Device
);
```

## Parse

Parses a raw User-Agent header value into a compact `UserAgent` record. The method extracts the browser family and major version into `Browser`, and the operating-system family and major version into `Device`.

When the input is empty, the method returns `Unknown` for both values so callers can safely store or display the result without special error handling. `GetUserAgent` calls this method with the current request's `User-Agent` header.

```csharp
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgent.Parse(
    "Mozilla/5.0 (Macintosh; Intel Mac OS X 14_4) AppleWebKit/605.1.15 Version/17.4 Safari/605.1.15"
);
```

### Type signature

```csharp
public static UserAgent Parse(string userAgent);
```
