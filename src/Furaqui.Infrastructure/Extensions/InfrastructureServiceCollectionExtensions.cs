using System.Reflection;
using Furaqui.Application.Interfaces;
using Furaqui.Domain.Entities;
using Furaqui.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Furaqui.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddFuraquiInfrastructure(this IServiceCollection services, IConfiguration iConfiguration)
    {
        services.AddHttpClient();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDataBaseContext, DataBaseContextRepository>();
        services.AddScoped<IHttpRestClient, HttpRestClientRepository>();
        services.AddScoped<IEmailSelector, EmailSelectorRepository>();
        services.AddScoped<ICryptoService, CryptoServiceRepository>();
        services.AddScoped<IExceptionFactory, ExceptionFactoryRepository>();
        services.AddScoped<IDocumentExporter, DocumentExporterRepository>();
        services.AddScoped<IAppConfiguration, AppConfigurationRepository>();
        services.AddScoped<IExceptionManager, ExceptionManagerRepository>();
        services.AddScoped<IKafkaProducer, KafkaProducerRepository>();
        services.AddScoped<IKafkaConsumer, KafkaConsumerRepository>();
        services.AddScoped<ILogRecorder, LogRecorderRepository>();
        services.AddScoped<IObjectMapper, ObjectMapperRepository>();
        services.AddScoped<IResponseFactory, ResponseFactoryRepository>();

        var appOptionsSection = iConfiguration.GetSection("AppOptions").Exists()
            ? iConfiguration.GetSection("AppOptions")
            : iConfiguration.GetSection("Application");

        services.Configure<ApplicationOptions>(appOptionsSection);

        services.PostConfigure<ApplicationOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.ApplicationVersion) || options.ApplicationVersion == "1.0.0")
            {
                var assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3);
                if (!string.IsNullOrEmpty(assemblyVersion))
                {
                    options.ApplicationVersion = assemblyVersion;
                }
            }
        });

        return services;
    }
}
