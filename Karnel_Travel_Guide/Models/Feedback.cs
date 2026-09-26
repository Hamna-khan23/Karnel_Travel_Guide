using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Karnel_Travel_Guide.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        // User who submitted the feedback
        [Required]
        public int UserID { get; set; }

        // Booking for which feedback is given
        [Required]
        public int BookingID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comments { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // Navigation Properties

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("BookingID")]
        public Booking Booking { get; set; }
    }
}