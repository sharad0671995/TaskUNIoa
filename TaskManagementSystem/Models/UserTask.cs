using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    public class UserTask
    {

        // public Guid Id { get; set; }
        [Key]
        public int Id { get; set; }  // Unique identifier for the task
        public string? Title { get; set; }  // Title of the task
        public string? Description { get; set; }  // Detailed description of the task
        public DateTime DueDate { get; set; }  // Due date of the task
        public bool IsCompleted { get; set; }  // Whether the task is completed or not

      
     //  public TaskStatus Status { get; set; } = TaskStatus.NotStarted;  // Default value is NotStarted
        // Foreign key to the user who created the task
        public int CreatedById { get; set; }
        public User?CreatedBy { get; set; }  // Navigation property to the creator

        // Foreign key to the user who the task is assigned to
        public int AssignedToId { get; set; }
        public User?AssignedTo { get; set; }
     
    }

    public enum TaskStatus
    {
        NotStarted,   // Task is not started yet
        InProgress,   // Task is currently being worked on
        Completed,    // Task is completed
        OnHold,       // Task is paused or put on hold
        Cancelled     // Task has been cancelled
    }



}
