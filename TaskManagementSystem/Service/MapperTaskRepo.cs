using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Service
{
    public class MapperTaskRepo
    {

        private readonly TaskManagementContext _context;
        private readonly IMapper _mapper;
        public MapperTaskRepo(TaskManagementContext context)
        {
            _context = context;
            // _mapper = mapper;
        }


        public async Task<UserTask> AddTaskAsync1(TaskCreateDto taskCreateDto)
        {
            if (taskCreateDto == null)
                throw new ArgumentNullException(nameof(taskCreateDto));

            // Validation: Check if the createdByUser and assignedToUser are valid
            var createdByUser = await _context.Users.FindAsync(taskCreateDto.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(taskCreateDto.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
                throw new ArgumentException("Invalid user(s).");

            // Create UserTask from TaskCreateDto using AutoMapper (if using AutoMapper)
            //  var task = _mapper.Map<UserTask>(taskCreateDto);
            // task.Status = TaskStatus.NotStarted; // Default status
            // task.IsCompleted = false; // Default completion status

            // Use stored procedure to insert task
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Title", SqlDbType.NVarChar) { Value = taskCreateDto.Title },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = taskCreateDto.Description },
            new SqlParameter("@DueDate", SqlDbType.DateTime) { Value = taskCreateDto.DueDate },
            new SqlParameter("@CreatedById", SqlDbType.Int) { Value = taskCreateDto.CreatedById },
            new SqlParameter("@AssignedToId", SqlDbType.Int) { Value = taskCreateDto.AssignedToId },
            new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = taskCreateDto.IsCompleted }
        };

                // Execute the stored procedure to insert the task
                await _context.Database.ExecuteSqlRawAsync("EXEC AddTask @Title, @Description, @DueDate, @CreatedById, @AssignedToId, @IsCompleted", parameters);

                // Retrieve the task from the database after insertion (assuming the DB auto-generates the ID)
                var createdTask = await _context.Tasks
                    .Where(t => t.Title == taskCreateDto.Title && t.CreatedById == taskCreateDto.CreatedById)
                    .FirstOrDefaultAsync(); // Retrieve by other unique properties if necessary

                // If the task is successfully inserted, return the created task
                if (createdTask != null)
                {
                    // Map back the created task to DTO if needed or return entity
                    return createdTask;  // This will contain the ID and any other properties from the DB
                }

                throw new InvalidOperationException("Task creation failed or task not found after insertion.");
            }
            catch (Exception ex)
            {
                // Handle error (log it, rethrow it, or return a specific message)
                throw new InvalidOperationException("Error executing the stored procedure.", ex);
            }
        }





        public async Task<UserTask> AddTaskAsync(UserTask task)
        {
            // Validate the task object to avoid null exceptions
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "Task cannot be null.");
            }

            // Optionally, validate that CreatedBy and AssignedTo are not null
            if (task.CreatedBy == null || task.AssignedTo == null)
            {
                throw new ArgumentException("Both CreatedBy and AssignedTo must be valid.");
            }



            // Add the task entity to the context
            _context.Tasks.Add(task);

            // Save changes to the database asynchronously
            // await _context.SaveChangesAsync();
            try
            {
                var parameters = new[]
                {
         new SqlParameter("@Title", SqlDbType.NVarChar) { Value = task.Title },
         new SqlParameter("@Description", SqlDbType.NVarChar) { Value = task.Description },
         new SqlParameter("@DueDate", SqlDbType.DateTime) { Value = task.DueDate },
         new SqlParameter("@CreatedById", SqlDbType.Int) { Value = task.CreatedById },
     new SqlParameter("@AssignedToId", SqlDbType.Int) { Value = task.AssignedToId },
     new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = task.IsCompleted }
 };

                // Execute the stored procedure to insert the task
                await _context.Database.ExecuteSqlRawAsync("EXEC AddTask @Title, @Description, @DueDate, @CreatedById, @AssignedToId, @IsCompleted", parameters);

                // Retrieve the task from the database after insertion (assuming the DB auto-generates the ID)
                var createdTask = await _context.Tasks
                    .Where(t => t.Title == task.Title && t.CreatedById == task.CreatedById)
                    .FirstOrDefaultAsync();

                // Return the created task (which now includes an ID)
                return task;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw new InvalidOperationException("An error occurred while adding the task.", ex);
            }





        }


        public async Task<UserTask> UpdateTaskAsync(UserTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            // Validate the existence of the task
           

            // Validate if the user exists (createdByUser and assignedToUser)
            var createdByUser = await _context.Users.FindAsync(task.CreatedById);
            var assignedToUser = await _context.Users.FindAsync(task.AssignedToId);

            if (createdByUser == null || assignedToUser == null)
                throw new ArgumentException("Invalid user(s).");

            try
            {
                // Call stored procedure to update the task
                var parameters = new[]
                {
            new SqlParameter("@Id", SqlDbType.Int) { Value = task.Id },
            new SqlParameter("@Title", SqlDbType.NVarChar) { Value = task.Title },
            new SqlParameter("@Description", SqlDbType.NVarChar) { Value = task.Description },
            new SqlParameter("@DueDate", SqlDbType.DateTime) { Value = task.DueDate },
            new SqlParameter("@CreatedById", SqlDbType.Int) { Value = task.CreatedById },
            new SqlParameter("@AssignedToId", SqlDbType.Int) { Value = task.AssignedToId },
            new SqlParameter("@IsCompleted", SqlDbType.Bit) { Value = task.IsCompleted }
        };

                // Execute the stored procedure to update the task
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateTask @Id, @Title, @Description, @DueDate, @CreatedById, @AssignedToId, @IsCompleted", parameters);

                // Fetch the updated task from the database (assuming the task is updated in the DB)


                // Retrieve the task from the database after insertion (assuming the DB auto-generates the ID)
                var createdTask = await _context.Tasks
                    .Where(t => t.Title == task.Title && t.CreatedById == task.CreatedById)
                    .FirstOrDefaultAsync();

                // Return the created task (which now includes an ID)
                return task;
            }
            catch (Exception ex)
            {
                // Handle error and throw a custom exception for repository layers
                throw new InvalidOperationException("Error executing the stored procedure to update the task.", ex);
            }
        }

        
    }

}
