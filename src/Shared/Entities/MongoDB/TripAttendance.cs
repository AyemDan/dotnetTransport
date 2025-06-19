using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class TripAttendance
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = string.Empty; // Present, Absent, Late
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 