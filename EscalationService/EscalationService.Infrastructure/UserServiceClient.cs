using System.Net.Http.Json;
using EscalationService.Appliacation.Common.Interfaces;

namespace EscalationService.Infrastructure;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    
    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> CheckUsersExistAsync(List<int> userIds)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users/check-exists", userIds);
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}