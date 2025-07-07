namespace EscalationService.Appliacation.Common.Interfaces;

public interface IUserServiceClient
{
    Task<bool> CheckUsersExistAsync(List<int> userIds);
}