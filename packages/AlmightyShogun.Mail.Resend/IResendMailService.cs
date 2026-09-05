namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Renders mail templates and sends them through Resend. Registered as transient by <c>AddResendEmail</c>, so every
/// resolution gets its own instance.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public interface IResendMailService
{
    /// <summary>
    /// Sends a mail template to one recipient, for the common case that needs no copies, attachments, or reply-to address.
    /// </summary>
    ///
    /// <param name="recipientEmail">
    /// The sole recipient, passed through unvalidated. A malformed address raises no argument exception here, so whatever
    /// becomes of it is settled by the send request.
    /// </param>
    /// <param name="mail">The template rendered into both bodies, and whose subject becomes the message subject.</param>
    /// <param name="cancellationToken">The token cancelling the template reads and the send request.</param>
    ///
    /// <returns>The outcome, carrying the Resend message id when the send succeeded.</returns>
    ///
    /// <exception cref="IOException">
    /// A shared template could not be read. <c>AddResendEmail</c> checks for them while registering, so this means one was
    /// removed or locked between then and now.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not read one of the shared templates. This does not derive from <see cref="IOException"/>, so a
    /// caller guarding only against that does not catch it.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> was signaled. Cancellation propagates rather than arriving as a failed result,
    /// so a caller shutting down is never told the send was rejected.
    /// </exception>
    ///
    /// <remarks>
    /// Every call generates its own idempotency key. Reach for the other overload when the caller can itself be retried and
    /// the key has to be stable across those retries.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<MailSendResult> SendAsync(string recipientEmail, BaseMailTemplate mail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a mail template with full addressing.
    /// </summary>
    ///
    /// <param name="mail">The template rendered into both bodies, and whose subject becomes the message subject.</param>
    /// <param name="options">
    /// Everything about the message other than its content. Empty recipients come back as a failed result rather than an
    /// exception.
    /// </param>
    /// <param name="cancellationToken">The token cancelling the template reads and the send request.</param>
    ///
    /// <returns>The outcome, carrying the Resend message id when the send succeeded.</returns>
    ///
    /// <exception cref="IOException">
    /// A shared template could not be read. <c>AddResendEmail</c> checks for them while registering, so this means one was
    /// removed or locked between then and now.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not read one of the shared templates. This does not derive from <see cref="IOException"/>, so a
    /// caller guarding only against that does not catch it.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> was signaled. Cancellation propagates rather than arriving as a failed result,
    /// so a caller shutting down is never told the send was rejected.
    /// </exception>
    ///
    /// <remarks>
    /// Rendering happens before the request and only the request is guarded, so a provider or transport failure arrives as a
    /// failed result while a template failure throws. Inspecting the result alone is not enough to catch both.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<MailSendResult> SendAsync(BaseMailTemplate mail, MailOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a mail template without sending it, for previewing what a template produces or asserting on it in a test.
    /// </summary>
    ///
    /// <param name="mail">The template to render. Its subject is not part of the result.</param>
    /// <param name="cancellationToken">The token cancelling the template reads.</param>
    ///
    /// <returns>The rendered HTML body and its plain-text alternative.</returns>
    ///
    /// <exception cref="IOException">
    /// A shared template could not be read. <c>AddResendEmail</c> checks for them while registering, so this means one was
    /// removed or locked between then and now.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The process may not read one of the shared templates. This does not derive from <see cref="IOException"/>, so a
    /// caller guarding only against that does not catch it.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled during a read.</exception>
    ///
    /// <remarks>
    /// Nothing is sent and Resend is never contacted, so the configured API token is not used at all on this path.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task<MailPreview> PreviewAsync(BaseMailTemplate mail, CancellationToken cancellationToken = default);
}
