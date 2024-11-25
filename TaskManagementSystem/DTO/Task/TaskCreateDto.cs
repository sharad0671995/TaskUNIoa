using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTO.Task
{
    public class TaskCreateDto
    {
        // public string? Title { get; set; }
        //  public string? Description { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; }
        public int AssignedToId { get; set; }
        public bool IsCompleted { get; set; }
    }

}
