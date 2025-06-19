using Transport.API.DTOs;
using Transport.Domain.Entities.MongoDB;
using System;
using System.Collections.Generic;

namespace Transport.API.Mappings
{
    public static class PaymentLogMapper
    {
        public static PaymentLog ToDomain(PaymentInfoDto dto)
        {
            return new PaymentLog
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Status = "Completed",
                Type = "Manual",
                CreatedAt = DateTime.UtcNow,
                TripId = null
            };
        }
    }
} 