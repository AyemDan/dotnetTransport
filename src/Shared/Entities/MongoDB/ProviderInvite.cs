using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class ProviderInvite
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime Expiry { get; set; }
    public bool Used { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 