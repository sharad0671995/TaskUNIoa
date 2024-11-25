using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTO;
using TaskManagementSystem.DTO.User;
using TaskManagementSystem.Service;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        
        public UserController(TaskManagementContext context)
        {
            _context=context;

        }
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {

                UserRepository userRepository = new UserRepository(_context);
                var userDto = await userRepository.GetUserDtoAsync(id);

                if (userDto == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(userDto);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle not found exception
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // GET: api/users/{id}/tasks
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<List<TaskSummaryDto>>> GetUserTasks(int id)
        {
            try
            {
                UserRepository userRepository = new UserRepository(_context);
                var tasks = await userRepository.GetUserTasksAsync(id);

                if (tasks == null || tasks.Count == 0)
                {
                    return NotFound($"No tasks found for user with ID {id}.");
                }

                return Ok(tasks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest("User data cannot be null.");
            }

            // Validate the incoming DTO using Data Annotations or FluentValidation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Returns all validation errors
            }

            try
            {

                var existingUser = await _context.Users
           .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

                if (existingUser != null)
                {
                    return BadRequest("Email is already in use.");
                }


                UserRepository userRepository = new UserRepository(_context);
                // Call repository method to create the user via stored procedure
                var createdUser = await userRepository.CreateUserAsync(userCreateDto);

                // Return the created user DTO
                // return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
                return Ok(new { message = $"User Create successfully." });
            }
            catch (ArgumentException ex)
            {
                // If the email is already taken, return a bad request with the error message
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // For general errors, return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet]
        [Route("UserTask")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
            }

            try
            {
                // Calculate the number of users to skip for pagination
                var skip = (page - 1) * pageSize;

                // Get the total count of users for pagination metadata
                var totalUsersCount = await _context.Users.CountAsync();

                // Fetch users with pagination applied
                var users = await _context.Users
                    .Skip(skip)           // Skip users for pagination
                    .Take(pageSize)       // Take a limited number of users
                    .Select(u => new UserDto   // Project to UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        
                    })
                    .ToListAsync();

                if (users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                // Calculate total pages based on total users count
                var totalPages = (int)Math.Ceiling((double)totalUsersCount / pageSize);

                // Return users with pagination metadata
                var result = new
                {
                    Users = users,
                    TotalUsers = totalUsersCount,
                    TotalPages = totalPages,
                    CurrentPage = page
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Return 500 if there's any other error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPut("users/{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto == null)
            {
                return BadRequest("User data cannot be null.");
            }

            try
            {
                UserRepository userRepository = new UserRepository(_context);
                // Call the service to update the user
                var updatedUser = await userRepository.UpdateUserAsync(id, userUpdateDto);

                if (updatedUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Return the updated user data
                // return Ok(updatedUser);
                return Ok(new { message = $"User Update successfully." });
            }
            catch (ArgumentException ex)
            {
                // If there's a validation error (e.g., email already taken)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // General error handling
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // DELETE: api/Users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var UsersD = await _context.Users.FindAsync(id);
            UserRepository userRepository = new UserRepository(_context);
            if (UsersD == null)
            {
                return NotFound();
            }

            _context.Users.Remove(UsersD);
            await userRepository.DeleteTaskAsync(id);

            return Ok(new { message = $"User with ID {id} has been successfully deleted." });
        }


    }
}
