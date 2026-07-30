using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PagoDirecto.Presentation.AuthConfiguration;
using System.Text;

namespace PagoDirecto.Presentation.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddPagoDirectoAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authConfig = configuration.GetSection("Autenticacion").Get<AuthConfig>() ?? new AuthConfig();

        if (authConfig.Modo.Equals("OpenIddict", System.StringComparison.OrdinalIgnoreCase))
        {
            services.AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer(authConfig.OpenIddict.Servidor);
                    
                    if (!string.IsNullOrEmpty(authConfig.OpenIddict.Api))
                    {
                        options.AddAudiences(authConfig.OpenIddict.Api);
                    }

                    options.UseIntrospection()
                           .SetClientId(authConfig.OpenIddict.CredencialesApi.Codigo)
                           .SetClientSecret(authConfig.OpenIddict.CredencialesApi.Contrasena);

                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });
        }
        else if (authConfig.Modo.Equals("Jwt", System.StringComparison.OrdinalIgnoreCase))
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authConfig.Jwt.Issuer,
                    ValidAudience = authConfig.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.Jwt.SecretKey))
                };
            });
            
            services.AddAuthorization();
        }

        return services;
    }
}
