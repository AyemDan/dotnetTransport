using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Domain.Entities.MongoDB;

public class Trip
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }  // Link to PostgreSQL User
    public string Status { get; set; }  // e.g., pending, ongoing, completed
    public List<TripPoint> Stops { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Trip Point Model (Embedded in Trip)
public class TripPoint
{
    public string Location { get; set; }
    public DateTime ArrivalTime { get; set; }
    public DateTime DepartureTime { get; set; }
    public string TransportMode { get; set; } // e.g., car, bus, train
}
