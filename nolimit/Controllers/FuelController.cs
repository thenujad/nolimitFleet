using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    public class FuelController : BaseController
    {
        private readonly AppDbContext _db;

        public FuelController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            FuelViewModel viewModel = new FuelViewModel
            {
                Vehicles = _db.Vehicles.ToList(),
                Drivers = _db.Drivers.ToList(),
                FuelHistory = _db.Fuel.ToList(),
                FuelRequestHistory = _db.FuelRequests.ToList()
            };

            return View("~/Views/Navigation/Fuel/Fuel.cshtml",viewModel);
        }




        [HttpPost]
        public IActionResult SubmitRequest(int vehicleId, float volume, decimal budget)
        {
            // Find the selected vehicle
            Vehicle selectedVehicle = _db.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

            if (selectedVehicle != null && selectedVehicle.Availability)
            {
                // Update the availability of the selected vehicle to false
                selectedVehicle.Availability = false;
                _db.Vehicles.Update(selectedVehicle);
                _db.SaveChanges();

                // Create a new fuel request
                FuelRequest fuelRequest = new FuelRequest
                {
                    VehicleId = vehicleId,
                    Volume = volume,
                    Budget = budget,
                    RequestDate = DateTime.Now,
                    Status = "Pending"
                };

                // Save the fuel request to the database
                _db.FuelRequests.Add(fuelRequest);
                _db.SaveChanges();

               
            }

            // Redirect to the fuel history page
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ConfirmRequest(int requestId, int driverId)
        {
            // Find the fuel request
            FuelRequest fuelRequest = _db.FuelRequests.Find(requestId);
            if (fuelRequest == null)
            {
                return NotFound();
            }

            // Find the driver
            Driver driver = _db.Drivers.Find(driverId);
            if (driver == null)
            {
                return NotFound();
            }

            // Update the driver's availability
            driver.Availability = false;
            _db.Drivers.Update(driver);
            _db.SaveChanges();

            // Create a new fuel entry
            Fuel fuelEntry = new Fuel
            {
                VehicleId = fuelRequest.VehicleId,
                DriverId = driver.Id,
                FuelRequestId = requestId,
                DateTime = DateTime.Now,
                Status = "Fueling"
            };

            // Add the fuel entry to the database
            _db.Fuel.Add(fuelEntry);           
            _db.SaveChanges();

            // Update the fuel request status
            fuelRequest.Status = "Assigned";
            _db.FuelRequests.Update(fuelRequest);
            _db.SaveChanges();

            // Redirect to the fuel history page or any other appropriate page
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult RejectRequest(int requestId)
        {
            // Find the fuel request by its ID
            FuelRequest fuelRequest = _db.FuelRequests.Find(requestId);

            if (fuelRequest != null)
            {
                // Find the associated vehicle
                Vehicle vehicle = _db.Vehicles.Find(fuelRequest.VehicleId);

                if (vehicle != null)
                {
                    // Update the vehicle availability
                    vehicle.Availability = true;
                    _db.Vehicles.Update(vehicle);
                }

                // Update the status of the fuel request
                fuelRequest.Status = "Rejected";
                _db.FuelRequests.Update(fuelRequest);

                // Save the changes to the database within a single transaction
                _db.SaveChanges();
            }

            // Redirect to the fuel request history page
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitRefueling(int fuelId, int vehicleId, float currentMeterReading, float fuelReading, float volume, decimal cost, IFormFile receipt)
        {
            // Find the vehicle by its ID
            Vehicle vehicle = _db.Vehicles.Find(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

            // Find the fuel entry by fuelId
            Fuel fuelEntry = _db.Fuel.FirstOrDefault(f => f.Id == fuelId);
            if (fuelEntry == null)
            {
                return NotFound();
            }

            // Check if a receipt image is provided
            if (receipt != null && receipt.Length > 0)
            {
                // Read the image data into a byte array
                byte[] imageData;
                using (var stream = new MemoryStream())
                {
                    await receipt.CopyToAsync(stream);
                    imageData = stream.ToArray();
                }

                // Create a new image record in the database
                Image imageRecord = new Image
                {
                    Type = "Fuel",
                    ImageData = imageData
                };

                // Add the image record to the database
                _db.Images.Add(imageRecord);
                _db.SaveChanges();

                // Associate the image with the fuel entry
                fuelEntry.ImageId = imageRecord.Id;
            }

            // Update the vehicle's current meter reading and fuel reading
            vehicle.CurrentMileage = currentMeterReading;
            vehicle.FuelLevel = fuelReading;
            vehicle.Availability = true; // Set vehicle availability to true
            _db.Vehicles.Update(vehicle);

            // Find the driver associated with the fuel entry
            Driver driver = _db.Drivers.FirstOrDefault(d => d.Id == fuelEntry.DriverId);
            if (driver != null)
            {
                // Update the driver's availability to true
                driver.Availability = true;
                _db.Drivers.Update(driver);
            }
            // Update the fuel entry with the new values
            fuelEntry.MeterReading = (int)currentMeterReading;
            fuelEntry.FuelReading = fuelReading;
            fuelEntry.Volume = volume;
            fuelEntry.Cost = cost;
            fuelEntry.DateTime = DateTime.Now;
            fuelEntry.Status = "Completed";
           

            // Save the changes to the database
            _db.Fuel.Update(fuelEntry);
            _db.SaveChanges();

            // Redirect to the fuel history page
            return RedirectToAction("Index");
        }



        public IActionResult ViewReceipt(int id)
        {
            // Find the fuel entry by its ID
            Fuel fuelEntry = _db.Fuel.FirstOrDefault(f => f.Id == id);
            if (fuelEntry == null || fuelEntry.ImageId == null)
            {
                return NotFound();
            }

            // Retrieve the image record from the database
            Image imageRecord = _db.Images.FirstOrDefault(i => i.Id == fuelEntry.ImageId);
            if (imageRecord == null)
            {
                return NotFound();
            }

            // Return the image file
            return File(imageRecord.ImageData, "image/jpeg"); // Adjust the content type if necessary
        }


    }
}
