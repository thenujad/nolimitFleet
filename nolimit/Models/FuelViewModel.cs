namespace nolimit.Models
{
    public class FuelViewModel
    {
        public List<Vehicle> Vehicles { get; set; }
        public List<Fuel> FuelHistory { get; set; }
        public List<FuelRequest> FuelRequestHistory { get; set; }
        public List<Driver> Drivers { get; set; }
    }
}
