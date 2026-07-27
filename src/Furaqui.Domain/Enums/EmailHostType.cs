using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailHostType
{
    Outlook = 1,
    Gmail = 2
}

public static class EmailHostTypeExtensions
{
    public static string GetHost(this EmailHostType hostType) => hostType switch
    {
        EmailHostType.Outlook => "smtp.office365.com",
        EmailHostType.Gmail => "smtp.gmail.com",
        _ => string.Empty
    };

    public static int GetPort(this EmailHostType hostType) => hostType switch
    {
        EmailHostType.Outlook => 587,
        EmailHostType.Gmail => 587,
        _ => 587
    };
}
