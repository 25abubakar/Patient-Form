using Microsoft.EntityFrameworkCore;
using Patient_Form.Models;

namespace Patient_Form.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CheckupModel> Appointments { get; set; }
    }
}