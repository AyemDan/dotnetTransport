namespace Transport.API.DTOs
{
    public class PaymentInfoDto
    {
        public Guid UserId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
} 