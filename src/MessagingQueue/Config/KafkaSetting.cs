namespace BackendApi.MessagingQueue.Config
{
    public class KafkaSetting
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
    }
}