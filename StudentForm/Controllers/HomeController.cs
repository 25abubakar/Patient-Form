using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient_Form.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Patient_Form.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // simple in-memory "database" for bookings
        private static List<CheckupModel> bookings = new List<CheckupModel>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult OnlineCheckup()
        {
            ViewBag.Appointments = bookings;
            return View();
        }

        [HttpPost]
        public IActionResult OnlineCheckup(CheckupModel model)
        {
            if (ModelState.IsValid) {
                bookings.Add(model);
                return RedirectToAction("OnlineCheckup");
            }

            ViewBag.Appointments = bookings;
            return View(model);
        }
    }
}