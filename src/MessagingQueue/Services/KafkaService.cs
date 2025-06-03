using BackendApi.MessagingQueue.DTOs;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using BackendApi.MessagingQueue.Config;
using BackendApi.MessagingQueue.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BackendApi.MessagingQueue.Services
{
    public class KafkaService : IKafkaService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topicName;
        private readonly ILogger<KafkaService> _logger;

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public KafkaService(IOptions<KafkaSetting> settings, ILogger<KafkaService> logger)
        {
            _logger = logger;
            try 
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = settings.Value.BootstrapServers,
                    // Add additional configurations for better error handling
                    Acks = Acks.All,
                    MessageTimeoutMs = 5000,
                    EnableDeliveryReports = true,
                    // Permitir creación automática de topics
                    AllowAutoCreateTopics = true
                };

                _logger.LogInformation("Initializing Kafka producer with bootstrap servers: {BootstrapServers}", settings.Value.BootstrapServers);
                _producer = new ProducerBuilder<string, string>(config).Build();
                _topicName = settings.Value.TopicName;
                _logger.LogInformation("Kafka producer initialized successfully for topic: {TopicName}", _topicName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Kafka producer");
                throw;
            }
        }

        public async Task SendMessageAsync(string OperationTypeString)
        {
            if (string.IsNullOrEmpty(_topicName))
            {
                _logger.LogError("Topic name is not configured properly");
                throw new InvalidOperationException("Topic name is not configured");
            }

            _logger.LogInformation("Preparing to send message for operation: {OperationType}", OperationTypeString);
            
            var message = new OperationMessageDto
            {
                NameOperation = OperationTypeString
            };

            var messageValue = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.Id.ToString(),
                Value = messageValue
            };

            try
            {
                _logger.LogDebug("Attempting to send message to topic {TopicName}. Message: {Message}", _topicName, messageValue);
                var deliveryResult = await _producer.ProduceAsync(_topicName, kafkaMessage);
                _logger.LogInformation("Message delivered successfully to {TopicPartitionOffset}. Operation: {OperationType}", 
                    deliveryResult.TopicPartitionOffset, 
                    OperationTypeString);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Failed to deliver message. Error code: {ErrorCode}, Reason: {Reason}, Operation: {OperationType}", 
                    ex.Error.Code, 
                    ex.Error.Reason,
                    OperationTypeString);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending message. Operation: {OperationType}", OperationTypeString);
                throw;
            }
        }
    }
}