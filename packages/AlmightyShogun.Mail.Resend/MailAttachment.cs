namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a file delivered alongside a message body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailAttachment
{
    /// <summary>
    /// The name carried with the attachment. Nothing here constrains or rewrites it, so what a recipient's client makes
    /// of it is that client's business.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string FileName { get; init; }

    /// <summary>
    /// The raw bytes. Nothing here imposes a size limit of its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required byte[] Content { get; init; }

    /// <summary>
    /// The MIME type, or <c>null</c> to leave it unstated.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? ContentType { get; init; }
}
