---
fields:
    - name: LocalizationSettings
      description: The optional `Localization` section, bound by [`AddMessageLocalization`](./extensions/add-message-localization). Every value has a default, so an application can register the services without adding the section at all.
      fields:
          - name: DefaultLanguage
            description: Language used when the request does not ask for one, and the final fallback when a key is missing from the requested language. Validated at startup against the same shape a message directory must have, so a malformed value fails the host. Point it at a language that actually has message files, since it ends the fallback chain and a key missing there reaches the client verbatim.
            type: string
            default: en

          - name: AutomaticReload
            description: Watches the message directories and clears the cache when a file changes, so edits apply without a restart.
            type: bool
            default: 'false'
---

# Configuration

The `Localization` section decides which language a message resolves in and whether message files are watched for changes. It is optional, and adding it is only worth doing to change one of the defaults below.

```json
{
    "Localization": {
        "DefaultLanguage": "en",
        "AutomaticReload": false
    }
}
```

::: warning
`AutomaticReload` uses a file watcher, which does not fire reliably on container bind mounts or network filesystems. On those, an edit may go unnoticed until the process restarts.
:::

<FrontmatterDocs/>
