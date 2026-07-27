using Furaqui.Domain.Enums;

namespace Furaqui.Domain.Entities;

public class RestBody
{
    public object? Payload { get; set; }
    public RestBodyType BodyType { get; set; }
}
