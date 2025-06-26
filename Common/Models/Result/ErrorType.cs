namespace Models.Result;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    AccessUnAuthorized,
    AccessForbidden,
    Failure
}