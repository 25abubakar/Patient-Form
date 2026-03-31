using Microsoft.EntityFrameworkCore;
using Patient_Form.Models; 
namespace Patient_Form.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSets for your tables
        DbSet<Patient_Form.Models.CheckupModel> Appointments { get; set; }
    }
}