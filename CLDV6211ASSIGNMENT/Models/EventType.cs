using System.ComponentModel.DataAnnotations;

namespace CLDV6211ASSIGNMENT.Models
{
    public class EventType
    {
      public int  EventTypeID { get; set; }

        [Required]
        public string name { get; set; }
    }
}
