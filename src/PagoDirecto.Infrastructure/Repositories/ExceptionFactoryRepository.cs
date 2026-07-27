using PagoDirecto.Application.Interfaces;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using System;

namespace PagoDirecto.Infrastructure.Repositories;

internal class ExceptionFactoryRepository : IExceptionFactory
{
    private static Exception CreateException(string errorMessage, NotificationType notificationType, int statusCode)
    {
        Exception excepcion = new(errorMessage);

        if (statusCode != 0)
        {
            excepcion.Data["StatusCode"] = statusCode;
        }

        excepcion.Data["Result"] = new Result()
        {
            RequestStatus = new RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = errorMessage,
                NotificationType = notificationType
            }
        };
        return excepcion;
    }

    public Exception Error(string errorMessage)
    {
        return CreateException(errorMessage, NotificationType.Error, 0);
    }

    public Exception Error(string errorMessage, int statusCode)
    {
        return CreateException(errorMessage, NotificationType.Error, statusCode);
    }

    public Exception Information(string errorMessage)
    {
        return CreateException(errorMessage, NotificationType.Information, 0);
    }

    public Exception Information(string errorMessage, int statusCode)
    {
        return CreateException(errorMessage, NotificationType.Information, statusCode);
    }

    public Exception Success(string errorMessage)
    {
        return CreateException(errorMessage, NotificationType.Success, 0);
    }

    public Exception Success(string errorMessage, int statusCode)
    {
        return CreateException(errorMessage, NotificationType.Success, statusCode);
    }

    public Exception Warning(string errorMessage)
    {
        return CreateException(errorMessage, NotificationType.Warning, 0);
    }

    public Exception Warning(string errorMessage, int statusCode)
    {
        return CreateException(errorMessage, NotificationType.Warning, statusCode);
    }
}
