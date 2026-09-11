using Resend;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Mail.Resend;

/// <summary>
/// Provides dependency injection extensions for Resend email services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public static class ResendMailExtensions
{
    private static readonly string[] _requiredTemplates =
    [
        "BaseEmailTemplate.html",
        "BaseEmailParagraph.html",
        "BaseEmailButton.html"
    ];

    /// <summary>
    /// Provides the registration helper on service collections.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.5.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the Resend email services and binds the <c>Email</c> configuration section.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration.</param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <exception cref="InvalidOperationException">
        /// The mail template directory or one of the required templates is missing.
        /// </exception>
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
    /// Ensures the required mail templates are present.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">The mail template directory or one of the required templates is missing.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void EnsureTemplatesExist()
    {
        string directory = FileEmailTemplateLoader.TemplatesDirectory;

        if (!Directory.Exists(directory))
            throw new InvalidOperationException(
                $"The mail template directory '{directory}' does not exist. Create it and add {string.Join(", ", _requiredTemplates)}."
            );

        string[] missing = [.. _requiredTemplates.Where(template => !File.Exists(Path.Combine(directory, template)))];

        if (missing.Length > 0)
            throw new InvalidOperationException(
                $"The mail template directory '{directory}' is missing {string.Join(", ", missing)}."
            );
    }
}
