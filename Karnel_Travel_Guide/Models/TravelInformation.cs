using System.ComponentModel.DataAnnotations;

namespace Karnel_Travel_Guide.Models
{
    public class TravelInformation
    {
        [Key]
        public int TravelID { get; set; }

        [Required]
        [StringLength(50)]
        public string TransportType { get; set; }
        // Bus / Train / Flight

        [Required]
        [StringLength(100)]
        public string FromCity { get; set; }

        [Required]
        [StringLength(100)]
        public string ToCity { get; set; }

        [Required]
        [StringLength(200)]
        public string Route { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public TimeSpan DepartureTime { get; set; }

        [Required]
        public TimeSpan ArrivalTime { get; set; }

        public bool Availability { get; set; } = true;

        [StringLength(1000)]
        public string Description { get; set; }

        
    }
}