using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Domain.Entities.MongoDB;

public class UserPreferences
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }  // Link to PostgreSQL User
    public string Theme { get; set; }
    public bool NotificationsEnabled { get; set; }
    public string? PreferredPaymentMethod { get; set; }
    public string? PaymentDetails { get; set; } // e.g., card token, wallet id, etc.
}
