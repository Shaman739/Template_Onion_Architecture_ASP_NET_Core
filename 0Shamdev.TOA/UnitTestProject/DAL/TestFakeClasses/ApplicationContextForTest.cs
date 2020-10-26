using Microsoft.EntityFrameworkCore;
using Shamdev.TOA.DAL;

namespace UnitTestProject.DAL.TestFakeClasses
{
    internal class ApplicationContextForTest : ApplicationContext
    {
        public ApplicationContextForTest(DbContextOptions<ApplicationContextForTest> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObjectMappingForTest>().Property(b => b.IntValue).IsRequired();
            modelBuilder.Entity<ObjectMappingForTest>().Property(b => b.IntValue2).IsRequired();
        }

        public DbSet<ObjectMappingForTest> TestObjects { get; set; }
        public DbSet<SubObjectMappingForTest> TestSubObjects { get; set; }
    }
}
