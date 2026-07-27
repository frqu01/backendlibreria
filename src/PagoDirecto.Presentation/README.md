# PagoDirecto.Presentation

Esta capa proporciona las herramientas base para construir APIs y controladores web estandarizados en PagoDirecto.

Contiene:
- **Controladores Base (`ApiController`)**: Define métodos estandarizados para responder a peticiones HTTP.
- **Filtros (Filters)**: Atributos y filtros de acción como `ValidatorFilterAttribute` para capturar errores de validación de FluentValidation de manera uniforme.
- **Extensiones Web**: Configuraciones listas para usar de `Swagger` (OpenAPI) y `MediatR` para proyectos ASP.NET Core.

## ¿Cuándo usar este paquete?
Debe ser referenciado por tus proyectos de **API Web (ASP.NET Core)**. Te permite estandarizar las respuestas JSON, centralizar el manejo de errores web, y facilitar la configuración del pipeline HTTP en tu `Program.cs`.
