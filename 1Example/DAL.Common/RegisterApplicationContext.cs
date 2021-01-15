using Core.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.DAL;

namespace DAL.Common
{
    public class RegisterApplicationContext : ApplicationContext
    {
        public RegisterApplicationContext(DbContextOptions<RegisterApplicationContext> options) : base(options)
        {
            RegisterCustomReposynoryType<House, HouseRepository>();
        }
        public DbSet<House> Houses { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<Street> Streets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<House>().Property(b => b.Number).IsRequired();
            modelBuilder.Entity<Flat>().Property(b => b.Number).IsRequired();
            modelBuilder.Entity<Street>().Property(b => b.Name).IsRequired();

            modelBuilder.Entity<Flat>()
            .HasOne(p => p.House)
            .WithMany(t => t.Flats)
            .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
