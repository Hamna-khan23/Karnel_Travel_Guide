using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Karnel_Travel_Guide.Models
{
    public class Package
    {
        [Key]
        public int PackageID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [Required]
        [Range(1, 365)]
        public int Duration { get; set; }
        [Required]
        [Range(0, 365)]
        public int Nights { get; set; } // Nights count added here

        [Required]
        [Range(1, 100)]
        public int TotalPersons { get; set; } = 2;

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string? Category { get; set; } // e.g., Family, Honeymoon, Adventure

        [StringLength(1000)]
        public string? Inclusions { get; set; }

        [StringLength(1000)]
        public string? Exclusions { get; set; }

        public bool IsFeatured { get; set; } = false;

        [Required]
        public int SpotID { get; set; }

        [ForeignKey("SpotID")]
        public TouristSpot? TouristSpot { get; set; }

        // Hotel Link
        public int? HotelID { get; set; }

        [ForeignKey("HotelID")]
        public Hotel? Hotel { get; set; }

        // Resort Link
        public int? ResortID { get; set; }

        [ForeignKey("ResortID")]
        public Resort? Resort { get; set; }

        // Restaurant Link
        public int? RestaurantID { get; set; }

        [ForeignKey("RestaurantID")]
        public Restaurant? Restaurant { get; set; }

        public bool Availability { get; set; } = true;

        public string? ImageUrl { get; set; }

        public int? TravelID { get; set; }

        [ForeignKey(nameof(TravelID))]
        public TravelInformation? TravelInformation { get; set; }

        // Navigation Properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}