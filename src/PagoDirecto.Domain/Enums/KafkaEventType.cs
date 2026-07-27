using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
public enum KafkaEventType
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Activate = 4,
        Read = 5,
        Other = 6
    }
}


