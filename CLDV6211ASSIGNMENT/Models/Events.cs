using System.ComponentModel.DataAnnotations;

namespace CLDV6211ASSIGNMENT.Models
{
    public class Events
    {
        public int Id { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        public int? Id { get; set; }
        public Venues? venues { get; set; }
    }
}
