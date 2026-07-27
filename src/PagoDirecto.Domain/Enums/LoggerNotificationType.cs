using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoggerNotificationType
    {
        Success = 1,
        Information = 2,
        Warning = 3,
        Error = 4
    }
}


