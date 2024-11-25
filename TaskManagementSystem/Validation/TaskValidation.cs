using FluentValidation;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.DTO.User;

namespace TaskManagementSystem.Validation
{
    public class TaskValidation: AbstractValidator<TaskCreateDto>
    {
    

        public TaskValidation()
        {
            // Validate Title
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters.");

            // Validate Description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(10, 500).WithMessage("Description must be between 10 and 500 characters.");
        }
    }
}
