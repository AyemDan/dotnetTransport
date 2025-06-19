using System;

namespace Transport.Shared.DTOs;

public class LocationUpdateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
} 