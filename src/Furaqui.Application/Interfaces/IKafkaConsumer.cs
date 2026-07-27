using Furaqui.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces;

public interface IKafkaConsumer
{
    Task<Result> ConsumeAsync(KafkaConsumer consumer, Func<KafkaProducer, IServiceCollection, Task<Result>> action);
}
