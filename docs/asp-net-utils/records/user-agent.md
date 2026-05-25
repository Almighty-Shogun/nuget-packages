---
outline: deep

returns: A parsed User-Agent record containing browser and device strings.
---

# UserAgent

Represents the simplified result returned by `UserAgentParser.Parse`. The record keeps only the browser and device strings that are usually useful for request logging, audit trails, and simple client displays.

Use this record when application code does not need the full `UAParser` result. If parsing starts with a missing or empty header, both values are set to `Unknown`.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgentParser.Parse(Request.Headers.UserAgent);

string browser = userAgent.Browser;
string device = userAgent.Device;
```

<FrontmatterDocs/>
