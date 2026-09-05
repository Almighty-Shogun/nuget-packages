# Localization

Auth resolves the message for each of its [exceptions](./exceptions) through [ASP.NET Localization](/asp-net-localization/localization), which loads JSON files from `messages/{language}` and uses each file name as the message group. Every message this package produces lives in the `auth` group. They reach the client only once [`UseHttpErrorResponses`](/asp-net-core/extensions/use-http-error-responses) is in the pipeline, since without it the exceptions still map but nothing writes the response they map to.

## Auth Messages

Create `messages/{language}/auth.json` for every language the API should support. The code-group labels below use language codes, but each block is the content for that language's `auth.json` file.

::: warning
The `auth` file is shared with [ASP.NET Auth Credentials](/asp-net-auth-credentials/localization) when both packages are installed. Add these keys alongside the ones that page lists rather than replacing the file, or that package's messages resolve to their raw keys.
:::

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
