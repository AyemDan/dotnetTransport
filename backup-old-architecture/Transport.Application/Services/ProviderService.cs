using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;
using Transport.Domain.Entities;

namespace Transport.Application.Services
{
    public class ProviderService
    {
        private readonly IMongoRepository<Trip> _tripRepo;
        private readonly IMongoRepository<Booking> _bookingRepo;
        private readonly IMongoRepository<Carpool> _carpoolRepo;
        private readonly IMongoRepository<PaymentLog> _paymentRepo;
        private readonly IMongoRepository<Notification> _notificationRepo;
        private readonly IMongoRepository<Driver> _driverRepo;

        public ProviderService(
            IMongoRepository<Trip> tripRepo,
            IMongoRepository<Booking> bookingRepo,
            IMongoRepository<Carpool> carpoolRepo,
            IMongoRepository<PaymentLog> paymentRepo,
            IMongoRepository<Notification> notificationRepo,
            IMongoRepository<Driver> driverRepo)
        {
            _tripRepo = tripRepo;
            _bookingRepo = bookingRepo;
            _carpoolRepo = carpoolRepo;
            _paymentRepo = paymentRepo;
            _notificationRepo = notificationRepo;
            _driverRepo = driverRepo;
        }

        // Placeholder: Accept or decline booking
        public async Task UpdateBookingStatusAsync(string bookingId, string status)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                await _bookingRepo.UpdateAsync(bookingId, booking);
            }
        }

        // Placeholder: Manage carpool (set seats)
        public async Task SetCarpoolSeatsAsync(Guid carpoolId, int seats)
        {
            var carpool = await _carpoolRepo.GetByIdAsync(carpoolId.ToString());
            if (carpool != null)
            {
                carpool.TotalSeats = seats;
                carpool.AvailableSeats = seats - carpool.ParticipantStudentIds.Count;
                await _carpoolRepo.UpdateAsync(carpoolId.ToString(), carpool);
            }
        }

        // Placeholder: View earnings
        public async Task<decimal> GetEarningsAsync(Guid providerId)
        {
            var payments = await _paymentRepo.FindAsync(p => p.UserId == providerId && p.Status == "Completed");
            decimal total = 0;
            foreach (var payment in payments)
                total += payment.Amount;
            return total;
        }

        // Placeholder: Send notification
        public async Task SendNotificationAsync(Notification notification)
        {
            await _notificationRepo.AddAsync(notification);
        }

        // Provider verifies a driver
        public async Task VerifyDriverAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver != null)
            {
                driver.IsVerified = true;
                await _driverRepo.UpdateAsync(driverId, driver);
            }
        }

        // Assign a trip to a driver
        public async Task AssignTripToDriverAsync(string driverId, Guid tripId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver != null)
            {
                if (!driver.AssignedTripIds.Contains(tripId))
                    driver.AssignedTripIds.Add(tripId);
                await _driverRepo.UpdateAsync(driverId, driver);
            }
        }
    }
} 