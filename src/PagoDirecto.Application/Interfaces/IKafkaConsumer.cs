using PagoDirecto.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces;

public interface IKafkaConsumer
{
    Task<Result> ConsumeAsync(KafkaConsumer consumer, Func<KafkaProducer, IServiceCollection, Task<Result>> action);
}

