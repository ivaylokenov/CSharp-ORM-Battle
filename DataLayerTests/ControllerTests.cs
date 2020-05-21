namespace DataLayerTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataLayerApproaches.Controllers;
    using DataLayerApproaches.Data;
    using DataLayerApproaches.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ControllerTests
    {
        // Or just use My Tested ASP.NET for fully featured integration tests! :) 

        [Fact]
        public async Task ControllerWithDbContextTest()
        {
            // Assert
            var data = await GetData();

            var controller = new CatsController(data);

            // Act
            var result = await controller.Get("TestCat");

            // Assert
            Assert.Single("TestCat", result.Value.FirstOrDefault()?.Name);
        }

        private static async Task<CatsDbContext> GetData()
        {
            var dbContextOptions = new DbContextOptionsBuilder<CatsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Or use SQLite, if you need relationships
                .EnableSensitiveDataLogging()
                .Options;

            var dbContext = new CatsDbContext(dbContextOptions);

            dbContext.Cats.Add(new Cat
            {
                Id = 1,
                Name = "TestCat",
                Age = 10,
                Owner = new Owner
                {
                    Id = 1,
                    Name = "TestOwner"
                }
            });

            await dbContext.SaveChangesAsync();

            return dbContext;
        }
    }
}
