using Furaqui.Domain.Enums;
using System.IO;

namespace Furaqui.Domain.Entities;

public class EmailAttachment
{
    public MemoryStream? AttachmentStream { get; set; }
    public string FileName { get; set; } = string.Empty;
    public FileExtensionType FileExtensionType { get; set; }
}
