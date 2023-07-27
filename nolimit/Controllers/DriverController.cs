using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    public class DriverController : BaseController
    {
        private readonly AppDbContext _db;

        public DriverController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var driverList = await _db.Drivers.ToListAsync();
            var driversOnRoad = _db.Allocations.Where(a => a.Status == "Started")
                                               .Select(a => a.Driver)
                                               .ToList();

            ViewBag.DriversOnRoad = driversOnRoad;

            return View("~/Views/Navigation/Driver/Driver_List.cshtml", driverList);
        }



        [HttpGet]
        public IActionResult AddDriver()
        {
            return View("~/Views/Navigation/Driver/AddDriver.cshtml");
        }

        [HttpPost]
        public IActionResult AddDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
                Driver newDriver = new Driver
                {
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    DateOfBirth = driver.DateOfBirth,
                    Gender = driver.Gender,
                    ContactNumber = driver.ContactNumber,
                    EmailAddress = driver.EmailAddress,
                    LicenseNumber = driver.LicenseNumber,
                    LicenseExpirationDate = driver.LicenseExpirationDate,
                    Address = driver.Address,
                    City = driver.City,
                    State = driver.State,
                    ZipCode = driver.ZipCode,
                    Country = driver.Country,
                    DateCreated = DateTime.Now,
                    Availability = true
                };

                _db.Drivers.Add(newDriver);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("~/Views/Navigation/Driver/AddDriver.cshtml", driver);
        }

        [HttpGet]
        public async Task<IActionResult> EditDriver(int id)
        {
            // Find the existing vehicle in the database
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            // Pass the existing vehicle as the model to the view
            return View("~/Views/Navigation/Driver/EditDiver.cshtml", driver);
        }

        [HttpPost]
        public async Task<IActionResult> EditDriver(int id, Driver updatedDriver)
        {
            if (id != updatedDriver.Id)
            {
                return BadRequest();
            }

            try
            {
                // Find the existing driver in the database
                var driver = await _db.Drivers.FindAsync(id);
                if (driver == null)
                {
                    return NotFound();
                }

                driver.FirstName = updatedDriver.FirstName;
                driver.LastName = updatedDriver.LastName;
                driver.DateOfBirth = updatedDriver.DateOfBirth;
                driver.Gender = updatedDriver.Gender;
                driver.ContactNumber = updatedDriver.ContactNumber;
                driver.EmailAddress = updatedDriver.EmailAddress;
                driver.LicenseNumber = updatedDriver.LicenseNumber;
                driver.LicenseExpirationDate = updatedDriver.LicenseExpirationDate;
                driver.Address = updatedDriver.Address;
                driver.City = updatedDriver.City;
                driver.State = updatedDriver.State;
                driver.ZipCode = updatedDriver.ZipCode;
                driver.Country = updatedDriver.Country;

                // Save the changes to the database
                await _db.SaveChangesAsync();

                // Redirect to the desired action or view
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // If the model state is not valid, return the view with validation errors
            return View("~/Views/Driver/EditDiver.cshtml", updatedDriver);
        }

        private bool DriverExists(int id)
        {
            return _db.Drivers.Any(d => d.Id == id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            // Find the existing driver in the database
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            // Delete the driver with the specified id
            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();

            // Redirect to the desired action or view
            return RedirectToAction("Index");
        }



    }
}
