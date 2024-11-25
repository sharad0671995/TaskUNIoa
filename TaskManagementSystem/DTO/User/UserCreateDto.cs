using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTO.User
{
    public class UserCreateDto
    {

        //  public string? FullName { get; set; }  
        //  public string? Email { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Full name must be between 1 and 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 200 characters.")]
        public string Email { get; set; }
    }
}
