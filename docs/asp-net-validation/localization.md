# Localization

Validation messages are resolved through [ASP.NET Utils localization](/asp-net-utils/localization). The resolver loads JSON files from `messages/{language}`, flattens the file name and nested JSON keys into message keys, and returns the original key when no language file or message exists.

ASP.NET Validation uses the `validation` message group for rule failures. A message stored in `messages/en/validation.json` at `required.default` resolves as `validation.required.default`. The package also uses ASP.NET Utils HTTP error messages for top-level `422` and invalid-body descriptions, so keep `messages/{language}/http-error.json` available as documented by [ASP.NET Utils Localization](/asp-net-utils/localization).

## Validation Messages

Create `messages/{language}/validation.json` for every language the API should support. The code-group labels below use language codes, but each block is the content for that language's `validation.json` file.

::: code-group

```json [en.json]
{
    "accepted": {
        "default": "This field must be accepted.",
        "if": "This field must be accepted when {0} is {1}."
    },
    "after": {
        "default": "This field must be a date after {0}.",
        "or-equal": "This field must be a date after or equal to {0}."
    },
    "alpha": {
        "default": "This field must only contain letters.",
        "dash": "This field must only contain letters, numbers, dashes, and underscores.",
        "num": "This field must only contain letters and numbers."
    },
    "any-of": "This field is invalid.",
    "array": "This field must be an array.",
    "ascii": "This field must only contain single-byte alphanumeric characters and symbols.",
    "before": {
        "default": "This field must be a date before {0}.",
        "or-equal": "This field must be a date before or equal to {0}."
    },
    "between": {
        "array": "This field must have between {0} and {1} items.",
        "file": "This field must be between {0} and {1} kilobytes.",
        "numeric": "This field must be between {0} and {1}.",
        "string": "This field must be between {0} and {1} characters."
    },
    "boolean": "This field must be true or false.",
    "confirmed": "This field confirmation does not match.",
    "contains": "This field is missing a required value.",
    "date": {
        "default": "This field must be a valid date.",
        "equals": "This field must be a date equal to {0}.",
        "format": "This field must match the format {0}."
    },
    "decimal": "This field must have {0} decimal places.",
    "declined": {
        "default": "This field must be declined.",
        "if": "This field must be declined when {0} is {1}."
    },
    "different": "This field and {0} must be different.",
    "digits": {
        "default": "This field must be {0} digits.",
        "between": "This field must be between {0} and {1} digits."
    },
    "dimensions": "This field has invalid image dimensions.",
    "distinct": "This field has a duplicate value.",
    "does-not": {
        "contain": "This field must not contain any of the following: {0}.",
        "end-with": "This field must not end with one of the following: {0}.",
        "start-with": "This field must not start with one of the following: {0}."
    },
    "email": "This field must be a valid email address.",
    "ends-with": "This field must end with one of the following: {0}.",
    "enum": "The selected {0} is invalid.",
    "extensions": "This field must have one of the following extensions: {0}.",
    "file": "This field must be a file.",
    "filled": "This field must have a value.",
    "greater-than": {
        "array": "This field must have more than {0} items.",
        "file": "This field must be greater than {0} kilobytes.",
        "numeric": "This field must be greater than {0}.",
        "string": "This field must be greater than {0} characters."
    },
    "greater-than-or-equal": {
        "array": "This field must have {0} items or more.",
        "file": "This field must be greater than or equal to {0} kilobytes.",
        "numeric": "This field must be greater than or equal to {0}.",
        "string": "This field must be greater than or equal to {0} characters."
    },
    "hex-color": "This field must be a valid hexadecimal color.",
    "image": "This field must be an image.",
    "in": {
        "default": "The selected {0} is invalid.",
        "array": "This field must exist in {0}.",
        "array-keys": "This field must contain at least one of the following keys: {0}."
    },
    "integer": "This field must be an integer.",
    "ip": {
        "default": "This field must be a valid IP address.",
        "ipv4": "This field must be a valid IPv4 address.",
        "ipv6": "This field must be a valid IPv6 address."
    },
    "json": "This field must be a valid JSON string.",
    "list": "This field must be a list.",
    "lowercase": "This field must be lowercase.",
    "less-than": {
        "array": "This field must have less than {0} items.",
        "file": "This field must be less than {0} kilobytes.",
        "numeric": "This field must be less than {0}.",
        "string": "This field must be less than {0} characters."
    },
    "less-than-or-equal": {
        "array": "This field must not have more than {0} items.",
        "file": "This field must be less than or equal to {0} kilobytes.",
        "numeric": "This field must be less than or equal to {0}.",
        "string": "This field must be less than or equal to {0} characters."
    },
    "mac-address": "This field must be a valid MAC address.",
    "max": {
        "array": "This field must not have more than {0} items.",
        "file": "This field must not be greater than {0} kilobytes.",
        "numeric": "This field must not be greater than {0}.",
        "string": "This field must not be greater than {0} characters.",
        "digits": "This field must not have more than {0} digits."
    },
    "min": {
        "array": "This field must have at least {0} items.",
        "file": "This field must be at least {0} kilobytes.",
        "numeric": "This field must be at least {0}.",
        "string": "This field must be at least {0} characters.",
        "digits": "This field must have at least {0} digits."
    },
    "mimes": "This field must be a file of type: {0}.",
    "mimetypes": "This field must be a file of type: {0}.",
    "missing": {
        "default": "This field must be missing.",
        "if": "This field must be missing when {0} is {1}.",
        "unless": "This field must be missing unless {0} is {1}.",
        "with": "This field must be missing when {0} is present.",
        "with-all": "This field must be missing when {0} are present."
    },
    "multiple-of": "This field must be a multiple of {0}.",
    "not": {
        "in": "The selected {0} is invalid.",
        "regex": "This field format is invalid."
    },
    "numeric": "This field must be a number.",
    "password": {
        "letters": "This field must contain at least one letter.",
        "mixed": "This field must contain at least one uppercase and one lowercase letter.",
        "numbers": "This field must contain at least one number.",
        "secure": "This field must contain uppercase and lowercase letters, at least one number, and at least one symbol.",
        "symbols": "This field must contain at least one symbol."
    },
    "present": {
        "default": "This field must be present.",
        "if": "This field must be present when {0} is {1}.",
        "unless": "This field must be present unless {0} is {1}.",
        "with": "This field must be present when {0} is present.",
        "with-all": "This field must be present when {0} are present."
    },
    "prohibited": {
        "default": "This field is prohibited.",
        "if": "This field is prohibited when {0} is {1}.",
        "if-accepted": "This field is prohibited when {0} is accepted.",
        "if-declined": "This field is prohibited when {0} is declined.",
        "unless": "This field is prohibited unless {0} is in {1}."
    },
    "prohibits": "This field prohibits {0} from being present.",
    "regex": "This field format is invalid.",
    "required": {
        "default": "This field is required.",
        "array-keys": "This field must contain entries for: {0}.",
        "if": "This field is required when {0} is {1}.",
        "if-accepted": "This field is required when {0} is accepted.",
        "if-declined": "This field is required when {0} is declined.",
        "unless": "This field is required unless {0} is in {1}.",
        "with": "This field is required when {0} is present.",
        "with-all": "This field is required when {0} are present.",
        "without": "This field is required when {0} is not present.",
        "without-all": "This field is required when none of {0} are present."
    },
    "same": "This field must match {0}.",
    "size": {
        "array": "This field must contain {0} items.",
        "file": "This field must be {0} kilobytes.",
        "numeric": "This field must be {0}.",
        "string": "This field must be {0} characters."
    },
    "starts-with": "This field must start with one of the following: {0}.",
    "string": "This field must be a string.",
    "timezone": "This field must be a valid timezone.",
    "ulid": "This field must be a valid ULID.",
    "unique": "This field has already been taken.",
    "uploaded": "The {0} failed to upload.",
    "uppercase": "This field must be uppercase.",
    "url": "This field must be a valid URL.",
    "uuid": "This field must be a valid UUID."
}
```

```json [nl.json]
{
    "accepted": {
        "default": "Dit veld moet geaccepteerd zijn.",
        "if": "Dit veld moet geaccepteerd zijn wanneer {0} {1} is."
    },
    "after": {
        "default": "Dit veld moet een datum na {0} zijn.",
        "or-equal": "Dit veld moet een datum na of gelijk aan {0} zijn."
    },
    "alpha": {
        "default": "Dit veld mag alleen letters bevatten.",
        "dash": "Dit veld mag alleen letters, cijfers, streepjes en underscores bevatten.",
        "num": "Dit veld mag alleen letters en cijfers bevatten."
    },
    "any-of": "Dit veld is ongeldig.",
    "array": "Dit veld moet een array zijn.",
    "ascii": "Dit veld mag alleen enkelbyte alfanumerieke tekens en symbolen bevatten.",
    "before": {
        "default": "Dit veld moet een datum voor {0} zijn.",
        "or-equal": "Dit veld moet een datum voor of gelijk aan {0} zijn."
    },
    "between": {
        "array": "Dit veld moet tussen {0} en {1} items bevatten.",
        "file": "Dit veld moet tussen {0} en {1} kilobytes zijn.",
        "numeric": "Dit veld moet tussen {0} en {1} zijn.",
        "string": "Dit veld moet tussen {0} en {1} tekens bevatten."
    },
    "boolean": "Dit veld moet true of false zijn.",
    "confirmed": "De bevestiging van dit veld komt niet overeen.",
    "contains": "Dit veld mist een verplichte waarde.",
    "date": {
        "default": "Dit veld moet een geldige datum zijn.",
        "equals": "Dit veld moet een datum gelijk aan {0} zijn.",
        "format": "Dit veld moet overeenkomen met het formaat {0}."
    },
    "decimal": "Dit veld moet {0} decimalen hebben.",
    "declined": {
        "default": "Dit veld moet geweigerd zijn.",
        "if": "Dit veld moet geweigerd zijn wanneer {0} {1} is."
    },
    "different": "Dit veld en {0} moeten verschillend zijn.",
    "digits": {
        "default": "Dit veld moet {0} cijfers bevatten.",
        "between": "Dit veld moet tussen {0} en {1} cijfers bevatten."
    },
    "dimensions": "Dit veld heeft ongeldige afbeeldingsafmetingen.",
    "distinct": "Dit veld bevat een dubbele waarde.",
    "does-not": {
        "contain": "Dit veld mag geen van de volgende waarden bevatten: {0}.",
        "end-with": "Dit veld mag niet eindigen met een van de volgende waarden: {0}.",
        "start-with": "Dit veld mag niet beginnen met een van de volgende waarden: {0}."
    },
    "email": "Dit veld moet een geldig e-mailadres zijn.",
    "ends-with": "Dit veld moet eindigen met een van de volgende waarden: {0}.",
    "enum": "De geselecteerde {0} is ongeldig.",
    "extensions": "Dit veld moet een van de volgende extensies hebben: {0}.",
    "file": "Dit veld moet een bestand zijn.",
    "filled": "Dit veld moet een waarde bevatten.",
    "greater-than": {
        "array": "Dit veld moet meer dan {0} items bevatten.",
        "file": "Dit veld moet groter zijn dan {0} kilobytes.",
        "numeric": "Dit veld moet groter zijn dan {0}.",
        "string": "Dit veld moet meer dan {0} tekens bevatten."
    },
    "greater-than-or-equal": {
        "array": "Dit veld moet {0} of meer items bevatten.",
        "file": "Dit veld moet groter dan of gelijk aan {0} kilobytes zijn.",
        "numeric": "Dit veld moet groter dan of gelijk aan {0} zijn.",
        "string": "Dit veld moet {0} of meer tekens bevatten."
    },
    "hex-color": "Dit veld moet een geldige hexadecimale kleur zijn.",
    "image": "Dit veld moet een afbeelding zijn.",
    "in": {
        "default": "De geselecteerde {0} is ongeldig.",
        "array": "Dit veld moet bestaan in {0}.",
        "array-keys": "Dit veld moet minimaal een van de volgende sleutels bevatten: {0}."
    },
    "integer": "Dit veld moet een geheel getal zijn.",
    "ip": {
        "default": "Dit veld moet een geldig IP-adres zijn.",
        "ipv4": "Dit veld moet een geldig IPv4-adres zijn.",
        "ipv6": "Dit veld moet een geldig IPv6-adres zijn."
    },
    "json": "Dit veld moet een geldige JSON-string zijn.",
    "list": "Dit veld moet een lijst zijn.",
    "lowercase": "Dit veld moet kleine letters bevatten.",
    "less-than": {
        "array": "Dit veld moet minder dan {0} items bevatten.",
        "file": "Dit veld moet kleiner zijn dan {0} kilobytes.",
        "numeric": "Dit veld moet kleiner zijn dan {0}.",
        "string": "Dit veld moet minder dan {0} tekens bevatten."
    },
    "less-than-or-equal": {
        "array": "Dit veld mag niet meer dan {0} items bevatten.",
        "file": "Dit veld moet kleiner dan of gelijk aan {0} kilobytes zijn.",
        "numeric": "Dit veld moet kleiner dan of gelijk aan {0} zijn.",
        "string": "Dit veld moet kleiner dan of gelijk aan {0} tekens zijn."
    },
    "mac-address": "Dit veld moet een geldig MAC-adres zijn.",
    "max": {
        "array": "Dit veld mag niet meer dan {0} items bevatten.",
        "file": "Dit veld mag niet groter zijn dan {0} kilobytes.",
        "numeric": "Dit veld mag niet groter zijn dan {0}.",
        "string": "Dit veld mag niet meer dan {0} tekens bevatten.",
        "digits": "Dit veld mag niet meer dan {0} cijfers bevatten."
    },
    "min": {
        "array": "Dit veld moet minimaal {0} items bevatten.",
        "file": "Dit veld moet minimaal {0} kilobytes zijn.",
        "numeric": "Dit veld moet minimaal {0} zijn.",
        "string": "Dit veld moet minimaal {0} tekens bevatten.",
        "digits": "Dit veld moet minimaal {0} cijfers bevatten."
    },
    "mimes": "Dit veld moet een bestand zijn van het type: {0}.",
    "mimetypes": "Dit veld moet een bestand zijn van het type: {0}.",
    "missing": {
        "default": "Dit veld moet ontbreken.",
        "if": "Dit veld moet ontbreken wanneer {0} {1} is.",
        "unless": "Dit veld moet ontbreken tenzij {0} {1} is.",
        "with": "Dit veld moet ontbreken wanneer {0} aanwezig is.",
        "with-all": "Dit veld moet ontbreken wanneer {0} aanwezig zijn."
    },
    "multiple-of": "Dit veld moet een veelvoud van {0} zijn.",
    "not": {
        "in": "De geselecteerde {0} is ongeldig.",
        "regex": "Het formaat van dit veld is ongeldig."
    },
    "numeric": "Dit veld moet een getal zijn.",
    "password": {
        "letters": "Dit veld moet minimaal een letter bevatten.",
        "mixed": "Dit veld moet minimaal een hoofdletter en een kleine letter bevatten.",
        "numbers": "Dit veld moet minimaal een cijfer bevatten.",
        "secure": "Dit veld moet hoofdletters en kleine letters, minimaal een cijfer en minimaal een symbool bevatten.",
        "symbols": "Dit veld moet minimaal een symbool bevatten."
    },
    "present": {
        "default": "Dit veld moet aanwezig zijn.",
        "if": "Dit veld moet aanwezig zijn wanneer {0} {1} is.",
        "unless": "Dit veld moet aanwezig zijn tenzij {0} {1} is.",
        "with": "Dit veld moet aanwezig zijn wanneer {0} aanwezig is.",
        "with-all": "Dit veld moet aanwezig zijn wanneer {0} aanwezig zijn."
    },
    "prohibited": {
        "default": "Dit veld is verboden.",
        "if": "Dit veld is verboden wanneer {0} {1} is.",
        "if-accepted": "Dit veld is verboden wanneer {0} geaccepteerd is.",
        "if-declined": "Dit veld is verboden wanneer {0} geweigerd is.",
        "unless": "Dit veld is verboden tenzij {0} in {1} zit."
    },
    "prohibits": "Dit veld verbiedt dat {0} aanwezig is.",
    "regex": "Het formaat van dit veld is ongeldig.",
    "required": {
        "default": "Dit veld is verplicht.",
        "array-keys": "Dit veld moet waarden bevatten voor: {0}.",
        "if": "Dit veld is verplicht wanneer {0} {1} is.",
        "if-accepted": "Dit veld is verplicht wanneer {0} geaccepteerd is.",
        "if-declined": "Dit veld is verplicht wanneer {0} geweigerd is.",
        "unless": "Dit veld is verplicht tenzij {0} in {1} zit.",
        "with": "Dit veld is verplicht wanneer {0} aanwezig is.",
        "with-all": "Dit veld is verplicht wanneer {0} aanwezig zijn.",
        "without": "Dit veld is verplicht wanneer {0} niet aanwezig is.",
        "without-all": "Dit veld is verplicht wanneer geen van {0} aanwezig is."
    },
    "same": "Dit veld moet overeenkomen met {0}.",
    "size": {
        "array": "Dit veld moet {0} items bevatten.",
        "file": "Dit veld moet {0} kilobytes zijn.",
        "numeric": "Dit veld moet {0} zijn.",
        "string": "Dit veld moet {0} tekens bevatten."
    },
    "starts-with": "Dit veld moet beginnen met een van de volgende waarden: {0}.",
    "string": "Dit veld moet een string zijn.",
    "timezone": "Dit veld moet een geldige tijdzone zijn.",
    "ulid": "Dit veld moet een geldige ULID zijn.",
    "unique": "Dit veld is al in gebruik.",
    "uploaded": "Het uploaden van {0} is mislukt.",
    "uppercase": "Dit veld moet hoofdletters bevatten.",
    "url": "Dit veld moet een geldige URL zijn.",
    "uuid": "Dit veld moet een geldige UUID zijn."
}
```

```json [fr.json]
{
    "accepted": {
        "default": "Ce champ doit être accepté.",
        "if": "Ce champ doit être accepté lorsque {0} vaut {1}."
    },
    "after": {
        "default": "Ce champ doit être une date après {0}.",
        "or-equal": "Ce champ doit être une date après ou égale à {0}."
    },
    "alpha": {
        "default": "Ce champ ne doit contenir que des lettres.",
        "dash": "Ce champ ne doit contenir que des lettres, des chiffres, des tirets et des underscores.",
        "num": "Ce champ ne doit contenir que des lettres et des chiffres."
    },
    "any-of": "Ce champ est invalide.",
    "array": "Ce champ doit être un tableau.",
    "ascii": "Ce champ ne doit contenir que des caractères alphanumériques et des symboles sur un octet.",
    "before": {
        "default": "Ce champ doit être une date avant {0}.",
        "or-equal": "Ce champ doit être une date avant ou égale à {0}."
    },
    "between": {
        "array": "Ce champ doit contenir entre {0} et {1} éléments.",
        "file": "Ce champ doit être compris entre {0} et {1} kilo-octets.",
        "numeric": "Ce champ doit être compris entre {0} et {1}.",
        "string": "Ce champ doit contenir entre {0} et {1} caractères."
    },
    "boolean": "Ce champ doit être true ou false.",
    "confirmed": "La confirmation de ce champ ne correspond pas.",
    "contains": "Ce champ ne contient pas une valeur requise.",
    "date": {
        "default": "Ce champ doit être une date valide.",
        "equals": "Ce champ doit être une date égale à {0}.",
        "format": "Ce champ doit correspondre au format {0}."
    },
    "decimal": "Ce champ doit avoir {0} décimales.",
    "declined": {
        "default": "Ce champ doit être refusé.",
        "if": "Ce champ doit être refusé lorsque {0} vaut {1}."
    },
    "different": "Ce champ et {0} doivent être différents.",
    "digits": {
        "default": "Ce champ doit contenir {0} chiffres.",
        "between": "Ce champ doit contenir entre {0} et {1} chiffres."
    },
    "dimensions": "Ce champ a des dimensions d'image invalides.",
    "distinct": "Ce champ contient une valeur en double.",
    "does-not": {
        "contain": "Ce champ ne doit contenir aucune des valeurs suivantes : {0}.",
        "end-with": "Ce champ ne doit pas se terminer par l'une des valeurs suivantes : {0}.",
        "start-with": "Ce champ ne doit pas commencer par l'une des valeurs suivantes : {0}."
    },
    "email": "Ce champ doit être une adresse e-mail valide.",
    "ends-with": "Ce champ doit se terminer par l'une des valeurs suivantes : {0}.",
    "enum": "Le {0} sélectionné est invalide.",
    "extensions": "Ce champ doit avoir l'une des extensions suivantes : {0}.",
    "file": "Ce champ doit être un fichier.",
    "filled": "Ce champ doit avoir une valeur.",
    "greater-than": {
        "array": "Ce champ doit contenir plus de {0} éléments.",
        "file": "Ce champ doit être supérieur à {0} kilo-octets.",
        "numeric": "Ce champ doit être supérieur à {0}.",
        "string": "Ce champ doit contenir plus de {0} caractères."
    },
    "greater-than-or-equal": {
        "array": "Ce champ doit contenir au moins {0} éléments.",
        "file": "Ce champ doit être supérieur ou égal à {0} kilo-octets.",
        "numeric": "Ce champ doit être supérieur ou égal à {0}.",
        "string": "Ce champ doit contenir au moins {0} caractères."
    },
    "hex-color": "Ce champ doit être une couleur hexadécimale valide.",
    "image": "Ce champ doit être une image.",
    "in": {
        "default": "Le {0} sélectionné est invalide.",
        "array": "Ce champ doit exister dans {0}.",
        "array-keys": "Ce champ doit contenir au moins l'une des clés suivantes : {0}."
    },
    "integer": "Ce champ doit être un entier.",
    "ip": {
        "default": "Ce champ doit être une adresse IP valide.",
        "ipv4": "Ce champ doit être une adresse IPv4 valide.",
        "ipv6": "Ce champ doit être une adresse IPv6 valide."
    },
    "json": "Ce champ doit être une chaîne JSON valide.",
    "list": "Ce champ doit être une liste.",
    "lowercase": "Ce champ doit être en minuscules.",
    "less-than": {
        "array": "Ce champ doit contenir moins de {0} éléments.",
        "file": "Ce champ doit être inférieur à {0} kilo-octets.",
        "numeric": "Ce champ doit être inférieur à {0}.",
        "string": "Ce champ doit contenir moins de {0} caractères."
    },
    "less-than-or-equal": {
        "array": "Ce champ ne doit pas contenir plus de {0} éléments.",
        "file": "Ce champ doit être inférieur ou égal à {0} kilo-octets.",
        "numeric": "Ce champ doit être inférieur ou égal à {0}.",
        "string": "Ce champ doit contenir au plus {0} caractères."
    },
    "mac-address": "Ce champ doit être une adresse MAC valide.",
    "max": {
        "array": "Ce champ ne doit pas contenir plus de {0} éléments.",
        "file": "Ce champ ne doit pas dépasser {0} kilo-octets.",
        "numeric": "Ce champ ne doit pas être supérieur à {0}.",
        "string": "Ce champ ne doit pas dépasser {0} caractères.",
        "digits": "Ce champ ne doit pas contenir plus de {0} chiffres."
    },
    "min": {
        "array": "Ce champ doit contenir au moins {0} éléments.",
        "file": "Ce champ doit être d'au moins {0} kilo-octets.",
        "numeric": "Ce champ doit être au moins {0}.",
        "string": "Ce champ doit contenir au moins {0} caractères.",
        "digits": "Ce champ doit contenir au moins {0} chiffres."
    },
    "mimes": "Ce champ doit être un fichier de type : {0}.",
    "mimetypes": "Ce champ doit être un fichier de type : {0}.",
    "missing": {
        "default": "Ce champ doit être absent.",
        "if": "Ce champ doit être absent lorsque {0} vaut {1}.",
        "unless": "Ce champ doit être absent sauf si {0} vaut {1}.",
        "with": "Ce champ doit être absent lorsque {0} est présent.",
        "with-all": "Ce champ doit être absent lorsque {0} sont présents."
    },
    "multiple-of": "Ce champ doit être un multiple de {0}.",
    "not": {
        "in": "Le {0} sélectionné est invalide.",
        "regex": "Le format de ce champ est invalide."
    },
    "numeric": "Ce champ doit être un nombre.",
    "password": {
        "letters": "Ce champ doit contenir au moins une lettre.",
        "mixed": "Ce champ doit contenir au moins une majuscule et une minuscule.",
        "numbers": "Ce champ doit contenir au moins un chiffre.",
        "secure": "Ce champ doit contenir des majuscules et des minuscules, au moins un chiffre et au moins un symbole.",
        "symbols": "Ce champ doit contenir au moins un symbole."
    },
    "present": {
        "default": "Ce champ doit être présent.",
        "if": "Ce champ doit être présent lorsque {0} vaut {1}.",
        "unless": "Ce champ doit être présent sauf si {0} vaut {1}.",
        "with": "Ce champ doit être présent lorsque {0} est présent.",
        "with-all": "Ce champ doit être présent lorsque {0} sont présents."
    },
    "prohibited": {
        "default": "Ce champ est interdit.",
        "if": "Ce champ est interdit lorsque {0} vaut {1}.",
        "if-accepted": "Ce champ est interdit lorsque {0} est accepté.",
        "if-declined": "Ce champ est interdit lorsque {0} est refusé.",
        "unless": "Ce champ est interdit sauf si {0} est dans {1}."
    },
    "prohibits": "Ce champ interdit la présence de {0}.",
    "regex": "Le format de ce champ est invalide.",
    "required": {
        "default": "Ce champ est requis.",
        "array-keys": "Ce champ doit contenir des entrées pour : {0}.",
        "if": "Ce champ est requis lorsque {0} vaut {1}.",
        "if-accepted": "Ce champ est requis lorsque {0} est accepté.",
        "if-declined": "Ce champ est requis lorsque {0} est refusé.",
        "unless": "Ce champ est requis sauf si {0} est dans {1}.",
        "with": "Ce champ est requis lorsque {0} est présent.",
        "with-all": "Ce champ est requis lorsque {0} sont présents.",
        "without": "Ce champ est requis lorsque {0} n'est pas présent.",
        "without-all": "Ce champ est requis lorsqu'aucun de {0} n'est présent."
    },
    "same": "Ce champ doit correspondre à {0}.",
    "size": {
        "array": "Ce champ doit contenir {0} éléments.",
        "file": "Ce champ doit faire {0} kilo-octets.",
        "numeric": "Ce champ doit être {0}.",
        "string": "Ce champ doit contenir {0} caractères."
    },
    "starts-with": "Ce champ doit commencer par l'une des valeurs suivantes : {0}.",
    "string": "Ce champ doit être une chaîne.",
    "timezone": "Ce champ doit être un fuseau horaire valide.",
    "ulid": "Ce champ doit être un ULID valide.",
    "unique": "Ce champ est déjà utilisé.",
    "uploaded": "Le téléversement de {0} a échoué.",
    "uppercase": "Ce champ doit être en majuscules.",
    "url": "Ce champ doit être une URL valide.",
    "uuid": "Ce champ doit être un UUID valide."
}
```

:::

## Message Parameters

Rule messages can include numbered placeholders such as `{0}` and `{1}`. The package passes rule-specific values into the resolver, such as comparison field names, allowed values, min/max values, or file extensions. Custom rules can return their own parameters through [`ValidationRuleResult.Failure`](./records/validation-rule-result).
