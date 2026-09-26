using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();

        public ICollection<Feedback> Feedbacks { get; set; }
            = new List<Feedback>();
    }
}