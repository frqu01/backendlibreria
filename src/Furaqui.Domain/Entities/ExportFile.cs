namespace Furaqui.Domain.Entities;

public class ExportFile
{
    public byte[] Content { get; set; } = System.Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
