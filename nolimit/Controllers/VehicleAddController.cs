using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    public class VehicleAddController : BaseController
    {
        private readonly AppDbContext _db;

        public VehicleAddController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
           : base(httpContextAccessor)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Vehicle> obj_vehicleList = _db.Vehicles;
            return View("~/Views/Navigation/Vehicle/Vehicle_List.cshtml", obj_vehicleList);
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            // Display the form to add a new vehicle
            return View("~/Views/Navigation/Vehicle/AddVehicle.cshtml");
        }


        [HttpPost]
        public IActionResult AddVehicle(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                // Check if the LicensePlateNumber is already taken
                bool isDuplicateLicensePlate = _db.Vehicles.Any(v => v.LicensePlateNumber == vehicle.LicensePlateNumber);
                if (isDuplicateLicensePlate)
                {
                    ModelState.AddModelError("LicensePlateNumber", "License plate number is already taken.");
                    return View("~/Views/Navigation/Vehicle/AddVehicle.cshtml", vehicle); // Redirect back to the "AddVehicle" action
                }
                // Create a new Vehicle object using the form values
                Vehicle newVehicle = new Vehicle
                {
                    RegistrationNumber = vehicle.RegistrationNumber,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    LicensePlateNumber = vehicle.LicensePlateNumber,
                    VehicleType = vehicle.VehicleType,
                    FuelType = vehicle.FuelType,
                    CurrentMileage = vehicle.CurrentMileage,
                    OwnerName = vehicle.OwnerName,
                    InsuranceProvider = vehicle.InsuranceProvider,
                    InsurancePolicyNumber = vehicle.InsurancePolicyNumber,
                    PolicyExpirationDate = vehicle.PolicyExpirationDate,
                    FuelLevel = vehicle.FuelLevel,
                    Availability = true
                };

                // Save the newVehicle object to the database using your DbContext
                _db.Vehicles.Add(newVehicle);
                _db.SaveChanges();

                // Redirect to the desired action or view
                return RedirectToAction("Index", vehicle);
            }

            // If the model state is not valid, return the view with validation errors
            return View("~/Views/Navigation/Vehicle/Vehicle_List.cshtml", vehicle);
        }


        [HttpGet]
        public async Task<IActionResult> EditVehicle(int id)
        {
            // Find the existing vehicle in the database
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            // Pass the existing vehicle as the model to the view
            return View("~/Views/Navigation/Vehicle/EditVehicle.cshtml", vehicle);
        }


        [HttpPost]
        public async Task<IActionResult> EditVehicle(int id, Vehicle updatedVehicle)
        {
            if (id != updatedVehicle.Id)
            {
                return BadRequest();
            }

              try
                {
                    // Find the existing vehicle in the database
                    var vehicle = await _db.Vehicles.FindAsync(id);
                    if (vehicle == null)
                    {
                        return NotFound();
                    }

                    // Update the vehicle properties with the values from updatedVehicle
                  
                    vehicle.Brand = updatedVehicle.Brand;
                    vehicle.Model = updatedVehicle.Model;
                    vehicle.Year = updatedVehicle.Year;
       
                    vehicle.VehicleType = updatedVehicle.VehicleType;
                    vehicle.FuelType = updatedVehicle.FuelType;
                    vehicle.CurrentMileage = updatedVehicle.CurrentMileage;
                    vehicle.OwnerName = updatedVehicle.OwnerName;
                    vehicle.InsuranceProvider = updatedVehicle.InsuranceProvider;
                    vehicle.InsurancePolicyNumber = updatedVehicle.InsurancePolicyNumber;
                    vehicle.PolicyExpirationDate = updatedVehicle.PolicyExpirationDate;
                    vehicle.FuelLevel = updatedVehicle.FuelLevel;

                    // Save the changes to the database
                    await _db.SaveChangesAsync();

                    // Redirect to the desired action or view
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            // If the model state is not valid, return the view with validation errors
            return View("~/Views/Navigation/Vehicle/EditVehicle.cshtml", updatedVehicle);
        }
        private bool VehicleExists(int id)
        {
            return _db.Vehicles.Any(v => v.Id == id);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            // Find the existing vehicle in the database
            var vehicle = await _db.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            if (!vehicle.Availability)
            {
                // Vehicle is not available for deletion
                return BadRequest("Cannot delete the vehicle. It is not available.");
            }

            // Delete the vehicle with the specified id
            _db.Vehicles.Remove(vehicle);
            await _db.SaveChangesAsync();

            // Redirect to the desired action or view
            return RedirectToAction("Index");
        }



    }
}