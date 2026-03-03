using Microsoft.EntityFrameworkCore;
using practice.Models;

namespace practice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public DbSet<EmployeeAddress> EmployeeAddresses { get; set; }

        public DbSet<EmployeePersonalDetails> EmployeePersonalDetails { get; set; }
        public DbSet<EmployeeEducation> EmployeeEducations { get; set; }
        public DbSet<EmployeeCompensation> EmployeeCompensations { get; set; }
        public DbSet<EmployeePreviousEmployment> EmployeePreviousEmployments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<EmployeeAddress>()
                .HasOne(a => a.Employee)
                .WithOne(e => e.Address)
                .HasForeignKey<EmployeeAddress>(a => a.EmpId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<EmployeePersonalDetails>()
                .HasOne(p => p.Employee)
                .WithOne(e => e.PersonalDetails)
                .HasForeignKey<EmployeePersonalDetails>(p => p.EmpId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeMaster>()
                .HasOne<EmployeeCompensation>()
                .WithOne(c => c.Employee)
                .HasForeignKey<EmployeeCompensation>(c => c.EmpId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeMaster>()
                .HasMany<EmployeePreviousEmployment>()
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmpId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}