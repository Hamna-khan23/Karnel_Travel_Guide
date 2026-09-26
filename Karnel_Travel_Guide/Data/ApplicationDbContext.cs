using Karnel_Travel_Guide.Models;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }

        public DbSet<TouristSpot> TouristSpots { get; set; }

        public DbSet<TravelInformation> TravelInformations { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<Resort> Resorts { get; set; }

        public DbSet<Package> Packages { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

        public DbSet<Promotion> Promotions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // USER -> BOOKING
            // ==========================================

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // USER -> FEEDBACK
            // ==========================================

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // TOURIST SPOT -> PACKAGE
            // ==========================================

            modelBuilder.Entity<Package>()
                .HasOne(p => p.TouristSpot)
                .WithMany(t => t.Packages)
                .HasForeignKey(p => p.SpotID)
                .OnDelete(DeleteBehavior.NoAction);
            // ==========================================
            // TRAVEL INFORMATION -> PACKAGE
            // ==========================================

            modelBuilder.Entity<Package>()
                .HasOne(p => p.TravelInformation)
                .WithMany()
                .HasForeignKey(p => p.TravelID)
                .OnDelete(DeleteBehavior.NoAction);
            // ==========================================
            // PACKAGE -> BOOKING
            // ==========================================

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Package)
                .WithMany()
                .HasForeignKey(b => b.PackageID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // HOTEL -> BOOKING
            // ==========================================

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Hotel)
                .WithMany()
                .HasForeignKey(b => b.HotelID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // RESORT -> BOOKING
            // ==========================================

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Resort)
                .WithMany()
                .HasForeignKey(b => b.ResortID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // TRAVEL INFORMATION -> BOOKING
            // ==========================================

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TravelInformation)
                .WithMany()
                .HasForeignKey(b => b.TravelID)
                .OnDelete(DeleteBehavior.NoAction);


            // ==========================================
            // DECIMAL PRECISION
            // ==========================================

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Hotel>()
                .Property(h => h.PricePerNight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Hotel>()
                .Property(h => h.Rating)
                .HasPrecision(2, 1);

            modelBuilder.Entity<Package>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Package>()
                .Property(p => p.Discount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Promotion>()
                .Property(p => p.Discount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Resort>()
                .Property(r => r.PricePerNight)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Resort>()
                .Property(r => r.Rating)
                .HasPrecision(2, 1);

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Rating)
                .HasPrecision(2, 1);

            modelBuilder.Entity<TravelInformation>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
}