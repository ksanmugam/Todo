using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Server.Controllers;
using Todo.Server.Models;
using Todo.Server.Services;

namespace Todo.Server.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly TodosController _controller;

        public TodoControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _controller = new TodosController(_mockTodoService.Object);
        }

        [Fact]
        public async Task GetTodos_ReturnsOkWithTodos_WhenTodosExist()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Test Todo 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TodoItem { Id = 2, Title = "Test Todo 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };
            _mockTodoService.Setup(s => s.GetAllTodosAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualTodos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Equal(expectedTodos, actualTodos);
            _mockTodoService.Verify(s => s.GetAllTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodos_ReturnsOkWithEmptyList_WhenNoTodosExist()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>();
            _mockTodoService.Setup(s => s.GetAllTodosAsync())
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualTodos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Empty(actualTodos);
            _mockTodoService.Verify(s => s.GetAllTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodos_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            _mockTodoService.Setup(s => s.GetAllTodosAsync())
                .ThrowsAsync(new InvalidOperationException("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetTodos());
            _mockTodoService.Verify(s => s.GetAllTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodo_ReturnsOkWithTodo_WhenTodoExists()
        {
            // Arrange
            var todoId = 1;
            var expectedTodo = new TodoItem
            {
                Id = todoId,
                Title = "Test Todo",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(todoId))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _controller.GetTodo(todoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualTodo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(expectedTodo, actualTodo);
            _mockTodoService.Verify(s => s.GetTodoByIdAsync(todoId), Times.Once);
        }

        [Fact]
        public async Task GetTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var todoId = 999;
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(todoId))
                .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.GetTodo(todoId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _mockTodoService.Verify(s => s.GetTodoByIdAsync(todoId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task GetTodo_ReturnsNotFound_WhenIdIsInvalid(int invalidId)
        {
            // Arrange
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(invalidId))
                .ReturnsAsync((TodoItem?)null);

            // Act
            var result = await _controller.GetTodo(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _mockTodoService.Verify(s => s.GetTodoByIdAsync(invalidId), Times.Once);
        }

        [Fact]
        public async Task GetTodo_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            var todoId = 1;
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(todoId))
                .ThrowsAsync(new InvalidOperationException("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetTodo(todoId));
            _mockTodoService.Verify(s => s.GetTodoByIdAsync(todoId), Times.Once);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "New Todo" };
            var expectedTodo = new TodoItem
            {
                Id = 1,
                Title = "New Todo",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            _mockTodoService.Setup(s => s.CreateTodoAsync(request))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TodosController.GetTodo), createdResult.ActionName);
            Assert.Equal(expectedTodo.Id, createdResult.RouteValues?["id"]);

            var actualTodo = Assert.IsType<TodoItem>(createdResult.Value);
            Assert.Equal(expectedTodo, actualTodo);

            _mockTodoService.Verify(s => s.CreateTodoAsync(request), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public async Task CreateTodo_ReturnsBadRequest_WhenTitleIsInvalid(string? invalidTitle)
        {
            // Arrange
            var request = new CreateTodoRequest { Title = invalidTitle! };

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Title is required", badRequestResult.Value);

            _mockTodoService.Verify(s => s.CreateTodoAsync(It.IsAny<CreateTodoRequest>()), Times.Never);
        }

        [Fact]
        public async Task CreateTodo_AcceptsTitleWithValidContent()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Valid Todo with spaces" };
            var expectedTodo = new TodoItem
            {
                Id = 1,
                Title = "Valid Todo with spaces",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            _mockTodoService.Setup(s => s.CreateTodoAsync(request))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _controller.CreateTodo(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            _mockTodoService.Verify(s => s.CreateTodoAsync(request), Times.Once);
        }

        [Fact]
        public async Task CreateTodo_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Test Todo" };
            _mockTodoService.Setup(s => s.CreateTodoAsync(request))
                .ThrowsAsync(new InvalidOperationException("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CreateTodo(request));
            _mockTodoService.Verify(s => s.CreateTodoAsync(request), Times.Once);
        }

        [Fact]
        public async Task DeleteTodo_ReturnsNoContent_WhenTodoIsDeletedSuccessfully()
        {
            // Arrange
            var todoId = 1;
            _mockTodoService.Setup(s => s.DeleteTodoAsync(todoId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTodo(todoId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockTodoService.Verify(s => s.DeleteTodoAsync(todoId), Times.Once);
        }

        [Fact]
        public async Task DeleteTodo_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            var todoId = 999;
            _mockTodoService.Setup(s => s.DeleteTodoAsync(todoId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTodo(todoId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockTodoService.Verify(s => s.DeleteTodoAsync(todoId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task DeleteTodo_ReturnsNotFound_WhenIdIsInvalid(int invalidId)
        {
            // Arrange
            _mockTodoService.Setup(s => s.DeleteTodoAsync(invalidId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTodo(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockTodoService.Verify(s => s.DeleteTodoAsync(invalidId), Times.Once);
        }

        [Fact]
        public async Task DeleteTodo_ThrowsException_WhenServiceThrowsException()
        {
            // Arrange
            var todoId = 1;
            _mockTodoService.Setup(s => s.DeleteTodoAsync(todoId))
                .ThrowsAsync(new InvalidOperationException("Service error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.DeleteTodo(todoId));
            _mockTodoService.Verify(s => s.DeleteTodoAsync(todoId), Times.Once);
        }

        [Fact]
        public async Task TodoWorkflow_CreateAndRetrieveTodo_WorksCorrectly()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Workflow Test Todo" };
            var createdTodo = new TodoItem
            {
                Id = 1,
                Title = "Workflow Test Todo",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockTodoService.Setup(s => s.CreateTodoAsync(request))
                .ReturnsAsync(createdTodo);
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(createdTodo.Id))
                .ReturnsAsync(createdTodo);

            // Act - Create
            var createResult = await _controller.CreateTodo(request);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(createResult.Result);
            var returnedTodo = Assert.IsType<TodoItem>(createdAtResult.Value);

            // Act - Retrieve
            var getResult = await _controller.GetTodo(returnedTodo.Id);
            var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
            var retrievedTodo = Assert.IsType<TodoItem>(okResult.Value);

            // Assert
            Assert.Equal(createdTodo.Id, retrievedTodo.Id);
            Assert.Equal(createdTodo.Title, retrievedTodo.Title);
            Assert.Equal(createdTodo.IsCompleted, retrievedTodo.IsCompleted);
        }

        [Fact]
        public async Task TodoWorkflow_CreateAndDeleteTodo_WorksCorrectly()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Delete Test Todo" };
            var createdTodo = new TodoItem
            {
                Id = 1,
                Title = "Delete Test Todo",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockTodoService.Setup(s => s.CreateTodoAsync(request))
                .ReturnsAsync(createdTodo);
            _mockTodoService.Setup(s => s.DeleteTodoAsync(createdTodo.Id))
                .ReturnsAsync(true);

            // Act - Create
            var createResult = await _controller.CreateTodo(request);
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(createResult.Result);
            var returnedTodo = Assert.IsType<TodoItem>(createdAtResult.Value);

            // Act - Delete
            var deleteResult = await _controller.DeleteTodo(returnedTodo.Id);

            // Assert
            Assert.IsType<NoContentResult>(deleteResult);
            _mockTodoService.Verify(s => s.CreateTodoAsync(request), Times.Once);
            _mockTodoService.Verify(s => s.DeleteTodoAsync(returnedTodo.Id), Times.Once);
        }
    }
}
