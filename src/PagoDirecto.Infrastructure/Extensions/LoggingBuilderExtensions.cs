using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Enums;
using Serilog;
using System.IO;

namespace PagoDirecto.Infrastructure.Extensions;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Configura Serilog leyendo las llaves "Logger:Directorio" y "Logger:TipoMostrado" del appsettings.json.
    /// Reemplaza al logger por defecto de .NET.
    /// </summary>
    public static ILoggingBuilder AddPagoDirectoSerilog(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var directorio = configuration.GetValue<string>("Logger:Directorio");
        var tipoMostrado = configuration.GetValue<string>("Logger:TipoMostrado");
        var appId = configuration.GetValue<string>("Application:Id") ?? "Unknown";

        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext();

        if (tipoMostrado == LoggerDisplayType.Console.GetString())
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss}] :: [{Level:u3}] :: [Message]: {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(directorio))
            {
                directorio = "Logs";
            }
            
            // Serilog maneja rotación diaria agregando la fecha donde esté el guión. 
            // Ej: log-20231024.txt
            string dirCompleto = Path.Combine(directorio, $"Api-{appId}", "log-.txt");
            
            loggerConfig.WriteTo.File(
                path: dirCompleto,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss}] :: [{Level:u3}] :: [Message]: {Message:lj}{NewLine}{Exception}");
        }

        var serilogLogger = loggerConfig.CreateLogger();

        // Limpiar providers antiguos (como la consola bloqueante de .NET) y usar Serilog
        builder.ClearProviders();
        builder.AddSerilog(serilogLogger);

        return builder;
    }
}
