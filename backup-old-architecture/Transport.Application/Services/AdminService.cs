using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transport.Domain.Entities;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class AdminService
    {
        private readonly IMongoRepository<Provider> _providerRepo;
        private readonly IMongoRepository<Trip> _tripRepo;
        private readonly IMongoRepository<Booking> _bookingRepo;
        private readonly IMongoRepository<Notification> _notificationRepo;

        public AdminService(
            IMongoRepository<Provider> providerRepo,
            IMongoRepository<Trip> tripRepo,
            IMongoRepository<Booking> bookingRepo,
            IMongoRepository<Notification> notificationRepo)
        {
            _providerRepo = providerRepo;
            _tripRepo = tripRepo;
            _bookingRepo = bookingRepo;
            _notificationRepo = notificationRepo;
        }

        // Placeholder: Approve provider
        public async Task ApproveProviderAsync(string providerId)
        {
            var provider = await _providerRepo.GetByIdAsync(providerId);
            if (provider != null)
            {
                provider.IsActive = true;
                await _providerRepo.UpdateAsync(providerId, provider);
            }
        }

        // Placeholder: Monitor trips
        public async Task<IEnumerable<Trip>> GetOngoingTripsAsync()
        {
            return await _tripRepo.FindAsync(t => t.Status == "ongoing");
        }

        // Placeholder: Generate report (basic stats)
        public async Task<int> GetTotalBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllAsync();
            return bookings.Count();
        }

        // Placeholder: Send notification
        public async Task SendNotificationAsync(Notification notification)
        {
            await _notificationRepo.AddAsync(notification);
        }
    }
} 