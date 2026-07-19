using System.Text;
using AlmightyShogun.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AlmightyShogun.AspNet.JwtAuth;

/// <summary>
/// Registers the services and authentication handlers provided by ASP.NET JWT Auth.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class PackageRegistry
{
    /// <summary>
    /// Provides service-collection extension methods for registering JWT authentication and authorization services.
    /// </summary>
    ///
    /// <param name="serviceCollection">The service collection used to register the JWT Auth services.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the JWT auth package services using the provided configuration root.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that contains the required <c>Auth</c> section.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with authentication configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddJwtAuth(IConfiguration configuration)
        {
            serviceCollection
                .AddConfiguration<AuthSettings>(configuration.GetSection("Auth"))
                .AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(configuration);

            return serviceCollection
                .AddSingleton<IAppHostResolver, AppHostResolver>()
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .RegisterOnInherit<IAuthorizationHandler>(ServiceLifetime.Scoped);
        }
    }

    /// <summary>
    /// Registers JWT bearer authentication configured from the <c>Auth</c> configuration section.
    /// </summary>
    ///
    /// <param name="builder">The authentication builder used to register the JWT functionality.</param>
    /// <param name="configuration">The application configuration that contains issuer, secret, lifetime, and host mapping settings.</param>
    ///
    /// <returns>The <see cref="AuthenticationBuilder"/> instance with JWT bearer configured.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        AuthSettings? authSettings = configuration.GetSection("Auth").Get<AuthSettings>();

        if (authSettings is null)
            throw new InvalidOperationException("Missing Auth configuration");

        return builder.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authSettings.Issuer,
            ValidAudiences = authSettings.ValidAudiences,
            ValidateAudience = authSettings.IsScoped(),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret))
        });
    }
}
