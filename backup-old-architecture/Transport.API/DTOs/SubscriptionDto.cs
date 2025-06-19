namespace Transport.API.DTOs
{
    public class SubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid RouteId { get; set; }
        public string Period { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
} 