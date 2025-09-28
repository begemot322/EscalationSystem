namespace UserService.Application.Common.Interfaces;

public interface IUserContext
{
    int GetUserId();
    string GetUserRole();
    bool IsAuthenticated();
}