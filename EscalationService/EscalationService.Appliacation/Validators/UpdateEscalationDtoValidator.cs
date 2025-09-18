using EscalationService.Appliacation.Models.DTOs;
using FluentValidation;

namespace EscalationService.Appliacation.Validators;

public class UpdateEscalationDtoValidator : AbstractValidator<UpdateEscalationDto>
{
    public UpdateEscalationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");
        
    }
}