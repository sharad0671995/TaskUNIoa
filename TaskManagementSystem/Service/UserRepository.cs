using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTO.User;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class UserRepository
    {

        private readonly TaskManagementContext _context;

        public UserRepository(TaskManagementContext context)
        {
            _context = context;
        }
        public async Task<UserDto> GetUserDtoAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Tasks)  // Include the tasks created by the user
                .Include(u => u.AssignedTasks)  // Include the tasks assigned to the user
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Tasks = user.Tasks.Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                }).ToList(),
                AssignedTasks = user.AssignedTasks.Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                }).ToList()
            };

            return userDto;
        }

        // Method to get tasks for a specific user
        public async Task<List<TaskSummaryDto>> GetUserTasksAsync(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.CreatedById == userId || t.AssignedToId == userId)
                .Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted
                })
                .ToListAsync();

            return tasks;
        }


        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
                throw new ArgumentNullException(nameof(userCreateDto));

            try
            {
                // Create parameters for the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = userCreateDto.FullName },
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = userCreateDto.Email }
        };

                // Call the stored procedure to create a user
                var userId = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.CreateUser @FullName, @Email", parameters);

                // Fetch the newly created user
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new KeyNotFoundException("User not found after creation.");

                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error creating the user.", ex);
            }
        }



        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto)
        {
            if (userUpdateDto == null)
                throw new ArgumentNullException(nameof(userUpdateDto));

            try
            {
                // Create parameters for the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@UserId", SqlDbType.Int) { Value = userId },
            new SqlParameter("@FullName", SqlDbType.NVarChar) { Value = userUpdateDto.FullName ?? (object)DBNull.Value }, // Nullability handled
            new SqlParameter("@Email", SqlDbType.NVarChar) { Value = userUpdateDto.Email ?? (object)DBNull.Value } // Nullability handled
        };

                // Execute the stored procedure to update the user
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.UpdateUser @UserId, @FullName, @Email", parameters);

                // Check if the result was successful (assuming the stored procedure returns an integer result)
                if (result == 0)
                    throw new KeyNotFoundException($"User with ID {userId} not found or not updated.");

                // Fetch the updated user
                var updatedUser = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email
                    })
                    .FirstOrDefaultAsync();

                if (updatedUser == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found after update.");

                return updatedUser;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error updating the user.", ex);
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            // Validate if the user exists
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            try
            {
                // Call the stored procedure to delete the task
                var parameters = new[]
                {
            new SqlParameter("@Id", SqlDbType.Int) { Value = id }
        };

                // Execute the stored procedure to delete the task
                var result = await _context.Database.ExecuteSqlRawAsync("EXEC DeleteUser @Id", parameters);

                // Check if the result was successful (typically, you'd expect 1 row affected on success)
                if (result == 0)
                    throw new InvalidOperationException($"Failed to delete User with ID {id}.");

                return true; // Return true to indicate successful deletion
            }
            catch (Exception ex)
            {
                // Handle error and throw a custom exception for repository layers
                throw new InvalidOperationException("Error executing the stored procedure to delete the User.", ex);
            }
        }


    }
}
