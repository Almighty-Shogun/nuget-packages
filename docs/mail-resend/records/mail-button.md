---
fields:
    - name: Label
      description: Visible text, HTML encoded when rendered, so markup in it is shown rather than applied. Blank is rejected.
      type: string

    - name: Url
      description: Destination the button points to. It must be an absolute `http`, `https`, or `mailto` URL; anything else is rejected.
      type: string
---

# MailButton

A call-to-action button rendered by [`BaseMailTemplate`](../types/base-mail-template), returned from a template's protected [`Buttons`](../types/base-mail-template#buttons) member when a message should carry one or more prominent links. The label becomes the visible text and the URL the destination. Both are encoded into the HTML body and written verbatim into the plain-text one.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailButton
{
    public MailButton(string label, string url);

    public string Label { get; }
    public string Url { get; }
}
```
