using PagoDirecto.Domain.Enums;
using System.ComponentModel;

namespace PagoDirecto.Domain.Entities;

public class RequestStatus
{
    private string _responseMessage = string.Empty;
    private string? _responseMessageDetail = null;

    [DefaultValue(false)]
    public bool IsSuccess { get; set; }

    public string ResponseMessage
    {
        get => _responseMessage;
        set => _responseMessage = string.IsNullOrEmpty(value) ? value : value.EndsWith(".") ? value : value + ".";
    }

    public NotificationType NotificationType { get; set; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public string? ResponseMessageDetail
    {
        get => _responseMessageDetail;
        set => _responseMessageDetail = string.IsNullOrWhiteSpace(value) ? null : value.EndsWith(".") ? value : value + ".";
    }
}
