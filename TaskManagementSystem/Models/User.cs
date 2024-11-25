using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Full name must be between 1 and 100 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 200 characters.")]
        public string? Email { get; set; }


        public List<UserTask> Tasks { get; set; } = new List<UserTask>();

        // Navigation property for tasks assigned to this user
        public List<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
    }

}
