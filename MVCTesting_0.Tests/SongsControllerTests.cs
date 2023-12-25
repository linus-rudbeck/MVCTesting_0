using EntityFrameworkCore.Testing.Moq;
using Microsoft.AspNetCore.Mvc;
using MVCTesting_0.Controllers;
using MVCTesting_0.Data;
using MVCTesting_0.Models;
using System.Reflection;

namespace MVCTesting_0.Tests
{
    [TestFixture]
    public class SongsControllerTests
    {
        private ApplicationDbContext mockContext;
        private SongsController controller;

        [SetUp]
        public void SetUp()
        {
            // Skapa en mockad DbContext
            mockContext = Create.MockedDbContextFor<ApplicationDbContext>();

            // Konfigurera mockad DbSet
            var songs = new List<Song>
            {
                new Song { Id = 1, Title = "Song 1", Artist = "Artist 1" },
                new Song { Id = 2, Title = "Song 2", Artist = "Artist 2" }
            };

            // Använd SetupDbSet för att konfigurera DbSet
            mockContext.Set<Song>().AddRange(songs);
            mockContext.SaveChanges();

            mockContext.ChangeTracker.Clear();

            // Skapa controller med mockad DbContext
            controller = new SongsController(mockContext);
        }



        [Test]
        public async Task Index_ReturnsViewResult_WithListOfSongs()
        {
            // Act
            var result = await controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = result as ViewResult;
            var model = viewResult.Model;
            Assert.That(model, Is.InstanceOf<List<Song>>());

            var songList = model as List<Song>;
            Assert.That(songList.Count, Is.EqualTo(2));
        }


        [Test]
        public async Task Details_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsNotFoundResult_WhenSongDoesNotExist()
        {
            // Act
            var result = await controller.Details(99); // Antag att detta ID inte finns

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsViewResult_WithSong()
        {
            // Arrange
            var song = new Song { Id = 3, Title = "Test Song", Artist = "Test Artist" };
            mockContext.Set<Song>().Add(song);
            mockContext.SaveChanges();

            // Act
            var result = await controller.Details(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<Song>(viewResult.Model);

            var model = viewResult.Model as Song;
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public void Create_ReturnsViewResult()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }


        [Test]
        public async Task Create_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var newSong = new Song { Title = "New Song", Artist = "New Artist" };

            // Act
            var result = await controller.Create(newSong);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            controller.ModelState.AddModelError("Error", "Model error");
            var newSong = new Song { Title = "New Song", Artist = "New Artist" };

            // Act
            var result = await controller.Create(newSong);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Edit_Get_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await controller.Edit(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ReturnsNotFoundResult_WhenSongDoesNotExist()
        {
            // Act
            var result = await controller.Edit(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewResult_WithSong()
        {
            // Arrange
            var song = new Song { Id = 3, Title = "Test Song", Artist = "Test Artist" };
            mockContext.Set<Song>().Add(song);
            mockContext.SaveChanges();

            // Act
            var result = await controller.Edit(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<Song>(viewResult.Model);
            var model = viewResult.Model as Song;
            Assert.That(model.Id, Is.EqualTo(1));
        }


        [Test]
        public async Task Edit_Post_ReturnsNotFoundResult_WhenIdDoesNotMatchSongId()
        {
            // Arrange
            var song = new Song { Id = 1, Title = "Test Song", Artist = "Test Artist" };

            // Act
            var result = await controller.Edit(2, song);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var song = new Song { Id = 1, Title = "Updated Song", Artist = "Updated Artist" };

            // Act
            var result = await controller.Edit(1, song);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Edit_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            controller.ModelState.AddModelError("Error", "Model error");
            var song = new Song { Id = 1, Title = "Updated Song", Artist = "Updated Artist" };

            // Act
            var result = await controller.Edit(1, song);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }


        [Test]
        public async Task Delete_Get_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Act
            var result = await controller.Delete(null);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Get_ReturnsNotFoundResult_WhenSongDoesNotExist()
        {
            // Act
            var result = await controller.Delete(99);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Get_ReturnsViewResult_WithSong()
        {
            // Arrange
            var song = new Song { Id = 3, Title = "Test Song", Artist = "Test Artist" };
            mockContext.Set<Song>().Add(song);
            mockContext.SaveChanges();

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsInstanceOf<Song>(viewResult.Model);
            var model = viewResult.Model as Song;
            Assert.That(model.Id, Is.EqualTo(1));
        }


        [Test]
        public async Task DeleteConfirmed_ReturnsRedirectToActionResult()
        {
            // Arrange
            var song = new Song { Id = 3, Title = "Test Song", Artist = "Test Artist" };
            mockContext.Set<Song>().Add(song);
            mockContext.SaveChanges();

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
        }


        [Test]
        public async Task Index_ThrowsException()
        {
            // Arrange
            mockContext.Dispose();

            // Act & Assert
            Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Index());
        }

        [Test]
        public async Task Create_ThrowsException()
        {
            // Arrange
            mockContext.Dispose();
            var newSong = new Song { Title = "New Song", Artist = "New Artist" };

            // Act & Assert
            Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Create(newSong));
        }

        [Test]
        public void Controller_ThrowsException()
        {
            // Arrange
            mockContext.Dispose();
            var newSong = new Song { Id = 1, Title = "New Song", Artist = "New Artist" };

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Index());
                Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Create(newSong));
                Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Edit(1));
                Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Edit(1, newSong));
                Assert.ThrowsAsync<TargetInvocationException>(() => controller.Delete(1));
                Assert.ThrowsAsync<ObjectDisposedException>(() => controller.DeleteConfirmed(1));
                Assert.ThrowsAsync<TargetInvocationException>(() => controller.Details(1));
            });
        }

    }
}
