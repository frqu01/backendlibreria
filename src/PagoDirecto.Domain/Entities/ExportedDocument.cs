namespace PagoDirecto.Domain.Entities;

public record ExportedDocument(byte[] Contenido, string Tipo, string Nombre);

