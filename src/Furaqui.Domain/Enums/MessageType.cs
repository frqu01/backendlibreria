using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Furaqui.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageType
{
    [Description("Logger configurado sin directorio base.")]
    LoggerWithoutDirectory = 1,

    [Description("Logger configurado sin tipo de salida.")]
    LoggerWithoutDisplayType = 2,

    [Description("Logger iniciado correctamente.")]
    LoggerOk = 3,

    [Description("Se ha producido una excepción no controlada.")]
    UnhandledException = 4,

    [Description("Error al inicializar la documentación de Swagger.")]
    SwaggerError = 5
}
