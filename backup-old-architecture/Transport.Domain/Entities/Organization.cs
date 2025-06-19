using System;
using System.Collections.Generic;

namespace Transport.Domain.Entities
{
    public class Organization
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Add more fields as needed (logo, website, etc.)
    }
} 