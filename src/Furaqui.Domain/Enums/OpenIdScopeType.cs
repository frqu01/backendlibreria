using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
public enum OpenIdScopeType
    {
        Create = 1,
        Read = 2,
        Update = 3,
        Delete = 4,
        Activate = 5
    }
}

