using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthMvc.Controllers;
using AuthMvc.Data;
using AuthMvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AuthMvc.Tests
{
    public class TaskItemsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_TaskItems")
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfTaskItems()
        {
            using var context = GetInMemoryDbContext();
            context.TaskItem.Add(new TaskItem { Id = 1, Titre = "Test Task", Description = "desc", OwnerId = "owner1", Owner = null! });
            context.SaveChanges();

            var controller = new TaskItemsController(context);
            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenTaskItemNotFound()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);
            var taskItem = new TaskItem
            {
                Titre = "New Task",
                Description = "Description",
                Statut = StatutTache.EnCours,
                Priorite = PrioriteTache.Moyenne,
                OwnerId = "owner1",
                Owner = null!
            };

            var result = await controller.Create(taskItem);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Single(context.TaskItem);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);
            controller.ModelState.AddModelError("Titre", "Required");
            var taskItem = new TaskItem
            {
                Titre = string.Empty,
                Description = string.Empty,
                OwnerId = string.Empty,
                Owner = null!
            };

            var result = await controller.Create(taskItem);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(taskItem, viewResult.Model);
        }

        [Fact]
        public async Task UploadFile_ValidFile_RedirectsToDetails()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);
            var taskItem = new TaskItem { Id = 1, Titre = "Task", Description = "desc", OwnerId = "owner1", Owner = null! };
            context.TaskItem.Add(taskItem);
            context.SaveChanges();

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a fake file";
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var result = await controller.UploadFile(taskItem.Id, fileMock.Object);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal(taskItem.Id, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task UploadFile_NullFile_ReturnsBadRequest()
        {
            using var context = GetInMemoryDbContext();
            var controller = new TaskItemsController(context);

            var result = await controller.UploadFile(1, null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier sélectionné.", badRequestResult.Value);
        }
    }
}
