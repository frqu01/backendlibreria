using System.Text.Json.Serialization;

namespace PagoDirecto.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KafkaTopicType
{
    None = 0,
    Username = 1,
    Person = 2,
    Scope = 3
}

