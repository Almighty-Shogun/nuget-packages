using System.Text;
using AlmightyShogun.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
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
    /// <param name="serviceCollection">The service collection used to register the API auth functionality.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the API auth package services using the provided configuration root.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that contains the required <c>Auth</c> section.</param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with authentication configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public IServiceCollection AddApiAuth(IConfiguration configuration)
        {
            serviceCollection
                .AddConfiguration<AuthSettings>(configuration.GetSection("Auth"))
                .AddHttpContextAccessor()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearerAuthentication(configuration);

            return serviceCollection
                .AddSingleton<IAppHostResolver, AppHostResolver>()
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }

    /// <param name="builder">The authentication builder used to register the JWT functionality.</param>
    extension(AuthenticationBuilder builder)
    {
        /// <summary>
        /// Registers JWT bearer authentication configured from the <c>Auth</c> configuration section.
        /// </summary>
        ///
        /// <param name="configuration">The application configuration that contains issuer, secret, lifetime, and host mapping settings.</param>
        ///
        /// <returns>The <see cref="AuthenticationBuilder"/> instance with JWT bearer configured.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>2.3.0</since>
        public AuthenticationBuilder AddJwtBearerAuthentication(IConfiguration configuration) => builder.AddJwtBearer(options =>
        {
            var authSettings = configuration.GetSection("Auth").Get<AuthSettings>();

            if (authSettings is null)
                throw new InvalidOperationException("Missing Auth configuration");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var appHostResolver = context.HttpContext.RequestServices.GetRequiredService<IAppHostResolver>();

                    if (!appHostResolver.TryResolveAppFromHost(context.HttpContext.Request.Host.Host, out string app))
                    {
                        context.Fail("Unknown application");

                        return Task.CompletedTask;
                    }

                    IEnumerable<string> audiences = context.SecurityToken switch
                    {
                        JwtSecurityToken jwtSecurityToken => jwtSecurityToken.Audiences,
                        JsonWebToken jsonWebToken => jsonWebToken.Audiences,
                        _ => []
                    };

                    bool validAudience = audiences.Any(audience => string.Equals(audience, app, StringComparison.OrdinalIgnoreCase));

                    if (!validAudience)
                    {
                        context.Fail("Invalid audience");
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }
}
