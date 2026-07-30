namespace PagoDirecto.Presentation.AuthConfiguration;

public class OpenIddictConfig
{
    public string Servidor { get; set; } = string.Empty;
    public string Api { get; set; } = string.Empty;
    public ApiCredentialsConfig CredencialesApi { get; set; } = new();
}
