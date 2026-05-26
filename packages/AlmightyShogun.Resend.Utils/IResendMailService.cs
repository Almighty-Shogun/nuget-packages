namespace AlmightyShogun.Resend.Utils;

public interface IResendMailService
{
    /// <summary>
    /// Sends an email using the specified mail template instance.
    /// </summary>
    ///
    /// <param name="recipientEmail">The email address of the recipient.</param>
    /// <param name="mail">The mail template instance to render and send.</param>
    ///
    /// <returns><c>true</c> when the email was sent successfully; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<bool> SendAsync(string recipientEmail, BaseMailTemplate mail);
}
