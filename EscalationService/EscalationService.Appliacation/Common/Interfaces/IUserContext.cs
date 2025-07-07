namespace EscalationService.Appliacation.Common.Interfaces;

public interface IUserContext
{
    int GetUserId();
    string? GetUserRole();
    bool IsAuthenticated();
}