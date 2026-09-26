using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class TouristSpot
    {
        [Key]
        public int SpotID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}