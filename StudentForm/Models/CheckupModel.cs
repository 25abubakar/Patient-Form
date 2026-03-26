using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patient_Form.Models
{
    [Table("Appointments")]
    public class CheckupModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone must be Pakistani format (03XXXXXXXXX)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Disease is required")]
        public string Disease { get; set; }

        [Required(ErrorMessage = "Appointment date required")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Date must be today or future")]
        public DateTime? AppointmentDate { get; set; }  // Nullable

        [Required(ErrorMessage = "Please select time slot")]
        public string SlotTime { get; set; }

        [StringLength(200)]
        public string Message { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            if (value is DateTime date) {
                return date.Date >= DateTime.Today;
            }

            return false;
        }
    }
}