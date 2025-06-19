using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Subscription
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RouteId { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 