namespace Transport.API.DTOs
{
    public class BookingDto
    {
        public Guid StudentId { get; set; }
        public Guid? ProviderId { get; set; }
        public Guid TripId { get; set; }
        public Guid? CarpoolId { get; set; }
        public string Status { get; set; }
        public int BoardingStopOrder { get; set; }
        public int DropOffStopOrder { get; set; }
        public decimal SegmentPrice { get; set; }
    }
} 