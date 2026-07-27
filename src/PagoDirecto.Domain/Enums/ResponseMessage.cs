using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PagoDirecto.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResponseMessage
{
    [Description("Creado Correctamente.")]
    CreatedSuccessfully = 1,

    [Description("Listado Correctamente.")]
    RetrievedSuccessfully = 2,

    [Description("Actualizado Correctamente.")]
    UpdatedSuccessfully = 3,

    [Description("Eliminado Correctamente.")]
    DeletedSuccessfully = 4,

    [Description("Activado Correctamente.")]
    ActivatedSuccessfully = 5,

    [Description("{0} ya existe.")]
    RecordAlreadyExists = 6,

    [Description("Se encontraron errores de validaciÃ³n.")]
    ValidationError = 7,

    [Description("{0} no existe.")]
    NotFound = 8
}

