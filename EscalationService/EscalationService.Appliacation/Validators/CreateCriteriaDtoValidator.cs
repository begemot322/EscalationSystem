using EscalationService.Appliacation.Models.DTOs.Criteria;
using FluentValidation;

namespace EscalationService.Appliacation.Validators;

public class CreateCriteriaDtoValidator : AbstractValidator<CreateCriteriaDto>
{
    public CreateCriteriaDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(700).WithMessage("Description is too long");
    }
}