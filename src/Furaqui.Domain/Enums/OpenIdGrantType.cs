using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
public enum OpenIdGrantType
    {

    }
}

