# PagoDirectoApi.Infrastructure

Capa de Infraestructura para la librerÃ­a **PagoDirectoApi**.

Implementa la persistencia con Entity Framework Core, consumo REST con `IHttpClientFactory`, exportaciÃ³n de documentos (ClosedXML, iText) y mensajerÃ­a con Confluent.Kafka.

## Registro DI
```csharp
services.AddInfrastructure();
```

## InstalaciÃ³n
```bash
dotnet add package PagoDirectoApi.Infrastructure
```

