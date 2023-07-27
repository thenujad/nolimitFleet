using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using nolimit.Data;
using nolimit.Models;
using System;
using System.Linq;

namespace nolimit.Controllers
{
    public class VehicleRequestController : BaseController
    {
        private readonly AppDbContext _db;

        public VehicleRequestController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }

        public IActionResult Index()
        {
          
            var pendingRequests = _db.Requests.Where(r => r.Status == "Pending").ToList();
            ViewData["PendingRequests"] = pendingRequests;

            var approvedRequests = _db.Requests.Where(r => r.Status == "Approved").ToList();
            var drivers = _db.Drivers.Where(d => d.Availability).ToList();
            var vehicles = _db.Vehicles.Where(v => v.Availability).ToList();

            var allocationViewModel = new AllocationViewModel
            {
                Requests = approvedRequests,
                Drivers = drivers,
                Vehicles = vehicles
            };

            ViewData["AllocationViewModel"] = allocationViewModel;

            return View("~/Views/Navigation/ControlPanels/Fleet_Operations_Control_Panel.cshtml");
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Users = _db.Users.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Request request)
        {
           
                // Retrieve the user ID from the session
                int? userId = HttpContext.Session.GetInt32("UserId");


                request.Status = "Pending";
                request.UserId = userId.Value;

                _db.Requests.Add(request);

                _db.SaveChanges();

                TempData["SuccessMessage"] = "Request created successfully.";


            // If the model state is not valid, return the view with validation errors
            return RedirectToAction("Index");
        }

        public IActionResult PendingRequests()
        {
            var pendingRequests = _db.Requests.Where(r => r.Status == "Pending").ToList();
            return View(pendingRequests);
        }


        [HttpPost]
        public IActionResult Approve(int id)
        {
            var request = _db.Requests.Find(id);
            if (request == null || request.Status != "Pending")
            {
                return NotFound();
            }

            request.Status = "Approved";
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = _db.Requests.Find(id);
            if (request == null || request.Status != "Pending")
            {
                return NotFound();
            }

            request.Status = "Rejected";
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Allocate()
        {
            var approvedRequests = _db.Requests.Where(r => r.Status == "Approved").ToList();
            var drivers = _db.Drivers.Where(d => d.Availability).ToList();
            var vehicles = _db.Vehicles.Where(v => v.Availability).ToList();

            var viewModel = new AllocationViewModel
            {
                Requests = approvedRequests,
                Drivers = drivers,
                Vehicles = vehicles
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Allocate(int requestId, int driverId, int vehicleId)
        {
            // Retrieve the user ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Handle the case when the user ID is not found in the session
                // Redirect to a login page or display an error message
                return RedirectToAction("Login", "Account"); // Replace with your login action and controller
            }

            // Check if the driver with the provided ID exists
            var driver = _db.Drivers.FirstOrDefault(d => d.Id == driverId);
            if (driver == null)
            {
                // Handle the case when the driver is not found
                // Redirect to a specific error page or perform any other desired action
                return RedirectToAction("DriverNotFound");
            }

            // Proceed with allocation if the driver is found
            var allocation = new Allocation
            {
                RequestId = requestId,
                UserId = userId.Value, // Unbox the nullable int to int
                DriverId = driverId,
                VehicleId = vehicleId,
                Note = "Allocation note",
                DepartureDate = DateTime.Now,
                Status = "Allocated"
            };

            // Update driver and vehicle availability
            driver.Availability = false;

            var vehicle = _db.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle != null)
            {
                vehicle.Availability = false;
            }

            _db.Allocations.Add(allocation);

            // Update the request status
            var request = _db.Requests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Allocated";
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Cancel(int requestId)
        {
            var request = _db.Requests.Find(requestId);
            if (request == null || request.Status != "Approved")
            {
                return NotFound();
            }

            request.Status = "Cancelled";
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
