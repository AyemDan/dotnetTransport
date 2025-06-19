using System;

namespace Transport.Shared.DTOs;

public class BookingDto
{
    public Guid UserId { get; set; }
    public Guid TripId { get; set; }
    public string BoardingStop { get; set; } = string.Empty;
    public string DropOffStop { get; set; } = string.Empty;
    public decimal SegmentPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; } = string.Empty;
} 