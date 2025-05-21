using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Events
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        [ForeignKey("Venues")]
        public int VenueId { get; set; }
        public Venues? Venue { get; set; }
    }
}
