using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QueryOrderType
{
    Ascending = 1,
    Descending = 2
}
