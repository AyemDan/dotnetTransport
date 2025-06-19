using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Transport.Domain.Entities.MongoDB;
using Transport.Domain.Interfaces.MongoDB;
using System.Linq;

namespace Transport.Application.Services
{
    public class RFIDCardService
    {
        private readonly IMongoRepository<RFIDCard> _cardRepo;
        private readonly IMongoRepository<TripAttendance> _attendanceRepo;
        private readonly IMongoRepository<PaymentLog> _paymentLogRepo;
        public RFIDCardService(IMongoRepository<RFIDCard> cardRepo, IMongoRepository<TripAttendance> attendanceRepo, IMongoRepository<PaymentLog> paymentLogRepo)
        {
            _cardRepo = cardRepo;
            _attendanceRepo = attendanceRepo;
            _paymentLogRepo = paymentLogRepo;
        }

        public async Task<IEnumerable<RFIDCard>> GetAllCardsAsync()
        {
            return await _cardRepo.GetAllAsync();
        }

        public async Task<RFIDCard?> GetCardByNumberAsync(string cardNumber)
        {
            var cards = await _cardRepo.FindAsync(c => c.CardNumber == cardNumber);
            return cards.FirstOrDefault();
        }

        public async Task AddCardAsync(RFIDCard card)
        {
            await _cardRepo.AddAsync(card);
        }

        public async Task TopUpAsync(string cardNumber, decimal amount)
        {
            var card = await GetCardByNumberAsync(cardNumber);
            if (card != null && card.Status == "Active")
            {
                card.Balance += amount;
                await _cardRepo.UpdateAsync(card.Id.ToString(), card);
            }
        }

        public async Task<bool> TapInAsync(string cardNumber, decimal fare, Guid tripId, Guid studentId, string stop)
        {
            var card = await GetCardByNumberAsync(cardNumber);
            if (card != null && card.Status == "Active" && card.Balance >= fare)
            {
                card.Balance -= fare;
                await _cardRepo.UpdateAsync(card.Id.ToString(), card);
                // Log attendance
                var attendance = new TripAttendance
                {
                    TripId = tripId,
                    StudentId = studentId,
                    TapInTime = DateTime.UtcNow,
                    TapInStop = stop
                };
                await _attendanceRepo.AddAsync(attendance);
                // Log payment
                var paymentLog = new PaymentLog
                {
                    UserId = studentId,
                    TripId = tripId,
                    Amount = fare,
                    Status = "Completed",
                    Type = "TapIn",
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentLogRepo.AddAsync(paymentLog);
                return true;
            }
            return false;
        }

        public async Task<(bool, string)> TapOutAsync(Guid tripId, Guid studentId, string stop, bool refund = false)
        {
            var attendances = await _attendanceRepo.FindAsync(a => a.TripId == tripId && a.StudentId == studentId);
            var attendance = attendances.FirstOrDefault();
            if (attendance == null)
                return (false, "No tap-in record found for this trip/student.");
            if (attendance.TapOutTime != null)
                return (false, "Already tapped out.");
            attendance.TapOutTime = DateTime.UtcNow;
            attendance.TapOutStop = stop;
            attendance.IsTapOutVetted = false;
            await _attendanceRepo.UpdateAsync(attendance.Id.ToString(), attendance);
            // Optionally process refund
            if (refund)
            {
                var logs = await _paymentLogRepo.FindAsync(l => l.UserId == studentId && l.TripId == tripId && l.Type == "TapIn" && l.Status == "Completed");
                var log = logs.OrderByDescending(l => l.CreatedAt).FirstOrDefault();
                if (log != null)
                {
                    log.Status = "Refunded";
                    log.Type = "TapOutRefund";
                    await _paymentLogRepo.UpdateAsync(log.Id.ToString(), log);
                }
            }
            return (true, "Tap-out recorded, pending vetting.");
        }

        public async Task VetTapOutAsync(string attendanceId, bool approve, string? reason = null)
        {
            var attendance = await _attendanceRepo.GetByIdAsync(attendanceId);
            if (attendance != null && attendance.TapOutTime != null)
            {
                attendance.IsTapOutVetted = approve;
                attendance.VettingReason = reason;
                await _attendanceRepo.UpdateAsync(attendanceId, attendance);
            }
        }

        public async Task<decimal?> GetBalanceAsync(string cardNumber)
        {
            var card = await GetCardByNumberAsync(cardNumber);
            return card?.Balance;
        }
    }
} 