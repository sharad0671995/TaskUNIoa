
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagementSystem.Models;

using TaskManagementSystem.Context;
using TaskDto = TaskManagementSystem.DTO.Task.TaskDto;
using TaskManagementSystem.Service;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.DTO;
using AutoMapper;

namespace TaskManagementSystem.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        
        public TasksController(TaskManagementContext context)
        {
            _context = context;
           // _mapper = mapper;
        }



      /*  // GET: api/Tasks
        [HttpGet]
        [Route("ViewTask1")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks1([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                 .Select(t => new TaskDto
                 {
                     Id = t.Id,
                     Title = t.Title,
                     Description = t.Description,
                     DueDate = t.DueDate,
                     IsCompleted = t.IsCompleted,
                     CreatedByName = t.CreatedBy.FullName,
                     AssignedToName = t.AssignedTo.FullName,
                     Status = (System.Threading.Tasks.TaskStatus)t.Status  // Include Status in DTO mapping
                 })
                 .ToListAsync();

             return Ok(tasks);
         }
        
        */

        // GET: api/Tasks
           [HttpGet]
           [Route("ViewTask")]
           public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
                   .Select(t => new TaskDto
                   {
                       Id = t.Id,
                       Title = t.Title,
                       Description = t.Description,
                       DueDate = t.DueDate,
                       IsCompleted = t.IsCompleted,
                       CreatedByName = t.CreatedBy.FullName,
                       AssignedToName = t.AssignedTo.FullName
                   })
                   .ToListAsync();

               return Ok(tasks);
           }


       




    // GET: api/Tasks/{id}
    //[HttpGet("tasks/{id}")]
    [HttpGet]
        [Route("GetTasks/{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.Id == id)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    CreatedByName = t.CreatedBy.FullName,
                    AssignedToName = t.AssignedTo.FullName
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        
        

        [HttpPost]
        [Route("CreateTask")]
        public async Task<ActionResult<UserTask>> CreateTask([FromBody] TaskCreateDto taskCreateDto)
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
                // Return a BadRequest with the validation errors
                return BadRequest(ModelState);  // This will return the validation errors from Data Annotations
            }

            var createdByUser = await _context.Users.FindAsync(taskCreateDto.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(taskCreateDto.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
            {
                return BadRequest("Invalid user(s).");
            }

            try
            {
               TaskRepository taskRepository = new TaskRepository(_context);
               // TaskRepository taskRepository =new ( _context);
               // TaskRepository taskRepository = _context;

                // Call the service to add a task
               // var createdTask = await taskRepository.AddTaskAsync1(taskCreateDto);

                // Return the newly created task with a status of 201 Created
                //return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
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
        [Route("EditTask/{id}")]
        public async Task<ActionResult<UserTask>> UpdateTask(int id, TaskCreateDto taskUpdateDto)
        {
            if (taskUpdateDto == null)
            {
                return BadRequest("Task update data cannot be null.");
            }

            try
            {
                TaskRepository taskRepository = new TaskRepository(_context);
                // Call the repository method to update the task
                var updatedTask = await taskRepository.UpdateTaskAsync(id, taskUpdateDto);

                // Return the updated task with a 200 OK status
                //return Ok(updatedTask);
                return Ok(new { message = $"Task with ID {id} has been Update successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                // Task not found, return 404
                return NotFound($"Task with ID {id} not found: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                // Invalid user(s), return 400
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Other errors (e.g., database issues), return 500
                return StatusCode(500, $"An error occurred while updating the task: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }



        // DELETE: api/Tasks/{id}
        // [HttpDelete("tasks/{id}")]
        [HttpDelete]
        [Route("TaskDelete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            TaskRepository taskRepository = new TaskRepository(_context);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await taskRepository.DeleteTaskAsync(id);

           return  Ok(new { message = $"Task with ID {id} has been successfully deleted." });
        }
        
    }

}
