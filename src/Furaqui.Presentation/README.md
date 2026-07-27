# FuraquiApi.Presentation

Capa de Presentación e Integración Web para la librería **FuraquiApi**.

Proporciona controladores base, filtros de validación de modelos (`FuraquiValidatorFilterApiRepository`) y el método de extensión para la inyección de dependencias completa (`AddFuraquiLibrary()`).

## Registro DI en Program.cs
```csharp
builder.Services.AddFuraquiLibrary();
```

## Instalación
```bash
dotnet add package FuraquiApi.Presentation
```
