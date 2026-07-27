using PagoDirecto.Domain.Entities;
using PagoDirecto.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Collections.Generic;
using System.Reflection;

namespace PagoDirecto.Presentation.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddPagoDirectoLibrary(this IServiceCollection services, IConfiguration iConfiguration)
    {
        services.AddPagoDirectoInfrastructure(iConfiguration);
        services.AddPagoDirectoSwagger(iConfiguration);
        return services;
    }

    public static IApplicationBuilder UsePagoDirectoLibrary(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UsePagoDirectoSwagger(configuration);
        return app;
    }

    public static IServiceCollection AddPagoDirectoSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var appOptions = configuration.GetSection("AppOptions").Get<ApplicationOptions>()
                         ?? configuration.GetSection("Application").Get<ApplicationOptions>()
                         ?? configuration.GetSection(nameof(ApplicationOptions)).Get<ApplicationOptions>()
                         ?? new ApplicationOptions();

        var entryAssemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);

        if (string.IsNullOrEmpty(appOptions.ApplicationVersion) || appOptions.ApplicationVersion == "1.0.0")
        {
            appOptions.ApplicationVersion = !string.IsNullOrEmpty(entryAssemblyVersion)
                ? entryAssemblyVersion
                : (configuration.GetValue<string>("Swagger:Version") ?? "1.0.0");
        }

        if (string.IsNullOrEmpty(appOptions.ApplicationName))
        {
            appOptions.ApplicationName = configuration.GetValue<string>("Swagger:Title") ?? "PagoDirecto API Service";
        }
        if (string.IsNullOrEmpty(appOptions.ApplicationCode))
        {
            appOptions.ApplicationCode = configuration.GetValue<string>("Keys:AplicacionId") ?? string.Empty;
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("service", new OpenApiInfo
            {
                Title = appOptions.ApplicationName,
                Version = appOptions.ApplicationVersion,
                Description = $"Documentación REST para {appOptions.ApplicationName}",
                Contact = new OpenApiContact
                {
                    Name = "Frank Quiroz Gil",
                    Email = "ing_fquirozg@hotmail.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Ingrese únicamente su token JWT sin la palabra 'Bearer'. Ejemplo: '12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UsePagoDirectoSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var appOptions = configuration.GetSection("AppOptions").Get<ApplicationOptions>()
                         ?? configuration.GetSection("Application").Get<ApplicationOptions>()
                         ?? configuration.GetSection(nameof(ApplicationOptions)).Get<ApplicationOptions>()
                         ?? new ApplicationOptions();

        if (string.IsNullOrEmpty(appOptions.ApplicationCode))
        {
            appOptions.ApplicationCode = configuration.GetValue<string>("Keys:AplicacionId") ?? string.Empty;
        }
        if (string.IsNullOrEmpty(appOptions.ApplicationName))
        {
            appOptions.ApplicationName = configuration.GetValue<string>("Swagger:Title") ?? "PagoDirecto API Service";
        }

        app.UseSwagger(c =>
        {
            c.RouteTemplate = string.IsNullOrEmpty(appOptions.ApplicationCode)
                ? "swagger/{documentName}/swagger.json"
                : $"swagger/{{documentName}}/aplicacionid{appOptions.ApplicationCode}.json";
        });

        app.UseSwaggerUI(c =>
        {
            var endpointUrl = string.IsNullOrEmpty(appOptions.ApplicationCode)
                ? "/swagger/service/swagger.json"
                : $"/swagger/service/aplicacionid{appOptions.ApplicationCode}.json";

            c.SwaggerEndpoint(endpointUrl, appOptions.ApplicationName);
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}

