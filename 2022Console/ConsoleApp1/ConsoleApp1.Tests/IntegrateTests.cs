using Xunit;
using Microsoft.EntityFrameworkCore;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Xml;
using NPOI.SS.Formula.PTG;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NPOI.SS.Formula.Functions;
using EnumsNET;

namespace ConsoleApp1.Tests
{
    public class IntegrationTests
    {
        private readonly ServiceProvider _serviceProvider;

        public IntegrationTests()
        {
            var serviceCollection1 = new ServiceCollection();
            serviceCollection1.AddDbContext<ZooContext>(options =>
            {
                options.UseSqlite("Data Source=TestDatabase");
            });

            _serviceProvider = serviceCollection1.BuildServiceProvider();
        }
        
        [Fact]
        public async Task Can_Add_And_Retrieve_Area_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Areas.FirstOrDefaultAsync(e => e.Area_name == "Area 1");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("Area 1", retrievedEntity.Area_name);
            Assert.Equal("Pskov", retrievedEntity.Adress);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_AdultAnimal_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var adultAnimal = new AdultAnimal { Name = "Pushok", AnimalId = 1 };
            var animal = new Animal { TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live" };
            var aviary = new Aviary { AviaryName = "Zone 1", AreaId = 1 };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Aviaries.Add(aviary);
            await context.SaveChangesAsync();
            context.Animals.Add(animal);
            await context.SaveChangesAsync();
            context.AdultAnimals.Add(adultAnimal);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.AdultAnimals.FirstOrDefaultAsync(e => e.Name == "Pushok");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("Pushok", retrievedEntity.Name);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_Animal_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var animal = new Animal {TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live" };
            var aviary = new Aviary { AviaryName = "Zone 1", AreaId = 1 };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Aviaries.Add(aviary);
            await context.SaveChangesAsync();
            context.Animals.Add(animal);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Animals.FirstOrDefaultAsync(e => e.TypeOfAnimal == "Tiger");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("Tiger", retrievedEntity.TypeOfAnimal);
            Assert.Equal("live", retrievedEntity.Status);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_Aviary_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var aviary = new Aviary { AviaryName = "Zone 1", AreaId = 1 };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Aviaries.Add(aviary);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Aviaries.FirstOrDefaultAsync(e => e.AviaryName == "Zone 1");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("Zone 1", retrievedEntity.AviaryName);
            Assert.Equal(1, retrievedEntity.AreaId);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_BabyAnimal_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var babyAnimal = new BabyAnimal { Name = "Puchok", AnimalId = 2, AdultAnimalId = 1};
            var adultAnimal = new AdultAnimal { Name = "Pushok", AnimalId = 1 };
            var animal = new Animal { TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live" };
            var animalB = new Animal { TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live" };
            var aviary = new Aviary { AviaryName = "Zone 1", AreaId = 1 };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Aviaries.Add(aviary);
            await context.SaveChangesAsync();
            context.Animals.Add(animal);
            await context.SaveChangesAsync();
            context.Animals.Add(animalB);
            await context.SaveChangesAsync();
            context.AdultAnimals.Add(adultAnimal);
            await context.SaveChangesAsync();
            context.BabyAnimals.Add(babyAnimal);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.BabyAnimals.FirstOrDefaultAsync(e => e.Name == "Puchok");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("Puchok", retrievedEntity.Name);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_Day_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var day = new Day
            {
                Date = DateTime.ParseExact("2025-04-12", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture)
            };
            context.Database.EnsureCreated();

            context.Days.Add(day);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Days.FirstOrDefaultAsync(e => e.Id == 1);

            Assert.NotNull(retrievedEntity);
            Assert.Equal(2025, retrievedEntity.Date.Year);
            Assert.Equal(04, retrievedEntity.Date.Month);
            Assert.Equal(12, retrievedEntity.Date.Day);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_Event_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var e = new Event
            {
                AnimalId = 1,
                DayId = 1,
                EventText = "nothing",
                IsBorn = false,
                IsDie = false,
                WorkingShiftId = 1,
                EventTime = DateTime.ParseExact("19:59:40", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture)
            };
            var animal = new Animal { TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live" };
            var aviary = new Aviary { AviaryName = "Zone 1", AreaId = 1 };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            var shift = new WorkingShift
            {
                EmployerId = 1,
                TimeBegin = DateTime.ParseExact("10:15:40", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture),
                TimeEnd = DateTime.ParseExact("20:15:20", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture)
            };
            var employer = new Employer
            {
                Fio = "GorshkovSergeyAlbertovich",
                AreaId = 1,
            };
            var day = new Day
            {
                Date = DateTime.ParseExact("2025-04-12", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture)
            };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Aviaries.Add(aviary);
            await context.SaveChangesAsync();
            context.Animals.Add(animal);
            await context.SaveChangesAsync();
            context.Employers.Add(employer);
            await context.SaveChangesAsync();
            context.WorkingShifts.Add(shift);
            await context.SaveChangesAsync();
            context.Days.Add(day);
            await context.SaveChangesAsync();
            context.Events.Add(e);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Events.FirstOrDefaultAsync(e => e.EventText == "nothing");

            Assert.NotNull(retrievedEntity);
            Assert.Equal(1, retrievedEntity.AnimalId);
            Assert.Equal(1, retrievedEntity.DayId);
            Assert.Equal("nothing", retrievedEntity.EventText);
            Assert.False(retrievedEntity.IsBorn);
            Assert.False(retrievedEntity.IsDie);
            Assert.Equal(1, retrievedEntity.WorkingShiftId);
            Assert.Equal(19, retrievedEntity.EventTime.Hour);
            Assert.Equal(59, retrievedEntity.EventTime.Minute);
            Assert.Equal(40, retrievedEntity.EventTime.Second);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_Employer_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var employer = new Employer
            {
                Fio = "GorshkovSergeyAlbertovich",
                AreaId = 1,
            };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Employers.Add(employer);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Employers.FirstOrDefaultAsync(e => e.Fio == "GorshkovSergeyAlbertovich");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("GorshkovSergeyAlbertovich", retrievedEntity.Fio);
            Assert.Equal(1, retrievedEntity.AreaId);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_WorkingShift_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var shift = new WorkingShift
            {
                EmployerId = 1,
                TimeBegin = DateTime.ParseExact("10:15:40", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture),
                TimeEnd = DateTime.ParseExact("20:15:20", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture)
            };
            var employer = new Employer
            {
                Fio = "GorshkovSergeyAlbertovich",
                AreaId = 1,
            };
            var area = new Area { Area_name = "Area 1", Adress = "Pskov" };
            context.Database.EnsureCreated();

            context.Areas.Add(area);
            await context.SaveChangesAsync();
            context.Employers.Add(employer);
            await context.SaveChangesAsync();
            context.WorkingShifts.Add(shift);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.WorkingShifts.FirstOrDefaultAsync(e => e.Id == 1);

            Assert.NotNull(retrievedEntity);
            Assert.Equal(1, retrievedEntity.EmployerId);
            Assert.Equal(10, retrievedEntity.TimeBegin.Hour);
            Assert.Equal(20, retrievedEntity.TimeEnd.Hour);
            Assert.Equal(15, retrievedEntity.TimeBegin.Minute);
            Assert.Equal(15, retrievedEntity.TimeEnd.Minute);
            Assert.Equal(40, retrievedEntity.TimeBegin.Second);
            Assert.Equal(20, retrievedEntity.TimeEnd.Second);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Can_Add_And_Retrieve_User_From_Database()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            var user = new User
            {
                Login = "user",
                Password = "04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb",
            };
            context.Database.EnsureCreated();

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var retrievedEntity = await context.Users.FirstOrDefaultAsync(e => e.Login == "user");

            Assert.NotNull(retrievedEntity);
            Assert.Equal("user", retrievedEntity.Login);
            Assert.Equal("04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb", retrievedEntity.Password);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Database_Is_Empty_Initially()
        {
            await using var context = _serviceProvider.GetRequiredService<ZooContext>();
            context.Database.EnsureCreated();

            var areas_count = await context.Areas.CountAsync();
            var adultAnimals_count = await context.AdultAnimals.CountAsync();
            var animals_count = await context.Animals.CountAsync();
            var aviaries_count = await context.Aviaries.CountAsync();
            var babyAnimals_count = await context.BabyAnimals.CountAsync();
            var days_count = await context.Days.CountAsync();
            var events_count = await context.Events.CountAsync();
            var employers_count = await context.Employers.CountAsync();
            var shifts_count = await context.WorkingShifts.CountAsync();
            var users_count = await context.Users.CountAsync();

            Assert.Equal(0, areas_count);
            Assert.Equal(0, adultAnimals_count);
            Assert.Equal(0, animals_count);
            Assert.Equal(0, aviaries_count);
            Assert.Equal(0, babyAnimals_count);
            Assert.Equal(0, days_count);
            Assert.Equal(0, events_count);
            Assert.Equal(0, employers_count);
            Assert.Equal(0, shifts_count);
            Assert.Equal(0, users_count);
            context.Database.EnsureDeleted();
        }
    }
}
