# PagoDirecto Librería Backend

Esta es la librería base (Seed / Boilerplate) para la construcción de microservicios y APIs de **PagoDirecto**. Está diseñada siguiendo los principios de **Clean Architecture**, lo que permite una alta cohesión y bajo acoplamiento entre las distintas responsabilidades de los servicios.

## Estructura de la Solución

El proyecto está dividido en 4 capas principales, representadas por 4 paquetes NuGet individuales. Esto permite que cada servicio consuma únicamente las dependencias que necesita:

1. **`PagoDirecto.Domain`**: Contiene las entidades, enumeraciones y modelos centrales del negocio. No tiene dependencias externas, lo que garantiza la pureza del modelo.
2. **`PagoDirecto.Application`**: Define los contratos (interfaces) para infraestructura, casos de uso, servicios externos y extensiones. Aquí se encuentran `IUnitOfWork`, `IKafkaProducer`, entre otros.
3. **`PagoDirecto.Infrastructure`**: Implementa los contratos definidos en la capa de aplicación. Contiene integraciones concretas con Entity Framework Core, Confluent Kafka, AutoMapper, ClosedXML, iText, entre otras tecnologías.
4. **`PagoDirecto.Presentation`**: Proporciona clases base, controladores (Controllers), filtros y configuraciones (como Swagger y MediatR) para exponer los servicios a través de HTTP/REST.

## Requisitos

- .NET 10.0
- Visual Studio 2022 o IDE compatible.

## Versionamiento

La versión de todos los paquetes está unificada a través del archivo `Directory.Build.props` ubicado en la raíz del repositorio. Las actualizaciones de versión deben realizarse en este archivo para asegurar consistencia en todos los paquetes NuGet generados.

## Instalación y Uso

Una vez generados los paquetes NuGet (mediante `dotnet pack`), puedes instalarlos en tus proyectos. Por ejemplo, en tu capa de API:

```xml
<PackageReference Include="PagoDirecto.Presentation" Version="1.2.77" />
<PackageReference Include="PagoDirecto.Infrastructure" Version="1.2.77" />
```

En tu capa de Lógica de Negocio:
```xml
<PackageReference Include="PagoDirecto.Application" Version="1.2.77" />
```
