using Microsoft.Extensions.Options;
using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
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
        protected readonly ApplicationOptions _applicationOptions;
        protected readonly ILoggerFactory _iLoggerFactory;
        
        public LogRecorderRepository(
            IOptions<ApplicationOptions> applicationOptions,
            ILoggerFactory iLoggerFactory)
        {
            _applicationOptions = applicationOptions.Value;
            _iLoggerFactory = iLoggerFactory;
        }
        private Result Log(string contenido, LoggerNotificationType tipoLogger)
        {
            Microsoft.Extensions.Logging.ILogger loggerApi = _iLoggerFactory.CreateLogger("LoggerApi");
            
            switch (tipoLogger)
            {
                case LoggerNotificationType.Success:
                case LoggerNotificationType.Information:
                    loggerApi.LogInformation("{Message}", contenido);
                    break;
                case LoggerNotificationType.Warning:
                    loggerApi.LogWarning("{Message}", contenido);
                    break;
                case LoggerNotificationType.Error:
                    loggerApi.LogError("{Message}", contenido);
                    break;
                default:
                    loggerApi.LogInformation("{Message}", contenido);
                    break;
            }

            return new Result
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = true,
                    ResponseMessage = "Log procesado correctamente.",
                    NotificationTypeId = NotificationType.Success
                }
            };
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

