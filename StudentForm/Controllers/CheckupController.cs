using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Patient_Form.Data;
using Patient_Form.Models;
using System;
using System.Collections.Generic;
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
            var appointments = new List<CheckupModel>();

            using (var conn = _context.Database.GetDbConnection()) {
                conn.Open();
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = @"
                SELECT p.FName, p.LstName, p.Age, p.Gender, p.DOB, p.Email, p.Phone, p.City,
                       a.PatientType, a.VisitType, a.Disease, a.AppointmentDate, a.SlotTime, a.Message
                FROM dbo.Patients p
                INNER JOIN dbo.Appointments a ON p.Id = a.PatientId
                ORDER BY a.AppointmentDate DESC";

                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            appointments.Add(new CheckupModel {
                                FName = reader["FName"].ToString(),
                                LstName = reader["LstName"].ToString(),
                                Age = Convert.ToInt32(reader["Age"]),
                                Gender = reader["Gender"].ToString(),
                                DOB = reader["DOB"] as DateTime?,
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                City = reader["City"].ToString(),
                                PatientType = reader["PatientType"].ToString(),
                                VisitType = reader["VisitType"].ToString(),
                                Disease = reader["Disease"].ToString(),
                                AppointmentDate = reader["AppointmentDate"] as DateTime?,
                                SlotTime = reader["SlotTime"].ToString(),
                                Message = reader["Message"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.Appointments = appointments;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnlineCheckup(CheckupModel model)
            {
            if (!ModelState.IsValid) {
                return View(model);
            }

            using (var conn = _context.Database.GetDbConnection()) {
                conn.Open();

                int patientId;
                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = @"
                INSERT INTO dbo.Patients 
                (FName, LstName, Age, Gender, DOB, Email, Phone, City)
                VALUES (@FName,@LstName,@Age,@Gender,@DOB,@Email,@Phone,@City);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                    cmd.Parameters.Add(new SqlParameter("@FName", model.FName));
                    cmd.Parameters.Add(new SqlParameter("@LstName", model.LstName));
                    cmd.Parameters.Add(new SqlParameter("@Age", model.Age));
                    cmd.Parameters.Add(new SqlParameter("@Gender", model.Gender));
                    cmd.Parameters.Add(new SqlParameter("@DOB", (object)model.DOB ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@Email", model.Email));
                    cmd.Parameters.Add(new SqlParameter("@Phone", model.Phone));
                    cmd.Parameters.Add(new SqlParameter("@City", model.City));

                    patientId = (int)cmd.ExecuteScalar();
                }

                using (var cmd = conn.CreateCommand()) {
                    cmd.CommandText = @"
                INSERT INTO dbo.Appointments
                (PatientId, PatientType, VisitType, Disease, AppointmentDate, SlotTime, Message)
                VALUES (@PatientId,@PatientType,@VisitType,@Disease,@AppointmentDate,@SlotTime,@Message)";

                    cmd.Parameters.Add(new SqlParameter("@PatientId", patientId));
                    cmd.Parameters.Add(new SqlParameter("@PatientType", model.PatientType));
                    cmd.Parameters.Add(new SqlParameter("@VisitType", model.VisitType));
                    cmd.Parameters.Add(new SqlParameter("@Disease", model.Disease));
                    cmd.Parameters.Add(new SqlParameter("@AppointmentDate", model.AppointmentDate.Value));
                    cmd.Parameters.Add(new SqlParameter("@SlotTime", model.SlotTime));
                    cmd.Parameters.Add(new SqlParameter("@Message", (object)model.Message ?? DBNull.Value));

                    cmd.ExecuteNonQuery();
                }
            }
            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("OnlineCheckup");
        }
    }
}