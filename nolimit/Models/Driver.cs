using System;
using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [Display(Name = "License Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? LicenseExpirationDate { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        public bool Availability { get; set; }
    }
}
