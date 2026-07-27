using Microsoft.Extensions.Options;
using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class LogRecorderRepository : ILogRecorder
    {
        protected readonly IAppConfiguration _iAppConfiguration;
        protected readonly ApplicationOptions _applicationOptions;
        protected readonly ILoggerFactory _iLoggerFactory;
        protected readonly IConfiguration _iConfiguration;
        public LogRecorderRepository(IAppConfiguration iAppConfiguration, 
            IOptions<ApplicationOptions> applicationOptions,
            ILoggerFactory iLoggerFactory,
            IConfiguration iConfiguration)
        {
            _iAppConfiguration = iAppConfiguration;
            _applicationOptions = applicationOptions.Value;
            _iLoggerFactory = iLoggerFactory;
            _iConfiguration = iConfiguration;
        }
        private Result Log(string contenido, LoggerNotificationType tipoLogger)
        {
            Result resultadoApi = new();

            Microsoft.Extensions.Logging.ILogger loggerApi = _iLoggerFactory.CreateLogger("LoggerApi");
            string tipoLoggerNotificacion = tipoLogger.GetString();

            contenido = $"[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $"] :: [{tipoLoggerNotificacion}] :: [Message]: " + contenido;

            switch (tipoLogger)
            {
                case LoggerNotificationType.Success:
                    loggerApi.LogInformation(contenido);
                    break;
                case LoggerNotificationType.Information:
                    loggerApi.LogInformation(contenido);
                    break;
                case LoggerNotificationType.Warning:
                    loggerApi.LogWarning(contenido);
                    break;
                case LoggerNotificationType.Error:
                    loggerApi.LogError(contenido);
                    break;
                default:
                    break;
            }

            //Validar que se haya enviado la dirección del logger
            if (_iConfiguration.GetValue<string>("Logger:Directorio") == null || _iConfiguration.GetValue<string>("Logger:Directorio") == "")
            {
                resultadoApi.RequestStatus = new RequestStatus
                {
                    IsSuccess = false,
                    ResponseMessage = "No se envió el valor de la variable 'Directorio' en la llave 'Logger'.",
                    NotificationTypeId = NotificationType.Warning
                };

                return resultadoApi;
            }
            //Validar que se haya enviado el tipo del logger
            if (_iConfiguration.GetValue<string>("Logger:TipoMostrado") == null || _iConfiguration.GetValue<string>("Logger:TipoMostrado") == "")
            {
                resultadoApi.RequestStatus = new RequestStatus
                {
                    IsSuccess = false,
                    ResponseMessage = "No se envió el valor de la variable 'Tipo' en la llave 'Logger'.",
                    NotificationTypeId = NotificationType.Warning
                };

                loggerApi.LogError(JsonConvert.SerializeObject(contenido));

                return resultadoApi;
            }

            //Guardar log
            if (_iConfiguration.GetValue<string>("Logger:TipoMostrado") == LoggerDisplayType.Console.GetString())
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine(contenido);
                Console.ReadLine();

                loggerApi.LogError(JsonConvert.SerializeObject(contenido));
            }
            else
            {
                string directorio = _iConfiguration.GetValue<string>("Logger:Directorio") + $"\\Api-" + _iConfiguration.GetValue<string>("Application:Id") + "\\";
                string nombre = DateTime.Now.ToString("yyyy-MM-dd") + $"-Api-" + _iConfiguration.GetValue<string>("Application:Id") + ".log.txt";
                string directorioCompleto = directorio + nombre;

                if (true)
                {
                    if (!Directory.Exists(directorio))
                    {
                        Directory.CreateDirectory(directorio);
                    }
                    if (!File.Exists(directorioCompleto))
                    {
                        using FileStream fs = File.Create(directorioCompleto);
                        using StreamWriter writer = new(fs, Encoding.UTF8);
                        writer.Write(contenido + Environment.NewLine);
                    }
                    else
                    {
                        using StreamWriter writer = File.AppendText(directorioCompleto);
                        writer.WriteLine(contenido);
                    }
                }
                else
                {
                    loggerApi.LogError(JsonConvert.SerializeObject(contenido));
                }
            }

            resultadoApi.RequestStatus = new RequestStatus()
            {
                IsSuccess = true,
                ResponseMessage = "Logg creado correctamente.",
                NotificationTypeId = NotificationType.Success
            };

            return resultadoApi;
        }
        public Result Error(string contenido)
        {
            return Log(contenido, LoggerNotificationType.Error);
        }

        public Result Information(string contenido)
        {
            return Log(contenido, LoggerNotificationType.Information);
        }

        public Result Success(string contenido)
        {
            return Log(contenido, LoggerNotificationType.Success);
        }

        public Result Warning(string contenido)
        {
            return Log(contenido, LoggerNotificationType.Warning);
        }
    }
}

