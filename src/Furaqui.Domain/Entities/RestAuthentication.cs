using Furaqui.Domain.Enums;

namespace Furaqui.Domain.Entities;

public class RestAuthentication
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RestAuthorizationType AuthorizationType { get; set; }
}
