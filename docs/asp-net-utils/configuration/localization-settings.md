# LocalizationSettings

Controls how localized HTTP messages are resolved. Bound from the optional `Localization` section by [`AddMessageLocalization`](../extensions/add-message-localization). Set `DefaultLanguage` to a language that actually has message files, since it ends the fallback chain and a key missing there reaches the client verbatim.

::: warning
`AutomaticReload` uses a file watcher, which does not fire reliably on container bind mounts or network filesystems. On those, an edit may go unnoticed until the process restarts.
:::

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;
using Microsoft.Extensions.Options;

public sealed class LanguageReporter(
    IOptions<LocalizationSettings> localizationOptions
)
{
    public string GetFallbackLanguage()
        => localizationOptions.Value.DefaultLanguage;
}
```
