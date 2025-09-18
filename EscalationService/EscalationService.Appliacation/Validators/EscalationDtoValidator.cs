using EscalationService.Appliacation.Models.DTOs;
using FluentValidation;

namespace EscalationService.Appliacation.Validators;

public class EscalationDtoValidator : AbstractValidator<EscalationDto>

{
    public EscalationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название обязательно")
            .MaximumLength(200).WithMessage("Максимальная длина названия - 200 символов");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Максимальная длина описания - 1000 символов");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Некорректный статус эскалации");

        RuleFor(x => x.ResponsibleUserIds)
            .NotEmpty().WithMessage("Не может быть ни одного ответственного");

    }
}