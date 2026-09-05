namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the addressing and delivery choices for one message.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailOptions
{
    /// <summary>
    /// Gets the primary recipients. An empty list is refused as a failed result rather than an exception, so it reaches the
    /// caller the same way a provider rejection does.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>
    /// Gets the recipients visible to everyone else on the message. Left empty, nothing is set on the outgoing message, and
    /// how the Resend client represents an unset field on the wire is its own concern.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> Cc { get; init; } = [];

    /// <summary>
    /// Gets the recipients hidden from the others. Nothing here caps how many there are, so any limit on a large blind list
    /// is Resend's own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> Bcc { get; init; } = [];

    /// <summary>
    /// Gets the addresses a reply is directed to instead of the configured sender, for sending from an address nobody
    /// monitors.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<string> ReplyTo { get; init; } = [];

    /// <summary>
    /// Gets the files delivered with the message. Each one is held in memory for the duration of the send.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public IReadOnlyList<MailAttachment> Attachments { get; init; } = [];

    /// <summary>
    /// Gets the idempotency key sent with the request. What Resend makes of a repeated key is the provider's own behavior,
    /// not something this package enforces.
    /// </summary>
    ///
    /// <remarks>
    /// Leave it unset to have one generated per send. Set it explicitly when the caller can itself be retried, for
    /// example a background job, so every one of those retries sends the same key.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? IdempotencyKey { get; init; }
}
