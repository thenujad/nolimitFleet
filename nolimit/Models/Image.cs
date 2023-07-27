using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
    }
}
