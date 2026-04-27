namespace AlmightyShogun.Resend.Utils.Templates;

public interface IEmailTemplateLoader
{
    /// <summary>
    /// Loads an email template by file name.
    /// </summary>
    ///
    /// <param name="templateName">The file name of the template to load.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    ///
    /// <returns>The loaded template content.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default);
}
