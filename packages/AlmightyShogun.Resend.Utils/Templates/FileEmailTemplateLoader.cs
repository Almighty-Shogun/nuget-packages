namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Loads shared mail template files from the application's <c>mail</c> output directory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
internal sealed class FileEmailTemplateLoader : IEmailTemplateLoader
{
    /// <summary>
    /// Stores the directory where shared mail template files are expected at runtime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    private static readonly string TemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "mail");

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    public async Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default)
    {
        string templatePath = Path.Combine(TemplatesDirectory, templateName);

        return await File.ReadAllTextAsync(templatePath, cancellationToken);
    }
}
