using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transport.Domain.Entities.MongoDB;

public class PaymentInfo
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid UserId { get; set; }
    public string PaymentMethod { get; set; }
    public string CardToken { get; set; } // Tokenized data
    public List<string> Invoices { get; set; } // Invoice references
}
