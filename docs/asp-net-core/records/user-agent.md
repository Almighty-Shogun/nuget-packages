---
fields:
    - name: Browser
      description: Browser family and major version, such as `Chrome 120`.
      type: string

    - name: Os
      description: Operating-system family and major version, such as `Windows 10`.
      type: string

    - name: Device
      description: Device family, such as `iPhone`, or `Other` for a desktop browser.
      type: string

    - name: IsBot
      description: Whether the User-Agent was recognized as a crawler or spider.
      type: bool
---

# UserAgent

Simplified client information parsed from a User-Agent header, read through [`GetUserAgent`](../extensions/get-user-agent).

An absent header yields `Unknown` for all three string values and `false` for `IsBot`, while an unrecognized one yields `Other` for whichever part failed to match. Every field is pattern matching on a header the client chooses, so none of it is trustworthy enough to make an authorization or billing decision on.

<FrontmatterDocs/>

## Parse

Parses a raw header value, for one held outside the current request such as a value read back from an audit record. The underlying parser is created once for the process, because building it compiles a large regular expression set.

```csharp
using AlmightyShogun.AspNet.Core;

ClientContext clientContext = httpContext.GetClientContext();

UserAgent userAgent = UserAgent
    .Parse(clientContext.UserAgent ?? string.Empty);
```

### Type signature

```csharp
public static UserAgent Parse(string userAgent);
```

## Type signature

```csharp
public sealed record UserAgent
{
    public required string Browser { get; init; }
    public required string Os { get; init; }
    public required string Device { get; init; }
    public required bool IsBot { get; init; }
}
```
