using System;
using System.Collections.Generic;

namespace Transport.Shared.DTOs;

public class RouteDto
{
    public string Name { get; set; } = string.Empty;
    public List<RouteStopDto>? Stops { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class RouteStopDto
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal PriceFromStart { get; set; }
} 