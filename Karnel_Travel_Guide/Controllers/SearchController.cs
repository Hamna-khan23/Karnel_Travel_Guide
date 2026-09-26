using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Karnel_Travel_Guide.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Results(string category, string location, decimal? price, int? quality)
        {
            List<SearchResultViewModel> searchResults = new List<SearchResultViewModel>();

            // 1. Tourist Spots Search
            if (category == "all" || category == "tourist_spot")
            {
                var spotsQuery = _context.TouristSpots.AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    spotsQuery = spotsQuery.Where(s => s.Name.Contains(location) || s.City.Contains(location) || s.City.Contains(location));
                }

                var spots = spotsQuery.Select(s => new SearchResultViewModel
                {
                    Id = s.SpotID,
                    Name = s.Name,
                    Category = "Tourist Spot",
                    Location = s.City + ", " + s.City,
                    Price = 0,
                    Rating = 5
                }).ToList();

                searchResults.AddRange(spots);
            }

            // 2. Hotels Search
            if (category == "all" || category == "hotel")
            {
                var hotelsQuery = _context.Hotels.AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    hotelsQuery = hotelsQuery.Where(h => h.Name.Contains(location) || h.City.Contains(location));
                }

                if (price.HasValue)
                {
                    hotelsQuery = hotelsQuery.Where(h => h.PricePerNight <= price.Value);
                }

                if (quality.HasValue)
                {
                    hotelsQuery = hotelsQuery.Where(h => h.Rating >= quality.Value);
                }

                var hotels = hotelsQuery.Select(h => new SearchResultViewModel
                {
                    Id = h.HotelID,
                    Name = h.Name,
                    Category = "Hotel",
                    Location = h.City,
                    Price = h.PricePerNight,
                    Rating = (int)h.Rating
                }).ToList();

                searchResults.AddRange(hotels);
            }

            // 3. Resorts Search
            if (category == "all" || category == "resort")
            {
                var resortsQuery = _context.Resorts.AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    resortsQuery = resortsQuery.Where(r => r.Name.Contains(location) || r.City.Contains(location));
                }

                if (price.HasValue)
                {
                    resortsQuery = resortsQuery.Where(r => r.PricePerNight <= price.Value);
                }

                if (quality.HasValue)
                {
                    resortsQuery = resortsQuery.Where(r => r.Rating >= quality.Value);
                }

                var resorts = resortsQuery.Select(r => new SearchResultViewModel
                {
                    Id = r.ResortID,
                    Name = r.Name,
                    Category = "Resort",
                    Location = r.City,
                    Price = r.PricePerNight,
                    Rating = (int)r.Rating
                }).ToList();

                searchResults.AddRange(resorts);
            }

            // 4. Restaurants Search
            if (category == "all" || category == "restaurant")
            {
                var restQuery = _context.Restaurants.AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    restQuery = restQuery.Where(r => r.Name.Contains(location) || r.City.Contains(location));
                }

                if (quality.HasValue)
                {
                    restQuery = restQuery.Where(r => r.Rating >= quality.Value);
                }

                var restaurants = restQuery.Select(r => new SearchResultViewModel
                {
                    Id = r.RestaurantID,
                    Name = r.Name,
                    Category = "Restaurant",
                    Location = r.City,
                    Price = 0,
                    Rating = (int)r.Rating
                }).ToList();

                searchResults.AddRange(restaurants);
            }

            // 5. Packages Search (TouristSpot ke sath join kar ke location check ki hai)
            if (category == "all" || category == "package")
            {
                var pkgQuery = _context.Packages.Include(p => p.TouristSpot).AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    pkgQuery = pkgQuery.Where(p => p.Title.Contains(location) ||
                                                   p.TouristSpot.City.Contains(location) ||
                                                   p.TouristSpot.Name.Contains(location));
                }

                if (price.HasValue)
                {
                    pkgQuery = pkgQuery.Where(p => p.Price <= price.Value);
                }

                var packages = pkgQuery.Select(p => new SearchResultViewModel
                {
                    Id = p.PackageID,
                    Name = p.Title,
                    Category = "Package",
                    Location = p.TouristSpot != null ? p.TouristSpot.City : "General",
                    Price = p.Price,
                    Rating = 5
                }).ToList();

                searchResults.AddRange(packages);
            }

            // 6. Travel Information Search
            if (category == "all" || category == "travel_info")
            {
                var travelQuery = _context.TravelInformations.AsQueryable();

                if (!string.IsNullOrEmpty(location))
                {
                    travelQuery = travelQuery.Where(t => t.TransportType.Contains(location) ||
                                                        t.ToCity.Contains(location) ||
                                                        t.Description.Contains(location));
                }

                if (price.HasValue)
                {
                    travelQuery = travelQuery.Where(t => t.Price <= price.Value);
                }

                var travels = travelQuery.Select(t => new SearchResultViewModel
                {
                    Id = t.TravelID,
                    Name = t.TransportType,
                    Category = "Travel Guide",
                    Location = t.ToCity,
                    Price = t.Price,
                    Rating = 5
                }).ToList();

                searchResults.AddRange(travels);
            }

            return View(searchResults);
        }
    }

    public class SearchResultViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
    }
}