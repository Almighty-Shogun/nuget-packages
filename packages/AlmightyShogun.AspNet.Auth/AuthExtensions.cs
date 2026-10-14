
using AlmightyShogun.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AlmightyShogun.AspNet.Auth;

/// <summary>
/// Registers JWT authentication and authorization services.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>2.3.0</since>
public static class AuthExtensions
{
    /// <summary>
    /// Provides authentication service registration extensions.
    /// </summary>
    /// <author>Almighty-Shogun</author>
    /// <since>2.3.0</since>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers JWT authentication, authorization, and related services.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="registerExceptionHandler">Whether to register the authentication exception handler.</param>
        /// <returns>The service collection.</returns>
        /// <author>Almighty-Shogun</author>
        /// <since>4.0.0</since>
        public IServiceCollection AddAuth(IConfiguration configuration, bool registerExceptionHandler = true)
        {
            serviceCollection
                .AddConfiguration<AuthSettings>(configuration.GetSection("Auth"))
                .AddSingleton<JwtAuthExceptionMapper>()
                .AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            serviceCollection.AddOptions<AuthSettings>().Validate(
                settings => settings.ValidAudiences.Count > 0,
                "Auth resolved no valid audience. Configure Auth:DefaultApp or at least one Auth:Hosts entry."
            );

            serviceCollection
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<AuthSettings>>(ConfigureJwtBearer);

            if (registerExceptionHandler)
                serviceCollection.AddExceptionHandler<JwtAuthExceptionHandler>();

            return serviceCollection
                .AddSingleton<IAppHostResolver, AppHostResolver>()
                .AddSingleton<IAuthTokenGenerator, AuthTokenGenerator>()
                .ReplaceService<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }

    /// <summary>
    /// Configures JWT bearer token validation using authentication settings.
    /// </summary>
    /// <param name="options">The JWT bearer options.</param>
    /// <param name="authOptions">The authentication settings.</param>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void ConfigureJwtBearer(JwtBearerOptions options, IOptions<AuthSettings> authOptions)
    {
        AuthSettings authSettings = authOptions.Value;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromSeconds(authSettings.ClockSkewSeconds),
            ValidIssuer = authSettings.Issuer,
            ValidAudiences = authSettings.ValidAudiences,
            IssuerSigningKey = AuthSigningKey.Create(authSettings.Secret)
        };

        options.Events = new JwtBearerEvents { OnTokenValidated = ValidateAppAudience };
    }

    /// <summary>
    /// Validates that the token audience matches the application resolved from the request host.
    /// </summary>
    /// <param name="context">The validated token context.</param>
    /// <returns>A completed task.</returns>
    /// <remarks>
    /// Skips application-specific validation when host scoping is disabled.
    /// </remarks>
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static Task ValidateAppAudience(TokenValidatedContext context)
    {
        if (!context.HttpContext.RequestServices.GetRequiredService<IOptions<AuthSettings>>().Value.IsScoped())
            return Task.CompletedTask;

        var appHostResolver = context.HttpContext.RequestServices.GetRequiredService<IAppHostResolver>();

        if (!appHostResolver.TryResolveAppFromHost(context.HttpContext.Request.Host.Host, out string app))
            context.Fail("The request host resolves to no configured application.");
        else if (!AppAudience.Matches(context.Principal, app))
            context.Fail("The token was issued for a different application.");

        return Task.CompletedTask;
    }
}
