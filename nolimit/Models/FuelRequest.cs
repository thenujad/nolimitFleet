using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class FuelRequest
    {
        [Key]
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public float Volume { get; set; }
        public decimal Budget { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public Vehicle Vehicle { get; set; }
    }


}
