# PagoDirecto.Application

Esta capa define las reglas, contratos y los casos de uso para las aplicaciones de PagoDirecto.

Contiene:
- **Interfaces**: Contratos que la capa de infraestructura deberá implementar (`IUnitOfWork`, `IDataBaseContext`, `IHttpRestClient`, `IKafkaConsumer`, `IEmailSelector`).
- **Extensiones (Extensions)**: Métodos extensores para Strings, Enums, Reflection, y Queries, que facilitan la manipulación de datos en la lógica de negocio.

## Características
Al igual que el Dominio, la capa de Aplicación no sabe *cómo* se guardan los datos o *cómo* se envían los correos, simplemente establece **qué** acciones pueden realizarse.

## ¿Cuándo usar este paquete?
Debe ser referenciado por tu capa de **Lógica de Negocio / Casos de Uso (Application)** en tus microservicios. Depende únicamente de `PagoDirecto.Domain`.
