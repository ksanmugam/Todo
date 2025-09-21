
using Todo.Server.Models;

namespace Todo.Server.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTodosAsync();
        Task<TodoItem?> GetTodoByIdAsync(int id);
        Task<TodoItem> CreateTodoAsync(CreateTodoRequest request);
        Task<bool> DeleteTodoAsync(int id);
    }

    public class TodoService : ITodoService
    {
        private readonly List<TodoItem> _todos = new();
        private int _nextId = 1;

        public TodoService()
        {
            // Seed with some initial data
            _todos.AddRange(new[]
            {
            new TodoItem { Id = _nextId++, Title = "Learn Angular", IsCompleted = false },
            new TodoItem { Id = _nextId++, Title = "Build TODO app", IsCompleted = false },
            new TodoItem { Id = _nextId++, Title = "Deploy to production", IsCompleted = false }
        });
        }

        public Task<IEnumerable<TodoItem>> GetAllTodosAsync()
        {
            return Task.FromResult(_todos.AsEnumerable());
        }

        public Task<TodoItem?> GetTodoByIdAsync(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(todo);
        }

        public Task<TodoItem> CreateTodoAsync(CreateTodoRequest request)
        {
            var todo = new TodoItem
            {
                Id = _nextId++,
                Title = request.Title,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _todos.Add(todo);
            return Task.FromResult(todo);
        }

        public Task<bool> DeleteTodoAsync(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return Task.FromResult(false);
            }

            _todos.Remove(todo);
            return Task.FromResult(true);
        }
    }
}
