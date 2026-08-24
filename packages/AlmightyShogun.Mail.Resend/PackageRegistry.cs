using Resend;
using AlmightyShogun.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Registers the Resend client, the shared template loader, and the mail service as one unit, so an application binds the
/// <c>Email</c> section once instead of wiring the three separately.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public static class PackageRegistry
{
    /// <summary>
    /// The template files every rendered message is assembled from. The package ships none of them, so this list is what the
    /// startup check reports when an application has not supplied them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly string[] RequiredTemplates =
    [
        "BaseEmailTemplate.html",
        "BaseEmailParagraph.html",
        "BaseEmailButton.html"
    ];

    /// <param name="serviceCollection">The service collection the Resend client, template loader, and mail service are added to.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the Resend email services and binds the <c>Email</c> configuration section.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The configuration carrying the <c>Email</c> section. An absent section binds successfully and fails validation
        /// instead, which is what reports the missing token rather than a binding error.
        /// </param>
        ///
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with the Resend client, template loader, and mail service registered.
        /// </returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// The <c>mail</c> directory or one of its templates is missing. This is thrown while registering rather than while
        /// the host starts, so a test that only builds a service collection hits it too.
        /// </exception>
        ///
        /// <remarks>
        /// The mail service is transient and the template loader a singleton, so the file cache is shared while nothing holds
        /// a settings snapshot longer than one message. The Resend client goes through a typed <c>HttpClient</c> with the
        /// standard resilience handler, meaning a transient provider failure is retried before it reaches a caller.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.5.0</since>
        public IServiceCollection AddResendEmail(IConfiguration configuration)
        {
            EnsureTemplatesExist();

            serviceCollection
                .AddConfiguration<EmailSettings>(configuration.GetSection("Email"))
                .AddOptions<ResendClientOptions>()
                .Configure<IOptions<EmailSettings>>((options, email) => options.ApiToken = email.Value.ApiToken);

            serviceCollection.AddHttpClient<IResend, ResendClient>()
                .AddStandardResilienceHandler();

            return serviceCollection
                .AddSingleton<IEmailTemplateLoader, FileEmailTemplateLoader>()
                .AddTransient<IResendMailService, ResendMailService>();
        }
    }

    /// <summary>
    /// Fails while registering when the shared templates are missing, rather than on the first send.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">The directory or a template file is missing.</exception>
    ///
    /// <remarks>
    /// The package ships no templates, so providing them is the application's job. Checking here turns a confusing
    /// <see cref="DirectoryNotFoundException"/> from the first email into a registration failure naming what to add.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void EnsureTemplatesExist()
    {
        string directory = FileEmailTemplateLoader.TemplatesDirectory;

        if (!Directory.Exists(directory))
            throw new InvalidOperationException(
                $"The mail template directory '{directory}' does not exist. Create it and add {string.Join(", ", RequiredTemplates)}."
            );

        string[] missing = [.. RequiredTemplates.Where(template => !File.Exists(Path.Combine(directory, template)))];

        if (missing.Length > 0)
            throw new InvalidOperationException(
                $"The mail template directory '{directory}' is missing {string.Join(", ", missing)}."
            );
    }
}
