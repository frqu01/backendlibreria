using System.Reflection;
using PagoDirecto.Application.Interfaces;
using PagoDirecto.Infrastructure.Services;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Infrastructure.Repositories;
using PagoDirecto.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;
using OpenIddict.Server;

namespace PagoDirecto.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPagoDirectoInfrastructure(this IServiceCollection services, IConfiguration iConfiguration)
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
        services.AddScoped<IExceptionManager, ExceptionManagerRepository>();
        services.AddScoped<IKafkaProducer, KafkaProducerRepository>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddSingleton<IProducer<Null, string>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var bootstrapServers = string.IsNullOrWhiteSpace(options.BootstrapServers) 
                ? "localhost:9092" : options.BootstrapServers;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                MessageTimeoutMs = 10000
            };
            return new ProducerBuilder<Null, string>(producerConfig).Build();
        });
        services.AddScoped<ILogRecorder, LogRecorderRepository>();
        services.AddScoped<IObjectMapper, ObjectMapperRepository>();
        services.AddScoped<IResponseFactory, ResponseFactoryRepository>();

        var appOptionsSection = iConfiguration.GetSection("AppOptions").Exists()
            ? iConfiguration.GetSection("AppOptions")
            : iConfiguration.GetSection("Application");

        services.Configure<ApplicationOptions>(appOptionsSection);
        services.Configure<CryptoOptions>(iConfiguration.GetSection("DataProtection"));
        services.Configure<KafkaOptions>(iConfiguration.GetSection("Kafka"));

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

    public static OpenIddictServerBuilder AddPagoDirectoOpenIddictHandlers(this OpenIddictServerBuilder builder)
    {
        builder.AddEventHandler<OpenIddictServerEvents.ApplyTokenResponseContext>(b =>
            b.UseScopedHandler<OpenIddictServerHandlerRepository>());
        
        return builder;
    }
}

