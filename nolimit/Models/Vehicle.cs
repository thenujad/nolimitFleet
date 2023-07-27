using System;
using System.ComponentModel.DataAnnotations;

namespace nolimit.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Display(Name = "License Plate Number")]
        public string LicensePlateNumber { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; }

        [Display(Name = "Current Mileage")]
        public float CurrentMileage { get; set; }

        [Display(Name = "Owner's Name")]
        public string OwnerName { get; set; }

        [Display(Name = "Insurance Provider")]
        public string InsuranceProvider { get; set; }

        [Display(Name = "Insurance Policy Number")]
        public string InsurancePolicyNumber { get; set; }

        [Display(Name = "Policy Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime? PolicyExpirationDate { get; set; }

        [Display(Name = "Fuel Level")]
        public float FuelLevel { get; set; }

        public bool Availability { get; set; }

       


    }
}
