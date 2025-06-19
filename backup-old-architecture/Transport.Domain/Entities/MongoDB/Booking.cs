using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Transport.Domain.Entities.MongoDB;

public class Booking
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid? ProviderId { get; set; }
    public Guid TripId { get; set; }
    public Guid? CarpoolId { get; set; }
    public string Status { get; set; } // e.g., Pending, Confirmed, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int BoardingStopOrder { get; set; }
    public int DropOffStopOrder { get; set; }
    public decimal SegmentPrice { get; set; }
} 