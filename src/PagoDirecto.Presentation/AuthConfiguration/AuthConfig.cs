namespace PagoDirecto.Presentation.AuthConfiguration;

public class AuthConfig
{
    public string Modo { get; set; } = "OpenIddict";
    public OpenIddictConfig OpenIddict { get; set; } = new();
    public JwtConfig Jwt { get; set; } = new();
}
