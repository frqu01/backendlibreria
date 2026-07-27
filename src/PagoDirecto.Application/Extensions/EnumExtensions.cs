using System;
using System.ComponentModel;
using System.Reflection;

namespace PagoDirecto.Application.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the description attribute value of an enum.
    /// Returns .ToString() if no [Description] attribute is present.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field == null) return value.ToString();

        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Gets the string representation or Description attribute of an enum value.
    /// </summary>
    public static string GetString(this Enum value)
    {
        return value.GetDescription();
    }

    /// <summary>
    /// Gets the response string for an enum.
    /// </summary>
    public static string GetResponse(this Enum value)
    {
        return value.GetDescription();
    }

    /// <summary>
    /// Parses an enum value by its name.
    /// </summary>
    public static TEnum GetEnumByName<TEnum>(string name) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(name, true, out var result))
            return result;
        return default;
    }

    /// <summary>
    /// Maps SchemaType to ResponseMessage.
    /// </summary>
    public static PagoDirecto.Domain.Enums.ResponseMessage ToResponseMessage(this PagoDirecto.Domain.Enums.SchemaType schemaType) => schemaType switch
    {
        PagoDirecto.Domain.Enums.SchemaType.Create => PagoDirecto.Domain.Enums.ResponseMessage.CreatedSuccessfully,
        PagoDirecto.Domain.Enums.SchemaType.Read => PagoDirecto.Domain.Enums.ResponseMessage.RetrievedSuccessfully,
        PagoDirecto.Domain.Enums.SchemaType.Update => PagoDirecto.Domain.Enums.ResponseMessage.UpdatedSuccessfully,
        PagoDirecto.Domain.Enums.SchemaType.Delete => PagoDirecto.Domain.Enums.ResponseMessage.DeletedSuccessfully,
        PagoDirecto.Domain.Enums.SchemaType.Activate => PagoDirecto.Domain.Enums.ResponseMessage.ActivatedSuccessfully,
        _ => PagoDirecto.Domain.Enums.ResponseMessage.RetrievedSuccessfully
    };
}
