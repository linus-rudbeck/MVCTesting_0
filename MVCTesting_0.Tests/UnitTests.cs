using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MVCTesting_0.Controllers;
using MVCTesting_0.Data;
using MVCTesting_0.Models;

namespace MVCTesting_0.Tests
{
    public class UnitTests
    {
        [Test]
        public async Task TestControllerIndexAsync_InMemory()
        {
            // Konfigurera in-memory databas
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Lägg till data
            using (var context = new ApplicationDbContext(options))
            {
                context.Songs.AddRange(new List<Song>
                {
                    new Song { Id = 1, Title = "Test title 1", Artist = "Test artist 1" },
                    new Song { Id = 2, Title = "Test title 2", Artist = "Test artist 2" }
                });
                await context.SaveChangesAsync();
            }

            // Testa kontrollerlogiken
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new SongsController(context);
                var result = await controller.Index() as ViewResult;
                var model = result?.Model as List<Song>;

                // Assertions
                Assert.That(result, Is.Not.Null);
                Assert.That(model?.Count, Is.EqualTo(2));
            }
        }

    }
}