using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class Notification
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
    public string Type { get; set; } // e.g., Info, Alert, Emergency
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 