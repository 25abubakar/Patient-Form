using Microsoft.AspNetCore.Mvc;
using Patient_Form.Data;
using Patient_Form.Models;
using System.Linq;

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
            ViewBag.Appointments = _context.Appointments.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnlineCheckup(CheckupModel model)
        {
            if (!ModelState.IsValid) {
                ViewBag.Appointments = _context.Appointments.ToList();
                return View(model);
            }

            bool slotTaken = _context.Appointments.Any(x =>
                x.AppointmentDate.Value.Date == model.AppointmentDate.Value.Date &&
                x.SlotTime == model.SlotTime);

            if (slotTaken) {
                ModelState.AddModelError("SlotTime", "This slot is already booked!");
                ViewBag.Appointments = _context.Appointments.ToList();
                return View(model);
            }

            _context.Appointments.Add(model);
            _context.SaveChanges();

            ViewBag.Success = "Appointment booked successfully!";
            ModelState.Clear();
            ViewBag.Appointments = _context.Appointments.ToList();

            return View();
        }
    }
}