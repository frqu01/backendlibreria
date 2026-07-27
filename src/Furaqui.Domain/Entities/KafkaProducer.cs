using Furaqui.Domain.Enums;
using System;

namespace Furaqui.Domain.Entities;

public class KafkaProducer
{
    public Guid KafkaProducerId { get; set; } = Guid.NewGuid();
    public KafkaEventType KafkaEventType { get; set; }
    public string KafkaTopic { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public object? Entity { get; set; }
    public string BootstrapServers { get; set; } = string.Empty;
}
