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

    public long UserRecordId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated) throw new UnauthorizedAccessException("El token es requerido.");
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "UserRecordId")?.Value;
            if (!long.TryParse(claim, out var val) || val == 0)
                throw new UnauthorizedAccessException("El token no contiene un UserRecordId válido.");
                
            return val;
        }
    }

    public int CompanyRecordId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated) throw new UnauthorizedAccessException("El token es requerido.");
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "CompanyRecordId")?.Value;
            if (!int.TryParse(claim, out var val) || val == 0)
                throw new UnauthorizedAccessException("El token no contiene un CompanyRecordId válido.");
                
            return val;
        }
    }
}
