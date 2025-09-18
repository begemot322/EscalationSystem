namespace EscalationService.Appliacation.Common.Interfaces;

public interface IRedisCacheService
{
    Task<T?> GetDataAsync<T>(string key, CancellationToken token = default);
    Task SetDataAsync<T>(string key, T data, TimeSpan? expiration = null, CancellationToken token = default);
    Task RemoveDataAsync(string key, CancellationToken token = default);

}