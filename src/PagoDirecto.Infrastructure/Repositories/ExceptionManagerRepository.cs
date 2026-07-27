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
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;
using static System.Net.Mime.MediaTypeNames;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class ExceptionManagerRepository : IExceptionManager
    {
        private readonly ILogRecorder _iLoggerApi;

        public ExceptionManagerRepository(ILogRecorder iLoggerApi)
        {
            _iLoggerApi = iLoggerApi;
        }
        //Errores controlados
        public async Task HandlerExceptionApplication(HttpContext context)
        {
            Result resultadoApi = new Result();
            string contenido = string.Empty;
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
                    resultadoApi = ex.Error.Data["Result"] as Result;
                    ex.Error.Data["Result"] = null;
                }
                else
                {
                    resultadoApi = new Result()
                    {
                        RequestStatus = new RequestStatus()
                        {
                            IsSuccess = false,
                            ResponseMessage = ex.Error.Message == null ? "" : ex.Error.Message, //MessageTypeApi.UnhandledException.GetString() + ,
                            ResponseMessageDetail = ex.Error.InnerException == null ? null : ex.Error.InnerException.Message,
                            NotificationTypeId = NotificationType.Error
                        }
                    };
                }

                if (ex.Error.Data["StatusCode"] != null)
                {
                    context.Response.StatusCode = (int)ex.Error.Data["StatusCode"];
                }

                contenido = mensaje + $" :: [StatusCode] :: {context.Response.StatusCode} :: [Tracer] :: {ex.Error.StackTrace}";
            }
            else
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        ResponseMessage = MessageType.UnhandledException.GetString(),
                        NotificationTypeId = NotificationType.Error
                    }
                };

                contenido = ex.Error.Message == null ? MessageType.UnhandledException.GetString() : ex.Error.Message + $" :: [StatusCode] :: 500 :: [Tracer] :: No se pudo obtener el detalle del error";
            }

            _iLoggerApi.Error(contenido);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(resultadoApi, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            })).ConfigureAwait(false);
        }
        //Errores de carga en el servidor
        public async Task HandlerExceptionServer(StatusCodeContext context)
        {
            Result resultadoApi = new Result();

            var ex = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            string contenido = string.Empty;

            if (!context.HttpContext.Response.ContentLength.HasValue || context.HttpContext.Response.ContentLength == 0)
            {
                contenido = "Código: " + context.HttpContext.Response.StatusCode.ToString() + ". " + ((HttpStatusCode)context.HttpContext.Response.StatusCode).ToString() + ". Url: " + context.HttpContext.Request.Path;

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = contenido,
                    NotificationTypeId = NotificationType.Error
                };
            }
            else
            {
                contenido = "Código: 500 - Error en el servidor.";

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = contenido,
                    NotificationTypeId = NotificationType.Error
                };
            }

            _iLoggerApi.Error(contenido);

            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(resultadoApi, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
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
                            NotificationTypeId = NotificationType.Warning
                        },
                        ValidationErrors = errores
                    };

                    exception.Data["Result"] = resultadoApi;
                    exception.Data["StatusCode"] = 400;

                    _iLoggerApi.Error("Se encontraron errores de validación.");

                    throw exception;
                }
            }
        }
    }
}

