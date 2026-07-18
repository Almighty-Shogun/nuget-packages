using Resend;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Resend.Utils;

/// <summary>
/// Registers Resend client, mail template loading, and mail sending services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.5.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the Resend functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Resend email services and binds the required <c>Email</c> configuration.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that contains the required <c>Email</c> section.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the Resend client, template loader, and mail service registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.5.0</since>
        public IServiceCollection AddResendEmail(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Email").Get<EmailSettings>();

            if (settings is null)
                throw new InvalidOperationException("Missing Email configuration");

            serviceCollection.AddOptions()
                .AddConfiguration<EmailSettings>(configuration.GetSection("Email"))
                .AddHttpClient<ResendClient>();

            return serviceCollection.Configure<ResendClientOptions>(options => options.ApiToken = settings.ApiToken)
                .AddTransient<IResend, ResendClient>()
                .AddSingleton<IEmailTemplateLoader, FileEmailTemplateLoader>()
                .AddTransient<IResendMailService, ResendMailService>();
        }
    }
}
