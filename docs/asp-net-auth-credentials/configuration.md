---
fields:
    - name: AuthCredentialsSettings
      description: The `AuthCredentials` section itself. Every value has a default, so the section may be absent.
      fields:
          - name: Lockout
            description: The nested `Lockout` object, bound to `LockoutPolicy`.
            type: LockoutPolicy

          - name: TwoFactor
            description: The nested `TwoFactor` object, bound to `TwoFactorPolicy`.
            type: TwoFactorPolicy

          - name: AbsoluteSessionLifetimeDays
            description: Ceiling on a session's total life, measured from creation, so refreshing cannot extend it forever. An explicit `null` removes the cap; an absent key keeps 30 days.
            type: int?
            default: '30'

          - name: PasswordResetMinutes
            description: How long a password reset token stays usable after it is issued. Shorter is safer, because the token arrives by email and email is not a secure channel.
            type: int
            default: '60'

          - name: ForgotPasswordMinimumMilliseconds
            description: The floor a forgot-password request is held to, so issuing a token and finding no account take the same time. Raise it above the slower of the two paths on your own hardware, or the difference stays measurable.
            type: int
            default: '200'

    - name: LockoutPolicy
      description: "The nested `AuthCredentials:Lockout` section. Disabled by default, deliberately: locking on failure count alone lets anyone deny service to a known account by failing logins against it."
      fields:
          - name: Enabled
            description: Whether repeated login failures lock the account. Enable it when you have a way to distinguish an attacker from the account owner, such as rate limiting by IP in front of the application.
            type: bool
            default: 'false'

          - name: MaxFailedAttempts
            description: Consecutive failures before the account locks. Every attempt is counted before its password is checked, so this bounds concurrent guesses as well as sequential ones. The counter resets on any successful login, so it measures a run of failures rather than a lifetime total.
            type: int
            default: '5'

          - name: DurationMinutes
            description: How long the account stays locked. The expiry is carried on the lockout failure so a client can say when to try again.
            type: int
            default: '15'

    - name: TwoFactorPolicy
      description: The nested `AuthCredentials:TwoFactor` section, describing the codes this package generates and accepts.
      fields:
          - name: Issuer
            description: The name an authenticator app shows above the code. When left unset, the issuer passed to `BeginEnrolmentAsync` is used, which lets one deployment label each app it hosts differently.
            type: string?
            default: 'null'

          - name: RecoveryCodeCount
            description: How many single-use recovery codes are issued when enrolment completes. They are shown once and stored only as hashes.
            type: int
            default: '10'

          - name: Digits
            description: The length of a generated code. Six is what authenticator apps assume; eight is accepted by most but not all of them.
            type: int
            default: '6'

          - name: PeriodSeconds
            description: How long one code stays valid. The adjacent windows are also accepted, so the real tolerance is roughly three times this value.
            type: int
            default: '30'

          - name: PendingSecretMinutes
            description: How long a secret offered by an enrolment stays confirmable before it is refused. An enrolment left unfinished expires without touching the secret already in use, so an interrupted setup cannot cost a user their working authenticator.
            type: int
            default: '10'
---

# Configuration

The optional `AuthCredentials` section is bound to `AuthCredentialsSettings`, with lockout and two-factor values on nested objects. Token issuer, secret, and lifetimes live in the [`Auth`](/asp-net-auth/configuration) section instead, because they describe token minting rather than credentials.

```json
{
    "AuthCredentials": {
        "AbsoluteSessionLifetimeDays": 30,
        "PasswordResetMinutes": 60,
        "ForgotPasswordMinimumMilliseconds": 200,
        "Lockout": {
            "Enabled": false,
            "MaxFailedAttempts": 5,
            "DurationMinutes": 15
        },
        "TwoFactor": {
            "Issuer": null,
            "RecoveryCodeCount": 10,
            "Digits": 6,
            "PeriodSeconds": 30,
            "PendingSecretMinutes": 10
        }
    }
}
```

::: danger
Changing `Digits` or `PeriodSeconds` invalidates every existing enrolment. An authenticator was set up from the values in force when the user scanned the code, and it keeps generating codes to those, so every enrolled user has to enrol again after the change.
:::

<FrontmatterDocs/>

## Usage

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class PasswordResetLinkBuilder(
    IOptions<AuthCredentialsSettings> options
)
{
    public string BuildEmailBody(string token)
    {
        string url = $"https://example.com/reset?token={token}";
        int minutes = options.Value.PasswordResetMinutes;

        return $"Reset your password at {url}. "
               + $"The link stops working in {minutes} minutes.";
    }
}
```
