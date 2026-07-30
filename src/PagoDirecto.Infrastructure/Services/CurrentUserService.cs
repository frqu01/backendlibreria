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
            if (user == null || !user.Identity.IsAuthenticated) return 0;
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "UserRecordId")?.Value;
            return long.TryParse(claim, out var val) ? val : 0;
        }
    }

    public int CompanyRecordId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated) return 0;
            
            var claim = user.Claims.FirstOrDefault(c => c.Type == "CompanyRecordId")?.Value;
            return int.TryParse(claim, out var val) ? val : 0;
        }
    }
}
