using EscalationService.Appliacation.Models.DTOs;
using FluentValidation;
using Models;

namespace EscalationService.Appliacation.Validators;

public class EscalationImportDtoValidator : AbstractValidator<EscalationImportDto>
{
    public EscalationImportDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must be less than 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description too long");

        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("Invalid status value");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("AuthorId must be greater than 0");

        RuleForEach(x => x.UserIds)
            .GreaterThan(0).WithMessage("Each UserId must be greater than 0");
    }

    private bool BeValidStatus(string status)
    {
        var normalized = NormalizeStatus(status);
        return Enum.TryParse<EscalationStatus>(normalized, true, out _);
    }

    public static string NormalizeStatus(string input)
    {
        return input.Trim().ToLower() switch
        {
            "new" or "новая" or "новый" => nameof(EscalationStatus.New),
            "inprogress" or "в работе" or "в_работе" => nameof(EscalationStatus.InProgress),
            "onreview" or "на проверке" or "на_проверке" => nameof(EscalationStatus.OnReview),
            "completed" or "завершена" or "закрыта" => nameof(EscalationStatus.Completed),
            "rejected" or "отклонена" or "отменена" => nameof(EscalationStatus.Rejected),
            _ => input
        };
    }
}