using System.Collections.Generic;

namespace Furaqui.Domain.Entities;

public class Email
{
    public string Sender { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> ReplyTo { get; set; } = new();
    public List<string> Recipients { get; set; } = new();
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<EmailAttachment> Attachments { get; set; } = new();
}
