using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class ContactMessage
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}