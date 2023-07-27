using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
        public string Department { get; set; }
       
        public string ContactNo { get; set; }
        public string ApprovedBy { get; set; }
        public string Location { get; set; }
        public string Purpose { get; set; }
        public int Passengers { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }

        public User User { get; set; }


    }
}
