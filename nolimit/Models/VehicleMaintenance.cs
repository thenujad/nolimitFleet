using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class VehicleMaintenance
    {
           [Key]
            public int Id { get; set; }
            public int VehicleId { get; set; }
            public int DriverId { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
            public string Detail { get; set; }
            public decimal Cost { get; set; }
            public int MeterReading { get; set; }
            public int FuelReading { get; set; }
            public int ImageId { get; set; } 


    }
}
