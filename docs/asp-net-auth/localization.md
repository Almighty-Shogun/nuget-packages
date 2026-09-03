# Localization

Auth resolves the message for each of its [exceptions](./exceptions) through [ASP.NET Localization](/asp-net-localization/localization). The resolver loads JSON files from `messages/{language}`, uses the file name as the message group, and returns the original message key when no language, file, or entry is found.

Every message this package produces lives in the `auth` group. That file is shared with [ASP.NET Auth Credentials](/asp-net-auth-credentials/localization) when both are installed, so add these keys alongside its own rather than replacing the file.

## Auth Messages

Create `messages/{language}/auth.json` for every language the API should support. The code-group labels below use language codes, but each block is the content for that language's `auth.json` file.

::: code-group

```json [en.json]
{
    "missing-user-id": "The request is not associated with a signed-in user.",
    "missing-refresh-token": "This request requires a refresh token.",
    "unknown-app": "This application is not available on this address."
}
```

```json [nl.json]
{
    "missing-user-id": "De aanvraag is niet gekoppeld aan een ingelogde gebruiker.",
    "missing-refresh-token": "Deze aanvraag vereist een vernieuwingstoken.",
    "unknown-app": "Deze applicatie is niet beschikbaar op dit adres."
}
```

```json [fr.json]
{
    "missing-user-id": "La requête n'est associée à aucun utilisateur connecté.",
    "missing-refresh-token": "Cette requête nécessite un jeton de rafraîchissement.",
    "unknown-app": "Cette application n'est pas disponible à cette adresse."
}
```

:::

::: tip
These messages reach the client only once [`UseHttpErrorResponses`](/asp-net-core/extensions/use-http-error-responses) is in the pipeline. Without it the exceptions still map, but nothing writes the response they map to.
:::
