using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PagoDirecto.Application.Interfaces;
using System;
using System.Text;

namespace PagoDirecto.Infrastructure.Extensions;

public static class InfrastructureJwtExtensions
{
    public static IServiceCollection AddPagoDirectoJwtAuthentication(this IServiceCollection services, string secretKey, string issuer, string audience)
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
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    // Prevenimos la respuesta 401 por defecto de .NET
                    context.HandleResponse();

                    var exceptionFactory = context.HttpContext.RequestServices.GetRequiredService<IExceptionFactory>();
                    
                    // Verificamos si el error fue porque el token expiró
                    if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        throw exceptionFactory.Error("El token ha expirado. Por favor, inicie sesión nuevamente.", StatusCodes.Status401Unauthorized);
                    }
                    
                    throw exceptionFactory.Error("No estás autorizado o el token JWT es inválido.", StatusCodes.Status401Unauthorized);
                },
                OnForbidden = context =>
                {
                    var exceptionFactory = context.HttpContext.RequestServices.GetRequiredService<IExceptionFactory>();
                    throw exceptionFactory.Error("No tienes permisos suficientes para acceder a este recurso.", StatusCodes.Status403Forbidden);
                }
            };
        });

        return services;
    }
}
