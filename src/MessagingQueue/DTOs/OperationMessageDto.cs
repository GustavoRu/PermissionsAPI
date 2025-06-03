namespace BackendApi.MessagingQueue.DTOs
{
    public class OperationMessageDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NameOperation { get; set; } = string.Empty;

        public static class Operations
        {
            public const string Modify = "modify";
            public const string Request = "request";
            public const string Get = "get";
        }
        
    }
}