using System.Text.Json;
using Models;
using Models.DTOs;
using NotificationService.API.Contracts;

namespace NotificationService.API.Services;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<UserDto>> GetUsersByIdsAsync(List<int> userIds)
    {
        var response = await _httpClient.PostAsJsonAsync("/internal/users/by-ids", userIds);
        return await response.Content.ReadFromJsonAsync<List<UserDto>>();
    }
}