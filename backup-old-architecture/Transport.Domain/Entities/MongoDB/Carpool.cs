using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Transport.Domain.Entities.MongoDB;

public enum CarpoolType { Public, Private }

public class Carpool
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid TripId { get; set; }
    public Guid ProviderId { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public List<Guid> ParticipantStudentIds { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public CarpoolType Type { get; set; } = CarpoolType.Public;
} 