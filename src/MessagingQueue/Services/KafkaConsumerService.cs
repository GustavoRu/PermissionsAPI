using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using BackendApi.MessagingQueue.Config;
using System.Text.Json;
using BackendApi.MessagingQueue.DTOs;

namespace BackendApi.MessagingQueue.Services
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly string _topicName;
        private readonly string _bootstrapServers;
        private IConsumer<string, string> _consumer;
        private Task _consumingTask;
        private CancellationTokenSource _cancellationTokenSource;

        public KafkaConsumerService(
            IOptions<KafkaSetting> settings,
            ILogger<KafkaConsumerService> logger)
        {
            _logger = logger;
            _topicName = settings.Value.TopicName;
            _bootstrapServers = settings.Value.BootstrapServers;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = "permissions-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                AllowAutoCreateTopics = true
            };

            _logger.LogInformation("Iniciando consumidor de Kafka para el topic: {TopicName}", _topicName);
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _consumer.Subscribe(_topicName);

            _consumingTask = Task.Run(() => ConsumeMessages(_cancellationTokenSource.Token));

            return Task.CompletedTask;
        }

        private async Task ConsumeMessages(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        if (consumeResult != null)
                        {
                            var message = JsonSerializer.Deserialize<OperationMessageDto>(consumeResult.Message.Value);
                            _logger.LogInformation(
                                "Mensaje RECIBIDO- Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Operation: {Operation}",
                                consumeResult.Topic,
                                consumeResult.Partition.Value,
                                consumeResult.Offset.Value,
                                message?.NameOperation
                            );
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consuming was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while consuming messages");
            }
            finally
            {
                _consumer?.Close();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _consumer?.Close();
            _consumer?.Dispose();
            return Task.CompletedTask;
        }
    }
} 