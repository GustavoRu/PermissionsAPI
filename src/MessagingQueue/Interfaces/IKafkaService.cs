using BackendApi.MessagingQueue.DTOs;
namespace BackendApi.MessagingQueue.Interfaces
{
    public interface IKafkaService
    {
        Task SendMessageAsync(string OperationTypeString);
    }
}

