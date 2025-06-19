using Transport.Shared.Entities.MongoDB;

namespace Transport.Shared.Services;

public interface IRFIDService
{
    Task<RFIDCard?> GetRFIDCardAsync(string cardNumber);
    Task<RFIDCard> CreateRFIDCardAsync(RFIDCard card);
    Task<bool> UpdateRFIDCardAsync(RFIDCard card);
    Task<bool> DeactivateRFIDCardAsync(string cardNumber);
    Task<IEnumerable<RFIDCard>> GetRFIDCardsByUserIdAsync(Guid userId);
    Task<bool> ValidateRFIDCardAsync(string cardNumber);
} 