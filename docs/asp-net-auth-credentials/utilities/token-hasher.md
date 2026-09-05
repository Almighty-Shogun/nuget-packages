# TokenHasher

Hashes the tokens the package stores. Refresh tokens, password reset tokens, and recovery codes are all kept as a digest, so this is what turns a token an application holds into the value a row is found by. It is a plain SHA-256 digest rather than a password hash.

## Hash

Returns the digest of a token as uppercase hexadecimal, which is the form every hash column in the package holds. The same input always produces the same output, so a presented token is found by an indexed equality match rather than compared row by row.

Hash the value exactly as it was issued. Trimming, lower-casing, or URL-decoding it first produces a different digest and the row will not be found.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class PasswordResetTokenChecker(AppDbContext database)
{
    public Task<bool> IsUsableAsync(string token)
    {
        string hash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        return database.PasswordResetTokens
            .Where(stored.UsedAt == null && stored.ExpiresAt > now)
            .AnyAsync(stored => stored.TokenHash == hash);
    }
}
```

### Type signature

```csharp
public static string Hash(string token);
```
