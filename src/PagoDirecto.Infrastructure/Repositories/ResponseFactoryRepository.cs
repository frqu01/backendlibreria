using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class ResponseFactoryRepository : IResponseFactory
    {
        public Result Success()
        {
            return ReplyToEstado(NotificationType.Success.GetString(), null, NotificationType.Success);
        }
        public Result Success(string mensaje, object? dato)
        {
            return ReplyToEstado(mensaje, dato, NotificationType.Success);
        }
        public Result Success(object? dato)
        {
            return dato is string ? ReplyToEstado((string)dato, null, NotificationType.Success) : ReplyToEstado(NotificationType.Success.GetString(), dato, NotificationType.Success);
        }
        public Result Information()
        {
            return ReplyToEstado(NotificationType.Information.GetString(), null, NotificationType.Information);
        }
        public Result Information(object? dato)
        {
            return dato is string ?  ReplyToEstado((string)dato, null, NotificationType.Information) : ReplyToEstado(NotificationType.Information.GetString(), dato, NotificationType.Information);
        }
        public Result Information(string mensaje, object? dato)
        {
            return ReplyToEstado(mensaje, dato, NotificationType.Information);
        }
        public Result Warning()
        {
            return ReplyToEstado(NotificationType.Warning.GetString(), null, NotificationType.Warning);
        }
        public Result Warning(object? dato)
        {
            return dato is string ? ReplyToEstado((string)dato, null, NotificationType.Warning) : ReplyToEstado(NotificationType.Warning.GetString(), dato, NotificationType.Warning);
        }
        public Result Warning(string mensaje, object? dato)
        {
            return ReplyToEstado(mensaje, dato, NotificationType.Warning);
        }
        public Result Error()
        {
            return ReplyToEstado(NotificationType.Error.GetString(), null, NotificationType.Error);
        }
        public Result Error(object? dato)
        {
            return dato is string ? ReplyToEstado((string)dato, null, NotificationType.Error) : ReplyToEstado(NotificationType.Error.GetString(), dato, NotificationType.Error);
        }
        public Result Error(string mensaje, object? dato)
        {
            return ReplyToEstado(mensaje, dato, NotificationType.Error);
        }
        private Result ReplyToEstado(string mensaje, object? dato, NotificationType tipoNotificacionApi)
        {
            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = tipoNotificacionApi switch
                    {
                        NotificationType.Success => true,
                        NotificationType.Information => true,
                        NotificationType.Warning => false,
                        NotificationType.Error => false,
                        _ => true
                    },
                    ResponseMessage = mensaje,
                    NotificationTypeId = tipoNotificacionApi
                },
                Data = dato
            };
        }
        public Result AlreadyExist(string campo)
        {
            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(ResponseMessage.RecordAlreadyExists.GetString(), campo),
                    NotificationTypeId = NotificationType.Warning
                }
            };
        }
        public Result AlreadyExist()
        {
            return AlreadyExist("Record");
        }
        public Result CreateOk()
        {
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully);
        }
        public Result ReadOk()
        {
            return ReplyToCorrecta(ResponseMessage.RetrievedSuccessfully);
        }
        public Result ReadOk(object? dato)
        {
            return ReplyToCorrecta(ResponseMessage.RetrievedSuccessfully, dato);
        }
        public Result ReadOk(object? dato, int cantidadDatos)
        {
            return ReplyToCorrecta(ResponseMessage.RetrievedSuccessfully,dato,cantidadDatos);
        }
        public Result UpdateOk()
        {
            return ReplyToCorrecta(ResponseMessage.UpdatedSuccessfully);
        }
        public Result DeleteOk()
        {
            return ReplyToCorrecta(ResponseMessage.DeletedSuccessfully);
        }
        public Result ActivateOk()
        {
            return ReplyToCorrecta(ResponseMessage.ActivatedSuccessfully);
        }
        private Result ReplyToCorrecta(ResponseMessage tipoResponseMessageApi)
        {
            return ReplyToCorrecta(tipoResponseMessageApi, null, null);
        }
        private Result ReplyToCorrecta(ResponseMessage tipoResponseMessageApi, object? dato)
        {
            return ReplyToCorrecta(tipoResponseMessageApi, dato, null);
        }
        private Result ReplyToCorrecta(ResponseMessage tipoResponseMessageApi, object? dato, int? cantidadDatos)
        {
            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = true,
                    ResponseMessage = tipoResponseMessageApi.GetString(),
                    NotificationTypeId = NotificationType.Success,
                    DataCount = cantidadDatos
                },
                Data = dato
            };
        }
        public Result New(Result resultadoApi)
        {
            return resultadoApi;
        }
        public Result ErrorValidate(string campo, string mensaje)
        {
            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = false,
                    ResponseMessage = ResponseMessage.ValidationError.GetString(),
                    NotificationTypeId = NotificationType.Warning
                },
                ValidationErrors = new()
                {
                    new ValidationError()
                    {
                        Field = campo,
                        Message = mensaje
                    }
                }
            };
        }
        public Result ReadByIdOk(object? dato)
        {
            return ReplyToCorrecta(ResponseMessage.RetrievedSuccessfully, dato);
        }

        public Result NotExist(string campo)
        {
            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(ResponseMessage.NotFound.GetString(), campo),
                    NotificationTypeId = NotificationType.Warning
                }
            };
        }
        public Result NotExist()
        {
            return NotExist("Record");
        }
        public Result CreateOk(int id)
        {
            var idResponse = new
            {
                Id = id
            };
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, idResponse);
        }
        public Result CreateOk(long id)
        {
            var idResponse = new
            {
                Id = id
            };
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, idResponse);
        }
        public Result CreateOk(List<int> ids)
        {
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, ids);
        }
        public Result CreateOk(List<long> ids)
        {
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, ids);
        }
        public Result AlreadyExist(object? dato)
        {
            if(dato is string)
            {
                return AlreadyExist((string) dato);
            }

            return new Result()
            {
                RequestStatus = new()
                {
                    IsSuccess = false,
                    ResponseMessage = string.Format(ResponseMessage.RecordAlreadyExists.GetString(), "La lista"),
                    NotificationTypeId = NotificationType.Warning
                },
                Data = dato
            };
        }

        public Result CreateOk(List<string> ids)
        {
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, ids);
        }
        public Result CreateOk(string id)
        {
            var idResponse = new
            {
                Id = id
            };
            return ReplyToCorrecta(ResponseMessage.CreatedSuccessfully, idResponse);
        }
    }
}

