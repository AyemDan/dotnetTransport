using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class RFIDCard
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string CardNumber { get; set; }
    public Guid? UserId { get; set; } // Nullable for unassigned cards
    public decimal Balance { get; set; }
    public string Status { get; set; } // e.g., Active, Blocked
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 