using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Venues
    {
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

    }
}
