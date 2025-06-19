namespace Transport.API.DTOs
{
    public class RouteDto
    {
        public string Name { get; set; }
        public List<RouteStopDto> Stops { get; set; }
        public string Schedule { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class RouteStopDto
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public decimal PriceFromStart { get; set; }
    }
} 