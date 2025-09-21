using Microsoft.AspNetCore.Mvc;
using Todo.Server.Models;
using Todo.Server.Services;

namespace Todo.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// Get all TODO items
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        /// <summary>
        /// Get a specific TODO item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        /// <summary>
        /// Create a new TODO item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodo(CreateTodoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }

            var todo = await _todoService.CreateTodoAsync(request);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }

        /// <summary>
        /// Delete a TODO item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var success = await _todoService.DeleteTodoAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
