namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Represents the result of sending an email.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record MailSendResult
{
    /// <summary>
    /// Gets whether the send succeeded.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the Resend message identifier when the send succeeds; otherwise, <c>null</c>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? MessageId { get; }

    /// <summary>
    /// Gets the failure message when the send fails; otherwise, <c>null</c>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MailSendResult"/> record.
    /// </summary>
    ///
    /// <param name="isSuccess">Whether the email was sent successfully.</param>
    /// <param name="messageId">The Resend message identifier when the send succeeds; otherwise, <c>null</c>.</param>
    /// <param name="error">The failure message when the send fails; otherwise, <c>null</c>.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private MailSendResult(bool isSuccess, string? messageId, string? error)
    {
        IsSuccess = isSuccess;
        MessageId = messageId;
        Error = error;
    }

    /// <summary>
    /// Creates a successful send result.
    /// </summary>
    ///
    /// <param name="messageId">The Resend message identifier.</param>
    ///
    /// <returns>A successful send result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static MailSendResult Success(string? messageId) => new(true, messageId, null);

    /// <summary>
    /// Creates a failed send result.
    /// </summary>
    ///
    /// <param name="error">The failure message.</param>
    ///
    /// <returns>
    /// A failed send result.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static MailSendResult Failure(string error) => new(false, null, error);
}
