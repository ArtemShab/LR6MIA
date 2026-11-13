using LR4Premium_Dry_Cleaning_and_Laundry.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LR4Premium_Dry_Cleaning_and_Laundry.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
