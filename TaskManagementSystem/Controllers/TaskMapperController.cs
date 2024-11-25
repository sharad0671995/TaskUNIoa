using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskMapperController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        private readonly IMapper _mapper;

        // Inject TaskManagementContext and IMapper via constructor
        public TaskMapperController(TaskManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get all tasks with pagination
        [HttpGet]
        [Route("MapView")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetMap([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .AsNoTracking()
                .OrderBy(t => t.DueDate); // Sort by DueDate or any other field

            // Implement pagination
            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Get the list of tasks

            // Use AutoMapper to map the entities to DTOs
            var taskDtos = _mapper.Map<List<TaskDto>>(tasks);

            return Ok(taskDtos);
        }

        // Get a single task by id and map it to TaskDto
        [HttpGet]
        [Route("GetTasksMap/{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskMap(int id)
        {
            // Fetch the task from the database
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            // If the task is not found, return a 404
            if (task == null)
            {
                return NotFound();
            }

            // Use AutoMapper to map Task entity to TaskDto
            var taskDto = _mapper.Map<TaskDto>(task);

            return Ok(taskDto);
        }

        [HttpPost]
        [Route("CreateTaskMap")]
        public async Task<ActionResult<UserTask>> CreateTaskM([FromBody] TaskCreateDto taskCreateDto)
        {
            if (taskCreateDto == null)
            {
                return BadRequest("Task data is required.");
            }

            // Validation (using simple model validation)
            if (string.IsNullOrEmpty(taskCreateDto.Title) || string.IsNullOrEmpty(taskCreateDto.Description))
            {
                return BadRequest("Title and Description are required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // This will return the validation errors from Data Annotations
            }

            // Find users by their ID (validate CreatedBy and AssignedTo)
            var createdByUser = await _context.Users.FindAsync(taskCreateDto.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(taskCreateDto.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
            {
                return BadRequest("Invalid user(s).");
            }

            try
            {
                // Use AutoMapper to map the DTO to the UserTask entity
                var task = _mapper.Map<UserTask>(taskCreateDto);

                // Manually set navigation properties after mapping (AutoMapper does not handle these well)
                task.CreatedBy = createdByUser;
                task.AssignedTo = assignedToUser;

                // Optional: Validate that the task has the correct navigation properties
                if (task.CreatedBy == null || task.AssignedTo == null)
                {
                    return BadRequest("Task is missing CreatedBy or AssignedTo user.");
                }

                // Optional: Log the task to verify mapping
                Console.WriteLine($"Mapped Task: {task.Title}, {task.Description}, DueDate: {task.DueDate}, Assigned to: {task.AssignedTo.FullName}, Created by: {task.CreatedBy.FullName}");

                // Use the repository pattern to add the task
                MapperTaskRepo taskRepository = new MapperTaskRepo(_context);

                // Call the service to add the task
                var createdTask = await taskRepository.AddTaskAsync(task);

                // Return the newly created task with a status of 201 Created
                // return CreatedAtAction(nameof(GetTaskMap), new { id = createdTask.Id }, createdTask);

                return Ok(new { message = $"Task Create successfully." });
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors or invalid data
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateTask/{id}")]
        public async Task<ActionResult<UserTask>> UpdateTask(int id, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            // Step 1: Validate the incoming data
            if (taskUpdateDto == null)
            {
                return BadRequest("Task update data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if any
            }

            // Step 2: Retrieve the task from the database by id
            var existingTask = await _context.Tasks
                .Include(t => t.CreatedBy)  // Include necessary navigation properties
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTask == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }

            // Step 3: Use AutoMapper to map the TaskUpdateDto to the existing UserTask entity
            _mapper.Map(taskUpdateDto, existingTask);  // Map the properties from DTO to entity

            // Step 4: Update the navigation property if necessary (e.g., AssignedTo)
            var assignedUser = await _context.Users.FindAsync(taskUpdateDto.AssignedToId);
            if (assignedUser != null)
            {
                existingTask.AssignedTo = assignedUser;  // Update the AssignedTo user
            }
            else
            {
                return BadRequest("Assigned user not found.");
            }

            // Step 5: Save the changes to the database
            try
            {
                await _context.SaveChangesAsync();  // Save changes asynchronously

                // Step 6: Map the updated task to a DTO and return it
                var taskDto = _mapper.Map<UserTask>(existingTask);
                MapperTaskRepo taskRepository = new MapperTaskRepo(_context);
                await taskRepository.UpdateTaskAsync(taskDto);
                return Ok(new { message = $"Task Update successfully." });
                // return Ok(taskDto); // Return the updated task DTO
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., database issues)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
