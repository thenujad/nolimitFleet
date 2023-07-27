using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nolimit.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using nolimit.Data;
using Microsoft.EntityFrameworkCore;

namespace nolimit.Controllers
{
    public class HomeController : BaseController
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
           : base(httpContextAccessor)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var maintenanceRequests = _db.VehicleMaintenance
     .Where(vm => vm.Type == "Maintenance" && vm.Status == "Pending")
     .Select(vm => new
     {
         vm.Id,
         vm.VehicleId,
         vm.Detail,
         vm.Status,
         LicensePlateNumber = _db.Vehicles
             .Where(v => v.Id == vm.VehicleId)
             .Select(v => v.LicensePlateNumber)
             .FirstOrDefault()
     })
     .ToList();

            var accidentRequests = _db.VehicleMaintenance
                .Where(vm => vm.Type == "Accident" && vm.Status == "Pending")
                .Select(vm => new
                {
                    vm.Id,
                    vm.VehicleId,
                    vm.Detail,
                    vm.Status,
                    LicensePlateNumber = _db.Vehicles
                        .Where(v => v.Id == vm.VehicleId)
                        .Select(v => v.LicensePlateNumber)
                        .FirstOrDefault()
                })
                .ToList();

            ViewBag.MaintenanceRequests = maintenanceRequests;
            ViewBag.AccidentRequests = accidentRequests;

        

            var requests = _db.Requests.ToList(); // Fetch requests data from the database
            var viewModel = new AllocationViewModel
            {
                Requests = requests
            };

            int outOfServiceCount = _db.Vehicles.Count(v => !v.Availability);
            int activeCount = _db.Allocations.Count(a => a.Status == "Started");
            int availableCount = _db.Vehicles.Count(v => v.Availability);
            int inServiceCount = _db.VehicleMaintenance.Count(vm => vm.Status == "Pending");
            int fuelingCount = _db.Fuel.Count(f => f.Status == "Fueling");

            ViewBag.OutOfServiceCount = outOfServiceCount;
            ViewBag.ActiveCount = activeCount;
            ViewBag.AvailableCount = availableCount;
            ViewBag.InServiceCount = inServiceCount;
            ViewBag.FuelingCount = fuelingCount;

         
            return View("~/Views/Home/Index.cshtml", viewModel); // Pass the requests data to the view
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
     

    }
}
