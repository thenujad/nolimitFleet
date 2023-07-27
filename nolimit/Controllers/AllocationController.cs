using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nolimit.Data;
using nolimit.Models;


namespace nolimit.Controllers
{
    public class AllocationController : BaseController
    {
        private readonly AppDbContext _db;

        public AllocationController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var allocations = _db.Allocations
                .Where(a => a.Status != "End")
                .ToList();

            var viewModel = new AllocationViewModel
            {
                Allocations = allocations
            };

            // Retrieve request data for each allocation
            foreach (var allocation in viewModel.Allocations)
            {
                var request = _db.Requests.FirstOrDefault(r => r.Id == allocation.RequestId);
                if (request != null)
                {
                    allocation.Request = request;
                }
            }
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }


            return View("~/Views/Navigation/ControlPanels/Allocation_Control_Panel.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult Start(int allocationId, [FromForm] Dictionary<int, int> MeterReading, [FromForm] Dictionary<int, float> FuelReading)
        {
            if (!MeterReading.TryGetValue(allocationId, out var meterReading) || !FuelReading.TryGetValue(allocationId, out var fuelReading))
            {
                ModelState.AddModelError("", "Invalid allocation data.");
                return View();
            }

            var allocation = _db.Allocations.Find(allocationId);
            if (allocation == null)
            {
                return NotFound();
            }

            allocation.Status = "Started";
            allocation.MeterReadingStart = meterReading;
            allocation.FuelAmountStart = fuelReading;
            allocation.StartTime = DateTime.Now;

          
            _db.Allocations.Update(allocation);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Allocation successfully started.";

            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult Complete(int allocationId, [FromForm] int MeterReadingEnd, [FromForm] float FuelAmountEnd)
        {
            var allocation = _db.Allocations.Find(allocationId);
            if (allocation == null )
            {
                return NotFound();
            }

            allocation.EndTime = DateTime.Now;
            allocation.MeterReadingEnd = MeterReadingEnd;
            allocation.FuelAmountEnd = FuelAmountEnd;
            allocation.Status = "Completed";

            var vehicle = _db.Vehicles.Find(allocation.VehicleId);
            var driver = _db.Drivers.Find(allocation.DriverId);

            if (vehicle != null && driver != null)
            {
                vehicle.Availability = true;
                driver.Availability = true;

                vehicle.CurrentMileage = allocation.MeterReadingEnd;
                vehicle.FuelLevel = FuelAmountEnd;
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

      
    }
}
