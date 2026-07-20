# Localization

Credential Auth resolves login and password messages through [ASP.NET Utils localization](/asp-net-utils/localization). The resolver loads JSON files from `messages/{language}`, uses the file name as the message group, and returns the original message key when no language, file, or message entry is found.

Credential-specific failures use the `auth` and `passwords` message groups. Built-in request validation still uses the `validation` group documented by [ASP.NET Validation Localization](/asp-net-validation/localization), and HTTP status descriptions still use `http-error.json` from [ASP.NET Utils Localization](/asp-net-utils/localization).

Only the keys shown below are returned by the package itself. Throttle or success messages belong in application code until Credential Auth exposes throttling or response-message behavior directly.

## Auth Messages

Create `messages/{language}/auth.json` for every language the API should support. The code-group labels below use language codes, but each block is the content for that language's `auth.json` file.

::: code-group

```json [en.json]
{
    "failed": "These credentials do not match our records."
}
```

```json [nl.json]
{
    "failed": "Deze inloggegevens komen niet overeen met onze gegevens."
}
```

```json [fr.json]
{
    "failed": "Ces identifiants ne correspondent pas à nos données."
}
```

:::

## Password Messages

Create `messages/{language}/passwords.json` for password-change and password-reset flows. Credential Auth uses these keys when validating the current password, rejecting reused passwords, matching password confirmation, and checking password reset tokens.

::: code-group

```json [en.json]
{
    "current": "The current password is incorrect.",
    "match": "The provided passwords do not match.",
    "reused": "The new password must be different from your current password.",
    "token": "This password reset token is invalid."
}
```

```json [nl.json]
{
    "current": "Het huidige wachtwoord is onjuist.",
    "match": "De opgegeven wachtwoorden komen niet overeen.",
    "reused": "Het nieuwe wachtwoord moet anders zijn dan je huidige wachtwoord.",
    "token": "Dit wachtwoordhersteltoken is ongeldig."
}
```

```json [fr.json]
{
    "current": "Le mot de passe actuel est incorrect.",
    "match": "Les mots de passe saisis ne correspondent pas.",
    "reused": "Le nouveau mot de passe doit être différent de votre mot de passe actuel.",
    "token": "Ce jeton de réinitialisation du mot de passe est invalide."
}
```

:::
