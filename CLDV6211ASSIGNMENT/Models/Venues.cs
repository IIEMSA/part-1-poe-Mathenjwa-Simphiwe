using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Venues
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]

        public IFormFile ImageFile { get; set; }

        public List<Bookings> Bookings { get; set; }

        [ForeignKey("EventTypeID")]
        public int? EventTypeID{ get; set; }
        public EventType EventType { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

    }
}
