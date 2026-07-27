using Furaqui.Domain.Enums;
using System.ComponentModel;

namespace Furaqui.Domain.Entities;

public class RequestStatus
{
    private string _responseMessage = string.Empty;
    private string _responseMessageDetail = string.Empty;

    public int? DataCount { get; set; } = null;

    [DefaultValue(false)]
    public bool IsSuccess { get; set; }

    public string ResponseMessage
    {
        get => _responseMessage;
        set => _responseMessage = string.IsNullOrEmpty(value) ? value : value.EndsWith(".") ? value : value + ".";
    }

    public NotificationType NotificationTypeId { get; set; }

    public string ResponseMessageDetail
    {
        get => _responseMessageDetail;
        set => _responseMessageDetail = string.IsNullOrEmpty(value) ? value : value.EndsWith(".") ? value : value + ".";
    }
}
