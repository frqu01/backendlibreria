namespace Furaqui.Domain.Entities;

public class KafkaConsumer
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string KafkaTopic { get; set; } = string.Empty;
}
