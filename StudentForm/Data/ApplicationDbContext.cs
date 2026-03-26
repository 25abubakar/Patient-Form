using Microsoft.EntityFrameworkCore;
using Patient_Form.Models; // Replace with your namespace

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
        public DbSet<CheckupModel> Appointments { get; set; }
    }
}