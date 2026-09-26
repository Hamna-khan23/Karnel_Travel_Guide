using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class Resort
    {
        [Key]
        public int ResortID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [StringLength(1000)]
        public string Amenities { get; set; }

        [Range(0, 5)]
        public decimal Rating { get; set; }

        public bool Availability { get; set; } = true;

        [StringLength(1000)]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}