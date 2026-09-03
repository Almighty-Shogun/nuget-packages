# Localization

Auth Credentials resolves the message for each of its [exceptions](./exceptions) through [ASP.NET Localization](/asp-net-localization/localization). The resolver loads JSON files from `messages/{language}`, uses the file name as the message group, and returns the original message key when no language, file, or entry is found.

Messages live in the `auth` and `passwords` groups. The `auth` file is shared with [ASP.NET Auth](/asp-net-auth/localization) when both are installed, so add these keys alongside its own rather than replacing the file. Request validation messages still come from the `validation` group documented by [ASP.NET Request Validation](/asp-net-request-validation/localization).

## Auth Messages

Create `messages/{language}/auth.json` for every language the API should support. The code-group labels below use language codes, but each block is the content for that language's `auth.json` file. `locked-out` receives the lockout expiry as `{0}`; every other key takes no parameters, so a `{0}` placed in one of them is written out to the client literally.

::: code-group

```json [en.json]
{
    "failed": "These credentials do not match our records.",
    "session-invalid": "This session is no longer valid. Please sign in again.",
    "two-factor-invalid": "This verification code is incorrect.",
    "disabled": "This account has been deactivated.",
    "locked-out": "Too many failed attempts. Try again after {0}.",
    "username-taken": "This username is already in use.",
    "email-taken": "This email address is already in use."
}
```

```json [nl.json]
{
    "failed": "Deze inloggegevens komen niet overeen met onze gegevens.",
    "session-invalid": "Deze sessie is niet langer geldig. Log opnieuw in.",
    "two-factor-invalid": "Deze verificatiecode is onjuist.",
    "disabled": "Dit account is gedeactiveerd.",
    "locked-out": "Te veel mislukte pogingen. Probeer het opnieuw na {0}.",
    "username-taken": "Deze gebruikersnaam is al in gebruik.",
    "email-taken": "Dit e-mailadres is al in gebruik."
}
```

```json [fr.json]
{
    "failed": "Ces identifiants ne correspondent pas à nos données.",
    "session-invalid": "Cette session n'est plus valide. Veuillez vous reconnecter.",
    "two-factor-invalid": "Ce code de vérification est incorrect.",
    "disabled": "Ce compte a été désactivé.",
    "locked-out": "Trop de tentatives échouées. Réessayez après {0}.",
    "username-taken": "Ce nom d'utilisateur est déjà utilisé.",
    "email-taken": "Cette adresse e-mail est déjà utilisée."
}
```

:::

## Password Messages

Create `messages/{language}/passwords.json` for the password-change and password-reset flows.

::: code-group

```json [en.json]
{
    "mismatch": "The provided passwords do not match.",
    "reused": "The new password must be different from your current password.",
    "token-invalid": "This password reset link is invalid or has expired."
}
```

```json [nl.json]
{
    "mismatch": "De opgegeven wachtwoorden komen niet overeen.",
    "reused": "Het nieuwe wachtwoord moet anders zijn dan je huidige wachtwoord.",
    "token-invalid": "Deze wachtwoordherstellink is ongeldig of verlopen."
}
```

```json [fr.json]
{
    "mismatch": "Les mots de passe saisis ne correspondent pas.",
    "reused": "Le nouveau mot de passe doit être différent de votre mot de passe actuel.",
    "token-invalid": "Ce lien de réinitialisation du mot de passe est invalide ou expiré."
}
```

:::
