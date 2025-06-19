using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Shared.Entities.MongoDB;

public class Route
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<RouteStop> Stops { get; set; } = new();
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public string Schedule { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class RouteStop
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal PriceFromStart { get; set; }
} 