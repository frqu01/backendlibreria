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
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });

            _iLoggerApi.Information($"Mensaje Kafka publicado en tópico '{topic}' (Offset: {result.Offset})");

            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = true,
                ResponseMessage = $"Message enviado a {topic} -res: {result.Offset}",
                NotificationTypeId = NotificationType.Success
            };
        }
        catch (ProduceException<Null, string> e)
        {
            _iLoggerApi.Error($"Error publicando mensaje Kafka en tópico '{topic}': {e.Error.Reason}");
            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = $"Fallo al enviar mensaje a Kafka: {e.Error.Reason}",
                NotificationTypeId = NotificationType.Error
            };
        }
        catch (Exception ex)
        {
            _iLoggerApi.Error($"Error inesperado en KafkaProducerRepository: {ex.Message}");
            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = $"Error inesperado al enviar mensaje: {ex.Message}",
                NotificationTypeId = NotificationType.Error
            };
        }

        return resultado;
    }
}

