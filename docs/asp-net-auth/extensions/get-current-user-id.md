---
returns: The caller's public identifier.
---

# GetCurrentUserId

Reads the caller's public identifier from a `ClaimsPrincipal`, checking the [`userId`](../constants/auth-claim-types) claim first and falling back to `ClaimTypes.NameIdentifier`.

If neither claim is present, or the value is not a well-formed identifier, it throws [`MissingUserIdClaimException`](../exceptions) rather than returning a default, which reaches the client as `401`. Use [`TryGetCurrentUserId`](./try-get-current-user-id) where an anonymous caller is expected.

## Usage

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

Guid identifier = principal.GetCurrentUserId();
```

<FrontmatterDocs/>

## Type signature

```csharp
public Guid GetCurrentUserId();
```
