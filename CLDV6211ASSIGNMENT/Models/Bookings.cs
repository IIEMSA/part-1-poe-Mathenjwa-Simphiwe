using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Bookings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Event")]  // Points to navigation property
       
        public int EventId { get; set; }
        public Events Event { get; set; }

        [Required]
        [ForeignKey("Venues")]
        public int VenueId { get; set; }
        public Venues Venue { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public string Status { get; set; } = "Pending";

    }
}
