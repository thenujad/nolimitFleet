using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    public class NavigationController : BaseController
    {
        private readonly AppDbContext _db; // Replace YourDbContext with your actual DbContext class

        public NavigationController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }

      
        
        public IActionResult Maintenance()
        {
            return View("~/Views/Navigation/Vehicle/Maintenance.cshtml");
        }

        public IActionResult View_and_update()
        {
            return View("~/Views/Navigation/Vehicle/View_and_update.cshtml");
        }
        public IActionResult alo()
        {
            return View("~/Views/Navigation/ControlPanels/Allocation_Control_Panel.cshtml");
        }


        public IActionResult Vehicle_status_history()
        {
            return View("~/Views/Navigation/Vehicle/Vehicle_status_history.cshtml");
        }
       
        
        public IActionResult Driver_History()
        {
            return View("~/Views/Navigation/Driver/Driver_History.cshtml");
        }
        public IActionResult Reports()
        {
            return View("~/Views/Navigation/Reports/Reports.cshtml");
        }
        
      
        public IActionResult Fuel()
        {
            return View("~/Views/Navigation/Fuel/Fuel.cshtml");
        }
        
      

    }
}
