using PagoDirecto.Application.Interfaces;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories;

internal class KafkaProducerRepository : IKafkaProducer
{
    protected readonly ILogRecorder _iLoggerApi;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerRepository(ILogRecorder iLoggerApi, IProducer<Null, string> producer)
    {
        _iLoggerApi = iLoggerApi;
        _producer = producer;
    }

    public async Task<Result> ProduceAsync(string topic, KafkaProducer message)
    {
        var resultado = new Result();
        var jsonMessage = JsonConvert.SerializeObject(message);

        var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });

        // Optional: log using _iLoggerApi instead of Console if preferred
        Console.WriteLine($"Message enviado a {topic} -res: {result.Offset}");

        resultado.RequestStatus = new RequestStatus()
        {
            IsSuccess = true,
            ResponseMessage = $"Message enviado a {topic} -res: {result.Offset}",
            NotificationTypeId = NotificationType.Success
        };

        return resultado;
    }
}

