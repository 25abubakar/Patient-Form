using Microsoft.AspNetCore.Mvc;
using Patient_Form.Data;
using Patient_Form.Models;
using System.Linq;
using CheckupModel = Patient_Form.Models.CheckupModel;

namespace Patient_Form.Controllers
{
    public class CheckupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnlineCheckup()
        {
            //ViewBag.Appointments = _context.Set<CheckupModel>().ToList();
            return View();
        }

        [HttpGet]
        public IActionResult CheckupModel()
        {
            //ViewBag.Appointments = _context.Set<CheckupModel>().ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckupModel(CheckupModel model)
        {
            if (!ModelState.IsValid) {
                //ViewBag.Appointments = _context.Set<CheckupModel>().ToList();
                return View(model);
            }

            bool slotTaken = _context.Set<CheckupModel>().Any(x =>
                x.AppointmentDate.Value.Date == model.AppointmentDate.Value.Date &&
                x.SlotTime == model.SlotTime);

            if (slotTaken) {
                ModelState.AddModelError("SlotTime", "This slot is already booked!");
                ViewBag.Appointments = _context.Set<CheckupModel>().ToList();
                return View(model);
            }

            _context.Set<CheckupModel>().Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("CheckupModel");
        }
    }
}