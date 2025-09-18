using EscalationService.Appliacation.Models.DTOs.Criteria;
using FluentValidation;

namespace EscalationService.Appliacation.Validators;

public class UpdateCriteriaDtoValidator : AbstractValidator<UpdateCriteriaDto>
{
    public UpdateCriteriaDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(700).WithMessage("Description is too long");
        
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be positive");
    }
}