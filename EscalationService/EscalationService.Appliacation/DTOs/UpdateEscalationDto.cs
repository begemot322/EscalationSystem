using Models;

namespace EscalationService.Appliacation.DTOs;

public record UpdateEscalationDto(
    string Name,
    string Description,
    EscalationStatus Status);