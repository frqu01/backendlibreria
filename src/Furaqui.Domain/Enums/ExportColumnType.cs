using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExportColumnType
{
    [Description("String")]
    String = 1,

    [Description("Int32")]
    Int32 = 2,

    [Description("Boolean")]
    Boolean = 3,

    [Description("Decimal")]
    Decimal = 4,

    [Description("Datetime")]
    Datetime = 5,

    [Description("Int64")]
    Int64 = 6,

    [Description("Int16")]
    Int16 = 7,

    [Description("Double")]
    Double = 8
}

public static class ExportColumnTypeExtensions
{
    public static ExportColumnType GetEnumByName(string name)
    {
        return name switch
        {
            "String" => ExportColumnType.String,
            "Int32" => ExportColumnType.Int32,
            "Boolean" => ExportColumnType.Boolean,
            "Decimal" => ExportColumnType.Decimal,
            "Datetime" => ExportColumnType.Datetime,
            "Int64" => ExportColumnType.Int64,
            "Int16" => ExportColumnType.Int16,
            "Double" => ExportColumnType.Double,
            _ => throw new NotImplementedException($"ExportColumnType '{name}' is not supported.")
        };
    }
}
