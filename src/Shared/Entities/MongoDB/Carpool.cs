using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Carpool
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid TripId { get; set; }
    public Guid DriverId { get; set; }
    public int AvailableSeats { get; set; }
    public int OccupiedSeats { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 