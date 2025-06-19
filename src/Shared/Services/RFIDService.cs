using MongoDB.Driver;
using Transport.Shared.Entities.MongoDB;
using Transport.Shared.Interfaces;
using MongoDB.Bson;

namespace Transport.Shared.Services;

public class RFIDService : IRFIDService
{
    private readonly IMongoRepository<RFIDCard> _rfidRepository;

    public RFIDService(IMongoRepository<RFIDCard> rfidRepository)
    {
        _rfidRepository = rfidRepository;
    }

    public async Task<RFIDCard?> GetRFIDCardAsync(string cardNumber)
    {
        var cards = await _rfidRepository.FindAsync(c => c.CardNumber == cardNumber);
        return cards.FirstOrDefault();
    }

    public async Task<RFIDCard> CreateRFIDCardAsync(RFIDCard card)
    {
        card.Id = ObjectId.GenerateNewId();
        card.CreatedAt = DateTime.UtcNow;
        card.IsActive = true;
        await _rfidRepository.AddAsync(card);
        return card;
    }

    public async Task<bool> UpdateRFIDCardAsync(RFIDCard card)
    {
        try
        {
            await _rfidRepository.UpdateAsync(card.Id.ToString(), card);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeactivateRFIDCardAsync(string cardNumber)
    {
        var card = await GetRFIDCardAsync(cardNumber);
        if (card == null) return false;
        
        card.IsActive = false;
        return await UpdateRFIDCardAsync(card);
    }

    public async Task<IEnumerable<RFIDCard>> GetRFIDCardsByUserIdAsync(Guid userId)
    {
        return await _rfidRepository.FindAsync(c => c.UserId == userId);
    }

    public async Task<bool> ValidateRFIDCardAsync(string cardNumber)
    {
        var card = await GetRFIDCardAsync(cardNumber);
        return card != null && card.IsActive;
    }
} 