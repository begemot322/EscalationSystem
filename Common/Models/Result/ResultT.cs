namespace Models.Result;

public class Result<T> : Result
{
    public T? Data { get; }

    protected Result(T? data, bool isSuccess, Error? error) 
        : base(isSuccess, error)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(data, true, null);
    public new static Result<T> Failure(Error error) => new(default, false, error);
}