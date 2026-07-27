using PagoDirecto.Domain.Enums;

namespace PagoDirecto.Domain.Entities;

public class RestAuthentication
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public RestAuthorizationType AuthorizationType { get; set; }
}

