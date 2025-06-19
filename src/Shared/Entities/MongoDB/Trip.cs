using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Trip
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid ProviderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Stops { get; set; } = new();
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string TripType { get; set; } = string.Empty;
    public string TrackingInfo { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 