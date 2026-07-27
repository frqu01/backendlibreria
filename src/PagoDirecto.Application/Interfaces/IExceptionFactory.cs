using PagoDirecto.Domain.Entities;
using System;

namespace PagoDirecto.Application.Interfaces;

public interface IExceptionFactory
{
    Exception Success(string errorMessage);
    Exception Success(string errorMessage, int statusCode);
    Exception Information(string errorMessage);
    Exception Information(string errorMessage, int statusCode);
    Exception Warning(string errorMessage);
    Exception Warning(string errorMessage, int statusCode);
    Exception Error(string errorMessage);
    Exception Error(string errorMessage, int statusCode);
}
