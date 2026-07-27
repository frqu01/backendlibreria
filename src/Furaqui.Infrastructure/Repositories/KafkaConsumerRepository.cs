using Furaqui.Application.Interfaces;
using Furaqui.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Furaqui.Infrastructure.Repositories;

internal class KafkaConsumerRepository : IKafkaConsumer
{
    protected readonly IAppConfiguration _iAppConfiguration;
    protected readonly ILogRecorder _iLoggerApi;

    public KafkaConsumerRepository(ILogRecorder iLoggerApi, IAppConfiguration iAppConfiguration)
    {
        _iLoggerApi = iLoggerApi;
        _iAppConfiguration = iAppConfiguration;
    }

    public async Task<Result> ConsumeAsync(KafkaConsumer kafkaConsumerApi, Func<KafkaProducer, IServiceCollection, Task<Result>> ejecutarFuncion)
    {
        await Task.Delay(0);

        var resultado = new Result();
        var consumerConfig = new ConsumerConfig
        {
            GroupId = kafkaConsumerApi.GroupId,
            BootstrapServers = kafkaConsumerApi.BootstrapServers
        };

        using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
        {
            consumer.Subscribe(kafkaConsumerApi.KafkaTopic);

            while (true)
            {
                var cr = consumer.Consume(CancellationToken.None);
                KafkaProducer kafkaProducerApi = JsonConvert.DeserializeObject<KafkaProducer>(cr.Message.Value);
                await ejecutarFuncion(kafkaProducerApi, null);
                Console.WriteLine(cr.Message.Value);
            }
        }

        return null;
    }
}
