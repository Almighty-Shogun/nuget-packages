using AlmightyShogun.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Provides dependency injection registration for credential authentication services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class AuthCredentialsExtensions
{
    /// <summary>
    /// Registers credential authentication services.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers credential authentication services using the application's database context and user type.
        /// </summary>
        ///
        /// <typeparam name="TDbContext">The application's authentication database context.</typeparam>
        /// <typeparam name="TUser">The application's user entity.</typeparam>
        /// <param name="configuration">The application configuration containing the optional AuthCredentials section.</param>
        /// <param name="registerExceptionHandler">
        /// Whether to register the credential exception handler.
        /// </param>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddAuthCredentials<TDbContext, TUser>(
            IConfiguration configuration,
            bool registerExceptionHandler = true
        ) where TDbContext : AuthDbContext<TUser> where TUser : AuthUser
        {
            serviceCollection
                .AddHttpContextAccessor()
                .AddConfiguration<AuthCredentialsSettings>(configuration.GetSection("AuthCredentials"))
                .AddSingleton<AuthCredentialsExceptionMapper>();

            if (registerExceptionHandler)
                serviceCollection.AddExceptionHandler<AuthCredentialsExceptionHandler>();

            return serviceCollection
                .AddScoped<AuthDbContext<TUser>>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>())
                .AddScoped<AuthLockoutGuard<TUser>>()
                .AddScoped<IAuthUserService<TUser>, AuthUserService<TUser>>()
                .AddScoped<IAuthSessionService<TUser>, AuthSessionService<TUser>>()
                .AddScoped<IAuthPasswordService, AuthPasswordService<TUser>>()
                .AddScoped<IAuthTwoFactorService<TUser>, AuthTwoFactorService<TUser>>()
                .AddScoped<IAuthEmailService, AuthEmailService<TUser>>();
        }
    }
}
