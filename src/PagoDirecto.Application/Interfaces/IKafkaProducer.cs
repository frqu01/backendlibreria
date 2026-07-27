using PagoDirecto.Domain.Entities;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces;

public interface IKafkaProducer
{
    Task<Result> ProduceAsync(string topic, KafkaProducer message);
}

