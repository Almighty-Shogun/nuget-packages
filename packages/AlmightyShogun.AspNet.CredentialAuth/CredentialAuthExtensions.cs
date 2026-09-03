using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Registers the credential authentication services against an application's own database context and user type.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class CredentialAuthExtensions
{
    /// <summary>
    /// Provides the registration helper on the service collection, which is where every registration this package makes
    /// lands.
    /// </summary>
    ///
    /// <param name="serviceCollection">
    /// The collection the services are registered into. Returned so the call chains with the rest of an application's
    /// startup.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers every credential service against the application's own context and user entity, so auth data lives in the
        /// application's database rather than a separate one. Call it after the JWT auth registration it builds on.
        /// </summary>
        ///
        /// <typeparam name="TDbContext">The application's own context, resolved per request and handed to every service.</typeparam>
        /// <typeparam name="TUser">The user entity the services read and write, so an application keeps its own columns.</typeparam>
        /// <param name="configuration">
        /// The application configuration. Read for a <c>CredentialAuth</c> section, which is optional: every value in it
        /// has a default, so an absent section leaves lockout off and the reset and two-factor policies as they ship.
        /// </param>
        /// <param name="registerExceptionHandler">
        /// Whether to register the handler that turns this package's exceptions into standardized responses. It needs
        /// <c>AddHttpErrorResponseWriter</c> and <c>AddMessageLocalization</c> from <c>AlmightyShogun.AspNet.Core</c>. The
        /// mapper is registered either way, so a replacement handler can still resolve it.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the credential authentication services registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddCredentialAuth<TDbContext, TUser>(
            IConfiguration configuration,
            bool registerExceptionHandler = true
        ) where TDbContext : AuthDbContext<TUser> where TUser : AuthUser
        {
            serviceCollection
                .AddHttpContextAccessor()
                .AddConfiguration<CredentialAuthSettings>(configuration.GetSection("CredentialAuth"))
                .AddSingleton<CredentialAuthExceptionMapper>();

            if (registerExceptionHandler)
                serviceCollection.AddExceptionHandler<CredentialAuthExceptionHandler>();

            return serviceCollection
                .AddScoped<AuthDbContext<TUser>>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>())
                .AddScoped<IAuthUserService<TUser>, AuthUserService<TUser>>()
                .AddScoped<IAuthSessionService<TUser>, AuthSessionService<TUser>>()
                .AddScoped<IAuthPasswordService, AuthPasswordService<TUser>>()
                .AddScoped<IAuthTwoFactorService<TUser>, AuthTwoFactorService<TUser>>();
        }
    }
}
