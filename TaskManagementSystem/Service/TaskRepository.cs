using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Data;
using System.Runtime.ExceptionServices;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class TaskRepository
    {
        private readonly TaskManagementContext _context;
        private readonly IMapper _mapper;
        public TaskRepository(TaskManagementContext context)
        {
            _context = context;
            //_mapper = mapper;
        }


        

      /*  public async Task<List<UserTask>> GetTaskListAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            try
            {
                // Define SQL parameters for the stored procedure (if needed)
                var parameters = new[]
                {
            new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
        };

                // Execute stored procedure to get tasks for the specific user
                var tasks = await _context.Tasks
                    .FromSqlRaw("EXEC GetTaskList @UserId", parameters)
                    .ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                // Handle error (log it, rethrow it, or return a specific message)
                throw new InvalidOperationException("Error executing the stored procedure to fetch tasks.", ex);
            }
        }

        */
        public async Task<UserTask> AddTaskAsync(TaskCreateDto taskCreateDto)
        {
            if (taskCreateDto == null)
                throw new ArgumentNullException(nameof(taskCreateDto));

            // Validation: Check if the createdByUser and assignedToUser are valid
            var createdByUser = await _context.Users.FindAsync(taskCreateDto.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(taskCreateDto.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
                throw new ArgumentException("Invalid user(s).");

            // Call stored procedure to insert task
            try
            {
                // Use raw SQL to execute the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@Title", SqlDbType.NVarChar) { Value = taskCreateDto.Title },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = taskCreateDto.Description },
            new SqlParameter("@DueDate", SqlDbType.DateTime) { Value = taskCreateDto.DueDate },
            new SqlParameter("@CreatedById", SqlDbType.Int) { Value = taskCreateDto.CreatedById },
            new SqlParameter("@AssignedToId", SqlDbType.Int) { Value = taskCreateDto.AssignedToId },
            new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = taskCreateDto.IsCompleted }
        };

                // Execute the stored procedure
                await _context.Database.ExecuteSqlRawAsync("EXEC AddTask @Title, @Description, @DueDate, @CreatedById, @AssignedToId, @IsCompleted", parameters);

                // Since stored procedures may not directly return the newly inserted entity,
                // we need to retrieve the task entity (we can either return null or fetch it again)
                // Here, I will return null, but you may choose to fetch the task after insertion.
                var task = new UserTask
                {
                    Title = taskCreateDto.Title,
                    Description = taskCreateDto.Description,
                    DueDate = taskCreateDto.DueDate,
                    CreatedById = taskCreateDto.CreatedById,
                    AssignedToId = taskCreateDto.AssignedToId,
                    IsCompleted = taskCreateDto.IsCompleted
                };

                return task; // You can choose to return the newly created task (without ID initially) or just a confirmation.
            }
            catch (Exception ex)
            {
                // Handle error (log it, rethrow it, or return a specific message)
                throw new InvalidOperationException("Error executing the stored procedure.", ex);
            }
        }

        
        public async Task<UserTask> UpdateTaskAsync(int id, TaskCreateDto taskUpdateDto)
        {
            if (taskUpdateDto == null)
                throw new ArgumentNullException(nameof(taskUpdateDto));

            // Validate the existence of the task
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            // Validate if the user exists (createdByUser and assignedToUser)
            var createdByUser = await _context.Users.FindAsync(taskUpdateDto.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(taskUpdateDto.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
                throw new ArgumentException("Invalid user(s).");

            try
            {
                // Call stored procedure to update the task
                var parameters = new[]
                {
            new SqlParameter("@Id", SqlDbType.Int) { Value = id },
            new SqlParameter("@Title", SqlDbType.NVarChar) { Value = taskUpdateDto.Title },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = taskUpdateDto.Description },
            new SqlParameter("@DueDate", SqlDbType.DateTime) { Value = taskUpdateDto.DueDate },
            new SqlParameter("@CreatedById", SqlDbType.Int) { Value = taskUpdateDto.CreatedById },
            new SqlParameter("@AssignedToId", SqlDbType.Int) { Value = taskUpdateDto.AssignedToId },
            new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = taskUpdateDto.IsCompleted }
        };

                // Execute the stored procedure to update the task
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateTask @Id, @Title, @Description, @DueDate, @CreatedById, @AssignedToId, @IsCompleted", parameters);

                // Fetch the updated task from the database (assuming the task is updated in the DB)
                var updatedTask = await _context.Tasks
                    .Where(t => t.Id == id)
                    .Select(t => new UserTask
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        CreatedById = t.CreatedById,
                        AssignedToId = t.AssignedToId,
                        IsCompleted = t.IsCompleted
                    })
                    .FirstOrDefaultAsync();

                if (updatedTask == null)
                    throw new KeyNotFoundException($"Task with ID {id} not found after update.");

                return updatedTask; // Return the updated task
            }
            catch (Exception ex)
            {
                // Handle error and throw a custom exception for repository layers
                throw new InvalidOperationException("Error executing the stored procedure to update the task.", ex);
            }
        }



        public async Task<bool> DeleteTaskAsync(int id)
        {
            // Validate if the task exists
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");

            try
            {
                // Call the stored procedure to delete the task
                var parameters = new[]
                {
            new SqlParameter("@Id", SqlDbType.Int) { Value = id }
        };

                // Execute the stored procedure to delete the task
                var result = await _context.Database.ExecuteSqlRawAsync("EXEC DeleteTask @Id", parameters);

                // Check if the result was successful (typically, you'd expect 1 row affected on success)
                if (result == 0)
                    throw new InvalidOperationException($"Failed to delete task with ID {id}.");

                return true; // Return true to indicate successful deletion
            }
            catch (Exception ex)
            {
                // Handle error and throw a custom exception for repository layers
                throw new InvalidOperationException("Error executing the stored procedure to delete the task.", ex);
            }
        }

       
    }
}

