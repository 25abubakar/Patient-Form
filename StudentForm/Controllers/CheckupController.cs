using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Patient_Form.Data;
using Patient_Form.Models;
using System;
using System.Collections.Generic;

namespace Patient_Form.Controllers
{
    public class CheckupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST + BOOK APPOINTMENT PAGE
        public IActionResult OnlineCheckup()
        {
            var appointments = new List<CheckupModel>();

            using var conn = _context.Database.GetDbConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.FName, p.LstName, p.Age, p.Gender, p.DOB, p.Email, p.Phone, p.City,
                       a.PatientType, a.VisitType, a.Disease, a.AppointmentDate, a.SlotTime, a.Message,
                       a.Id AS AppointmentId, a.PatientId
                FROM Patients p
                INNER JOIN Appointments a ON p.Id = a.PatientId
                ORDER BY a.AppointmentDate DESC";

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                appointments.Add(new CheckupModel {
                    FName = reader["FName"]?.ToString(),
                    LstName = reader["LstName"]?.ToString(),
                    Age = Convert.ToInt32(reader["Age"]),
                    Gender = reader["Gender"]?.ToString(),
                    DOB = reader["DOB"] as DateTime?,
                    Email = reader["Email"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    City = reader["City"]?.ToString(),
                    PatientType = reader["PatientType"]?.ToString(),
                    VisitType = reader["VisitType"]?.ToString(),
                    Disease = reader["Disease"]?.ToString(),
                    AppointmentDate = reader["AppointmentDate"] as DateTime?,
                    SlotTime = reader["SlotTime"]?.ToString(),
                    Message = reader["Message"]?.ToString(),
                    AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                    PatientId = Convert.ToInt32(reader["PatientId"])
                });
            }

            ViewBag.Appointments = appointments;
            return View();
        }

        // CREATE APPOINTMENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnlineCheckup(CheckupModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var conn = _context.Database.GetDbConnection();
            conn.Open();

            int patientId;

            // Insert Patient
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = @"
                    INSERT INTO Patients(FName,LstName,Age,Gender,DOB,Email,Phone,City)
                    VALUES(@FName,@LstName,@Age,@Gender,@DOB,@Email,@Phone,@City);
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

            // Insert Appointment
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = @"
                    INSERT INTO Appointments
                    (PatientId,PatientType,VisitType,Disease,AppointmentDate,SlotTime,Message)
                    VALUES(@PatientId,@PatientType,@VisitType,@Disease,@AppointmentDate,@SlotTime,@Message)";

                cmd.Parameters.Add(new SqlParameter("@PatientId", patientId));
                cmd.Parameters.Add(new SqlParameter("@PatientType", model.PatientType));
                cmd.Parameters.Add(new SqlParameter("@VisitType", model.VisitType));
                cmd.Parameters.Add(new SqlParameter("@Disease", model.Disease));
                cmd.Parameters.Add(new SqlParameter("@AppointmentDate", model.AppointmentDate.Value));
                cmd.Parameters.Add(new SqlParameter("@SlotTime", model.SlotTime));
                cmd.Parameters.Add(new SqlParameter("@Message", (object)model.Message ?? DBNull.Value));

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("OnlineCheckup");
        }

        // GET EDIT
        public IActionResult Edit(int appointmentId)
        {
            CheckupModel model = null;

            using var conn = _context.Database.GetDbConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.Id AS PatientId, p.FName, p.LstName, p.Age, p.Gender, p.DOB, p.Email, p.Phone, p.City,
                       a.Id AS AppointmentId, a.PatientType, a.VisitType, a.Disease, a.AppointmentDate, a.SlotTime, a.Message
                FROM Patients p
                INNER JOIN Appointments a ON p.Id = a.PatientId
                WHERE a.Id=@AppointmentId";

            cmd.Parameters.Add(new SqlParameter("@AppointmentId", appointmentId));

            using var reader = cmd.ExecuteReader();
            if (reader.Read()) {
                model = new CheckupModel {
                    PatientId = Convert.ToInt32(reader["PatientId"]),
                    AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                    FName = reader["FName"]?.ToString(),
                    LstName = reader["LstName"]?.ToString(),
                    Age = Convert.ToInt32(reader["Age"]),
                    Gender = reader["Gender"]?.ToString(),
                    DOB = reader["DOB"] as DateTime?,
                    Email = reader["Email"]?.ToString(),
                    Phone = reader["Phone"]?.ToString(),
                    City = reader["City"]?.ToString(),
                    PatientType = reader["PatientType"]?.ToString(),
                    VisitType = reader["VisitType"]?.ToString(),
                    Disease = reader["Disease"]?.ToString(),
                    AppointmentDate = reader["AppointmentDate"] as DateTime?,
                    SlotTime = reader["SlotTime"]?.ToString(),
                    Message = reader["Message"]?.ToString()
                };
            }

            if (model == null) return NotFound();
            return View(model);
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CheckupModel model)
        {
            using var conn = _context.Database.GetDbConnection();
            conn.Open();

            // Update Patient
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = @"UPDATE Patients SET
                    FName=@FName,LstName=@LstName,Age=@Age,Gender=@Gender,DOB=@DOB,
                    Email=@Email,Phone=@Phone,City=@City
                    WHERE Id=@PatientId";

                cmd.Parameters.Add(new SqlParameter("@FName", model.FName));
                cmd.Parameters.Add(new SqlParameter("@LstName", model.LstName));
                cmd.Parameters.Add(new SqlParameter("@Age", model.Age));
                cmd.Parameters.Add(new SqlParameter("@Gender", model.Gender));
                cmd.Parameters.Add(new SqlParameter("@DOB", (object)model.DOB ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Email", model.Email));
                cmd.Parameters.Add(new SqlParameter("@Phone", model.Phone));
                cmd.Parameters.Add(new SqlParameter("@City", model.City));
                cmd.Parameters.Add(new SqlParameter("@PatientId", model.PatientId));
                cmd.ExecuteNonQuery();
            }

            // Update Appointment
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = @"UPDATE Appointments SET
                    PatientType=@PatientType,VisitType=@VisitType,Disease=@Disease,
                    AppointmentDate=@AppointmentDate,SlotTime=@SlotTime,Message=@Message
                    WHERE Id=@AppointmentId";

                cmd.Parameters.Add(new SqlParameter("@PatientType", model.PatientType));
                cmd.Parameters.Add(new SqlParameter("@VisitType", model.VisitType));
                cmd.Parameters.Add(new SqlParameter("@Disease", model.Disease));
                cmd.Parameters.Add(new SqlParameter("@AppointmentDate", model.AppointmentDate.Value));
                cmd.Parameters.Add(new SqlParameter("@SlotTime", model.SlotTime));
                cmd.Parameters.Add(new SqlParameter("@Message", (object)model.Message ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@AppointmentId", model.AppointmentId));
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Appointment updated successfully!";
            return RedirectToAction("OnlineCheckup");
        }

        // DELETE Appointment
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(CheckupModel model)
        {
            using var conn = _context.Database.GetDbConnection();
            conn.Open();

            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = "DELETE FROM Appointments WHERE Id=@AppointmentId";
                cmd.Parameters.Add(new SqlParameter("@AppointmentId", model.AppointmentId));
                cmd.ExecuteNonQuery();
            }
            TempData["Success"] = "Record deleted successfully!";
            return RedirectToAction("OnlineCheckup");
        }
    }
}