namespace Transport.API.DTOs
{
    public class RFIDCardDto
    {
        public string CardNumber { get; set; }
        public Guid? UserId { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
    }
    public class TopUpDto
    {
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
    }
    public class TapInDto
    {
        public string CardNumber { get; set; }
        public decimal Fare { get; set; }
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string Stop { get; set; }
    }
    public class TapOutDto
    {
        public string CardNumber { get; set; }
        public Guid TripId { get; set; }
        public Guid StudentId { get; set; }
        public string Stop { get; set; }
        public bool Refund { get; set; }
    }
    public class VetTapOutDto
    {
        public string AttendanceId { get; set; }
        public bool Approve { get; set; }
        public string Reason { get; set; }
    }
} 