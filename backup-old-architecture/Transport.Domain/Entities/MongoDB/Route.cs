using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Transport.Domain.Entities.MongoDB;

public class RouteStop
{
    public string Name { get; set; }
    public string Location { get; set; }
    public int Order { get; set; }
    public decimal PriceFromStart { get; set; }
}

public class Route
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public List<RouteStop> Stops { get; set; } = new();
    public string Schedule { get; set; } // e.g., "Mon-Fri 7:00AM, 4:00PM"
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 