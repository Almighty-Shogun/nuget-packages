namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents options for sending an email.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailOptions
{
    /// <summary>
    /// Gets the primary recipients.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>
    /// Gets the carbon copy recipients.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> Cc { get; init; } = [];

    /// <summary>
    /// Gets the blind carbon copy recipients.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> Bcc { get; init; } = [];

    /// <summary>
    /// Gets the reply-to addresses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> ReplyTo { get; init; } = [];

    /// <summary>
    /// Gets the attachments.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<MailAttachment> Attachments { get; init; } = [];

    /// <summary>
    /// Gets the idempotency key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? IdempotencyKey { get; init; }
}
