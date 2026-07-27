using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RestBodyType
{
    [Description("application/x-www-form-urlencoded")]
    FormData = 1,

    [Description("application/json")]
    RawJson = 2
}
