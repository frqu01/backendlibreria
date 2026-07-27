# FuraquiApi.Infrastructure

Capa de Infraestructura para la librería **FuraquiApi**.

Implementa la persistencia con Entity Framework Core, consumo REST con `IHttpClientFactory`, exportación de documentos (ClosedXML, iText) y mensajería con Confluent.Kafka.

## Registro DI
```csharp
services.AddInfrastructure();
```

## Instalación
```bash
dotnet add package FuraquiApi.Infrastructure
```
