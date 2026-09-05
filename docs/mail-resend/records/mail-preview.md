---
fields:
    - name: Html
      description: The rendered HTML body, exactly as it would be sent.
      type: string
    - name: Text
      description: The rendered plain-text body, for clients that do not display HTML.
      type: string
---

# MailPreview

Both rendered bodies for a template, returned by [`PreviewAsync`](../services/resend-mail-service#previewasync) without sending anything.

Writing them to a file is left to the caller, so the package takes on no path validation or overwrite semantics.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailPreview
{
    public required string Html { get; init; }
    public required string Text { get; init; }
}
```
