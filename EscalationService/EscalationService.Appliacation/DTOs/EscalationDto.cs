using EscalationService.Domain.Enums;

namespace EscalationService.Appliacation.DTOs;

public record EscalationDto(
    string Name,
    string Description,
    int AuthorId,         
    EscalationStatus Status = EscalationStatus.New);