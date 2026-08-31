namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the addressing and delivery choices for one message.
/// </summary>
///
/// <remarks>
/// An options object rather than a growing parameter list, so adding another addressing concern later does not change
/// the method signature again.
/// </remarks>
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
    /// Gets the recipients hidden from the others. They still count toward whatever recipient limit the Resend account
    /// enforces, which is what a large blind list runs into first.
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
    /// Gets the idempotency key. When set, Resend treats a repeated send with the same key as the same message, so a
    /// retried request cannot deliver twice.
    /// </summary>
    ///
    /// <remarks>
    /// Leave it unset to have one generated per send. Set it explicitly when the caller can itself be retried, for
    /// example a background job, so the whole operation is idempotent rather than only the HTTP call.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? IdempotencyKey { get; init; }
}
