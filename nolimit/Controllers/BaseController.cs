using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace nolimit.Controllers
{
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            string userName = _httpContextAccessor.HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;

            int? userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
        }
    }
}
