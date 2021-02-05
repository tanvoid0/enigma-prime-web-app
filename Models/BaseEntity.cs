using System;

namespace enigma_prime.Data
{
    // Extends the timestamp fields to other classes
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}