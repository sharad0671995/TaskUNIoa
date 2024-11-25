using FluentValidation;
using TaskManagementSystem.Models;

using FluentValidation;
using TaskManagementSystem.DTO.User;

namespace TaskManagementSystem.Validation
{
    public class UserValidation: AbstractValidator<UserCreateDto>
    {
    public UserValidation()
        {
            RuleFor(user => user.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(1, 100).WithMessage("Full name cannot be longer than 100 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .Length(1, 200).WithMessage("Email cannot be longer than 200 characters.");
        }
    }

}
