using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;

namespace Transport.Application.Services
{
    public class StudentService
    {
        private readonly IMongoRepository<Trip> _tripRepo;
        private readonly IMongoRepository<Booking> _bookingRepo;
        private readonly IMongoRepository<Carpool> _carpoolRepo;
        private readonly IMongoRepository<PaymentLog> _paymentRepo;
        private readonly IMongoRepository<Notification> _notificationRepo;

        public StudentService(
            IMongoRepository<Trip> tripRepo,
            IMongoRepository<Booking> bookingRepo,
            IMongoRepository<Carpool> carpoolRepo,
            IMongoRepository<PaymentLog> paymentRepo,
            IMongoRepository<Notification> notificationRepo)
        {
            _tripRepo = tripRepo;
            _bookingRepo = bookingRepo;
            _carpoolRepo = carpoolRepo;
            _paymentRepo = paymentRepo;
            _notificationRepo = notificationRepo;
        }

        // Placeholder: Trip planning logic
        public async Task<IEnumerable<(Trip trip, int boardingOrder, int dropOffOrder, decimal price)>> SuggestTripsAsync(Guid studentId, string boardingStop, string dropOffStop)
        {
            var trips = await _tripRepo.GetAllAsync();
            var results = new List<(Trip, int, int, decimal)>();
            foreach (var trip in trips)
            {
                var routeStops = trip.Stops;
                int boardingIdx = routeStops.FindIndex(s => s.Location == boardingStop);
                int dropOffIdx = routeStops.FindIndex(s => s.Location == dropOffStop);
                if (boardingIdx >= 0 && dropOffIdx > boardingIdx)
                {
                    decimal price = routeStops[dropOffIdx].PriceFromStart - routeStops[boardingIdx].PriceFromStart;
                    results.Add((trip, boardingIdx, dropOffIdx, price));
                }
            }
            return results;
        }

        // Placeholder: Book a trip (direct or carpool)
        public async Task BookTripAsync(Booking booking)
        {
            await _bookingRepo.AddAsync(booking);
        }

        // Placeholder: Join a carpool
        public async Task JoinCarpoolAsync(Guid carpoolId, Guid studentId)
        {
            var carpool = await _carpoolRepo.GetByIdAsync(carpoolId.ToString());
            if (carpool != null && carpool.AvailableSeats > 0)
            {
                carpool.ParticipantStudentIds.Add(studentId);
                carpool.AvailableSeats--;
                await _carpoolRepo.UpdateAsync(carpoolId.ToString(), carpool);
            }
        }

        // Placeholder: Simulate payment
        public async Task SimulatePaymentAsync(PaymentLog payment)
        {
            payment.Status = "Completed";
            await _paymentRepo.AddAsync(payment);
        }

        // Placeholder: Send notification
        public async Task SendNotificationAsync(Notification notification)
        {
            await _notificationRepo.AddAsync(notification);
        }
    }
} 