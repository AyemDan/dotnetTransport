using System;

namespace Transport.Shared.DTOs;

public class NotificationDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 