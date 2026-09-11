namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Sends an email to a single recipient.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public interface IResendMailService
{
    /// <summary>
    /// Sends an email to a single recipient.
    /// </summary>
    ///
    /// <param name="recipientEmail">The recipient email address.</param>
    /// <param name="mail">The mail template to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The result of the send operation.</returns>
    ///
    /// <exception cref="IOException">
    /// A template could not be read.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A template could not be accessed.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// The operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<MailSendResult> SendAsync(string recipientEmail, BaseMailTemplate mail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email using the specified options.
    /// </summary>
    ///
    /// <param name="mail">The mail template to send.</param>
    /// <param name="options">The mail options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The result of the send operation.</returns>
    ///
    /// <exception cref="IOException">
    /// A template could not be read.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// A template could not be accessed.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// The operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<MailSendResult> SendAsync(BaseMailTemplate mail, MailOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders an email without sending it.
    /// </summary>
    ///
    /// <param name="mail">The mail template to render.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The rendered email preview.</returns>
    ///
    /// <exception cref="IOException">
    /// A template could not be read.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    ///  A template could not be accessed.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// The operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    Task<MailPreview> PreviewAsync(BaseMailTemplate mail, CancellationToken cancellationToken = default);
}
