namespace PagoDirecto.Domain.Entities;

public class ValidationError
{
    public string Entity { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

