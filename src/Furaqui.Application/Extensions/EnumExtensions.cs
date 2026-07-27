using System;
using System.ComponentModel;
using System.Reflection;

namespace Furaqui.Application.Extensions;

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
    public static Furaqui.Domain.Enums.ResponseMessage ToResponseMessage(this Furaqui.Domain.Enums.SchemaType schemaType) => schemaType switch
    {
        Furaqui.Domain.Enums.SchemaType.Create => Furaqui.Domain.Enums.ResponseMessage.CreatedSuccessfully,
        Furaqui.Domain.Enums.SchemaType.Read => Furaqui.Domain.Enums.ResponseMessage.RetrievedSuccessfully,
        Furaqui.Domain.Enums.SchemaType.Update => Furaqui.Domain.Enums.ResponseMessage.UpdatedSuccessfully,
        Furaqui.Domain.Enums.SchemaType.Delete => Furaqui.Domain.Enums.ResponseMessage.DeletedSuccessfully,
        Furaqui.Domain.Enums.SchemaType.Activate => Furaqui.Domain.Enums.ResponseMessage.ActivatedSuccessfully,
        _ => Furaqui.Domain.Enums.ResponseMessage.RetrievedSuccessfully
    };
}