using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Booking
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TripId { get; set; }
    public string BoardingStop { get; set; } = string.Empty;
    public string DropOffStop { get; set; } = string.Empty;
    public decimal SegmentPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 