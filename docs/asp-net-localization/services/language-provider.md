# LanguageProvider

Decides which languages a request wants. The default implementation reads the `Accept-Language` header, falling back to `DefaultLanguage` from [`LocalizationSettings`](../configuration) when the header is absent or contains no well-formed language tag. [`AddMessageLocalization`](../extensions/add-message-localization) registers that default unconditionally, so a custom provider has to replace it afterwards rather than being registered ahead of it.

## Usage

```csharp
using AlmightyShogun.AspNet.Localization;

public sealed class WelcomeService(ILanguageProvider languageProvider)
{
    public string WelcomeMessage() => languageProvider.GetLanguage() switch
    {
        "nl" => "Welkom",
        "fr" => "Bienvenue",
        _ => "Welcome"
    };
}
```

::: warning
The returned value is used as a directory name when message files are resolved, so the message provider accepts only a well-formed language tag: two or three letters, then any number of `-` separated parts of two to eight letters or digits. A replacement implementation that returns raw user input therefore resolves no messages at all rather than reading elsewhere on disk.
:::

## GetLanguage

Returns the single best language for the current request, ignoring any lower-ranked alternative the caller would accept. Never blank: with nothing to negotiate from it returns the configured default, since the result is looked up as-is.

```csharp
using System.Globalization;
using AlmightyShogun.AspNet.Localization;

public sealed class CurrencyFormatter(ILanguageProvider languageProvider)
{
    public string Format(decimal amount) => amount.ToString(
        "C",
        new CultureInfo(languageProvider.GetLanguage())
    );
}
```

### Type signature

```csharp
string GetLanguage();
```

## GetLanguages

Returns every language the caller accepts, in preference order, so [`MessageResolver`](./message-resolver) can try a lower-ranked language before falling back to the configured default. Never empty, and a provider with nothing to rank returns a single entry.

It has a default implementation returning just `GetLanguage()`, so a custom provider that only knows one language implements `GetLanguage` alone and inherits this.

```csharp
using AlmightyShogun.AspNet.Localization;

public sealed class ArticleService(ILanguageProvider languageProvider)
{
    public string? FindBestTranslation(
        IReadOnlyDictionary<string, string> byLanguage
    ) => languageProvider.GetLanguages()
            .Select(language => byLanguage.GetValueOrDefault(language))
            .FirstOrDefault(translation => translation != null);
}
```

### Type signature

```csharp
IReadOnlyList<string> GetLanguages();
```
