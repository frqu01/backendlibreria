using PagoDirecto.Domain.Enums;
using System;

namespace PagoDirecto.Domain.Entities;

public class KafkaProducer
{
    public Guid KafkaProducerId { get; set; } = Guid.NewGuid();
    public KafkaEventType KafkaEventType { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public object? Entity { get; set; }
}

