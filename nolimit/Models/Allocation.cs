using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Allocation
    {
        [Key]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string Note { get; set; }
        public DateTime DepartureDate { get; set; }
        public int MeterReadingStart { get; set; }
        public float FuelAmountStart { get; set; }
        public DateTime StartTime { get; set; }
        public int MeterReadingEnd { get; set; }
        public float FuelAmountEnd { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Driver Driver { get; set; }
        public Request Request { get; set; }

    }

}
