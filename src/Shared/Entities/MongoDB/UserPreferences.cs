using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class UserPreferences
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SMSNotifications { get; set; } = false;
    public string? PreferredPaymentMethod { get; set; }
    public string? PreferredRoute { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 