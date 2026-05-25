using Resend;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.Resend.Utils;

public static class PackageRegistry
{
    /// <param name="serviceCollection">The service collection used to register the Resend functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers Resend services with the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// 
        /// <param name="configuration">The<see cref="IConfiguration"/> to be provided if using Configuration</param>
        /// 
        /// <returns>The <see cref="IServiceCollection"/> instance with the remote commands handlers registered.</returns>
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
                .AddTransient<IMailService, ResendMailService>();
        }
    }
}
