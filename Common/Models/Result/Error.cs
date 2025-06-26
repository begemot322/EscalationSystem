namespace Models.Result;

public class Error
{
    public int StatusCode { get; }
    public string Code { get; }
    public string Message { get; }
    
    private Error(int statusCode, string code, string message)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
    }
    
    public static Error ValidationFailed(string details) 
        => new(400, "validation_failed", $"Validation error: {details}");

    public static Error NotFound<T>(int id) 
        => new(404, "not_found", $"{typeof(T).Name} with ID '{id}' not found");

    public static Error Duplicate<T>(int id) 
        => new(409, "duplicate", $"{typeof(T).Name} with ID '{id}' already exists");

    public static Error Unauthorized() 
        => new(401, "unauthorized", "Access denied");
    
    public static Error InternalServerError(string message)
        => new(500, "internal_error", $"Internal server error: {message}");
}