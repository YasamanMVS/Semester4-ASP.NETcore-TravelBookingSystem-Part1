using GBC_Travel_Group_50.Models;
using Microsoft.EntityFrameworkCore;

public class TravelBookingContext : DbContext
{
    public TravelBookingContext(DbContextOptions<TravelBookingContext> options) : base(options)
    {
    }

    public DbSet<Flight> Flights { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<CarRental> CarRentals { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}
