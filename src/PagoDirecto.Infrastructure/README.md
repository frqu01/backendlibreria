# PagoDirecto.Infrastructure

Este paquete provee las implementaciones tecnológicas concretas para las interfaces definidas en la capa de Aplicación.

Contiene:
- **Bases de Datos**: Contextos y repositorios genéricos usando `EntityFrameworkCore` y `Microsoft.Data.SqlClient`.
- **Mensajería**: Productores y consumidores implementados usando `Confluent.Kafka`.
- **Exportación de Documentos**: Generación de archivos usando `ClosedXML` (Excel) e `iText` (PDF).
- **Utilidades**: Implementaciones criptográficas, mappers con `AutoMapper`, y clientes HTTP.
- **Seguridad**: Configuración base de Identity y `OpenIddict`.

## ¿Cuándo usar este paquete?
Debe ser referenciado únicamente en la capa más externa de tu aplicación (normalmente la capa de **Presentación** o de arranque `Program.cs`) para registrar las dependencias en el contenedor de Inyección de Dependencias (DI).

Tus casos de uso en la capa de Aplicación **nunca** deben hacer referencia directa a este proyecto.
