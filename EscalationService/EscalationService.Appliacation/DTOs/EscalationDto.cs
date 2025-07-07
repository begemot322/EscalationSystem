using Models;

namespace EscalationService.Appliacation.DTOs;

public record EscalationDto(
    string Name,
    string Description,
    List<int> ResponsibleUserIds,
    EscalationStatus Status = EscalationStatus.New);