using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PagoDirecto.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TableColumnType
{
    [Description("smallint")]
    Int16 = 1,

    [Description("int")]
    Int32 = 2,

    [Description("bigint")]
    Int64 = 3,

    [Description("bit")]
    Boolean = 4,

    [Description("char")]
    Char = 5,

    [Description("date")]
    Date = 6,

    [Description("datetime")]
    DateTime = 7,

    [Description("decimal")]
    Decimal = 8,

    [Description("float")]
    Double = 9,

    [Description("float")]
    Float = 10,

    [Description("money")]
    Money = 11,

    [Description("nchar")]
    NChar = 12,

    [Description("ntext")]
    NText = 13,

    [Description("numeric")]
    Numeric = 14,   

    [Description("nvarchar")]
    NVarchar = 15,

    [Description("text")]
    Text = 16,

    [Description("time")]
    Time = 17,

    [Description("timestamp")]
    TimeStamp = 18,

    [Description("varchar")]
    Varchar = 19,

    [Description("datetime2")]
    DateTime2 = 20
}

public static class TableColumnTypeExtensions
{
    public static string GetSqlType(this TableColumnType columnType, int length = 100)
    {
        return columnType switch
        {
            TableColumnType.Char => $"char({length})",
            TableColumnType.NChar => $"nchar({length})",
            TableColumnType.NVarchar => $"nvarchar({length})",
            TableColumnType.Varchar => $"varchar({length})",
            _ => columnType.ToString().ToLower()
        };
    }

    public static string GetSqlType(this TableColumnType columnType, int lengthInteger, int lengthDecimal)
    {
        return columnType switch
        {
            TableColumnType.Decimal => $"decimal({lengthInteger},{lengthDecimal})",
            TableColumnType.Numeric => $"numeric({lengthInteger},{lengthDecimal})",
            _ => columnType.ToString().ToLower()
        };
    }
}

