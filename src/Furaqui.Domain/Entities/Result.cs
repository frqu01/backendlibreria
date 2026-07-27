using Furaqui.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace Furaqui.Domain.Entities;

public class Result
{
    [DefaultValue(null)]
    public object? Data { get; set; }

    [DefaultValue(null)]
    public RequestStatus? RequestStatus { get; set; }

    public List<ValidationError>? ValidationErrors { get; set; }

    public bool IsSuccessful()
    {
        return RequestStatus?.IsSuccess ?? false;
    }
}
