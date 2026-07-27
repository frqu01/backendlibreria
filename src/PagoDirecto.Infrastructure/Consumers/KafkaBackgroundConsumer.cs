using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Consumers;

public abstract class KafkaBackgroundConsumer<T> : BackgroundService where T : class
{
    private readonly ILogger<KafkaBackgroundConsumer<T>> _logger;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    protected KafkaBackgroundConsumer(ILogger<KafkaBackgroundConsumer<T>> logger, string bootstrapServers, string groupId, string topic)
    {
        _logger = logger;
        _topic = topic;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected abstract Task ProcessMessageAsync(T message, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando Kafka Consumer para el tópico: {Topic}", _topic);

        await Task.Yield(); // Ensures the method becomes asynchronous immediately

        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message?.Value != null)
                    {
                        var message = JsonConvert.DeserializeObject<T>(consumeResult.Message.Value);
                        if (message != null)
                        {
                            await ProcessMessageAsync(message, stoppingToken);
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error consumiendo mensaje: {Reason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancelación solicitada, cerrando consumidor Kafka...");
        }
        finally
        {
            consumer.Close();
        }
    }
}
