namespace UserService.Domain;

public enum UserRole
{
    /// <summary>
    /// Может только комментировать
    /// </summary>
    Junior = 0,  
    /// <summary>
    /// Может создавать и закрывать свои эскалации
    /// </summary>
    Middle = 1,  
    /// <summary>
    /// Может все
    /// </summary>
    Senior = 2   
}