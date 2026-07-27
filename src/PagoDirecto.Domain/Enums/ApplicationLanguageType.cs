using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PagoDirecto.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationLanguageType
{
    [Description("en-us")]
    EnglishUs = 1,

    [Description("es-pe")]
    SpanishPe = 2
}

