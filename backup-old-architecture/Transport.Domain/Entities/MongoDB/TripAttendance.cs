using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class TripAttendance
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid TripId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime? TapInTime { get; set; }
    public DateTime? TapOutTime { get; set; }
    public string? TapInStop { get; set; }
    public string? TapOutStop { get; set; }
    public bool IsTapOutVetted { get; set; } = false;
    public string? VettingReason { get; set; }
} 