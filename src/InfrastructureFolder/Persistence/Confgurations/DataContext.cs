using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Confgurations
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<EmployeeContact> EmployeeContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            return result;
        }
    }
}
