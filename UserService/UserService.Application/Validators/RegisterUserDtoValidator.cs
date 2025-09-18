using FluentValidation;
using UserService.Application.Models.DTOs;

namespace UserService.Application.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("First name must be between 1 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name must be between 1 and 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .WithMessage("Valid email is required (max 100 characters)");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .WithMessage("Password must be 8-100 characters long");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-\(\)]{10,20}$")
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}