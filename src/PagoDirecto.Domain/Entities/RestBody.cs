using PagoDirecto.Domain.Enums;

namespace PagoDirecto.Domain.Entities;

public class RestBody
{
    public object? Payload { get; set; }
    public RestBodyType BodyType { get; set; }
}

