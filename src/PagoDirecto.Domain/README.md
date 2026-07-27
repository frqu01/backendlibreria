# PagoDirecto.Domain

Este paquete representa el núcleo y corazón de la arquitectura (Core). 

Contiene:
- **Entidades (Entities)**: Clases que representan el modelo de datos transversal (`Email`, `Pagination`, `Result`, etc.).
- **Enumeraciones (Enums)**: Constantes globales de configuración e identificación para toda la organización.

## Características

- **Sin dependencias tecnológicas**: No tiene referencias a Entity Framework, SQL, ni APIs web. Su pureza garantiza que las reglas de negocio no estén acopladas a la infraestructura.
- **Validaciones Integradas**: Utiliza `FluentValidation` para asegurar que las entidades cumplan con las reglas esenciales.

## ¿Cuándo usar este paquete?
Debe ser referenciado por la capa **Application** de tu microservicio o por cualquier proyecto de Dominio que requiera heredar los modelos base de la empresa.
