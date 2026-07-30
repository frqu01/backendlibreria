using Microsoft.AspNetCore.Http;
using PagoDirecto.Application.Interfaces;
using System.Linq;

namespace PagoDirecto.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private void ThrowValidationException(string field)
    {
        var exception = new Exception("Validación de seguridad fallida.");
        var resultadoApi = new PagoDirecto.Domain.Entities.Result()
        {
            RequestStatus = new PagoDirecto.Domain.Entities.RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = "Se encontraron errores de validación.",
                NotificationType = PagoDirecto.Domain.Enums.NotificationType.Warning
            },
            ValidationErrors = new List<PagoDirecto.Domain.Entities.ValidationError>()
            {
                new PagoDirecto.Domain.Entities.ValidationError() { Field = field, Message = $"'{field}' es requerido y debe venir en el Token válido." }
            }
        };

        exception.Data["Result"] = resultadoApi;
        exception.Data["StatusCode"] = StatusCodes.Status401Unauthorized;

        throw exception;
    }

    public long UserRecordId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated) ThrowValidationException("UserRecordId");
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "UserRecordId")?.Value;
            if (!long.TryParse(claim, out var val) || val == 0)
                ThrowValidationException("UserRecordId");
                
            return val;
        }
    }

    public int CompanyRecordId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated) ThrowValidationException("CompanyRecordId");
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "CompanyRecordId")?.Value;
            if (!int.TryParse(claim, out var val) || val == 0)
                ThrowValidationException("CompanyRecordId");
                
            return val;
        }
    }
}
