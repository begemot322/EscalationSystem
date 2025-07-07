namespace Models;

public enum EscalationStatus
{
    /// <summary>
    /// Новая
    /// </summary>
    New,
    /// <summary>
    /// в работе
    /// </summary>
    InProgress,
    /// <summary>
    /// // На проверке
    /// </summary>
    OnReview,  
    /// <summary>
    /// Завершена
    /// </summary>
    Completed, 
    /// <summary>
    /// отклонена
    /// </summary>
    Rejected    
}