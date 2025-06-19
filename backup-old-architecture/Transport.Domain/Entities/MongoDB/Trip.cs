using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Domain.Entities.MongoDB;

public class Trip
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }  // Link to PostgreSQL User
    public string Status { get; set; }  // e.g., pending, ongoing, completed
    public List<RouteStop> Stops { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? ProviderId { get; set; } // For direct trips
    public Guid? CarpoolId { get; set; } // For carpool trips
    public string TripType { get; set; } // "Direct" or "Carpool"
    public string TrackingInfo { get; set; } // For ride tracking simulation
    public List<LocationUpdate> Locations { get; set; } = new(); // For tracking trip/vehicle locations
}

// Location update for tracking
public class LocationUpdate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}
