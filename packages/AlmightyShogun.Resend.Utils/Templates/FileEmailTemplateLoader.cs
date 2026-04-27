namespace AlmightyShogun.Resend.Utils.Templates;

public sealed class FileEmailTemplateLoader : IEmailTemplateLoader
{
    private static readonly string TemplatesDirectory = Path.Combine(AppContext.BaseDirectory, "mail");

    /// <inheritdoc/>
    public async Task<string> LoadAsync(string templateName, CancellationToken cancellationToken = default)
    {
        string templatePath = Path.Combine(TemplatesDirectory, templateName);
        
        return await File.ReadAllTextAsync(templatePath, cancellationToken);
    }
}
