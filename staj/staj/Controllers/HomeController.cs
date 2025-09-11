using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using staj.Models;

namespace staj.Controllers
{
    
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ControlPanel()
        {
            return View();
        }
    }
}
