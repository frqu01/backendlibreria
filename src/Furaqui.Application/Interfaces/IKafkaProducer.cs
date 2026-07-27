using Furaqui.Domain.Entities;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces;

public interface IKafkaProducer
{
    Task<Result> ProduceAsync(KafkaProducer producer);
}
