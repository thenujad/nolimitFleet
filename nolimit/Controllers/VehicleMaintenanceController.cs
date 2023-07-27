using Microsoft.AspNetCore.Mvc;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    public class VehicleMaintenanceController : BaseController
    {
        private readonly AppDbContext _db;

        public VehicleMaintenanceController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var availableVehicles = _db.Vehicles.Where(v => v.Availability).ToList();
            var maintenanceRequests = _db.VehicleMaintenance.Where(m => m.Status != "Completed").ToList();
            var availableDrivers = _db.Drivers.Where(d => d.Availability).ToList();

            var viewModel = new MaintenanceViewModel
            {
                AvailableVehicles = availableVehicles,
                VehicleMaintenances = maintenanceRequests,
                AvailableDrivers = availableDrivers // Make sure to populate the AvailableDrivers property
            };

            return View("~/Views/Navigation/Vehicle/Maintenance.cshtml", viewModel);
        }


        [HttpPost]
        public IActionResult SubmitMaintenanceRequest(int vehicleId, string details, string accident)
        {
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle != null && vehicle.Availability)
            {
                var isAccident = accident == "true";
                var type = isAccident ? "Accident" : "Maintenance";

                var maintenance = new VehicleMaintenance
                {
                    VehicleId = vehicleId,
                    Status = "Pending",
                    Type = type,
                    Detail = details
                };

                _db.VehicleMaintenance.Add(maintenance);
                vehicle.Availability = false; // Set vehicle availability to false
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Handle the case where the vehicle doesn't exist or is not available
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult StartMaintenance(int maintenanceId, int driverId)
        {
            var maintenance = _db.VehicleMaintenance.Find(maintenanceId);
            var driver = _db.Drivers.Find(driverId);
            

           
                maintenance.Status = "Started";
                maintenance.DriverId = driverId;

                var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == maintenance.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Availability = false;
                }

                driver.Availability = false;

                _db.SaveChanges();

                return RedirectToAction("Index");
            

           
        }




        [HttpPost]
        public IActionResult EndMaintenance(int maintenanceId, int meterReading, int fuelReading, decimal cost, IFormFile image)
        {
            var maintenance = _db.VehicleMaintenance.Find(maintenanceId);

            var driverId = maintenance.DriverId;
            var vehicleId = maintenance.VehicleId;

            // Retrieve the driver and vehicle using the driverId and vehicleId
            var driver = _db.Drivers.Find(driverId);
            var vehicle = _db.Vehicles.Find(vehicleId);

            if (driver != null && vehicle != null)
            {
                maintenance.Status = "Completed";
                maintenance.MeterReading = meterReading;
                maintenance.FuelReading = fuelReading;
                maintenance.Cost = cost;

                // Check if an image file was uploaded
                if (image != null && image.Length > 0)
                {
                    // Save the image file to the database or file storage system
                    var savedImage = SaveImageToStorage(image, maintenance.Type);

                    // Associate the image with the maintenance record
                    maintenance.ImageId = savedImage.Id;
                }

                _db.SaveChanges(); // Save changes to the maintenance record

                // Update the availability of the vehicle and driver
                vehicle.Availability = true;
                driver.Availability = true;

                _db.SaveChanges(); // Save changes to the vehicle and driver records

                // Update the meter reading and fuel reading in the vehicle table
                vehicle.CurrentMileage = (float)meterReading;
                vehicle.FuelLevel = (float)fuelReading;

                _db.SaveChanges(); // Save changes to the vehicle record

                return RedirectToAction("Index");
            }

            // Handle the scenario when maintenance, driver, or vehicle is not found
            // Redirect or return appropriate response
            return RedirectToAction("Index");
        }

        private Image SaveImageToStorage(IFormFile image, string type)
        {
            // Convert the uploaded image to byte array
            byte[] imageData;
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                imageData = memoryStream.ToArray();
            }

            // Save the image data to the database or file storage system
            var savedImage = new Image
            {
                ImageData = imageData,
                Type = type
            };

            _db.Images.Add(savedImage);
            _db.SaveChanges();

            return savedImage;
        }



        [HttpPost]
        public IActionResult RejectMaintenanceRequest(int maintenanceRequestId)
        {
            var maintenanceRequest = _db.VehicleMaintenance.FirstOrDefault(m => m.Id == maintenanceRequestId);
            if (maintenanceRequest != null)
            {
                maintenanceRequest.Status = "Rejected";
                _db.SaveChanges();

                // Set vehicle availability to true
                var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == maintenanceRequest.VehicleId);
                if (vehicle != null)
                {
                    vehicle.Availability = true;
                    _db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Handle the case where the maintenance request doesn't exist
            return RedirectToAction("Index");
        }



    }
}
