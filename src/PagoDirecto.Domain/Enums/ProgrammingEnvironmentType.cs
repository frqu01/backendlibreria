using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PagoDirecto.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProgrammingEnvironmentType
{
    [Description("Desarrollo")]
    Development = 1,

    [Description("ProducciÃ³n")]
    Production = 2,

    [Description("Testing")]
    Testing = 3
}

