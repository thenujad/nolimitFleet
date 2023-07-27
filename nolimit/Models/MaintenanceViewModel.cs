namespace nolimit.Models
{
    public class MaintenanceViewModel
    {
        public List<Vehicle> AvailableVehicles { get; set; }
        public List<Driver> AvailableDrivers { get; set; }  // Add this property
        public List<VehicleMaintenance> VehicleMaintenances { get; set;}
    }
}
