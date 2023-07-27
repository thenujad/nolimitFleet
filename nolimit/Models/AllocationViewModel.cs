namespace nolimit.Models
{
    public class AllocationViewModel
    {
        public List<Request> Requests { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<Allocation> Allocations { get; set; }
    }
}
