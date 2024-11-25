using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTO.User
{
    public class UserDto
    {
        [Key]
        public int Id { get; set; }
        //  public string? FullName { get; set; }  
        //  public string? Email { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Full name must be between 1 and 100 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 200 characters.")]
        public string? Email { get; set; }
        // A simplified list of tasks(either tasks created by the user or assigned to them)
        public List<TaskSummaryDto> Tasks { get; set; } = new List<TaskSummaryDto>();

        // A simplified list of tasks assigned to the user
        public List<TaskSummaryDto> AssignedTasks { get; set; } = new List<TaskSummaryDto>();
    }
}
