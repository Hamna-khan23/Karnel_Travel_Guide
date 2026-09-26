using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Karnel_Travel_Guide.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        // User who made the booking
        [Required]
        public int UserID { get; set; }

        // Module-specific Foreign Keys (Optional)
        public int? PackageID { get; set; }
        public int? HotelID { get; set; }
        public int? ResortID { get; set; }
        public int? TravelID { get; set; }
        public int? TouristSpotID { get; set; }
        public int? RestaurantID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Range(1, 100)]
        public int Persons { get; set; }

        [Required]
        [StringLength(20)]
        public string? Status { get; set; } = "Pending";
        [StringLength(100)]
        public string? PriceRange { get; set; }


        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [StringLength(1000)]
        public string? SpecialRequest { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(1);

        public int Nights { get; set; } = 1;


        // ==========================================
        // Navigation Properties
        // ==========================================

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("PackageID")]
        public Package Package { get; set; }

        [ForeignKey("HotelID")]
        public Hotel Hotel { get; set; }

        [ForeignKey("ResortID")]
        public Resort Resort { get; set; }
        [ForeignKey("TravelID")]
        public TravelInformation TravelInformation { get; set; }

        [ForeignKey("TouristSpotID")]
        public TouristSpot TouristSpot { get; set; }

        [ForeignKey("RestaurantID")]
        public Restaurant Restaurant { get; set; }
    }
}