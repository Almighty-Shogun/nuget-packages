using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Registers the services provided by ASP.NET Credential Auth.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides service-collection extension methods for registering credential authentication services.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection used to register the credential authentication services.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers credential authentication services for the supplied authentication database context and user type.
        /// </summary>
        ///
        /// <typeparam name="TDbContext">The authentication database context type.</typeparam>
        /// <typeparam name="TUser">The authentication user entity type.</typeparam>
        ///
        /// <returns>The service collection.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddCredentialAuth<TDbContext, TUser>()
            where TDbContext : AuthDbContext<TUser>
            where TUser : AuthUser
        {
            serviceCollection
                .AddHttpContextAccessor()
                .AddScoped<AuthDbContext<TUser>>(serviceProvider => serviceProvider.GetRequiredService<TDbContext>())
                .AddScoped<IAuthValidationService, AuthValidationService<TUser>>()
                .AddScoped<AuthService<TUser>>()
                .AddScoped<CurrentPasswordRule>()
                .AddScoped<LoginIdentifierExistsRule>()
                .AddScoped<NotCurrentPasswordRule>()
                .AddScoped<PasswordMatchRule>()
                .AddScoped<PasswordResetTokenRule>()
                .AddScoped<UniqueEmailRule>()
                .AddScoped<UniqueUsernameRule>()
                .AddScoped<IAuthTokenService<TUser>>(serviceProvider => serviceProvider.GetRequiredService<AuthService<TUser>>())
                .AddScoped<IAuthUserService<TUser>>(serviceProvider => serviceProvider.GetRequiredService<AuthService<TUser>>())
                .AddScoped<IAuthSessionService<TUser>>(serviceProvider => serviceProvider.GetRequiredService<AuthService<TUser>>())
                .AddScoped<IAuthPasswordService>(serviceProvider => serviceProvider.GetRequiredService<AuthService<TUser>>());

            return serviceCollection;
        }
    }
}
