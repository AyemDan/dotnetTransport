using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class RFIDCard
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string CardType { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastUsed { get; set; }
    public string? LastLocation { get; set; }
    public int UsageCount { get; set; }
    public string? AssignedVehicle { get; set; }
    public string? AssignedRoute { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 