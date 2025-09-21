using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Server.Models;
using Todo.Server.Services;

namespace Todo.Server.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly TodoService _todoService;

        public TodoServiceTests()
        {
            _todoService = new TodoService();
        }

        [Fact]
        public async Task GetAllTodosAsync_ShouldReturnInitialTodos()
        {
            // Act
            var todos = await _todoService.GetAllTodosAsync();

            // Assert
            Assert.NotNull(todos);
            Assert.Equal(3, todos.Count());
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldCreateNewTodo()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Test Todo" };

            // Act
            var todo = await _todoService.CreateTodoAsync(request);

            // Assert
            Assert.NotNull(todo);
            Assert.Equal("Test Todo", todo.Title);
            Assert.False(todo.IsCompleted);
            Assert.True(todo.Id > 0);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ShouldReturnCorrectTodo()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Test Todo" };
            var createdTodo = await _todoService.CreateTodoAsync(request);

            // Act
            var todo = await _todoService.GetTodoByIdAsync(createdTodo.Id);

            // Assert
            Assert.NotNull(todo);
            Assert.Equal(createdTodo.Id, todo.Id);
            Assert.Equal("Test Todo", todo.Title);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ShouldReturnNullForInvalidId()
        {
            // Act
            var todo = await _todoService.GetTodoByIdAsync(999);

            // Assert
            Assert.Null(todo);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldDeleteExistingTodo()
        {
            // Arrange
            var request = new CreateTodoRequest { Title = "Test Todo" };
            var createdTodo = await _todoService.CreateTodoAsync(request);

            // Act
            var result = await _todoService.DeleteTodoAsync(createdTodo.Id);

            // Assert
            Assert.True(result);

            var deletedTodo = await _todoService.GetTodoByIdAsync(createdTodo.Id);
            Assert.Null(deletedTodo);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldReturnFalseForInvalidId()
        {
            // Act
            var result = await _todoService.DeleteTodoAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
