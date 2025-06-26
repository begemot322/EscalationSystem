namespace Models.Result;

public class Error
{
    public ErrorType ErrorType { get; }
    public string Code { get; }
    public string Description { get; }

    private Error(ErrorType errorType, string code, string description)
    {
        ErrorType = errorType;
        Code = code;
        Description = description;
    }
    
    public static Error ValidationFailed(string details)
        => new(ErrorType.Validation, "validation_failed", $"Validation error: {details}");

    public static Error NotFound<T>(int id)
        => new(ErrorType.NotFound, "not_found", $"{typeof(T).Name} with ID '{id}' not found");

    public static Error Duplicate<T>(int id)
        => new(ErrorType.Conflict, "duplicate", $"{typeof(T).Name} with ID '{id}' already exists");

    public static Error Unauthorized(string? message = null)
        => new(ErrorType.AccessUnAuthorized, "unauthorized", message ?? "Access denied");

    public static Error Forbidden(string? message = null)
        => new(ErrorType.AccessForbidden, "forbidden", message ?? "You do not have permission to perform this action");

    public static Error InternalServerError(string message)
        => new(ErrorType.Failure, "internal_error", $"Internal server error: {message}");
    
}