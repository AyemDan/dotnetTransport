namespace Transport.API.DTOs
{
    public class NotificationDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
} 