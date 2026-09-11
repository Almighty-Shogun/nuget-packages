namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents an email attachment.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailAttachment
{
    /// <summary>
    /// Gets the attachment file name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the attachment content.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required byte[] Content { get; init; }

    /// <summary>
    /// Gets the MIME content type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? ContentType { get; init; }
}
