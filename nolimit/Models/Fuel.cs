using System;
using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Fuel
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; } // New property: VehicleId

        public int MeterReading { get; set; }
        public float Volume { get; set; }
        public decimal Cost { get; set; }
        public float FuelReading { get; set; }
        public DateTime DateTime { get; set; }
        public int ImageId { get; set; }
        public string Status { get; set; } // New property: Status
        public int FuelRequestId { get; set; } // New property: FuelRequestId
        public int DriverId { get; set; } // New property: DriverId
        public Vehicle Vehicle { get; set; }
        public Image Image { get; set; }
    }
}
