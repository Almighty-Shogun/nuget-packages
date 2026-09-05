namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents a file delivered alongside a message body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailAttachment
{
    /// <summary>
    /// Gets the name carried with the attachment. It is handed to the Resend client verbatim, with no path handling applied here, so
    /// what a recipient's client makes of it is that client's business.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the raw bytes, held in memory for the whole send. The package imposes no size limit of its own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required byte[] Content { get; init; }

    /// <summary>
    /// Gets the MIME type. Leave it unset to send none and leave the type to Resend.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? ContentType { get; init; }
}
