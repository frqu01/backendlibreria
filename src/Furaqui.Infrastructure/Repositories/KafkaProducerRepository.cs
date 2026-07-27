using Furaqui.Application.Interfaces;
using Furaqui.Domain.Entities;
using Furaqui.Domain.Enums;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Furaqui.Infrastructure.Repositories;

internal class KafkaProducerRepository : IKafkaProducer
{
    protected readonly IAppConfiguration _iAppConfiguration;
    protected readonly ILogRecorder _iLoggerApi;

    public KafkaProducerRepository(ILogRecorder iLoggerApi, IAppConfiguration iAppConfiguration)
    {
        _iLoggerApi = iLoggerApi;
        _iAppConfiguration = iAppConfiguration;
    }

    public async Task<Result> ProduceAsync(KafkaProducer kafkaSolicitudApi)
    {
        var resultado = new Result();
        var jsonMessage = JsonConvert.SerializeObject(kafkaSolicitudApi);
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaSolicitudApi.BootstrapServers,
            MessageTimeoutMs = 10000
        };

        using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
        {
            var result = await producer.ProduceAsync(kafkaSolicitudApi.KafkaTopic,
                new Message<Null, string> { Value = jsonMessage });

            Console.WriteLine($"Message enviado a {kafkaSolicitudApi.KafkaTopic} -res: {result.Offset}");

            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = true,
                ResponseMessage = $"Message enviado a {kafkaSolicitudApi.KafkaTopic} -res: {result.Offset}",
                NotificationTypeId = NotificationType.Success
            };
        }

        return resultado;
    }
}
