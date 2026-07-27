using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;
using static System.Net.Mime.MediaTypeNames;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class ExceptionManagerRepository : IExceptionManager
    {
        private readonly ILogger<ExceptionManagerRepository> _logger;

        public ExceptionManagerRepository(ILogger<ExceptionManagerRepository> logger)
        {
            _logger = logger;
        }
        //Errores controlados
        public async Task HandlerExceptionApplication(HttpContext context)
        {
            Result resultadoApi = new Result();
            string detalle = string.Empty;
            string mensaje = string.Empty; 

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Text.Plain;

            var ex = context.Features.Get<IExceptionHandlerPathFeature>();

            if (ex != null)
            {
                mensaje = ex.Error.GetBaseException().Message;

                if (ex.Error.Data["Result"] != null)
                {
                    resultadoApi = ex.Error.Data["Result"] as Result ?? new Result();
                    ex.Error.Data["Result"] = null;
                }
                else
                {
                    var traceId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
                    ex.Error.Data["TraceId"] = traceId;

                    resultadoApi = new Result()
                    {
                        RequestStatus = new RequestStatus()
                        {
                            IsSuccess = false,
                            ResponseMessage = "Ocurrió un error inesperado al procesar la solicitud.",
                            ResponseMessageDetail = $"Reference ID: {traceId}",
                            NotificationType = NotificationType.Error
                        }
                    };
                }

                if (ex.Error.Data["StatusCode"] is int statusCode)
                {
                    context.Response.StatusCode = statusCode;
                }

                if (ex.Error.Data.Contains("TraceId"))
                {
                    _logger.LogError(ex.Error, "Excepción no controlada en la aplicación. StatusCode: {StatusCode}, ReferenceID: {TraceId}", context.Response.StatusCode, ex.Error.Data["TraceId"]);
                }
                else
                {
                    _logger.LogError(ex.Error, "Excepción controlada devuelta por Result. StatusCode: {StatusCode}, Mensaje: {Mensaje}", context.Response.StatusCode, mensaje);
                }
            }
            else
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        ResponseMessage = MessageType.UnhandledException.GetString(),
                        NotificationType = NotificationType.Error
                    }
                };

                _logger.LogError("Ocurrió una excepción no controlada, pero no se pudo obtener el detalle del IExceptionHandlerPathFeature. StatusCode: 500");
            }

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(resultadoApi, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                            })).ConfigureAwait(false);
        }
        //Errores de carga en el servidor
        public async Task HandlerExceptionServer(StatusCodeContext context)
        {
            Result resultadoApi = new Result();

            var ex = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            string contenido = string.Empty;
            var traceId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            if (!context.HttpContext.Response.ContentLength.HasValue || context.HttpContext.Response.ContentLength == 0)
            {
                contenido = "Código: " + context.HttpContext.Response.StatusCode.ToString() + ". Url: " + context.HttpContext.Request.Path;

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = "Ocurrió un error en el servidor HTTP.",
                    ResponseMessageDetail = $"Reference ID: {traceId}",
                    NotificationType = NotificationType.Error
                };
            }
            else
            {
                contenido = "Código: 500 - Error en el servidor.";

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = "Error interno crítico en el servidor.",
                    ResponseMessageDetail = $"Reference ID: {traceId}",
                    NotificationType = NotificationType.Error
                };
            }

            if (ex?.Error != null)
            {
                _logger.LogError(ex.Error, "Error en el pipeline del servidor. StatusCode: {StatusCode}, ReferenceID: {TraceId}", context.HttpContext.Response.StatusCode, traceId);
            }
            else
            {
                _logger.LogError("Error en el pipeline del servidor. StatusCode: {StatusCode}, Url: {Url}, ReferenceID: {TraceId}", context.HttpContext.Response.StatusCode, context.HttpContext.Request.Path, traceId);
            }

            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(resultadoApi, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                            }));
        }
        public void ExceptionSaveRecord(ChangeTracker changeTracker)
        {
            var errores = new List<ValidationError>();
            int count = 1;
            int registroEmpresaId = 0;
            long registroApiUsernameId = 0;

            if (changeTracker.Entries().Any())
            {
                foreach (var item in changeTracker.Entries())
                {
                    var nameEntity = item.Entity.GetType().Name;

                    if (item.Entity is EntityRecord record)
                    {
                        if (record.CompanyRecordId == 0)
                        {
                            if (registroEmpresaId != 0)
                            {
                                record.CompanyRecordId = registroEmpresaId;
                            }
                            else
                            {
                                errores.Add(new ValidationError()
                                {
                                    Entity = nameEntity,
                                    Field = "CompanyRecordId",
                                    Message = "'CompanyRecordId' no debería estar vacío."
                                });
                            }
                        }
                        else
                        {
                            registroEmpresaId = record.CompanyRecordId;
                        }

                        if (record.UserRecordId == 0)
                        {
                            if (registroApiUsernameId != 0)
                            {
                                record.UserRecordId = registroApiUsernameId;
                            }
                            else
                            {
                                errores.Add(new ValidationError()
                                {
                                    Entity = nameEntity,
                                    Field = "UserRecordId",
                                    Message = "'UserRecordId' no debería estar vacío."
                                });
                            }
                        }
                        else
                        {
                            registroApiUsernameId = record.UserRecordId;
                        }
                    }

                    count++;
                }

                if (errores.Count > 0)
                {
                    Exception exception = new Exception();

                    var resultadoApi = new Result()
                    {
                        RequestStatus = new RequestStatus()
                        {
                            IsSuccess = false,
                            ResponseMessage = "Se encontraron errores de validación.",
                            NotificationType = NotificationType.Warning
                        },
                        ValidationErrors = errores.Count > 0 ? errores : null
                    };

                    exception.Data["Result"] = resultadoApi;
                    exception.Data["StatusCode"] = 400;

                    _logger.LogWarning("Se abortó el guardado porque se encontraron {Count} errores de validación.", errores.Count);

                    throw exception;
                }
            }
        }
    }
}

