---
outline: deep

returns: The default cookie name used for refresh tokens.
---

# CookieNames

Contains the public cookie names used by ASP.NET JWT Auth. Use these constants when application code needs to interact with the same cookies as the package helpers instead of duplicating string literals.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;

string refreshTokenCookie = CookieNames.RefreshToken;
```

<FrontmatterDocs/>
