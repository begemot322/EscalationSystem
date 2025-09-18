namespace EscalationService.Appliacation.Models.Constants;

public class CacheConstants
{
    public const string FEATURED_ESCALATIONS_KEY = "featured_escalations";

    public static readonly TimeSpan FEATURED_CACHE_EXPIRATION = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan DEFAULT_CACHE_EXPIRATION = TimeSpan.FromMinutes(15);
}