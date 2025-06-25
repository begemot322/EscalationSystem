namespace Exceptions;

public class ValidationException : Exception
{
    public ValidationException() :
        base() { }

    public ValidationException(string message) :
        base(message) { }

    public ValidationException(string fieldName, string issue)
        : base($"Validation failed for '{fieldName}': {issue}") { }
}