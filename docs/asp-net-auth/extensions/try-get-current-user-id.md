---
returns: The caller's public identifier, or `null` when the principal carries no well-formed one.
---

# TryGetCurrentUserId

Reads the caller's public identifier from the [`userId`](../constants/auth-claim-types) claim, falling back to `ClaimTypes.NameIdentifier`, and returns `null` when neither yields a usable value.

Use it where an anonymous caller is ordinary rather than exceptional. Where an identifier is required, [`GetCurrentUserId`](./get-current-user-id) throws instead.

## Usage

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

Guid? identifier = principal.TryGetCurrentUserId();
```

<FrontmatterDocs/>

## Type signature

```csharp
public Guid? TryGetCurrentUserId();
```
