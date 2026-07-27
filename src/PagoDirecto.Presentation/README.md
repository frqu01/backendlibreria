# PagoDirectoApi.Presentation

Capa de PresentaciÃ³n e IntegraciÃ³n Web para la librerÃ­a **PagoDirectoApi**.

Proporciona controladores base, filtros de validaciÃ³n de modelos (`PagoDirectoValidatorFilterApiRepository`) y el mÃ©todo de extensiÃ³n para la inyecciÃ³n de dependencias completa (`AddPagoDirectoLibrary()`).

## Registro DI en Program.cs
```csharp
builder.Services.AddPagoDirectoLibrary();
```

## InstalaciÃ³n
```bash
dotnet add package PagoDirectoApi.Presentation
```

