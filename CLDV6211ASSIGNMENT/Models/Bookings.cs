using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Bookings
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        public Events Event { get; set; }

        [Required]
        public int VenueId { get; set; }
        public Venues Venue { get; set; }

        public DateTime BookingDate { get; set; }

    }
}
