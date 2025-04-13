using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ConsoleApp1
{
    public class ZooContext : DbContext
    {

        public ZooContext(DbContextOptions<ZooContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Day> Days { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<BabyAnimal> BabyAnimals { get; set; }
        public DbSet<Aviary> Aviaries { get; set; } = null!;
        public DbSet<Employer> Employers { get; set; } = null!;
        public DbSet<WorkingShift> WorkingShifts { get; set; } = null!;
        public DbSet<Animal> Animals { get; set; } = null!;
        public DbSet<AdultAnimal> AdultAnimals { get; set; } = null!;
        
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
            //options.UseSqlite($"Data Source={Path.GetFullPath(@"..\..\..\zoo.db")}");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Aviary>()
                .HasOne(a => a.Area)
                .WithMany(a => a.Aviaries)
                .HasForeignKey(a => a.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Aviary)
                .WithMany(a => a.Animals)
                .HasForeignKey(a => a.AviaryId);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.AdultAnimal)
                .WithOne(a => a.Animal)
                .HasForeignKey<AdultAnimal>(a => a.AnimalId);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.BabyAnimal)
                .WithOne(a => a.Animal)
                .HasForeignKey<BabyAnimal>(a => a.AnimalId);

            modelBuilder.Entity<Employer>()
                .HasOne(a => a.Area)
                .WithMany(a => a.Employers)
                .HasForeignKey(a => a.AreaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(a => a.Day)
                .WithMany(a => a.Events)
                .HasForeignKey(a => a.DayId);

            modelBuilder.Entity<Event>()
                .HasOne(a => a.WorkingShift)
                .WithMany(a => a.Events)
                .HasForeignKey(a => a.WorkingShiftId);

            modelBuilder.Entity<Event>()
                .HasOne(a => a.Animal)
                .WithMany(a => a.Events)
                .HasForeignKey(a => a.AnimalId);

            modelBuilder.Entity<WorkingShift>()
                .HasOne(e => e.Employer)
                .WithMany(e => e.Shifts)
                .HasForeignKey(e => e.EmployerId);

            modelBuilder.Entity<BabyAnimal>()
                .HasOne(e => e.AdultAnimal)
                .WithMany(e => e.BabyAnimals)
                .HasForeignKey(e => e.AdultAnimalId);
        }
    }
}
