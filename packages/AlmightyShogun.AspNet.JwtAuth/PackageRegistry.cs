using System.Text;
using AlmightyShogun.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
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
    /// <param name="serviceCollection">
    /// The collection every service this package registers lands in. Returned so the call chains with the rest of an
    /// application's startup.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers bearer authentication, the permission policy provider, and the app-audience requirement, binding the
        /// <c>Auth</c> section they all read from and forcing its audience list to be built while the host starts.
        /// </summary>
        ///
        /// <param name="configuration">
        /// The application configuration. Read for an <c>Auth</c> section, which is required: without a signing secret
        /// there is nothing to validate a token against.
        /// </param>
        /// <param name="registerExceptionHandler">
        /// Whether to register the handler that turns this package's exceptions into standardized responses. It needs
        /// <c>AddHttpErrorResponseWriter</c> and <c>AddMessageLocalization</c> from <c>AlmightyShogun.AspNet.Core</c>, and
        /// runs ahead of whatever <c>AddExceptionHandling</c> registers. The mapper is registered either way, so a
        /// replacement handler can still resolve it.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with JWT authentication and authorization registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection AddJwtAuth(IConfiguration configuration, bool registerExceptionHandler = true)
        {
            serviceCollection
                .AddConfiguration<AuthSettings>(configuration.GetSection("Auth"))
                .AddSingleton<JwtAuthExceptionMapper>()
                .AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => ConfigureJwtBearer(options, configuration));

            serviceCollection.AddOptions<AuthSettings>().Validate(
                settings => settings.ValidAudiences.Count > 0,
                "Auth resolved no valid audience. Configure Auth:DefaultApp or at least one Auth:Hosts entry."
            );

            if (registerExceptionHandler)
                serviceCollection.AddExceptionHandler<JwtAuthExceptionHandler>();

            return serviceCollection
                .AddSingleton<IAppHostResolver, AppHostResolver>()
                .AddSingleton<IAuthTokenGenerator, AuthTokenGenerator>()
                .ReplaceService<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                .AddScoped<IAuthorizationHandler, AppAudienceAuthorizationHandler>();
        }
    }

    /// <summary>
    /// Applies the bound settings to the bearer options, so issuer, signing key, audience, and lifetime are configured
    /// from one place rather than independently.
    /// </summary>
    ///
    /// <param name="options">The bearer options being built for the authentication scheme.</param>
    /// <param name="configuration">The configuration the <c>Auth</c> section is read from.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void ConfigureJwtBearer(JwtBearerOptions options, IConfiguration configuration)
    {
        AuthSettings authSettings = configuration.GetSection("Auth").Get<AuthSettings>()
                                    ?? throw new InvalidOperationException("Missing Auth configuration");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(authSettings.ClockSkewSeconds),
            ValidIssuer = authSettings.Issuer,
            ValidAudiences = authSettings.ValidAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret))
        };
    }
}
