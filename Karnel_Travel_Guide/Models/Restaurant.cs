using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantID { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Cuisine { get; set; }

        [Required]
        [StringLength(50)]
        public string PriceRange { get; set; }

        [Range(0, 5)]
        public decimal Rating { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        public bool Availability { get; set; } = true;

        [StringLength(1000)]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}