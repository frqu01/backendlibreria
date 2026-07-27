using PagoDirecto.Domain.Enums;
using System.IO;

namespace PagoDirecto.Domain.Entities;

public class EmailAttachment
{
    public MemoryStream? AttachmentStream { get; set; }
    public string FileName { get; set; } = string.Empty;
    public FileExtensionType FileExtensionType { get; set; }
}

