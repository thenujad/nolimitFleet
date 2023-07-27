using Microsoft.EntityFrameworkCore;
using nolimit.Controllers;
using nolimit.Models;


namespace nolimit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }
       
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Allocation>Allocations { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<FuelRequest> FuelRequests { get; set; }
        public DbSet<VehicleMaintenance> VehicleMaintenance { get; set; }
        public DbSet<Image> Images { get; set; }

    }
}
