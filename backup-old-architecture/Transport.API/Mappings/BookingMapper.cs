using Transport.API.DTOs;
using Transport.Domain.Entities.MongoDB;
using System;

namespace Transport.API.Mappings
{
    public static class BookingMapper
    {
        public static Booking ToDomain(BookingDto dto)
        {
            return new Booking
            {
                StudentId = dto.StudentId,
                ProviderId = dto.ProviderId,
                TripId = dto.TripId,
                CarpoolId = dto.CarpoolId,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
} 