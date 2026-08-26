---
fields:
    - name: Id
      description: The surrogate key. Never leaves the server; the refresh token is the only handle a client has on a session.
      type: int

    - name: UserId
      description: The owning user. Sessions cascade with the user, so deleting an account takes its sessions with it.
      type: int

    - name: RefreshTokenHash
      description: Hash of the current refresh token, uniquely indexed. The token itself is never stored, so a database copy cannot be used to refresh.
      type: string

    - name: PreviousRefreshTokenHash
      description: Hash of the token this one replaced. Presenting it again after the grace period is what identifies a stolen token and revokes every session for the user.
      type: string?
      default: 'null'

    - name: App
      description: The application audience this session belongs to. Refresh only matches sessions for the currently resolved application, so a token from one app cannot refresh another.
      type: string?
      default: 'null'

    - name: ExpiresAt
      description: When the session stops refreshing. Extended on each refresh, but never past the ceiling set by `AbsoluteSessionLifetimeDays`.
      type: DateTimeOffset

    - name: CreatedAt
      description: When the session began. The absolute lifetime cap is measured from here, not from the last refresh.
      type: DateTimeOffset

    - name: LastActiveAt
      description: When the session last refreshed. Also what the reuse grace period is measured against.
      type: DateTimeOffset

    - name: IsRevoked
      description: Whether the session was ended deliberately, by logout, a password change, or reuse detection. Revoked rows are kept rather than deleted.
      type: bool
      default: 'false'

    - name: IpAddress
      description: The address of the most recent request on this session, truncated to 45 characters so an IPv6 address still fits.
      type: string?
      default: 'null'

    - name: UserAgent
      description: The raw User-Agent header, kept alongside the parsed values because parsing loses detail that matters when auditing.
      type: string?
      default: 'null'

    - name: Device
      description: The device parsed from the User-Agent, for showing a user their own session list.
      type: string?
      default: 'null'

    - name: Browser
      description: The browser parsed from the User-Agent, for showing a user their own session list.
      type: string?
      default: 'null'

    - name: Os
      description: The operating system parsed from the User-Agent, for showing a user their own session list.
      type: string?
      default: 'null'

    - name: IsExpired
      description: Whether the expiry has passed. Computed, not mapped, so it cannot be used in a query.
      type: bool

    - name: IsActive
      description: Whether the session would still refresh, meaning neither revoked nor expired. Computed, not mapped.
      type: bool
---

# UserSession

One refresh-token session. Created at login and registration, rotated on refresh, and revoked by logout, a password change, or reuse detection.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class SessionAuditService(AppDbContext database)
{
    public Task<List<UserSession>> GetActiveSessionsAsync(int userId)
        => database.UserSessions
            .Where(session => session.UserId == userId && !session.IsRevoked)
            .Where(session => session.ExpiresAt > DateTimeOffset.UtcNow)
            .ToListAsync();
}
```

<FrontmatterDocs/>
