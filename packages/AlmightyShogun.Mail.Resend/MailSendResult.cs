namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the outcome of a send, reporting a provider failure as a value rather than an exception so a caller sending
/// in a loop does not have to wrap each message.
/// </summary>
///
/// <param name="IsSuccess">
/// Whether Resend accepted the message. Acceptance is not delivery, which is reported later by a webhook.
/// </param>
/// <param name="MessageId">
/// The Resend id, for correlating with a webhook or the dashboard, and <c>null</c> whenever the send failed.
/// </param>
/// <param name="Error">
/// The provider or transport failure message, and <c>null</c> whenever the send succeeded. It is not localized.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record MailSendResult(bool IsSuccess, string? MessageId, string? Error)
{
    /// <summary>
    /// Creates the accepted outcome.
    /// </summary>
    ///
    /// <param name="messageId">The id Resend returned, which is <c>null</c> when the response carried none.</param>
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
