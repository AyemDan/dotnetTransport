using Xunit;
using Transport.API.DTOs;
using Transport.API.Mappings;
using Transport.Domain.Entities.MongoDB;
using System;

namespace Transport.Tests.Unit
{
    public class BookingMapperTests
    {
        [Fact]
        public void ToDomain_MapsDtoToDomainCorrectly()
        {
            // Arrange
            var dto = new BookingDto
            {
                StudentId = Guid.NewGuid(),
                ProviderId = Guid.NewGuid(),
                TripId = Guid.NewGuid(),
                CarpoolId = Guid.NewGuid(),
                Status = "Pending"
            };

            // Act
            var domain = BookingMapper.ToDomain(dto);

            // Assert
            Assert.Equal(dto.StudentId, domain.StudentId);
            Assert.Equal(dto.ProviderId, domain.ProviderId);
            Assert.Equal(dto.TripId, domain.TripId);
            Assert.Equal(dto.CarpoolId, domain.CarpoolId);
            Assert.Equal(dto.Status, domain.Status);
        }
    }
} 