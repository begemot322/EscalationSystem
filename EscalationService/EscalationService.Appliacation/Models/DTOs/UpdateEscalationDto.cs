using Models;

namespace EscalationService.Appliacation.Models.DTOs;

public record UpdateEscalationDto(
    string Name,
    string Description,
    EscalationStatus Status,
    bool IsFeatured);