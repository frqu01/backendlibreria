using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KafkaTopicType
{
    None = 0,
    Username = 1,
    Person = 2,
    Scope = 3
}
