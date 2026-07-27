using PagoDirecto.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace PagoDirecto.Domain.Entities;

public class Result
{
    [DefaultValue(null)]
    public object? Data { get; set; }

    [DefaultValue(null)]
    public RequestStatus? RequestStatus { get; set; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    public List<ValidationError>? ValidationErrors { get; set; }

    public bool IsSuccessful()
    {
        return RequestStatus?.IsSuccess ?? false;
    }
}

