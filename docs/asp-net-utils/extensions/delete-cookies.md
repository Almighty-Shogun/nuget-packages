---
params:
    - name: cookieNames
      description: One or more cookie names to delete. Empty or whitespace names are ignored.
      type: string[]
      default: '[]'
---

# DeleteCookies

Deletes one or more cookies from an `HttpResponse`. The method skips empty and whitespace-only names, so callers can pass filtered or optional cookie-name arrays without manually checking every value first.

Use this helper in logout endpoints, cleanup flows, or anywhere an API needs to remove several application cookies in a consistent way.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var httpContext = new DefaultHttpContext();

httpContext.Response.DeleteCookies("session", "preferences");
```

<FrontmatterDocs/>

## Type signature

```csharp
public void DeleteCookies(params string[] cookieNames);
```
