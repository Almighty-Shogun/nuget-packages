---
fields:
    - name: FileName
      description: Name the recipient sees. Resend uses it to infer a content type when none is given.
      type: string
    - name: Content
      description: The file bytes. They are held in memory for the whole send, which is what bounds a workable attachment size rather than a limit the package imposes.
      type: 'byte[]'
    - name: ContentType
      description: MIME type. Leave it null to let Resend infer one from the file name.
      type: string?
      default: 'null'
---

# MailAttachment

One file attached to a send, supplied through [`MailOptions.Attachments`](./mail-options).

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailAttachment
{
    public required string FileName { get; init; }
    public required byte[] Content { get; init; }
    public string? ContentType { get; init; }
}
```
