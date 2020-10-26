using Core.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.DAL;

namespace DAL.Common
{
    public class RegisterApplicationContext : ApplicationContext
    {
        public RegisterApplicationContext(DbContextOptions<RegisterApplicationContext> options) : base(options)
        {

        }
        public DbSet<House> Houses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<House>().Property(b => b.Number).IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }
}
