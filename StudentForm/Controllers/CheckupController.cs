using Microsoft.AspNetCore.Mvc;
using Patient_Form.Data;
using Patient_Form.Models;
using System.Linq;

public class CheckupController : Controller
{
    private readonly AppDbContext _context;

    public CheckupController(AppDbContext context)
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
            x.AppointmentDate.Date == model.AppointmentDate.Date &&
            x.SlotTime == model.SlotTime);

        if (slotTaken) {
            ModelState.AddModelError("SlotTime", "This slot already booked!");
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