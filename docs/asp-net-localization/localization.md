# Localization

Messages are stored as language-specific JSON files under `messages/{language}`. A language directory can contain one file or several files; when that language is requested, the resolver loads every `.json` file in that directory and flattens them into message keys. Files are read once and cached for the life of the process unless `AutomaticReload` is on, and a language with no directory is cached the same way, so a header naming languages the deployment does not have costs one lookup rather than a filesystem walk per request.

## Language negotiation

`Accept-Language` is a ranked list, and all of it is honoured. Every accepted language is tried in the client's preference order, each one immediately followed by its own progressively shorter forms, with `DefaultLanguage` from the [`Localization`](./configuration) section last. The first candidate that has any messages at all wins, and every key for that request is then resolved from it alone.

For `Accept-Language: nl-BE,fr;q=0.9` with `DefaultLanguage` of `en`, the resolver tries:

```text
nl-BE  ->  nl  ->  fr  ->  en
```

So an application with `messages/nl/` but no `messages/nl-BE/` serves Dutch, and an application with neither still serves French before falling back to English, because the client said it would accept French.

A tag is shortened one subtag at a time rather than cut straight back to its primary subtag, so `Accept-Language: zh-Hant-TW` tries:

```text
zh-Hant-TW  ->  zh-Hant  ->  zh  ->  en
```

That middle step is what lets a deployment keep `messages/zh-Hant/` and `messages/zh-Hans/` side by side: skipping it would serve whichever script `messages/zh/` happens to hold, or none at all.

A shorter form sits directly behind its own tag rather than after every accepted language, because `nl` is a closer match for a client asking for `nl-BE` than an `fr` the client ranked lower. Quality values decide the order between different languages: `nl-BE;q=0.2,fr;q=0.9` tries `fr` first.

Candidates appear once each, so a header that repeats a language, or names both `nl` and `nl-BE`, does not read the same directory twice.

A key the winning language does not define is returned as itself, so a caller still receives a stable value rather than an empty string, and a warning is logged naming both the key and the language. It is not looked up in the next candidate, so a half-translated language directory serves its own gaps rather than borrowing from the one behind it.

::: warning
An entry that is not a well-formed language tag is dropped, as is the `*` wildcard, which names no directory. Each value is combined into a filesystem path when message files are resolved, so anything containing a path separator or `..` is refused rather than followed.
:::
