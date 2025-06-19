using Transport.Shared.Entities.MongoDB;

namespace Transport.Shared.Services;

public interface IBookingService
{
    Task<Booking?> GetBookingAsync(Guid bookingId);
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<bool> UpdateBookingAsync(Booking booking);
    Task<bool> CancelBookingAsync(Guid bookingId);
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId);
    Task<IEnumerable<Booking>> GetBookingsByTripIdAsync(Guid tripId);
    Task<bool> ConfirmBookingAsync(Guid bookingId);
    Task<bool> RejectBookingAsync(Guid bookingId);
} 