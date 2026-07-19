# Localization

ASP.NET Utils supports localized messages through language-specific JSON files under `messages/{language}`. A language directory can contain one file or several files; when that language is requested, the resolver loads every `.json` file in that directory and flattens them into message keys.

The file name becomes the message group. For example, `messages/en/users.json` with a `not-found` property resolves the key `users.not-found`, while `messages/en/http-error.json` with a `404` property resolves the key `http-error.404`.

The resolver first tries the request language from `Accept-Language`, then the neutral language for culture-specific values such as `nl-BE`, and finally the configured `DefaultLanguage` from `appsettings.json`. When no matching language directory, message file, or message key is found, the resolver returns the original message key so callers still receive a stable fallback value.

## HTTP Error Messages

Standardized HTTP error responses require an `http-error.json` file for every language the application should support. Place each file in the matching language directory, for example `messages/en/http-error.json` or `messages/nl/http-error.json`.

Copy each example below into `http-error.json` in the matching language directory. The code-group labels show the language example, not the file name to use.

::: code-group

```json [en.json]
{
    "invalid-body": "The request body is missing or invalid.",
    "400": "The request is invalid.",
    "401": "Authentication is required.",
    "402": "Payment is required.",
    "403": "You do not have permission to access this resource.",
    "404": "The requested resource was not found.",
    "405": "This request method is not allowed for this resource.",
    "406": "The requested response format is not acceptable.",
    "407": "Proxy authentication is required.",
    "408": "The request timed out.",
    "409": "The request conflicts with the current state of the resource.",
    "410": "The requested resource is no longer available.",
    "411": "The request must include a valid content length.",
    "412": "A request precondition failed.",
    "413": "The request body is too large.",
    "414": "The request URI is too long.",
    "415": "The request content type is not supported.",
    "416": "The requested range is not satisfiable.",
    "417": "The request expectation could not be met.",
    "418": "The request cannot be processed.",
    "421": "The request was directed at a server that cannot produce a response.",
    "422": "The given data was invalid.",
    "423": "The resource is locked.",
    "424": "The request failed because a dependent request failed.",
    "425": "The server is unwilling to risk processing a request that might be replayed.",
    "426": "An upgrade is required to complete the request.",
    "428": "A precondition is required for this request.",
    "429": "Too many requests. Please try again later.",
    "431": "The request headers are too large.",
    "451": "The requested resource is unavailable for legal reasons.",
    "500": "An unexpected error occurred.",
    "501": "This request is not implemented.",
    "502": "The server received an invalid response from an upstream service.",
    "503": "The service is temporarily unavailable.",
    "504": "The upstream service did not respond in time.",
    "505": "The HTTP version is not supported.",
    "506": "The server has an internal configuration error.",
    "507": "The server does not have enough storage to complete the request.",
    "508": "The server detected a loop while processing the request.",
    "510": "Further extensions are required to complete the request.",
    "511": "Network authentication is required."
}
```

```json [nl.json]
{
    "invalid-body": "De aanvraagbody ontbreekt of is ongeldig.",
    "400": "De aanvraag is ongeldig.",
    "401": "Authenticatie is vereist.",
    "402": "Betaling is vereist.",
    "403": "Je hebt geen toestemming om deze resource te openen.",
    "404": "De gevraagde resource is niet gevonden.",
    "405": "Deze aanvraagmethode is niet toegestaan voor deze resource.",
    "406": "Het gevraagde antwoordformaat is niet acceptabel.",
    "407": "Proxy-authenticatie is vereist.",
    "408": "De aanvraag is verlopen.",
    "409": "De aanvraag conflicteert met de huidige staat van de resource.",
    "410": "De gevraagde resource is niet langer beschikbaar.",
    "411": "De aanvraag moet een geldige contentlengte bevatten.",
    "412": "Een aanvraagvoorwaarde is mislukt.",
    "413": "De aanvraagbody is te groot.",
    "414": "De aanvraag-URI is te lang.",
    "415": "Het contenttype van de aanvraag wordt niet ondersteund.",
    "416": "Het gevraagde bereik kan niet worden geleverd.",
    "417": "Aan de verwachting van de aanvraag kon niet worden voldaan.",
    "418": "De aanvraag kan niet worden verwerkt.",
    "421": "De aanvraag is naar een server gestuurd die geen antwoord kan produceren.",
    "422": "De opgegeven gegevens zijn ongeldig.",
    "423": "De resource is vergrendeld.",
    "424": "De aanvraag is mislukt omdat een afhankelijke aanvraag is mislukt.",
    "425": "De server wil het risico niet nemen om een aanvraag te verwerken die opnieuw kan worden afgespeeld.",
    "426": "Een upgrade is vereist om de aanvraag te voltooien.",
    "428": "Een voorwaarde is vereist voor deze aanvraag.",
    "429": "Te veel aanvragen. Probeer het later opnieuw.",
    "431": "De aanvraagheaders zijn te groot.",
    "451": "De gevraagde resource is om juridische redenen niet beschikbaar.",
    "500": "Er is een onverwachte fout opgetreden.",
    "501": "Deze aanvraag is niet geïmplementeerd.",
    "502": "De server heeft een ongeldig antwoord ontvangen van een upstreamservice.",
    "503": "De service is tijdelijk niet beschikbaar.",
    "504": "De upstreamservice heeft niet op tijd geantwoord.",
    "505": "De HTTP-versie wordt niet ondersteund.",
    "506": "De server heeft een interne configuratiefout.",
    "507": "De server heeft onvoldoende opslagruimte om de aanvraag te voltooien.",
    "508": "De server heeft een lus gedetecteerd tijdens het verwerken van de aanvraag.",
    "510": "Er zijn verdere uitbreidingen vereist om de aanvraag te voltooien.",
    "511": "Netwerkauthenticatie is vereist."
}
```

```json [fr.json]
{
    "invalid-body": "Le corps de la requête est manquant ou invalide.",
    "400": "La requête est invalide.",
    "401": "Une authentification est requise.",
    "402": "Un paiement est requis.",
    "403": "Vous n'avez pas l'autorisation d'accéder à cette ressource.",
    "404": "La ressource demandée est introuvable.",
    "405": "Cette méthode de requête n'est pas autorisée pour cette ressource.",
    "406": "Le format de réponse demandé n'est pas acceptable.",
    "407": "Une authentification proxy est requise.",
    "408": "La requête a expiré.",
    "409": "La requête est en conflit avec l'état actuel de la ressource.",
    "410": "La ressource demandée n'est plus disponible.",
    "411": "La requête doit inclure une longueur de contenu valide.",
    "412": "Une précondition de la requête a échoué.",
    "413": "Le corps de la requête est trop volumineux.",
    "414": "L'URI de la requête est trop longue.",
    "415": "Le type de contenu de la requête n'est pas pris en charge.",
    "416": "La plage demandée ne peut pas être satisfaite.",
    "417": "L'attente de la requête n'a pas pu être satisfaite.",
    "418": "La requête ne peut pas être traitée.",
    "421": "La requête a été envoyée à un serveur qui ne peut pas produire de réponse.",
    "422": "Les données fournies sont invalides.",
    "423": "La ressource est verrouillée.",
    "424": "La requête a échoué parce qu'une requête dépendante a échoué.",
    "425": "Le serveur refuse de risquer le traitement d'une requête qui pourrait être rejouée.",
    "426": "Une mise à niveau est requise pour terminer la requête.",
    "428": "Une précondition est requise pour cette requête.",
    "429": "Trop de requêtes. Veuillez réessayer plus tard.",
    "431": "Les en-têtes de la requête sont trop volumineux.",
    "451": "La ressource demandée est indisponible pour des raisons légales.",
    "500": "Une erreur inattendue s'est produite.",
    "501": "Cette requête n'est pas implémentée.",
    "502": "Le serveur a reçu une réponse invalide d'un service en amont.",
    "503": "Le service est temporairement indisponible.",
    "504": "Le service en amont n'a pas répondu à temps.",
    "505": "La version HTTP n'est pas prise en charge.",
    "506": "Le serveur présente une erreur de configuration interne.",
    "507": "Le serveur ne dispose pas d'un espace de stockage suffisant pour terminer la requête.",
    "508": "Le serveur a détecté une boucle lors du traitement de la requête.",
    "510": "Des extensions supplémentaires sont requises pour terminer la requête.",
    "511": "Une authentification réseau est requise."
}
```

:::
