namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the outcome of a send, reporting a provider failure as a value rather than an exception.
/// </summary>
///
/// <remarks>
/// Only <see cref="Success"/> and <see cref="Failure"/> construct one, both of which are internal, so the two states cannot
/// be mixed into a combination the package never produces, such as a success carrying an error. A caller reads the result
/// and never builds one.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailSendResult
{
    /// <summary>
    /// Initializes the outcome. Private so the two factories stay the only way to produce one, which is what keeps the
    /// combinations of the three values to the two the package actually returns.
    /// </summary>
    ///
    /// <param name="isSuccess">Whether Resend accepted the message.</param>
    /// <param name="messageId">The Resend id, or <c>null</c> when the send failed.</param>
    /// <param name="error">The failure message, or <c>null</c> when the send succeeded.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private MailSendResult(bool isSuccess, string? messageId, string? error)
    {
        IsSuccess = isSuccess;
        MessageId = messageId;
        Error = error;
    }

    /// <summary>
    /// Gets whether the send succeeded. A <c>true</c> means the request to Resend returned without an error, and nothing
    /// here follows the message any further than that. A <c>false</c> does not always mean Resend declined it: a send with
    /// no recipient fails before the provider is contacted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the Resend id, for correlating with a webhook or the dashboard. Always <c>null</c> when the send failed, and
    /// always present when it succeeded, since the only success path formats the identifier Resend returns.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? MessageId { get; }

    /// <summary>
    /// Gets the failure message, and <c>null</c> whenever the send succeeded. It carries either this package's own
    /// rejection, such as a send with no recipient, or the message of the exception the send request threw, which may come
    /// from the client library rather than from Resend. It is for logs and diagnostics rather than for showing to a user.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string? Error { get; }

    /// <summary>
    /// Creates the accepted outcome.
    /// </summary>
    ///
    /// <param name="messageId">The id Resend returned for the accepted message.</param>
    ///
    /// <returns>A result whose <see cref="IsSuccess"/> is <c>true</c> and whose <see cref="Error"/> is <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static MailSendResult Success(string? messageId) => new(true, messageId, null);

    /// <summary>
    /// Creates the rejected outcome.
    /// </summary>
    ///
    /// <param name="error">The reason surfaced to whoever inspects the result.</param>
    ///
    /// <returns>
    /// A result whose <see cref="IsSuccess"/> is <c>false</c> and whose <see cref="MessageId"/> is <c>null</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static MailSendResult Failure(string error) => new(false, null, error);
}
