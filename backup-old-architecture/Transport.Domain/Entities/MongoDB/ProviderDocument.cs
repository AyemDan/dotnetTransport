using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class ProviderDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid ProviderId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FilePath { get; set; } // Simulated path or URL
    public string Status { get; set; } // Pending, Approved, Rejected
    public string? AdminNote { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
} 