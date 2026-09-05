namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the addressing and delivery choices for one message.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailOptions
{
    /// <summary>
    /// The primary recipients. Nothing here checks that an entry is a well-formed address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>
    /// The recipients visible to everyone else on the message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> Cc { get; init; } = [];

    /// <summary>
    /// The recipients hidden from the others. Nothing here caps how many there are, so any limit on a large blind list
    /// is Resend's own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> Bcc { get; init; } = [];

    /// <summary>
    /// The addresses a reply is directed to instead of the configured sender, for sending from an address nobody
    /// monitors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<string> ReplyTo { get; init; } = [];

    /// <summary>
    /// The files delivered with the message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public IReadOnlyList<MailAttachment> Attachments { get; init; } = [];

    /// <summary>
    /// The idempotency key sent with the request. What Resend makes of a repeated key is the provider's own behavior,
    /// not something this package enforces.
    /// </summary>
    ///
    /// <remarks>
    /// Set it explicitly when the caller can itself be retried, for example a background job, so every one of those retries
    /// sends the same key.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? IdempotencyKey { get; init; }
}
