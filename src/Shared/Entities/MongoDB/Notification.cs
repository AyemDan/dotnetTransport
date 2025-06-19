using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Notification
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
} 