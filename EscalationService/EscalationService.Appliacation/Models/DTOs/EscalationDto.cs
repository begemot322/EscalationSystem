using Models;

namespace EscalationService.Appliacation.Models.DTOs;

public record EscalationDto(
    string Name,
    string Description,
    List<int> ResponsibleUserIds,
    EscalationStatus Status = EscalationStatus.New);