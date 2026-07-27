using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
public enum OpenIdErrorType
    {
        OpenIdNotRetrieved = 1,
        GrantNotImplemented = 2,
        ClientNotFound = 3
    }
}


