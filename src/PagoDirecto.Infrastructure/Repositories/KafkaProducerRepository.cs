using PagoDirecto.Application.Interfaces;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories;

internal class KafkaProducerRepository : IKafkaProducer
{
    protected readonly ILogger<KafkaProducerRepository> _logger;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerRepository(ILogger<KafkaProducerRepository> logger, IProducer<Null, string> producer)
    {
        _logger = logger;
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

            _logger.LogInformation("Mensaje Kafka publicado en tópico '{Topic}' (Offset: {Offset})", topic, result.Offset);

            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = true,
                ResponseMessage = $"Message enviado a {topic} -res: {result.Offset}",
                NotificationType = NotificationType.Success
            };
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError(e, "Error publicando mensaje Kafka en tópico '{Topic}': {Reason}", topic, e.Error.Reason);
            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = $"Fallo al enviar mensaje a Kafka: {e.Error.Reason}",
                NotificationType = NotificationType.Error
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en KafkaProducerRepository al publicar en tópico '{Topic}'", topic);
            resultado.RequestStatus = new RequestStatus()
            {
                IsSuccess = false,
                ResponseMessage = $"Error inesperado al enviar mensaje: {ex.Message}",
                NotificationType = NotificationType.Error
            };
        }

        return resultado;
    }
}

